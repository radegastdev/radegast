using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radegast.Plugin.Voice
{
    internal partial class SessionForm : Form
    {
        // These enumerated values must match the sequence of icons.
        private enum State {
            Idle = 0,
            Talking,
            Muted };

        private Session session;
        private VoiceControl control;

        internal SessionForm( VoiceControl pc, Session s)
        {
            control = pc;
            session = s;

            InitializeComponent();
            Text = "Voice: " + s.RegionName;

            // Watch for updates on session participants.
            session.OnParticipantUpdate += new EventHandler(session_OnParticipantUpdate);
            session.OnParticipantAdded += new EventHandler(session_OnParticipantAdded);
            session.OnParticipantRemoved += new EventHandler(session_OnParticipantRemoved);

            // Watch for context-menu clicks.

        }

        void session_OnParticipantRemoved(object sender, EventArgs e)
        {
             Participant p = sender as Participant;
 
             this.BeginInvoke(new MethodInvoker(delegate()
             {
                 ListViewItem item = participants.Items[p.Name];
                 participants.Items.Remove(item);
            }));

        }

        void session_OnParticipantUpdate(object sender, EventArgs e)
        {
            Participant p = sender as Participant;

            this.BeginInvoke(new MethodInvoker(delegate()
            {
                ListViewItem item = participants.Items[p.Name];

                // Set icon depending on what they are doing
                if (p.IsMuted)
                    item.StateImageIndex = (int)State.Muted;
                else if (p.IsSpeaking)
                    item.StateImageIndex = (int)State.Talking;
                else
                    item.StateImageIndex = (int)State.Idle;
            }));
        }

        void session_OnParticipantAdded(object sender, EventArgs e)
        {
            Participant p = sender as Participant;

            this.BeginInvoke(new MethodInvoker(delegate()
            {
                ListViewItem item = new ListViewItem(p.Name);
                item.Tag = p;
                item.StateImageIndex = (int)State.Idle;
                item.Name = p.Name;
                participants.Items.Add(item);
            }));
        }

        private void SessionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            session.Close();
        }

        private void SessionForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Give user a chance to cancel session closing.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SessionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult r = MessageBox.Show(
                "Do you mean to close this Voice session?",
                "Really?",
                MessageBoxButtons.YesNo );

            if (r == DialogResult.No)
                e.Cancel = true;
        }

        private void participants_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            RadegastContextMenuStrip cms = new RadegastContextMenuStrip();
            control.instance.ContextActionManager.AddContributions(cms, control.instance.Client);
            cms.Show((Control)sender, new Point(e.X, e.Y));
        }

    }
}
