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
    public partial class AttachmentTab : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private Avatar av;

        public AttachmentTab(RadegastInstance instance, Avatar iav)
        {
            this.instance = instance;
            av = iav;
            InitializeComponent();
            AutoScrollPosition = new Point(0, 0);

            InitializeComponent(); // TODO: Was this second initialization intentional...?

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        private void AttachmentTab_Load(object sender, EventArgs e)
        {
            RefreshList();
        }

        public void RefreshList()
        {
            List<Primitive> attachments = client.Network.CurrentSim.ObjectsPrimitives.FindAll(
                delegate(Primitive prim)
                {
                    return (prim.ParentID == av.LocalID);
                }
            );

            List<Control> toRemove = new List<Control>();

            foreach (Control c in Controls)
            {
                if (c is AttachmentDetail)
                {
                    toRemove.Add(c);
                }
            }

            for (int i = 0; i < toRemove.Count; i++)
            {
                Controls.Remove(toRemove[i]);
                toRemove[i].Dispose();
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
