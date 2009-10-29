// This class is temporary.  It contains functions that will eventually
// move into OpenMetaverse.Voice.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;
using System.Windows.Forms;
using OpenMetaverse.Voice;
using Radegast;

namespace Radegast.Core
{
    public class VoiceSession
    {
        internal string SessionHandle;
        private static Dictionary<string, VoiceParticipant> knownParticipants;
        public string RegionName;
        internal bool IsSpatial { get; set; }
        private VoiceGateway connector;

        public VoiceGateway Connector { get { return connector; } }

        public event System.EventHandler OnParticipantAdded;
        public event System.EventHandler OnParticipantUpdate;
        public event System.EventHandler OnParticipantRemoved;

        public VoiceSession(VoiceGateway conn, string handle)
        {
            SessionHandle = handle;
            connector = conn;

            IsSpatial = true;
            knownParticipants = new Dictionary<string, VoiceParticipant>();
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
            VoiceParticipant p = FindParticipant(URI);

            // Set properties
            p.SetProperties(isSpeaking, isMuted, energy);

            // Inform interested parties.
            if (OnParticipantUpdate != null)
                OnParticipantUpdate(p, null);
        }

        internal void AddParticipant(string URI)
        {
            VoiceParticipant p = FindParticipant(URI);

            // Inform interested parties.
            if (OnParticipantAdded != null)
                OnParticipantAdded(p, null);

            // If this is ME, start sending position updates.
            if (p.ID == connector.Client.Self.AgentID)
                connector.PosUpdating(true);
        }

        internal void RemoveParticipant(string URI)
        {
            if (!knownParticipants.ContainsKey(URI))
                return;

            VoiceParticipant p = knownParticipants[URI];

            // Remove from list for this session.
            knownParticipants.Remove(URI);

            // Inform interested parties.
            if (OnParticipantRemoved != null)
                OnParticipantRemoved(p, null);

            // If this was ME, stop sending position.
            if (p.ID == connector.Client.Self.AgentID)
                connector.PosUpdating(false);
        }

        private VoiceParticipant FindParticipant(string puri)
        {
            VoiceParticipant p;

            lock (knownParticipants)
            {
                if (knownParticipants.ContainsKey(puri))
                    p = knownParticipants[puri];
                else
                {
                    // It was not found, so add it.
                    p = new VoiceParticipant(puri, this);
                    knownParticipants.Add(puri, p);
                }
            }
/* TODO
            // Fill in the name.
            if (p.Name == null || p.Name.StartsWith("Loading..."))
                    p.Name = control.instance.getAvatarName(p.ID);
                return p;
*/
            return p;
        }


    }
}
