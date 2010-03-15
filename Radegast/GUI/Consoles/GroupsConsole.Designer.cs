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
            this.btnNewGroup = new System.Windows.Forms.Button();
            this.pnlNewGroup = new System.Windows.Forms.Panel();
            this.lblGroupNr = new System.Windows.Forms.Label();
            this.txtNewGroupName = new System.Windows.Forms.TextBox();
            this.txtNewGroupCharter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btnCreateGroup = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblCreateStatus = new System.Windows.Forms.Label();
            this.pnlGroupList.SuspendLayout();
            this.pnlNewGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select group";
            // 
            // btnActivate
            // 
            this.btnActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnActivate.Location = new System.Drawing.Point(457, 55);
            this.btnActivate.Name = "btnActivate";
            this.btnActivate.Size = new System.Drawing.Size(79, 23);
            this.btnActivate.TabIndex = 2;
            this.btnActivate.Text = "&Activate";
            this.btnActivate.UseVisualStyleBackColor = true;
            this.btnActivate.Click += new System.EventHandler(this.activateBtn_Click);
            // 
            // btnLeave
            // 
            this.btnLeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLeave.Location = new System.Drawing.Point(457, 113);
            this.btnLeave.Name = "btnLeave";
            this.btnLeave.Size = new System.Drawing.Size(79, 23);
            this.btnLeave.TabIndex = 4;
            this.btnLeave.Text = "&Leave";
            this.btnLeave.UseVisualStyleBackColor = true;
            this.btnLeave.Click += new System.EventHandler(this.leaveBtn_Click);
            // 
            // btnIM
            // 
            this.btnIM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnIM.Location = new System.Drawing.Point(457, 84);
            this.btnIM.Name = "btnIM";
            this.btnIM.Size = new System.Drawing.Size(79, 23);
            this.btnIM.TabIndex = 3;
            this.btnIM.Text = "&IM";
            this.btnIM.UseVisualStyleBackColor = true;
            this.btnIM.Click += new System.EventHandler(this.imBtn_Click);
            // 
            // listBox1
            // 
            this.listBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(9, 26);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(442, 277);
            this.listBox1.Sorted = true;
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDoubleClick);
            // 
            // btnInfo
            // 
            this.btnInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInfo.Location = new System.Drawing.Point(457, 26);
            this.btnInfo.Name = "btnInfo";
            this.btnInfo.Size = new System.Drawing.Size(79, 23);
            this.btnInfo.TabIndex = 1;
            this.btnInfo.Text = "In&fo";
            this.btnInfo.UseVisualStyleBackColor = true;
            this.btnInfo.Click += new System.EventHandler(this.btnInfo_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(457, 142);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(79, 23);
            this.btnRefresh.TabIndex = 5;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // pnlGroupList
            // 
            this.pnlGroupList.Controls.Add(this.lblGroupNr);
            this.pnlGroupList.Controls.Add(this.btnNewGroup);
            this.pnlGroupList.Controls.Add(this.listBox1);
            this.pnlGroupList.Controls.Add(this.btnInfo);
            this.pnlGroupList.Controls.Add(this.label1);
            this.pnlGroupList.Controls.Add(this.btnActivate);
            this.pnlGroupList.Controls.Add(this.btnIM);
            this.pnlGroupList.Controls.Add(this.btnLeave);
            this.pnlGroupList.Controls.Add(this.btnRefresh);
            this.pnlGroupList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGroupList.Location = new System.Drawing.Point(0, 0);
            this.pnlGroupList.Name = "pnlGroupList";
            this.pnlGroupList.Size = new System.Drawing.Size(545, 305);
            this.pnlGroupList.TabIndex = 6;
            // 
            // btnNewGroup
            // 
            this.btnNewGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewGroup.Location = new System.Drawing.Point(457, 171);
            this.btnNewGroup.Name = "btnNewGroup";
            this.btnNewGroup.Size = new System.Drawing.Size(79, 23);
            this.btnNewGroup.TabIndex = 6;
            this.btnNewGroup.Text = "New Group";
            this.btnNewGroup.UseVisualStyleBackColor = true;
            this.btnNewGroup.Click += new System.EventHandler(this.btnNewGroup_Click);
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
            this.pnlNewGroup.Location = new System.Drawing.Point(0, 305);
            this.pnlNewGroup.Name = "pnlNewGroup";
            this.pnlNewGroup.Size = new System.Drawing.Size(545, 120);
            this.pnlNewGroup.TabIndex = 7;
            this.pnlNewGroup.Visible = false;
            // 
            // lblGroupNr
            // 
            this.lblGroupNr.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGroupNr.AutoSize = true;
            this.lblGroupNr.Location = new System.Drawing.Point(454, 197);
            this.lblGroupNr.Name = "lblGroupNr";
            this.lblGroupNr.Size = new System.Drawing.Size(48, 13);
            this.lblGroupNr.TabIndex = 7;
            this.lblGroupNr.Text = "0 groups";
            // 
            // txtNewGroupName
            // 
            this.txtNewGroupName.AccessibleName = "New Group Name";
            this.txtNewGroupName.Location = new System.Drawing.Point(97, 6);
            this.txtNewGroupName.Name = "txtNewGroupName";
            this.txtNewGroupName.Size = new System.Drawing.Size(231, 20);
            this.txtNewGroupName.TabIndex = 0;
            this.txtNewGroupName.TextChanged += new System.EventHandler(this.txtNewGroupName_TextChanged);
            this.txtNewGroupName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtNewGroupName_KeyDown);
            // 
            // txtNewGroupCharter
            // 
            this.txtNewGroupCharter.AccessibleName = "Group Charter";
            this.txtNewGroupCharter.Location = new System.Drawing.Point(97, 32);
            this.txtNewGroupCharter.Multiline = true;
            this.txtNewGroupCharter.Name = "txtNewGroupCharter";
            this.txtNewGroupCharter.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNewGroupCharter.Size = new System.Drawing.Size(231, 55);
            this.txtNewGroupCharter.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Group name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Group Charter:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(334, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "4-35 chars";
            // 
            // btnCreateGroup
            // 
            this.btnCreateGroup.Enabled = false;
            this.btnCreateGroup.Location = new System.Drawing.Point(337, 35);
            this.btnCreateGroup.Name = "btnCreateGroup";
            this.btnCreateGroup.Size = new System.Drawing.Size(75, 23);
            this.btnCreateGroup.TabIndex = 3;
            this.btnCreateGroup.Text = "Create";
            this.btnCreateGroup.UseVisualStyleBackColor = true;
            this.btnCreateGroup.Click += new System.EventHandler(this.btnCreateGroup_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(337, 64);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblCreateStatus
            // 
            this.lblCreateStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblCreateStatus.Location = new System.Drawing.Point(0, 100);
            this.lblCreateStatus.Name = "lblCreateStatus";
            this.lblCreateStatus.Size = new System.Drawing.Size(545, 20);
            this.lblCreateStatus.TabIndex = 5;
            this.lblCreateStatus.TextChanged += new System.EventHandler(this.lblCreateStatus_TextChanged);
            // 
            // GroupsConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlGroupList);
            this.Controls.Add(this.pnlNewGroup);
            this.Name = "GroupsConsole";
            this.Size = new System.Drawing.Size(545, 425);
            this.pnlGroupList.ResumeLayout(false);
            this.pnlGroupList.PerformLayout();
            this.pnlNewGroup.ResumeLayout(false);
            this.pnlNewGroup.PerformLayout();
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
        private System.Windows.Forms.Panel pnlGroupList;
        private System.Windows.Forms.Button btnNewGroup;
        private System.Windows.Forms.Panel pnlNewGroup;
        private System.Windows.Forms.Label lblGroupNr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtNewGroupCharter;
        private System.Windows.Forms.TextBox txtNewGroupName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCreateGroup;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblCreateStatus;

    }
}