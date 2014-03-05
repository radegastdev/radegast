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
using System.Timers;
using System.Collections.Generic;
using OpenMetaverse;

namespace Radegast
{
    public class AutoPilot2
    {
        #region Declarations
        /// <summary>
        /// Delegate to pass the coordinate value of next waypoint to an event-handler
        /// </summary>
        public delegate void WaypointDelegate(Vector3d waypoint);

        /// <summary>
        /// Delegate to pass the coordinate value of next waypoint and the new Status of AutoPilot to an event-handler
        /// </summary>
        public delegate void AutoPilotStatusDelegate(AutoPilotStatus newStatus, Vector3d nextWaypoint);

        /// <summary>
        /// Enum declaration representing the statuses of AutoPilot
        /// </summary>
        public enum AutoPilotStatus
        {
            Idle,
            Paused,
            Moving,
            Cancelled,
            Finished,
            Failed
        }
        #endregion Declarations

        #region Private Variables
        private GridClient Client;
        private Vector3d myGlobalPosition;
        private List<Vector3d> waypoints = new List<Vector3d>();
        private int waypointIndex = 0;
        private AutoPilotStatus status = AutoPilotStatus.Idle;
        private double waypointRadius = 2d;
        private Timer ticker = new Timer(500);
        private int stuckTimeout = 10000;
        private int lastDistance = 0;
        private int lastDistanceChanged = -1;
        #endregion Private Variables

        #region Public Variables/Properties
        public bool Loop = false;

        /// <summary>
        /// The Status of the AutoPilot instance
        /// </summary>
        public AutoPilotStatus Status
        {
            get
            {
                return status;
            }
        }

        /// <summary>
        /// The Vector3d Waypoints in the AutoPilot instance
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Must have at least 2 Waypoints</exception>
        public List<Vector3d> Waypoints
        {
            get
            {
                return waypoints;
            }
            set
            {
                if (value.Count > 1)
                {
                    Stop(AutoPilotStatus.Idle);
                    waypoints = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Waypoints", "Must have at least 2 Waypoints");
                }
            }
        }

        /// <summary>
        /// The previous Vector3d Waypoint along the path. Returns Vector3d.Zero if there is no previous waypoint.
        /// </summary>
        public Vector3d PreviousWaypoint
        {
            get
            {
                return waypointIndex >= 1 ? waypoints[waypointIndex - 1] : Vector3d.Zero;
            }
        }

        /// <summary>
        /// The next Vector3d Waypoint along the path. Returns Vector3d.Zero if there is no next waypoint.
        /// </summary>
        public Vector3d NextWaypoint
        {
            get
            {
                return waypointIndex < waypoints.Count ? waypoints[waypointIndex] : Vector3d.Zero;
            }
        }

        /// <summary>
        /// The next Waypoint's index. A new value will immediately take effect if AutoPilot is not Idle
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">NextWaypointIndex must be greater than or equal to 0 and less than the number of Waypoints</exception>
        /// <exception cref="Exception">Must have at least 2 Waypoints</exception>
        public int NextWaypointIndex
        {
            set
            {
                if (waypoints.Count > 1)
                {
                    if (Loop)
                    {
                        waypointIndex = value % waypoints.Count;
                    }
                    else if (value < waypoints.Count && value >= 0)
                    {
                        waypointIndex = value;
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException("NextWaypointIndex", "Value must be greater than or equal to 0 and less than the number of Waypoints");
                    }
                    if (status != AutoPilotStatus.Idle)
                    {
                        Client.Self.AutoPilotCancel();
                        SetStatus(AutoPilotStatus.Moving);
                        Client.Self.AutoPilot(waypoints[waypointIndex].X, waypoints[waypointIndex].Y, waypoints[waypointIndex].Z);
                    }
                }
                else
                {
                    throw new Exception("Must have at least 2 Waypoints");
                }
            }
            get
            {
                return waypointIndex;
            }
        }

        /// <summary>
        /// The next Waypoint's index
        /// </summary>
        public bool NextWaypointIsFinal
        {
            get
            {
                return waypointIndex == (waypoints.Count - 1);
            }
        }

        /// <summary>
        /// Returns true if next Waypoint is the Start
        /// </summary>
        public bool NextWaypointIsStart
        {
            get
            {
                return waypointIndex == 0 && waypoints.Count > 1;
            }
        }

        /// <summary>
        /// The Waypoint detection radius
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">WaypointRadius must be greater than 0</exception>
        public double WaypointRadius
        {
            get
            {
                return waypointRadius;
            }
            set
            {
                if (value > 0)
                {
                    waypointRadius = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("WaypointRadius", "Value must be greater than 0");
                }
            }
        }

        /// <summary>
        /// The timeout in milliseconds before being considered stuck
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">StuckTimeout must be greater than 0</exception>
        public int StuckTimeout
        {
            get
            {
                return stuckTimeout;
            }
            set
            {
                if (value > 0)
                {
                    stuckTimeout = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("StuckTimeout", "Value must be greater than 0");
                }
            }
        }
        #endregion Public Variables/Properties

        #region Public Events
        /// <summary>
        /// Event for when agent arrives at a Waypoint
        /// </summary>
        public event WaypointDelegate OnWaypointArrival;

        /// <summary>
        /// Event for when AutoPilot's status changes
        /// </summary>
        public event AutoPilotStatusDelegate OnStatusChange;
        #endregion Public Events

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The GridClient to use</param>
        public AutoPilot2(GridClient client)
        {
            Client = client;
            Client.Objects.TerseObjectUpdate += new System.EventHandler<TerseObjectUpdateEventArgs>(Objects_TerseObjectUpdate);
            ticker.Elapsed += new ElapsedEventHandler(ticker_Elapsed);
        }

        #region Public Methods
        /// <summary>
        /// Triggers Autopilot to move towards next waypoint along the path
        /// </summary>
        /// <returns>Next Waypoint index</returns>
        public int MoveToNextWaypoint()
        {
            return MoveToNextWaypoint(true);
        }

        /// <summary>
        /// Triggers Autopilot to move towards next waypoint along the path
        /// </summary>
        /// <param name="increment">Increment current Waypoint index</param>
        /// <exception cref="Exception">Must have at least 2 Waypoints</exception>
        /// <returns>Next Waypoint index</returns>
        public int MoveToNextWaypoint(bool increment)
        {
            if (waypoints.Count > 1)
            {
                if (increment)
                {
                    NextWaypointIndex++;
                }
                else
                {
                    Client.Self.AutoPilotCancel();
                    Vector3d nextWaypoint = NextWaypoint;
                    SetStatus(AutoPilotStatus.Moving);
                    Client.Self.AutoPilot(nextWaypoint.X, nextWaypoint.Y, nextWaypoint.Z);
                }
                return waypointIndex;
            }
            else
            {
                throw new Exception("Must have at least 2 Waypoints");
            }
        }

        /// <summary>
        /// Starts AutoPilot from an Idle state. Will trigger a Moving status change.
        /// </summary>
        /// <exception cref="Exception">Must have at least 2 Waypoints</exception>
        /// <exception cref="Exception">Status must be Idle</exception>
        /// <returns>Next Waypoint index</returns>
        public int Start()
        {
            if (waypoints.Count > 1)
            {
                if (status == AutoPilotStatus.Idle)
                {
                    ticker.Start();
                    return MoveToNextWaypoint(false);
                }
                else
                {
                    throw new Exception("Status must be Idle");
                }
            }
            else
            {
                throw new Exception("Must have at least 2 Waypoints");
            }
        }

        /// <summary>
        /// Restarts AutoPilot. Will trigger a Cancel status change if not already cancelled.
        /// </summary>
        /// <exception cref="Exception">Must have at least 2 Waypoints</exception>
        /// <returns>Next Waypoint index</returns>
        public int Restart()
        {
            if (waypoints.Count > 1)
            {
                Stop();
                lastDistanceChanged = -1;
                return MoveToNextWaypoint(false);
            }
            else
            {
                throw new Exception("Must have at least 2 Waypoints");
            }
        }

        /// <summary>
        /// Pauses AutoPilot from a Moving state. Will trigger a Paused status change.
        /// </summary>
        /// <exception cref="Exception">Status must be Moving</exception>
        /// <returns>Next Waypoint index</returns>
        public int Pause()
        {
            if (status == AutoPilotStatus.Moving)
            {
                ticker.Stop();
                Client.Self.AutoPilotCancel();
                SetStatus(AutoPilotStatus.Paused);
                return waypointIndex;
            }
            else
            {
                throw new Exception("Status must be Moving");
            }
        }

        /// <summary>
        /// Resumes AutoPilot from a Paused state. Will trigger a Moving status change.
        /// </summary>
        /// <exception cref="Exception">Status must be Paused</exception>
        /// <returns>Next Waypoint index</returns>
        public int Resume()
        {
            if (status == AutoPilotStatus.Paused)
            {
                ticker.Start();
                return MoveToNextWaypoint(false);
            }
            else
            {
                throw new Exception("Status must be Paused");
            }
        }

        /// <summary>
        /// Stops AutoPilot. Will trigger a Cancel status change if not already cancelled.
        /// </summary>
        public void Stop()
        {
            Stop(AutoPilotStatus.Cancelled);
        }

        /// <summary>
        /// Stops AutoPilot. Will trigger the given status if not already in that state.
        /// </summary>
        /// <param name="newStatus">The new status for AutoPilot. Cannot be Moving</param>
        /// <exception cref="ArgumentOutOfRangeException">newStatus cannot be Moving</exception>
        public void Stop(AutoPilotStatus newStatus)
        {
            if (newStatus != AutoPilotStatus.Moving)
            {
                ticker.Stop();
                Client.Self.AutoPilotCancel();
                SetStatus(newStatus);
                lastDistanceChanged = -1;
                waypointIndex = 0;
            }
            else
            {
                throw new ArgumentOutOfRangeException("newStatus", "Value cannot be Moving");
            }
        }
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// Sets AutoPilot's Status. If newStatus is different from current Status than it will cause OnStatusChange event trigger.
        /// </summary>
        /// <param name="newStatus">The new Status AutoPilot is to be changed to</param>
        /// <returns>True if OnStatusChanged triggered</returns>
        private bool SetStatus(AutoPilotStatus newStatus)
        {
            AutoPilotStatus oldStatus = status;
            if (oldStatus != newStatus)
            {
                status = newStatus;
                if (OnStatusChange != null)
                {
                    OnStatusChange(status, NextWaypoint);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Event Handler for waypoint distance checking
        /// </summary>
        private void Objects_TerseObjectUpdate(object sender, TerseObjectUpdateEventArgs e)
        {
            if (status == AutoPilotStatus.Moving && e.Update.Avatar && e.Update.LocalID == Client.Self.LocalID)
            {
                uint regionX, regionY;
                Utils.LongToUInts(e.Simulator.Handle, out regionX, out regionY);
                myGlobalPosition = new Vector3d(
                    regionX + e.Update.Position.X,
                    regionY + e.Update.Position.Y,
                    e.Update.Position.Z
                );
                if (Vector3d.Distance(myGlobalPosition, NextWaypoint) <= waypointRadius)
                {
                    if (NextWaypointIsFinal && !Loop)
                    {
                        Stop(AutoPilotStatus.Finished);
                    }
                    else
                    {
                        if (OnWaypointArrival != null)
                        {
                            OnWaypointArrival(NextWaypoint);
                        }
                        MoveToNextWaypoint();
                    }
                }
            }
        }

        /// <summary>
        /// Event Handler for Timer which detects if agent is stuck
        /// </summary>
        private void ticker_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (status == AutoPilotStatus.Moving)
            {
                int distance = (int)Vector3d.Distance(myGlobalPosition, NextWaypoint);
                if (distance != lastDistance || lastDistanceChanged < 0)
                {
                    lastDistance = distance;
                    lastDistanceChanged = System.Environment.TickCount;
                }
                else if (Math.Abs(System.Environment.TickCount - lastDistanceChanged) > stuckTimeout)
                {
                    Vector3d nextWaypoint = NextWaypoint;
                    Stop(AutoPilotStatus.Failed);
                }
            }
        }
        #endregion Private Methods
    }
}