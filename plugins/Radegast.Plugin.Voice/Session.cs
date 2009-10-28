using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;
using System.Windows.Forms;
using OpenMetaverse.Voice;
using Radegast;

namespace Radegast.Plugin.Voice
{
    public class Session
    {
        private VoiceControl control;
        internal string SessionHandle;
        private static Dictionary<string, Participant> knownParticipants;
        private SessionForm sessionForm;
        public string RegionName;
        internal bool IsSpatial { get; set; }
        private VoiceGateway connector;

        public VoiceGateway Connector { get { return connector; } }

        public event System.EventHandler OnParticipantAdded;
        public event System.EventHandler OnParticipantUpdate;
        public event System.EventHandler OnParticipantRemoved;

        public Session(VoiceControl pc, VoiceGateway conn, string handle)
        {
            control = pc;
            SessionHandle = handle;
            connector = conn;

            IsSpatial = true;
            knownParticipants = new Dictionary<string, Participant>();
        }

        /// <summary>
        /// Close this session.
        /// </summary>
        internal void Close()
        {
        }

        internal void ParticipantUpdate(string URI,
            bool isMuted,
            bool isSpeaking,
            int volume,
            float energy)
        {
            // Locate in this session
            Participant p = FindParticipant(URI);

            // Set properties
            p.SetProperties(isSpeaking, isMuted, energy);

            // Inform interested parties.
            if (OnParticipantUpdate != null)
                OnParticipantUpdate( p, null );
        }

        internal void AddParticipant(string URI)
        {
            Participant p = FindParticipant(URI);

            // Inform interested parties.
            if (OnParticipantAdded != null)
                OnParticipantAdded(p, null);

            // If this is ME, start sending position updates.
            if (p.ID == control.instance.Client.Self.AgentID)
                control.vClient.PosUpdating(true);
        }

        internal void RemoveParticipant(string URI)
        {
            if (!knownParticipants.ContainsKey(URI))
                return;

            Participant p = knownParticipants[URI];

            // Remove from list for this session.
            knownParticipants.Remove(URI);

            // Inform interested parties.
            if (OnParticipantRemoved != null)
                OnParticipantRemoved(p, null);

            // If this was ME, stop sending position.
            if (p.ID == control.instance.Client.Self.AgentID)
                control.vClient.PosUpdating(false);
        }

        private Participant FindParticipant(string puri)
        {
            Participant p;

            if (knownParticipants.ContainsKey(puri))
            {
                p = knownParticipants[puri];
                if (p.Name.StartsWith("Loading..."))
                    p.Name = control.instance.getAvatarName(p.ID);
                return p;
            }

            // It was not found, so add it.
            lock (knownParticipants)
            {
                p = new Participant( puri, this );
                knownParticipants.Add(puri, p);

                // Set the name.  This might turn out to be "Loading..."
                p.Name = control.instance.getAvatarName(p.ID);
            }

            return p;
        }


    }
}
