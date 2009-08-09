
﻿using System;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class ntfGroupInvitation : UserControl
    {
        private RadegastInstance instance;
        private InstantMessage msg;

        public ntfGroupInvitation(RadegastInstance instance, InstantMessage msg)
        {
            InitializeComponent();

            this.instance = instance;
            this.msg = msg;

            txtMessage.BackColor = instance.MainForm.NotificationBackground;
            txtMessage.Text = msg.Message.Replace("\n", "\r\n");
            btnYes.Focus();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            instance.Client.Self.InstantMessage(instance.Client.Self.Name, msg.FromAgentID, "", msg.IMSessionID, InstantMessageDialog.GroupInvitationAccept,
   InstantMessageOnline.Online, Vector3.Zero, UUID.Zero, null);
            instance.MainForm.RemoveNotification(this);
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            instance.Client.Self.InstantMessage(instance.Client.Self.Name, msg.FromAgentID, "", msg.IMSessionID, InstantMessageDialog.GroupInvitationDecline,
   InstantMessageOnline.Online, Vector3.Zero, UUID.Zero, null);
            instance.MainForm.RemoveNotification(this);
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            instance.MainForm.RemoveNotification(this);
        }
    }
}
