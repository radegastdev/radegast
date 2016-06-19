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
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Imaging;
using OpenMetaverse.Assets;
using System.IO;

namespace Radegast
{
    public partial class SLImageHandler : DettachableControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private UUID imageID;

        byte[] jpegdata;
        ManagedImage imgManaged;
        Image image;
        bool allowSave = false;

        public event EventHandler<ImageUpdatedEventArgs> ImageUpdated;

        public PictureBoxSizeMode SizeMode
        {
            get { return pictureBox1.SizeMode; }
            set { pictureBox1.SizeMode = value; }
        }

        public override string Text
        {
            get
            {
                
                return base.Text;
            }
            set
            {
                base.Text = value;

                if (image != null)
                {
                    base.Text += string.Format(" ({0}x{1})", image.Width, image.Height);
                }

                SetTitle();
            }
        }

        public bool AllowUpdateImage = false;

        public SLImageHandler()
        {
            InitializeComponent();

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        public SLImageHandler(RadegastInstance instance, UUID image, string label)
            : this(instance, image, label, false)
        {
        }

        public SLImageHandler(RadegastInstance instance, UUID image, string label, bool allowSave)
        {
            this.allowSave = allowSave;
            InitializeComponent();
            Init(instance, image, label);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        public void Init(RadegastInstance instance, UUID image, string label)
        {
            Disposed += new EventHandler(SLImageHandler_Disposed);
            pictureBox1.AllowDrop = true;

            this.instance = instance;
            this.imageID = image;

            Text = string.IsNullOrEmpty(label) ? "Image" : label;

            if (image == UUID.Zero)
            {
                progressBar1.Hide();
                lblProgress.Hide();
                pictureBox1.Enabled = true;
                return;
            }

            // Callbacks
            client.Assets.ImageReceiveProgress += new EventHandler<ImageReceiveProgressEventArgs>(Assets_ImageReceiveProgress);
            UpdateImage(imageID);
        }

        public void UpdateImage(UUID imageID)
        {
            this.imageID = imageID;
            progressBar1.Visible = true;
            pictureBox1.Image = null;
            
            if (imageID == UUID.Zero)
            {
                progressBar1.Visible = false;
                return;
            }

            client.Assets.RequestImage(imageID, ImageType.Normal, 101300.0f, 0, 0, delegate(TextureRequestState state, AssetTexture assetTexture)
            {
                if (state == TextureRequestState.Finished || state == TextureRequestState.Timeout)
                {
                    Assets_OnImageReceived(assetTexture);
                }
                else if (state == TextureRequestState.Progress)
                {
                    // DisplayPartialImage(assetTexture);
                }
            },
            true);
        }

        void SLImageHandler_Disposed(object sender, EventArgs e)
        {
            client.Assets.ImageReceiveProgress -= new EventHandler<ImageReceiveProgressEventArgs>(Assets_ImageReceiveProgress);
        }

        void Assets_ImageReceiveProgress(object sender, ImageReceiveProgressEventArgs e)
        {
            if (this.imageID != e.ImageID)
            {
                return;
            }

            if (InvokeRequired)
            {
                if (IsHandleCreated || !instance.MonoRuntime)
                    BeginInvoke(new MethodInvoker(() => Assets_ImageReceiveProgress(sender, e)));
                return;
            }

            int pct = 0;
            if (e.Total> 0)
            {
                pct = (e.Received * 100) / e.Total;
            }
            if (pct < 0 || pct > 100)
            {
                return;
            }
            lblProgress.Text = String.Format("{0} of {1}KB ({2}%)", (int)e.Received / 1024, (int)e.Total / 1024, pct);
            progressBar1.Value = pct;
        }

        void DisplayPartialImage(AssetTexture assetTexture)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated || !instance.MonoRuntime)
                    BeginInvoke(new MethodInvoker(() => DisplayPartialImage(assetTexture)));
                return;
            }

            try
            {
                ManagedImage tmp;
                System.Drawing.Image img;
                if (OpenJPEG.DecodeToImage(assetTexture.AssetData, out tmp, out img))
                {
                    pictureBox1.Image = img;
                    pictureBox1.Enabled = true;
                }
            }
            catch (Exception) { }
        }

        private void Assets_OnImageReceived(AssetTexture assetTexture)
        {
            if (assetTexture.AssetID != this.imageID)
            {
                return;
            }

            if (InvokeRequired)
            {
                if (IsHandleCreated || !instance.MonoRuntime)
                    BeginInvoke(new MethodInvoker(() => Assets_OnImageReceived(assetTexture)));
                return;
            }

            try
            {
                progressBar1.Hide();
                lblProgress.Hide();

                if (!OpenJPEG.DecodeToImage(assetTexture.AssetData, out imgManaged, out image))
                {
                    throw new Exception("decoding failure");
                }

                Text = Text; // yeah, really ;)

                pictureBox1.Image = image;
                pictureBox1.Enabled = true;
                jpegdata = assetTexture.AssetData;
                if (Detached)
                {
                    ClientSize = pictureBox1.Size = new Size(image.Width, image.Height);
                }
            }
            catch (Exception excp)
            {
                this.Hide();
                System.Console.WriteLine("Error decoding image: " + excp.Message);
            }
        }

        protected override void Detach()
        {
            base.Detach();
            if (image != null)
            {
                ClientSize = pictureBox1.Size = new Size(image.Width, image.Height);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (imgManaged == null)
            {
                return;
            }

            System.Windows.Forms.SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.RestoreDirectory = true;
            dlg.Title = "Save image as...";
            dlg.Filter = "Targa (*.tga)|*.tga|Jpeg2000 (*.j2c)|*.j2c|PNG (*.png)|*.png|Jpeg (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp";



            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int type = dlg.FilterIndex;
                if (type == 2)
                { // jpeg200
                    File.WriteAllBytes(dlg.FileName, jpegdata);
                }
                else if (type == 1)
                { // targa
                    File.WriteAllBytes(dlg.FileName, imgManaged.ExportTGA());
                }
                else if (type == 3)
                { // png
                    image.Save(dlg.FileName, ImageFormat.Png);
                }
                else if (type == 4)
                { // jpg
                    image.Save(dlg.FileName, ImageFormat.Jpeg);
                }
                else
                { // BMP
                    image.Save(dlg.FileName, ImageFormat.Bmp);
                }
            }

            dlg.Dispose();
            dlg = null;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!Detached)
            {
                Detached = true;
            }
        }

        private void copyUUIDToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(imageID.ToString(), TextDataFormat.Text);
        }

        private void tbtnCopy_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(pictureBox1.Image);
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            TreeNode node = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (!AllowUpdateImage || node == null)
            {
                e.Effect = DragDropEffects.None;
            }
            else if (node.Tag is InventorySnapshot || node.Tag is InventoryTexture)
            {
                e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode node = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (!AllowUpdateImage || node == null) return;

            if (node.Tag is InventorySnapshot || node.Tag is InventoryTexture)
            {
                UUID imgID = UUID.Zero;
                if (node.Tag is InventorySnapshot)
                {
                    imgID = ((InventorySnapshot)node.Tag).AssetUUID;
                }
                else
                {
                    imgID = ((InventoryTexture)node.Tag).AssetUUID;
                }

                var handler = ImageUpdated;
                if (handler != null)
                {
                    handler(this, new ImageUpdatedEventArgs(imgID));
                }
            }
        }

        private void cmsImage_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            if (AllowUpdateImage)
            {
                tbtnClear.Visible = tbtnPaste.Visible = true;
                tbtnPaste.Enabled = false;
                if (instance.InventoryClipboard != null)
                {
                    if (instance.InventoryClipboard.Item is InventoryTexture ||
                        instance.InventoryClipboard.Item is InventorySnapshot)
                    {
                        tbtnPaste.Enabled = true;
                    }
                }
            }
            else
            {
                tbtnClear.Visible = tbtnPaste.Visible = false;
            }

            tbtbInvShow.Enabled = false;

            InventoryItem found = null;
            foreach (var traversed in client.Inventory.Store.Items.Values)
            {
                if (traversed.Data is InventoryItem)
                {
                    InventoryItem item = (InventoryItem)traversed.Data;
                    if (item.AssetUUID == imageID)
                    {
                        found = item;
                        break;
                    }
                }
            }

            bool save = allowSave;

            if (found == null)
            {
                tbtbInvShow.Enabled = false;
                tbtbInvShow.Tag = null;
            }
            else
            {
                tbtbInvShow.Enabled = true;
                tbtbInvShow.Tag = found;
                save |= InventoryConsole.IsFullPerm(found);
            }

            save |= instance.advancedDebugging;

            if (save)
            {
                tbtnCopy.Visible = true;
                tbtnCopyUUID.Visible = true;
                tbtnSave.Visible = true;
            }
            else
            {
                tbtnCopy.Visible = false;
                tbtnCopyUUID.Visible = false;
                tbtnSave.Visible = false;
            }
        }


        private void tbtnClear_Click(object sender, EventArgs e)
        {
            if (AllowUpdateImage)
            {
                UpdateImage(UUID.Zero);
                var handler = ImageUpdated;
                if (handler != null)
                {
                    handler(this, new ImageUpdatedEventArgs(UUID.Zero));
                }
            }
        }

        private void tbtnPaste_Click(object sender, EventArgs e)
        {
            if (!AllowUpdateImage) return;
            if (instance.InventoryClipboard != null)
            {
                UUID newID = UUID.Zero;

                if (instance.InventoryClipboard.Item is InventoryTexture)
                {
                    newID = ((InventoryTexture)instance.InventoryClipboard.Item).AssetUUID;
                }
                else if (instance.InventoryClipboard.Item is InventorySnapshot)
                {
                    newID = ((InventorySnapshot)instance.InventoryClipboard.Item).AssetUUID;
                }
                else
                {
                    return;
                }

                UpdateImage(newID);

                var handler = ImageUpdated;
                if (handler != null)
                {
                    handler(this, new ImageUpdatedEventArgs(newID));
                }
            }
        }

        private void tbtbInvShow_Click(object sender, EventArgs e)
        {
            if (tbtbInvShow.Tag == null || !(tbtbInvShow.Tag is InventoryItem)) return;
            
            InventoryItem item = (InventoryItem)tbtbInvShow.Tag;

            if (instance.TabConsole.TabExists("inventory"))
            {
                instance.TabConsole.SelectTab("inventory");
                InventoryConsole inv = (InventoryConsole)instance.TabConsole.Tabs["inventory"].Control;
                inv.SelectInventoryNode(item.UUID);
            }
        }
    }

    public class ImageUpdatedEventArgs : EventArgs
    {
        public UUID NewImageID;

        public ImageUpdatedEventArgs(UUID imageID)
        {
            this.NewImageID = imageID;
        }
    }
}
