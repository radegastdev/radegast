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
    public partial class PermissionsDialog : Form
    {
        private UUID ptaskID;
        private UUID pitemID;
        private string pobjectName;
        private string pobjectOwner;
        private ScriptPermission pquestions;
        private GridClient pclient;
        private Simulator psimulator;

        public PermissionsDialog(GridClient client, Simulator simulator, UUID taskID, UUID itemID, string objectName, string objectOwner, ScriptPermission questions)
        {
            InitializeComponent();
            this.Focus();
            this.pclient = client;
            this.psimulator = simulator;
            this.ptaskID = taskID;
            this.pitemID = itemID;
            this.pobjectName = objectName;
            this.pobjectOwner = objectOwner;
            this.pquestions = questions;

            descBox.Text = "Object " + objectName + " ownded by " + objectOwner + " is asking permission to " + questions.ToString() + ". Do you accept?";

        }

        private void noBtn_Click(object sender, EventArgs e)
        {
            pclient.Self.ScriptQuestionReply(psimulator, pitemID, ptaskID, 0);
            this.Close();
        }

        private void yesBtn_Click(object sender, EventArgs e)
        {
            pclient.Self.ScriptQuestionReply(psimulator, pitemID, ptaskID, pquestions);
            this.Close();
        }
    }
}