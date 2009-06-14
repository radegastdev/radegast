using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class Notecard : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private InventoryNotecard notecard;

        public Notecard(RadegastInstance instance, InventoryNotecard notecard)
        {
            InitializeComponent();
            Disposed += new EventHandler(Notecard_Disposed);

            this.instance = instance;
            this.notecard = notecard;

            txtName.Text = notecard.Name;
            txtDesc.Text = notecard.Description;
            lblStatus.Text = "Downloading";

            // Callbacks
            client.Assets.OnAssetReceived += new AssetManager.AssetReceivedCallback(Assets_OnAssetReceived);

            client.Assets.RequestInventoryAsset(notecard, true);
        }

        void Notecard_Disposed(object sender, EventArgs e)
        {
            client.Assets.OnAssetReceived += new AssetManager.AssetReceivedCallback(Assets_OnAssetReceived);
        }

        void Assets_OnAssetReceived(AssetDownload transfer, Asset asset)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Assets_OnAssetReceived(transfer, asset);
                    }
                ));
                return;
            }

            if (transfer.Success && asset.AssetType == AssetType.Notecard)
            {
                lblStatus.Text = "Done";
                AssetNotecard n = (AssetNotecard)asset;
                n.Decode();
                rtbContent.Text = n.BodyText;
            }
            else
            {
                lblStatus.Text = "Failed";
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Downloading";
            client.Assets.RequestInventoryAsset(notecard, true);
        }
    }
}
