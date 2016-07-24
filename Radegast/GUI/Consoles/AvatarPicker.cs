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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class AvatarPicker : UserControl
    {
        RadegastInstance instance;
        GridClient client { get { return instance.Client; } }
        UUID searchID;
        public ListView currentList;

        [Browsable(true)]
        public event EventHandler SelectionChaged;

        [Browsable(false)]
        public Dictionary<UUID, string> SelectedAvatars
        {
            get
            {
                Dictionary<UUID, string> res = new Dictionary<UUID,string>();
                if (currentList != null)
                {
                    foreach (ListViewItem item in currentList.SelectedItems)
                    {
                        res.Add((UUID)item.Tag, item.Text);
                    }
                }
                return res;
            }
        }

        public AvatarPicker(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(AvatarPicker_Disposed);
            
            this.instance = instance;

            // events
            client.Avatars.AvatarPickerReply += new EventHandler<AvatarPickerReplyEventArgs>(Avatars_AvatarPickerReply);

            List<NearbyAvatar> nearAvatars = instance.TabConsole.NearbyAvatars;
            for (int i = 0; i < nearAvatars.Count; i++)
            {
                string name = instance.Names.Get(nearAvatars[i].ID, nearAvatars[i].Name);
                lvwNear.Items.Add(new ListViewItem() { Text = nearAvatars[i].Name, Tag = nearAvatars[i].ID });
            }

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void AvatarPicker_Disposed(object sender, EventArgs e)
        {
            client.Avatars.AvatarPickerReply -= new EventHandler<AvatarPickerReplyEventArgs>(Avatars_AvatarPickerReply);
        }

        void Avatars_AvatarPickerReply(object sender, AvatarPickerReplyEventArgs e)
        {
            if (searchID == e.QueryID && e.Avatars.Count > 0)
            {
                BeginInvoke(new MethodInvoker(() =>
                    {
                        foreach (KeyValuePair<UUID, string> kvp in e.Avatars)
                        {
                            string name = instance.Names.Get(kvp.Key, kvp.Value);
                            lvwSearch.Items.Add(new ListViewItem(name) { Text = kvp.Value, Tag = kvp.Key });
                        }
                    }));
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text.Length > 2)
            {
                searchID = UUID.Random();
                lvwSearch.Items.Clear();
                client.Avatars.RequestAvatarNameSearch(txtSearch.Text, searchID);
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            btnSearch.Enabled = txtSearch.Text.Length > 2;
        }

        private void lvwNear_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentList = lvwNear;
            if (SelectionChaged != null)
                SelectionChaged(this, EventArgs.Empty);
        }

        private void lvwSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentList = lvwSearch;
            if (SelectionChaged != null)
                SelectionChaged(this, EventArgs.Empty);
        }

        private void lvwSearch_SizeChanged(object sender, EventArgs e)
        {
            ListView view = (ListView)sender;
            view.Columns[0].Width = view.Width - 30;
        }

        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = e.Handled = true;
                if (btnSearch.Enabled)
                    btnSearch.PerformClick();
            }
        }

    }
}
