using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Text;
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
        public static ImageList ResourceImages = new ImageList();
        public static List<string> ImageNames = new List<string>();

        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;

        public TabsConsole tabsConsole;
        public frmMap worldMap;
        private frmDebugLog debugLogForm;
        private System.Timers.Timer statusTimer;
        private AutoPilot ap;
        private bool AutoPilotActive = false;

        public frmMain(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmMain_Disposed);

            this.instance = instance;
            client = this.instance.Client;
            netcom = this.instance.Netcom;
            netcom.NetcomSync = this;

            pnlDialog.Visible = false;

            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            netcom.InstantMessageReceived += new EventHandler<InstantMessageEventArgs>(netcom_InstantMessageReceived);
            client.Parcels.OnParcelProperties += new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);
            
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
            netcom.InstantMessageReceived -= new EventHandler<InstantMessageEventArgs>(netcom_InstantMessageReceived);
            client.Parcels.OnParcelProperties -= new ParcelManager.ParcelPropertiesCallback(Parcels_OnParcelProperties);

            this.instance.Config.ConfigApplied -= new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
            this.instance.CleanUp();
        }

        //Separate thread
        private void Parcels_OnParcelProperties(Simulator simulator, Parcel parcel, ParcelResult result, int selectedPrims,
            int sequenceID, bool snapSelection) 
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Parcel: ");
            sb.Append(parcel.Name);

            if ((parcel.Flags & ParcelFlags.AllowFly) != ParcelFlags.AllowFly)
                sb.Append(" (no fly)");

            if ((parcel.Flags & ParcelFlags.CreateObjects) != ParcelFlags.CreateObjects)
                sb.Append(" (no build)");

            if ((parcel.Flags & ParcelFlags.AllowOtherScripts) != ParcelFlags.AllowOtherScripts)
                sb.Append(" (no scripts)");

            if ((parcel.Flags & ParcelFlags.RestrictPushObject) == ParcelFlags.RestrictPushObject)
                sb.Append(" (no push)");

            if ((parcel.Flags & ParcelFlags.AllowDamage) == ParcelFlags.AllowDamage)
                sb.Append(" (damage)");

            BeginInvoke(new MethodInvoker(delegate()
            {
                tlblParcel.Text = sb.ToString();
            }));
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

        private void statusTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            RefreshWindowTitle();
            RefreshStatusBar();
        }

        private void netcom_InstantMessageReceived(object sender, InstantMessageEventArgs e)
        {
            if (e.IM.Dialog == InstantMessageDialog.StartTyping ||
                e.IM.Dialog == InstantMessageDialog.StopTyping)
                return;

            if (!this.Focused) FormFlash.Flash(this);
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;

            tbtnStatus.Enabled = tbtnControl.Enabled = tbnTeleprotMulti.Enabled = tmnuImport.Enabled = true;
            statusTimer.Start();
            RefreshWindowTitle();
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            tbtnStatus.Enabled = tbtnControl.Enabled = tbnTeleprotMulti.Enabled = tmnuImport.Enabled = false;

            statusTimer.Stop();

            RefreshStatusBar();
            RefreshWindowTitle();
        }

        private void netcom_ClientDisconnected(object sender, ClientDisconnectEventArgs e)
        {
            if (e.Type == NetworkManager.DisconnectType.ClientInitiated) return;

            tbtnStatus.Enabled = tbtnControl.Enabled = tbnTeleprotMulti.Enabled = false;

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

        private void RefreshStatusBar()
        {
            if (netcom.IsLoggedIn)
            {
                tlblLoginName.Text = netcom.LoginOptions.FullName;
                tlblMoneyBalance.Text = "L$" + client.Self.Balance.ToString();
                tlblHealth.Text = "Health: " + client.Self.Health.ToString();

                tlblRegionInfo.Text =
                    client.Network.CurrentSim.Name +
                    " (" + Math.Floor(client.Self.SimPosition.X).ToString() + ", " +
                    Math.Floor(client.Self.SimPosition.Y).ToString() + ", " +
                    Math.Floor(client.Self.SimPosition.Z).ToString() + ")";

                int totalPrims = 0;
                int totalAvatars = 0;

                foreach (Simulator sim in client.Network.Simulators)
                {
                    totalPrims += sim.ObjectsPrimitives.Count;
                    totalAvatars += sim.ObjectsAvatars.Count;
                }

                toolTip1.SetToolTip(
                    statusStrip1,
                    "Region: " + client.Network.CurrentSim.Name + "\n" +
                    "X: " + client.Self.SimPosition.X.ToString() + "\n" +
                    "Y: " + client.Self.SimPosition.Y.ToString() + "\n" +
                    "Z: " + client.Self.SimPosition.Z.ToString() + "\n\n" +
                    "Nearby prims: " + totalPrims.ToString() + "\n" +
                    "Nearby avatars: " + totalAvatars.ToString());
            }
            else
            {
                tlblLoginName.Text = "Offline";
                tlblMoneyBalance.Text = "L$0";
                tlblHealth.Text = "Health: 0";
                tlblRegionInfo.Text = "No Region";
                tlblParcel.Text = "No Parcel";
                toolTip1.SetToolTip(statusStrip1, string.Empty);
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

        private void tmnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Alt && e.KeyCode == Keys.D)
                tbtnDebug.Visible = !tbtnDebug.Visible;
        }

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

        public TabsConsole TabConsole
        {
            get { return tabsConsole; }
        }

        private void tmnuPrefs_Click(object sender, EventArgs e)
        {
            (new frmPreferences(instance)).ShowDialog();
        }

        private void tmnuDonate_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"http://delta.slinked.net/donate/");
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

        #region Notifications
        CircularList<Control> notifications = new CircularList<Control>();

        public Color NotificationBackground
        {
            get { return pnlDialog.BackColor; }
        }

        void ResizeNotificationByControl(Control active)
        {
            int Width = active.Size.Width + 6;
            int Height = notifications.HasNext ? active.Size.Height + 20 : active.Size.Height + 6;
            pnlDialog.Size = new Size(Width, Height);
            pnlDialog.Top = 0;
            pnlDialog.Left = pnlDialog.Parent.ClientSize.Width - Width;
            btnDialogNextControl.BringToFront();
        }

        public void AddNotification(Control control)
        {
            this.Focus();
            pnlDialog.Visible = true;

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
    }
}