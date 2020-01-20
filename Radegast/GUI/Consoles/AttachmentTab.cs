/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class AttachmentTab : UserControl
    {
        private RadegastInstance instance;
        private GridClient client => instance.Client;
        private Avatar av;

        public AttachmentTab(RadegastInstance instance, Avatar iav)
        {
            this.instance = instance;
            av = iav;
            InitializeComponent();
            AutoScrollPosition = new Point(0, 0);

            InitializeComponent(); // TODO: Was this second initialization intentional...?

            GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        private void AttachmentTab_Load(object sender, EventArgs e)
        {
            RefreshList();
        }

        public void RefreshList()
        {
            List<Primitive> attachments = client.Network.CurrentSim.ObjectsPrimitives.FindAll(
                prim => (prim.ParentID == av.LocalID)
            );

            List<Control> toRemove = new List<Control>();

            foreach (Control c in Controls)
            {
                if (c is AttachmentDetail)
                {
                    toRemove.Add(c);
                }
            }

            foreach (var control in toRemove)
            {
                Controls.Remove(control);
                control.Dispose();
            }

            List<UUID> added = new List<UUID>();

            int n = 0;
            foreach (Primitive prim in attachments)
            {
                if (!added.Contains(prim.ID))
                {
                    AttachmentDetail ad = new AttachmentDetail(instance, av, prim);
                    ad.Location = new Point(0, pnlControls.Height + n * ad.Height);
                    ad.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                    ad.Width = ClientSize.Width;
                    Controls.Add(ad);
                    added.Add(prim.ID);
                    n++;
                }
            }

            AutoScrollPosition = new Point(0, 0);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshList();
        }
    }
}
