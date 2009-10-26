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
using System.Timers;
using System.Threading;
using OpenMetaverse;
using Radegast.Netcom;

namespace Radegast
{
    public class StateManager : IDisposable
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private RadegastNetcom netcom { get { return instance.Netcom; } }

        private bool typing = false;
        private bool away = false;
        private bool busy = false;
        private bool flying = false;
        private bool alwaysrun = false;
        private bool sitting = false;
        
        private bool following = false;
        private string followName = string.Empty;
        private float followDistance = 3.0f;

        private UUID awayAnimationID = new UUID("fd037134-85d4-f241-72c6-4f42164fedee");
        private UUID busyAnimationID = new UUID("efcf670c2d188128973a034ebc806b67");
        private UUID typingAnimationID = new UUID("c541c47f-e0c0-058b-ad1a-d6ae3a4584d9");

        /// <summary>
        /// Passes walk state
        /// </summary>
        /// <param name="walking">True if we are walking towards a targer</param>
        public delegate void WalkStateCanged(bool walking);

        /// <summary>
        /// Fires when we start or stop walking towards a target
        /// </summary>
        public event WalkStateCanged OnWalkStateCanged;


        public StateManager(RadegastInstance instance)
        {
            this.instance = instance;
            this.instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);

            beamTimer = new System.Timers.Timer();
            beamTimer.Enabled = false;
            beamTimer.Elapsed += new ElapsedEventHandler(beamTimer_Elapsed);

            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            RegisterClientEvents(client);
        }

        private void RegisterClientEvents(GridClient client)
        {
            client.Objects.ObjectUpdated += new EventHandler<ObjectUpdatedEventArgs>(Objects_ObjectUpdated);
            client.Self.AlertMessage += new EventHandler<AlertMessageEventArgs>(Self_AlertMessage);
        }

        private void UnregisterClientEvents(GridClient client)
        {
            client.Objects.ObjectUpdated -= new EventHandler<ObjectUpdatedEventArgs>(Objects_ObjectUpdated);
            client.Self.AlertMessage -= new EventHandler<AlertMessageEventArgs>(Self_AlertMessage);
        }

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(client);
        }

        public void Dispose()
        {
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
            UnregisterClientEvents(client);
            beamTimer.Dispose();
            beamTimer = null;

            if (walkTimer != null)
            {
                walkTimer.Dispose();
                walkTimer = null;
            }
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            typing = away = busy = walking = false;
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status == LoginStatus.Success) {
                client.Self.Movement.Camera.Far = 256f;
                effectSource = client.Self.AgentID;
            }
        }

        void Objects_ObjectUpdated(object sender, ObjectUpdatedEventArgs e)
        {
            if (!e.Update.Avatar) return;
            if (!following) return;

            Avatar av;
            client.Network.CurrentSim.ObjectsAvatars.TryGetValue(e.Update.LocalID, out av);
            if (av == null) return;

            if (av.Name == followName)
            {
                Vector3 pos;

                if (av.ParentID == 0)
                {
                    pos = av.Position;
                }
                else
                {
                    Primitive prim;
                    client.Network.CurrentSim.ObjectsPrimitives.TryGetValue(av.ParentID, out prim);
                    
                    if (prim == null)
                        pos = client.Self.SimPosition;
                    else
                        pos = prim.Position + av.Position;
                }

                if (Vector3.Distance(pos, client.Self.SimPosition) > followDistance)
                {
                    int followRegionX = (int)(e.Simulator.Handle >> 32);
                    int followRegionY = (int)(e.Simulator.Handle & 0xFFFFFFFF);
                    ulong x = (ulong)(pos.X + followRegionX);
                    ulong y = (ulong)(pos.Y + followRegionY);

                    client.Self.AutoPilotCancel();
                    client.Self.AutoPilot(x, y, pos.Z);
                }
            }
        }

        public void Follow(string name)
        {
            followName = name;
            following = !string.IsNullOrEmpty(followName);

            if (following)
            {
                walking = false;
            }
        }

        #region Walking (move to)
        private bool walking = false;
        private System.Threading.Timer walkTimer;
        private int walkChekInterval = 500;
        private Vector3d walkToTarget;
        int lastDistance = 0;
        int lastDistanceChanged = 0;

        public void WalkTo(Primitive prim)
        {
            WalkTo(GlobalPosition(prim));
        }

        public void WalkTo(Vector3d globalPos)
        {
            walkToTarget = globalPos;

            if (following)
            {
                following = false;
                followName = string.Empty;
            }

            if (walkTimer == null)
            {
                walkTimer = new System.Threading.Timer(new TimerCallback(walkTimerElapsed), null, walkChekInterval, Timeout.Infinite);
            }

            lastDistanceChanged = System.Environment.TickCount;
            client.Self.AutoPilotCancel();
            walking = true;
            client.Self.AutoPilot(walkToTarget.X, walkToTarget.Y, walkToTarget.Z);
            FireWalkStateCanged();
        }

        void walkTimerElapsed(object sender)
        {

            double distance = Vector3d.Distance(client.Self.GlobalPosition, walkToTarget);

            if (distance < 2d)
            {
                // We're there
                EndWalking();
            }
            else
            {
                if (lastDistance != (int)distance)
                {
                    lastDistanceChanged = System.Environment.TickCount;
                    lastDistance = (int)distance;
                }
                else if ((System.Environment.TickCount - lastDistanceChanged) > 10000)
                {
                    // Our distance to the target has not changed in 10s, give up
                    EndWalking();
                    return;
                }
                walkTimer.Change(walkChekInterval, Timeout.Infinite);
            }
        }

        void Self_AlertMessage(object sender, AlertMessageEventArgs e)
        {
            if (e.Message.Contains("Autopilot cancel"))
            {
                if (walking)
                {
                    EndWalking();
                }
            }
        }

        void FireWalkStateCanged()
        {
            if (OnWalkStateCanged != null)
            {
                try { OnWalkStateCanged(walking); }
                catch (Exception) { }
            }
        }

        public void EndWalking()
        {
            if (walking)
            {
                walking = false;
                Logger.Log("Finished walking.", Helpers.LogLevel.Debug, client);
                walkTimer.Dispose();
                walkTimer = null;
                client.Self.AutoPilotCancel();
                FireWalkStateCanged();
            }
        }
        #endregion

        public void SetTyping(bool typing)
        {
            Dictionary<UUID, bool> typingAnim = new Dictionary<UUID, bool>();
            typingAnim.Add(typingAnimationID, typing);

            client.Self.Animate(typingAnim, false);

            if (typing)
                client.Self.Chat(string.Empty, 0, ChatType.StartTyping);
            else
                client.Self.Chat(string.Empty, 0, ChatType.StopTyping);

            this.typing = typing;
        }

        public void SetAway(bool away)
        {
            Dictionary<UUID, bool> awayAnim = new Dictionary<UUID, bool>();
            awayAnim.Add(awayAnimationID, away);

            client.Self.Animate(awayAnim, true);
            this.away = away;
        }

        public void SetBusy(bool busy)
        {
            Dictionary<UUID, bool> busyAnim = new Dictionary<UUID, bool>();
            busyAnim.Add(busyAnimationID, busy);

            client.Self.Animate(busyAnim, true);
            this.busy = busy;
        }

        public void SetFlying(bool flying)
        {
            this.flying = client.Self.Movement.Fly = flying;
        }

        public void SetAlwaysRun(bool alwaysrun)
        {
            this.alwaysrun = client.Self.Movement.AlwaysRun = alwaysrun;
        }

        public void SetSitting(bool sitting, UUID target)
        {
            this.sitting = sitting;

            if (sitting)
            {
                client.Self.RequestSit(target, Vector3.Zero);
                client.Self.Sit();
            }
            else
            {
                client.Self.Stand();
            }
        }

        public Vector3d GlobalPosition(Primitive prim)
        {
            uint globalX, globalY;
            Utils.LongToUInts(client.Network.CurrentSim.Handle, out globalX, out globalY);
            Vector3 pos = prim.Position;

            return new Vector3d(
                (double)globalX + (double)pos.X,
                (double)globalY + (double)pos.Y,
                (double)pos.Z);
        }

        private System.Timers.Timer beamTimer;
        private List<Vector3d> beamTarget;
        private Random beamRandom = new Random();
        private UUID pointID;
        private UUID sphereID;
        private List<UUID> beamID;
        private int numBeans;
        private Color4[] beamColors = new Color4[3] { new Color4(0, 255, 0, 255), new Color4(255, 0, 0, 255), new Color4(0, 0, 255, 255) };
        private Primitive targetPrim;
        private UUID effectSource;

        public void UnSetPointing()
        {
            beamTimer.Enabled = false;
            if (pointID != UUID.Zero)
            {
                client.Self.PointAtEffect(effectSource, UUID.Zero, Vector3d.Zero, PointAtType.None, pointID);
                pointID = UUID.Zero;
            }

            if (beamID != null)
            {
                foreach (UUID id in beamID)
                {
                    client.Self.BeamEffect(UUID.Zero, UUID.Zero, Vector3d.Zero, new Color4(255, 255, 255, 255), 0, id);
                }
                beamID = null;
            }

            if (sphereID != UUID.Zero)
            {
                client.Self.SphereEffect(Vector3d.Zero, Color4.White, 0, sphereID);
                sphereID = UUID.Zero;
            }

        }

        void beamTimer_Elapsed(object sender, EventArgs e)
        {
            if (beamID == null) return;

            try
            {
                client.Self.SphereEffect(GlobalPosition(targetPrim), beamColors[beamRandom.Next(0, 3)], 0.85f, sphereID);
                int i = 0;
                for (i = 0; i < numBeans; i++)
                {
                    UUID newBeam = UUID.Random();
                    Vector3d scatter;

                    if (i == 0)
                    {
                        scatter = GlobalPosition(targetPrim);
                    }
                    else
                    {
                        Vector3d direction = client.Self.GlobalPosition - GlobalPosition(targetPrim);
                        Vector3d cross = direction % new Vector3d(0, 0, 1);
                        cross.Normalize();
                        scatter = GlobalPosition(targetPrim) + cross * (i * 0.2d) * (i % 2 == 0 ? 1 : -1);
                    }
                    client.Self.BeamEffect(effectSource, UUID.Zero, scatter, beamColors[beamRandom.Next(0, 3)], 1.0f, beamID[i]);
                }

                for (int j = 1; j < numBeans; j++)
                {
                    UUID newBeam = UUID.Random();
                    Vector3d scatter;
                    Vector3d cross = new Vector3d(0, 0, 1);
                    cross.Normalize();
                    scatter = GlobalPosition(targetPrim) + cross * (j * 0.2d) * (j % 2 == 0 ? 1 : -1);

                    client.Self.BeamEffect(effectSource, UUID.Zero, scatter, beamColors[beamRandom.Next(0, 3)], 1.0f, beamID[j + i - 1]);
                }
            }
            catch (Exception) { };

        }

        public void SetPointing(Primitive prim, int numBeans)
        {
            UnSetPointing();
            client.Self.Movement.TurnToward(prim.Position);
            pointID = UUID.Random();
            sphereID = UUID.Random();
            beamID = new List<UUID>();
            beamTarget = new List<Vector3d>();
            targetPrim = prim;
            this.numBeans = numBeans;

            client.Self.PointAtEffect(effectSource, prim.ID, Vector3d.Zero, PointAtType.Select, pointID);

            for (int i = 0; i < numBeans; i++)
            {
                UUID newBeam = UUID.Random();
                beamID.Add(newBeam);
                beamTarget.Add(Vector3d.Zero);
            }

            for (int i = 1; i < numBeans; i++)
            {
                UUID newBeam = UUID.Random();
                beamID.Add(newBeam);
                beamTarget.Add(Vector3d.Zero);
            }

            beamTimer.Interval = 1000;
            beamTimer.Enabled = true;
        }

        public UUID TypingAnimationID
        {
            get { return typingAnimationID; }
            set { typingAnimationID = value; }
        }
        
        public UUID AwayAnimationID
        {
            get { return awayAnimationID; }
            set { awayAnimationID = value; }
        }

        public UUID BusyAnimationID
        {
            get { return busyAnimationID; }
            set { busyAnimationID = value; }
        }

        public UUID EffectSource
        {
            get { return effectSource; }
            set { effectSource = value; }
        }

        public bool IsTyping
        {
            get { return typing; }
        }

        public bool IsAway
        {
            get { return away; }
        }

        public bool IsBusy
        {
            get { return busy; }
        }

        public bool IsFlying
        {
            get { return flying; }
        }

        public bool IsSitting
        {
            get { return sitting; }
        }

        public bool IsPointing
        {
            get { return pointID != UUID.Zero; }
        }

        public bool IsFollowing
        {
            get { return following; }
        }

        public string FollowName
        {
            get { return followName; }
            set { followName = value; }
        }

        public float FollowDistance
        {
            get { return followDistance; }
            set { followDistance = value; }
        }

        public bool IsWalking
        {
            get { return walking; }
        }
    }
}
