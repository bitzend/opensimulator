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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using libsecondlife;
using libsecondlife.StructuredData;
using Nwc.XmlRpc;
using OpenSim.Framework.Console;
using OpenSim.Framework.Statistics;

namespace OpenSim.Framework.UserManagement
{
    /// <summary>
    /// Base class for user management (create, read, etc)
    /// </summary>
    public abstract class UserManagerBase : IUserService
    {
        private static readonly log4net.ILog m_log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public UserConfig _config;
        private Dictionary<string, IUserData> _plugins = new Dictionary<string, IUserData>();
        public bool RexMode = false; // _config is not initiated in local mode

        /// <summary>
        /// Adds a new user server plugin - user servers will be requested in the order they were loaded.
        /// </summary>
        /// <param name="FileName">The filename to the user server plugin DLL</param>
        public void AddPlugin(string FileName)
        {
            if (!String.IsNullOrEmpty(FileName))
            {
                m_log.Info("[USERSTORAGE]: Attempting to load " + FileName);
                Assembly pluginAssembly = Assembly.LoadFrom(FileName);

                m_log.Info("[USERSTORAGE]: Found " + pluginAssembly.GetTypes().Length + " interfaces.");
                foreach (Type pluginType in pluginAssembly.GetTypes())
                {
                    if (!pluginType.IsAbstract)
                    {
                        Type typeInterface = pluginType.GetInterface("IUserData", true);

                        if (typeInterface != null)
                        {
                            IUserData plug =
                                (IUserData) Activator.CreateInstance(pluginAssembly.GetType(pluginType.ToString()));
                            AddPlugin(plug);
                        }
                    }
                }
            }
        }

        public void AddPlugin(IUserData plug)
        {
            plug.Initialise();
            _plugins.Add(plug.getName(), plug);
            m_log.Info("[USERSTORAGE]: Added IUserData Interface");
        }

        #region Get UserProfile 

        // see IUserService
        public UserProfileData GetUserProfile(string fname, string lname)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                UserProfileData profile = plugin.Value.GetUserByName(fname, lname);

                if (profile != null)
                {
                    profile.currentAgent = getUserAgent(profile.UUID);
                    return profile;
                }
            }

            return null;
        }
        
        // see IUserService        
        public UserProfileData GetUserProfile(LLUUID uuid, string authAddr)
        {
            if (!RexMode)
            {
                foreach (KeyValuePair<string, IUserData> plugin in _plugins)
                {
                    try
                    {
                        UserProfileData profile = plugin.Value.GetUserByUUID(uuid);
                        if (null != profile)
                        {
                            profile.currentAgent = getUserAgent(profile.UUID);
                            return profile;
                        }
                    }
                    catch (Exception e)
                    {
                        MainLog.Instance.Verbose("USERSTORAGE", "Unable to find user via " + plugin.Key + "(" + e.ToString() + ")");
                    }
                }
                return null;
            }
            else
            {
                try
                {
                    UserProfileData userpd = null;
                    System.Collections.Hashtable param = new System.Collections.Hashtable();
                    param["avatar_uuid"] = uuid.ToString();
                    param["AuthenticationAddress"] = authAddr;
                    System.Collections.Hashtable resp = MakeCommonRequest("get_user_by_uuid", param, authAddr, 3000);
                    userpd = RexLoginHandler.HashtableToUserProfileData(resp);
                    return userpd;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error when trying to fetch profile data by uuid from remote authentication server: " +
                                      e.Message);
                }
                return null;
            }
        }

        public System.Collections.Hashtable MakeCommonRequest(string method, System.Collections.Hashtable param, string addr, int timeout)//rex
        {
            System.Collections.IList parameters = new System.Collections.ArrayList();
            parameters.Add(param);
            XmlRpcRequest req = new XmlRpcRequest(method, parameters);
            if (!addr.StartsWith("http://"))
                addr = "http://" + addr;
            XmlRpcResponse resp = req.Send(addr, timeout);
            return (System.Collections.Hashtable)resp.Value;
        }


        /// <summary>
        /// Loads a user profile by name
        /// </summary>
        /// <param name="name">The target name</param>
        /// <returns>A user profile</returns>
        public UserProfileData GetUserProfileByAccount(string account)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    UserProfileData profile = plugin.Value.GetUserByAccount(account);
                    profile.currentAgent = getUserAgent(profile.UUID);
                    return profile;
                }
                catch (Exception e)
                {
                    MainLog.Instance.Verbose("USERSTORAGE", "Unable to find user by account via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }

            return null;
        }


        public List<AvatarPickerAvatar> GenerateAgentPickerRequestResponse(LLUUID queryID, string query)
        {
            List<AvatarPickerAvatar> pickerlist = new List<AvatarPickerAvatar>();
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    pickerlist = plugin.Value.GeneratePickerResults(queryID, query);
                }
                catch (Exception)
                {
                    m_log.Info("[USERSTORAGE]: Unable to generate AgentPickerData via  " + plugin.Key + "(" + query + ")");
                    return new List<AvatarPickerAvatar>();
                }
            }
            return pickerlist;
        }

        /// <summary>
        /// Loads a user profile by name
        /// </summary>
        /// <param name="fname">First name</param>
        /// <param name="lname">Last name</param>
        /// <returns>A user profile.  Returns null if no profile is found</returns>
        public UserProfileData GetUserProfile(string fname, string lname, string authAddr)
        {
            if (!RexMode)
            {
                foreach (KeyValuePair<string, IUserData> plugin in _plugins)
                {
                    try
                    {
                        UserProfileData profile = plugin.Value.GetUserByName(fname, lname);

                        if (profile != null)
                        {
                            profile.currentAgent = getUserAgent(profile.UUID);
                            return profile;
                        }
                    }
                    catch (Exception e)
                    {
                        MainLog.Instance.Verbose("USERSTORAGE", "Unable to find user via " + plugin.Key + "(" + e.ToString() + ")");
                    }
                }
            }
            else
            {
                try
                {
                    UserProfileData userpd = null;
                    System.Collections.Hashtable param = new System.Collections.Hashtable();
                    param["avatar_name"] = fname + " "+lname;
                    param["AuthenticationAddress"] = authAddr;
                    System.Collections.Hashtable resp = MakeCommonRequest("get_user_by_name", param, authAddr, 3000);
                    if (resp.Contains("error_type"))
                    {
                        return null;
                    }
                    else
                    {
                        userpd = RexLoginHandler.HashtableToUserProfileData(resp);
                        return userpd;
                    }
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error when trying to fetch profile data by firstname, lastname from remote authentication server: " +
                                      e.Message);
                }
            }

            return null;
        }

        /// <summary>
        /// Set's user profile from object
        /// </summary>
        /// <param name="fname">First name</param>
        /// <param name="lname">Last name</param>
        /// <returns>A user profile</returns>
        public bool setUserProfile(UserProfileData data)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    plugin.Value.UpdateUserProfile(data);
                    return true;
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to set user via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }

            return false;
        }

        #endregion

        #region Get UserAgent

        /// <summary>
        /// Loads a user agent by uuid (not called directly)
        /// </summary>
        /// <param name="uuid">The agent's UUID</param>
        /// <returns>Agent profiles</returns>
        public UserAgentData getUserAgent(LLUUID uuid)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    return plugin.Value.GetAgentByUUID(uuid);
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to find user via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }

            return null;
        }

        /// <summary>
        /// Loads a user's friend list
        /// </summary>
        /// <param name="name">the UUID of the friend list owner</param>
        /// <returns>A List of FriendListItems that contains info about the user's friends</returns>
        public List<FriendListItem> GetUserFriendList(LLUUID ownerID)
        {

            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    return plugin.Value.GetUserFriendList(ownerID);
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to GetUserFriendList via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }

            return null;

        }

        public void StoreWebLoginKey(LLUUID agentID, LLUUID webLoginKey)
        {

            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    plugin.Value.StoreWebLoginKey(agentID, webLoginKey);
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to Store WebLoginKey via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }
        }

        public void AddNewUserFriend(LLUUID friendlistowner, LLUUID friend, uint perms)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    plugin.Value.AddNewUserFriend(friendlistowner,friend,perms);
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to AddNewUserFriend via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }

        }


        public void RemoveUserFriend(LLUUID friendlistowner, LLUUID friend)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                   plugin.Value.RemoveUserFriend(friendlistowner, friend);
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to RemoveUserFriend via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }
        }

        public void UpdateUserFriendPerms(LLUUID friendlistowner, LLUUID friend, uint perms)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    plugin.Value.UpdateUserFriendPerms(friendlistowner, friend, perms);
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to UpdateUserFriendPerms via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }
        }
        /// <summary>
        /// Loads a user agent by name (not called directly)
        /// </summary>
        /// <param name="name">The agent's name</param>
        /// <returns>A user agent</returns>
        public UserAgentData getUserAgent(string name)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    return plugin.Value.GetAgentByName(name);
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to find user via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }

            return null;
        }

        // TODO: document
        public void clearUserAgent(LLUUID agentID, string authAddr)
        {
            UserProfileData profile = GetUserProfile(agentID, authAddr);
            if (profile != null)
            {
                profile.currentAgent = null;
                if (!RexMode)
                {
                    setUserProfile(profile);
                }
                else
                {
                    try
                    {
                        System.Collections.Hashtable param = new System.Collections.Hashtable();
                        param["agentID"] = profile.UUID.ToString();
                        System.Collections.Hashtable resp = MakeCommonRequest("remove_user_agent", param, authAddr, 3000);
                    }
                    catch (Exception e)
                    {
                        System.Console.WriteLine("Error when trying to fetch agent data by uuid from remote authentication server: " +
                                          e.Message);
                    }
                }
                
            }
            else
            {
                MainLog.Instance.Verbose("USERSTORAGE", "Unable to clear user agent with agentID : " + agentID);
            }
        }

        /// <summary>
        /// Loads a user agent by name (not called directly)
        /// </summary>
        /// <param name="fname">The agent's firstname</param>
        /// <param name="lname">The agent's lastname</param>
        /// <returns>A user agent</returns>
        public UserAgentData getUserAgent(string fname, string lname)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    return plugin.Value.GetAgentByName(fname, lname);
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to find user via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }

            return null;
        }

        #endregion

        #region CreateAgent

        /// <summary>
        /// Creates and initialises a new user agent - make sure to use CommitAgent when done to submit to the DB
        /// </summary>
        /// <param name="profile">The users profile</param>
        /// <param name="request">The users loginrequest</param>
        public void CreateAgent(UserProfileData profile, XmlRpcRequest request)
        {
            //Hashtable requestData = (Hashtable) request.Params[0];

            UserAgentData agent = new UserAgentData();

            // User connection
            agent.agentOnline = true;

            // Generate sessions
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
            byte[] randDataS = new byte[16];
            byte[] randDataSS = new byte[16];
            rand.GetBytes(randDataS);
            rand.GetBytes(randDataSS);

            agent.secureSessionID = new LLUUID(randDataSS, 0);
            agent.sessionID = new LLUUID(randDataS, 0);

            // Profile UUID
            agent.UUID = profile.UUID;

            // Current position (from Home)
            agent.currentHandle = profile.homeRegion;
            agent.currentPos = profile.homeLocation;

            // If user specified additional start, use that
//            if (requestData.ContainsKey("start"))
//            {
//                string startLoc = ((string) requestData["start"]).Trim();
//                if (!(startLoc == "last" || startLoc == "home"))
//                {
//                    // Format: uri:Ahern&162&213&34
//                    try
//                    {
//                        string[] parts = startLoc.Remove(0, 4).Split('&');
//                        //string region = parts[0];
//
//                        ////////////////////////////////////////////////////
//                        //SimProfile SimInfo = new SimProfile();
//                        //SimInfo = SimInfo.LoadFromGrid(theUser.currentAgent.currentHandle, _config.GridServerURL, _config.GridSendKey, _config.GridRecvKey);
//                    }
//                    catch (Exception)
//                    {
//                    }
//                }
//            }

            // What time did the user login?
            agent.loginTime = Util.UnixTimeSinceEpoch();
            agent.logoutTime = 0;

            // Current location
            agent.regionID = LLUUID.Zero; // Fill in later
            agent.currentRegion = LLUUID.Zero; // Fill in later

            profile.currentAgent = agent;
        }
        
        /// <summary>
        /// Process a user logoff from OpenSim.
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="regionid"></param>
        /// <param name="regionhandle"></param>
        /// <param name="posx"></param>
        /// <param name="posy"></param>
        /// <param name="posz"></param>
        public void LogOffUser(LLUUID userid, LLUUID regionid, ulong regionhandle, float posx, float posy, float posz)
        {
            if (StatsManager.UserStats != null)
                StatsManager.UserStats.AddLogout();
            
            UserProfileData userProfile;
            UserAgentData userAgent;
            LLVector3 currentPos = new LLVector3(posx, posy, posz);

            userProfile = GetUserProfile(userid);

            if (userProfile != null)
            {
                // This line needs to be in side the above if statement or the UserServer will crash on some logouts.
                m_log.Info("[LOGOUT]: " + userProfile.username + " " + userProfile.surname + " from " + regionhandle + "(" + posx + "," + posy + "," + posz + ")");
                
                userAgent = userProfile.currentAgent;
                if (userAgent != null)
                {
                    userAgent.agentOnline = false;
                    userAgent.logoutTime = Util.UnixTimeSinceEpoch();
                    userAgent.sessionID = LLUUID.Zero;
                    if (regionid != null)
                    {
                        userAgent.currentRegion = regionid;
                    }
                    userAgent.currentHandle = regionhandle;

                    userAgent.currentPos = currentPos;

                    userProfile.currentAgent = userAgent;


                    CommitAgent(ref userProfile);
                }
                else
                {
                    // If currentagent is null, we can't reference it here or the UserServer crashes!
                    m_log.Info("[LOGOUT]: didn't save logout position: " + userid.ToString());
                }
                
            }
            else
            {
                m_log.Warn("[LOGOUT]: Unknown User logged out");
            }
        }
        
        public void CreateAgent(UserProfileData profile, LLSD request)
        {
            UserAgentData agent = new UserAgentData();

            // User connection
            agent.agentOnline = true;

            // Generate sessions
            RNGCryptoServiceProvider rand = new RNGCryptoServiceProvider();
            byte[] randDataS = new byte[16];
            byte[] randDataSS = new byte[16];
            rand.GetBytes(randDataS);
            rand.GetBytes(randDataSS);

            agent.secureSessionID = new LLUUID(randDataSS, 0);
            agent.sessionID = new LLUUID(randDataS, 0);

            // Profile UUID
            agent.UUID = profile.UUID;

            // Current position (from Home)
            agent.currentHandle = profile.homeRegion;
            agent.currentPos = profile.homeLocation;

            // What time did the user login?
            agent.loginTime = Util.UnixTimeSinceEpoch();
            agent.logoutTime = 0;

            // Current location
            agent.regionID = LLUUID.Zero; // Fill in later
            agent.currentRegion = LLUUID.Zero; // Fill in later

            profile.currentAgent = agent;
        }

        /// <summary>
        /// Saves a target agent to the database
        /// </summary>
        /// <param name="profile">The users profile</param>
        /// <returns>Successful?</returns>
        public bool CommitAgent(ref UserProfileData profile)
        {
            // TODO: how is this function different from setUserProfile?
            return setUserProfile(profile);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        public LLUUID AddUserProfile(string firstName, string lastName, string pass, uint regX, uint regY)
        {
            UserProfileData user = new UserProfileData();
            user.homeLocation = new LLVector3(128, 128, 100);
            user.UUID = LLUUID.Random();
            user.username = firstName;
            user.surname = lastName;
            user.passwordHash = pass;
            user.passwordSalt = String.Empty;
            user.created = Util.UnixTimeSinceEpoch();
            user.homeLookAt = new LLVector3(100, 100, 100);
            user.homeRegionX = regX;
            user.homeRegionY = regY;

            
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    plugin.Value.AddNewUserProfile(user);
                }
                catch (Exception e)
                {
                    m_log.Info("[USERSTORAGE]: Unable to add user via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }

            return user.UUID;
        }

        public abstract UserProfileData SetupMasterUser(string firstName, string lastName);
        public abstract UserProfileData SetupMasterUser(string firstName, string lastName, string password);
        public abstract UserProfileData SetupMasterUser(LLUUID uuid);


        public bool AuthenticateUser(LLUUID agentId, string sessionhash, out String avatarstorage)
        {
            avatarstorage = "";
            return true;
        }

        /// <summary>
        /// Loads a user profile by name
        /// </summary>
        /// <param name="name">The target name</param>
        /// <returns>A user profile</returns>
        public UserProfileData GetUserProfile(string name, string authAddr)
        {
            foreach (KeyValuePair<string, IUserData> plugin in _plugins)
            {
                try
                {
                    UserProfileData profile = plugin.Value.GetUserByName(name, authAddr);
                    profile.currentAgent = getUserAgent(profile.UUID);
                    return profile;
                }
                catch (Exception e)
                {
                    MainLog.Instance.Verbose("USERSTORAGE", "Unable to find user via " + plugin.Key + "(" + e.ToString() + ")");
                }
            }

            return null;
        }

        public void UpdateUserAgentData(LLUUID agentId, bool agentOnline, LLVector3 currentPos, int logoutTime, string authAddr)
        {
            // Saves the agent to database
            //return true;
            if (!RexMode)
            {
                foreach (KeyValuePair<string, IUserData> plugin in _plugins)
                {
                    try
                    {
                        UserAgentData agent = plugin.Value.GetAgentByUUID(agentId);
                        if (agent != null)
                        {
                            agent.agentOnline = agentOnline;
                            agent.logoutTime = logoutTime;
                            agent.currentPos = currentPos;
                            agent.currentPos = new LLVector3(
                                    Convert.ToSingle(currentPos.X),
                                    Convert.ToSingle(currentPos.Y),
                                    Convert.ToSingle(currentPos.Z));
                            plugin.Value.AddNewUserAgent(agent);
                            MainLog.Instance.Verbose("USERSTORAGE", "Agent updated UUID = " + agent.UUID.ToString());
                        }
                        else
                        {
                            MainLog.Instance.Verbose("USERSTORAGE", "Agent update, agent not found with UUID = " + agentId);
                        }

                    }
                    catch (Exception e)
                    {
                        MainLog.Instance.Verbose("USERSTORAGE", "Unable to add or update agent via " + plugin.Key + "(" + e.ToString() + ")");
                    }
                }
            }
            else
            {
                
                try
                {
                    System.Collections.Hashtable param = new System.Collections.Hashtable();
                    param["agentID"] = agentId.ToString();
                    param["agentOnline"] = agentOnline.ToString();
                    param["logoutTime"] = logoutTime.ToString();
                    param["agent_currentPosX"] = Convert.ToSingle(currentPos.X).ToString();
                    param["agent_currentPosY"] = Convert.ToSingle(currentPos.Y).ToString();
                    param["agent_currentPosZ"] = Convert.ToSingle(currentPos.Z).ToString();
                    param["AuthenticationAddress"] = authAddr;
                    System.Collections.Hashtable resp = MakeCommonRequest("update_user_agent", param, authAddr, 3000);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine("Error when trying to update user agent data to remote authentication server: " +
                                      e.Message);
                }
                
            }

        }

    }
}
