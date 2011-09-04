using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

namespace RadegastSpeech.Conversation
{
    /// <summary>
    /// Instant Message session with an individual
    /// </summary>
    class SingleIMSession : IMSession
    {
        internal SingleIMSession(PluginControl pc, string label, UUID agent, UUID session) : 
            base(pc, session)
        {
            AgentID = agent;
            Title = label;

            Talker.Say("New I.M. from "+Title, Talk.BeepType.Comm );
        }

        internal override void Start()
        {
            base.Start();
            Talker.SayMore("I.M. from "+Title );
        }

        internal override void OnMessage(UUID agentID, string agentName, string message)
        {
            if (isMuted) return;

            string who = FriendlyName(agentName, agentID);
            if (!amCurrent())
                who = "I.M. from " + who;

            Talker.SayIMPerson(
                who,
                message,
                SessionPosition,
                Talker.voices.VoiceFor(agentID));
        }

        internal override bool Hear(string message)
        {
            if (base.Hear(message)) return true;

            if (message.ToLower() == "disconnect")
            {
                Radegast.RadegastTab tab = control.instance.TabConsole.Tabs[Title];
                tab.Close();
                return true;
            }

            Talker.Say(control.converse.LoginName, message);
            control.instance.Netcom.SendInstantMessage(
                        message,
                        AgentID,
                        SessionID);
            return true;
        }


    }
}
