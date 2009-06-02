using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Security.Permissions;
using System.IO;
using OpenMetaverse;

namespace Radegast
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class frmMap : Form
    {
        RadegastInstance instance;
        GridClient client {get {return instance.Client;}}
        bool Active { get { return client.Network.Connected; } }
        WebBrowser map;
        Regex slscheme = new Regex("^secondlife://(.+)/([0-9]+)/([0-9]+)");
        bool InTeleport = false;

        public frmMap(RadegastInstance i)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmMap_Disposed);

            instance = i;
            map = new WebBrowser();
            map.Dock = DockStyle.Fill;
            map.AllowWebBrowserDrop = false;
            map.Navigate(Path.GetDirectoryName(Application.ExecutablePath) + @"/slmap.html");
            map.WebBrowserShortcutsEnabled = false;
            // map.ScriptErrorsSuppressed = true;
            map.ObjectForScripting = this;
            map.AllowNavigation = false;
            pnlMap.Controls.Add(map);
            pnlSearch.Visible = Active;

            // Register callbacks
            client.Grid.OnGridRegion += new GridManager.GridRegionCallback(Grid_OnGridRegion);
            client.Network.OnDisconnected += new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Self.OnTeleport += new AgentManager.TeleportCallback(Self_OnTeleport);
            client.Network.OnLogin += new NetworkManager.LoginCallback(Network_OnLogin);
        }

        void frmMap_Disposed(object sender, EventArgs e)
        {
            // Unregister callbacks
            client.Grid.OnGridRegion -= new GridManager.GridRegionCallback(Grid_OnGridRegion);
            client.Network.OnDisconnected -= new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Self.OnTeleport -= new AgentManager.TeleportCallback(Self_OnTeleport);
            client.Network.OnLogin -= new NetworkManager.LoginCallback(Network_OnLogin);
        }

        #region PublicMethods
        public void GoHome()
        {
            btnGoHome_Click(this, new EventArgs());
        }
        #endregion

        #region NetworkEvents

        void Self_OnTeleport(string message, TeleportStatus status, TeleportFlags flags)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Self_OnTeleport(message, status, flags);
                }
                ));
                return;
            }

            switch (status)
            {
                case TeleportStatus.Progress:
                    lblStatus.Text = "Progress: " + message;
                    InTeleport = true;
                    break;

                case TeleportStatus.Start:
                    lblStatus.Text = "Teleporting to " + txtRegion.Text;
                    InTeleport = true;
                    break;
             
                case TeleportStatus.Cancelled:
                case TeleportStatus.Failed:
                    InTeleport = false;
                    lblStatus.Text = "Failed: " + message;
                    break;
                
                case TeleportStatus.Finished:
                    InTeleport = false;
                    lblStatus.Text = "Success, now in " + client.Network.CurrentSim.Name;
                    gotoRegion(client.Network.CurrentSim.Name);
                    break;

                default:
                    InTeleport = false;
                    break;
            }

            if (!InTeleport)
            {
                prgTeleport.Style = ProgressBarStyle.Blocks;
            }
        }

        void Network_OnDisconnected(NetworkManager.DisconnectType reason, string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Network_OnDisconnected(reason, message);
                }
                ));
                return;
            }
            pnlSearch.Visible = false;
        }

        void Network_OnLogin(LoginStatus login, string message)
        {
            if (login != LoginStatus.Success) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Network_OnLogin(login, message);
                }
                ));
                return;
            }
            pnlSearch.Visible = true;
        }

        void Grid_OnGridRegion(GridRegion region)
        {
            if (InvokeRequired) 
            {
                BeginInvoke(new MethodInvoker(delegate ()
                    {
                        Grid_OnGridRegion(region);
                    }
                ));
                return;
            }

            ListViewItem item = new ListViewItem(region.Name);
            lstRegions.Items.Add(item);
        }
        #endregion NetworkEvents

        void DoSearch()
        {
            if (!Active || txtRegion.Text.Length < 2) return;
            lstRegions.Clear();
            client.Grid.RequestMapRegion(txtRegion.Text, GridLayerType.Terrain);

        }

        void DoTeleport()
        {
            if (InTeleport || !Active) return;

            Thread t = new Thread(new ThreadStart(delegate()
                {
                    if (!client.Self.Teleport(txtRegion.Text, new Vector3((int)nudX.Value, (int)nudY.Value, (int)nudZ.Value)))
                    {
                        Self_OnTeleport("", TeleportStatus.Failed, TeleportFlags.Default);
                    }
                    InTeleport = false;
                }
            ));
            t.IsBackground = true;
            lblStatus.Text = "Teleporting to " + txtRegion.Text;
            prgTeleport.Style = ProgressBarStyle.Marquee;
            t.Start();
        }

        #region JavascriptHooks
        void focusMap(int regX, int regY, int zoom)
        {
            if (!Visible) return;

            object[] parms = new object[3];
            parms[0] = regX;
            parms[1] = regY;
            parms[2] = zoom;

            map.Document.InvokeScript("focus", parms);
        }

        void gotoRegion(string regionName)
        {
            if (!Visible) return;

            object[] param = new object[1];
            param[0] = regionName;

            map.Document.InvokeScript("gotoRegion", param);
        }

        public void doNavigate(string region, string strx, string stry)
        {
            txtRegion.Text = region;
            nudX.Value = int.Parse(strx);
            nudY.Value = int.Parse(stry);
            nudZ.Value = 0;
            btnTeleport.Enabled = true;
            btnTeleport.Focus();
            lblStatus.Text = "Ready for " + region;
        }

        public void setStatus(string msg)
        {
            lblStatus.Text = msg;
            btnTeleport.Enabled = false;
        }

        #endregion

        #region GUIEvents

        private void frmMap_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void frmMap_VisibleChanged(object sender, EventArgs e)
        {
            if (Active)
            {
                pnlSearch.Visible = true;
            }
            else
            {
                pnlSearch.Visible = false;
            }

            if (Visible && Active)
            {
                gotoRegion(client.Network.CurrentSim.Name);
            }
        }

        private void txtRegion_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtRegion.Text = txtRegion.Text.Trim();
                e.SuppressKeyPress = true;
                DoSearch();
            }
        }

        private void lstRegions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstRegions.SelectedItems.Count != 1)
            {
                btnTeleport.Enabled = false;
                return;
            }
            btnTeleport.Enabled = true;
            txtRegion.Text = lstRegions.SelectedItems[0].Text;
            lblStatus.Text = "Ready for " + txtRegion.Text;
            gotoRegion(txtRegion.Text);
        }

        private void lstRegions_Enter(object sender, EventArgs e)
        {
            lstRegions_SelectedIndexChanged(sender, e);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            DoSearch();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void btnTeleport_Click(object sender, EventArgs e)
        {
            DoTeleport();
        }

        private void btnGoHome_Click(object sender, EventArgs e)
        {
            if (!Active || InTeleport) return;
            InTeleport = true;

            prgTeleport.Style = ProgressBarStyle.Marquee;
            lblStatus.Text = "Teleporting home...";

            Thread t = new Thread(new ThreadStart(delegate()
                {
                    client.Self.GoHome();
                    InTeleport = false;
                }
            ));
            t.IsBackground = true;
            t.Start();
        }
        #endregion GUIEvents

    }
}
