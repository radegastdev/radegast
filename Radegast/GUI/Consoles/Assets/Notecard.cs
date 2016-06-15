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
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class Notecard : DettachableControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private InventoryNotecard notecard;
        private AssetNotecard receivedNotecard;
        private Primitive prim;

        public Notecard(RadegastInstance instance, InventoryNotecard notecard)
            : this(instance, notecard, null)
        {
        }

        public Notecard(RadegastInstance instance, InventoryNotecard notecard, Primitive prim)
        {
            InitializeComponent();
            Disposed += new EventHandler(Notecard_Disposed);

            this.instance = instance;
            this.notecard = notecard;
            this.prim = prim;

            Text = notecard.Name;

            rtbContent.DetectUrls = false;


            if (notecard.AssetUUID == UUID.Zero)
            {
                UpdateStatus("Blank");
            }
            else
            {
                rtbContent.Text = " ";
                UpdateStatus("Loading...");

                if (prim == null)
                {
                    client.Assets.RequestInventoryAsset(notecard, true, Assets_OnAssetReceived);
                }
                else
                {
                    client.Assets.RequestInventoryAsset(notecard.AssetUUID, notecard.UUID, prim.ID, prim.OwnerID, notecard.AssetType, true, Assets_OnAssetReceived);
                }
            }

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void Notecard_Disposed(object sender, EventArgs e)
        {
        }

        void Assets_OnAssetReceived(AssetDownload transfer, Asset asset)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => Assets_OnAssetReceived(transfer, asset)));
                return;
            }

            if (transfer.Success)
            {
                AssetNotecard n = (AssetNotecard)asset;
                n.Decode();
                receivedNotecard = n;

                string noteText = string.Empty;
                rtbContent.Clear();

                for (int i = 0; i < n.BodyText.Length; i++)
                {
                    char c = n.BodyText[i];

                    if ((int)c == 0xdbc0)
                    {
                        int index = (int)n.BodyText[++i] - 0xdc00;
                        InventoryItem e = n.EmbeddedItems[index];
                        rtbContent.AppendText(noteText);
                        rtbContent.InsertLink(e.Name, string.Format("radegast://embeddedasset/{0}", index));
                        noteText = string.Empty;
                    }
                    else
                    {
                        noteText += c;
                    }
                }

                rtbContent.Text += noteText;

                if (n.EmbeddedItems != null && n.EmbeddedItems.Count > 0)
                {
                    tbtnAttachments.Enabled = true;
                    tbtnAttachments.Visible = true;
                    foreach (InventoryItem item in n.EmbeddedItems)
                    {
                        int ix = InventoryConsole.GetItemImageIndex(item.AssetType.ToString().ToLower());
                        ToolStripMenuItem titem = new ToolStripMenuItem(item.Name);

                        if (ix != -1)
                        {
                            titem.Image = frmMain.ResourceImages.Images[ix];
                            titem.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                        }
                        else
                        {
                            titem.DisplayStyle = ToolStripItemDisplayStyle.Text;
                        }

                        titem.Name = item.UUID.ToString(); ;
                        titem.Tag = item;
                        titem.Click += new EventHandler(attachmentMenuItem_Click);

                        var saveToInv = new ToolStripMenuItem("Save to inventory");
                        saveToInv.Click += (object xsender, EventArgs xe) =>
                            {
                                client.Inventory.RequestCopyItemFromNotecard(UUID.Zero,
                                    notecard.UUID,
                                    client.Inventory.FindFolderForType(item.AssetType),
                                    item.UUID,
                                    Inventory_OnInventoryItemCopied);
                            };

                        titem.DropDownItems.Add(saveToInv);
                        tbtnAttachments.DropDownItems.Add(titem);
                    }
                }
                UpdateStatus("OK");
                rtbContent.Focus();
            }
            else
            {
                UpdateStatus("Failed");
                rtbContent.Text = "Failed to download notecard. " + transfer.Status;
            }
        }

        private void Inventory_OnInventoryItemCopied(InventoryBase item)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => Inventory_OnInventoryItemCopied(item)));
                return;
            }

            if (null == item) return;

            instance.TabConsole.DisplayNotificationInChat(
                string.Format("{0} saved to inventory", item.Name),
                ChatBufferTextStyle.Invisible);

            tlblStatus.Text = "Saved";
            
            if (item is InventoryNotecard)
            {
                Notecard nc = new Notecard(instance, (InventoryNotecard)item);
                nc.pnlKeepDiscard.Visible = true;
                nc.ShowDetached();
            }
        }

        void attachmentMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem titem = (ToolStripMenuItem)sender;
                InventoryItem item = (InventoryItem)titem.Tag;

                switch (item.AssetType)
                {
                    case AssetType.Texture:
                        SLImageHandler ih = new SLImageHandler(instance, item.AssetUUID, string.Empty);
                        ih.Text = item.Name;
                        ih.ShowDetached();
                        break;

                    case AssetType.Landmark:
                        Landmark ln = new Landmark(instance, (InventoryLandmark)item);
                        ln.ShowDetached();
                        break;

                    case AssetType.Notecard:
                        client.Inventory.RequestCopyItemFromNotecard(UUID.Zero,
                            notecard.UUID,
                            notecard.ParentUUID,
                            item.UUID,
                            Inventory_OnInventoryItemCopied);
                        break;
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (notecard.AssetUUID == UUID.Zero) return;

            rtbContent.Text = "Loading...";
            client.Assets.RequestInventoryAsset(notecard, true, Assets_OnAssetReceived);
        }

        private void rtbContent_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            //instance.MainForm.processLink(e.LinkText);
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
            tbtnExit.Enabled = true;
        }

        protected override void Retach()
        {
            base.Retach();
            tbtnAttach.Text = "Detach";
            tbtnExit.Enabled = false;
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

        private void tbtnExit_Click(object sender, EventArgs e)
        {
            if (Detached)
            {
                FindForm().Close();
            }
        }

        private void tbtnSave_Click(object sender, EventArgs e)
        {
            bool success = false;
            string message = "";
            AssetNotecard n = new AssetNotecard();
            n.BodyText = rtbContent.Text;
            n.EmbeddedItems = new List<InventoryItem>();

            if (receivedNotecard != null)
            {
                for (int i = 0; i < receivedNotecard.EmbeddedItems.Count; i++)
                {
                    n.EmbeddedItems.Add(receivedNotecard.EmbeddedItems[i]);
                    int indexChar = 0xdc00 + i;
                    n.BodyText += (char)0xdbc0;
                    n.BodyText += (char)indexChar;
                }
            }

            n.Encode();

            UpdateStatus("Saving...");

            InventoryManager.InventoryUploadedAssetCallback handler = delegate(bool uploadSuccess, string status, UUID itemID, UUID assetID)
                    {
                        success = uploadSuccess;
                        if (itemID == notecard.UUID)
                        {
                            if (success)
                            {
                                UpdateStatus("OK");
                                notecard.AssetUUID = assetID;
                            }
                            else
                            {
                                UpdateStatus("Failed");
                            }

                        }
                        message = status ?? "Unknown error uploading notecard asset";
                    };

            if (prim == null)
            {
                client.Inventory.RequestUploadNotecardAsset(n.AssetData, notecard.UUID, handler);
            }
            else
            {
                client.Inventory.RequestUpdateNotecardTask(n.AssetData, notecard.UUID, prim.ID, handler);
            }
        }

        void UpdateStatus(string status)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => UpdateStatus(status)));
                return;
            }
            instance.TabConsole.DisplayNotificationInChat("Notecard status: " + status, ChatBufferTextStyle.Invisible);
            tlblStatus.Text = status;
        }

        private void rtbContent_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                if (e.Shift)
                {
                }
                else
                {
                    tbtnSave_Click(this, EventArgs.Empty);
                    e.Handled = e.SuppressKeyPress = true;
                }
            }

        }

        private void rtbContent_Enter(object sender, EventArgs e)
        {
            instance.TabConsole.DisplayNotificationInChat("Editing notecard", ChatBufferTextStyle.Invisible);
        }

        private void btnKeep_Click(object sender, EventArgs e)
        {
            Retach();
        }

        private void btnDiscard_Click(object sender, EventArgs e)
        {
            client.Inventory.MoveItem(notecard.UUID, client.Inventory.FindFolderForType(FolderType.Trash), notecard.Name);
            Retach();
        }
    }
}
