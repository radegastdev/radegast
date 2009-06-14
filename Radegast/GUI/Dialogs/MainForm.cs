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
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using System.Resources;
using System.IO;
using RadegastNc;
using OpenMetaverse;
using OpenMetaverse.Imaging;

namespace Radegast
{
    public partial class frmMain : Form
    {
        #region Public members
        public static ImageList ResourceImages = new ImageList();
        public static List<string> ImageNames = new List<string>();
        public frmMap worldMap;
        public TabsConsole TabConsole
        {
            get { return tabsConsole; }
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
        #endregion

        #region Constructor and disposal
        public frmMain(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmMain_Disposed);

            this.instance = instance;
            client = this.instance.Client;
            netcom = this.instance.Netcom;
            netcom.NetcomSync = this;

            pnlDialog.Visible = false;
            btnDialogNextControl = new TransparentButton();
            pnlDialog.Controls.Add(btnDialogNextControl);

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

            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            client.Parcels.OnParcelProperties += new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);
            client.Self.OnMoneyBalanceReplyReceived += new AgentManager.MoneyBalanceReplyCallback(Self_OnMoneyBalanceReplyReceived);
            
            InitializeStatusTimer();
            RefreshWindowTitle();

            ApplyConfig(this.instance.Config.CurrentConfig, true);
            this.instance.Config.ConfigApplied += new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
        }

        void frmMain_Disposed(object sender, EventArgs e)
        {
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected -= new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            client.Parcels.OnParcelProperties -= new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);
            client.Self.OnMoneyBalanceReplyReceived -= new AgentManager.MoneyBalanceReplyCallback(Self_OnMoneyBalanceReplyReceived);

            this.instance.Config.ConfigApplied -= new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
            this.instance.CleanUp();
        }
        #endregion

        #region Event handlers
        void Self_OnMoneyBalanceReplyReceived(UUID transactionID, bool transactionSuccess, int balance, int metersCredit, int metersCommitted, string description)
        {
            if (!String.IsNullOrEmpty(description))
            {
                AddNotification(new ntfGeneric(instance, description));
            }
        }

        private void Config_ConfigApplied(object sender, ConfigAppliedEventArgs e)
        {
            ApplyConfig(e.AppliedConfig, false);
        }

        private void ApplyConfig(Config config, bool doingInit)
        {
            if (doingInit)
                this.WindowState = (FormWindowState)config.MainWindowState;

            if (config.InterfaceStyle == 0) //System
                toolStrip1.RenderMode = ToolStripRenderMode.System;
            else if (config.InterfaceStyle == 1) //Office 2003
                toolStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;
            aLICEToolStripMenuItem.Checked = config.UseAlice;
        }

        public void InitializeControls()
        {
            InitializeTabsConsole();
            InitializeDebugLogForm();
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;

            tbnObjects.Enabled = tbtnStatus.Enabled = tbtnControl.Enabled = tbnTeleprotMulti.Enabled = tmnuImport.Enabled = true;
            statusTimer.Start();
            RefreshWindowTitle();
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            tbnObjects.Enabled = tbtnStatus.Enabled = tbtnControl.Enabled = tbnTeleprotMulti.Enabled = tmnuImport.Enabled = false;

            statusTimer.Stop();

            RefreshStatusBar();
            RefreshWindowTitle();
        }

        private void netcom_ClientDisconnected(object sender, ClientDisconnectEventArgs e)
        {
            if (e.Type == NetworkManager.DisconnectType.ClientInitiated) return;

            tbnObjects.Enabled = tbtnStatus.Enabled = tbtnControl.Enabled = tbnTeleprotMulti.Enabled = false;

            statusTimer.Stop();

            RefreshStatusBar();
            RefreshWindowTitle();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (debugLogForm != null) {
                debugLogForm.Close();
                debugLogForm = null;
            };

            if (worldMap != null)
            {
                worldMap.Close();
                worldMap = null;
            }

            if (netcom.IsLoggedIn) netcom.Logout();

            instance.Config.CurrentConfig.MainWindowState = (int)this.WindowState;
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

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Alt && e.KeyCode == Keys.D)
                tbtnDebug.Visible = !tbtnDebug.Visible;

            if (e.Control && e.KeyCode == Keys.O && client.Network.Connected)
            {
                (new frmObjects(instance)).Show();
            }
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            worldMap = new frmMap(instance);
            tabsConsole.SelectTab("Main");
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
        public void processLink(string link)
        {
            if (!(link.StartsWith("http://") || link.StartsWith("ftp://")))
            {
                link = "http://" + link;
            }

            Regex r = new Regex(@"^(http://slurl.com/secondlife/|secondlife://)([^/]+)/(\d+)/(\d+)(/(\d+))?");
            Match m = r.Match(link);

            if (m.Success)
            {
                string region = Uri.UnescapeDataString(m.Groups[2].Value);
                int x = int.Parse(m.Groups[3].Value);
                int y = int.Parse(m.Groups[4].Value);
                int z = 0;

                if (m.Groups.Count > 5 && m.Groups[6].Value != String.Empty)
                {
                    z = int.Parse(m.Groups[6].Value);
                }
            
                worldMap.Show();
                worldMap.Focus();
                worldMap.displayLocation(region, x, y, z);
            }
            else
            {
                System.Diagnostics.Process.Start(link);
            }

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
            (new frmPreferences(instance)).ShowDialog();
        }

        private void tbtnObjects_Click(object sender, EventArgs e)
        {
            (new frmObjects(instance)).Show();
        }

        private void tbtnAppearance_Click(object sender, EventArgs e)
        {
            client.Appearance.SetPreviousAppearance(true);
        }

        private void groupsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            (new GroupsDialog(instance)).Show();
        }

        private void homeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            worldMap.GoHome();
        }

        private void importObjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrimDeserializer.ImportFromFile(client);
        }

        private void autopilotToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ap == null) {
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
            if (AutoPilotActive) {
                AutoPilotActive = false;
                ap.Stop();
            } else {
                AutoPilotActive = true;
                ap.Start();
            }

        }

        private void aLICEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (instance.Config.CurrentConfig.UseAlice == false) {
                instance.Config.CurrentConfig.UseAlice = true;
                aLICEToolStripMenuItem.Checked = true;
            } else {
                instance.Config.CurrentConfig.UseAlice = false;
                aLICEToolStripMenuItem.Checked = false;
            }
            instance.Config.SaveCurrentConfig();
        }

        private void cleanCacheToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Assets.Cache.Clear();
            DirectoryInfo di = new DirectoryInfo(instance.animCacheDir);
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
            client.Appearance.ForceRebakeAvatarTextures();
        }

        private void mapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!worldMap.Visible)
            {
                worldMap.Show();
            }
            else
            {
                worldMap.Focus();
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

        private void tbnObjects_Click(object sender, EventArgs e)
        {
            (new frmObjects(instance)).Show();
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
        #endregion
    }
}