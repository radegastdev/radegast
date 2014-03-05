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
using System.Collections.Generic;
using OpenMetaverse;

namespace Radegast
{
    class AutoPilot
    {
        GridClient Client;
        List<Vector3> Waypoints = new List<Vector3>();
        System.Timers.Timer Ticker = new System.Timers.Timer(500);
        Vector3 mypos;
        int nwp = 0;

        int NextWaypoint
        {
            set
            {
                if (value == Waypoints.Count) {
                    nwp = 0;
                } else {
                    nwp = value;
                }
                System.Console.WriteLine("Way point " + nwp + " " + Waypoints[nwp]);
                Client.Self.AutoPilotCancel();
                Client.Self.Movement.TurnToward(Waypoints[nwp]);
                System.Threading.Thread.Sleep(500);
                Client.Self.AutoPilotLocal((int)Waypoints[nwp].X, (int)Waypoints[nwp].Y, (int)Waypoints[nwp].Z);
            }
            get
            {
                return nwp;
            }
        }

        public AutoPilot(GridClient client)
        {
            Client = client;
            Ticker.Enabled = false;
            Ticker.Elapsed += new System.Timers.ElapsedEventHandler(Ticker_Elapsed);
            Client.Objects.TerseObjectUpdate += new System.EventHandler<TerseObjectUpdateEventArgs>(Objects_TerseObjectUpdate);
        }

        void Objects_TerseObjectUpdate(object sender, TerseObjectUpdateEventArgs e)
        {
            if (e.Update.Avatar && e.Update.LocalID == Client.Self.LocalID) {
                mypos = e.Update.Position;
                if (Vector3.Distance(mypos, Waypoints[NextWaypoint]) < 2f) {
                    NextWaypoint++;
                }
            }
        }

        void Ticker_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
        }

        public int InsertWaypoint(Vector3 wp)
        {
            Waypoints.Add(wp);
            return Waypoints.Count;
        }

        public void Start()
        {
            if (Waypoints.Count < 2) {
                return;
            }

            //Client.Self.Teleport(Client.Network.CurrentSim.Handle, Waypoints[0]);
            NextWaypoint++;
        }

        public void Stop()
        {
            Client.Self.AutoPilotCancel();
        }
    }
}
