// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
        bool displayEndWalk = false;
        Vector3 targetPos = Vector3.Zero;

        public GoCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "go";
            Description = "Moves avatar";
            Usage = Name;

            subCommand = new Regex(@"(?<subcmd>.*)\s*=\s*(?<subarg>.*)", regexOptions);
            Chat = (ChatConsole)TC.Tabs["chat"].Control;
            Instance.State.OnWalkStateCanged += new StateManager.WalkStateCanged(State_OnWalkStateCanged);
        }

        public override void Dispose()
        {
            Objects = null;
            Chat = null;
            base.Dispose();
        }

        void State_OnWalkStateCanged(bool walking)
        {
            if (!walking && displayEndWalk)
            {
                displayEndWalk = false;
                Vector3 p = Client.Self.SimPosition;
                string msg = "Finished walking";

                if (targetPos != Vector3.Zero)
                {
                    System.Threading.Thread.Sleep(1000);
                    msg += string.Format(" {0:0} meters from destination", Vector3.Distance(Client.Self.SimPosition, targetPos));
                    targetPos = Vector3.Zero;
                }

                TC.DisplayNotificationInChat(msg);
            }
        }

        void PrintUsage()
        {
        }

        void MoveTo(Vector3 target, bool useTP)
        {
            Instance.State.SetSitting(false, UUID.Zero);

            if (useTP)
            {
                Client.Self.RequestTeleport(Client.Network.CurrentSim.Handle, targetPos);
            }
            else
            {
                displayEndWalk = true;
                Client.Self.Movement.TurnToward(targetPos);
                Instance.State.WalkTo(Instance.State.GlobalPosition(Client.Network.CurrentSim, target));
            }
        }

        public override void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            Client.Self.Movement.UpdateFromHeading(0.0, true);
            if (Chat.InvokeRequired)
            {
                if (Chat.IsHandleCreated)
                    Chat.Invoke(new MethodInvoker(() => Execute(name, cmdArgs, WriteLine)));
                return;
            }

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
                    kh = Instance.State.KnownHeadings.Find((KnownHeading h) => { return h.ID == args[0].ToUpper(); });
                    if (kh != null)
                        heading = kh.Heading;
                }

                targetPos = Client.Self.SimPosition + new Vector3((float)distance, 0f, 0f) * heading;
                Client.Self.Movement.BodyRotation = Client.Self.Movement.HeadRotation = heading;
                Client.Self.Movement.Camera.LookAt(Client.Self.SimPosition, targetPos);
                Client.Self.Movement.SendUpdate(true);
                WriteLine("Going {0} to {1:0},{2:0},{3:0}", kh == null ? string.Empty : kh.Name, targetPos.X, targetPos.Y, targetPos.Z);
                MoveTo(targetPos, useTP);
                return;
            }

            if (args.Count == 0) { PrintUsage(); return; }
            subarg = string.Join(" ", args.ToArray());

            // Move towards
            switch (subcmd)
            {
                case "name":
                    break;

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
                                && prim.Properties.Name.ToLower().StartsWith(subarg.ToLower());
                        });

                    if (target == null)
                    {
                        WriteLine("Could not find '{0}' nearby", subarg);
                        return;
                    }

                    targetPos = target.Position;

                    WriteLine("Going to object '{0}' at {1:0},{2:0},{3:0}", target.Properties.Name, targetPos.X, targetPos.Y, targetPos.Z);
                    MoveTo(targetPos, useTP);
                    return;

                default:
                    WriteLine("Unrecognized go command {0}", subcmd);
                    return;
            }

        }
    }
}
