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
    public class FaceCommand : RadegastCommand
    {
        TabsConsole TC { get { return Instance.TabConsole; } }
        ObjectsConsole Objects;
        ChatConsole Chat;
        Vector3 targetPos = Vector3.Zero;
        RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;
        ConsoleWriteLine wl;

        public FaceCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "face";
            Description = "Changes the direction";
            Usage = "face (direction|heading|object|person|help) [additional args] (type \"" + CommandsManager.CmdPrefix + "face help\" for full usage)";

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
            wl("Wrong arguments for \"face\" command. For detailed description type: " + CommandsManager.CmdPrefix + "face help");
        }

        void PrintFullUsage()
        {
            wl(@"Usage:
{0}face (direction|heading|object|person|help) [additional args]

Direction (in degrees) mode:
Specifies heading in degrees which we should be oriented in
Examples:
{0}face 0 -- face east
{0}face 180 -- face west

Heading mode:
Specifies compass heading we should be facing
Examples:
{0}face n -- face east
{0}face sw -- face southwest

Object mode:
Turns toward a named object
Example:
{0}face object desk chair -- turn toward object whose names begins with ""desk chair""

Person mode:
Turns toward a person
Examples:
{0}face person Latif -- turn toward the closest person whose name begins with Latif", CommandsManager.CmdPrefix);
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

            string cmd = string.Join(" ", cmdArgs);
            List<string> args = new List<string>(Regex.Split(cmd, @"\s", regexOptions));

            if (args.Count == 0) { PrintUsage(); return; }

            string subcmd = args[0];
            args.RemoveAt(0);
            string subarg = string.Empty;

            // Face certain direction
            int heading = 0;
            if (int.TryParse(subcmd, out heading))
            {
                double rad = 0.0174532925d * heading;
                Client.Self.Movement.UpdateFromHeading(rad, true);
                WriteLine("Facing {0} degrees", heading % 360);
                return;
            }

            if (subcmd == "help")
            {
                PrintFullUsage();
                return;
            }

            KnownHeading kh = null;
            kh = StateManager.KnownHeadings.Find((KnownHeading h) => { return h.ID == subcmd.ToUpper(); });
            if (kh != null)
            {
                Client.Self.Movement.BodyRotation = Client.Self.Movement.HeadRotation = kh.Heading;
                WriteLine("Facing {0}", kh.Name);
                return;
            }

            if (args.Count == 0) { PrintUsage(); return; }
            subarg = string.Join(" ", args.ToArray());

            // Move towards
            switch (subcmd)
            {
                case "person":
                    List<UUID> people = Chat.GetAvatarList();
                    UUID person = people.Find((UUID id) => { return Instance.Names.Get(id).ToLower().StartsWith(subarg.ToLower()); });
                    if (person == UUID.Zero)
                    {
                        WriteLine("Could not find {0}", subarg);
                        return;
                    }
                    string pname = Instance.Names.Get(person);

                    if (!Instance.State.TryFindAvatar(person, out targetPos))
                    {
                        WriteLine("Could not locate {0}", pname);
                        return;
                    }

                    WriteLine("Facing {0}", pname);
                    Client.Self.Movement.TurnToward(targetPos);

                    return;

                case "object":

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
                            && prim.Properties.Name.ToLower().Contains(subarg.ToLower());
                    });

                    if (target == null)
                    {
                        WriteLine("Could not find '{0}' nearby", subarg);
                        return;
                    }

                    targetPos = target.Position;

                    WriteLine("Facing object '{0}'", target.Properties.Name);
                    Client.Self.Movement.TurnToward(targetPos);
                    return;

                default:
                    WriteLine("Unrecognized face command {0}", subcmd);
                    return;
            }
        }
    }
}
