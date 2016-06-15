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
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class BanGroupMember : RadegastForm
    {
        AvatarPicker picker;
        RadegastInstance instance;
        Netcom.RadegastNetcom netcom;
        GroupDetails parent;

        Group group;

        public BanGroupMember(RadegastInstance instance, Group group, GroupDetails parent)
            :base(instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(GroupInvite_Disposed);
            AutoSavePosition = true;

            this.instance = instance;
            this.group = group;
            this.netcom = instance.Netcom;
            this.parent = parent;

            picker = new AvatarPicker(instance) { Dock = DockStyle.Fill };
            Controls.Add(picker);
            picker.SelectionChaged += new EventHandler(picker_SelectionChaged);
            picker.BringToFront();
            
            netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void picker_SelectionChaged(object sender, EventArgs e)
        {
            btnBan.Enabled = picker.SelectedAvatars.Count > 0;
        }

        void GroupInvite_Disposed(object sender, EventArgs e)
        {
            netcom.ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);
            netcom = null;
            instance = null;
            picker.Dispose();
            Logger.DebugLog("Group picker disposed");
        }

        void Netcom_ClientDisconnected(object sender, OpenMetaverse.DisconnectedEventArgs e)
        {
            ((Radegast.Netcom.RadegastNetcom)sender).ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);

            if (!instance.MonoRuntime || IsHandleCreated)
                BeginInvoke(new MethodInvoker(() =>
                {
                    Close();
                }
                ));
        }


        private void GroupInvite_Load(object sender, EventArgs e)
        {
            picker.txtSearch.Focus();
        }

        private void btnIvite_Click(object sender, EventArgs e)
        {
            List<UUID> toBan = new List<UUID>();
            foreach (UUID key in picker.SelectedAvatars.Keys)
            {
                toBan.Add(key);
            }
            if (toBan.Count > 0)
            {
                Client.Groups.RequestBanAction(group.ID, GroupBanAction.Ban, toBan.ToArray(), (xs, xe) =>
                {
                    parent.RefreshBans();
                });
            }
            Close();
        }
    }
}
