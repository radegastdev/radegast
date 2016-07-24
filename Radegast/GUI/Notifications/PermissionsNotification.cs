// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id$
//
using System;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class ntfPermissions : Notification
    {
        private UUID taskID;
        private UUID itemID;
        private string objectName;
        private string objectOwner;
        private ScriptPermission questions;
        private RadegastInstance instance;
        private Simulator simulator;


        public ntfPermissions(RadegastInstance instance, Simulator simulator, UUID taskID, UUID itemID, string objectName, string objectOwner, ScriptPermission questions)
            : base(NotificationType.PermissionsRequest)
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
            txtMessage.Text = "Object " + objectName + " owned by " + objectOwner + " is asking permission to " + questions.ToString() + ". Do you accept?";

            // Fire off event
            NotificationEventArgs args = new NotificationEventArgs(instance);
            args.Text = txtMessage.Text;
            args.Buttons.Add(btnYes);
            args.Buttons.Add(btnNo);
            args.Buttons.Add(btnMute);
            FireNotificationCallback(args);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
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

        private void btnMute_Click(object sender, EventArgs e)
        {
            instance.Client.Self.UpdateMuteListEntry(MuteType.Object, taskID, objectName);
            instance.MainForm.RemoveNotification(this);
        }
    }
}
