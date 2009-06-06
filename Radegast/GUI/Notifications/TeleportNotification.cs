using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class ntfTeleport : UserControl
    {
        private RadegastInstance instance;
        private InstantMessage msg;

        public ntfTeleport(RadegastInstance instance, InstantMessage msg)
        {
            InitializeComponent();
            this.instance = instance;
            this.msg = msg;

            txtHead.BackColor = instance.MainForm.NotificationBackground;
            txtHead.Text = String.Format("{0} has offered to teleport you to their location.", msg.FromAgentName);
            txtMessage.BackColor = instance.MainForm.NotificationBackground;
            txtMessage.Text = msg.Message;
            btnTeleport.Focus();
        }

        private void btnTeleport_Click(object sender, EventArgs e)
        {
            instance.Client.Self.TeleportLureRespond(msg.FromAgentID, true);
            instance.MainForm.RemoveNotification(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            instance.Client.Self.TeleportLureRespond(msg.FromAgentID, false);
            instance.MainForm.RemoveNotification(this);
        }
    }
}
