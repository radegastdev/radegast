using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

namespace RadegastSpeech.Conversation
{
    class GroupIMSession : IMSession
    {
        Group group;
        private string shortGroupName;

        internal GroupIMSession(PluginControl pc, UUID session)
            : base(pc, session)
        {
            AgentID = session;
            group = control.instance.Groups[session];
            Title = group.Name;
            shortGroupName = Title.Split(' ')[0];

            Talker.Say("New group I.M. from " + Title, Talk.BeepType.Comm);
        }

        /// <summary>
        /// A group IM session is made active.
        /// </summary>
        internal override void Start()
        {
            base.Start();
            Talker.SayMore("Group " + Title);
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

            string who = FriendlyName(agentName, agentID );
            if (!amCurrent())
                who = shortGroupName + ", " + who;

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
/*
        private void WhoIsHere()
        {
            List<ChatSessionMember> members =
                control.instance.Client.Self.GroupChatSessions[SessionID];
            string list = "Current session members are ";
            foreach (ChatSessionMember member in members)
            {
                control.instance.Client.Avatars.RequestAvatarName(member.AvatarKey);
            }
        }
        */
    }
}
