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
using System.Drawing;
using System.Windows.Forms;
using OpenMetaverse;
using Radegast.Netcom;

namespace Radegast
{
    public partial class frmTeleport : Form
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;
        bool IsTeleporting = false;
        GridRegion region;

        public frmTeleport(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmTeleport_Disposed);

            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;

            // Callbacks
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            client.Grid.OnGridRegion += new GridManager.GridRegionCallback(Grid_OnGridRegion);
            client.Self.OnTeleport += new AgentManager.TeleportCallback(Self_OnTeleport);

            SetDefaultValues();
        }

        void frmTeleport_Disposed(object sender, EventArgs e)
        {
            netcom.ClientDisconnected -= new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            client.Grid.OnGridRegion -= new GridManager.GridRegionCallback(Grid_OnGridRegion);
            client.Self.OnTeleport -= new AgentManager.TeleportCallback(Self_OnTeleport);
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

            statusLabel.Text = message;

            switch (status)
            {
                case TeleportStatus.Progress:
                    IsTeleporting = true;
                    break;
                case TeleportStatus.Cancelled:
                case TeleportStatus.Failed:
                    statusLabel.Text = "Teleport failed: " + message;
                    IsTeleporting = false;
                    break;
                case TeleportStatus.Finished:
                    IsTeleporting = false;
                    Close();
                    break;
            }
            RefreshControls();
        }

        private void RemoveClientEvents()
        {
            client.Grid.OnGridRegion -= new GridManager.GridRegionCallback(Grid_OnGridRegion);
        }

        //Separate thread
        private void Grid_OnGridRegion(GridRegion region)
        {
            BeginInvoke(new GridManager.GridRegionCallback(RegionSearchResult), new object[] { region });
        }

        //UI thread
        private void RegionSearchResult(GridRegion region)
        {
            RegionSearchResultItem item = new RegionSearchResultItem(instance, region, lbxRegionSearch);
            int index = lbxRegionSearch.Items.Add(item);
            item.ListIndex = index;
            if (lbxRegionSearch.Items.Count == 1)
            {
                lbxRegionSearch.SelectedItem = item;
                lbxRegionSearch_DoubleClick(this, new EventArgs());
            }
        }

        private void SetDefaultValues()
        {
            string region = client.Network.CurrentSim.Name;
            decimal x = (decimal)client.Self.SimPosition.X;
            decimal y = (decimal)client.Self.SimPosition.Y;
            decimal z = (decimal)client.Self.SimPosition.Z;

            if (x < 0) x = 0;
            if (x > 256) x = 256;
            if (y < 0) y = 0;
            if (y > 256) y = 256;

            txtRegion.Text = region;
            nudX.Value = x;
            nudY.Value = y;
            nudZ.Value = z;
        }

        private void netcom_TeleportStatusChanged(object sender, TeleportStatusEventArgs e)
        {
            switch (e.Status)
            {
                case TeleportStatus.Start:
                case TeleportStatus.Progress:
                    lblTeleportStatus.Text = e.Message;
                    break;

                case TeleportStatus.Failed:
                    RefreshControls();

                    MessageBox.Show(e.Message, "Teleport", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;

                case TeleportStatus.Finished:
                    this.Close();
                    break;
            }
        }

        private void netcom_ClientDisconnected(object sender, ClientDisconnectEventArgs e)
        {
            this.Close();
        }

        private void netcom_Teleporting(object sender, TeleportingEventArgs e)
        {
            RefreshControls();
        }

        private void RefreshControls()
        {
            if (IsTeleporting)
            {
                pnlTeleportOptions.Enabled = false;
                btnTeleport.Enabled = false;
                pnlTeleporting.Visible = true;
            }
            else
            {
                pnlTeleportOptions.Enabled = true;
                btnTeleport.Enabled = true;
                pnlTeleporting.Visible = false;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTeleport_Click(object sender, EventArgs e)
        {
            IsTeleporting = true;
            RefreshControls();
            client.Self.RequestTeleport(region.RegionHandle, new Vector3((float)nudX.Value, (float)nudY.Value, (float)nudZ.Value));
        }

        private void frmTeleport_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsTeleporting && netcom.IsLoggedIn)
                e.Cancel = true;
            else
                RemoveClientEvents();
        }

         private void lbxRegionSearch_DoubleClick(object sender, EventArgs e)
        {
            if (lbxRegionSearch.SelectedItem == null) return;
            RegionSearchResultItem item = (RegionSearchResultItem)lbxRegionSearch.SelectedItem;

            region = item.region;

            txtRegion.Text = item.Region.Name;
            nudX.Value = 128;
            nudY.Value = 128;
            nudZ.Value = 0;

            RefreshControls();
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            if (btnFind.Text.Trim().Length > 0)
            {
                StartRegionSearch();
            }
        }

        private void StartRegionSearch()
        {
            lbxRegionSearch.Items.Clear();

            client.Grid.RequestMapRegion(txtSearchFor.Text, GridLayerType.Terrain);
            
        }

        private void lbxRegionSearch_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            if (e.Index < 0) return;

            RegionSearchResultItem itemToDraw = (RegionSearchResultItem)lbxRegionSearch.Items[e.Index];
            Brush textBrush = null;

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                textBrush = new SolidBrush(Color.FromKnownColor(KnownColor.HighlightText));
            else
                textBrush = new SolidBrush(Color.FromKnownColor(KnownColor.ControlText));
            
            Font newFont = new Font(e.Font, FontStyle.Bold);
            SizeF stringSize = e.Graphics.MeasureString(itemToDraw.Region.Name, newFont);
            
            float iconSize = (float)trkIconSize.Value;
            float leftTextMargin = e.Bounds.Left + iconSize + 6.0f;
            float topTextMargin = e.Bounds.Top + 4.0f;

            if (itemToDraw.IsImageDownloaded)
            {
                if (itemToDraw.MapImage != null)
                    e.Graphics.DrawImage(itemToDraw.MapImage, new RectangleF(e.Bounds.Left + 4.0f, e.Bounds.Top + 4.0f, iconSize, iconSize));
            }
            else
            {
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(200, 200, 200)), e.Bounds.Left + 4.0f, e.Bounds.Top + 4.0f, iconSize, iconSize);

                if (!itemToDraw.IsImageDownloading)
                    itemToDraw.RequestMapImage(125000.0f);
            }
            
            e.Graphics.DrawString(itemToDraw.Region.Name, newFont, textBrush, new PointF(leftTextMargin, topTextMargin));

            if (itemToDraw.GotAgentCount)
            {
                e.Graphics.DrawString(itemToDraw.Region.Agents.ToString() + " people", e.Font, textBrush, new PointF(leftTextMargin + stringSize.Width + 6.0f, topTextMargin));
            }
            else
            {
                if (!itemToDraw.GettingAgentCount)
                    itemToDraw.RequestAgentLocations();
            }

            switch (itemToDraw.Region.Access)
            {
                case SimAccess.PG:
                    e.Graphics.DrawString("PG", e.Font, textBrush, new PointF(leftTextMargin, topTextMargin + stringSize.Height));
                    break;

                case SimAccess.Mature:
                    e.Graphics.DrawString("Mature", e.Font, textBrush, new PointF(leftTextMargin, topTextMargin + stringSize.Height));
                    break;

                case SimAccess.Down:
                    e.Graphics.DrawString("Offline", e.Font, new SolidBrush(Color.Red), new PointF(leftTextMargin, topTextMargin + stringSize.Height));
                    break;
            }

            e.Graphics.DrawLine(new Pen(Color.FromArgb(200, 200, 200)), new Point(e.Bounds.Left, e.Bounds.Bottom - 1), new Point(e.Bounds.Right, e.Bounds.Bottom - 1));
            e.DrawFocusRectangle();

            textBrush.Dispose();
            newFont.Dispose();
            textBrush = null;
            newFont = null;
        }

        private void trkIconSize_Scroll(object sender, EventArgs e)
        {
            lbxRegionSearch.ItemHeight = trkIconSize.Value + 10;
        }

        private void txtSearchFor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                if (btnFind.Text.Trim().Length > 0)
                {
                    StartRegionSearch();
                }
            }
        }
    }
}