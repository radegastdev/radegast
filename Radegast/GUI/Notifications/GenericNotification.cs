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

        public ntfGeneric(RadegastInstance instance, string msg)
        {
            InitializeComponent();

            this.instance = instance;
            txtMessage.BackColor = instance.MainForm.NotificationBackground;
            txtMessage.Text = msg.Replace("\n", "\r\n");
            btnOk.Focus();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            instance.MainForm.RemoveNotification(this);
        }
    }
}
