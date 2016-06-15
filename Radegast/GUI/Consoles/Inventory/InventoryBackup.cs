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
// $Id: RadegastInstance.cs 152 2009-08-24 14:19:58Z latifer@gmail.com $
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;
using OpenMetaverse.Imaging;

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
        private TextWriter csvFile = null;
        private int traversed = 0;
        private InventoryNode rootNode;

        public InventoryBackup(RadegastInstance instance, UUID rootFolder)
        {
            InitializeComponent();
            Disposed += new System.EventHandler(InventoryBackup_Disposed);

            this.instance = instance;

            inv = client.Inventory.Store;
            rootNode = inv.RootNode;
            if (inv.Items.ContainsKey(rootFolder) && inv.Items[rootFolder].Data is InventoryFolder)
            {
                rootNode = inv.GetNodeFor(rootFolder);
            }

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void InventoryBackup_Disposed(object sender, System.EventArgs e)
        {

        }

        private void WriteCSVLine(params object[] args)
        {
            if (csvFile != null)
            {
                try
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        string s = args[i].ToString();
                        s = s.Replace("\"", "\"\"");
                        csvFile.Write("\"{0}\"", s);
                        if (i != args.Length - 1) csvFile.Write(",");
                    }
                    csvFile.WriteLine();
                }
                catch { }
            }
        }

        private void btnFolder_Click(object sender, System.EventArgs e)
        {
            openFileDialog1.CheckFileExists = false;
            DialogResult res = openFileDialog1.ShowDialog();
            traversed = 0;

            if (res == DialogResult.OK)
            {
                string dir = Path.GetDirectoryName(openFileDialog1.FileName);
                txtFolderName.Text = folderName = dir;
                btnFolder.Enabled = false;

                lvwFiles.Items.Clear();

                if (cbList.Checked)
                {
                    try
                    {
                        csvFile = new StreamWriter(Path.Combine(dir, "inventory.csv"), false);
                        WriteCSVLine("Type", "Path", "Name", "Description", "Created", "Creator", "Last Owner");
                    }
                    catch
                    {
                        csvFile = null;
                    }
                }

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
                    TraverseDir(rootNode, Path.DirectorySeparatorChar.ToString());
                    if (csvFile != null)
                    {
                        try
                        {
                            csvFile.Close();
                            csvFile.Dispose();
                        }
                        catch { }
                    }
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
            var nodes = new List<InventoryNode>(node.Nodes.Values);
            foreach (InventoryNode n in nodes)
            {
                traversed++;
                try
                {
                    if (IsHandleCreated && (traversed % 13 == 0))
                    {
                        BeginInvoke(new MethodInvoker(() =>
                        {
                            lblStatus.Text = string.Format("Traversed {0} nodes...", traversed);
                        }));
                    }

                    if (n.Data is InventoryFolder)
                    {
                        WriteCSVLine("Folder", path, n.Data.Name, "", "", "", "");
                        TraverseDir(n, Path.Combine(path, RadegastInstance.SafeFileName(n.Data.Name)));
                    }
                    else
                    {
                        InventoryItem item = (InventoryItem)n.Data;
                        string creator = item.CreatorID == UUID.Zero ? string.Empty : instance.Names.Get(item.CreatorID, true);
                        string lastOwner = item.LastOwnerID == UUID.Zero ? string.Empty : instance.Names.Get(item.LastOwnerID, true);
                        string type = item.AssetType.ToString();
                        if (item.InventoryType == InventoryType.Wearable) type = ((WearableType)item.Flags).ToString();
                        string created = item.CreationDate.ToString("yyyy-MM-dd HH:mm:ss");
                        WriteCSVLine(type, path, item.Name, item.Description, created, creator, lastOwner);

                        PermissionMask fullPerm = PermissionMask.Modify | PermissionMask.Copy | PermissionMask.Transfer;
                        if ((item.Permissions.OwnerMask & fullPerm) != fullPerm)
                            continue;

                        string filePartial = Path.Combine(path, RadegastInstance.SafeFileName(n.Data.Name));
                        string fullName = folderName + filePartial;
                        switch (item.AssetType)
                        {
                            case AssetType.LSLText:
                                client.Settings.USE_ASSET_CACHE = false;
                                fullName += ".lsl";
                                break;
                            case AssetType.Notecard: fullName += ".txt"; break;
                            case AssetType.Texture: fullName += ".png"; break;
                            default: fullName += ".bin"; break;
                        }
                        string dirName = Path.GetDirectoryName(fullName);
                        bool dateOK = item.CreationDate > new DateTime(1970, 1, 2);

                        if (
                            (item.AssetType == AssetType.LSLText && cbScripts.Checked) ||
                            (item.AssetType == AssetType.Notecard && cbNoteCards.Checked) ||
                            (item.AssetType == AssetType.Texture && cbImages.Checked)
                            )
                        {
                            ListViewItem lvi = new ListViewItem();
                            lvi.Text = n.Data.Name;
                            lvi.Tag = n.Data;
                            lvi.Name = n.Data.UUID.ToString();

                            ListViewItem.ListViewSubItem fileName = new ListViewItem.ListViewSubItem(lvi, filePartial);
                            lvi.SubItems.Add(fileName);

                            ListViewItem.ListViewSubItem status = new ListViewItem.ListViewSubItem(lvi, "Fetching asset");
                            lvi.SubItems.Add(status);

                            //bool cached = dateOK && File.Exists(fullName) && File.GetCreationTimeUtc(fullName) == item.CreationDate;

                            //if (cached)
                            //{
                            //    status.Text = "Cached";
                            //}

                            BeginInvoke(new MethodInvoker(() =>
                            {
                                lvwFiles.Items.Add(lvi);
                                lvwFiles.EnsureVisible(lvwFiles.Items.Count - 1);
                            }));

                            //if (cached) continue;

                            Asset receivedAsset = null;
                            using (AutoResetEvent done = new AutoResetEvent(false))
                            {
                                if (item.AssetType == AssetType.Texture)
                                {
                                    client.Assets.RequestImage(item.AssetUUID, (state, asset) =>
                                    {
                                        if (state == TextureRequestState.Finished && asset != null && asset.Decode())
                                        {
                                            receivedAsset = asset;
                                            done.Set();
                                        }
                                    });
                                }
                                else
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
                                }

                                done.WaitOne(30 * 1000, false);
                            }

                            client.Settings.USE_ASSET_CACHE = true;

                            if (receivedAsset == null)
                            {
                                BeginInvoke(new MethodInvoker(() => status.Text = "Failed to fetch asset"));
                            }
                            else
                            {
                                BeginInvoke(new MethodInvoker(() => status.Text = "Saving..."));

                                try
                                {
                                    if (!Directory.Exists(dirName))
                                    {
                                        Directory.CreateDirectory(dirName);
                                    }

                                    switch (item.AssetType)
                                    {
                                        case AssetType.Notecard:
                                            AssetNotecard note = (AssetNotecard)receivedAsset;
                                            if (note.Decode())
                                            {
                                                File.WriteAllText(fullName, note.BodyText, System.Text.Encoding.UTF8);
                                                if (dateOK)
                                                {
                                                    File.SetCreationTimeUtc(fullName, item.CreationDate);
                                                    File.SetLastWriteTimeUtc(fullName, item.CreationDate);
                                                }
                                            }
                                            else
                                            {
                                                Logger.Log(string.Format("Falied to decode asset for '{0}' - {1}", item.Name, receivedAsset.AssetID), Helpers.LogLevel.Warning, client);
                                            }

                                            break;

                                        case AssetType.LSLText:
                                            AssetScriptText script = (AssetScriptText)receivedAsset;
                                            if (script.Decode())
                                            {
                                                File.WriteAllText(fullName, script.Source, System.Text.Encoding.UTF8);
                                                if (dateOK)
                                                {
                                                    File.SetCreationTimeUtc(fullName, item.CreationDate);
                                                    File.SetLastWriteTimeUtc(fullName, item.CreationDate);
                                                }
                                            }
                                            else
                                            {
                                                Logger.Log(string.Format("Falied to decode asset for '{0}' - {1}", item.Name, receivedAsset.AssetID), Helpers.LogLevel.Warning, client);
                                            }

                                            break;

                                        case AssetType.Texture:
                                            AssetTexture imgAsset = (AssetTexture)receivedAsset;
                                            var img = LoadTGAClass.LoadTGA(new MemoryStream(imgAsset.Image.ExportTGA()));
                                            img.Save(fullName, System.Drawing.Imaging.ImageFormat.Png);
                                            if (dateOK)
                                            {
                                                File.SetCreationTimeUtc(fullName, item.CreationDate);
                                                File.SetLastWriteTimeUtc(fullName, item.CreationDate);
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
                catch { }
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
