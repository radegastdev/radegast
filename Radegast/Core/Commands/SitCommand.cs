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
    public class SitCommand : RadegastCommand
    {
        TabsConsole TC { get { return Instance.TabConsole; } }
        ObjectsConsole Objects;
        ChatConsole Chat;
        ConsoleWriteLine wl;

        public SitCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "sit";
            Description = "Sits on an object or on the ground";
            Usage = "sit (object name|ground)";

            Chat = (ChatConsole)TC.Tabs["chat"].Control;
        }

        public override void Dispose()
        {
            Objects = null;
            Chat = null;
            base.Dispose();
        }

        void PrintUsage()
        {
            wl("Wrong arguments for \"sit\" command. Use {0}{1}", CommandsManager.CmdPrefix, Usage);
        }


        public override void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            if (Chat.InvokeRequired)
            {
                if (!Instance.MonoRuntime || Chat.IsHandleCreated)
                    Chat.Invoke(new MethodInvoker(() => Execute(name, cmdArgs, WriteLine)));
                return;
            }
            wl = WriteLine;

            if (cmdArgs.Length == 0) { PrintUsage(); return; }

            string cmd = string.Join(" ", cmdArgs);

            if (cmd == "ground")
            {
                Client.Self.SitOnGround();
                wl("Sat on the ground");
                return;
            }

            if (!TC.TabExists("objects"))
            {
                RadegastTab tab = TC.AddTab("objects", "Objects", new ObjectsConsole(Instance));
                tab.AllowClose = true;
                tab.AllowDetach = true;
                tab.Visible = true;
                tab.AllowHide = false;
                ((ObjectsConsole)tab.Control).RefreshObjectList();
                TC.Tabs["chat"].Select();

                WriteLine("Objects list was not active. Started getting object names, please try again in a minute.");
                return;
            }

            Objects = (ObjectsConsole)TC.Tabs["objects"].Control;
            List<Primitive> prims = Objects.GetObjectList();

            Primitive target = prims.Find((Primitive prim) =>
            {
                return prim.Properties != null
                    && prim.Properties.Name.ToLower().Contains(cmd.ToLower());
            });

            if (target == null)
            {
                WriteLine("Could not find '{0}' nearby", cmd);
                return;
            }

            WriteLine("Requesting to sit on object '{0}'", target.Properties.Name);
            Instance.State.SetSitting(true, target.ID);
        }
    }
}
