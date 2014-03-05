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
    partial class AvatarPicker
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
            this.tcPicker = new System.Windows.Forms.TabControl();
            this.tpSearch = new System.Windows.Forms.TabPage();
            this.lvwSearch = new ListViewNoFlicker();
            this.pnlSearch = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.tpNear = new System.Windows.Forms.TabPage();
            this.lvwNear = new ListViewNoFlicker();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.tcPicker.SuspendLayout();
            this.tpSearch.SuspendLayout();
            this.pnlSearch.SuspendLayout();
            this.tpNear.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcPicker
            // 
            this.tcPicker.Controls.Add(this.tpSearch);
            this.tcPicker.Controls.Add(this.tpNear);
            this.tcPicker.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcPicker.Location = new System.Drawing.Point(0, 0);
            this.tcPicker.Name = "tcPicker";
            this.tcPicker.SelectedIndex = 0;
            this.tcPicker.Size = new System.Drawing.Size(259, 296);
            this.tcPicker.TabIndex = 0;
            // 
            // tpSearch
            // 
            this.tpSearch.Controls.Add(this.lvwSearch);
            this.tpSearch.Controls.Add(this.pnlSearch);
            this.tpSearch.Location = new System.Drawing.Point(4, 22);
            this.tpSearch.Name = "tpSearch";
            this.tpSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tpSearch.Size = new System.Drawing.Size(251, 270);
            this.tpSearch.TabIndex = 0;
            this.tpSearch.Text = "Search";
            this.tpSearch.UseVisualStyleBackColor = true;
            // 
            // lvwSearch
            // 
            this.lvwSearch.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.lvwSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwSearch.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwSearch.HideSelection = false;
            this.lvwSearch.Location = new System.Drawing.Point(3, 57);
            this.lvwSearch.Name = "lvwSearch";
            this.lvwSearch.Size = new System.Drawing.Size(245, 210);
            this.lvwSearch.TabIndex = 2;
            this.lvwSearch.UseCompatibleStateImageBehavior = false;
            this.lvwSearch.View = System.Windows.Forms.View.Details;
            this.lvwSearch.SelectedIndexChanged += new System.EventHandler(this.lvwSearch_SelectedIndexChanged);
            this.lvwSearch.SizeChanged += new System.EventHandler(this.lvwSearch_SizeChanged);
            // 
            // pnlSearch
            // 
            this.pnlSearch.Controls.Add(this.label1);
            this.pnlSearch.Controls.Add(this.btnSearch);
            this.pnlSearch.Controls.Add(this.txtSearch);
            this.pnlSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSearch.Location = new System.Drawing.Point(3, 3);
            this.pnlSearch.Name = "pnlSearch";
            this.pnlSearch.Size = new System.Drawing.Size(245, 54);
            this.pnlSearch.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Type part of residents name:";
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Enabled = false;
            this.btnSearch.Location = new System.Drawing.Point(167, 25);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 1;
            this.btnSearch.Text = "&Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.AccessibleName = "Search";
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(3, 25);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(143, 20);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // tpNear
            // 
            this.tpNear.Controls.Add(this.lvwNear);
            this.tpNear.Location = new System.Drawing.Point(4, 22);
            this.tpNear.Name = "tpNear";
            this.tpNear.Padding = new System.Windows.Forms.Padding(3);
            this.tpNear.Size = new System.Drawing.Size(251, 270);
            this.tpNear.TabIndex = 1;
            this.tpNear.Text = "Near Me";
            this.tpNear.UseVisualStyleBackColor = true;
            // 
            // lvwNear
            // 
            this.lvwNear.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2});
            this.lvwNear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwNear.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwNear.HideSelection = false;
            this.lvwNear.Location = new System.Drawing.Point(3, 3);
            this.lvwNear.Name = "lvwNear";
            this.lvwNear.Size = new System.Drawing.Size(245, 264);
            this.lvwNear.TabIndex = 1;
            this.lvwNear.UseCompatibleStateImageBehavior = false;
            this.lvwNear.View = System.Windows.Forms.View.Details;
            this.lvwNear.SelectedIndexChanged += new System.EventHandler(this.lvwNear_SelectedIndexChanged);
            this.lvwNear.SizeChanged += new System.EventHandler(this.lvwSearch_SizeChanged);
            // 
            // AvatarPicker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcPicker);
            this.Name = "AvatarPicker";
            this.Size = new System.Drawing.Size(259, 296);
            this.tcPicker.ResumeLayout(false);
            this.tpSearch.ResumeLayout(false);
            this.pnlSearch.ResumeLayout(false);
            this.pnlSearch.PerformLayout();
            this.tpNear.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TabControl tcPicker;
        public System.Windows.Forms.TabPage tpSearch;
        public System.Windows.Forms.TabPage tpNear;
        public ListViewNoFlicker lvwSearch;
        public ListViewNoFlicker lvwNear;
        public System.Windows.Forms.Panel pnlSearch;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnSearch;
        public System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;

    }
}
