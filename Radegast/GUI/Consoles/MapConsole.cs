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
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using System.Collections.Generic;
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
        MapControl mmap;
        Regex slscheme = new Regex("^secondlife://(.+)/([0-9]+)/([0-9]+)");
        bool InTeleport = false;
        bool mapCreated = false;
        Dictionary<string, ulong> regionHandles = new Dictionary<string, ulong>();

        public MapConsole(RadegastInstance inst)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmMap_Disposed);

            this.instance = inst;
            instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);

            Visible = false;
            VisibleChanged += new EventHandler(MapConsole_VisibleChanged);

            // Register callbacks
            RegisterClientEvents(client);
        }

        private void RegisterClientEvents(GridClient client)
        {
            client.Grid.GridRegion += new EventHandler<GridRegionEventArgs>(Grid_GridRegion);
            client.Self.TeleportProgress += new EventHandler<TeleportEventArgs>(Self_TeleportProgress);
            client.Network.SimChanged += new EventHandler<SimChangedEventArgs>(Network_SimChanged);
        }

        private void UnregisterClientEvents(GridClient client)
        {
            client.Grid.GridRegion -= new EventHandler<GridRegionEventArgs>(Grid_GridRegion);
            client.Self.TeleportProgress -= new EventHandler<TeleportEventArgs>(Self_TeleportProgress);
            client.Network.SimChanged -= new EventHandler<SimChangedEventArgs>(Network_SimChanged);
        }

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(client);
        }

        void createMap()
        {
            //if (instance.Netcom.Grid.ID == "secondlife")
            //{
            //    try
            //    {
            //        map = new WebBrowser();
            //        map.Dock = DockStyle.Fill;
            //        map.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(map_DocumentCompleted);
            //        map.Navigate(Path.GetDirectoryName(Application.ExecutablePath) + @"/worldmap.html");
            //        pnlMap.Controls.Add(map);
            //    }
            //    catch (Exception e)
            //    {
            //        Logger.Log(e.Message, Helpers.LogLevel.Warning, client, e);
            //        pnlMap.Visible = false;
            //        map = null;
            //    }
            //}

            if (map == null)
            {
                mmap = new MapControl(instance);
                mmap.MapTargetChanged += (object sender, MapTargetChangedEventArgs e) =>
                {
                    txtRegion.Text = e.Region.Name;
                    nudX.Value = e.LocalX;
                    nudY.Value = e.LocalY;
                    lblStatus.Text = "Ready for " + e.Region.Name;
                };

                mmap.ZoomChanged += new EventHandler<EventArgs>(mmap_ZoomChaged);

                if (instance.Netcom.Grid.ID == "secondlife")
                {
                    mmap.UseExternalTiles = true;
                }
                mmap.Dock = DockStyle.Fill;
                pnlMap.Controls.Add(mmap);
                mmap_ZoomChaged(null, null);
                zoomTracker.Visible = true;
            }

        }

        void mmap_ZoomChaged(object sender, EventArgs e)
        {
            int newval = (int)(100f * (mmap.Zoom - mmap.MinZoom) / (mmap.MaxZoom - mmap.MinZoom));
            if (newval >= zoomTracker.Minimum && newval <= zoomTracker.Maximum)
                zoomTracker.Value = newval;
        }

        void mmap_MapTargetChanged(object sender, MapTargetChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        void map_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            map.DocumentCompleted -= new WebBrowserDocumentCompletedEventHandler(map_DocumentCompleted);
            map.AllowWebBrowserDrop = false;
            map.WebBrowserShortcutsEnabled = false;
            map.ScriptErrorsSuppressed = true;
            map.ObjectForScripting = this;
            map.AllowNavigation = false;

            if (instance.MonoRuntime)
            {
                map.Navigating += new WebBrowserNavigatingEventHandler(map_Navigating);
            }

            ThreadPool.QueueUserWorkItem(sync =>
                {
                    Thread.Sleep(1000);
                    if (InvokeRequired && IsHandleCreated)
                        BeginInvoke(new MethodInvoker(() =>
                            {
                                if (savedRegion != null)
                                {
                                    gotoRegion(savedRegion, savedX, savedY);
                                }
                                else if (Active)
                                {
                                    gotoRegion(client.Network.CurrentSim.Name, 128, 128);
                                }
                            }
                    ));
                }
            );
        }

        void frmMap_Disposed(object sender, EventArgs e)
        {
            // Unregister callbacks
            UnregisterClientEvents(client);

            if (map != null)
            {
                if (instance.MonoRuntime)
                {
                    map.Navigating -= new WebBrowserNavigatingEventHandler(map_Navigating);
                }
                else
                {
                    map.Dispose();
                }
                map = null;
            }

            if (mmap != null)
            {
                mmap.Dispose();
                mmap = null;
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

        void Network_SimChanged(object sender, SimChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Network_SimChanged(sender, e)));
                return;
            }

            gotoRegion(client.Network.CurrentSim.Name, (int)client.Self.SimPosition.X, (int)client.Self.SimPosition.Y);
            lblStatus.Text = "Now in " + client.Network.CurrentSim.Name;
        }

        int lastTick = 0;

        void Self_TeleportProgress(object sender, TeleportEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Self_TeleportProgress(sender, e)));
                return;
            }

            switch (e.Status)
            {
                case TeleportStatus.Progress:
                    lblStatus.Text = "Progress: " + e.Message;
                    InTeleport = true;
                    break;

                case TeleportStatus.Start:
                    lblStatus.Text = "Teleporting to " + txtRegion.Text;
                    InTeleport = true;
                    break;

                case TeleportStatus.Cancelled:
                case TeleportStatus.Failed:
                    InTeleport = false;
                    lblStatus.Text = "Failed: " + e.Message;
                    if (Environment.TickCount - lastTick > 500)
                        instance.TabConsole.DisplayNotificationInChat("Teleport failed");
                    lastTick = Environment.TickCount;
                    break;

                case TeleportStatus.Finished:
                    lblStatus.Text = "Teleport complete";
                    if (Environment.TickCount - lastTick > 500)
                        instance.TabConsole.DisplayNotificationInChat("Teleport complete");
                    lastTick = Environment.TickCount;
                    InTeleport = false;
                    Network_SimChanged(null, null);
                    if (mmap != null)
                    {
                        mmap.ClearTarget();
                    }
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

        void Grid_GridRegion(object sender, GridRegionEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Grid_GridRegion(sender, e)));
                return;
            }

            if (e.Region.Name.ToLower().Contains(txtRegion.Text.ToLower())
                && !lstRegions.Items.ContainsKey(e.Region.Name))
            {
                ListViewItem item = new ListViewItem(e.Region.Name);
                item.Tag = e.Region;
                item.Name = e.Region.Name;
                lstRegions.Items.Add(item);
            }

            regionHandles[e.Region.Name] = Utils.UIntsToLong((uint)e.Region.X, (uint)e.Region.Y);

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
            client.Grid.RequestMapRegion(txtRegion.Text, GridLayerType.Objects);

        }

        public void DoTeleport()
        {
            if (!Active) return;

            if (instance.MonoRuntime && map != null)
            {
                map.Navigate(Path.GetDirectoryName(Application.ExecutablePath) + @"/worldmap.html");
            }

            lblStatus.Text = "Teleporting to " + txtRegion.Text;
            prgTeleport.Style = ProgressBarStyle.Marquee;

            ThreadPool.QueueUserWorkItem((object state) =>
                {
                    if (!client.Self.Teleport(txtRegion.Text, new Vector3((int)nudX.Value, (int)nudY.Value, (int)nudZ.Value)))
                    {
                        Self_TeleportProgress(this, new TeleportEventArgs(string.Empty, TeleportStatus.Failed, TeleportFlags.Default));
                    }
                    InTeleport = false;
                }
            );
        }

        #region JavascriptHooks
        string savedRegion = null;
        int savedX, savedY;

        void gotoRegion(string regionName, int simX, int simY)
        {
            savedRegion = regionName;
            savedX = simX;
            savedY = simY;

            if (!Visible) return;

            if (mmap != null)
            {
                if (!regionHandles.ContainsKey(regionName))
                {
                    ThreadPool.QueueUserWorkItem(sync =>
                        {
                            ManualResetEvent done = new ManualResetEvent(false);
                            EventHandler<GridRegionEventArgs> handler = (object sender, GridRegionEventArgs e) =>
                                {
                                    regionHandles[e.Region.Name] = Utils.UIntsToLong((uint)e.Region.X, (uint)e.Region.Y);
                                    if (e.Region.Name == regionName)
                                        done.Set();
                                };
                            client.Grid.GridRegion += handler;
                            client.Grid.RequestMapRegion(regionName, GridLayerType.Objects);
                            if (done.WaitOne(30 * 1000))
                            {
                                if (IsHandleCreated)
                                    BeginInvoke(new MethodInvoker(() => gotoRegion(regionName, simX, simY)));
                            }
                            client.Grid.GridRegion -= handler;
                        }
                    );
                    return;
                }
                mmap.CenterMap(regionHandles[regionName], (uint)simX, (uint)simY);
                return;
            }

            if (map == null || map.Document == null) return;

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

        private void MapConsole_VisibleChanged(object sender, EventArgs e)
        {
            if (!mapCreated && Visible)
            {
                createMap();
                mapCreated = true;
            }
            else if (Visible && instance.MonoRuntime && savedRegion != null)
            {
                gotoRegion(savedRegion, savedX, savedY);
            }
        }

        private void zoomTracker_Scroll(object sender, EventArgs e)
        {
            if (mmap != null)
            {
                mmap.Zoom = mmap.MinZoom + (mmap.MaxZoom - mmap.MinZoom) * (float)((float)zoomTracker.Value / 100f);
            }
        }

        #endregion GUIEvents
    }
}
