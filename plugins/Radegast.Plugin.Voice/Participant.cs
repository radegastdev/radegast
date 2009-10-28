using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse.Voice;
using OpenMetaverse;

namespace Radegast.Plugin.Voice
{
    public class Participant
    {
        public string Name
        {
            get { return part.AvatarName; }
            set { part.AvatarName = value; }
        }

        private bool muted;
        private int volume;
        private Session session;
        private float energy;
        private VoiceParticipant part;
        public float Energy { get { return energy; } }
        private bool speaking;
        public bool IsSpeaking { get { return speaking; } }
        public string URI { get { return part.Sip; } }
        public UUID ID { get { return part.id; }}

        public Participant( string uri, Session s )
        {
            session = s;
            part = new VoiceParticipant( uri );
            Volume = 64;
        }

        public bool IsMuted
        {
            get { return muted; }
            set
            {
                muted = value;
                StringBuilder sb = new StringBuilder();
                sb.Append(VoiceGateway.MakeXML("SessionHandle", session.SessionHandle));
                sb.Append(VoiceGateway.MakeXML("ParticipantURI", part.Sip));
                sb.Append(VoiceGateway.MakeXML("Mute", muted?"1":"0"));
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
                sb.Append(VoiceGateway.MakeXML("SessionHandle", session.SessionHandle));
                sb.Append(VoiceGateway.MakeXML("ParticipantURI", part.Sip));
                sb.Append(VoiceGateway.MakeXML("Volume", volume.ToString()));
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
