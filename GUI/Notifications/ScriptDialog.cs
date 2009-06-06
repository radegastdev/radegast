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
    public partial class ntfScriptDialog : UserControl
    {
        RadegastInstance instance;
        UUID objectID;
        int chatChannel;

        public ntfScriptDialog(RadegastInstance instance, string message, string objectName, UUID imageID, UUID objectID, string firstName, string lastName, int chatChannel, List<string> buttons)
        {
            InitializeComponent();

            this.instance = instance;
            this.chatChannel = chatChannel;
            this.objectID = objectID;

            descBox.BackColor = instance.MainForm.NotificationBackground;
            descBox.Text = firstName + " " + lastName + "'s " + objectName + "\r\n\r\n" + message.Replace("\n", "\r\n") + "\r\n";
 
            int btnWidth = 90;
            int btnHeight = 23;

            int i = 0;
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
                i++;
            }
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
    }
}
