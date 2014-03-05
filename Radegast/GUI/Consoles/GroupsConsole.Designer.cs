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
    partial class GroupsConsole
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
            if (disposing && (components != null)) {
                components.Dispose();
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnActivate = new System.Windows.Forms.Button();
            this.btnLeave = new System.Windows.Forms.Button();
            this.btnIM = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.btnInfo = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pnlGroupList = new System.Windows.Forms.Panel();
            this.lblGrpMax = new System.Windows.Forms.Label();
            this.lblGroupNr = new System.Windows.Forms.Label();
            this.btnKeys = new System.Windows.Forms.Button();
            this.btnNewGroup = new System.Windows.Forms.Button();
            this.btnMute = new System.Windows.Forms.Button();
            this.pnlNewGroup = new System.Windows.Forms.Panel();
            this.lblCreateStatus = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnCreateGroup = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNewGroupCharter = new System.Windows.Forms.TextBox();
            this.txtNewGroupName = new System.Windows.Forms.TextBox();
            this.pnlKeys = new System.Windows.Forms.Panel();
            this.txtKeys = new System.Windows.Forms.TextBox();
            this.pnlGroupList.SuspendLayout();
            this.pnlNewGroup.SuspendLayout();
            this.pnlKeys.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select group";
            // 
            // btnActivate
            // 
            this.btnActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivate.Location = new System.Drawing.Point(609, 68);
            this.btnActivate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(105, 28);
            this.btnActivate.TabIndex = 2;
            this.btnActivate.Text = "&Activate";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.activateBtn_Click);
            // 
            // btnLeave
            // 
            this.btnLeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeave.Location = new System.Drawing.Point(609, 139);
            this.btnLeave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnLeave.Name = "btnLeave";
            this.btnLeave.Size = new System.Drawing.Size(105, 28);
            this.btnLeave.TabIndex = 4;
            this.btnLeave.Text = "&Leave";
            this.btnLeave.UseVisualStyleBackColor = true;
            this.btnLeave.Click += new System.EventHandler(this.leaveBtn_Click);
            // 
            // btnIM
            // 
            this.btnIM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIM.Location = new System.Drawing.Point(609, 103);
            this.btnIM.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnIM.Name = "btnIM";
            this.btnIM.Size = new System.Drawing.Size(105, 28);
            this.btnIM.TabIndex = 3;
            this.btnIM.Text = "&IM";
            this.btnIM.UseVisualStyleBackColor = true;
            this.btnIM.Click += new System.EventHandler(this.imBtn_Click);
            // 
            // listBox1
            // 
            this.listBox1.AccessibleName = "List of groups";
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 32);
            this.listBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(588, 329);
            this.listBox1.Sorted = true;
            this.listBox1.TabIndex = 0;
            this.listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listBox1_DrawItem);
            this.listBox1.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.listBox1_MeasureItem);
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            // 
            // btnInfo
            // 
            this.btnInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfo.Location = new System.Drawing.Point(609, 32);
            this.btnInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(105, 28);
            this.btnInfo.TabIndex = 1;
            this.btnInfo.Text = "In&fo";
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(609, 175);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(105, 28);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // pnlGroupList
            // 
            this.pnlGroupList.Controls.Add(this.lblGrpMax);
            this.pnlGroupList.Controls.Add(this.lblGroupNr);
            this.pnlGroupList.Controls.Add(this.btnKeys);
            this.pnlGroupList.Controls.Add(this.btnNewGroup);
            this.pnlGroupList.Controls.Add(this.listBox1);
            this.pnlGroupList.Controls.Add(this.btnInfo);
            this.pnlGroupList.Controls.Add(this.label1);
            this.pnlGroupList.Controls.Add(this.btnActivate);
            this.pnlGroupList.Controls.Add(this.btnIM);
            this.pnlGroupList.Controls.Add(this.btnLeave);
            this.pnlGroupList.Controls.Add(this.btnMute);
            this.pnlGroupList.Controls.Add(this.btnRefresh);
            this.pnlGroupList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGroupList.Location = new System.Drawing.Point(0, 0);
            this.pnlGroupList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlGroupList.Name = "pnlGroupList";
            this.pnlGroupList.Size = new System.Drawing.Size(727, 365);
            this.pnlGroupList.TabIndex = 6;
            // 
            // lblGrpMax
            // 
            this.lblGrpMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGrpMax.AutoSize = true;
            this.lblGrpMax.Location = new System.Drawing.Point(605, 346);
            this.lblGrpMax.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGrpMax.Name = "lblGrpMax";
            this.lblGrpMax.Size = new System.Drawing.Size(33, 17);
            this.lblGrpMax.TabIndex = 7;
            this.lblGrpMax.Text = "max";
            // 
            // lblGroupNr
            // 
            this.lblGroupNr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGroupNr.AutoSize = true;
            this.lblGroupNr.Location = new System.Drawing.Point(605, 330);
            this.lblGroupNr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblGroupNr.Name = "lblGroupNr";
            this.lblGroupNr.Size = new System.Drawing.Size(64, 17);
            this.lblGroupNr.TabIndex = 7;
            this.lblGroupNr.Text = "0 groups";
            // 
            // btnKeys
            // 
            this.btnKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnKeys.Location = new System.Drawing.Point(609, 282);
            this.btnKeys.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnKeys.Name = "btnKeys";
            this.btnKeys.Size = new System.Drawing.Size(105, 28);
            this.btnKeys.TabIndex = 8;
            this.btnKeys.Text = "Keys";
            this.btnKeys.UseVisualStyleBackColor = true;
            this.btnKeys.Click += new System.EventHandler(this.btnKeys_Click);
            // 
            // btnNewGroup
            // 
            this.btnNewGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewGroup.Location = new System.Drawing.Point(609, 246);
            this.btnNewGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnNewGroup.Name = "btnNewGroup";
            this.btnNewGroup.Size = new System.Drawing.Size(105, 28);
            this.btnNewGroup.TabIndex = 7;
            this.btnNewGroup.Text = "New Group";
            this.btnNewGroup.UseVisualStyleBackColor = true;
            this.btnNewGroup.Click += new System.EventHandler(this.btnNewGroup_Click);
            // 
            // btnMute
            // 
            this.btnMute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMute.Location = new System.Drawing.Point(609, 210);
            this.btnMute.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnMute.Name = "btnMute";
            this.btnMute.Size = new System.Drawing.Size(105, 28);
            this.btnMute.TabIndex = 6;
            this.btnMute.Text = "Mute";
            this.btnMute.UseVisualStyleBackColor = true;
            this.btnMute.Click += new System.EventHandler(this.btnMute_Click);
            // 
            // pnlNewGroup
            // 
            this.pnlNewGroup.Controls.Add(this.lblCreateStatus);
            this.pnlNewGroup.Controls.Add(this.btnCancel);
            this.pnlNewGroup.Controls.Add(this.btnCreateGroup);
            this.pnlNewGroup.Controls.Add(this.label3);
            this.pnlNewGroup.Controls.Add(this.label4);
            this.pnlNewGroup.Controls.Add(this.label2);
            this.pnlNewGroup.Controls.Add(this.txtNewGroupCharter);
            this.pnlNewGroup.Controls.Add(this.txtNewGroupName);
            this.pnlNewGroup.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlNewGroup.Location = new System.Drawing.Point(0, 529);
            this.pnlNewGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlNewGroup.Name = "pnlNewGroup";
            this.pnlNewGroup.Size = new System.Drawing.Size(727, 148);
            this.pnlNewGroup.TabIndex = 8;
            this.pnlNewGroup.Visible = false;
            // 
            // lblCreateStatus
            // 
            this.lblCreateStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblCreateStatus.Location = new System.Drawing.Point(0, 123);
            this.lblCreateStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCreateStatus.Name = "lblCreateStatus";
            this.lblCreateStatus.Size = new System.Drawing.Size(727, 25);
            this.lblCreateStatus.TabIndex = 5;
            this.lblCreateStatus.TextChanged += new System.EventHandler(this.lblCreateStatus_TextChanged);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(449, 79);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnCreateGroup
            // 
            this.btnCreateGroup.Enabled = false;
            this.btnCreateGroup.Location = new System.Drawing.Point(449, 43);
            this.btnCreateGroup.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCreateGroup.Name = "btnCreateGroup";
            this.btnCreateGroup.Size = new System.Drawing.Size(100, 28);
            this.btnCreateGroup.TabIndex = 3;
            this.btnCreateGroup.Text = "Create";
            this.btnCreateGroup.UseVisualStyleBackColor = true;
            this.btnCreateGroup.Click += new System.EventHandler(this.btnCreateGroup_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 43);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 17);
            this.label3.TabIndex = 3;
            this.label3.Text = "Group Charter:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(445, 11);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 17);
            this.label4.TabIndex = 2;
            this.label4.Text = "4-35 chars";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Group name:";
            // 
            // txtNewGroupCharter
            // 
            this.txtNewGroupCharter.AccessibleName = "Group Charter";
            this.txtNewGroupCharter.Location = new System.Drawing.Point(129, 39);
            this.txtNewGroupCharter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtNewGroupCharter.Multiline = true;
            this.txtNewGroupCharter.Name = "txtNewGroupCharter";
            this.txtNewGroupCharter.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNewGroupCharter.Size = new System.Drawing.Size(307, 67);
            this.txtNewGroupCharter.TabIndex = 1;
            // 
            // txtNewGroupName
            // 
            this.txtNewGroupName.AccessibleName = "New Group Name";
            this.txtNewGroupName.Location = new System.Drawing.Point(129, 7);
            this.txtNewGroupName.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtNewGroupName.Name = "txtNewGroupName";
            this.txtNewGroupName.Size = new System.Drawing.Size(307, 22);
            this.txtNewGroupName.TabIndex = 0;
            this.txtNewGroupName.TextChanged += new System.EventHandler(this.txtNewGroupName_TextChanged);
            this.txtNewGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewGroupName_KeyDown);
            // 
            // pnlKeys
            // 
            this.pnlKeys.Controls.Add(this.txtKeys);
            this.pnlKeys.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlKeys.Location = new System.Drawing.Point(0, 365);
            this.pnlKeys.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlKeys.Name = "pnlKeys";
            this.pnlKeys.Size = new System.Drawing.Size(727, 164);
            this.pnlKeys.TabIndex = 7;
            this.pnlKeys.Visible = false;
            // 
            // txtKeys
            // 
            this.txtKeys.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeys.BackColor = System.Drawing.SystemColors.Window;
            this.txtKeys.Location = new System.Drawing.Point(12, 4);
            this.txtKeys.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtKeys.Multiline = true;
            this.txtKeys.Name = "txtKeys";
            this.txtKeys.ReadOnly = true;
            this.txtKeys.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtKeys.Size = new System.Drawing.Size(588, 154);
            this.txtKeys.TabIndex = 0;
            // 
            // GroupsConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlGroupList);
            this.Controls.Add(this.pnlKeys);
            this.Controls.Add(this.pnlNewGroup);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "GroupsConsole";
            this.Size = new System.Drawing.Size(727, 677);
            this.pnlGroupList.ResumeLayout(false);
            this.pnlGroupList.PerformLayout();
            this.pnlNewGroup.ResumeLayout(false);
            this.pnlNewGroup.PerformLayout();
            this.pnlKeys.ResumeLayout(false);
            this.pnlKeys.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Button btnActivate;
        public System.Windows.Forms.Button btnLeave;
        public System.Windows.Forms.Button btnIM;
        public System.Windows.Forms.ListBox listBox1;
        public System.Windows.Forms.Button btnInfo;
        public System.Windows.Forms.Button btnRefresh;
        public System.Windows.Forms.Label lblGrpMax;
        public System.Windows.Forms.Panel pnlGroupList;
        public System.Windows.Forms.Button btnNewGroup;
        public System.Windows.Forms.Panel pnlNewGroup;
        public System.Windows.Forms.Label lblGroupNr;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtNewGroupCharter;
        public System.Windows.Forms.TextBox txtNewGroupName;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button btnCreateGroup;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Label lblCreateStatus;
        public System.Windows.Forms.Button btnMute;
        public System.Windows.Forms.Panel pnlKeys;
        public System.Windows.Forms.Button btnKeys;
        private System.Windows.Forms.TextBox txtKeys;

    }
}