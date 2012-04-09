using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

namespace RadegastSpeech.Conversation
{
    class ConferenceIMSession : IMSession
    {
        internal ConferenceIMSession(PluginControl pc, UUID session, string title)
            : base(pc, session)
        {
            AgentID = session;
            Title = title;
            Talker.Say("New " + Title, Talk.BeepType.Comm);
        }

        /// <summary>
        /// A group IM session is made active.
        /// </summary>
        internal override void Start()
        {
            base.Start();
            Talker.SayMore(Title);
        }

        /// <summary>
        /// Handle arriving message within an IM session.
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="agentName"></param>
        /// <param name="message"></param>
        internal override void OnMessage(UUID agentID, string agentName, string message)
        {
            if (isMuted) return;

            string who = FriendlyName(agentName, agentID);
            if (!amCurrent())
                who = Title + ", " + who;

            Talker.SayIMPerson(who,
                    message,
                    SessionPosition,
                    Talker.voices.VoiceFor(agentID));
        }

        /// <summary>
        /// Handle recognized speech during a Group IM Session.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        internal override bool Hear(string message)
        {
            // Let higher level commands have a chance.
            if (base.Hear(message)) return true;

            // Command to Close the session?
            if (message.ToLower() == "disconnect")
            {
                Radegast.RadegastTab tab = TabConsole.Tabs[Title];
                tab.Close();
                return true;
            }

            // Otherwise put the text into the session.
            Talker.Say(control.converse.LoginName, message);
            control.instance.Netcom.SendInstantMessage(
                        message,
                        SessionID,
                        SessionID);
            return true;
        }
    }
}
