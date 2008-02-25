﻿/*
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
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using libsecondlife;
using Axiom.Math;
using OpenSim.Region.Environment.Interfaces;
using OpenSim.Region.Environment.Modules;
using OpenSim.Region.Environment.Scenes;
using OpenSim.Framework;
using OpenSim.Region.ScriptEngine.Common.ScriptEngineBase.AsyncCommandPlugins;

namespace OpenSim.Region.ScriptEngine.Common.ScriptEngineBase.AsyncCommandPlugins
{
    public class Listener
    {
        public AsyncCommandManager m_CmdManager;

        public Listener(AsyncCommandManager CmdManager)
        {
            m_CmdManager = CmdManager;
        }


        public void CheckListeners()
        {
            if (m_CmdManager.m_ScriptEngine.World == null)
                return;
            IWorldComm comms = m_CmdManager.m_ScriptEngine.World.RequestModuleInterface<IWorldComm>();

            if (comms != null)
            {
                while (comms.HasMessages())
                {
                    if (m_CmdManager.m_ScriptEngine.m_ScriptManager.GetScript(
                        comms.PeekNextMessageLocalID(), comms.PeekNextMessageItemID()) != null)
                    {
                        ListenerInfo lInfo = comms.GetNextMessage();

                        //Deliver data to prim's listen handler
                        object[] resobj = new object[]
                        {
                        //lInfo.GetChannel(), lInfo.GetName(), lInfo.GetID().ToString(), lInfo.GetMessage()
                            lInfo.GetChannel(), lInfo.GetName(), lInfo.GetSourceItemID().ToString(), lInfo.GetMessage()
                        };

                        m_CmdManager.m_ScriptEngine.m_EventQueueManager.AddToScriptQueue(
                            lInfo.GetLocalID(), lInfo.GetItemID(), "listen", EventQueueManager.llDetectNull, resobj
                        );
                    }
                }
            }
        }
    }
}
