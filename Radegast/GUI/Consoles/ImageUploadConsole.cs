/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.IO;
using OpenMetaverse;
using OpenMetaverse.Imaging;

namespace Radegast
{
    public partial class ImageUploadConsole : RadegastTabControl
    {
        public string FileName, TextureName, TextureDescription;
        public byte[] UploadData;
        public UUID InventoryID, AssetID, TransactionID;
        bool ImageLoaded;
        int OriginalCapsTimeout;

        public ImageUploadConsole()
        {
            InitializeComponent();

            GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        public ImageUploadConsole(RadegastInstance instance)
            : base(instance)
        {
            InitializeComponent();

            Disposed += new EventHandler(ImageUploadConsole_Disposed);
            instance.Netcom.ClientConnected += new EventHandler<EventArgs>(Netcom_ClientConnected);
            instance.Netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);
            client.Assets.AssetUploaded += new EventHandler<AssetUploadEventArgs>(Assets_AssetUploaded);
            UpdateButtons();
            OriginalCapsTimeout = client.Settings.CAPS_TIMEOUT;

            GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void ImageUploadConsole_Disposed(object sender, EventArgs e)
        {
            client.Assets.AssetUploaded -= new EventHandler<AssetUploadEventArgs>(Assets_AssetUploaded);
        }

        void Assets_AssetUploaded(object sender, AssetUploadEventArgs e)
        {
            if (e.Upload.ID == TransactionID)
            {
                if (!e.Upload.Success)
                {
                    TempUploadHandler(false, new InventoryTexture(UUID.Zero));
                }
                else
                {
                    client.Inventory.RequestCreateItem(client.Inventory.FindFolderForType(AssetType.Texture),
                        TextureName, TextureDescription, AssetType.Texture, TransactionID,
                        InventoryType.Texture, PermissionMask.All, TempUploadHandler);
                }
            }
        }

        void Netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Netcom_ClientDisconnected(sender, e)));
                }
                return;
            }

            UpdateButtons();
        }

        void Netcom_ClientConnected(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Netcom_ClientConnected(sender, e)));
                }
                return;
            }

            UpdateButtons();
        }

        private bool IsPowerOfTwo(uint n)
        {
            return (n & (n - 1)) == 0 && n != 0;
        }

        private int ClosestPowerOwTwo(int n)
        {
            int res = 1;

            while (res < n)
            {
                res <<= 1;
            }

            return res > 1 ? res / 2 : 1;
        }

        public void LoadImage(string fname)
        {
            FileName = fname;

            if (String.IsNullOrEmpty(FileName))
                return;

            txtStatus.AppendText("Loading...\n");

            string extension = Path.GetExtension(FileName).ToLower();

            try
            {
                Bitmap bitmap = null;
                switch (extension)
                {
                    case ".jp2":
                    case ".j2c":
                        Image image;
                        ManagedImage managedImage;

                        // Upload JPEG2000 images untouched
                        UploadData = File.ReadAllBytes(FileName);

                        OpenJPEG.DecodeToImage(UploadData, out managedImage, out image);
                        bitmap = (Bitmap)image;

                        txtStatus.AppendText("Loaded raw JPEG2000 data " + FileName + "\n");
                        break;
                    case ".tga":
                        bitmap = LoadTGAClass.LoadTGA(FileName);
                        break;
                    default:
                        bitmap = (Bitmap)Image.FromFile(FileName);
                        break;
                }

                txtStatus.AppendText("Loaded image " + FileName + "\n");

                int width = bitmap.Width;
                int height = bitmap.Height;

                // Handle resizing to prevent excessively large images and irregular dimensions
                if (!IsPowerOfTwo((uint)width) || !IsPowerOfTwo((uint)height) || width > 1024 || height > 1024)
                {
                    txtStatus.AppendText("Image has irregular dimensions " + width + "x" + height + "\n");

                    width = ClosestPowerOwTwo(width);
                    height = ClosestPowerOwTwo(height);

                    width = width > 1024 ? 1024 : width;
                    height = height > 1024 ? 1024 : height;

                    txtStatus.AppendText("Resizing to " + width + "x" + height + "\n");

                    Bitmap resized = new Bitmap(width, height, bitmap.PixelFormat);
                    Graphics graphics = Graphics.FromImage(resized);

                    graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    graphics.InterpolationMode =
                       System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    graphics.DrawImage(bitmap, 0, 0, width, height);

                    bitmap.Dispose();
                    bitmap = resized;
                }

                txtStatus.AppendText("Encoding image...\n");

                UploadData = OpenJPEG.EncodeFromImage(bitmap, chkLossless.Checked);

                txtStatus.AppendText("Finished encoding.\n");
                ImageLoaded = true;
                UpdateButtons();
                txtAssetID.Text = UUID.Zero.ToString();

                pbPreview.Image = bitmap;
                lblSize.Text = string.Format("{0}x{1} {2} KB", bitmap.Width, bitmap.Height, Math.Round((double)UploadData.Length / 1024.0d, 2));
            }
            catch (Exception ex)
            {
                UploadData = null;
                btnSave.Enabled = false;
                btnUpload.Enabled = false;
                txtStatus.AppendText(string.Format("Failed to load the image:\n{0}\n", ex.Message));
            }
        }

        void UpdateButtons()
        {
            btnLoad.Enabled = true;

            if (ImageLoaded)
            {
                btnSave.Enabled = true;
            }
            else
            {
                btnSave.Enabled = false;
            }

            if (ImageLoaded && client.Network.Connected)
            {
                btnUpload.Enabled = true;
            }
            else
            {
                btnUpload.Enabled = false;
            }
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter =
                    "Image Files (*.jp2,*.j2c,*.jpg,*.jpeg,*.gif,*.png,*.bmp,*.tga,*.tif,*.tiff,*.ico,*.wmf,*.emf)|" +
                    "*.jp2;*.j2c;*.jpg;*.jpeg;*.gif;*.png;*.bmp;*.tga;*.tif;*.tiff;*.ico;*.wmf;*.emf|" +
                    "All files (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                LoadImage(dialog.FileName);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            SaveFileDialog dlg = new SaveFileDialog
            {
                AddExtension = true,
                RestoreDirectory = true,
                Title = "Save image as...",
                Filter =
                    "PNG (*.png)|*.png|Targa (*.tga)|*.tga|Jpeg2000 (*.j2c)|*.j2c|Jpeg (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp"
            };



            if (dlg.ShowDialog() == DialogResult.OK)
            {
                int type = dlg.FilterIndex;
                if (type == 3)
                { // jpeg2000
                    File.WriteAllBytes(dlg.FileName, UploadData);
                }
                else if (type == 2)
                { // targa
                    ManagedImage imgManaged;
                    OpenJPEG.DecodeToImage(UploadData, out imgManaged);
                    File.WriteAllBytes(dlg.FileName, imgManaged.ExportTGA());
                }
                else if (type == 1)
                { // png
                    pbPreview.Image.Save(dlg.FileName, ImageFormat.Png);
                }
                else if (type == 4)
                { // jpg
                    pbPreview.Image.Save(dlg.FileName, ImageFormat.Jpeg);
                }
                else
                { // BMP
                    pbPreview.Image.Save(dlg.FileName, ImageFormat.Bmp);
                }
            }

            dlg.Dispose();
        }

        private void TempUploadHandler(bool success, InventoryItem item)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => TempUploadHandler(success, item)));
                }
                return;
            }

            InventoryID = item.UUID;

            UpdateButtons();
            txtAssetID.Text = AssetID.ToString();

            if (!success)
            {
                txtStatus.AppendText("Upload failed.\n");
                return;
            }

            txtStatus.AppendText("Upload success.\n");
            txtStatus.AppendText("New image ID: " + AssetID + "\n");
        }

        private void UploadHandler(bool success, string status, UUID itemID, UUID assetID)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => UploadHandler(success, status, itemID, assetID)));
                }
                return;
            }

            client.Settings.CAPS_TIMEOUT = OriginalCapsTimeout;

            AssetID = assetID;
            InventoryID = itemID;

            UpdateButtons();
            txtAssetID.Text = AssetID.ToString();

            if (!success)
            {
                txtStatus.AppendText("Upload failed: " + status + "\n");
                return;
            }

            txtStatus.AppendText("Upload success.\n");
            txtStatus.AppendText("New image ID: " + AssetID + "\n");
        }

        private void btnUpload_Click(object sender, EventArgs e)
        {
            bool tmp = chkTemp.Checked;
            txtStatus.AppendText("Uploading...");
            btnLoad.Enabled = false;
            btnUpload.Enabled = false;
            AssetID = InventoryID = UUID.Zero;

            TextureName = Path.GetFileNameWithoutExtension(FileName);
            if (tmp) TextureName += " (temp)";
            TextureDescription = $"Uploaded with Radegast on {DateTime.Now.ToLongDateString()}";

            Permissions perms = new Permissions {EveryoneMask = PermissionMask.All, NextOwnerMask = PermissionMask.All};

            if (!tmp)
            {
                client.Settings.CAPS_TIMEOUT = 180 * 1000;
                client.Inventory.RequestCreateItemFromAsset(UploadData, TextureName, TextureDescription, AssetType.Texture, InventoryType.Texture,
                    client.Inventory.FindFolderForType(AssetType.Texture), perms, UploadHandler);
            }
            else
            {
                TransactionID = UUID.Random();
                client.Assets.RequestUpload(out AssetID, AssetType.Texture, UploadData, true, TransactionID);
            }
        }
    }
}
