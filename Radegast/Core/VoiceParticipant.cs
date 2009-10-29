// This class is temporary.  It contains functions that will eventually
// move into OpenMetaverse.Voice.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse.Voice;
using OpenMetaverse;

namespace Radegast.Core
{
    public class VoiceParticipant
    {
        public string Name
        {
            get { return part.AvatarName; }
            set { part.AvatarName = value; }
        }

        private bool muted;
        private int volume;
        private VoiceSession session;
        private float energy;
        private OpenMetaverse.Voice.VoiceParticipant part;
        public float Energy { get { return energy; } }
        private bool speaking;
        public bool IsSpeaking { get { return speaking; } }
        public string URI { get { return part.Sip; } }
        public UUID ID { get { return part.id; } }

        public VoiceParticipant(string uri, VoiceSession s)
        {
            session = s;

            // Some of the functionality is already in OMV
            part = new OpenMetaverse.Voice.VoiceParticipant(uri);
            Volume = 64;
        }

        public bool IsMuted
        {
            get { return muted; }
            set
            {
                muted = value;
                StringBuilder sb = new StringBuilder();
                sb.Append(OpenMetaverse.Voice.VoiceGateway.MakeXML("SessionHandle", session.SessionHandle));
                sb.Append(OpenMetaverse.Voice.VoiceGateway.MakeXML("ParticipantURI", part.Sip));
                sb.Append(OpenMetaverse.Voice.VoiceGateway.MakeXML("Mute", muted ? "1" : "0"));
                session.Connector.Request("Session.SetParticipantMuteForMe.1", sb.ToString());
            }
        }

        public int Volume
        {
            get { return volume; }
            set
            {
                volume = value;
                StringBuilder sb = new StringBuilder();
                sb.Append(OpenMetaverse.Voice.VoiceGateway.MakeXML("SessionHandle", session.SessionHandle));
                sb.Append(OpenMetaverse.Voice.VoiceGateway.MakeXML("ParticipantURI", part.Sip));
                sb.Append(OpenMetaverse.Voice.VoiceGateway.MakeXML("Volume", volume.ToString()));
                session.Connector.Request("Session.SetParticipantVolumeForMe.1", sb.ToString());
            }
        }

        internal void SetProperties(bool speak, bool mute, float en)
        {
            speaking = speak;
            muted = mute;
            energy = en;
        }
    }
}
