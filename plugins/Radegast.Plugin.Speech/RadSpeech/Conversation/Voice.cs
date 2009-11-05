using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Radegast;
using Radegast.Core;
using OpenMetaverse;

namespace RadegastSpeech.Conversation
{
    class Voice : Mode
    {
        private VoiceConsole vTab;
        private System.Windows.Forms.ListView participants;
        private VoiceSession session;
 
        #region State Change
        internal Voice(PluginControl pc)
            : base(pc)
        {
            Title = "voice";

            vTab = (VoiceConsole)control.instance.TabConsole.Tabs["voice"].Control;
            participants = vTab.participants;
        }

        internal override void Start()
        {
            vTab.gateway.OnSessionCreate +=new EventHandler(OnSessionCreate);
            vTab.gateway.OnSessionRemove += new EventHandler(gateway_OnSessionRemove);
        }

        internal override void Stop()
        {
            vTab.gateway.OnSessionCreate -= new EventHandler(OnSessionCreate);
            vTab.gateway.OnSessionRemove -= new EventHandler(gateway_OnSessionRemove);
        }
#endregion

#region Sessions
        void OnSessionCreate(object sender, EventArgs e)
        {
            session = sender as VoiceSession;
            control.talker.Say("Voice session started in " + session.RegionName);
//            session.OnParticipantAdded += new EventHandler(session_OnParticipantAdded);
        }

        void gateway_OnSessionRemove(object sender, EventArgs e)
        {
            VoiceSession session = sender as VoiceSession;
            control.talker.Say("Voice session closed.");
//            session.OnParticipantAdded -= new EventHandler(session_OnParticipantAdded);
        }

        void session_OnParticipantAdded(object sender, EventArgs e)
        {
            VoiceParticipant p = sender as VoiceParticipant;

            string pName = (p.Name == null) ? control.instance.getAvatarName(p.ID) : p.Name;
            control.talker.SayMore(pName + " is in voice range.");
        }

        #endregion
    }
}
