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
    public partial class ntfFriendshipOffer : UserControl
    {
        private RadegastInstance instance;
        private InstantMessage msg;

        public ntfFriendshipOffer(RadegastInstance instance, InstantMessage msg)
        {
            InitializeComponent();
            this.instance = instance;
            this.msg = msg;

            txtHead.BackColor = instance.MainForm.NotificationBackground;
            txtHead.Text = String.Format("{0} has offered you friendship.", msg.FromAgentName);
            txtMessage.BackColor = instance.MainForm.NotificationBackground;
            txtMessage.Text = msg.Message;
            btnYes.Focus();
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            instance.Client.Friends.AcceptFriendship(msg.FromAgentID, msg.IMSessionID);
            instance.MainForm.RemoveNotification(this);
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            instance.Client.Friends.DeclineFriendship(msg.FromAgentID, msg.IMSessionID);
            instance.MainForm.RemoveNotification(this);
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            instance.MainForm.RemoveNotification(this);
        }
    }
}
