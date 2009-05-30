using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using RadegastNc;
using OpenMetaverse;

namespace Radegast
{
    public partial class GroupInvitationDialog : Form
    {
        private GridClient client;
        InstantMessage im;

        public GroupInvitationDialog(GridClient iclient, InstantMessage iim)
        {
            InitializeComponent();
            this.Focus();
            client = iclient;
            im = iim;

            descBox.Text = im.Message.Replace("\n", "\r\n");

        }

        private void noBtn_Click(object sender, EventArgs e)
        {
            client.Self.InstantMessage(client.Self.Name, im.FromAgentID, "", im.IMSessionID, InstantMessageDialog.GroupInvitationDecline,
               InstantMessageOnline.Offline, Vector3.Zero, UUID.Zero, null);
            this.Close();
        }

        private void yesBtn_Click(object sender, EventArgs e)
        {
            client.Self.InstantMessage(client.Self.Name, im.FromAgentID, "", im.IMSessionID, InstantMessageDialog.GroupInvitationAccept,
               InstantMessageOnline.Offline, Vector3.Zero, UUID.Zero, null);
            this.Close();
        }
    }
}