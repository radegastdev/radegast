using System;
using System.Collections.Generic;
using System.Text;
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
            Client.Objects.OnObjectUpdated += new ObjectManager.ObjectUpdatedCallback(Objects_OnObjectUpdated);
        }

        void Objects_OnObjectUpdated(Simulator simulator, ObjectUpdate update, ulong regionHandle, ushort timeDilation)
        {
            if (update.Avatar && update.LocalID == Client.Self.LocalID) {
                mypos = update.Position;
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
