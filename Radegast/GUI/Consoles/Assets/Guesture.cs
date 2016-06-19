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
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class Guesture : DettachableControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private InventoryGesture gesture;
        private AssetGesture gestureAsset;

        public Guesture(RadegastInstance instance, InventoryGesture gesture)
        {
            InitializeComponent();
            Disposed += new EventHandler(Guesture_Disposed);

            if (!instance.advancedDebugging)
            {
                tbtnReupload.Visible = false;
            }

            this.instance = instance;
            this.gesture = gesture;

            // Start download
            tlblStatus.Text = "Downloading...";
            client.Assets.RequestAsset(gesture.AssetUUID, AssetType.Gesture, true, Assets_OnAssetReceived);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void Guesture_Disposed(object sender, EventArgs e)
        {
        }

        void Assets_OnAssetReceived(AssetDownload transfer, Asset asset)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { Assets_OnAssetReceived(transfer, asset); }));
                return;
            }

            if (!transfer.Success)
            {
                tlblStatus.Text = "Download failed";
                return;
            }

            tlblStatus.Text = "OK";
            tbtnPlay.Enabled = true;

            gestureAsset = (AssetGesture)asset;
            if (gestureAsset.Decode())
            {
                for (int i = 0; i < gestureAsset.Sequence.Count; i++)
                {
                    rtbInfo.AppendText(gestureAsset.Sequence[i].ToString().Trim() + Environment.NewLine);
                }
            }
        }

        #region Detach/Attach
        protected override void ControlIsNotRetachable()
        {
            tbtnAttach.Visible = false;
        }

        protected override void Detach()
        {
            base.Detach();
            tbtnAttach.Text = "Attach";
        }

        protected override void Retach()
        {
            base.Retach();
            tbtnAttach.Text = "Detach";
        }

        private void tbtnAttach_Click(object sender, EventArgs e)
        {
            if (Detached)
            {
                Retach();
            }
            else
            {
                Detach();
            }
        }
        #endregion

        private void tbtnPlay_Click(object sender, EventArgs e)
        {
            client.Self.PlayGesture(gesture.AssetUUID);
        }

        private void UpdateStatus(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { UpdateStatus(msg); }));
                return;
            }

            tlblStatus.Text = msg;
        }

        private void tbtnReupload_Click(object sender, EventArgs e)
        {
            UpdateStatus("Creating new item...");

            client.Inventory.RequestCreateItem(gesture.ParentUUID, "Copy of " + gesture.Name, gesture.Description, AssetType.Gesture, UUID.Random(), InventoryType.Gesture, PermissionMask.All,
                delegate(bool success, InventoryItem item)
                {
                    if (success)
                    {
                        UpdateStatus("Uploading data...");

                        client.Inventory.RequestUploadGestureAsset(gestureAsset.AssetData, item.UUID,
                            delegate(bool assetSuccess, string status, UUID itemID, UUID assetID)
                            {
                                if (assetSuccess)
                                {
                                    gesture.AssetUUID = assetID;
                                    UpdateStatus("OK");
                                }
                                else
                                {
                                    UpdateStatus("Asset failed");
                                }
                            }
                        );
                    }
                    else
                    {
                        UpdateStatus("Inv. failed");
                    }
                }
            );
        }
    }
}
