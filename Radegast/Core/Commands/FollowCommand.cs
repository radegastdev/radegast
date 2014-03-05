// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id$
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using OpenMetaverse;

namespace Radegast.Commands
{
    public class FollowCommand : RadegastCommand
    {
        TabsConsole TC { get { return Instance.TabConsole; } }
        ChatConsole Chat;

        public FollowCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "follow";
            Description = "Follows a person";
            Usage = "follow (person name|stop)";

            Chat = (ChatConsole)TC.Tabs["chat"].Control;
        }

        public override void Dispose()
        {
            Chat = null;
            base.Dispose();
        }

        public override void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            if (Chat.InvokeRequired)
            {
                if (!Instance.MonoRuntime || Chat.IsHandleCreated)
                    Chat.Invoke(new MethodInvoker(() => Execute(name, cmdArgs, WriteLine)));
                return;
            }

            if (cmdArgs.Length == 0)
            {
                if (Instance.State.FollowName == string.Empty)
                    WriteLine("Follow inactive, type {0}follow person name, to start following them", CommandsManager.CmdPrefix);
                else
                    WriteLine("Following {0}", Instance.State.FollowName);
                return;
            }

            string cmd = string.Join(" ", cmdArgs);

            if (cmd == "stop")
            {
                Instance.State.StopFollowing();
                WriteLine("Following stopped");
                return;
            }

            List<UUID> people = Chat.GetAvatarList();
            UUID person = people.Find((UUID id) => { return Instance.Names.Get(id).ToLower().StartsWith(cmd.ToLower()); });
            if (person == UUID.Zero)
            {
                WriteLine("Could not find {0}", cmd);
                return;
            }

            Instance.State.Follow(Instance.Names.Get(person), person);
            WriteLine("Following {0}", Instance.State.FollowName);
        }
    }
}
