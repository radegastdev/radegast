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
    public partial class ScriptDialog : Form
    {
        GridClient pclient;
        UUID pobjectID;
        int pchatChannel;

        public ScriptDialog(GridClient client, string message, string objectName, UUID imageID, UUID objectID, string firstName, string lastName, int chatChannel, List<string> buttons)
        {
            InitializeComponent();
            this.Focus();
            pclient = client;
            pchatChannel = chatChannel;
            pobjectID = objectID;


            descBox.Text = firstName + " " + lastName + "'s " + objectName + "\r\n\r\n" + message.Replace("\n", "\r\n") + "\r\n";

            int i = 0;
            foreach (string label in buttons) {
                Button b = new Button();
                b.Size = new Size(110, 23);
                b.Text = label;
                b.Location = new Point(5 + (i % 3) * 120, 125 - (i/3)*30);
                b.Name = i.ToString();
                b.Click += new EventHandler(b_Click);
                btnsPanel.Controls.Add(b);
                i++;
            }

        }

        void b_Click(object sender, EventArgs e)
        {
            string label = ((Button)sender).Text;
            int index = int.Parse(((Button)sender).Name);
            pclient.Self.ReplyToScriptDialog(pchatChannel, index, label, pobjectID);
            this.Close();
        }

        private void ignoreBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}