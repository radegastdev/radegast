using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using RadegastNc;
using OpenMetaverse;
using OpenMetaverse.Imaging;
using OpenMetaverse.Assets;
using System.IO;

namespace Radegast
{
    public partial class SLImageHandler : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private UUID imageID;
        private ImageCache cache;

        public SLImageHandler(RadegastInstance instance, UUID image, string label)
        {
            InitializeComponent();
            Disposed += new EventHandler(SLImageHandler_Disposed);
  
            if (!instance.advancedDebugging)
            {
                topPanel.Visible = false;
                pnlSave.Visible = false;
                btnSave.Visible = false;
            }

            this.instance = instance;
            this.imageID = image;
            this.cache = this.instance.ImageCache;
            lblDesc.Text = label;
            tboxImageId.Text = image.ToString();

            // Callbacks
            client.Assets.OnImageRecieveProgress += new AssetManager.ImageReceiveProgressCallback(Assets_OnImageProgress);

            if (cache.ContainsImage(image)) {
                pictureBox1.Image = cache.GetImage(imageID);
                pictureBox1.Enabled = true;
                btnSave.Enabled = true;
                pnlProgress.Hide();
            } else {
                client.Assets.RequestImage(imageID, ImageType.Normal, delegate(TextureRequestState state, AssetTexture assetTexture)
                {
                    if (state == TextureRequestState.Finished || state == TextureRequestState.Timeout)
                    {
                        Assets_OnImageReceived(assetTexture);
                    }
                    else if (state == TextureRequestState.Progress)
                    {
                        DisplayPartialImage(assetTexture);
                    }
                }, true);
            }
        }

        void SLImageHandler_Disposed(object sender, EventArgs e)
        {
            client.Assets.OnImageRecieveProgress -= new AssetManager.ImageReceiveProgressCallback(Assets_OnImageProgress);
        }

        private void Assets_OnImageProgress(UUID imageID, int recieved, int total)
        {
            if (this.imageID != imageID) {
                return;
            }
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate()
                {
                    Assets_OnImageProgress(imageID, recieved, total);
                }));
            }
            int pct = 0;
            if (total > 0)
            {
                pct = (recieved * 100) / total;
            }
            if (pct < 0 || pct > 100)
            {
                return;
            }
            lblProgress.Text = String.Format("{0} of {1}KB ({2}%)", (int)recieved / 1024, (int)total / 1024, pct);
            progressBar1.Value = pct;
        }

        void DisplayPartialImage(AssetTexture assetTexture)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    DisplayPartialImage(assetTexture);
                }));
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

            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate()
                {
                    Assets_OnImageReceived(assetTexture);
                }));
                return;
            }

            try {
                btnSave.Enabled = true;
                pnlProgress.Hide();
                ManagedImage tmp;
                System.Drawing.Image img;
                if (!OpenJPEG.DecodeToImage(assetTexture.AssetData, out tmp, out img))
                {
                    throw new Exception("decoding failure");
                }
                pictureBox1.Image = img;
                pictureBox1.Enabled = true;
                cache.AddImage(assetTexture.AssetID, img);
                cache.AddJ2Image(assetTexture.AssetID, assetTexture.AssetData);
            } catch (Exception excp) {
                this.Hide();
                System.Console.WriteLine("Error decoding image: " + excp.Message);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dlg = new SaveFileDialog();
            dlg.AddExtension = true;
            dlg.RestoreDirectory = true;
            dlg.Title = "Save image as...";
            dlg.Filter = "Targa (*.tga)|*.tga|Jpeg2000 (*.j2c)|*.j2c|PNG (*.png)|*.png|Jpeg (*.jpg)|*.jpg|Bitmap (*.bmp)|*.bmp";

            

            if (dlg.ShowDialog() == DialogResult.OK) {
                int type = dlg.FilterIndex;
                if (type == 2) { // jpeg200
                    File.WriteAllBytes(dlg.FileName, cache.GetJ2Image(imageID));
                } else if (type == 1) { // targa
                    File.WriteAllBytes(dlg.FileName, new ManagedImage(new Bitmap(cache.GetImage(imageID))).ExportTGA());
                } else if (type == 3) { // png
                    cache.GetImage(imageID).Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Png);
                } else if (type == 4) { // jpg
                    cache.GetImage(imageID).Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                } else { // BMP
                    cache.GetImage(imageID).Save(dlg.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
                }
            }
            
            dlg.Dispose();
            dlg = null;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ImageFullSize img = new ImageFullSize(pictureBox1.Image);
            img.Show();
        }
    }
}
