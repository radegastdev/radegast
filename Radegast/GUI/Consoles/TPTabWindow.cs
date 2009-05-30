using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using RadegastNc;

namespace Radegast
{
    public partial class TPTabWindow : UserControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;
        private string targetName;
        private UUID targetUUID;

        public TPTabWindow(RadegastInstance instance, InstantMessageEventArgs e)
        {
            InitializeComponent();

            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;
            ProcessEventArgs(e);
        }

        private void ProcessEventArgs(InstantMessageEventArgs e)
        {
            targetName = e.IM.FromAgentName;
            targetUUID = e.IM.FromAgentID;

            lblSubheading.Text =
                "Received teleport offer from " + targetName + " with message:";

            rtbOfferMessage.AppendText(e.IM.Message);
        }

        public void CloseTab()
        {
            instance.TabConsole.GetTab("chat").Select();
            instance.TabConsole.GetTab(targetUUID.ToString()).Close();
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            client.Self.TeleportLureRespond(targetUUID, true);
            CloseTab();
        }

        private void btnDecline_Click(object sender, EventArgs e)
        {
            client.Self.TeleportLureRespond(targetUUID, false);
            CloseTab();
        }

        public string TargetName
        {
            get { return targetName; }
        }

        public UUID TargetUUID
        {
            get { return targetUUID; }
        }
    }
}
