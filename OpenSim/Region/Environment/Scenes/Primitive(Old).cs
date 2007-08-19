using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using Axiom.Math;
using libsecondlife;
using libsecondlife.Packets;
using OpenSim.Framework.Interfaces;
using OpenSim.Framework.Inventory;
using OpenSim.Framework.Types;

using InventoryItem = OpenSim.Framework.Inventory.InventoryItem;

namespace OpenSim.Region.Environment.Scenes
{
    public delegate void PrimCountTaintedDelegate();

    public class Primitive : EntityBase
    {
        private const uint FULL_MASK_PERMISSIONS = 2147483647;

        private LLVector3 m_positionLastFrame = new LLVector3(0, 0, 0);
        private ulong m_regionHandle;
        private byte m_updateFlag;
        private uint m_flags = 32 + 65536 + 131072 + 256 + 4 + 8 + 2048 + 524288 + 268435456 + 128;

        private Dictionary<LLUUID, InventoryItem> m_inventoryItems;

        private string m_description = "";

        public LLUUID CreatorID;
        public LLUUID OwnerID;
        public LLUUID LastOwnerID;

        public Int32 CreationDate;

        public uint ParentID = 0;

        public uint OwnerMask = FULL_MASK_PERMISSIONS;
        public uint NextOwnerMask = FULL_MASK_PERMISSIONS;
        public uint GroupMask = 0;// FULL_MASK_PERMISSIONS;
        public uint EveryoneMask = 0;//FULL_MASK_PERMISSIONS;
        public uint BaseMask = 0;//FULL_MASK_PERMISSIONS;

        private PrimitiveBaseShape m_shape;
        private byte[] m_particleSystem = new byte[0];

        public SceneObjectOLD m_RootParent;
        public bool m_isRootPrim;
        public EntityBase m_Parent;

        public event PrimCountTaintedDelegate OnPrimCountTainted;

        #region Properties

        /// <summary>
        /// If rootprim, will return world position
        /// otherwise will return local offset from rootprim
        /// </summary>
        public override LLVector3 AbsolutePosition
        {
            get
            {
                if (m_isRootPrim)
                {
                    //if we are rootprim then our offset should be zero
                    return m_pos + m_Parent.AbsolutePosition;
                }
                else
                {
                    return m_pos;
                }
            }
            set
            {
                if (m_isRootPrim)
                {
                    m_Parent.AbsolutePosition = value;
                }
                m_pos = value - m_Parent.AbsolutePosition;
            }
        }

        public PrimitiveBaseShape Shape
        {
            get { return m_shape; }
        }

        public LLVector3 WorldPos
        {
            get
            {
                if (!m_isRootPrim)
                {
                    Primitive parentPrim = (Primitive)m_Parent;
                    Vector3 offsetPos = new Vector3(m_pos.X, m_pos.Y, m_pos.Z);
                    offsetPos = parentPrim.Rotation * offsetPos;
                    return parentPrim.WorldPos + new LLVector3(offsetPos.x, offsetPos.y, offsetPos.z);
                }
                else
                {
                    return AbsolutePosition;
                }
            }
        }

        public string Description
        {
            get { return m_description; }
            set { m_description = value; }
        }

        public LLVector3 Scale
        {
            set { m_shape.Scale = value; }
            get { return m_shape.Scale; }
        }

        private string m_sitName = "";
        public string SitName
        {
            get { return m_sitName; }
        }

        private string m_touchName = "";
        public string TouchName
        {
            get { return m_touchName; }
        }

        private string m_text = "";
        public string Text
        {
            get { return m_text; }
            set
            {
                m_text = value;
                ScheduleFullUpdate();
            }
        }

        #endregion

        #region Constructors

        public Primitive(ulong regionHandle, Scene scene, LLUUID ownerID, uint localID, bool isRoot, EntityBase parent,
                         SceneObjectOLD rootObject, PrimitiveBaseShape shape, LLVector3 pos)
        {
            m_regionHandle = regionHandle;
            m_scene = scene;
            m_inventoryItems = new Dictionary<LLUUID, InventoryItem>();
            m_Parent = parent;
            m_isRootPrim = isRoot;
            m_RootParent = rootObject;
            ClearUpdateSchedule();
            CreateFromShape(ownerID, localID, pos, shape);

            Rotation = Quaternion.Identity;

            m_scene.AcknowledgeNewPrim(this);

            OnPrimCountTainted();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Empty constructor for duplication</remarks>
        public Primitive()
        {
        }

        #endregion

        #region Destructors

        ~Primitive()
        {
            if (OnPrimCountTainted != null)
                OnPrimCountTainted();
        }

        #endregion

        #region Duplication

        public Primitive Copy(EntityBase parent, SceneObjectOLD rootParent)
        {
            Primitive dupe = (Primitive)MemberwiseClone();

            dupe.m_Parent = parent;
            dupe.m_RootParent = rootParent;

            // TODO: Copy this properly.

            dupe.m_inventoryItems = m_inventoryItems;
            dupe.m_children = new List<EntityBase>();
            dupe.m_shape = m_shape.Copy();
            dupe.m_regionHandle = m_regionHandle;
            dupe.m_scene = m_scene;


            uint newLocalID = m_scene.PrimIDAllocate();
            dupe.m_uuid = LLUUID.Random();
            dupe.LocalId = newLocalID;

            if (parent is SceneObjectGroup)
            {
                dupe.m_isRootPrim = true;
                dupe.ParentID = 0;
            }
            else
            {
                dupe.m_isRootPrim = false;
                dupe.ParentID = ((Primitive)parent).LocalId;
            }

            dupe.Scale = new LLVector3(Scale.X, Scale.Y, Scale.Z);
            dupe.Rotation = new Quaternion(Rotation.w, Rotation.x, Rotation.y, Rotation.z);
            dupe.m_pos = new LLVector3(m_pos.X, m_pos.Y, m_pos.Z);

            rootParent.AddChildToList(dupe);
            m_scene.AcknowledgeNewPrim(dupe);
            dupe.TriggerOnPrimCountTainted();


            foreach (Primitive prim in m_children)
            {
                Primitive primClone = prim.Copy(dupe, rootParent);

                dupe.m_children.Add(primClone);
            }

            return dupe;
        }

        #endregion

        #region Override from EntityBase

        /// <summary>
        /// 
        /// </summary>
        public override void Update()
        {
            if (m_updateFlag == 1) //some change has been made so update the clients
            {
                SendTerseUpdateToALLClients();
                ClearUpdateSchedule();
            }
            else
            {
                if (m_updateFlag == 2) // is a new prim just been created/reloaded or has major changes
                {
                    SendFullUpdateToAllClients();
                    ClearUpdateSchedule();
                }
            }

            foreach (EntityBase child in m_children)
            {
                child.Update();
            }
        }

        private void ClearUpdateSchedule()
        {
            m_updateFlag = 0;
        }

        #endregion

        #region Setup

        /// <summary>
        /// 
        /// </summary>
        /// <param name="addPacket"></param>
        /// <param name="ownerID"></param>
        /// <param name="localID"></param>
        public void CreateFromShape(LLUUID ownerID, uint localID, LLVector3 pos, PrimitiveBaseShape shape)
        {
            CreationDate = (Int32)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            OwnerID = ownerID;
            CreatorID = OwnerID;
            LastOwnerID = LLUUID.Zero;
            AbsolutePosition = pos;
            m_uuid = LLUUID.Random();
            m_localId = (uint)(localID);

            m_shape = shape;

            ScheduleFullUpdate();
        }

        private void ScheduleFullUpdate()
        {
            m_updateFlag = 2;
        }

        private void ScheduleTerseUpdate()
        {
            if (m_updateFlag < 1)
            {
                m_updateFlag = 1;
            }
        }

        #endregion

        #region Linking / unlinking

        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkObject"></param>
        public void AddNewChildren(SceneObjectOLD linkObject)
        {
            // Console.WriteLine("linking new prims " + linkObject.rootLocalID + " to me (" + this.LocalId + ")");
            //TODO check permissions

            m_children.Add(linkObject.rootPrimitive);
           linkObject.rootPrimitive.SetNewParent(this, m_RootParent);

            m_scene.DeleteEntity(linkObject.rootUUID);
            linkObject.DeleteAllChildren();

            OnPrimCountTainted();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newParent"></param>
        /// <param name="rootParent"></param>
        public void SetNewParent(Primitive newParent, SceneObjectOLD rootParent)
        {
            LLVector3 oldPos = new LLVector3(AbsolutePosition.X, AbsolutePosition.Y, AbsolutePosition.Z);
            m_isRootPrim = false;
            m_Parent = newParent;
            ParentID = newParent.LocalId;
            m_RootParent = rootParent;
            m_RootParent.AddChildToList(this);
            AbsolutePosition = oldPos;
            Vector3 axPos = new Vector3(m_pos.X, m_pos.Y, m_pos.Z);
            axPos = m_Parent.Rotation.Inverse() * axPos;
            m_pos = new LLVector3(axPos.x, axPos.y, axPos.z);
            Quaternion oldRot = new Quaternion(Rotation.w, Rotation.x, Rotation.y, Rotation.z);
            Rotation = m_Parent.Rotation.Inverse() * Rotation;
            ScheduleFullUpdate();


            foreach (Primitive child in m_children)
            {
                child.SetRootParent(rootParent, newParent, oldPos, oldRot);
            }

            m_children.Clear();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newRoot"></param>
        public void SetRootParent(SceneObjectOLD newRoot, Primitive newParent, LLVector3 oldParentPosition,
                                  Quaternion oldParentRotation)
        {
            LLVector3 oldPos = new LLVector3(AbsolutePosition.X, AbsolutePosition.Y, AbsolutePosition.Z);
            Vector3 axOldPos = new Vector3(oldPos.X, oldPos.Y, oldPos.Z);
            axOldPos = oldParentRotation * axOldPos;
            oldPos = new LLVector3(axOldPos.x, axOldPos.y, axOldPos.z);
            oldPos += oldParentPosition;
            Quaternion oldRot = new Quaternion(Rotation.w, Rotation.x, Rotation.y, Rotation.z);
            m_isRootPrim = false;
            m_Parent = newParent;
            ParentID = newParent.LocalId;
            newParent.AddToChildrenList(this);

            m_RootParent = newRoot;
            m_RootParent.AddChildToList(this);
            AbsolutePosition = oldPos;
            Vector3 axPos = new Vector3(m_pos.X, m_pos.Y, m_pos.Z);
            axPos = m_Parent.Rotation.Inverse() * axPos;
            m_pos = new LLVector3(axPos.x, axPos.y, axPos.z);
            Rotation = oldParentRotation * Rotation;
            Rotation = m_Parent.Rotation.Inverse() * Rotation;
            ScheduleFullUpdate();
            foreach (Primitive child in m_children)
            {
                child.SetRootParent(newRoot, newParent, oldPos, oldRot);
            }

            m_children.Clear();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public void AddOffsetToChildren(LLVector3 offset)
        {
            foreach (Primitive prim in m_children)
            {
                prim.m_pos += offset;
                prim.ScheduleTerseUpdate();
            }
            OnPrimCountTainted();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prim"></param>
        public void AddToChildrenList(Primitive prim)
        {
            m_children.Add(prim);
        }

        #endregion

        #region Resizing/Scale

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scale"></param>
        public void ResizeGoup(LLVector3 scale)
        {
            m_shape.Scale = scale;

            ScheduleFullUpdate();
        }

        #endregion

        #region Position

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        public void UpdateGroupPosition(LLVector3 pos)
        {
            LLVector3 newPos = new LLVector3(pos.X, pos.Y, pos.Z);

            AbsolutePosition = newPos;
            ScheduleTerseUpdate();

            OnPrimCountTainted();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        public void UpdateSinglePosition(LLVector3 pos)
        {
            // Console.WriteLine("updating single prim position");
            if (m_isRootPrim)
            {
                LLVector3 newPos = new LLVector3(pos.X, pos.Y, pos.Z);
                LLVector3 oldPos = new LLVector3(AbsolutePosition.X, AbsolutePosition.Y, AbsolutePosition.Z);
                LLVector3 diff = oldPos - newPos;
                Vector3 axDiff = new Vector3(diff.X, diff.Y, diff.Z);
                axDiff = Rotation.Inverse() * axDiff;
                diff.X = axDiff.x;
                diff.Y = axDiff.y;
                diff.Z = axDiff.z;
                AbsolutePosition = newPos;

                foreach (Primitive prim in m_children)
                {
                    prim.m_pos += diff;
                    prim.ScheduleTerseUpdate();
                }
                ScheduleTerseUpdate();
            }
            else
            {
                LLVector3 newPos = new LLVector3(pos.X, pos.Y, pos.Z);
                m_pos = newPos;
                ScheduleTerseUpdate();
            }
        }

        #endregion

        #region Rotation

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rot"></param>
        public void UpdateGroupRotation(LLQuaternion rot)
        {
            Rotation = new Quaternion(rot.W, rot.X, rot.Y, rot.Z);
            ScheduleTerseUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="rot"></param>
        public void UpdateGroupMouseRotation(LLVector3 pos, LLQuaternion rot)
        {
            Rotation = new Quaternion(rot.W, rot.X, rot.Y, rot.Z);
            AbsolutePosition = pos;
            ScheduleTerseUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rot"></param>
        public void UpdateSingleRotation(LLQuaternion rot)
        {
            //Console.WriteLine("updating single prim rotation");

            Quaternion axRot = new Quaternion(rot.W, rot.X, rot.Y, rot.Z);
            Quaternion oldParentRot = new Quaternion(Rotation.w, Rotation.x, Rotation.y, Rotation.z);
            Rotation = axRot;
            foreach (Primitive prim in m_children)
            {
                Vector3 axPos = new Vector3(prim.m_pos.X, prim.m_pos.Y, prim.m_pos.Z);
                axPos = oldParentRot * axPos;
                axPos = axRot.Inverse() * axPos;
                prim.m_pos = new LLVector3(axPos.x, axPos.y, axPos.z);
                prim.Rotation = oldParentRot * prim.Rotation;
                prim.Rotation = axRot.Inverse() * prim.Rotation;
                prim.ScheduleTerseUpdate();
            }
            ScheduleTerseUpdate();
        }

        #endregion

        #region Shape

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapeBlock"></param>
        public void UpdateShape(ObjectShapePacket.ObjectDataBlock shapeBlock)
        {
            m_shape.PathBegin = shapeBlock.PathBegin;
            m_shape.PathEnd = shapeBlock.PathEnd;
            m_shape.PathScaleX = shapeBlock.PathScaleX;
            m_shape.PathScaleY = shapeBlock.PathScaleY;
            m_shape.PathShearX = shapeBlock.PathShearX;
            m_shape.PathShearY = shapeBlock.PathShearY;
            m_shape.PathSkew = shapeBlock.PathSkew;
            m_shape.ProfileBegin = shapeBlock.ProfileBegin;
            m_shape.ProfileEnd = shapeBlock.ProfileEnd;
            m_shape.PathCurve = shapeBlock.PathCurve;
            m_shape.ProfileCurve = shapeBlock.ProfileCurve;
            m_shape.ProfileHollow = shapeBlock.ProfileHollow;
            m_shape.PathRadiusOffset = shapeBlock.PathRadiusOffset;
            m_shape.PathRevolutions = shapeBlock.PathRevolutions;
            m_shape.PathTaperX = shapeBlock.PathTaperX;
            m_shape.PathTaperY = shapeBlock.PathTaperY;
            m_shape.PathTwist = shapeBlock.PathTwist;
            m_shape.PathTwistBegin = shapeBlock.PathTwistBegin;
            ScheduleFullUpdate();
        }

        #endregion

        #region Inventory
        public void GetInventory(IClientAPI client, uint localID)
        {
            if (localID == this.m_localId)
            {
                client.SendTaskInventory(this.m_uuid, 0, new byte[0]);
            }
        }
        #endregion

        public void UpdateExtraParam(ushort type, bool inUse, byte[] data)
        {
            this.m_shape.ExtraParams = new byte[data.Length + 7];
            int i =0;
            uint length = (uint) data.Length;
            this.m_shape.ExtraParams[i++] = 1;
            this.m_shape.ExtraParams[i++] = (byte)(type % 256);
            this.m_shape.ExtraParams[i++] = (byte)((type >> 8) % 256);
               
            this.m_shape.ExtraParams[i++] = (byte)(length % 256);
            this.m_shape.ExtraParams[i++] = (byte)((length >> 8) % 256);
            this.m_shape.ExtraParams[i++] = (byte)((length >> 16) % 256);
            this.m_shape.ExtraParams[i++] = (byte)((length >> 24) % 256);
            Array.Copy(data, 0, this.m_shape.ExtraParams, i, data.Length);

            this.ScheduleFullUpdate();
        }

        #region Texture

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textureEntry"></param>
        public void UpdateTextureEntry(byte[] textureEntry)
        {
            m_shape.TextureEntry = textureEntry;
            ScheduleFullUpdate();
        }

        #endregion

        public void AddNewParticleSystem(libsecondlife.Primitive.ParticleSystem pSystem)
        {
            this.m_particleSystem = pSystem.GetBytes();
            ScheduleFullUpdate();
        }

        #region Client Update Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteClient"></param>
        public void SendFullUpdateForAllChildren(IClientAPI remoteClient)
        {

            SendFullUpdateToClient(remoteClient);
            for (int i = 0; i < m_children.Count; i++)

            {

                if (m_children[i] is Primitive)
                {
                    ((Primitive)m_children[i]).SendFullUpdateForAllChildren(remoteClient);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteClient"></param>
        public void SendFullUpdateToClient(IClientAPI remoteClient)
        {
            LLVector3 lPos;
            lPos = AbsolutePosition;
            LLQuaternion lRot;
            lRot = new LLQuaternion(Rotation.x, Rotation.y, Rotation.z, Rotation.w);

            remoteClient.SendPrimitiveToClient(m_regionHandle, 64096, LocalId, m_shape, lPos, m_flags, m_uuid, OwnerID,
                                               m_text, ParentID, this.m_particleSystem, lRot);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendFullUpdateToAllClients()
        {
            List<ScenePresence> avatars = m_scene.RequestAvatarList();
            for (int i = 0; i < avatars.Count; i++)
            {
                SendFullUpdateToClient(avatars[i].ControllingClient);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteClient"></param>
        public void SendTerseUpdateForAllChildren(IClientAPI remoteClient)
        {

            SendTerseUpdateToClient(remoteClient);
            for (int i = 0; i < m_children.Count; i++)
            {
                if (m_children[i] is Primitive)
                {
                    ((Primitive)m_children[i]).SendTerseUpdateForAllChildren(remoteClient);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="RemoteClient"></param>
        public void SendTerseUpdateToClient(IClientAPI RemoteClient)
        {
            LLVector3 lPos;
            Quaternion lRot;

            lPos = AbsolutePosition;
            lRot = Rotation;

            LLQuaternion mRot = new LLQuaternion(lRot.x, lRot.y, lRot.z, lRot.w);
            RemoteClient.SendPrimTerseUpdate(m_regionHandle, 64096, LocalId, lPos, mRot);
        }

        /// <summary>
        /// 
        /// </summary>
        public void SendTerseUpdateToALLClients()
        {
            List<ScenePresence> avatars = m_scene.RequestAvatarList();
            for (int i = 0; i < avatars.Count; i++)
            {
                SendTerseUpdateToClient(avatars[i].ControllingClient);
            }
        }

        #endregion

        public void TriggerOnPrimCountTainted()
        {
            OnPrimCountTainted();
        }

        public override void SetText(string text, Vector3 color, double alpha)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}