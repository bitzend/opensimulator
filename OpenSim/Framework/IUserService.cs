/*
* Copyright (c) Contributors, http://opensimulator.org/
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
* THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
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
using System.Collections.Generic;
using libsecondlife;

namespace OpenSim.Framework
{
    public interface IUserService
    {
        UserProfileData GetUserProfile(string firstName, string lastName, string authAddr);
        UserProfileData GetUserProfile(string firstName, string authAddr); // must differentiate this from GetUserProfile call
        
        //UserProfileData GetUserProfile(string name);
        UserProfileData GetUserProfileByAccount(string account);
        UserProfileData GetUserProfile(LLUUID userId, string authAddr);
        void clearUserAgent(LLUUID avatarID, string authAddr);
        void UpdateUserAgentData(LLUUID agentId, bool agentOnline, LLVector3 currentPos, int logoutTime, string authAddr);


        List<AvatarPickerAvatar> GenerateAgentPickerRequestResponse(LLUUID QueryID, string Query);

        UserProfileData SetupMasterUser(string firstName, string lastName);
        UserProfileData SetupMasterUser(string firstName, string lastName, string password);
        UserProfileData SetupMasterUser(LLUUID userId);

        bool AuthenticateUser(LLUUID agentID, string sessionhash, out string asAddress);//rex

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        LLUUID AddUserProfile(string firstName, string lastName, string pass, uint regX, uint regY);


        /// <summary>
        /// Adds a new friend to the database for XUser
        /// </summary>
        /// <param name="friendlistowner">The agent that who's friends list is being added to</param>
        /// <param name="friend">The agent that being added to the friends list of the friends list owner</param>
        /// <param name="perms">A uint bit vector for set perms that the friend being added has; 0 = none, 1=This friend can see when they sign on, 2 = map, 4 edit objects </param>
        void AddNewUserFriend(LLUUID friendlistowner, LLUUID friend, uint perms);

        /// <summary>
        /// Delete friend on friendlistowner's friendlist.
        /// </summary>
        /// <param name="friendlistowner">The agent that who's friends list is being updated</param>
        /// <param name="friend">The Ex-friend agent</param>
        void RemoveUserFriend(LLUUID friendlistowner, LLUUID friend);

        /// <summary>
        /// Update permissions for friend on friendlistowner's friendlist.
        /// </summary>
        /// <param name="friendlistowner">The agent that who's friends list is being updated</param>
        /// <param name="friend">The agent that is getting or loosing permissions</param>
        /// <param name="perms">A uint bit vector for set perms that the friend being added has; 0 = none, 1=This friend can see when they sign on, 2 = map, 4 edit objects </param>
        void UpdateUserFriendPerms(LLUUID friendlistowner, LLUUID friend, uint perms);

        /// <summary>
        /// Logs off a user on the user server
        /// </summary>
        /// <param name="UserID">UUID of the user</param>
        /// <param name="regionData">UUID of the Region</param>
        /// <param name="posx">final position x</param>
        /// <param name="posy">final position y</param>
        /// <param name="posz">final position z</param>
        void LogOffUser(LLUUID userid, LLUUID regionid, ulong regionhandle, float posx, float posy, float posz);

        /// <summary>
        /// Returns a list of FriendsListItems that describe the friends and permissions in the friend relationship for LLUUID friendslistowner
        /// </summary>
        /// <param name="friendlistowner">The agent that we're retreiving the friends Data.</param>
        List<FriendListItem> GetUserFriendList(LLUUID friendlistowner);
    }
}
