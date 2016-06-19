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
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class ntfScriptDialog : Notification
    {
        RadegastInstance instance;
        UUID objectID;
        int chatChannel;

        public ntfScriptDialog(RadegastInstance instance, string message, string objectName, UUID imageID, UUID objectID, string firstName, string lastName, int chatChannel, List<string> buttons)
            : base(NotificationType.ScriptDialog)
        {
            InitializeComponent();

            this.instance = instance;
            this.chatChannel = chatChannel;
            this.objectID = objectID;

            descBox.BackColor = instance.MainForm.NotificationBackground;
            descBox.Text = firstName + " " + lastName + "'s " + objectName + "\r\n\r\n" + message.Replace("\n", "\r\n") + "\r\n";

            NotificationEventArgs args = new NotificationEventArgs(instance);
            args.Text = descBox.Text;

            int btnWidth = 90;
            int btnHeight = 23;

            int i = 0;
            if (buttons.Count == 1 && buttons[0] == "!!llTextBox!!")
            {
                txtTextBox.Visible = true;
                sendBtn.Visible = true;
                args.Buttons.Add(ignoreBtn);
            }
            else
            {
                foreach (string label in buttons)
                {
                    Button b = new Button();
                    b.Size = new Size(btnWidth, btnHeight);
                    b.Text = label;
                    b.Location = new Point(5 + (i % 3) * (btnWidth + 5), btnsPanel.Size.Height - (i / 3) * (btnHeight + 5) - (btnHeight + 5));
                    b.Name = i.ToString();
                    b.Click += new EventHandler(b_Click);
                    b.UseVisualStyleBackColor = true;
                    b.Margin = new Padding(0, 3, 0, 3);
                    b.Padding = new Padding(0);
                    btnsPanel.Controls.Add(b);
                    args.Buttons.Add(b);
                    i++;
                }
            }
            // Fire off event
            args.Buttons.Add(ignoreBtn);
            FireNotificationCallback(args);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void b_Click(object sender, EventArgs e)
        {
            string label = ((Button)sender).Text;
            int index = int.Parse(((Button)sender).Name);
            instance.Client.Self.ReplyToScriptDialog(chatChannel, index, label, objectID);
            instance.MainForm.RemoveNotification(this);
        }

        private void ignoreBtn_Click(object sender, EventArgs e)
        {
            instance.MainForm.RemoveNotification(this);
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            instance.Client.Self.ReplyToScriptDialog(chatChannel, 0, txtTextBox.Text, objectID);
            instance.MainForm.RemoveNotification(this);
        }
    }
}
