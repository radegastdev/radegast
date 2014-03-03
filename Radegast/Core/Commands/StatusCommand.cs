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
using System.Windows.Forms;
using System.Threading;
using OpenMetaverse;

namespace Radegast.Commands
{
    public class StatusCommand : RadegastCommand
    {
        private RadegastInstance instance;
        private List<string> args;

        public StatusCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "status";
            Description = "Prints various status infromation";
            Usage = "status (doing|region|parcel|money|location|time)";

            this.instance = instance;
            args = new List<string>();
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        bool arg(string a)
        {
            return args.Count == 0 || args.Contains(a);
        }

        public override void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            args.Clear();
            args.AddRange(cmdArgs);
            StringBuilder sb = new StringBuilder();

            if (arg("doing"))
            {
                if (Client.Self.SittingOn != 0)
                {
                    sb.Append("Sitting");
                    Primitive seat;
                    if (Client.Network.CurrentSim.ObjectsPrimitives.TryGetValue(Client.Self.SittingOn, out seat))
                    {
                        if (seat.Properties != null)
                        {
                            sb.AppendFormat(" on object {0}", seat.Properties.Name);
                        }
                        else
                        {
                            ManualResetEvent gotName = new ManualResetEvent(false);
                            EventHandler<ObjectPropertiesFamilyEventArgs> handler = (object sender, ObjectPropertiesFamilyEventArgs e) =>
                            {
                                if (e.Properties.ObjectID == seat.ID)
                                    gotName.Set();
                            };

                            Client.Objects.ObjectPropertiesFamily += handler;
                            Client.Objects.RequestObjectPropertiesFamily(Client.Network.CurrentSim, seat.ID);
                            if (gotName.WaitOne(3000, false))
                            {
                                sb.Append(sb.AppendFormat(" on object {0}", seat.Properties.Name));
                            }

                            Client.Objects.ObjectPropertiesFamily -= handler;
                        }
                    }
                }
                else
                {
                    if (Client.Self.Movement.SitOnGround)
                        sb.Append("Sitting on the ground");
                    else if (Client.Self.Movement.Fly)
                        sb.Append("Flying");
                    else
                        sb.Append("Standing");
                    if (Client.Self.Movement.AlwaysRun)
                        sb.Append(", always running when moving");
                }
                
                if (Instance.State.FollowName != string.Empty)
                    sb.AppendFormat(", following {0}", Instance.State.FollowName);

                sb.AppendLine();
            }

            if (arg("region"))
            {
                sb.AppendLine(string.Format("Region: {0} ({1})",
                    Client.Network.CurrentSim.Name,
                    Utils.EnumToText(Client.Network.CurrentSim.Access)));
            }

            if (arg("parcel") && Instance.State.Parcel != null)
            {
                Parcel parcel = Instance.State.Parcel;
                sb.AppendFormat(@"Parcel: {0}", parcel.Name);
                List<string> mods = new List<string>();

                if ((parcel.Flags & ParcelFlags.AllowFly) != ParcelFlags.AllowFly)
                    mods.Add("no fly");
                if ((parcel.Flags & ParcelFlags.CreateObjects) != ParcelFlags.CreateObjects)
                    mods.Add("no build");
                if ((parcel.Flags & ParcelFlags.AllowOtherScripts) != ParcelFlags.AllowOtherScripts)
                    mods.Add("no scripts");
                if ((parcel.Flags & ParcelFlags.RestrictPushObject) == ParcelFlags.RestrictPushObject)
                    mods.Add("no push");
                if ((parcel.Flags & ParcelFlags.AllowDamage) == ParcelFlags.AllowDamage)
                    mods.Add("damage allowed");
                if ((parcel.Flags & ParcelFlags.AllowVoiceChat) != ParcelFlags.AllowVoiceChat)
                    mods.Add("no voice");

                if (mods.Count > 0)
                    sb.AppendFormat(" ({0})", string.Join(", ", mods.ToArray()));
                sb.AppendLine();
            }

            if (arg("location"))
            {
                Primitive seat = null;
                if (Client.Self.SittingOn != 0)
                    Client.Network.CurrentSim.ObjectsPrimitives.TryGetValue(Client.Self.SittingOn, out seat);

                Vector3 pos = Client.Self.RelativePosition;
                if (seat != null) pos = seat.Position + pos;

                sb.AppendFormat("Position <{0:0}, {1:0}, {2:0}>", pos.X, pos.Y, pos.Z);

                Quaternion rot;
                if (seat == null)
                    rot = Client.Self.Movement.BodyRotation;
                else
                    rot = seat.Rotation + Client.Self.RelativeRotation;

                Vector3 heading = StateManager.RotToEuler(rot);
                int facing = (int)(57.2957795d * heading.Z);
                if (facing < 0) facing = 360 + facing;
                sb.AppendFormat(" heading {0:0} degrees, ", facing);
                sb.AppendFormat(" facing {0}", StateManager.ClosestKnownHeading(facing).Name);
                sb.AppendLine();
            }

            if (arg("money"))
            {
                sb.AppendFormat("Account balance: ${0}", Client.Self.Balance);
                sb.AppendLine();
            }

            if (arg("time"))
            {
                try
                {
                    TimeZoneInfo SLTime = null;
                    foreach (TimeZoneInfo tz in TimeZoneInfo.GetSystemTimeZones())
                    {
                        if (tz.Id == "Pacific Standard Time" || tz.Id == "America/Los_Angeles")
                        {
                            SLTime = tz;
                            break;
                        }
                    }

                    DateTime now;
                    if (SLTime != null)
                        now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, SLTime);
                    else
                        now = DateTime.UtcNow.AddHours(-7);

                    sb.AppendLine(string.Format("Current time is " + now.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture)));
                }
                catch { }
            }

            WriteLine(sb.ToString());
        }
    }
}
