/*
* Copyright (c) Contributors, http://www.openmetaverse.org/
* See CONTRIBUTORS.TXT for a full list of copyright holders.
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the OpenSim Project nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* 
*/
using System.Text;
using libsecondlife;
using libsecondlife.Packets;
using OpenSim.Framework.Data;
using OpenSim.Framework.Interfaces;
using OpenSim.Framework.Types;

namespace OpenSim.Framework.Communications
{
 
    public class CommunicationsManager
    {
        public IUserServices UserServer;
        public IGridServices GridServer;
        public IInterRegionCommunications InterRegion;

        public NetworkServersInfo ServersInfo;
        public CommunicationsManager(NetworkServersInfo serversInfo)
        {
            ServersInfo = serversInfo;
        }

        #region Packet Handlers
        public void HandleUUIDNameRequest(LLUUID uuid, IClientAPI remote_client)
        {
            Encoding enc = Encoding.ASCII;
            UserProfileData profileData = this.UserServer.GetUserProfile(uuid);
            if (profileData != null)
            {
                UUIDNameReplyPacket packet = new UUIDNameReplyPacket();
                packet.UUIDNameBlock = new UUIDNameReplyPacket.UUIDNameBlockBlock[1];
                packet.UUIDNameBlock[0] = new UUIDNameReplyPacket.UUIDNameBlockBlock();
                packet.UUIDNameBlock[0].ID = profileData.UUID;
                packet.UUIDNameBlock[0].FirstName = enc.GetBytes(profileData.username + "\0");
                packet.UUIDNameBlock[0].LastName = enc.GetBytes(profileData.surname +"\0");
                remote_client.OutPacket((Packet)packet);
            }
            
        }
        #endregion
    }
}
