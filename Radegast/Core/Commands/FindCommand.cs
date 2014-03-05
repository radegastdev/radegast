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
    public class FindCommand : RadegastCommand
    {
        TabsConsole TC { get { return Instance.TabConsole; } }
        ObjectsConsole Objects;
        ChatConsole Chat;
        ConsoleWriteLine wl;

        public FindCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "find";
            Description = "Finds nearby person or object";
            Usage = "find (object|person) name";

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
            wl("Wrong arguments for \"find\" command. Use {0}{1}", CommandsManager.CmdPrefix, Usage);
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
            List<string> args = new List<string>(Regex.Split(cmd, @"\s"));

            string subcmd = args[0];
            args.RemoveAt(0);
            if (args.Count == 0) { PrintUsage(); return; }
            string subarg = string.Join(" ", args.ToArray());

            Primitive seat = null;
            if (Client.Self.SittingOn != 0)
                Client.Network.CurrentSim.ObjectsPrimitives.TryGetValue(Client.Self.SittingOn, out seat);

            Vector3 mypos = Client.Self.RelativePosition;
            if (seat != null) mypos = seat.Position + mypos;
            StringBuilder sb = new StringBuilder();

            if (subcmd == "object")
            {
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

                List<Primitive> targets = prims.FindAll((Primitive prim) =>
                {
                    return prim.Properties != null
                        && prim.Properties.Name.ToLower().Contains(subarg.ToLower());
                });

                if (targets == null || targets.Count == 0)
                {
                    WriteLine("Could not find '{0}' nearby", subarg);
                    return;
                }

                foreach (Primitive target in targets)
                {
                    Vector3 heading = StateManager.RotToEuler(Vector3.RotationBetween(Vector3.UnitX, Vector3.Normalize(target.Position - mypos)));
                    int facing = (int)(57.2957795d * heading.Z);
                    if (facing < 0) facing = 360 + facing;

                    sb.AppendFormat("{0} is {1:0} meters away to the {2}",
                        target.Properties.Name,
                        Vector3.Distance(mypos, target.Position),
                        StateManager.ClosestKnownHeading(facing)
                        );

                    float elev = target.Position.Z - mypos.Z;
                    if (Math.Abs(elev) < 2f)
                        sb.Append(" at our level");
                    else if (elev > 0)
                        sb.AppendFormat(", {0:0} meters above our level", elev);
                    else
                        sb.AppendFormat(", {0:0} meters below our level", -elev);

                    sb.AppendLine();
                }

                wl(sb.ToString());

                return;
            }

            if (subcmd == "person")
            {
                List<UUID> people = Chat.GetAvatarList();
                people = people.FindAll((UUID id) => { return id != Client.Self.AgentID && Instance.Names.Get(id).ToLower().StartsWith(subarg.ToLower()); });
                if (people == null || people.Count == 0)
                {
                    WriteLine("Could not find {0}", subarg);
                    return;
                }

                foreach (UUID person in people)
                {
                    string pname = Instance.Names.Get(person);

                    Vector3 targetPos = Vector3.Zero;

                    // try to find where they are
                    Avatar avi = Client.Network.CurrentSim.ObjectsAvatars.Find((Avatar av) => { return av.ID == person; });

                    if (avi != null)
                    {
                        if (avi.ParentID == 0)
                        {
                            targetPos = avi.Position;
                        }
                        else
                        {
                            Primitive theirSeat;
                            if (Client.Network.CurrentSim.ObjectsPrimitives.TryGetValue(avi.ParentID, out theirSeat))
                            {
                                targetPos = theirSeat.Position + avi.Position;
                            }
                        }
                    }
                    else
                    {
                        if (Client.Network.CurrentSim.AvatarPositions.ContainsKey(person))
                            targetPos = Client.Network.CurrentSim.AvatarPositions[person];
                    }

                    if (targetPos.Z < 0.01f)
                    {
                        WriteLine("Could not locate {0}", pname);
                        return;
                    }

                    Vector3 heading = StateManager.RotToEuler(Vector3.RotationBetween(Vector3.UnitX, Vector3.Normalize(targetPos - mypos)));
                    int facing = (int)(57.2957795d * heading.Z);
                    if (facing < 0) facing = 360 + facing;

                    sb.AppendFormat("{0} is {1:0} meters away to the {2}",
                        pname,
                        Vector3.Distance(mypos, targetPos),
                        StateManager.ClosestKnownHeading(facing)
                        );

                    float elev = targetPos.Z - mypos.Z;
                    if (Math.Abs(elev) < 2f)
                        sb.Append(" at our level");
                    else if (elev > 0)
                        sb.AppendFormat(", {0:0} meters above our level", elev);
                    else
                        sb.AppendFormat(", {0:0} meters below our level", -elev);

                    sb.AppendLine();
                }

                wl(sb.ToString());
                return;
            }
        }
    }
}
