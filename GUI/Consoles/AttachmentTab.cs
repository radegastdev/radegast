using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class AttachmentTab : UserControl
    {
        private GridClient client;
        private Avatar av;

        public AttachmentTab(RadegastInstance instance, Avatar iav)
        {
            client = instance.Client;
            av = iav;
            InitializeComponent();
            AutoScrollPosition = new Point(0, 0);

            InitializeComponent();
        }

        private void AttachmentTab_Load(object sender, EventArgs e)
        {
            List<Primitive> attachments = client.Network.CurrentSim.ObjectsPrimitives.FindAll(
                delegate(Primitive prim)
                {
                    return (prim.ParentID == av.LocalID);
                }
            );

            Controls.Clear();
            List<UUID> added = new List<UUID>();

            int n = 0;
            foreach (Primitive prim in attachments) {
                if (!added.Contains(prim.ID)) {
                    AttachmentDetail ad = new AttachmentDetail(client, av, prim);
                    ad.Location = new Point(0, n++ * ad.Height);
                    ad.Dock = DockStyle.Top;
                    Controls.Add(ad);
                    added.Add(prim.ID);
                }
            }

            AutoScrollPosition = new Point(0, 0);
        }
    }
}
