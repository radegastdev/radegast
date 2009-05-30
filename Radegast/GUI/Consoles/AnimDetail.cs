using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
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
        private AssetManager.AssetReceivedCallback assetCallback;
        private InventoryManager.ItemCreatedFromAssetCallback invCallback;
        private List<FriendInfo> friends;
        private FriendInfo friend;

        public AnimDetail(RadegastInstance instance, Avatar av, UUID anim, int n)
        {
            this.instance = instance;
            this.av = av;
            this.anim = anim;
            this.n = n;

            InitializeComponent();
        }

        private void AnimDetail_Load(object sender, EventArgs e)
        {
            if (File.Exists(instance.animCacheDir + "/" + anim + ".fail")) {
                Visible = false;
                return;
            }

            groupBox1.Text = "Animation " + n + " (" + anim + ") for " + av.Name;

            friends = instance.Client.Friends.FriendList.FindAll(delegate(FriendInfo f) { return true; });

            pnlSave.Visible = false;
            boxAnimName.Text = "Animation " + n;
            cbFriends.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (FriendInfo f in friends) {
                cbFriends.Items.Add(f);
            }

            if (File.Exists(instance.animCacheDir + "/" + anim + ".sla"))
            {
                pnlSave.Visible = true;
                return;
            }

            if (assetCallback == null) {
                assetCallback = new AssetManager.AssetReceivedCallback(Assets_OnAssetReceived);
                instance.Client.Assets.OnAssetReceived += assetCallback;
            }

            instance.Client.Assets.RequestAsset(anim, AssetType.Animation, true);
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

            if (res == DialogResult.OK) {
                try {
                    byte[] data = File.ReadAllBytes(instance.animCacheDir + "/" + anim + ".sla");
                    File.WriteAllBytes(dlg.FileName, data);
                } catch (Exception ex) {
                    Logger.Log("Saving animation failed: " + ex.Message, Helpers.LogLevel.Debug);
                }
            }
        }

        void Assets_OnAssetReceived(AssetDownload transfer, Asset asset)
        {
            if (transfer.AssetID != anim) return;

            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate()
                {
                    Assets_OnAssetReceived(transfer, asset);
                }));
                return;
            }

            if (transfer.Success) {
                Logger.Log("Animation " + anim + " download success " + asset.AssetData.Length + " bytes.", Helpers.LogLevel.Debug);
                File.WriteAllBytes(instance.animCacheDir + "/" + anim + ".sla", asset.AssetData);
                pnlSave.Visible = true;
            } else {
                Logger.Log("Animation " + anim + " download failed.", Helpers.LogLevel.Debug);
                FileStream f = File.Create(instance.animCacheDir + "/" + anim + ".fail");
                f.Close();
                Visible = false;
            }
        }

        private void playBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (playBox.CheckState == CheckState.Checked) {
                instance.Client.Self.AnimationStart(anim, true);
            } else {
                instance.Client.Self.AnimationStop(anim, true);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (invCallback == null) {
                invCallback = new InventoryManager.ItemCreatedFromAssetCallback(On_ItemCreated);
            }
            byte[] data = File.ReadAllBytes(instance.animCacheDir + "/" + anim + ".sla");
            instance.Client.Inventory.RequestCreateItemFromAsset(data, boxAnimName.Text, "(No description)", AssetType.Animation, InventoryType.Animation, instance.Client.Inventory.FindFolderForType(AssetType.Animation), invCallback);
            lblStatus.Text = "Uploading...";
            cbFriends.Enabled = false;
        }

        void On_ItemCreated(bool success, string status, UUID itemID, UUID assetID)
        {
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate()
                {
                    On_ItemCreated(success, status, itemID, assetID);
                }
                ));
                return;
            }

            if (!success) {
                lblStatus.Text = "Failed.";
                Logger.Log("Failed creating asset", Helpers.LogLevel.Debug);
                return;
            } else {
                Logger.Log("Created inventory item " + itemID.ToString(), Helpers.LogLevel.Info);

                lblStatus.Text = "Sending to " + friend.Name;
                Refresh();

                // Fix the permissions on the new upload since they are fscked by default
                InventoryItem item = instance.Client.Inventory.FetchItem(itemID, instance.Client.Self.AgentID, 1000);

                if (item != null) {
                    // item.Permissions.EveryoneMask = PermissionMask.All;
                    item.Permissions.NextOwnerMask = PermissionMask.All;
                    instance.Client.Inventory.RequestUpdateItem(item);


                    // FIXME: We should be watching the callback for RequestUpdateItem instead of a dumb sleep
                    System.Threading.Thread.Sleep(2000);

                    

                    Logger.Log("Sending item to " + friend.Name, Helpers.LogLevel.Info);
                    instance.Client.Inventory.GiveItem(itemID, item.Name, AssetType.Animation, friend.UUID, true);
                    lblStatus.Text = "Sent";
                }
            }

            cbFriends.Enabled = true;
        }


        private void cbFriends_SelectedValueChanged(object sender, EventArgs e)
        {
            if (cbFriends.SelectedIndex >= 0) {
                btnSend.Enabled = true;
                friend = friends[cbFriends.SelectedIndex];
            } else {
                btnSend.Enabled = false;
            }
        }
    }
}
