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
﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [ComVisibleAttribute(true)]
    public partial class MapConsole : UserControl
    {
        RadegastInstance instance;
        GridClient client { get { return instance.Client; } }
        bool Active { get { return client.Network.Connected; } }
        WebBrowser map;
        Regex slscheme = new Regex("^secondlife://(.+)/([0-9]+)/([0-9]+)");
        bool InTeleport = false;

        public MapConsole(RadegastInstance i)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmMap_Disposed);

            instance = i;
            try
            {
                map = new WebBrowser();
                map.Dock = DockStyle.Fill;
                map.AllowWebBrowserDrop = false;
                map.Navigate(Path.GetDirectoryName(Application.ExecutablePath) + @"/worldmap.html");
                map.WebBrowserShortcutsEnabled = false;
                map.ScriptErrorsSuppressed = true;
                map.ObjectForScripting = this;
                map.AllowNavigation = false;
                if (instance.MonoRuntime)
                {
                    map.Navigating += new WebBrowserNavigatingEventHandler(map_Navigating);
                }
                pnlMap.Controls.Add(map);
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, Helpers.LogLevel.Warning, client, e);
                pnlMap.Visible = false;
                map = null;
            }

            // Register callbacks
            client.Grid.OnGridRegion += new GridManager.GridRegionCallback(Grid_OnGridRegion);
            client.Self.OnTeleport += new AgentManager.TeleportCallback(Self_OnTeleport);
            client.Network.OnCurrentSimChanged += new NetworkManager.CurrentSimChangedCallback(Network_OnCurrentSimChanged);
        }

        void frmMap_Disposed(object sender, EventArgs e)
        {
            // Unregister callbacks
            client.Grid.OnGridRegion -= new GridManager.GridRegionCallback(Grid_OnGridRegion);
            client.Self.OnTeleport -= new AgentManager.TeleportCallback(Self_OnTeleport);
            client.Network.OnCurrentSimChanged -= new NetworkManager.CurrentSimChangedCallback(Network_OnCurrentSimChanged);

            if (map != null)
            {
                map.Dispose();
                map = null;
            }
        }

        #region PublicMethods
        public void GoHome()
        {
            btnGoHome_Click(this, EventArgs.Empty);
        }

        public void DoNavigate(string region, string x, string y)
        {
            DisplayLocation(region, int.Parse(x), int.Parse(y), 0);
        }

        public void DisplayLocation(string region, int x, int y, int z)
        {
            txtRegion.Text = region;
            nudX.Value = x;
            nudY.Value = y;
            nudZ.Value = z;
            gotoRegion(txtRegion.Text, x, y);
            btnTeleport.Enabled = true;
            btnTeleport.Focus();
            lblStatus.Text = "Ready for " + region;
        }

        public void SetStatus(string msg)
        {
            lblStatus.Text = msg;
            btnTeleport.Enabled = false;
        }

        #endregion

        #region NetworkEvents

        void Network_OnCurrentSimChanged(Simulator PreviousSimulator)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Network_OnCurrentSimChanged(PreviousSimulator);
                    }
                ));
                return;
            }

            gotoRegion(client.Network.CurrentSim.Name, (int)client.Self.SimPosition.X, (int)client.Self.SimPosition.Y);
            lblStatus.Text = "Now in " + client.Network.CurrentSim.Name;
        }

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
                    lblStatus.Text = "Teleport complete";
                    InTeleport = false;
                    Network_OnCurrentSimChanged(null);
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

        void Grid_OnGridRegion(GridRegion region)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
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

        void map_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            e.Cancel = true;
            Regex r = new Regex(@"^(http://slurl.com/secondlife/|secondlife://)([^/]+)/(\d+)/(\d+)(/(\d+))?");
            Match m = r.Match(e.Url.ToString());

            if (m.Groups.Count > 3)
            {
                txtRegion.Text = m.Groups[2].Value;
                nudX.Value = int.Parse(m.Groups[3].Value);
                nudY.Value = int.Parse(m.Groups[4].Value);
                nudZ.Value = 0;

                if (m.Groups.Count > 5 && m.Groups[6].Value != String.Empty)
                {
                    nudZ.Value = int.Parse(m.Groups[6].Value);
                }
                BeginInvoke(new MethodInvoker(DoTeleport));
            }
        }

        void DoSearch()
        {
            if (!Active || txtRegion.Text.Length < 2) return;
            lstRegions.Clear();
            client.Grid.RequestMapRegion(txtRegion.Text, GridLayerType.Terrain);

        }

        public void DoTeleport()
        {
            if (!Active) return;

            if (instance.MonoRuntime)
            {
                map.Navigate(Path.GetDirectoryName(Application.ExecutablePath) + @"/worldmap.html");
            }

            lblStatus.Text = "Teleporting to " + txtRegion.Text;
            prgTeleport.Style = ProgressBarStyle.Marquee;

            ThreadPool.QueueUserWorkItem((object state) =>
                {
                    if (!client.Self.Teleport(txtRegion.Text, new Vector3((int)nudX.Value, (int)nudY.Value, (int)nudZ.Value)))
                    {
                        Self_OnTeleport("", TeleportStatus.Failed, TeleportFlags.Default);
                    }
                    InTeleport = false;
                }
            );
        }

        #region JavascriptHooks
        void gotoRegion(string regionName, int simX, int simY)
        {
            if (!Visible || map == null || map.Document == null) return;
            if (instance.MonoRuntime)
            {
                map.Document.InvokeScript(string.Format("gReg = \"{0}\"; gSimX = {1}; gSimY = {2}; monosucks", regionName, simX, simY));
            }
            else
            {
                map.Document.InvokeScript("gotoRegion", new object[] { regionName, simX, simY });
            }
        }

        #endregion

        #region GUIEvents

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
            nudX.Value = 128;
            nudY.Value = 128;
            gotoRegion(txtRegion.Text, (int)nudX.Value, (int)nudY.Value);
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
            if (!Active) return;
            InTeleport = true;

            prgTeleport.Style = ProgressBarStyle.Marquee;
            lblStatus.Text = "Teleporting home...";

            client.Self.RequestTeleport(UUID.Zero);
        }

        private void btnMyPos_Click(object sender, EventArgs e)
        {
            gotoRegion(client.Network.CurrentSim.Name, (int)client.Self.SimPosition.X, (int)client.Self.SimPosition.Y);
        }

        private void btnDestination_Click(object sender, EventArgs e)
        {
            if (txtRegion.Text != string.Empty)
            {
                gotoRegion(txtRegion.Text, (int)nudX.Value, (int)nudY.Value);
            }
        }

        #endregion GUIEvents
    }
}
