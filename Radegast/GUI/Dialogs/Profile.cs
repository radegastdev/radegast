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
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Radegast.Netcom;
using OpenMetaverse;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif


namespace Radegast
{
    public partial class frmProfile : RadegastForm
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;
        private string fullName;

        public UUID AgentID { get; }
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
            AgentID = agentID;

            Text = fullName + " (profile) - " + Properties.Resources.ProgramName;
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

            GUI.GuiHelpers.ApplyGuiFixes(this);
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
            bool isMuted = client.Self.MuteList.Find(me => (me.Type == MuteType.Resident && me.ID == AgentID)) != null;

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
            if (e.AvatarID != AgentID) return;

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
                    ListViewItem item = new ListViewItem {Name = g.GroupID.ToString(), Text = g.GroupName, Tag = g};
                    item.SubItems.Add(new ListViewItem.ListViewSubItem(item, g.GroupTitle));

                    lvwGroups.Items.Add(item);
                }
            }

            lvwGroups.EndUpdate();

        }

        void Avatars_AvatarPicksReply(object sender, AvatarPicksReplyEventArgs e)
        {
            if (e.AvatarID != AgentID) return;

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
                Button b = new Button
                {
                    AutoSize = false,
                    Tag = PickInfo.Key,
                    Name = PickInfo.Key.ToString(),
                    Text = PickInfo.Value,
                    Width = 135,
                    Height = 25,
                    Left = 2
                };
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

            firstButton?.PerformClick();
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
                client.Avatars.RequestPickInfo(AgentID, requestedPick);
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

            if (AgentID == client.Self.AgentID || e.Pick.SnapshotID != UUID.Zero)
            {
                SLImageHandler img = new SLImageHandler(instance, e.Pick.SnapshotID, string.Empty)
                {
                    Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.StretchImage
                };
                pickPicturePanel.Controls.Add(img);

                if (AgentID == client.Self.AgentID)
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
            if (e.AvatarID != AgentID) return;

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => Avatars_AvatarPropertiesReply(sender, e)));
                return;
            }
            Profile = e.Properties;

            FLImageID = e.Properties.FirstLifeImage;
            SLImageID = e.Properties.ProfileImage;

            if (AgentID == client.Self.AgentID || SLImageID != UUID.Zero)
            {
                SLImageHandler pic = new SLImageHandler(instance, SLImageID, "");
                
                if (AgentID == client.Self.AgentID)
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

            if (AgentID == client.Self.AgentID || FLImageID != UUID.Zero)
            {
                SLImageHandler pic = new SLImageHandler(instance, FLImageID, "") {Dock = DockStyle.Fill};

                if (AgentID == client.Self.AgentID)
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

            BeginInvoke(
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
            txtFullName.AgentID = AgentID;
            btnOfferTeleport.Enabled = btnPay.Enabled = (AgentID != client.Self.AgentID);

            client.Avatars.RequestAvatarProperties(AgentID);
            UpdateMuteButton();

            if (AgentID == client.Self.AgentID)
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
            WebBrowser web = new WebBrowser {Dock = DockStyle.Fill, Url = new Uri(txtWebURL.Text)};

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
            instance.MainForm.AddNotification(new ntfSendLureOffer(instance, AgentID));
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            (new frmPay(instance, AgentID, fullName, false)).ShowDialog();
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

            if (node.Tag is InventoryItem item)
            {
                client.Inventory.GiveItem(item.UUID, item.Name, item.AssetType, AgentID, true);
                instance.TabConsole.DisplayNotificationInChat("Offered item " + item.Name + " to " + fullName + ".");
            }
            else if (node.Tag is InventoryFolder folder)
            {
                client.Inventory.GiveFolder(folder.UUID, folder.Name, AgentID, true);
                instance.TabConsole.DisplayNotificationInChat("Offered folder " + folder.Name + " to " + fullName + ".");
            }
        }

        private void btnFriend_Click(object sender, EventArgs e)
        {
            client.Friends.OfferFriendship(AgentID);
        }

        private void btnIM_Click(object sender, EventArgs e)
        {
            instance.TabConsole.ShowIMTab(AgentID, fullName, true);
        }

        private void tabProfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabProfile.SelectedTab.Name == "tbpPicks" && !gotPicks)
            {
                client.Avatars.RequestAvatarPicks(AgentID);
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
                client.Inventory.GiveItem(item.UUID, item.Name, item.AssetType, AgentID, true);
                instance.TabConsole.DisplayNotificationInChat("Offered item " + item.Name + " to " + fullName + ".");
            }
            else if (inv is InventoryFolder)
            {
                InventoryFolder folder = inv as InventoryFolder;
                client.Inventory.GiveFolder(folder.UUID, folder.Name, AgentID, true);
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
            client.Avatars.RequestAvatarPicks(AgentID);
        }

        private void btnNewPick_Click(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(sync =>
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
                    client.Avatars.RequestAvatarPicks(AgentID);
                });
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            client.Self.UpdateMuteListEntry(MuteType.Resident, AgentID, instance.Names.GetLegacyName(AgentID));
        }

        private void btnUnmute_Click(object sender, EventArgs e)
        {
            MuteEntry me = client.Self.MuteList.Find(mle => mle.Type == MuteType.Resident && mle.ID == AgentID);

            if (me != null)
            {
                client.Self.RemoveMuteListEntry(me.ID, me.Name);
            }
            else
            {
                client.Self.RemoveMuteListEntry(AgentID, instance.Names.GetLegacyName(AgentID));
            }
        }

        private void btnRequestTeleport_Click(object sender, EventArgs e)
        {
            instance.MainForm.AddNotification(new ntfSendLureRequest(instance, AgentID));
        }
    }
}