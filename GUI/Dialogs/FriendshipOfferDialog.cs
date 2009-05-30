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
    public partial class FriendshipOfferDialog : Form
    {
        private GridClient client;
        InstantMessage im;

        public FriendshipOfferDialog(GridClient iclient, InstantMessage iim)
        {
            InitializeComponent();
            this.Focus();
            client = iclient;
            im = iim;

            descBox.Text = im.FromAgentName + " would like to add you to their friends list.\r\nDo you wish to accept?";

        }

        private void noBtn_Click(object sender, EventArgs e)
        {
            client.Friends.DeclineFriendship(im.FromAgentID, im.IMSessionID);
            this.Close();
        }

        private void yesBtn_Click(object sender, EventArgs e)
        {
            client.Friends.AcceptFriendship(im.FromAgentID, im.IMSessionID);
            this.Close();
        }
    }
}