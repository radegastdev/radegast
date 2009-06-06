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
    public partial class ntfPermissions : UserControl
    {
        private UUID taskID;
        private UUID itemID;
        private string objectName;
        private string objectOwner;
        private ScriptPermission questions;
        private RadegastInstance instance;
        private Simulator simulator;


        public ntfPermissions(RadegastInstance instance, Simulator simulator, UUID taskID, UUID itemID, string objectName, string objectOwner, ScriptPermission questions)
        {
            InitializeComponent();

            this.instance = instance;
            this.simulator = simulator;
            this.taskID = taskID;
            this.itemID = itemID;
            this.objectName = objectName;
            this.objectOwner = objectOwner;
            this.questions = questions;

            txtMessage.BackColor = instance.MainForm.NotificationBackground;
            txtMessage.Text = "Object " + objectName + " ownded by " + objectOwner + " is asking permission to " + questions.ToString() + ". Do you accept?";

        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            instance.Client.Self.ScriptQuestionReply(simulator, itemID, taskID, questions);
            instance.MainForm.RemoveNotification(this);
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            instance.Client.Self.ScriptQuestionReply(simulator, itemID, taskID, 0);
            instance.MainForm.RemoveNotification(this);
        }

        private void btnIgnore_Click(object sender, EventArgs e)
        {
            instance.MainForm.RemoveNotification(this);
        }
    }
}
