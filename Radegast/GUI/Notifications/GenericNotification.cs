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
    public partial class ntfGeneric : UserControl
    {
        private RadegastInstance instance;
        private InstantMessage msg;

        public ntfGeneric(RadegastInstance instance, InstantMessage msg)
        {
            InitializeComponent();

            this.instance = instance;
            this.msg = msg;
            txtMessage.BackColor = instance.MainForm.NotificationBackground;
            txtMessage.Text = msg.Message.Replace("\n", "\r\n");
            btnOk.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            instance.MainForm.RemoveNotification(this);
        }
    }
}
