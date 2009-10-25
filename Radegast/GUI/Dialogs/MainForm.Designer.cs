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
namespace Radegast
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (InvokeRequired)
            {
                Invoke(new System.Windows.Forms.MethodInvoker(() => { Dispose(disposing); }));
                return;
            }

            if (disposing)
            {
                if (components != null)
                    components.Dispose();

                if (statusTimer != null)
                    statusTimer.Dispose();
            }


            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbtnSLeek = new System.Windows.Forms.ToolStripDropDownButton();
            this.newWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmnuImport = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmnuPrefs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tmnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnWorld = new System.Windows.Forms.ToolStripDropDownButton();
            this.tmnuControlFly = new System.Windows.Forms.ToolStripMenuItem();
            this.tmnuControlAlwaysRun = new System.Windows.Forms.ToolStripMenuItem();
            this.groundSitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.standToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.tmnuCreateLandmark = new System.Windows.Forms.ToolStripMenuItem();
            this.tmnuSetHome = new System.Windows.Forms.ToolStripMenuItem();
            this.tmnuTeleportHome = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.tmnuStatusAway = new System.Windows.Forms.ToolStripMenuItem();
            this.tmnuStatusBusy = new System.Windows.Forms.ToolStripMenuItem();
            this.tbnTools = new System.Windows.Forms.ToolStripDropDownButton();
            this.locationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.homeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autopilotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanCacheToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setAppearanceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebakeTexturesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbnPlugins = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnFriends = new System.Windows.Forms.ToolStripButton();
            this.tbtnGroups = new System.Windows.Forms.ToolStripButton();
            this.tbtnInventory = new System.Windows.Forms.ToolStripButton();
            this.tbtnSearch = new System.Windows.Forms.ToolStripButton();
            this.tbtnMap = new System.Windows.Forms.ToolStripButton();
            this.tbnObjects = new System.Windows.Forms.ToolStripButton();
            this.lblTime = new System.Windows.Forms.ToolStripLabel();
            this.tbtnMedia = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnHelp = new System.Windows.Forms.ToolStripDropDownButton();
            this.reportBugsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.checkForUpdatesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutRadegastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tlblLoginName = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlblMoneyBalance = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlblRegionInfo = new System.Windows.Forms.ToolStripStatusLabel();
            this.icoHealth = new System.Windows.Forms.ToolStripStatusLabel();
            this.icoNoFly = new System.Windows.Forms.ToolStripStatusLabel();
            this.icoNoBuild = new System.Windows.Forms.ToolStripStatusLabel();
            this.icoNoScript = new System.Windows.Forms.ToolStripStatusLabel();
            this.icoNoPush = new System.Windows.Forms.ToolStripStatusLabel();
            this.icoNoVoice = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlblParcel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.pnlDialog = new System.Windows.Forms.Panel();
            this.timerWorldClock = new System.Windows.Forms.Timer(this.components);
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnSLeek,
            this.tbtnWorld,
            this.tbnTools,
            this.tbnPlugins,
            this.toolStripSeparator1,
            this.tbtnFriends,
            this.tbtnGroups,
            this.tbtnInventory,
            this.tbtnSearch,
            this.tbtnMap,
            this.tbnObjects,
            this.lblTime,
            this.tbtnMedia,
            this.toolStripSeparator2,
            this.tbtnHelp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(738, 25);
            this.toolStrip1.Stretch = true;
            this.toolStrip1.TabIndex = 8;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbtnSLeek
            // 
            this.tbtnSLeek.AutoToolTip = false;
            this.tbtnSLeek.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnSLeek.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newWindowToolStripMenuItem,
            this.tmnuImport,
            this.scriptEditorToolStripMenuItem,
            this.toolStripMenuItem3,
            this.disconnectToolStripMenuItem,
            this.reconnectToolStripMenuItem,
            this.tmnuPrefs,
            this.toolStripMenuItem2,
            this.tmnuExit});
            this.tbtnSLeek.Image = global::Radegast.Properties.Resources.computer_16;
            this.tbtnSLeek.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnSLeek.Name = "tbtnSLeek";
            this.tbtnSLeek.Size = new System.Drawing.Size(38, 22);
            this.tbtnSLeek.Text = "&File";
            // 
            // newWindowToolStripMenuItem
            // 
            this.newWindowToolStripMenuItem.Name = "newWindowToolStripMenuItem";
            this.newWindowToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.newWindowToolStripMenuItem.Text = "&New window...";
            this.newWindowToolStripMenuItem.Click += new System.EventHandler(this.newWindowToolStripMenuItem_Click);
            // 
            // tmnuImport
            // 
            this.tmnuImport.Enabled = false;
            this.tmnuImport.Name = "tmnuImport";
            this.tmnuImport.Size = new System.Drawing.Size(155, 22);
            this.tmnuImport.Text = "Import object...";
            this.tmnuImport.Click += new System.EventHandler(this.importObjectToolStripMenuItem_Click);
            // 
            // scriptEditorToolStripMenuItem
            // 
            this.scriptEditorToolStripMenuItem.Name = "scriptEditorToolStripMenuItem";
            this.scriptEditorToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.scriptEditorToolStripMenuItem.Text = "Script editor...";
            this.scriptEditorToolStripMenuItem.Click += new System.EventHandler(this.scriptEditorToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(152, 6);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Enabled = false;
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // reconnectToolStripMenuItem
            // 
            this.reconnectToolStripMenuItem.Enabled = false;
            this.reconnectToolStripMenuItem.Name = "reconnectToolStripMenuItem";
            this.reconnectToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.reconnectToolStripMenuItem.Text = "Reconnect";
            this.reconnectToolStripMenuItem.Click += new System.EventHandler(this.reconnectToolStripMenuItem_Click);
            // 
            // tmnuPrefs
            // 
            this.tmnuPrefs.Name = "tmnuPrefs";
            this.tmnuPrefs.Size = new System.Drawing.Size(155, 22);
            this.tmnuPrefs.Text = "Preferences...";
            this.tmnuPrefs.Click += new System.EventHandler(this.tmnuPrefs_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 6);
            // 
            // tmnuExit
            // 
            this.tmnuExit.Name = "tmnuExit";
            this.tmnuExit.Size = new System.Drawing.Size(155, 22);
            this.tmnuExit.Text = "E&xit";
            this.tmnuExit.Click += new System.EventHandler(this.tmnuExit_Click);
            // 
            // tbtnWorld
            // 
            this.tbtnWorld.AutoToolTip = false;
            this.tbtnWorld.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnWorld.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmnuControlFly,
            this.tmnuControlAlwaysRun,
            this.groundSitToolStripMenuItem,
            this.standToolStripMenuItem,
            this.toolStripMenuItem4,
            this.tmnuCreateLandmark,
            this.tmnuSetHome,
            this.tmnuTeleportHome,
            this.toolStripMenuItem5,
            this.tmnuStatusAway,
            this.tmnuStatusBusy});
            this.tbtnWorld.Enabled = false;
            this.tbtnWorld.Image = ((System.Drawing.Image)(resources.GetObject("tbtnWorld.Image")));
            this.tbtnWorld.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnWorld.Name = "tbtnWorld";
            this.tbtnWorld.Size = new System.Drawing.Size(52, 22);
            this.tbtnWorld.Text = "&World";
            // 
            // tmnuControlFly
            // 
            this.tmnuControlFly.CheckOnClick = true;
            this.tmnuControlFly.Name = "tmnuControlFly";
            this.tmnuControlFly.Size = new System.Drawing.Size(223, 22);
            this.tmnuControlFly.Text = "Fly";
            this.tmnuControlFly.Click += new System.EventHandler(this.tmnuControlFly_Click);
            // 
            // tmnuControlAlwaysRun
            // 
            this.tmnuControlAlwaysRun.CheckOnClick = true;
            this.tmnuControlAlwaysRun.Name = "tmnuControlAlwaysRun";
            this.tmnuControlAlwaysRun.Size = new System.Drawing.Size(223, 22);
            this.tmnuControlAlwaysRun.Text = "Always Run";
            this.tmnuControlAlwaysRun.Click += new System.EventHandler(this.tmnuControlAlwaysRun_Click);
            // 
            // groundSitToolStripMenuItem
            // 
            this.groundSitToolStripMenuItem.Name = "groundSitToolStripMenuItem";
            this.groundSitToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.groundSitToolStripMenuItem.Text = "Sit On Ground";
            this.groundSitToolStripMenuItem.Click += new System.EventHandler(this.groundSitToolStripMenuItem_Click);
            // 
            // standToolStripMenuItem
            // 
            this.standToolStripMenuItem.Name = "standToolStripMenuItem";
            this.standToolStripMenuItem.Size = new System.Drawing.Size(223, 22);
            this.standToolStripMenuItem.Text = "Stand";
            this.standToolStripMenuItem.Click += new System.EventHandler(this.standToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(220, 6);
            // 
            // tmnuCreateLandmark
            // 
            this.tmnuCreateLandmark.Name = "tmnuCreateLandmark";
            this.tmnuCreateLandmark.Size = new System.Drawing.Size(223, 22);
            this.tmnuCreateLandmark.Text = "Create Landmark Here";
            this.tmnuCreateLandmark.Click += new System.EventHandler(this.tmnuCreateLandmark_Click);
            // 
            // tmnuSetHome
            // 
            this.tmnuSetHome.Name = "tmnuSetHome";
            this.tmnuSetHome.Size = new System.Drawing.Size(223, 22);
            this.tmnuSetHome.Text = "Set Home To Here";
            this.tmnuSetHome.Click += new System.EventHandler(this.tmnuSetHome_Click);
            // 
            // tmnuTeleportHome
            // 
            this.tmnuTeleportHome.Name = "tmnuTeleportHome";
            this.tmnuTeleportHome.ShortcutKeyDisplayString = "Ctrl-Shift-H";
            this.tmnuTeleportHome.Size = new System.Drawing.Size(223, 22);
            this.tmnuTeleportHome.Text = "Teleport Home";
            this.tmnuTeleportHome.Click += new System.EventHandler(this.tmnuTeleportHome_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(220, 6);
            // 
            // tmnuStatusAway
            // 
            this.tmnuStatusAway.CheckOnClick = true;
            this.tmnuStatusAway.Name = "tmnuStatusAway";
            this.tmnuStatusAway.Size = new System.Drawing.Size(223, 22);
            this.tmnuStatusAway.Text = "Away";
            this.tmnuStatusAway.Click += new System.EventHandler(this.tmnuStatusAway_Click);
            // 
            // tmnuStatusBusy
            // 
            this.tmnuStatusBusy.CheckOnClick = true;
            this.tmnuStatusBusy.Name = "tmnuStatusBusy";
            this.tmnuStatusBusy.Size = new System.Drawing.Size(223, 22);
            this.tmnuStatusBusy.Text = "Busy";
            this.tmnuStatusBusy.Click += new System.EventHandler(this.tmnuStatusBusy_Click);
            // 
            // tbnTools
            // 
            this.tbnTools.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbnTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.locationToolStripMenuItem,
            this.homeToolStripMenuItem,
            this.autopilotToolStripMenuItem,
            this.cleanCacheToolStripMenuItem,
            this.setAppearanceToolStripMenuItem,
            this.rebakeTexturesToolStripMenuItem,
            this.toolStripMenuItem1});
            this.tbnTools.Enabled = false;
            this.tbnTools.Image = ((System.Drawing.Image)(resources.GetObject("tbnTools.Image")));
            this.tbnTools.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbnTools.Name = "tbnTools";
            this.tbnTools.Size = new System.Drawing.Size(49, 22);
            this.tbnTools.Text = "&Tools";
            // 
            // locationToolStripMenuItem
            // 
            this.locationToolStripMenuItem.Name = "locationToolStripMenuItem";
            this.locationToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.locationToolStripMenuItem.Text = "Teleport sim";
            this.locationToolStripMenuItem.Click += new System.EventHandler(this.tbtnTeleport_Click);
            // 
            // homeToolStripMenuItem
            // 
            this.homeToolStripMenuItem.Name = "homeToolStripMenuItem";
            this.homeToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.homeToolStripMenuItem.Text = "Teleport home";
            this.homeToolStripMenuItem.Click += new System.EventHandler(this.tmnuTeleportHome_Click);
            // 
            // autopilotToolStripMenuItem
            // 
            this.autopilotToolStripMenuItem.Enabled = false;
            this.autopilotToolStripMenuItem.Name = "autopilotToolStripMenuItem";
            this.autopilotToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.autopilotToolStripMenuItem.Text = "&Autopilot";
            this.autopilotToolStripMenuItem.Click += new System.EventHandler(this.autopilotToolStripMenuItem_Click);
            // 
            // cleanCacheToolStripMenuItem
            // 
            this.cleanCacheToolStripMenuItem.Name = "cleanCacheToolStripMenuItem";
            this.cleanCacheToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.cleanCacheToolStripMenuItem.Text = "Clean cache";
            this.cleanCacheToolStripMenuItem.Click += new System.EventHandler(this.cleanCacheToolStripMenuItem_Click);
            // 
            // setAppearanceToolStripMenuItem
            // 
            this.setAppearanceToolStripMenuItem.Name = "setAppearanceToolStripMenuItem";
            this.setAppearanceToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.setAppearanceToolStripMenuItem.Text = "Set appearance";
            this.setAppearanceToolStripMenuItem.Click += new System.EventHandler(this.tbtnAppearance_Click);
            // 
            // rebakeTexturesToolStripMenuItem
            // 
            this.rebakeTexturesToolStripMenuItem.Name = "rebakeTexturesToolStripMenuItem";
            this.rebakeTexturesToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.rebakeTexturesToolStripMenuItem.Text = "Rebake textures";
            this.rebakeTexturesToolStripMenuItem.Click += new System.EventHandler(this.rebakeTexturesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(153, 6);
            // 
            // tbnPlugins
            // 
            this.tbnPlugins.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbnPlugins.Image = ((System.Drawing.Image)(resources.GetObject("tbnPlugins.Image")));
            this.tbnPlugins.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbnPlugins.Name = "tbnPlugins";
            this.tbnPlugins.Size = new System.Drawing.Size(59, 22);
            this.tbnPlugins.Text = "&Plugins";
            this.tbnPlugins.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbtnFriends
            // 
            this.tbtnFriends.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnFriends.Enabled = false;
            this.tbtnFriends.Image = ((System.Drawing.Image)(resources.GetObject("tbtnFriends.Image")));
            this.tbtnFriends.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnFriends.Name = "tbtnFriends";
            this.tbtnFriends.Size = new System.Drawing.Size(49, 22);
            this.tbtnFriends.Text = "Friends";
            this.tbtnFriends.ToolTipText = "Friends (Alt-2)";
            this.tbtnFriends.Click += new System.EventHandler(this.tbtnFriends_Click);
            // 
            // tbtnGroups
            // 
            this.tbtnGroups.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnGroups.Enabled = false;
            this.tbtnGroups.Image = ((System.Drawing.Image)(resources.GetObject("tbtnGroups.Image")));
            this.tbtnGroups.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnGroups.Name = "tbtnGroups";
            this.tbtnGroups.Size = new System.Drawing.Size(49, 22);
            this.tbtnGroups.Text = "Groups";
            this.tbtnGroups.ToolTipText = "Groups (Alt-3)";
            this.tbtnGroups.Click += new System.EventHandler(this.tbtnGroups_Click);
            // 
            // tbtnInventory
            // 
            this.tbtnInventory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnInventory.Enabled = false;
            this.tbtnInventory.Image = ((System.Drawing.Image)(resources.GetObject("tbtnInventory.Image")));
            this.tbtnInventory.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnInventory.Name = "tbtnInventory";
            this.tbtnInventory.Size = new System.Drawing.Size(61, 22);
            this.tbtnInventory.Text = "Inventory";
            this.tbtnInventory.ToolTipText = "Inventory (Alt-4)";
            this.tbtnInventory.Click += new System.EventHandler(this.tbtnInventory_Click);
            // 
            // tbtnSearch
            // 
            this.tbtnSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnSearch.Enabled = false;
            this.tbtnSearch.Image = ((System.Drawing.Image)(resources.GetObject("tbtnSearch.Image")));
            this.tbtnSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnSearch.Name = "tbtnSearch";
            this.tbtnSearch.Size = new System.Drawing.Size(46, 22);
            this.tbtnSearch.Text = "Search";
            this.tbtnSearch.ToolTipText = "Search (Alt-5)";
            this.tbtnSearch.Click += new System.EventHandler(this.tbtnSearch_Click);
            // 
            // tbtnMap
            // 
            this.tbtnMap.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnMap.Enabled = false;
            this.tbtnMap.Image = ((System.Drawing.Image)(resources.GetObject("tbtnMap.Image")));
            this.tbtnMap.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnMap.Name = "tbtnMap";
            this.tbtnMap.Size = new System.Drawing.Size(35, 22);
            this.tbtnMap.Text = "Map";
            this.tbtnMap.ToolTipText = "Map (Alt-6)";
            this.tbtnMap.Click += new System.EventHandler(this.tbtnMap_Click);
            // 
            // tbnObjects
            // 
            this.tbnObjects.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbnObjects.Enabled = false;
            this.tbnObjects.Image = ((System.Drawing.Image)(resources.GetObject("tbnObjects.Image")));
            this.tbnObjects.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbnObjects.Name = "tbnObjects";
            this.tbnObjects.Size = new System.Drawing.Size(51, 22);
            this.tbnObjects.Text = "Objects";
            this.tbnObjects.ToolTipText = "Displays a list of nearby objects that you can perform various operations on (Alt" +
                "-7)";
            this.tbnObjects.Click += new System.EventHandler(this.tbnObjects_Click);
            // 
            // lblTime
            // 
            this.lblTime.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(30, 22);
            this.lblTime.Text = "--:--";
            // 
            // tbtnMedia
            // 
            this.tbtnMedia.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnMedia.Image = ((System.Drawing.Image)(resources.GetObject("tbtnMedia.Image")));
            this.tbtnMedia.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnMedia.Name = "tbtnMedia";
            this.tbtnMedia.Size = new System.Drawing.Size(44, 22);
            this.tbtnMedia.Text = "Media";
            this.tbtnMedia.ToolTipText = "Media (Alt-8)";
            this.tbtnMedia.Visible = false;
            this.tbtnMedia.Click += new System.EventHandler(this.tbtnMedia_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tbtnHelp
            // 
            this.tbtnHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reportBugsToolStripMenuItem,
            this.checkForUpdatesToolStripMenuItem,
            this.toolStripMenuItem6,
            this.aboutRadegastToolStripMenuItem});
            this.tbtnHelp.Image = ((System.Drawing.Image)(resources.GetObject("tbtnHelp.Image")));
            this.tbtnHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnHelp.Name = "tbtnHelp";
            this.tbtnHelp.Size = new System.Drawing.Size(45, 22);
            this.tbtnHelp.Text = "&Help";
            // 
            // reportBugsToolStripMenuItem
            // 
            this.reportBugsToolStripMenuItem.Name = "reportBugsToolStripMenuItem";
            this.reportBugsToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.reportBugsToolStripMenuItem.Text = "Bugs/Feature Requests...";
            this.reportBugsToolStripMenuItem.Click += new System.EventHandler(this.reportBugsToolStripMenuItem_Click);
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            this.checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            this.checkForUpdatesToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.checkForUpdatesToolStripMenuItem.Text = "Check for Updates...";
            this.checkForUpdatesToolStripMenuItem.Click += new System.EventHandler(this.checkForUpdatesToolStripMenuItem_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(200, 6);
            // 
            // aboutRadegastToolStripMenuItem
            // 
            this.aboutRadegastToolStripMenuItem.Name = "aboutRadegastToolStripMenuItem";
            this.aboutRadegastToolStripMenuItem.Size = new System.Drawing.Size(203, 22);
            this.aboutRadegastToolStripMenuItem.Text = "About Radegast...";
            this.aboutRadegastToolStripMenuItem.Click += new System.EventHandler(this.aboutRadegastToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlblLoginName,
            this.tlblMoneyBalance,
            this.tlblRegionInfo,
            this.icoHealth,
            this.icoNoFly,
            this.icoNoBuild,
            this.icoNoScript,
            this.icoNoPush,
            this.icoNoVoice,
            this.tlblParcel});
            this.statusStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.statusStrip1.Location = new System.Drawing.Point(0, 480);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.ShowItemToolTips = true;
            this.statusStrip1.Size = new System.Drawing.Size(738, 25);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tlblLoginName
            // 
            this.tlblLoginName.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tlblLoginName.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tlblLoginName.Name = "tlblLoginName";
            this.tlblLoginName.Size = new System.Drawing.Size(47, 19);
            this.tlblLoginName.Text = "Offline";
            // 
            // tlblMoneyBalance
            // 
            this.tlblMoneyBalance.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tlblMoneyBalance.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tlblMoneyBalance.Image = global::Radegast.Properties.Resources.status_buy_currency;
            this.tlblMoneyBalance.Name = "tlblMoneyBalance";
            this.tlblMoneyBalance.Size = new System.Drawing.Size(33, 20);
            this.tlblMoneyBalance.Text = "0";
            // 
            // tlblRegionInfo
            // 
            this.tlblRegionInfo.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
            this.tlblRegionInfo.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tlblRegionInfo.Name = "tlblRegionInfo";
            this.tlblRegionInfo.Size = new System.Drawing.Size(67, 19);
            this.tlblRegionInfo.Text = "No Region";
            this.tlblRegionInfo.Click += new System.EventHandler(this.tlblRegionInfo_Click);
            // 
            // icoHealth
            // 
            this.icoHealth.Image = global::Radegast.Properties.Resources.status_health;
            this.icoHealth.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.icoHealth.Name = "icoHealth";
            this.icoHealth.Size = new System.Drawing.Size(67, 16);
            this.icoHealth.Text = "100%";
            this.icoHealth.ToolTipText = "Damage enabled on the parcel";
            this.icoHealth.Visible = false;
            // 
            // icoNoFly
            // 
            this.icoNoFly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.icoNoFly.Image = global::Radegast.Properties.Resources.status_no_fly;
            this.icoNoFly.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.icoNoFly.Name = "icoNoFly";
            this.icoNoFly.Size = new System.Drawing.Size(32, 16);
            this.icoNoFly.Text = "Fly";
            this.icoNoFly.ToolTipText = "Flying not allowed here";
            this.icoNoFly.Visible = false;
            // 
            // icoNoBuild
            // 
            this.icoNoBuild.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.icoNoBuild.Image = global::Radegast.Properties.Resources.status_no_build;
            this.icoNoBuild.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.icoNoBuild.Name = "icoNoBuild";
            this.icoNoBuild.Size = new System.Drawing.Size(32, 16);
            this.icoNoBuild.Text = "Build";
            this.icoNoBuild.ToolTipText = "No building or rezzing objects allowed on this parcel";
            this.icoNoBuild.Visible = false;
            // 
            // icoNoScript
            // 
            this.icoNoScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.icoNoScript.Image = global::Radegast.Properties.Resources.status_no_scripts;
            this.icoNoScript.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.icoNoScript.Name = "icoNoScript";
            this.icoNoScript.Size = new System.Drawing.Size(32, 16);
            this.icoNoScript.Text = "Script";
            this.icoNoScript.ToolTipText = "Scripts disallowd on the parcel";
            this.icoNoScript.Visible = false;
            // 
            // icoNoPush
            // 
            this.icoNoPush.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.icoNoPush.Image = global::Radegast.Properties.Resources.status_no_push;
            this.icoNoPush.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.icoNoPush.Name = "icoNoPush";
            this.icoNoPush.Size = new System.Drawing.Size(32, 16);
            this.icoNoPush.Text = "Push";
            this.icoNoPush.ToolTipText = "No pushing by scripts allowed";
            this.icoNoPush.Visible = false;
            // 
            // icoNoVoice
            // 
            this.icoNoVoice.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.icoNoVoice.Image = global::Radegast.Properties.Resources.status_no_voice;
            this.icoNoVoice.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.icoNoVoice.Name = "icoNoVoice";
            this.icoNoVoice.Size = new System.Drawing.Size(32, 16);
            this.icoNoVoice.Text = "Push";
            this.icoNoVoice.ToolTipText = "Voice chat disabled";
            this.icoNoVoice.Visible = false;
            // 
            // tlblParcel
            // 
            this.tlblParcel.AutoToolTip = true;
            this.tlblParcel.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tlblParcel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tlblParcel.Name = "tlblParcel";
            this.tlblParcel.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.tlblParcel.Size = new System.Drawing.Size(58, 19);
            this.tlblParcel.Text = "No Parcel";
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(738, 455);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(738, 480);
            this.toolStripContainer1.TabIndex = 10;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // pnlDialog
            // 
            this.pnlDialog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlDialog.AutoSize = true;
            this.pnlDialog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(170)))), ((int)(((byte)(255)))));
            this.pnlDialog.Location = new System.Drawing.Point(540, 50);
            this.pnlDialog.Name = "pnlDialog";
            this.pnlDialog.Size = new System.Drawing.Size(198, 151);
            this.pnlDialog.TabIndex = 11;
            // 
            // timerWorldClock
            // 
            this.timerWorldClock.Enabled = true;
            this.timerWorldClock.Interval = 1000;
            this.timerWorldClock.Tick += new System.EventHandler(this.timerWorldClock_Tick);
            // 
            // frmMain
            // 
            this.AutoSavePosition = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(738, 505);
            this.Controls.Add(this.pnlDialog);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.statusStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(754, 541);
            this.Name = "frmMain";
            this.Text = "Radegast";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyUp);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ToolStrip toolStrip1;
        public System.Windows.Forms.ToolStripContainer toolStripContainer1;
        public System.Windows.Forms.Panel pnlDialog;
        public System.Windows.Forms.ToolStripDropDownButton tbtnSLeek;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        public System.Windows.Forms.ToolStripMenuItem tmnuExit;
        public System.Windows.Forms.StatusStrip statusStrip1;
        public System.Windows.Forms.ToolStripStatusLabel tlblLoginName;
        public System.Windows.Forms.ToolStripStatusLabel tlblRegionInfo;
        public System.Windows.Forms.ToolStripStatusLabel tlblMoneyBalance;
        public System.Windows.Forms.ToolStripMenuItem tmnuPrefs;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        public System.Windows.Forms.ToolStripDropDownButton tbtnWorld;
        public System.Windows.Forms.ToolStripMenuItem tmnuControlFly;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripMenuItem tmnuControlAlwaysRun;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        public System.Windows.Forms.ToolStripStatusLabel tlblParcel;
        public System.Windows.Forms.ToolStripDropDownButton tbnTools;
        public System.Windows.Forms.ToolStripMenuItem locationToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem homeToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem tmnuImport;
        public System.Windows.Forms.ToolStripMenuItem autopilotToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem cleanCacheToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem setAppearanceToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem rebakeTexturesToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem standToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem groundSitToolStripMenuItem;
        public System.Windows.Forms.ToolStripButton tbnObjects;
        public System.Windows.Forms.ToolStripMenuItem newWindowToolStripMenuItem;
        public System.Windows.Forms.ToolStripStatusLabel icoNoBuild;
        public System.Windows.Forms.ToolStripStatusLabel icoHealth;
        public System.Windows.Forms.ToolStripStatusLabel icoNoFly;
        public System.Windows.Forms.ToolStripStatusLabel icoNoScript;
        public System.Windows.Forms.ToolStripStatusLabel icoNoPush;
        public System.Windows.Forms.ToolStripStatusLabel icoNoVoice;
        public System.Windows.Forms.ToolStripButton tbtnGroups;
        public System.Windows.Forms.ToolStripMenuItem scriptEditorToolStripMenuItem;
        public System.Windows.Forms.ToolStripDropDownButton tbnPlugins;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        public System.Windows.Forms.ToolStripMenuItem tmnuCreateLandmark;
        public System.Windows.Forms.ToolStripMenuItem tmnuSetHome;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        public System.Windows.Forms.ToolStripMenuItem tmnuStatusAway;
        public System.Windows.Forms.ToolStripMenuItem tmnuStatusBusy;
        public System.Windows.Forms.ToolStripMenuItem tmnuTeleportHome;
        public System.Windows.Forms.ToolStripLabel lblTime;
        public System.Windows.Forms.Timer timerWorldClock;
        public System.Windows.Forms.ToolStripButton tbtnMedia;
        public System.Windows.Forms.ToolStripDropDownButton tbtnHelp;
        public System.Windows.Forms.ToolStripMenuItem reportBugsToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem aboutRadegastToolStripMenuItem;
        public System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        public System.Windows.Forms.ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        public System.Windows.Forms.ToolStripButton tbtnFriends;
        public System.Windows.Forms.ToolStripButton tbtnInventory;
        public System.Windows.Forms.ToolStripButton tbtnSearch;
        public System.Windows.Forms.ToolStripButton tbtnMap;
        public System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        public System.Windows.Forms.ToolStripMenuItem reconnectToolStripMenuItem;
    }
}

