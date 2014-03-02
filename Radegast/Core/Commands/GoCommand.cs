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
    public class GoCommand : RadegastCommand
    {
        Regex subCommand;
        RegexOptions regexOptions = RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase;
        TabsConsole TC { get { return Instance.TabConsole; } }
        ObjectsConsole Objects;
        ChatConsole Chat;
        Vector3 targetPos = Vector3.Zero;
        ConsoleWriteLine wl;

        public GoCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "go";
            Description = "Moves avatar";
            Usage = "go [tp] (distance|xyz|object|person|help) [additional args] (type \"" + CommandsManager.CmdPrefix + "go help\" for full usage)";

            subCommand = new Regex(@"(?<subcmd>.*)\s*=\s*(?<subarg>.*)", regexOptions);
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
            wl("Wrong arguments for \"go\" command. For detailed description type: " + CommandsManager.CmdPrefix + "go help");
        }

        void PrintFullUsage()
        {
            wl(@"Usage:

{0}go [tp] (distance|xyz|object|person|help) [additional args]
- tp is an optional parameter after go command. If speciefied use teleport instead of walking to reach the destination when parcel permits it.

Distance mode:
Specifies distance in meters to move with optional direction. If direction is not specified we move forward.
Examples:
{0}go 10 -- moves 10m forward
{0}go 15 e -- moves 15m to the east
{0}go tp 20 se -- teleports 20m to the southeast of our current position

XYZ mode:
Moves to X, Y and optionally Z coordinate
Examples:
{0}go xyz 128,128  -- walks toward the center of the sim, using our current elevation (Z)
{0}go tp xyz 32,32,128 -- teleports to postion 32,32,128 within our current region

Object mode:
Moves towards a named object
Examples:
{0}go object desk chair -- walk toward the closest object whose name begins with ""desk chair""
{0}go tp object dance floor -- teleports to the closest object whose name beings with ""dance floor""

Person mode:
Moves toward a person
Examples:
{0}go person Latif -- walk toward the closest person whose name begins with Latif
{0}go tp person John -- teleports to the closest person whose name begins with John", CommandsManager.CmdPrefix);
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

            bool useTP = false;
            if (args[0] == "tp")
            {
                useTP = true;
                args.RemoveAt(0);
            }

            if (args.Count == 0) { PrintUsage(); return; }

            string subcmd = args[0];
            args.RemoveAt(0);
            string subarg = string.Empty;

            // Move certain distance
            int distance = 0;
            if (int.TryParse(subcmd, out distance))
            {
                if (distance < 1) return;
                Quaternion heading = Client.Self.Movement.BodyRotation;
                KnownHeading kh = null;

                if (args.Count > 0)
                {
                    kh = StateManager.KnownHeadings.Find((KnownHeading h) => { return h.ID == args[0].ToUpper(); });
                    if (kh != null)
                        heading = kh.Heading;
                }

                targetPos = Client.Self.SimPosition + new Vector3((float)distance, 0f, 0f) * heading;
                Client.Self.Movement.BodyRotation = Client.Self.Movement.HeadRotation = heading;
                Client.Self.Movement.Camera.LookAt(Client.Self.SimPosition, targetPos);
                Client.Self.Movement.SendUpdate(true);
                WriteLine("Going {0} to {1:0},{2:0},{3:0}", kh == null ? string.Empty : kh.Name, targetPos.X, targetPos.Y, targetPos.Z);
                Instance.State.MoveTo(targetPos, useTP);
                return;
            }

            if (subcmd == "help")
            {
                PrintFullUsage();
                return;
            }

            if (args.Count == 0) { PrintUsage(); return; }
            subarg = string.Join(" ", args.ToArray());

            // Move towards
            switch (subcmd)
            {
                case "xyz":
                    string[] coords = Regex.Split(subarg, @"\D+");
                    if (coords.Length < 2) { PrintUsage(); return; }
                    int x = int.Parse(coords[0]);
                    int y = int.Parse(coords[1]);
                    int z = coords.Length > 2 ? int.Parse(coords[2]) : (int)Client.Self.SimPosition.Z;
                    targetPos = new Vector3(x, y, z);
                    WriteLine("Going to {0:0},{1:0},{2:0}", targetPos.X, targetPos.Y, targetPos.Z);
                    Instance.State.MoveTo(targetPos, useTP);
                    return;
                
                case "person":
                    List<UUID> people = Chat.GetAvatarList();
                    UUID person = people.Find((UUID id) => { return Instance.Names.Get(id).ToLower().StartsWith(subarg.ToLower()); });
                    if (person == UUID.Zero)
                    {
                        WriteLine("Could not find {0}", subarg);
                        return;
                    }
                    string pname = Instance.Names.Get(person);

                    targetPos = Vector3.Zero;
                    
                    if (!Instance.State.TryFindAvatar(person, out targetPos))
                    {
                        WriteLine("Could not locate {0}", pname);
                        return;
                    }

                    WriteLine("Going to {3} at {0:0},{1:0},{2:0}", targetPos.X, targetPos.Y, targetPos.Z, pname);
                    Instance.State.MoveTo(targetPos, useTP);

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
                        WriteLine("Objects list was not active. Started getting object names, please try again in a minute.");
                        TC.Tabs["chat"].Select();
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

                    WriteLine("Going to object '{0}' at {1:0},{2:0},{3:0}", target.Properties.Name, targetPos.X, targetPos.Y, targetPos.Z);
                    Instance.State.MoveTo(targetPos, useTP);
                    return;

                default:
                    WriteLine("Unrecognized go command {0}", subcmd);
                    return;
            }

        }
    }
}
