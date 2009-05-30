using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using OpenMetaverse;
using OpenMetaverse.Packets;
using RadegastNc;

namespace Radegast
{
    public class StateManager
    {
        private RadegastInstance instance;
        private GridClient client;
        private RadegastNetcom netcom;

        private bool typing = false;
        private bool away = false;
        private bool busy = false;
        private bool flying = false;
        private bool alwaysrun = false;
        private bool sitting = false;
        
        private bool pointing = false;
        private UUID pointID;
        private UUID beamID;

        private bool following = false;
        private string followName = string.Empty;
        private float followDistance = 3.0f;

        private Timer agentUpdateTicker;

        private UUID awayAnimationID = new UUID("fd037134-85d4-f241-72c6-4f42164fedee");
        private UUID busyAnimationID = new UUID("efcf670c2d188128973a034ebc806b67");
        private UUID typingAnimationID = new UUID("c541c47f-e0c0-058b-ad1a-d6ae3a4584d9");

        public StateManager(RadegastInstance instance)
        {
            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;

            AddNetcomEvents();
            AddClientEvents();
            InitializeAgentUpdateTimer();
        }

        private void AddNetcomEvents()
        {
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            agentUpdateTicker.Enabled = false;
            typing = away = busy = false;
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status == LoginStatus.Success) {
                client.Self.Movement.Camera.Far = 256f;
                agentUpdateTicker.Enabled = true;
            }
        }

        private void AddClientEvents()
        {
            client.Objects.OnObjectUpdated += new ObjectManager.ObjectUpdatedCallback(Objects_OnObjectUpdated);
        }

        private void Objects_OnObjectUpdated(Simulator simulator, ObjectUpdate update, ulong regionHandle, ushort timeDilation)
        {
            if (!update.Avatar) return;
            if (!following) return;

            Avatar av;
            client.Network.CurrentSim.ObjectsAvatars.TryGetValue(update.LocalID, out av);
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
                    int followRegionX = (int)(regionHandle >> 32);
                    int followRegionY = (int)(regionHandle & 0xFFFFFFFF);
                    ulong x = (ulong)(pos.X + followRegionX);
                    ulong y = (ulong)(pos.Y + followRegionY);

                    client.Self.AutoPilotCancel();
                    client.Self.AutoPilot(x, y, pos.Z);
                }
            }
        }

        private void InitializeAgentUpdateTimer()
        {
            agentUpdateTicker = new Timer(500);
            agentUpdateTicker.Elapsed += new ElapsedEventHandler(agentUpdateTicker_Elapsed);
        }

        private void agentUpdateTicker_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            //client.Self.Status.Camera.BodyRotation = Quaternion.Identity;
            //client.Self.Status.Camera.HeadRotation = Quaternion.Identity;
            //client.Self.Status.Camera.CameraCenter = client.Self.SimPosition;
            //client.Self.Status.Camera.CameraAtAxis = new Vector3(0, 0.9999f, 0);
            //client.Self.Status.Camera.CameraLeftAxis = new Vector3(0.9999f, 0, 0);
            //client.Self.Status.Camera.CameraUpAxis = new Vector3(0, 0, 0.9999f);
            client.Self.Movement.Camera.Far = 128.0f;
            
            client.Self.Movement.Away = away;
            client.Self.Movement.Fly = flying;

            //client.Self.Status.SendUpdate();
        }

        public void Follow(string name)
        {
            followName = name;
            following = !string.IsNullOrEmpty(followName);
        }

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
            this.flying = flying;
        }

        public void SetAlwaysRun(bool alwaysrun)
        {
            client.Self.Movement.AlwaysRun = alwaysrun;
            this.alwaysrun = alwaysrun;
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

        public void SetPointing(bool pointing, UUID target)
        {
            this.pointing = pointing;

            if (pointing)
            {
                pointID = UUID.Random();
                beamID = UUID.Random();
                client.Self.PointAtEffect(client.Self.SessionID, target, Vector3d.Zero, PointAtType.Select, pointID);
                client.Self.BeamEffect(client.Self.SessionID, target, Vector3d.Zero, new Color4(255, 255, 255, 0), 60.0f, beamID);
            }
            else
            {
                if (pointID == UUID.Zero || beamID == UUID.Zero) return;

                client.Self.PointAtEffect(UUID.Zero, UUID.Zero, Vector3d.Zero, PointAtType.Select, pointID);
                client.Self.BeamEffect(UUID.Zero, UUID.Zero, Vector3d.Zero, new Color4(255, 255, 255, 0), 0, beamID);
                pointID = UUID.Zero;
                beamID = UUID.Zero;
            }
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
            get { return pointing; }
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
    }
}
