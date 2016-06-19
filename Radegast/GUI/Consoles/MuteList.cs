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
    public partial class MuteList : RadegastTabControl
    {
        public MuteList()
        {
            InitializeComponent();

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        public MuteList(RadegastInstance instance)
            : base(instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(MuteList_Disposed);
            RegisterClientEvents(client);
            instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
            RefreshMuteList();

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(e.Client);
        }

        void MuteList_Disposed(object sender, EventArgs e)
        {
            UnregisterClientEvents(client);
        }

        void RegisterClientEvents(GridClient client)
        {
            if (null == client) return;
            client.Self.MuteListUpdated += new EventHandler<EventArgs>(Self_MuteListUpdated);
        }

        void UnregisterClientEvents(GridClient client)
        {
            if (null == client) return;
            client.Self.MuteListUpdated -= new EventHandler<EventArgs>(Self_MuteListUpdated);
        }

        void Self_MuteListUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Self_MuteListUpdated(sender, e)));
                }
                return;
            }

            RefreshMuteList();
        }

        public void RefreshMuteList()
        {
            try
            {
                lvMuteList.BeginUpdate();
                lvMuteList.Items.Clear();

                client.Self.MuteList.ForEach((MuteEntry me) =>
                    {
                        string type = "";
                        switch (me.Type)
                        {
                            case MuteType.ByName: type = "Object by name"; break;
                            case MuteType.Object: type = "Object"; break;
                            case MuteType.Resident: type = "Resident"; break;
                            case MuteType.Group: type = "Group"; break;
                        }

                        var item = new ListViewItem(type);
                        item.Tag = me;
                        item.SubItems.Add(new ListViewItem.ListViewSubItem(item, me.Name));
                        lvMuteList.Items.Add(item);
                    }
                );
            }
            finally
            {
                lvMuteList.EndUpdate();
            }
        }

        private void lvMuteList_SizeChanged(object sender, EventArgs e)
        {
            chName.Width = Width - chType.Width - 130;
        }

        private void btnMuteByName_Click(object sender, EventArgs e)
        {
            txtMuteByName.Text = string.Empty;
            pnlMuteObjectByName.Visible = true;
            txtMuteByName.Select();
        }

        private void btnMuteObjectByName_Click(object sender, EventArgs e)
        {
            pnlMuteObjectByName.Visible = false;
            client.Self.UpdateMuteListEntry(MuteType.ByName, UUID.Zero, txtMuteByName.Text);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lvMuteList.Items.Clear();
            btnUnmute.Enabled = false;
            client.Self.RequestMuteList();
        }

        private void lvMuteList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvMuteList.SelectedItems.Count == 0)
            {
                btnUnmute.Enabled = false;
            }
            else
            {
                btnUnmute.Enabled = true;
            }
        }

        private void txtMuteByName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                e.SuppressKeyPress = e.Handled = true;
                btnMuteObjectByName.PerformClick();
            }
        }

        private void btnUnmute_Click(object sender, EventArgs e)
        {
            List<MuteEntry> toRemove = new List<MuteEntry>();
            foreach (ListViewItem item in lvMuteList.SelectedItems)
            {
                if (item.Tag is MuteEntry)
                {
                    toRemove.Add((MuteEntry)item.Tag);
                }
            }

            foreach (var item in toRemove)
            {
                client.Self.RemoveMuteListEntry(item.ID, item.Name);
            }
        }

        private void btnMuteResident_Click(object sender, EventArgs e)
        {
            new MuteResidentForm(instance).ShowDialog();
        }
    }
}
