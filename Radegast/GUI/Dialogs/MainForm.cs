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
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Threading;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using System.Web;
using Radegast.Netcom;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class frmMain : RadegastForm
    {
        #region Public members
        public static ImageList ResourceImages = new ImageList();
        public static List<string> ImageNames = new List<string>();

        public TabsConsole TabConsole
        {
            get { return tabsConsole; }
        }

        public MapConsole WorldMap
        {
            get
            {
                if (MapTab != null)
                {
                    return (MapConsole)MapTab.Control;
                }
                return null;
            }
        }

        public RadegastTab MapTab
        {
            get
            {
                if (tabsConsole.TabExists("map"))
                {
                    return tabsConsole.Tabs["map"];
                }
                else
                {
                    return null;
                }
            }
        }

        public MediaConsole MediaConsole { get { return mediaConsole; } }

        /// <summary>
        /// Drop down that contains the tools menu
        /// </summary>
        public ToolStripDropDownButton ToolsMenu
        {
            get { return tbnTools; }
        }

        /// <summary>
        /// Dropdown that contains the heelp menu
        /// </summary>
        public ToolStripDropDownButton HelpMenu
        {
            get { return tbtnHelp; }
        }

        /// <summary>
        /// Drop down that contants the plugins menu. Make sure to set it Visible if
        /// you add items to this menu, it's hidden by default
        /// </summary>
        public ToolStripDropDownButton PluginsMenu
        {
            get { return tbnPlugins; }
        }

        #endregion

        #region Private members
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private TabsConsole tabsConsole;
        private System.Timers.Timer statusTimer;
        private AutoPilot ap;
        private bool AutoPilotActive = false;
        private TransparentButton btnDialogNextControl;
        private MediaConsole mediaConsole;
        #endregion

        #region Constructor and disposal
        public frmMain(RadegastInstance instance)
            : base(instance)
        {
            GetSLTimeZone();
            InitializeComponent();
            Disposed += new EventHandler(frmMain_Disposed);

            this.instance = instance;
            this.instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
            netcom.NetcomSync = this;

            pnlDialog.Visible = false;
            btnDialogNextControl = new TransparentButton();
            pnlDialog.Controls.Add(btnDialogNextControl);
            pnlDialog.Top = 0;

            btnDialogNextControl.Size = new Size(35, 20);
            btnDialogNextControl.BackColor = Color.Transparent;
            btnDialogNextControl.ForeColor = Color.Gold;
            btnDialogNextControl.FlatAppearance.BorderSize = 0;
            btnDialogNextControl.FlatStyle = FlatStyle.Flat;
            btnDialogNextControl.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnDialogNextControl.Text = ">>";
            btnDialogNextControl.Font = new Font(btnDialogNextControl.Font, FontStyle.Bold);
            btnDialogNextControl.Margin = new Padding(0);
            btnDialogNextControl.Padding = new Padding(0);
            btnDialogNextControl.UseVisualStyleBackColor = false;
            btnDialogNextControl.Top = btnDialogNextControl.Parent.ClientSize.Height - btnDialogNextControl.Size.Height;
            btnDialogNextControl.Left = btnDialogNextControl.Parent.ClientSize.Width - btnDialogNextControl.Size.Width;
            btnDialogNextControl.Click += new EventHandler(btnDialogNextControl_Click);

            if (instance.MonoRuntime)
            {
                statusStrip1.LayoutStyle = ToolStripLayoutStyle.Table;
            }

            // Config options
            if (instance.GlobalSettings["transaction_notification_chat"].Type == OSDType.Unknown)
                instance.GlobalSettings["transaction_notification_chat"] = OSD.FromBoolean(true);

            if (instance.GlobalSettings["transaction_notification_dialog"].Type == OSDType.Unknown)
                instance.GlobalSettings["transaction_notification_dialog"] = OSD.FromBoolean(true);

            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(netcom_ClientDisconnected);
            RegisterClientEvents(client);

            InitializeStatusTimer();
            RefreshWindowTitle();
        }

        private void RegisterClientEvents(GridClient client)
        {
            client.Parcels.ParcelProperties += new EventHandler<ParcelPropertiesEventArgs>(Parcels_ParcelProperties);
            client.Self.MoneyBalanceReply += new EventHandler<MoneyBalanceReplyEventArgs>(Self_MoneyBalanceReply);
        }

        private void UnregisterClientEvents(GridClient client)
        {
            client.Parcels.ParcelProperties -= new EventHandler<ParcelPropertiesEventArgs>(Parcels_ParcelProperties);
            client.Self.MoneyBalanceReply -= new EventHandler<MoneyBalanceReplyEventArgs>(Self_MoneyBalanceReply);
        }

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(client);
        }

        void frmMain_Disposed(object sender, EventArgs e)
        {
            netcom.NetcomSync = null;
            netcom.ClientLoginStatus -= new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(netcom_ClientDisconnected);
            UnregisterClientEvents(client);
            this.instance.CleanUp();
        }
        #endregion

        #region Event handlers
        void Self_MoneyBalanceReply(object sender, MoneyBalanceReplyEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Description))
            {
                if (instance.GlobalSettings["transaction_notification_dialog"].AsBoolean())
                    AddNotification(new ntfGeneric(instance, e.Description));
                if (instance.GlobalSettings["transaction_notification_chat"].AsBoolean())
                    TabConsole.DisplayNotificationInChat(e.Description);
            }
        }

        public void InitializeControls()
        {
            InitializeTabsConsole();

            if (instance.MediaManager.SoundSystemAvailable)
            {
                mediaConsole = new MediaConsole(instance);
                tbtnMedia.Visible = true;
            }
        }

        public bool InAutoReconnect { get; set; }

        private void DisplayAutoReconnectForm()
        {
            if (IsDisposed) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(DisplayAutoReconnectForm));
                return;
            }

            InAutoReconnect = true;
            frmReconnect dialog = new frmReconnect(instance, 120);
            dialog.ShowDialog(this);
            dialog.Dispose();
            dialog = null;
        }

        public void BeginAutoReconnect()
        {
            // Sleep for 3 seconds on a separate thread while things unwind on
            // disconnect, since ShowDialog() blocks GUI thread
            (new Thread(new ThreadStart(() =>
                {
                    System.Threading.Thread.Sleep(3000);
                    DisplayAutoReconnectForm();
                }
                ))
                {
                    Name = "Reconnect Delay Thread",
                    IsBackground = true
                }
            ).Start();
        }

        private void netcom_ClientLoginStatus(object sender, LoginProgressEventArgs e)
        {
            if (e.Status == LoginStatus.Failed)
            {
                if (InAutoReconnect)
                {
                    if (instance.GlobalSettings["auto_reconnect"].AsBoolean())
                        BeginAutoReconnect();
                    else
                        InAutoReconnect = false;
                }
            }
            else if (e.Status == LoginStatus.Success)
            {
                InAutoReconnect = false;
                tbtnVoice.Enabled = disconnectToolStripMenuItem.Enabled =
                tbtnGroups.Enabled = tbnObjects.Enabled = tbtnWorld.Enabled = tbnTools.Enabled = tmnuImport.Enabled =
                    tbtnFriends.Enabled = tbtnInventory.Enabled = tbtnSearch.Enabled = tbtnMap.Enabled = true;

                statusTimer.Start();
                RefreshWindowTitle();
            }
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            tbtnVoice.Enabled = disconnectToolStripMenuItem.Enabled =
            tbtnGroups.Enabled = tbnObjects.Enabled = tbtnWorld.Enabled = tbnTools.Enabled = tmnuImport.Enabled =
                tbtnFriends.Enabled = tbtnInventory.Enabled = tbtnSearch.Enabled = tbtnMap.Enabled = false;

            reconnectToolStripMenuItem.Enabled = true;
            InAutoReconnect = false;

            if (statusTimer != null)
                statusTimer.Stop();

            RefreshStatusBar();
            RefreshWindowTitle();
        }

        private void netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (e.Reason == NetworkManager.DisconnectType.ClientInitiated) return;
            netcom_ClientLoggedOut(sender, EventArgs.Empty);

            if (instance.GlobalSettings["auto_reconnect"].AsBoolean())
            {
                BeginAutoReconnect();
            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (statusTimer != null)
            {
                statusTimer.Stop();
                statusTimer.Dispose();
                statusTimer = null;
            }

            if (mediaConsole != null)
            {
                if (tabsConsole.TabExists("media"))
                {
                    tabsConsole.Tabs["media"].AllowClose = true;
                    tabsConsole.Tabs["media"].Close();
                }
                else
                {
                    mediaConsole.Dispose();
                }
                mediaConsole = null;
            }

            if (netcom.IsLoggedIn)
            {
                Thread saveInvToDisk = new Thread(new ThreadStart(
                    delegate()
                    {
                        client.Inventory.Store.SaveToDisk(instance.InventoryCacheFileName);
                    }));
                saveInvToDisk.Name = "Save inventory to disk";
                saveInvToDisk.Start();

                netcom.Logout();
            }
        }
        #endregion

        # region Update status

        void Parcels_ParcelProperties(object sender, ParcelPropertiesEventArgs e)
        {
            if (e.Result != ParcelResult.Single) return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Parcels_ParcelProperties(sender, e)));
                return;
            }

            Parcel parcel = e.Parcel;

            tlblParcel.Text = parcel.Name;
            tlblParcel.ToolTipText = parcel.Desc;

            if ((parcel.Flags & ParcelFlags.AllowFly) != ParcelFlags.AllowFly)
                icoNoFly.Visible = true;
            else
                icoNoFly.Visible = false;

            if ((parcel.Flags & ParcelFlags.CreateObjects) != ParcelFlags.CreateObjects)
                icoNoBuild.Visible = true;
            else
                icoNoBuild.Visible = false;

            if ((parcel.Flags & ParcelFlags.AllowOtherScripts) != ParcelFlags.AllowOtherScripts)
                icoNoScript.Visible = true;
            else
                icoNoScript.Visible = false;

            if ((parcel.Flags & ParcelFlags.RestrictPushObject) == ParcelFlags.RestrictPushObject)
                icoNoPush.Visible = true;
            else
                icoNoPush.Visible = false;

            if ((parcel.Flags & ParcelFlags.AllowDamage) == ParcelFlags.AllowDamage)
                icoHealth.Visible = true;
            else
                icoHealth.Visible = false;

            if ((parcel.Flags & ParcelFlags.AllowVoiceChat) != ParcelFlags.AllowVoiceChat)
                icoNoVoice.Visible = true;
            else
                icoNoVoice.Visible = false;
        }

        private void RefreshStatusBar()
        {
            if (netcom.IsLoggedIn)
            {
                tlblLoginName.Text = netcom.LoginOptions.FullName;
                tlblMoneyBalance.Text = client.Self.Balance.ToString();
                icoHealth.Text = client.Self.Health.ToString() + "%";

                tlblRegionInfo.Text =
                    client.Network.CurrentSim.Name +
                    " (" + Math.Floor(client.Self.SimPosition.X).ToString() + ", " +
                    Math.Floor(client.Self.SimPosition.Y).ToString() + ", " +
                    Math.Floor(client.Self.SimPosition.Z).ToString() + ")";
            }
            else
            {
                tlblLoginName.Text = "Offline";
                tlblMoneyBalance.Text = "0";
                icoHealth.Text = "0%";
                tlblRegionInfo.Text = "No Region";
                tlblParcel.Text = "No Parcel";

                icoHealth.Visible = false;
                icoNoBuild.Visible = false;
                icoNoFly.Visible = false;
                icoNoPush.Visible = false;
                icoNoScript.Visible = false;
                icoNoVoice.Visible = false;
            }
        }

        private void RefreshWindowTitle()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Radegast - ");

            if (netcom.IsLoggedIn)
            {
                sb.Append("[" + netcom.LoginOptions.FullName + "]");

                if (instance.State.IsAway)
                {
                    sb.Append(" - Away");
                    if (instance.State.IsBusy) sb.Append(", Busy");
                }
                else if (instance.State.IsBusy)
                {
                    sb.Append(" - Busy");
                }

                if (instance.State.IsFollowing)
                {
                    sb.Append(" - Following ");
                    sb.Append(instance.State.FollowName);
                }
            }
            else
            {
                sb.Append("Logged Out");
            }

            this.Text = sb.ToString();
            sb = null;
        }

        private void InitializeStatusTimer()
        {
            statusTimer = new System.Timers.Timer(250);
            statusTimer.SynchronizingObject = this;
            statusTimer.Elapsed += new ElapsedEventHandler(statusTimer_Elapsed);
        }

        private void statusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Mono sometimes fires timer after is's disposed
            try
            {
                RefreshWindowTitle();
                RefreshStatusBar();
            }
            catch { }
        }
        #endregion

        #region Initialization, configuration, and key shortcuts
        private void InitializeTabsConsole()
        {
            tabsConsole = new TabsConsole(instance);
            tabsConsole.Dock = DockStyle.Fill;
            toolStripContainer1.ContentPanel.Controls.Add(tabsConsole);
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl-W: Close tab
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.W)
            {
                e.Handled = e.SuppressKeyPress = true;
                RadegastTab tab = tabsConsole.SelectedTab;
                
                if (tab.AllowClose)
                {
                    tab.Close();
                }
                else if (tab.AllowHide)
                {
                    tab.Hide();
                }

                return;
            }

            // Ctl-Shift-H: Teleport Home
            if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.H)
            {
                e.Handled = e.SuppressKeyPress = true;
                tmnuTeleportHome.PerformClick();
                return;
            }

            // Alt 1-8: Toggle various tabs
            if (e.Modifiers == Keys.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.D1:
                        e.Handled = e.SuppressKeyPress = true;
                        tabsConsole.Tabs["chat"].Select();
                        break;

                    case Keys.D2:
                        e.Handled = e.SuppressKeyPress = true;
                        tbtnFriends.PerformClick();
                        break;

                    case Keys.D3:
                        e.Handled = e.SuppressKeyPress = true;
                        tbtnGroups.PerformClick();
                        break;

                    case Keys.D4:
                        e.Handled = e.SuppressKeyPress = true;
                        tbtnInventory.PerformClick();
                        break;

                    case Keys.D5:
                        e.Handled = e.SuppressKeyPress = true;
                        tbtnSearch.PerformClick();
                        break;

                    case Keys.D6:
                        e.Handled = e.SuppressKeyPress = true;
                        tbtnMap.PerformClick();
                        break;

                    case Keys.D7:
                        e.Handled = e.SuppressKeyPress = true;
                        tbnObjects.PerformClick();
                        break;

                    case Keys.D8:
                        e.Handled = e.SuppressKeyPress = true;
                        tbtnMedia.PerformClick();
                        break;

                    case Keys.D9:
                        e.Handled = e.SuppressKeyPress = true;
                        tbtnVoice.PerformClick();
                        break;
                }
            }

            // ctrl-g, goto slurl
            if (e.Control && e.KeyCode == Keys.G)
            {
                if (!ProcessLink(Clipboard.GetText(), true))
                    MapToCurrentLocation();

                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            // ctrl-(shift)-tab for next/previous tab
            if (e.Control && e.KeyCode == Keys.Tab)
            {
                if (e.Shift)
                {
                    TabConsole.SelectPreviousTab();
                }
                else
                {
                    TabConsole.SelectNextTab();
                }
                e.Handled = e.SuppressKeyPress = true;
                return;
            }
        }

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            tabsConsole.SelectTab("login");
            ResourceManager rm = Properties.Resources.ResourceManager;
            ResourceSet set = rm.GetResourceSet(System.Globalization.CultureInfo.CurrentCulture, true, true);
            System.Collections.IDictionaryEnumerator de = set.GetEnumerator();
            while (de.MoveNext() == true)
            {
                if (de.Entry.Value is Image)
                {
                    Bitmap bitMap = de.Entry.Value as Bitmap;
                    ResourceImages.Images.Add(bitMap);
                    ImageNames.Add(de.Entry.Key.ToString());
                }
            }
            StartUpdateCheck(false);
        }
        #endregion

        #region Public methods

        private Dictionary<UUID, frmProfile> shownProfiles = new Dictionary<UUID, frmProfile>();

        public void ShowAgentProfile(string name, UUID agentID)
        {
            lock (shownProfiles)
            {
                frmProfile profile = null;
                if (shownProfiles.TryGetValue(agentID, out profile))
                {
                    profile.WindowState = FormWindowState.Normal;
                    profile.Focus();
                }
                else
                {
                    profile = new frmProfile(instance, name, agentID);

                    profile.Disposed += (object sender, EventArgs e) =>
                        {
                            lock (shownProfiles)
                            {
                                frmProfile agentProfile = (frmProfile)sender;
                                if (shownProfiles.ContainsKey(agentProfile.AgentID))
                                    shownProfiles.Remove(agentProfile.AgentID);
                            }
                        };

                    profile.Show();
                    profile.Focus();
                    shownProfiles.Add(agentID, profile);
                }
            }
        }

        private Dictionary<UUID, frmGroupInfo> shownGroupProfiles = new Dictionary<UUID, frmGroupInfo>();

        public void ShowGroupProfile(AvatarGroup group)
        {
            ShowGroupProfile(new OpenMetaverse.Group()
            {
                ID = group.GroupID,
                InsigniaID = group.GroupInsigniaID,
                Name = group.GroupName
            }
            );
        }

        public void ShowGroupProfile(OpenMetaverse.Group group)
        {
            lock (shownGroupProfiles)
            {
                frmGroupInfo profile = null;
                if (shownGroupProfiles.TryGetValue(group.ID, out profile))
                {
                    profile.WindowState = FormWindowState.Normal;
                    profile.Focus();
                }
                else
                {
                    profile = new frmGroupInfo(instance, group);

                    profile.Disposed += (object sender, EventArgs e) =>
                        {
                            lock (shownGroupProfiles)
                            {
                                frmGroupInfo groupProfile = (frmGroupInfo)sender;
                                if (shownGroupProfiles.ContainsKey(groupProfile.Group.ID))
                                    shownGroupProfiles.Remove(groupProfile.Group.ID);
                            }
                        };

                    profile.Show();
                    profile.Focus();
                    shownGroupProfiles.Add(group.ID, profile);
                }
            }
        }

        public void ProcessLink(string link)
        {
            ProcessLink(link, false);
        }

        public bool ProcessLink(string link, bool onlyMap)
        {
            if (!link.Contains("://"))
            {
                link = "http://" + link;
            }

            Regex r = new Regex(@"^(http://slurl.com/secondlife/|secondlife://)([^/]+)/(\d+)/(\d+)(/(\d+))?");
            Match m = r.Match(link);

            if (m.Success)
            {
                string region = HttpUtility.UrlDecode(m.Groups[2].Value);
                int x = int.Parse(m.Groups[3].Value);
                int y = int.Parse(m.Groups[4].Value);
                int z = 0;

                if (m.Groups.Count > 5 && m.Groups[6].Value != String.Empty)
                {
                    z = int.Parse(m.Groups[6].Value);
                }

                MapTab.Select();
                WorldMap.DisplayLocation(region, x, y, z);
                return true;
            }
            else if (!onlyMap)
            {
                System.Diagnostics.Process.Start(link);
            }
            return false;
        }
        #endregion

        #region Notifications
        CircularList<Control> notifications = new CircularList<Control>();

        public Color NotificationBackground
        {
            get { return pnlDialog.BackColor; }
        }

        void ResizeNotificationByControl(Control active)
        {
            int Width = active.Size.Width + 6;
            int Height = notifications.Count > 1 ? active.Size.Height + 3 + btnDialogNextControl.Size.Height : active.Size.Height + 3;
            pnlDialog.Size = new Size(Width, Height);
            pnlDialog.Top = 0;
            pnlDialog.Left = pnlDialog.Parent.ClientSize.Width - Width;

            btnDialogNextControl.Top = btnDialogNextControl.Parent.ClientSize.Height - btnDialogNextControl.Size.Height;
            btnDialogNextControl.Left = btnDialogNextControl.Parent.ClientSize.Width - btnDialogNextControl.Size.Width;

            btnDialogNextControl.BringToFront();
        }

        public void AddNotification(Control control)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    AddNotification(control);
                }
                ));
                return;
            }

            FormFlash.StartFlash(this);
            pnlDialog.Visible = true;
            pnlDialog.BringToFront();

            foreach (Control existing in notifications)
            {
                existing.Visible = false;
            }

            notifications.Add(control);
            control.Visible = true;
            control.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            control.Top = 3;
            control.Left = 3;
            pnlDialog.Controls.Add(control);
            ResizeNotificationByControl(control);

            btnDialogNextControl.Visible = notifications.Count > 1;
        }

        public void RemoveNotification(Control control)
        {
            pnlDialog.Controls.Remove(control);
            notifications.Remove(control);
            control.Dispose();

            if (notifications.HasNext)
            {
                pnlDialog.Visible = true;
                Control active = notifications.Next;
                active.Visible = true;
                ResizeNotificationByControl(active);
            }
            else
            {
                pnlDialog.Visible = false;
            }

            btnDialogNextControl.Visible = notifications.Count > 1;
        }

        private void btnDialogNextControl_Click(object sender, EventArgs e)
        {
            foreach (Control existing in notifications)
            {
                existing.Visible = false;
            }

            if (notifications.HasNext)
            {
                pnlDialog.Visible = true;
                Control active = notifications.Next;
                active.Visible = true;
                ResizeNotificationByControl(active);
            }
            else
            {
                pnlDialog.Visible = false;
            }

        }
        #endregion Notifications

        #region Menu click handlers

        private void tbtnTeleport_Click(object sender, EventArgs e)
        {
            (new frmTeleport(instance)).ShowDialog();
        }

        private void tmnuStatusAway_Click(object sender, EventArgs e)
        {
            instance.State.SetAway(tmnuStatusAway.Checked);
        }

        private void tmnuHelpReadme_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath + @"\Readme.txt");
        }

        private void tmnuStatusBusy_Click(object sender, EventArgs e)
        {
            instance.State.SetBusy(tmnuStatusBusy.Checked);
        }

        private void tmnuControlFly_Click(object sender, EventArgs e)
        {
            instance.State.SetFlying(tmnuControlFly.Checked);
        }

        private void tmnuControlAlwaysRun_Click(object sender, EventArgs e)
        {
            instance.State.SetAlwaysRun(tmnuControlAlwaysRun.Checked);
        }

        private void tmnuPrefs_Click(object sender, EventArgs e)
        {
            (new frmSettings(instance)).ShowDialog();
        }

        private void tbtnAppearance_Click(object sender, EventArgs e)
        {
            client.Appearance.RequestSetAppearance(false);
        }

        private void importObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrimDeserializer.ImportFromFile(client);
        }

        private void autopilotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ap == null)
            {
                ap = new AutoPilot(client);
                /*
                ap.InsertWaypoint(new Vector3(66, 163, 21));
                ap.InsertWaypoint(new Vector3(66, 98, 21));

                ap.InsertWaypoint(new Vector3(101, 98, 21));
                ap.InsertWaypoint(new Vector3(101, 45, 21));
                ap.InsertWaypoint(new Vector3(93, 27, 21));
                ap.InsertWaypoint(new Vector3(106, 12, 21));
                ap.InsertWaypoint(new Vector3(123, 24, 21));
                ap.InsertWaypoint(new Vector3(114, 45, 21));
                ap.InsertWaypoint(new Vector3(114, 98, 21));

                ap.InsertWaypoint(new Vector3(130, 98, 21));
                ap.InsertWaypoint(new Vector3(130, 163, 21));
                 **/
                ap.InsertWaypoint(new Vector3(64, 68, 21));
                ap.InsertWaypoint(new Vector3(65, 20, 21));
                ap.InsertWaypoint(new Vector3(33, 23, 21));
                ap.InsertWaypoint(new Vector3(17, 39, 21));
                ap.InsertWaypoint(new Vector3(17, 62, 21));


            }
            if (AutoPilotActive)
            {
                AutoPilotActive = false;
                ap.Stop();
            }
            else
            {
                AutoPilotActive = true;
                ap.Start();
            }

        }

        private void cleanCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Assets.Cache.Clear();
        }

        private void rebakeTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Appearance.RequestSetAppearance(true);
        }

        public void MapToCurrentLocation()
        {
            if (MapTab != null && client.Network.Connected)
            {
                MapTab.Select();
                WorldMap.DisplayLocation(client.Network.CurrentSim.Name,
                    (int)client.Self.SimPosition.X,
                    (int)client.Self.SimPosition.Y,
                    (int)client.Self.SimPosition.Z);
            }
        }

        private void standToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Self.Stand();
        }

        private void groundSitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Self.SitOnGround();
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try { System.Diagnostics.Process.Start(Application.ExecutablePath); }
            catch (Exception) { }
        }

        private void tmnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void tlblRegionInfo_Click(object sender, EventArgs e)
        {
            if (WorldMap != null && client.Network.Connected)
            {
                MapTab.Select();
            }
        }

        private void scriptEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptEditor se = new ScriptEditor(instance);
            se.Dock = DockStyle.Fill;
            se.ShowDetached();
        }

        private void tmnuSetHome_Click(object sender, EventArgs e)
        {
            client.Self.SetHome();
        }

        private void tmnuCreateLandmark_Click(object sender, EventArgs e)
        {
            string location = string.Format(", {0} ({1}, {2}, {3})",
                client.Network.CurrentSim.Name,
                (int)client.Self.SimPosition.X,
                (int)client.Self.SimPosition.Y,
                (int)client.Self.SimPosition.Z
                );

            string name = tlblParcel.Text;
            int maxLen = 63 - location.Length;

            if (name.Length > maxLen)
                name = name.Substring(0, maxLen);

            name += location;

            client.Inventory.RequestCreateItem(
                client.Inventory.FindFolderForType(AssetType.Landmark),
                name,
                tlblParcel.ToolTipText,
                AssetType.Landmark,
                UUID.Random(),
                InventoryType.Landmark,
                PermissionMask.All,
                (bool success, InventoryItem item) =>
                {
                    if (success)
                    {
                        BeginInvoke(new MethodInvoker(() =>
                            {
                                Landmark ln = new Landmark(instance, (InventoryLandmark)item);
                                ln.Dock = DockStyle.Fill;
                                ln.Detached = true;
                            }));
                    }
                }
            );
        }

        private void tmnuTeleportHome_Click(object sender, EventArgs e)
        {
            if (WorldMap != null)
            {
                WorldMap.GoHome();
            }
        }

        private TimeZoneInfo SLTime;

        private void GetSLTimeZone()
        {
            try
            {
                foreach (TimeZoneInfo tz in TimeZoneInfo.GetSystemTimeZones())
                {
                    if (tz.Id == "Pacific Standard Time" || tz.Id == "America/Los_Angeles")
                    {
                        SLTime = tz;
                        break;
                    }
                }
            }
            catch (Exception) { }
        }

        private void timerWorldClock_Tick(object sender, EventArgs e)
        {
            DateTime now;
            try
            {
                if (SLTime != null)
                    now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, SLTime);
                else
                    now = DateTime.UtcNow.AddHours(-7);
            }
            catch (Exception)
            {
                now = DateTime.UtcNow.AddHours(-7);
            }
            lblTime.Text = now.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture);
        }

        private void reportBugsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessLink("http://jira.openmetaverse.org/browse/RAD");
        }

        private void aboutRadegastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new frmAbout(instance)).ShowDialog();
        }

        #region Update Checking
        private UpdateChecker updateChecker = null;
        private bool ManualUpdateCheck = false;

        public void StartUpdateCheck(bool userInitiated)
        {
            ManualUpdateCheck = userInitiated;

            if (updateChecker != null)
            {
                if (ManualUpdateCheck)
                    tabsConsole.DisplayNotificationInChat("Update check already in progress.");
                return;
            }

            if (ManualUpdateCheck)
                tabsConsole.DisplayNotificationInChat("Checking for updates...", ChatBufferTextStyle.StatusBlue);
            updateChecker = new UpdateChecker();
            updateChecker.OnUpdateInfoReceived += new UpdateChecker.UpdateInfoCallback(OnUpdateInfoReceived);
            updateChecker.StartCheck();
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabsConsole.SelectTab("chat");
            StartUpdateCheck(true);
        }

        void OnUpdateInfoReceived(object sender, UpdateCheckerArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => OnUpdateInfoReceived(sender, e)));
                return;
            }

            if (!e.Success)
            {
                if (ManualUpdateCheck)
                    tabsConsole.DisplayNotificationInChat("Error: Failed connecting to the update site.", ChatBufferTextStyle.StatusBlue);
            }
            else
            {
                if (!ManualUpdateCheck && e.Info.DisplayMOTD)
                {
                    tabsConsole.DisplayNotificationInChat(e.Info.MOTD, ChatBufferTextStyle.StatusBlue);
                }

                if (e.Info.UpdateAvailable)
                {
                    tabsConsole.DisplayNotificationInChat("New version available at " + e.Info.DownloadSite, ChatBufferTextStyle.Alert);
                }
                else
                {
                    if (ManualUpdateCheck)
                        tabsConsole.DisplayNotificationInChat("Your version is up to date.", ChatBufferTextStyle.StatusBlue);
                }
            }

            updateChecker.Dispose();
            updateChecker = null;
        }
        #endregion

        private void ToggleHidden(string tabName)
        {
            if (!tabsConsole.TabExists(tabName)) return;

            RadegastTab tab = tabsConsole.Tabs[tabName];

            if (tab.Hidden)
            {
                tab.Show();
            }
            else
            {
                if (!tab.Selected)
                {
                    tab.Select();
                }
                else
                {
                    tab.Hide();
                }
            }
        }

        private void tbtnFriends_Click(object sender, EventArgs e)
        {
            ToggleHidden("friends");
        }

        private void tbtnInventory_Click(object sender, EventArgs e)
        {
            ToggleHidden("inventory");
        }

        private void tbtnSearch_Click(object sender, EventArgs e)
        {
            ToggleHidden("search");
        }

        private void tbtnGroups_Click(object sender, EventArgs e)
        {
            ToggleHidden("groups");
        }

        private void tbtnVoice_Click(object sender, EventArgs e)
        {
            ToggleHidden("voice");
        }

        private void tbtnMedia_Click(object sender, EventArgs e)
        {
            if (tabsConsole.TabExists("media"))
            {
                ToggleHidden("media");
            }
            else
            {
                RadegastTab tab = tabsConsole.AddTab("media", "Media", mediaConsole);
                tab.AllowClose = false;
                tab.AllowHide = true;
                tab.Select();
            }
        }

        private void tbnObjects_Click(object sender, EventArgs e)
        {
            if (tabsConsole.TabExists("objects"))
            {
                RadegastTab tab = tabsConsole.Tabs["objects"];
                if (!tab.Selected)
                {
                    tab.Select();
                    ((ObjectsConsole)tab.Control).RefreshObjectList();
                }
                else
                {
                    tab.Close();
                }
            }
            else
            {
                RadegastTab tab = tabsConsole.AddTab("objects", "Objects", new ObjectsConsole(instance));
                tab.AllowClose = true;
                tab.AllowDetach = true;
                tab.Visible = true;
                tab.AllowHide = false;
                tab.Select();
                ((ObjectsConsole)tab.Control).RefreshObjectList();
            }
        }

        private void tbtnMap_Click(object sender, EventArgs e)
        {
            ToggleHidden("map");
            if (!MapTab.Hidden)
                MapToCurrentLocation();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (client.Network.Connected)
                client.Network.RequestLogout();
        }

        private void reconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            instance.Reconnect();
        }

        private frmKeyboardShortcuts keyboardShortcutsForm = null;
        private void keyboardShortcutsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (keyboardShortcutsForm != null)
            {
                keyboardShortcutsForm.Focus();
            }
            else
            {
                keyboardShortcutsForm = new frmKeyboardShortcuts(instance);

                keyboardShortcutsForm.Disposed += (object senderx, EventArgs ex) =>
                    {
                        if (components != null)
                        {
                            components.Remove(keyboardShortcutsForm);
                        }
                        keyboardShortcutsForm = null;
                    };

                keyboardShortcutsForm.Show(this);
                keyboardShortcutsForm.Top = Top + 100;
                keyboardShortcutsForm.Left = Left + 100;

                if (components != null)
                {
                    components.Add(keyboardShortcutsForm);
                }
            }
        }

        // Menu item for testing out stuff
        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (KeyValuePair<UUID, string> kvp in Sounds.ToDictionary())
            {
                client.Assets.RequestAsset(kvp.Key, AssetType.Sound, true, (AssetDownload transfer, Asset asset) =>
                    {
                        System.Console.WriteLine("Sound '{0}' download success: {1}", transfer.AssetID, transfer.Success);
                    }
                );
            }
        }

        #endregion
    }
}