/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

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
            get => nwp;
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
