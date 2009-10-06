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
// $Id$
//
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Radegast.Netcom;
using OpenMetaverse;

namespace Radegast
{
    public partial class frmProfile : Form
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;
        private string fullName;
        private UUID agentID;

        private UUID FLImageID;
        private UUID SLImageID;

        private bool gotPicks = false;
        private UUID requestedPick;
        private ProfilePick currentPick;
        private Dictionary<UUID, ProfilePick> pickCache = new Dictionary<UUID, ProfilePick>();
        private Dictionary<UUID, ParcelInfo> parcelCache = new Dictionary<UUID, ParcelInfo>();

        public frmProfile(RadegastInstance instance, string fullName, UUID agentID)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmProfile_Disposed);

            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;
            this.fullName = fullName;
            this.agentID = agentID;

            this.Text = fullName + " (profile) - " + Properties.Resources.ProgramName;
            txtUUID.Text = agentID.ToString();

            if (client.Friends.FriendList.ContainsKey(agentID))
            {
                btnFriend.Enabled = false;
            }

            // Callbacks
            client.Avatars.OnAvatarNames += new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client.Avatars.OnAvatarProperties += new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);
            client.Avatars.OnAvatarPicks += new AvatarManager.AvatarPicksCallback(Avatars_OnAvatarPicks);
            client.Avatars.OnPickInfo += new AvatarManager.PickInfoCallback(Avatars_OnPickInfo);
            client.Parcels.OnParcelInfo += new ParcelManager.ParcelInfoCallback(Parcels_OnParcelInfo);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);

            InitializeProfile();
        }

        void frmProfile_Disposed(object sender, EventArgs e)
        {
            client.Avatars.OnAvatarNames -= new AvatarManager.AvatarNamesCallback(Avatars_OnAvatarNames);
            client.Avatars.OnAvatarProperties -= new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);
            client.Avatars.OnAvatarPicks -= new AvatarManager.AvatarPicksCallback(Avatars_OnAvatarPicks);
            client.Avatars.OnPickInfo -= new AvatarManager.PickInfoCallback(Avatars_OnPickInfo);
            client.Parcels.OnParcelInfo -= new ParcelManager.ParcelInfoCallback(Parcels_OnParcelInfo);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
        }

        void Avatars_OnAvatarPicks(UUID id, Dictionary<UUID, string> picks)
        {
            if (id != agentID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Avatars_OnAvatarPicks(id, picks)));
                return;
            }
            gotPicks = true;
            DisplayListOfPicks(picks);

        }

        private void DisplayListOfPicks(Dictionary<UUID, string> picks)
        {
            pickListPanel.Controls.Clear();

            int i = 0;
            Button firstButton = null;

            foreach (KeyValuePair<UUID, string> PickInfo in picks)
            {
                Button b = new Button();
                b.AutoSize = false;
                b.Tag = PickInfo.Key;
                b.Name = PickInfo.Key.ToString();
                b.Text = PickInfo.Value;
                b.Width = 135;
                b.Height = 25;
                b.Left = 2;
                b.Top = i++ * b.Height + 5;
                b.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
                b.Click += new EventHandler(PickButtonClick);
                pickListPanel.Controls.Add(b);

                if (firstButton == null)
                    firstButton = b;
            }

            if (firstButton != null)
            {
                firstButton.PerformClick();
                pickDetailPanel.Visible = true;
            }

        }

        void PickButtonClick(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            requestedPick = (UUID)b.Tag;

            if (pickCache.ContainsKey(requestedPick))
            {
                Avatars_OnPickInfo(requestedPick, pickCache[requestedPick]);
            }
            else
            {
                client.Avatars.RequestPickInfo(agentID, requestedPick);
            }
        }

        void Avatars_OnPickInfo(UUID pickid, ProfilePick pick)
        {
            if (pickid != requestedPick) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Avatars_OnPickInfo(pickid, pick)));
                return;
            }

            lock (pickCache)
            {
                if (!pickCache.ContainsKey(pickid))
                    pickCache.Add(pickid, pick);
            }

            currentPick = pick;

            if (pickPicturePanel.Controls.Count > 0)
                pickPicturePanel.Controls[0].Dispose();
            pickPicturePanel.Controls.Clear();

            if (pick.SnapshotID != UUID.Zero)
            {
                SLImageHandler img = new SLImageHandler(instance, pick.SnapshotID, string.Empty);
                img.Dock = DockStyle.Fill;
                img.SizeMode = PictureBoxSizeMode.StretchImage;
                pickPicturePanel.Controls.Add(img);
            }

            pickTitle.Text = pick.Name;

            pickDetail.Text = pick.Desc;

            if (!parcelCache.ContainsKey(pick.ParcelID))
            {
                pickLocation.Text = string.Format("Unkown parcel, {0} ({1}, {2}, {3})",
                    pick.SimName,
                    ((int)pick.PosGlobal.X) % 256,
                    ((int)pick.PosGlobal.Y) % 256,
                    ((int)pick.PosGlobal.Z) % 256
                );
                client.Parcels.InfoRequest(pick.ParcelID);
            }
            else
            {
                Parcels_OnParcelInfo(parcelCache[pick.ParcelID]);
            }
        }

        void Parcels_OnParcelInfo(ParcelInfo parcel)
        {
            if (currentPick.ParcelID != parcel.ID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Parcels_OnParcelInfo(parcel)));
                return;
            }

            lock (parcelCache)
            {
                if (!parcelCache.ContainsKey(parcel.ID))
                    parcelCache.Add(parcel.ID, parcel);
            }

            pickLocation.Text = string.Format("{0}, {1} ({2}, {3}, {4})",
                parcel.Name,
                currentPick.SimName,
                ((int)currentPick.PosGlobal.X) % 256,
                ((int)currentPick.PosGlobal.Y) % 256,
                ((int)currentPick.PosGlobal.Z) % 256
            );
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            Close();
        }

        private void Avatars_OnAvatarNames(Dictionary<UUID, string> names)
        {
            foreach (KeyValuePair<UUID, string> kvp in names)
            {
                BeginInvoke(new OnSetPartnerText(SetPartnerText), new object[] { kvp.Value });
                break;
            }
        }

        private delegate void OnSetPartnerText(string partner);
        private void SetPartnerText(string partner)
        {
            txtPartner.Text = partner;
        }

        //comes in on separate thread
        private void Avatars_OnAvatarProperties(UUID avatarID, Avatar.AvatarProperties properties)
        {
            if (avatarID != agentID) return;

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(delegate()
                {
                    Avatars_OnAvatarProperties(avatarID, properties);
                }));
                return;
            }

            FLImageID = properties.FirstLifeImage;
            SLImageID = properties.ProfileImage;

            if (SLImageID != UUID.Zero)
            {
                SLImageHandler pic = new SLImageHandler(instance, SLImageID, "");
                pic.Dock = DockStyle.Fill;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                slPicPanel.Controls.Add(pic);
                slPicPanel.Show();
            }
            else
            {
                slPicPanel.Hide();
            }

            if (FLImageID != UUID.Zero)
            {
                SLImageHandler pic = new SLImageHandler(instance, FLImageID, "");
                pic.Dock = DockStyle.Fill;
                rlPicPanel.Controls.Add(pic);
                rlPicPanel.Show();
            }
            else
            {
                rlPicPanel.Hide();
            }

            this.BeginInvoke(
                new OnSetProfileProperties(SetProfileProperties),
                new object[] { properties });
        }

        //called on GUI thread
        private delegate void OnSetProfileProperties(Avatar.AvatarProperties properties);
        private void SetProfileProperties(Avatar.AvatarProperties properties)
        {
            txtBornOn.Text = properties.BornOn;
            if (properties.Partner != UUID.Zero) client.Avatars.RequestAvatarName(properties.Partner);

            if (fullName.EndsWith("Linden")) rtbAccountInfo.AppendText("Linden Lab Employee\n");
            if (properties.Identified) rtbAccountInfo.AppendText("Identified\n");
            if (properties.Transacted) rtbAccountInfo.AppendText("Transacted\n");

            rtbAbout.AppendText(properties.AboutText);

            txtWebURL.Text = properties.ProfileURL;
            btnWebView.Enabled = btnWebOpen.Enabled = (txtWebURL.TextLength > 0);

            rtbAboutFL.AppendText(properties.FirstLifeText);
        }

        private void InitializeProfile()
        {
            txtFullName.Text = fullName;
            btnOfferTeleport.Enabled = btnPay.Enabled = (agentID != client.Self.AgentID);

            client.Avatars.RequestAvatarProperties(agentID);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnWebView_Click(object sender, EventArgs e)
        {
            WebBrowser web = new WebBrowser();
            web.Dock = DockStyle.Fill;
            web.Url = new Uri(txtWebURL.Text);

            pnlWeb.Controls.Add(web);
        }

        private void ProcessWebURL(string url)
        {
            if (url.StartsWith("http://") || url.StartsWith("ftp://"))
                System.Diagnostics.Process.Start(url);
            else
                System.Diagnostics.Process.Start("http://" + url);
        }

        private void btnWebOpen_Click(object sender, EventArgs e)
        {
            ProcessWebURL(txtWebURL.Text);
        }

        private void rtbAbout_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            ProcessWebURL(e.LinkText);
        }

        private void rtbAboutFL_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            ProcessWebURL(e.LinkText);
        }

        private void btnOfferTeleport_Click(object sender, EventArgs e)
        {
            client.Self.SendTeleportLure(agentID, "Join me in " + client.Network.CurrentSim.Name + "!");
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            (new frmPay(instance, agentID, fullName, false)).ShowDialog();
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            TreeNode node = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (node == null)
            {
                e.Effect = DragDropEffects.None;
            }
            else
            {
                e.Effect = DragDropEffects.Copy | DragDropEffects.Move;
            }
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode node = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (node == null) return;

            if (node.Tag is InventoryItem)
            {
                InventoryItem item = node.Tag as InventoryItem;
                client.Inventory.GiveItem(item.UUID, item.Name, item.AssetType, agentID, true);
                instance.TabConsole.DisplayNotificationInChat("Offered item " + item.Name + " to " + fullName + ".");
            }
            else if (node.Tag is InventoryFolder)
            {
                InventoryFolder folder = node.Tag as InventoryFolder;
                client.Inventory.GiveFolder(folder.UUID, folder.Name, AssetType.Folder, agentID, true);
                instance.TabConsole.DisplayNotificationInChat("Offered folder " + folder.Name + " to " + fullName + ".");
            }
        }

        private void btnFriend_Click(object sender, EventArgs e)
        {
            client.Friends.OfferFriendship(agentID);
        }

        private void btnIM_Click(object sender, EventArgs e)
        {
            if (instance.TabConsole.TabExists((client.Self.AgentID ^ agentID).ToString()))
            {
                instance.TabConsole.SelectTab((client.Self.AgentID ^ agentID).ToString());
                return;
            }

            instance.TabConsole.AddIMTab(agentID, client.Self.AgentID ^ agentID, fullName);
            instance.TabConsole.SelectTab((client.Self.AgentID ^ agentID).ToString());
        }

        private void tabProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabProfile.SelectedTab.Name == "tbpPicks" && !gotPicks)
            {
                client.Avatars.RequestAvatarPicks(agentID);
            }
        }

        private void btnTeleport_Click(object sender, EventArgs e)
        {
            if (currentPick.PickID == UUID.Zero) return;
            btnShowOnMap_Click(this, EventArgs.Empty);
            instance.MainForm.WorldMap.DoTeleport();
        }

        private void btnShowOnMap_Click(object sender, EventArgs e)
        {
            if (currentPick.PickID == UUID.Zero) return;
            instance.MainForm.MapTab.Select();
            instance.MainForm.WorldMap.DisplayLocation(
                currentPick.SimName,
                ((int)currentPick.PosGlobal.X) % 256,
                ((int)currentPick.PosGlobal.Y) % 256,
                ((int)currentPick.PosGlobal.Z) % 256
            );
        }
    }
}