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
using System.Windows.Forms;
using System.IO;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class AnimDetail : UserControl
    {
        private RadegastInstance instance;
        private Avatar av;
        private UUID anim;
        private int n;
        private List<FriendInfo> friends;
        private FriendInfo friend;
        private byte[] animData;

        public AnimDetail(RadegastInstance instance, Avatar av, UUID anim, int n)
        {
            InitializeComponent();
            Disposed += new EventHandler(AnimDetail_Disposed);
            this.instance = instance;
            this.av = av;
            this.anim = anim;
            this.n = n;

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void AnimDetail_Disposed(object sender, EventArgs e)
        {
        }

        private void AnimDetail_Load(object sender, EventArgs e)
        {
            if (Animations.ToDictionary().ContainsKey(anim))
            {
                Visible = false;
                Dispose();
                return;
            }

            groupBox1.Text = "Animation " + n + " (" + anim + ") for " + av.Name;

            friends = instance.Client.Friends.FriendList.FindAll(delegate(FriendInfo f) { return true; });

            pnlSave.Visible = false;
            boxAnimName.Text = "Animation " + n;
            cbFriends.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (FriendInfo f in friends)
            {
                cbFriends.Items.Add(f);
            }

            instance.Client.Assets.RequestAsset(anim, AssetType.Animation, true, Assets_OnAssetReceived);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WindowWrapper mainWindow = new WindowWrapper(frmMain.ActiveForm.Handle);
            System.Windows.Forms.SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.RestoreDirectory = true;
            dlg.Title = "Save animation as...";
            dlg.Filter = "Second Life Animation (*.sla)|*.sla";
            DialogResult res = dlg.ShowDialog();

            if (res == DialogResult.OK)
            {
                try
                {
                    File.WriteAllBytes(dlg.FileName, animData);
                }
                catch (Exception ex)
                {
                    Logger.Log("Saving animation failed: " + ex.Message, Helpers.LogLevel.Debug);
                }
            }
        }

        void Assets_OnAssetReceived(AssetDownload transfer, Asset asset)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => Assets_OnAssetReceived(transfer, asset)));
                return;
            }

            if (transfer.Success)
            {
                Logger.Log("Animation " + anim + " download success " + asset.AssetData.Length + " bytes.", Helpers.LogLevel.Debug);
                pnlSave.Visible = true;
                animData = asset.AssetData;
            }
            else
            {
                Logger.Log("Animation " + anim + " download failed.", Helpers.LogLevel.Debug);
                Visible = false;
                Dispose();
            }
        }

        private void playBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (playBox.CheckState == CheckState.Checked)
            {
                instance.Client.Self.AnimationStart(anim, true);
            }
            else
            {
                instance.Client.Self.AnimationStop(anim, true);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (animData == null) return;

            instance.Client.Inventory.RequestCreateItemFromAsset(animData, boxAnimName.Text, "(No description)", AssetType.Animation, InventoryType.Animation, instance.Client.Inventory.FindFolderForType(AssetType.Animation), On_ItemCreated);
            lblStatus.Text = "Uploading...";
            cbFriends.Enabled = false;
        }

        void On_ItemCreated(bool success, string status, UUID itemID, UUID assetID)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate()
                {
                    On_ItemCreated(success, status, itemID, assetID);
                }
                ));
                return;
            }

            if (!success)
            {
                lblStatus.Text = "Failed.";
                Logger.Log("Failed creating asset", Helpers.LogLevel.Debug);
                return;
            }
            else
            {
                Logger.Log("Created inventory item " + itemID.ToString(), Helpers.LogLevel.Info);

                lblStatus.Text = "Sending to " + friend.Name;
                Logger.Log("Sending item to " + friend.Name, Helpers.LogLevel.Info);

                instance.Client.Inventory.GiveItem(itemID, boxAnimName.Text, AssetType.Animation, friend.UUID, false);
                lblStatus.Text = "Sent";
            }

            cbFriends.Enabled = true;
        }


        private void cbFriends_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbFriends.SelectedIndex >= 0)
            {
                btnSend.Enabled = true;
                friend = friends[cbFriends.SelectedIndex];
            }
            else
            {
                btnSend.Enabled = false;
            }
        }
    }
}
