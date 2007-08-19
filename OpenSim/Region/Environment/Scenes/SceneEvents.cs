using libsecondlife;
using OpenSim.Framework.Interfaces;

namespace OpenSim.Region.Environment.Scenes
{
    /// <summary>
    /// A class for triggering remote scene events.
    /// </summary>
    public class EventManager
    {
        public delegate void OnFrameDelegate();
        public event OnFrameDelegate OnFrame;

        public delegate void OnBackupDelegate(Interfaces.IRegionDataStore datastore);
        public event OnBackupDelegate OnBackup;

        public delegate void OnNewPresenceDelegate(ScenePresence presence);
        public event OnNewPresenceDelegate OnNewPresence;

        public delegate void OnRemovePresenceDelegate(LLUUID uuid);
        public event OnRemovePresenceDelegate OnRemovePresence;

        public delegate void OnParcelPrimCountUpdateDelegate();
        public event OnParcelPrimCountUpdateDelegate OnParcelPrimCountUpdate;

        public delegate void OnParcelPrimCountAddDelegate(SceneObjectGroup obj);
        public event OnParcelPrimCountAddDelegate OnParcelPrimCountAdd;

        public delegate void OnScriptConsoleDelegate(string[] args);
        public event OnScriptConsoleDelegate OnScriptConsole;

        public delegate void OnShutdownDelegate();
        public event OnShutdownDelegate OnShutdown;

        public delegate void ObjectGrabDelegate(uint localID, LLVector3 offsetPos, IClientAPI remoteClient);
        public delegate void OnPermissionErrorDelegate(LLUUID user, string reason);
        public event ObjectGrabDelegate OnObjectGrab;
        public event OnPermissionErrorDelegate OnPermissionError;


        public void TriggerPermissionError(LLUUID user, string reason)
        {
            if (OnPermissionError != null)
                OnPermissionError(user, reason);
        }

        public void TriggerOnScriptConsole(string[] args)
        {
            if (OnScriptConsole != null)
                OnScriptConsole(args);
        }

        public void TriggerOnFrame()
        {
            if (OnFrame != null)
            {
                OnFrame();
            }
        }

        public void TriggerOnNewPresence(ScenePresence presence)
        {
            if (OnNewPresence != null)
                OnNewPresence(presence);
        }

        public void TriggerOnRemovePresence(LLUUID uuid)
        {
            if (OnRemovePresence != null)
            {
                OnRemovePresence(uuid);
            }
        }

        public void TriggerOnBackup(Interfaces.IRegionDataStore dstore)
        {
            if (OnBackup != null)
            {
                OnBackup(dstore);
            }
        }

        public void TriggerParcelPrimCountUpdate()
        {
            if (OnParcelPrimCountUpdate != null)
            {
                OnParcelPrimCountUpdate();
            }
        }
        public void TriggerParcelPrimCountAdd(SceneObjectGroup obj)
        {
            if (OnParcelPrimCountAdd != null)
            {
                OnParcelPrimCountAdd(obj);
            }
        }

        public void TriggerShutdown()
        {
            if (OnShutdown != null)
                OnShutdown();
        }

        public void TriggerObjectGrab(uint localID, LLVector3 offsetPos, IClientAPI remoteClient)
        {
            if (OnObjectGrab != null)
                OnObjectGrab(localID, offsetPos, remoteClient);
        }
    }
}
