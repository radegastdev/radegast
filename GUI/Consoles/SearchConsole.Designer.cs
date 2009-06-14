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
    partial class SearchConsole
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
            this.pnlFindPeople = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPersonName = new System.Windows.Forms.TextBox();
            this.btnFind = new System.Windows.Forms.Button();
            this.btnNewIM = new System.Windows.Forms.Button();
            this.lblResultCount = new System.Windows.Forms.Label();
            this.btnPrevious = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnProfile = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpgPeople = new System.Windows.Forms.TabPage();
            this.btnLink = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tpgPeople.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlFindPeople
            // 
            this.pnlFindPeople.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFindPeople.Location = new System.Drawing.Point(93, 62);
            this.pnlFindPeople.Name = "pnlFindPeople";
            this.pnlFindPeople.Size = new System.Drawing.Size(441, 252);
            this.pnlFindPeople.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Person\'s Name:";
            // 
            // txtPersonName
            // 
            this.txtPersonName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPersonName.Location = new System.Drawing.Point(93, 6);
            this.txtPersonName.Name = "txtPersonName";
            this.txtPersonName.Size = new System.Drawing.Size(360, 21);
            this.txtPersonName.TabIndex = 2;
            this.txtPersonName.TextChanged += new System.EventHandler(this.txtPersonName_TextChanged);
            this.txtPersonName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPersonName_KeyDown);
            this.txtPersonName.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtPersonName_KeyUp);
            // 
            // btnFind
            // 
            this.btnFind.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFind.Enabled = false;
            this.btnFind.Location = new System.Drawing.Point(459, 4);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(75, 23);
            this.btnFind.TabIndex = 3;
            this.btnFind.Text = "Search";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnNewIM
            // 
            this.btnNewIM.Enabled = false;
            this.btnNewIM.Location = new System.Drawing.Point(6, 33);
            this.btnNewIM.Name = "btnNewIM";
            this.btnNewIM.Size = new System.Drawing.Size(81, 23);
            this.btnNewIM.TabIndex = 4;
            this.btnNewIM.Text = "New IM";
            this.btnNewIM.UseVisualStyleBackColor = true;
            this.btnNewIM.Click += new System.EventHandler(this.btnNewIM_Click);
            // 
            // lblResultCount
            // 
            this.lblResultCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblResultCount.AutoSize = true;
            this.lblResultCount.Location = new System.Drawing.Point(6, 350);
            this.lblResultCount.Name = "lblResultCount";
            this.lblResultCount.Size = new System.Drawing.Size(79, 13);
            this.lblResultCount.TabIndex = 5;
            this.lblResultCount.Text = "0 people found";
            // 
            // btnPrevious
            // 
            this.btnPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevious.Enabled = false;
            this.btnPrevious.Location = new System.Drawing.Point(378, 345);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(75, 23);
            this.btnPrevious.TabIndex = 6;
            this.btnPrevious.Text = "< Previous";
            this.btnPrevious.UseVisualStyleBackColor = true;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnNext
            // 
            this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(459, 345);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 7;
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnProfile
            // 
            this.btnProfile.Enabled = false;
            this.btnProfile.Location = new System.Drawing.Point(6, 62);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(81, 23);
            this.btnProfile.TabIndex = 8;
            this.btnProfile.Text = "Profile";
            this.btnProfile.UseVisualStyleBackColor = true;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpgPeople);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(548, 400);
            this.tabControl1.TabIndex = 9;
            // 
            // tpgPeople
            // 
            this.tpgPeople.Controls.Add(this.btnLink);
            this.tpgPeople.Controls.Add(this.label1);
            this.tpgPeople.Controls.Add(this.btnProfile);
            this.tpgPeople.Controls.Add(this.pnlFindPeople);
            this.tpgPeople.Controls.Add(this.btnNext);
            this.tpgPeople.Controls.Add(this.txtPersonName);
            this.tpgPeople.Controls.Add(this.btnPrevious);
            this.tpgPeople.Controls.Add(this.btnFind);
            this.tpgPeople.Controls.Add(this.lblResultCount);
            this.tpgPeople.Controls.Add(this.btnNewIM);
            this.tpgPeople.Location = new System.Drawing.Point(4, 22);
            this.tpgPeople.Name = "tpgPeople";
            this.tpgPeople.Padding = new System.Windows.Forms.Padding(3);
            this.tpgPeople.Size = new System.Drawing.Size(540, 374);
            this.tpgPeople.TabIndex = 0;
            this.tpgPeople.Text = "People";
            this.tpgPeople.UseVisualStyleBackColor = true;
            // 
            // btnLink
            // 
            this.btnLink.Location = new System.Drawing.Point(93, 33);
            this.btnLink.Name = "btnLink";
            this.btnLink.Size = new System.Drawing.Size(432, 23);
            this.btnLink.TabIndex = 10;
            this.btnLink.UseVisualStyleBackColor = true;
            this.btnLink.Visible = false;
            this.btnLink.Click += new System.EventHandler(this.btnLink_Click);
            // 
            // SearchConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SearchConsole";
            this.Size = new System.Drawing.Size(548, 400);
            this.tabControl1.ResumeLayout(false);
            this.tpgPeople.ResumeLayout(false);
            this.tpgPeople.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlFindPeople;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtPersonName;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Button btnNewIM;
        private System.Windows.Forms.Label lblResultCount;
        private System.Windows.Forms.Button btnPrevious;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpgPeople;
        private System.Windows.Forms.Button btnLink;
    }
}
