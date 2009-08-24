// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
// $Id: RadegastInstance.cs 152 2009-08-24 14:19:58Z latifer@gmail.com $
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class InventoryBackup : Form
    {
        private RadegastInstance instance;
        GridClient client { get { return instance.Client; } }
        private Inventory inv;
        private Thread backupThread;
        private string folderName;
        private int fetched = 0;

        public InventoryBackup(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new System.EventHandler(InventoryBackup_Disposed);
            this.instance = instance;
            inv = client.Inventory.Store;
        }

        void InventoryBackup_Disposed(object sender, System.EventArgs e)
        {

        }

        private void btnFolder_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.CheckFileExists = false;
            DialogResult res = openFileDialog1.ShowDialog();

            if (res == DialogResult.OK)
            {
                string dir = Path.GetDirectoryName(openFileDialog1.FileName);
                txtFolderName.Text = folderName = dir;
                btnFolder.Enabled = false;

                lvwFiles.Items.Clear();

                lblStatus.Text = "Fetching items...";
                sbrProgress.Style = ProgressBarStyle.Marquee;
                fetched = 0;

                if (backupThread != null)
                {
                    if (backupThread.IsAlive)
                        backupThread.Abort();
                    backupThread = null;
                }

                backupThread = new Thread(new ThreadStart(() =>
                {
                    TraverseDir(inv.RootNode, Path.DirectorySeparatorChar.ToString());
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        lblStatus.Text = string.Format("Done ({0} items saved).", fetched);
                        sbrProgress.Style = ProgressBarStyle.Blocks;
                        btnFolder.Enabled = true;
                    }
                    ));
                }
                ));

                backupThread.IsBackground = false;
                backupThread.Name = "Inventory Backup";
                backupThread.Start();

            }

        }

        private void TraverseDir(InventoryNode node, string path)
        {
            foreach (InventoryNode n in node.Nodes.Values)
            {
                if (n.Data is InventoryFolder)
                {
                    TraverseDir(n, Path.Combine(path, RadegastInstance.SafeFileName(n.Data.Name)));
                }
                else
                {
                    InventoryItem item = (InventoryItem)n.Data;

                    if (item.AssetType == AssetType.LSLText || item.AssetType == AssetType.Notecard)
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Text = n.Data.Name;
                        lvi.Tag = n.Data;
                        lvi.Name = n.Data.UUID.ToString();

                        string filePartial = Path.Combine(path, RadegastInstance.SafeFileName(n.Data.Name));
                        ListViewItem.ListViewSubItem fileName = new ListViewItem.ListViewSubItem(lvi, filePartial);
                        lvi.SubItems.Add(fileName);

                        ListViewItem.ListViewSubItem status = new ListViewItem.ListViewSubItem(lvi, "Fetching asset");
                        lvi.SubItems.Add(status);

                        BeginInvoke(new MethodInvoker(() =>
                        {
                            lvwFiles.Items.Add(lvi);
                            lvwFiles.EnsureVisible(lvwFiles.Items.Count - 1);
                        }));

                        Asset receivedAsset = null;
                        using (AutoResetEvent done = new AutoResetEvent(false))
                        {
                            client.Assets.RequestInventoryAsset(item, true, (AssetDownload transfer, Asset asset) =>
                                {
                                    if (transfer.Success)
                                    {
                                        receivedAsset = asset;
                                    }
                                    done.Set();
                                }
                            );

                            done.WaitOne(30 * 1000, false);
                        }

                        if (receivedAsset == null)
                        {
                            BeginInvoke(new MethodInvoker(() => status.Text = "Failed to fetch asset"));
                        }
                        else
                        {
                            BeginInvoke(new MethodInvoker(() => status.Text = "Saving..."));


                            string fullName = string.Empty;
                            string dirName = string.Empty;

                            try
                            {
                                switch (item.AssetType)
                                {
                                    case AssetType.Notecard:
                                        fullName = folderName + filePartial + ".txt";
                                        dirName = Path.GetDirectoryName(fullName);

                                        if (!Directory.Exists(dirName))
                                        {
                                            Directory.CreateDirectory(dirName);
                                        }

                                        AssetNotecard note = (AssetNotecard)receivedAsset;
                                        if (note.Decode())
                                        {
                                            File.WriteAllText(fullName, note.BodyText, System.Text.Encoding.UTF8);
                                        }
                                        break;

                                    case AssetType.LSLText:
                                        if ((item.Permissions.OwnerMask & (PermissionMask.Modify | PermissionMask.Copy | PermissionMask.Transfer)) == 0)
                                            break;
                                        fullName = folderName + filePartial + ".lsl";
                                        dirName = Path.GetDirectoryName(fullName);

                                        if (!Directory.Exists(dirName))
                                        {
                                            Directory.CreateDirectory(dirName);
                                        }

                                        AssetScriptText script = (AssetScriptText)receivedAsset;
                                        if (script.Decode())
                                        {
                                            File.WriteAllText(fullName, script.Source, System.Text.Encoding.UTF8);
                                        }
                                        break;

                                }

                                BeginInvoke(new MethodInvoker(() =>
                                {
                                    fileName.Text = fullName;
                                    status.Text = "Saved";
                                    lblStatus.Text = string.Format("Saved {0} items", ++fetched);
                                }));

                            }
                            catch (Exception ex)
                            {
                                BeginInvoke(new MethodInvoker(() => status.Text = "Failed to save " + Path.GetFileName(fullName) + ": " + ex.Message));
                            }

                        }
                    }
                }
            }
        }

        private void InventoryBackup_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backupThread != null)
            {
                if (backupThread.IsAlive)
                {
                    backupThread.Abort();
                    Thread.Sleep(1000);
                }
                backupThread = null;
            }
        }

        private void lvwFiles_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (lvwFiles.SelectedItems.Count == 1)
                {
                    ListViewItem item = lvwFiles.SelectedItems[0];

                    if (item.SubItems.Count >= 3)
                    {
                        if ("Saved" == item.SubItems[2].Text)
                        {
                            System.Diagnostics.Process.Start(item.SubItems[1].Text);
                        }
                    }
                }
            }
            catch (Exception) { }
        }
    }
}
