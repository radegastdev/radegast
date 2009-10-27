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
        }

        internal void AddParticipant( string name )
        {
            if (name == "") name = "?";

            this.BeginInvoke(new MethodInvoker(delegate()
            {
                ListViewItem item = new ListViewItem(name);
                item.StateImageIndex = (int)State.Idle;
                item.Name = name;
                participants.Items.Add(item);
            }));
        }

        internal void RemoveParticipant(string name)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                foreach (ListViewItem item in participants.Items)
                {
                    if (item.Name == name)
                    {
                        participants.Items.Remove(item);
                        return;
                    }
                }
            }));

        }

        internal void SetTitle(string t)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                this.Text = t;
            }));
        }

        private int FindItem(string name)
        {
            for (int n = 0; n < participants.Items.Count; n++)
            {
                ListViewItem item = participants.Items[n];
                if (item.Name == name) return n;
            }

            return -1;
        }

        /// <summary>
        /// Indicate who is speaking
        /// </summary>
        /// <param name="name">Avatar name</param>
        /// <param name="isSpeaking"></param>
        internal void SetParticipant(string name, bool isMuted, bool isSpeaking, float energy)
        {
            this.BeginInvoke(new MethodInvoker(delegate()
            {
                int n = FindItem(name);
                if (n < 0) return;

                ListViewItem item = participants.Items[n];

                // Set icon depending on what they are doing
                if (isMuted)
                    item.StateImageIndex = (int)State.Muted;
                else if (isSpeaking)
                    item.StateImageIndex = (int)State.Talking;
                else
                    item.StateImageIndex = (int)State.Idle;
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

 
    }
}
