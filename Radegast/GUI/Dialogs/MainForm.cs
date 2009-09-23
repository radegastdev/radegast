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

namespace Radegast
{
    public partial class frmMain : RadegastForm
    {
        #region Public members
        public static ImageList ResourceImages = new ImageList();
        public static List<string> ImageNames = new List<string>();
        public frmMap WorldMap { get { return worldMap; } }
        private frmMap worldMap;
        public TabsConsole TabConsole
        {
            get { return tabsConsole; }
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
        private RadegastNetcom netcom;
        private GridClient client;
        private TabsConsole tabsConsole;
        private frmDebugLog debugLogForm;
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
            client = this.instance.Client;
            netcom = this.instance.Netcom;
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
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            client.Parcels.OnParcelProperties += new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);
            client.Self.OnMoneyBalanceReplyReceived += new AgentManager.MoneyBalanceReplyCallback(Self_OnMoneyBalanceReplyReceived);

            InitializeStatusTimer();
            RefreshWindowTitle();
        }

        void frmMain_Disposed(object sender, EventArgs e)
        {
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected -= new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            client.Parcels.OnParcelProperties -= new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);
            client.Self.OnMoneyBalanceReplyReceived -= new AgentManager.MoneyBalanceReplyCallback(Self_OnMoneyBalanceReplyReceived);
            this.instance.CleanUp();
        }
        #endregion

        #region Event handlers
        void Self_OnMoneyBalanceReplyReceived(UUID transactionID, bool transactionSuccess, int balance, int metersCredit, int metersCommitted, string description)
        {
            if (!String.IsNullOrEmpty(description))
            {
                if (instance.GlobalSettings["transaction_notification_dialog"].AsBoolean())
                    AddNotification(new ntfGeneric(instance, description));
                if (instance.GlobalSettings["transaction_notification_chat"].AsBoolean())
                    TabConsole.DisplayNotificationInChat(description);
            }
        }

        public void InitializeControls()
        {
            InitializeTabsConsole();
            InitializeDebugLogForm();
            if (instance.MediaManager.SoundSystemAvailable)
            {
                mediaConsole = new MediaConsole(instance);
                tbtnMedia.Visible = true;
            }
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;

            tbtnGroups.Enabled = tbnObjects.Enabled = tbtnWorld.Enabled = tbnTools.Enabled = tmnuImport.Enabled = true;
            statusTimer.Start();
            RefreshWindowTitle();
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            tbtnGroups.Enabled = tbnObjects.Enabled = tbtnWorld.Enabled = tbnTools.Enabled = tmnuImport.Enabled = false;

            statusTimer.Stop();

            RefreshStatusBar();
            RefreshWindowTitle();
        }

        private void netcom_ClientDisconnected(object sender, ClientDisconnectEventArgs e)
        {
            if (e.Type == NetworkManager.DisconnectType.ClientInitiated) return;

            tbnObjects.Enabled = tbtnWorld.Enabled = tbnTools.Enabled = false;

            statusTimer.Stop();

            RefreshStatusBar();
            RefreshWindowTitle();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mediaConsole != null)
            {
                mediaConsole.Dispose();
                mediaConsole = null;
            }

            if (debugLogForm != null)
            {
                debugLogForm.Close();
                debugLogForm.Dispose();
                debugLogForm = null;
            }

            if (worldMap != null)
            {
                worldMap.Close();
                worldMap.Dispose();
                worldMap = null;
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

        private void Parcels_OnParcelProperties(Simulator simulator, Parcel parcel, ParcelResult result, int selectedPrims,
    int sequenceID, bool snapSelection)
        {
            if (result != ParcelResult.Single) return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Parcels_OnParcelProperties(simulator, parcel, result, selectedPrims, sequenceID, snapSelection);
                }
                ));
                return;
            }

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
            RefreshWindowTitle();
            RefreshStatusBar();
        }
        #endregion

        #region Initialization, configuration, and key shortcuts
        private void InitializeTabsConsole()
        {
            tabsConsole = new TabsConsole(instance);
            tabsConsole.Dock = DockStyle.Fill;
            toolStripContainer1.ContentPanel.Controls.Add(tabsConsole);
        }

        private void InitializeDebugLogForm()
        {
            debugLogForm = new frmDebugLog(instance);
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Alt && e.KeyCode == Keys.D)
            {
                e.Handled = e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.O && client.Network.Connected)
            {
                e.Handled = e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.G)
            {
                e.Handled = e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.M && Clipboard.ContainsText() && client.Network.Connected)
            {
                e.Handled = e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.Right)
            {
                e.Handled = e.SuppressKeyPress = true;
            }

            if (e.Control && e.KeyCode == Keys.Left)
            {
                e.Handled = e.SuppressKeyPress = true;
            }
        }


        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            // alt-ctrl-d activate debug menu
            if (e.Control && e.Alt && e.KeyCode == Keys.D)
            {
                tbtnDebug.Visible = !tbtnDebug.Visible;
                e.Handled = e.SuppressKeyPress = true;
            }

            // ctrl-o, open objects finder
            if (e.Control && e.KeyCode == Keys.O && client.Network.Connected)
            {
                (new frmObjects(instance)).Show();
                e.Handled = e.SuppressKeyPress = true;
            }

            // ctrl-g, goto slurl
            if (e.Control && e.KeyCode == Keys.G && Clipboard.ContainsText() && client.Network.Connected)
            {
                if (!ProcessLink(Clipboard.GetText(), true))
                    MapToCurrentLocation();
                e.Handled = e.SuppressKeyPress = true;
            }

            // ctrl-m, open map
            if (e.Control && e.KeyCode == Keys.M && Clipboard.ContainsText() && client.Network.Connected)
            {
                MapToCurrentLocation();
                e.Handled = e.SuppressKeyPress = true;
            }

            // ctrl-right_arrow, select next tab
            if (e.Control && e.KeyCode == Keys.Right)
            {
                TabConsole.SelectNextTab();
                e.Handled = e.SuppressKeyPress = true;
            }

            // ctrl-right_arrow, select previous tab
            if (e.Control && e.KeyCode == Keys.Left)
            {
                TabConsole.SelectPreviousTab();
                e.Handled = e.SuppressKeyPress = true;
            }
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            worldMap = new frmMap(instance);
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
        }
        #endregion

        #region Public methods

        public void AddLogMessage(string msg, log4net.Core.Level level)
        {
            if (debugLogForm != null && !debugLogForm.IsDisposed)
            {            
                // Log form handlles the InvokeNeeded                                      
                debugLogForm.AddLogMessage(msg, level);
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

                worldMap.Show();
                worldMap.Focus();
                worldMap.DisplayLocation(region, x, y, z);
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

        private void tmnuDebugLog_Click(object sender, EventArgs e)
        {
            debugLogForm.Show();
        }

        private void tmnuPrefs_Click(object sender, EventArgs e)
        {
            (new frmSettings(instance)).ShowDialog();
        }

        private void tbtnObjects_Click(object sender, EventArgs e)
        {
            (new frmObjects(instance)).Show();
        }

        private void tbtnAppearance_Click(object sender, EventArgs e)
        {
            client.Appearance.RequestSetAppearance(false);
        }

        private void groupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new GroupsDialog(instance)).Show();
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
            DirectoryInfo di = new DirectoryInfo(instance.AnimCacheDir);
            FileInfo[] files = di.GetFiles();

            int num = 0;
            foreach (FileInfo file in files)
            {
                file.Delete();
                ++num;
            }

            Logger.Log("Wiped out " + num + " files from the anim cache directory.", Helpers.LogLevel.Debug);

        }

        private void rebakeTexturesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Appearance.RequestSetAppearance(true);
        }

        public void MapToCurrentLocation()
        {
            if (worldMap != null && client.Network.Connected)
            {
                worldMap.Show();
                worldMap.Focus();
                worldMap.DisplayLocation(client.Network.CurrentSim.Name,
                    (int)client.Self.SimPosition.X,
                    (int)client.Self.SimPosition.Y,
                    (int)client.Self.SimPosition.Z);
            }
        }

        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MapToCurrentLocation();
        }

        private void standToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Self.Stand();
        }

        private void groundSitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Self.SitOnGround();
        }

        private frmObjects objectWindow;

        private void tbnObjects_Click(object sender, EventArgs e)
        {
            if (objectWindow == null)
            {
                objectWindow = new frmObjects(instance);
                objectWindow.Disposed += new EventHandler(objectWindow_Disposed);
                objectWindow.Show();
            }
            objectWindow.Focus();
        }

        void objectWindow_Disposed(object sender, EventArgs e)
        {
            objectWindow = null;
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
            if (worldMap != null && client.Network.Connected)
            {
                worldMap.Show();
            }
        }

        private void tbtnGroups_Click(object sender, EventArgs e)
        {
            (new GroupsDialog(instance)).Show();
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
            worldMap.GoHome();
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

        private void tbtnMedia_Click(object sender, EventArgs e)
        {
            if (!mediaConsole.Detached)
                mediaConsole.Detached = true;
            else
                mediaConsole.Focus();
        }

        private void reportBugsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessLink("http://jira.openmetaverse.org/browse/RAD");
        }

        private void aboutRadegastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new frmAbout(instance)).ShowDialog();
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabsConsole.SelectTab("chat");
            tabsConsole.DisplayNotificationInChat("Checking for updates...", ChatBufferTextStyle.StatusBlue);
            UpdateChecker upd = new UpdateChecker();
            upd.OnUpdateInfoReceived += new UpdateChecker.UpdateInfoCallback(OnUpdateInfoReceived);
            upd.StartCheck();
        }

        void OnUpdateInfoReceived(object sender, UpdateCheckerArgs e)
        {
            if (!e.Success)
            {
                tabsConsole.DisplayNotificationInChat("Error: Failed connecting to the update site.");
            }
            ((UpdateChecker)sender).Dispose();
        }
        #endregion
    }
}