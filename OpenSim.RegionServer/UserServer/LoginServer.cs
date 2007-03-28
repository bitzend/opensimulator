/*
* Copyright (c) OpenSim project, http://sim.opensecondlife.org/
*
* Redistribution and use in source and binary forms, with or without
* modification, are permitted provided that the following conditions are met:
*     * Redistributions of source code must retain the above copyright
*       notice, this list of conditions and the following disclaimer.
*     * Redistributions in binary form must reproduce the above copyright
*       notice, this list of conditions and the following disclaimer in the
*       documentation and/or other materials provided with the distribution.
*     * Neither the name of the <organization> nor the
*       names of its contributors may be used to endorse or promote products
*       derived from this software without specific prior written permission.
*
* THIS SOFTWARE IS PROVIDED BY <copyright holder> ``AS IS'' AND ANY
* EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
* WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
* DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
* (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
* LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
* ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
* (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
* SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
* 
*/

using Nwc.XmlRpc;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections;
using System.Security.Cryptography;
using System.Xml;
using libsecondlife;
using OpenSim;
using OpenSim.Framework.Interfaces;
using OpenSim.Framework.Grid;
using OpenSim.Framework.Inventory;
using OpenSim.Framework.User;
using OpenSim.Framework.Utilities;

namespace OpenSim.UserServer
{

    /// <summary>
    /// When running in local (default) mode , handles client logins.
    /// </summary>
    public class LoginServer : LoginService, IUserServer
    {
        private IGridServer m_gridServer;
        private ushort _loginPort = 8080;
        public IPAddress clientAddress = IPAddress.Loopback;
        public IPAddress remoteAddress = IPAddress.Any;
        private Socket loginServer;
        private int NumClients;
        private string _defaultResponse;
        public bool userAccounts = false;
        private string _mpasswd;
        private bool _needPasswd = false;
        private LocalUserProfileManager m_localUserManager;
        private int m_simPort;
        private string m_simAddr;

        public LocalUserProfileManager LocalUserManager
        {
            get
            {
                return m_localUserManager;
            }
        }

        public LoginServer(IGridServer gridServer, string simAddr, int simPort)
        {
            m_gridServer = gridServer;
            m_simPort = simPort;
            m_simAddr = simAddr;
        }

        public void Startup()
        {
            this.InitializeLogin();
            //Thread runLoginProxy = new Thread(new ThreadStart(RunLogin));
            //runLoginProxy.IsBackground = true;
            //runLoginProxy.Start();
        }

        // InitializeLogin: initialize the login 
        private void InitializeLogin()
        {
            this._needPasswd = false;
            //read in default response string
            StreamReader SR;
            string lines;
            SR = File.OpenText("new-login.dat");

            //lines=SR.ReadLine();

            while (!SR.EndOfStream)
            {
                lines = SR.ReadLine();
                _defaultResponse += lines;
                //lines = SR.ReadLine();
            }
            SR.Close();
            this._mpasswd = EncodePassword("testpass");

            m_localUserManager = new LocalUserProfileManager(this.m_gridServer, m_simPort, m_simAddr);
            m_localUserManager.InitUserProfiles();
            m_localUserManager.SetKeys("", "", "", "Welcome to OpenSim");

            //loginServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //loginServer.Bind(new IPEndPoint(remoteAddress, _loginPort));
            //loginServer.Listen(1);
        }


        //private void RunLogin()
        //{
        //    Console.WriteLine("Starting Login Server");
        //    try
        //    {
        //        for (; ; )
        //        {
        //            Socket client = loginServer.Accept();
        //            IPEndPoint clientEndPoint = (IPEndPoint)client.RemoteEndPoint;


        //            NetworkStream networkStream = new NetworkStream(client);
        //            StreamReader networkReader = new StreamReader(networkStream);
        //            StreamWriter networkWriter = new StreamWriter(networkStream);

        //            try
        //            {
        //                LoginRequest(networkReader, networkWriter);
        //            }
        //            catch (Exception e)
        //            {
        //                Console.WriteLine(e.Message);
        //            }

        //            networkWriter.Close();
        //            networkReader.Close();
        //            networkStream.Close();

        //            client.Close();

        //            // send any packets queued for injection

        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //        Console.WriteLine(e.StackTrace);
        //    }
        //}

        // ProxyLogin: proxy a login request
        //private void LoginRequest(StreamReader reader, StreamWriter writer)
        //{
        //    lock (this)
        //    {
        //        string line;
        //        int contentLength = 0;
        //        // read HTTP header
        //        do
        //        {
        //            // read one line of the header
        //            line = reader.ReadLine();

        //            // check for premature EOF
        //            if (line == null)
        //                throw new Exception("EOF in client HTTP header");

        //            // look for Content-Length
        //            Match match = (new Regex(@"Content-Length: (\d+)$")).Match(line);
        //            if (match.Success)
        //                contentLength = Convert.ToInt32(match.Groups[1].Captures[0].ToString());
        //        } while (line != "");

        //        // read the HTTP body into a buffer
        //        char[] content = new char[contentLength];
        //        reader.Read(content, 0, contentLength);

        //        if (this.userAccounts)
        //        {
        //            //ask the UserProfile Manager to process the request
        //            string reply = this.userManager.ParseXMLRPC(new String(content));
        //            // forward the XML-RPC response to the client
        //            writer.WriteLine("HTTP/1.0 200 OK");
        //            writer.WriteLine("Content-type: text/xml");
        //            writer.WriteLine();
        //            writer.WriteLine(reply);
        //        }
        //        else
        //        {
        //            //handle ourselves
        //            XmlRpcRequest request = (XmlRpcRequest)(new XmlRpcRequestDeserializer()).Deserialize(new String(content));
        //            if (request.MethodName == "login_to_simulator")
        //            {
        //                this.ProcessXmlRequest(request, writer);
        //            }
        //            else
        //            {

        //                string reply = Regex.Replace(XmlRpcResponseSerializer.Singleton.Serialize(PresenceErrorResp), " encoding=\"utf-16\"", "");
        //                writer.WriteLine("HTTP/1.0 200 OK");
        //                writer.WriteLine("Content-type: text/xml");
        //                writer.WriteLine();
        //                writer.WriteLine(reply);
        //            }
        //        }
        //    }
        //}

        //public bool ProcessXmlRequest(XmlRpcRequest request, StreamWriter writer)
        //{
        //    XmlRpcResponse response = XmlRpcLoginMethod(request);

        //    // forward the XML-RPC response to the client
        //    writer.WriteLine("HTTP/1.0 200 OK");
        //    writer.WriteLine("Content-type: text/xml");
        //    writer.WriteLine();

        //    XmlTextWriter responseWriter = new XmlTextWriter(writer);
        //    XmlRpcResponseSerializer.Singleton.Serialize(responseWriter, response);
        //    responseWriter.Close();

        //    return true;
        //}

        public XmlRpcResponse XmlRpcLoginMethod(XmlRpcRequest request)
        {
            Hashtable requestData = (Hashtable)request.Params[0];
            string first;
            string last;
            string passwd;
            LLUUID Agent;
            LLUUID Session;

            XmlRpcResponse response = new XmlRpcResponse();

            //get login name
            if (requestData.Contains("first"))
            {
                first = (string)requestData["first"];
            }
            else
            {
                first = "test";
            }

            if (requestData.Contains("last"))
            {
                last = (string)requestData["last"];
            }
            else
            {
                last = "User" + NumClients.ToString();
            }

            if (requestData.Contains("passwd"))
            {
                passwd = (string)requestData["passwd"];
            }
            else
            {
                passwd = "notfound";
            }

            if (!Authenticate(first, last, passwd))
            {
                Hashtable loginError = new Hashtable();
                loginError["reason"] = "key"; ;
                loginError["message"] = "You have entered an invalid name/password combination. Check Caps/lock.";
                loginError["login"] = "false";
                response.Value = loginError;
            }
            else
            {
                NumClients++;

                //create a agent and session LLUUID
                Agent = GetAgentId(first, last);
                int SessionRand = Util.RandomClass.Next(1, 999);
                Session = new LLUUID("aaaabbbb-0200-" + SessionRand.ToString("0000") + "-8664-58f53e442797");
                LLUUID secureSess = LLUUID.Random();
                //create some login info
                Hashtable LoginFlagsHash = new Hashtable();
                LoginFlagsHash["daylight_savings"] = "N";
                LoginFlagsHash["stipend_since_login"] = "N";
                LoginFlagsHash["gendered"] = "Y";
                LoginFlagsHash["ever_logged_in"] = "Y";
                ArrayList LoginFlags = new ArrayList();
                LoginFlags.Add(LoginFlagsHash);

                Hashtable GlobalT = new Hashtable();
                GlobalT["sun_texture_id"] = "cce0f112-878f-4586-a2e2-a8f104bba271";
                GlobalT["cloud_texture_id"] = "fc4b9f0b-d008-45c6-96a4-01dd947ac621";
                GlobalT["moon_texture_id"] = "fc4b9f0b-d008-45c6-96a4-01dd947ac621";
                ArrayList GlobalTextures = new ArrayList();
                GlobalTextures.Add(GlobalT);

                response = (XmlRpcResponse)(new XmlRpcResponseDeserializer()).Deserialize(this._defaultResponse);
                Hashtable responseData = (Hashtable)response.Value;

                responseData["sim_port"] = m_simPort;
                responseData["sim_ip"] = m_simAddr;
                responseData["agent_id"] = Agent.ToStringHyphenated();
                responseData["session_id"] = Session.ToStringHyphenated();
                responseData["secure_session_id"] = secureSess.ToStringHyphenated();
                responseData["circuit_code"] = (Int32)(Util.RandomClass.Next());
                responseData["seconds_since_epoch"] = (Int32)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
                responseData["login-flags"] = LoginFlags;
                responseData["global-textures"] = GlobalTextures;

                //inventory
                ArrayList InventoryList = (ArrayList)responseData["inventory-skeleton"];
                Hashtable Inventory1 = (Hashtable)InventoryList[0];
                Hashtable Inventory2 = (Hashtable)InventoryList[1];
                LLUUID BaseFolderID = LLUUID.Random();
                LLUUID InventoryFolderID = LLUUID.Random();
                Inventory2["name"] = "Textures";
                Inventory2["folder_id"] = BaseFolderID.ToStringHyphenated();
                Inventory2["type_default"] = 0;
                Inventory1["folder_id"] = InventoryFolderID.ToStringHyphenated();

                ArrayList InventoryRoot = (ArrayList)responseData["inventory-root"];
                Hashtable Inventoryroot = (Hashtable)InventoryRoot[0];
                Inventoryroot["folder_id"] = InventoryFolderID.ToStringHyphenated();

                CustomiseLoginResponse(responseData, first, last);

                Login _login = new Login();
                //copy data to login object
                _login.First = first;
                _login.Last = last;
                _login.Agent = Agent;
                _login.Session = Session;
                _login.SecureSession = secureSess;
                _login.BaseFolder = BaseFolderID;
                _login.InventoryFolder = InventoryFolderID;

                //working on local computer if so lets add to the gridserver's list of sessions?
                if (m_gridServer.GetName() == "Local")
                {
                    ((LocalGridBase)m_gridServer).AddNewSession(_login);
                }
            }
            return response;
        }

        protected virtual void CustomiseLoginResponse(Hashtable responseData, string first, string last)
        {
        }

        protected virtual LLUUID GetAgentId(string firstName, string lastName)
        {
            LLUUID Agent;
            int AgentRand = Util.RandomClass.Next(1, 9999);
            Agent = new LLUUID("99998888-0100-" + AgentRand.ToString("0000") + "-8ec1-0b1d5cd6aead");
            return Agent;
        }

        protected virtual bool Authenticate(string first, string last, string passwd)
        {
            if (this._needPasswd)
            {
                //every user needs the password to login
                string encodedPass = passwd.Remove(0, 3); //remove $1$
                if (encodedPass == this._mpasswd)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                //do not need password to login
                return true;
            }
        }

        private static string EncodePassword(string passwd)
        {
            Byte[] originalBytes;
            Byte[] encodedBytes;
            MD5 md5;

            md5 = new MD5CryptoServiceProvider();
            originalBytes = ASCIIEncoding.Default.GetBytes(passwd);
            encodedBytes = md5.ComputeHash(originalBytes);

            return Regex.Replace(BitConverter.ToString(encodedBytes), "-", "").ToLower();
        }

        //IUserServer implementation
        public AgentInventory RequestAgentsInventory(LLUUID agentID)
        {
            AgentInventory aInventory = null;
            if (this.userAccounts)
            {
                aInventory = this.m_localUserManager.GetUsersInventory(agentID);
            }

            return aInventory;
        }

        public void SetServerInfo(string ServerUrl, string SendKey, string RecvKey)
        {

        }

    }


}
