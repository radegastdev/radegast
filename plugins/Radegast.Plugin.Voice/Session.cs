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
    internal class Session
    {
        private VoiceControl control;
        internal string SessionHandle;
        private static Dictionary<string, VoiceParticipant> knownParticipants;
        private SessionForm sessionForm;
        private string regionName;
        internal bool IsSpatial { get; set; }
        private VoiceGateway connector;

        internal Session(VoiceControl pc, VoiceGateway conn, string handle)
        {
            control = pc;
            SessionHandle = handle;
            connector = conn;

            IsSpatial = true;
            knownParticipants = new Dictionary<string, VoiceParticipant>();

            IAsyncResult creation = control.instance.MainForm.BeginInvoke(
               new MethodInvoker(delegate()
            {
                sessionForm = new SessionForm(control, this);
                sessionForm.Show();
            }));

            // Wait for that to complete.
            control.instance.MainForm.EndInvoke(creation);
        }

        internal void SetTitle(string n)
        {
            regionName = n;
            sessionForm.SetTitle("Voice: " + regionName);
        }

        /// <summary>
        /// Close this session.
        /// </summary>
        internal void Close()
        {
            control.instance.MainForm.BeginInvoke(new MethodInvoker(delegate()
            {
                // Close the visible form
                sessionForm.Hide();

                // Remove the session from the contexts.
                if (control.vClient != null)
                    control.vClient.CloseSession(SessionHandle);
            }));
        }

        internal void OnParticipantUpdate(string URI,
            bool isMuted,
            bool isSpeaking,
            int volume,
            float energy)
        {
            VoiceParticipant p = FindParticipant(URI);
            sessionForm.SetParticipant(p.AvatarName, isMuted, isSpeaking, energy);
        }

        internal void AddParticipant(string URI)
        {
            VoiceParticipant p = FindParticipant(URI);

            if (p.AvatarName == control.instance.Client.Self.Name)
                control.vClient.PosUpdating(true);
        }

        internal void RemoveParticipant(string URI)
        {
            if (!knownParticipants.ContainsKey(URI))
                return;
            VoiceParticipant p = knownParticipants[URI];
            sessionForm.RemoveParticipant(p.AvatarName);

            if (p.AvatarName == control.instance.Client.Self.Name)
                control.vClient.PosUpdating(false);
        }

        private VoiceParticipant FindParticipant(string puri)
        {
            VoiceParticipant p;

            if (knownParticipants.ContainsKey(puri))
            {
                p = knownParticipants[puri];
                if (p.AvatarName.StartsWith("Loading..."))
                    p.AvatarName = control.instance.getAvatarName(p.id);
                return p;
            }


            lock (knownParticipants)
            {
                p = new VoiceParticipant(puri);
                knownParticipants.Add(puri, p);
                p.AvatarName = control.instance.getAvatarName(p.id);
                sessionForm.AddParticipant(p.AvatarName);
            }

            return p;
        }


    }
}
