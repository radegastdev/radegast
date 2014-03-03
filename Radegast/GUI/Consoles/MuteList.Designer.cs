// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
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
    partial class MuteList
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlMuteObjectByName = new System.Windows.Forms.Panel();
            this.btnMuteObjectByName = new System.Windows.Forms.Button();
            this.txtMuteByName = new System.Windows.Forms.TextBox();
            this.lblToMute = new System.Windows.Forms.Label();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnMuteByName = new System.Windows.Forms.Button();
            this.btnMuteResident = new System.Windows.Forms.Button();
            this.btnUnmute = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lvMuteList = new Radegast.ListViewNoFlicker();
            this.chType = new System.Windows.Forms.ColumnHeader();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.pnlMuteObjectByName.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMuteObjectByName
            // 
            this.pnlMuteObjectByName.Controls.Add(this.btnMuteObjectByName);
            this.pnlMuteObjectByName.Controls.Add(this.txtMuteByName);
            this.pnlMuteObjectByName.Controls.Add(this.lblToMute);
            this.pnlMuteObjectByName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlMuteObjectByName.Location = new System.Drawing.Point(0, 347);
            this.pnlMuteObjectByName.Name = "pnlMuteObjectByName";
            this.pnlMuteObjectByName.Size = new System.Drawing.Size(637, 42);
            this.pnlMuteObjectByName.TabIndex = 2;
            this.pnlMuteObjectByName.Visible = false;
            // 
            // btnMuteObjectByName
            // 
            this.btnMuteObjectByName.AccessibleName = "Mute object";
            this.btnMuteObjectByName.Location = new System.Drawing.Point(441, 10);
            this.btnMuteObjectByName.Name = "btnMuteObjectByName";
            this.btnMuteObjectByName.Size = new System.Drawing.Size(75, 23);
            this.btnMuteObjectByName.TabIndex = 2;
            this.btnMuteObjectByName.Text = "Mute";
            this.btnMuteObjectByName.UseVisualStyleBackColor = true;
            this.btnMuteObjectByName.Click += new System.EventHandler(this.btnMuteObjectByName_Click);
            // 
            // txtMuteByName
            // 
            this.txtMuteByName.AccessibleName = "Name of the object to mute";
            this.txtMuteByName.Location = new System.Drawing.Point(126, 12);
            this.txtMuteByName.Name = "txtMuteByName";
            this.txtMuteByName.Size = new System.Drawing.Size(309, 20);
            this.txtMuteByName.TabIndex = 1;
            this.txtMuteByName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMuteByName_KeyDown);
            // 
            // lblToMute
            // 
            this.lblToMute.AutoSize = true;
            this.lblToMute.Location = new System.Drawing.Point(3, 15);
            this.lblToMute.Name = "lblToMute";
            this.lblToMute.Size = new System.Drawing.Size(117, 13);
            this.lblToMute.TabIndex = 0;
            this.lblToMute.Text = "Name of object to mute";
            // 
            // pnlButtons
            // 
            this.pnlButtons.Controls.Add(this.btnMuteByName);
            this.pnlButtons.Controls.Add(this.btnMuteResident);
            this.pnlButtons.Controls.Add(this.btnUnmute);
            this.pnlButtons.Controls.Add(this.btnRefresh);
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlButtons.Location = new System.Drawing.Point(522, 0);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(115, 347);
            this.pnlButtons.TabIndex = 1;
            // 
            // btnMuteByName
            // 
            this.btnMuteByName.Location = new System.Drawing.Point(19, 129);
            this.btnMuteByName.Name = "btnMuteByName";
            this.btnMuteByName.Size = new System.Drawing.Size(75, 52);
            this.btnMuteByName.TabIndex = 3;
            this.btnMuteByName.Text = "Mute object by name";
            this.btnMuteByName.UseVisualStyleBackColor = true;
            this.btnMuteByName.Click += new System.EventHandler(this.btnMuteByName_Click);
            // 
            // btnMuteResident
            // 
            this.btnMuteResident.Location = new System.Drawing.Point(19, 71);
            this.btnMuteResident.Name = "btnMuteResident";
            this.btnMuteResident.Size = new System.Drawing.Size(75, 52);
            this.btnMuteResident.TabIndex = 2;
            this.btnMuteResident.Text = "Mute\r\nresident";
            this.btnMuteResident.UseVisualStyleBackColor = true;
            this.btnMuteResident.Click += new System.EventHandler(this.btnMuteResident_Click);
            // 
            // btnUnmute
            // 
            this.btnUnmute.Enabled = false;
            this.btnUnmute.Location = new System.Drawing.Point(19, 42);
            this.btnUnmute.Name = "btnUnmute";
            this.btnUnmute.Size = new System.Drawing.Size(75, 23);
            this.btnUnmute.TabIndex = 1;
            this.btnUnmute.Text = "Unmute";
            this.btnUnmute.UseVisualStyleBackColor = true;
            this.btnUnmute.Click += new System.EventHandler(this.btnUnmute_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(19, 13);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lvMuteList
            // 
            this.lvMuteList.AllowColumnReorder = true;
            this.lvMuteList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chType,
            this.chName});
            this.lvMuteList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvMuteList.FullRowSelect = true;
            this.lvMuteList.GridLines = true;
            this.lvMuteList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvMuteList.HideSelection = false;
            this.lvMuteList.Location = new System.Drawing.Point(0, 0);
            this.lvMuteList.Name = "lvMuteList";
            this.lvMuteList.ShowGroups = false;
            this.lvMuteList.Size = new System.Drawing.Size(522, 347);
            this.lvMuteList.TabIndex = 0;
            this.lvMuteList.UseCompatibleStateImageBehavior = false;
            this.lvMuteList.View = System.Windows.Forms.View.Details;
            this.lvMuteList.SelectedIndexChanged += new System.EventHandler(this.lvMuteList_SelectedIndexChanged);
            this.lvMuteList.SizeChanged += new System.EventHandler(this.lvMuteList_SizeChanged);
            // 
            // chType
            // 
            this.chType.Text = "Type";
            this.chType.Width = 90;
            // 
            // chName
            // 
            this.chName.Text = "Name";
            this.chName.Width = 394;
            // 
            // MuteList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lvMuteList);
            this.Controls.Add(this.pnlButtons);
            this.Controls.Add(this.pnlMuteObjectByName);
            this.Name = "MuteList";
            this.Size = new System.Drawing.Size(637, 389);
            this.pnlMuteObjectByName.ResumeLayout(false);
            this.pnlMuteObjectByName.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel pnlMuteObjectByName;
        public System.Windows.Forms.Panel pnlButtons;
        public System.Windows.Forms.Label lblToMute;
        public System.Windows.Forms.Button btnMuteByName;
        public System.Windows.Forms.Button btnUnmute;
        public System.Windows.Forms.Button btnRefresh;
        public System.Windows.Forms.Button btnMuteObjectByName;
        public System.Windows.Forms.TextBox txtMuteByName;
        public ListViewNoFlicker lvMuteList;
        public System.Windows.Forms.ColumnHeader chType;
        public System.Windows.Forms.ColumnHeader chName;
        public System.Windows.Forms.Button btnMuteResident;

    }
}
