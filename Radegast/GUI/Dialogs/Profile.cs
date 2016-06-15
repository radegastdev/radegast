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
using Radegast.Netcom;
using OpenMetaverse;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;


namespace Radegast
{
    public partial class frmProfile : RadegastForm
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;
        private string fullName;

        private UUID agentID;
        public UUID AgentID { get { return agentID; } }
        private Avatar.AvatarProperties Profile;
        bool myProfile = false;
        UUID newPickID = UUID.Zero;

        private UUID FLImageID;
        private UUID SLImageID;

        private bool gotPicks = false;
        private UUID requestedPick;
        private ProfilePick currentPick;
        private Dictionary<UUID, ProfilePick> pickCache = new Dictionary<UUID, ProfilePick>();
        private Dictionary<UUID, ParcelInfo> parcelCache = new Dictionary<UUID, ParcelInfo>();

        public frmProfile(RadegastInstance instance, string fullName, UUID agentID)
            : base(instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmProfile_Disposed);

            AutoSavePosition = true;

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

            if (instance.InventoryClipboard != null)
            {
                btnGive.Enabled = true;
            }

            if (agentID == client.Self.AgentID)
            {
                myProfile = true;
                rtbAbout.ReadOnly = false;
                rtbAboutFL.ReadOnly = false;
                txtWebURL.ReadOnly = false;
                pickTitle.ReadOnly = false;
                pickDetail.ReadOnly = false;
                btnRequestTeleport.Visible = false;
                btnDeletePick.Visible = true;
                btnNewPick.Visible = true;
            }

            // Callbacks
            client.Avatars.AvatarPropertiesReply += new EventHandler<AvatarPropertiesReplyEventArgs>(Avatars_AvatarPropertiesReply);
            client.Avatars.AvatarPicksReply += new EventHandler<AvatarPicksReplyEventArgs>(Avatars_AvatarPicksReply);
            client.Avatars.PickInfoReply += new EventHandler<PickInfoReplyEventArgs>(Avatars_PickInfoReply);
            client.Parcels.ParcelInfoReply += new EventHandler<ParcelInfoReplyEventArgs>(Parcels_ParcelInfoReply);
            client.Avatars.AvatarGroupsReply += new EventHandler<AvatarGroupsReplyEventArgs>(Avatars_AvatarGroupsReply);
            client.Self.MuteListUpdated += new EventHandler<EventArgs>(Self_MuteListUpdated);
            netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(netcom_ClientDisconnected);
            instance.InventoryClipboardUpdated += new EventHandler<EventArgs>(instance_InventoryClipboardUpdated);
            InitializeProfile();

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void frmProfile_Disposed(object sender, EventArgs e)
        {
            client.Avatars.AvatarPropertiesReply -= new EventHandler<AvatarPropertiesReplyEventArgs>(Avatars_AvatarPropertiesReply);
            client.Avatars.AvatarPicksReply -= new EventHandler<AvatarPicksReplyEventArgs>(Avatars_AvatarPicksReply);
            client.Avatars.PickInfoReply -= new EventHandler<PickInfoReplyEventArgs>(Avatars_PickInfoReply);
            client.Parcels.ParcelInfoReply -= new EventHandler<ParcelInfoReplyEventArgs>(Parcels_ParcelInfoReply);
            client.Avatars.AvatarGroupsReply -= new EventHandler<AvatarGroupsReplyEventArgs>(Avatars_AvatarGroupsReply);
            client.Self.MuteListUpdated -= new EventHandler<EventArgs>(Self_MuteListUpdated);
            netcom.ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(netcom_ClientDisconnected);
            instance.InventoryClipboardUpdated -= new EventHandler<EventArgs>(instance_InventoryClipboardUpdated);
        }

        void Self_MuteListUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Self_MuteListUpdated(sender, e)));
                }
                return;
            }

            UpdateMuteButton();
        }

        void UpdateMuteButton()
        {
            bool isMuted = client.Self.MuteList.Find(me => (me.Type == MuteType.Resident && me.ID == agentID)) != null;

            if (isMuted)
            {
                btnMute.Enabled = false;
                btnUnmute.Enabled = true;
            }
            else
            {
                btnMute.Enabled = true;
                btnUnmute.Enabled = false;
            }
        }

        void instance_InventoryClipboardUpdated(object sender, EventArgs e)
        {
            btnGive.Enabled = instance.InventoryClipboard != null;
        }

        void Avatars_AvatarGroupsReply(object sender, AvatarGroupsReplyEventArgs e)
        {
            if (e.AvatarID != agentID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Avatars_AvatarGroupsReply(sender, e)));
                return;
            }

            lvwGroups.BeginUpdate();

            foreach (AvatarGroup g in e.Groups)
            {
                if (!lvwGroups.Items.ContainsKey(g.GroupID.ToString()))
                {
                    ListViewItem item = new ListViewItem();
                    item.Name = g.GroupID.ToString();
                    item.Text = g.GroupName;
                    item.Tag = g;
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, g.GroupTitle));

                    lvwGroups.Items.Add(item);
                }
            }

            lvwGroups.EndUpdate();

        }

        void Avatars_AvatarPicksReply(object sender, AvatarPicksReplyEventArgs e)
        {
            if (e.AvatarID != agentID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Avatars_AvatarPicksReply(sender, e)));
                return;
            }
            gotPicks = true;
            DisplayListOfPicks(e.Picks);
        }

        private void ClearPicks()
        {
            List<Control> controls = new List<Control>();
            foreach (Control c in pickListPanel.Controls)
                if (c != btnNewPick)
                    controls.Add(c);
            foreach (Control c in controls)
                c.Dispose();
            pickDetailPanel.Visible = false;
        }

        private void DisplayListOfPicks(Dictionary<UUID, string> picks)
        {
            ClearPicks();

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

                if (newPickID == PickInfo.Key)
                    firstButton = b;
            }

            newPickID = UUID.Zero;

            if (firstButton != null)
            {
                firstButton.PerformClick();
            }
        }

        void PickButtonClick(object sender, EventArgs e)
        {
            pickDetailPanel.Visible = true;
            Button b = (Button)sender;
            requestedPick = (UUID)b.Tag;

            if (pickCache.ContainsKey(requestedPick))
            {
                Avatars_PickInfoReply(this, new PickInfoReplyEventArgs(requestedPick, pickCache[requestedPick]));
            }
            else
            {
                client.Avatars.RequestPickInfo(agentID, requestedPick);
            }
        }

        void Avatars_PickInfoReply(object sender, PickInfoReplyEventArgs e)
        {
            if (e.PickID != requestedPick) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Avatars_PickInfoReply(sender, e)));
                return;
            }

            lock (pickCache)
            {
                if (!pickCache.ContainsKey(e.PickID))
                    pickCache.Add(e.PickID, e.Pick);
            }

            currentPick = e.Pick;

            if (pickPicturePanel.Controls.Count > 0)
                pickPicturePanel.Controls[0].Dispose();
            pickPicturePanel.Controls.Clear();

            if (agentID == client.Self.AgentID || e.Pick.SnapshotID != UUID.Zero)
            {
                SLImageHandler img = new SLImageHandler(instance, e.Pick.SnapshotID, string.Empty);
                img.Dock = DockStyle.Fill;
                img.SizeMode = PictureBoxSizeMode.StretchImage;
                pickPicturePanel.Controls.Add(img);

                if (agentID == client.Self.AgentID)
                {
                    img.AllowUpdateImage = true;
                    ProfilePick p = e.Pick;
                    img.ImageUpdated += (psender, pe) =>
                    {
                        img.UpdateImage(pe.NewImageID);
                        p.SnapshotID = pe.NewImageID;
                        client.Self.PickInfoUpdate(p.PickID, p.TopPick, p.ParcelID, p.Name, p.PosGlobal, p.SnapshotID, p.Desc);
                    };
                }
            }

            pickTitle.Text = e.Pick.Name;

            pickDetail.Text = e.Pick.Desc;

            if (!parcelCache.ContainsKey(e.Pick.ParcelID))
            {
                pickLocation.Text = string.Format("Unkown parcel, {0} ({1}, {2}, {3})",
                    e.Pick.SimName,
                    ((int)e.Pick.PosGlobal.X) % 256,
                    ((int)e.Pick.PosGlobal.Y) % 256,
                    ((int)e.Pick.PosGlobal.Z) % 256
                );
                client.Parcels.RequestParcelInfo(e.Pick.ParcelID);
            }
            else
            {
                Parcels_ParcelInfoReply(this, new ParcelInfoReplyEventArgs(parcelCache[e.Pick.ParcelID]));
            }
        }

        void Parcels_ParcelInfoReply(object sender, ParcelInfoReplyEventArgs e)
        {
            if (currentPick.ParcelID != e.Parcel.ID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Parcels_ParcelInfoReply(sender, e)));
                return;
            }

            lock (parcelCache)
            {
                if (!parcelCache.ContainsKey(e.Parcel.ID))
                    parcelCache.Add(e.Parcel.ID, e.Parcel);
            }

            pickLocation.Text = string.Format("{0}, {1} ({2}, {3}, {4})",
                e.Parcel.Name,
                currentPick.SimName,
                ((int)currentPick.PosGlobal.X) % 256,
                ((int)currentPick.PosGlobal.Y) % 256,
                ((int)currentPick.PosGlobal.Z) % 256
            );
        }

        void netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(Close));
            }
            else
            {
                Close();
            }
        }

        void Avatars_AvatarPropertiesReply(object sender, AvatarPropertiesReplyEventArgs e)
        {
            if (e.AvatarID != agentID) return;

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => Avatars_AvatarPropertiesReply(sender, e)));
                return;
            }
            Profile = e.Properties;

            FLImageID = e.Properties.FirstLifeImage;
            SLImageID = e.Properties.ProfileImage;

            if (agentID == client.Self.AgentID || SLImageID != UUID.Zero)
            {
                SLImageHandler pic = new SLImageHandler(instance, SLImageID, "");
                
                if (agentID == client.Self.AgentID)
                {
                    pic.AllowUpdateImage = true;
                    pic.ImageUpdated += (usender, ue) =>
                    {
                        Profile.ProfileImage = ue.NewImageID;
                        pic.UpdateImage(ue.NewImageID);
                        client.Self.UpdateProfile(Profile);
                    };
                }

                pic.Dock = DockStyle.Fill;
                pic.SizeMode = PictureBoxSizeMode.StretchImage;
                slPicPanel.Controls.Add(pic);
                slPicPanel.Show();
            }
            else
            {
                slPicPanel.Hide();
            }

            if (agentID == client.Self.AgentID || FLImageID != UUID.Zero)
            {
                SLImageHandler pic = new SLImageHandler(instance, FLImageID, "");
                pic.Dock = DockStyle.Fill;

                if (agentID == client.Self.AgentID)
                {
                    pic.AllowUpdateImage = true;
                    pic.ImageUpdated += (usender, ue) =>
                    {
                        Profile.FirstLifeImage = ue.NewImageID;
                        pic.UpdateImage(ue.NewImageID);
                        client.Self.UpdateProfile(Profile);
                    };
                }

                rlPicPanel.Controls.Add(pic);
                rlPicPanel.Show();
            }
            else
            {
                rlPicPanel.Hide();
            }

            this.BeginInvoke(
                new OnSetProfileProperties(SetProfileProperties),
                new object[] { e.Properties });
        }

        //called on GUI thread
        private delegate void OnSetProfileProperties(Avatar.AvatarProperties properties);
        private void SetProfileProperties(Avatar.AvatarProperties properties)
        {
            txtBornOn.Text = properties.BornOn;
            anPartner.AgentID = properties.Partner;

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
            txtFullName.AgentID = agentID;
            btnOfferTeleport.Enabled = btnPay.Enabled = (agentID != client.Self.AgentID);

            client.Avatars.RequestAvatarProperties(agentID);
            UpdateMuteButton();

            if (agentID == client.Self.AgentID)
            {
                btnGive.Visible =
                    btnIM.Visible =
                    btnMute.Visible =
                    btnUnmute.Visible =
                    btnOfferTeleport.Visible =
                    btnPay.Visible =
                    btnFriend.Visible =
                    false;
            }
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

        private void btnWebOpen_Click(object sender, EventArgs e)
        {
            instance.MainForm.ProcessLink(txtWebURL.Text);
        }

        private void rtbAbout_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            instance.MainForm.ProcessLink(e.LinkText);
        }

        private void rtbAboutFL_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            instance.MainForm.ProcessLink(e.LinkText);
        }

        private void btnOfferTeleport_Click(object sender, EventArgs e)
        {
            instance.MainForm.AddNotification(new ntfSendLureOffer(instance, agentID));
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
            instance.TabConsole.ShowIMTab(agentID, fullName, true);
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

        private void lvwGroups_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = lvwGroups.GetItemAt(e.X, e.Y);

            if (item != null)
            {
                try
                {
                    instance.MainForm.ShowGroupProfile((AvatarGroup)item.Tag);
                }
                catch (Exception) { }
            }
        }

        private void btnGive_Click(object sender, EventArgs e)
        {
            if (instance.InventoryClipboard == null) return;

            InventoryBase inv = instance.InventoryClipboard.Item;

            if (inv is InventoryItem)
            {
                InventoryItem item = inv as InventoryItem;
                client.Inventory.GiveItem(item.UUID, item.Name, item.AssetType, agentID, true);
                instance.TabConsole.DisplayNotificationInChat("Offered item " + item.Name + " to " + fullName + ".");
            }
            else if (inv is InventoryFolder)
            {
                InventoryFolder folder = inv as InventoryFolder;
                client.Inventory.GiveFolder(folder.UUID, folder.Name, AssetType.Folder, agentID, true);
                instance.TabConsole.DisplayNotificationInChat("Offered folder " + folder.Name + " to " + fullName + ".");
            }

        }

        private void rtbAbout_Leave(object sender, EventArgs e)
        {
            if (!myProfile) return;
            Profile.AboutText = rtbAbout.Text;
            client.Self.UpdateProfile(Profile);
        }

        private void rtbAboutFL_Leave(object sender, EventArgs e)
        {
            if (!myProfile) return;
            Profile.FirstLifeText = rtbAboutFL.Text;
            client.Self.UpdateProfile(Profile);
        }

        private void txtWebURL_Leave(object sender, EventArgs e)
        {
            if (!myProfile) return;
            btnWebView.Enabled = btnWebOpen.Enabled = (txtWebURL.TextLength > 0);
            Profile.ProfileURL = txtWebURL.Text;
            client.Self.UpdateProfile(Profile);
        }

        private void pickTitle_Leave(object sender, EventArgs e)
        {
            if (!myProfile) return;
            currentPick.Name = pickTitle.Text;
            currentPick.Desc = pickDetail.Text;

            client.Self.PickInfoUpdate(currentPick.PickID,
                currentPick.TopPick,
                currentPick.ParcelID,
                currentPick.Name,
                currentPick.PosGlobal,
                currentPick.SnapshotID,
                currentPick.Desc);

            pickCache[currentPick.PickID] = currentPick;
        }

        private void btnDeletePick_Click(object sender, EventArgs e)
        {
            if (!myProfile) return;
            client.Self.PickDelete(currentPick.PickID);
            ClearPicks();
            client.Avatars.RequestAvatarPicks(agentID);
        }

        private void btnNewPick_Click(object sender, EventArgs e)
        {
            WorkPool.QueueUserWorkItem(sync =>
                {
                    UUID parcelID = client.Parcels.RequestRemoteParcelID(client.Self.SimPosition, client.Network.CurrentSim.Handle, client.Network.CurrentSim.ID);
                    newPickID = UUID.Random();

                    client.Self.PickInfoUpdate(
                        newPickID,
                        false,
                        parcelID,
                        Instance.State.Parcel.Name,
                        client.Self.GlobalPosition,
                        Instance.State.Parcel.SnapshotID,
                        Instance.State.Parcel.Desc
                        );

                    Invoke(new MethodInvoker(() => ClearPicks()));
                    client.Avatars.RequestAvatarPicks(agentID);
                });
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            client.Self.UpdateMuteListEntry(MuteType.Resident, agentID, instance.Names.GetLegacyName(agentID));
        }

        private void btnUnmute_Click(object sender, EventArgs e)
        {
            MuteEntry me = client.Self.MuteList.Find(mle => mle.Type == MuteType.Resident && mle.ID == agentID);

            if (me != null)
            {
                client.Self.RemoveMuteListEntry(me.ID, me.Name);
            }
            else
            {
                client.Self.RemoveMuteListEntry(agentID, instance.Names.GetLegacyName(agentID));
            }
        }

        private void btnRequestTeleport_Click(object sender, EventArgs e)
        {
            instance.MainForm.AddNotification(new ntfSendLureRequest(instance, agentID));
        }
    }
}