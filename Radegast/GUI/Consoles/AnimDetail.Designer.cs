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
    partial class AnimDetail
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pnlSave = new System.Windows.Forms.Panel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cbFriends = new System.Windows.Forms.ComboBox();
            this.boxAnimName = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.playBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.pnlSave.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pnlSave);
            this.groupBox1.Controls.Add(this.playBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(635, 41);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // pnlSave
            // 
            this.pnlSave.BackColor = System.Drawing.Color.Transparent;
            this.pnlSave.Controls.Add(this.lblStatus);
            this.pnlSave.Controls.Add(this.cbFriends);
            this.pnlSave.Controls.Add(this.boxAnimName);
            this.pnlSave.Controls.Add(this.btnSend);
            this.pnlSave.Controls.Add(this.btnSave);
            this.pnlSave.Location = new System.Drawing.Point(58, 0);
            this.pnlSave.Name = "pnlSave";
            this.pnlSave.Size = new System.Drawing.Size(577, 41);
            this.pnlSave.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(483, 16);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(91, 18);
            this.lblStatus.TabIndex = 6;
            // 
            // cbFriends
            // 
            this.cbFriends.FormattingEnabled = true;
            this.cbFriends.Location = new System.Drawing.Point(190, 13);
            this.cbFriends.Name = "cbFriends";
            this.cbFriends.Size = new System.Drawing.Size(162, 21);
            this.cbFriends.TabIndex = 5;
            this.cbFriends.SelectedValueChanged += new System.EventHandler(this.cbFriends_SelectedValueChanged);
            // 
            // boxAnimName
            // 
            this.boxAnimName.Location = new System.Drawing.Point(358, 13);
            this.boxAnimName.Name = "boxAnimName";
            this.boxAnimName.Size = new System.Drawing.Size(119, 20);
            this.boxAnimName.TabIndex = 4;
            // 
            // btnSend
            // 
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(95, 11);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(89, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "$L10 Send to";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(14, 11);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // playBox
            // 
            this.playBox.AutoSize = true;
            this.playBox.Location = new System.Drawing.Point(6, 15);
            this.playBox.Name = "playBox";
            this.playBox.Size = new System.Drawing.Size(46, 17);
            this.playBox.TabIndex = 0;
            this.playBox.Text = "Play";
            this.playBox.UseVisualStyleBackColor = true;
            this.playBox.CheckStateChanged += new System.EventHandler(this.playBox_CheckStateChanged);
            // 
            // AnimDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "AnimDetail";
            this.Size = new System.Drawing.Size(635, 41);
            this.Load += new System.EventHandler(this.AnimDetail_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnlSave.ResumeLayout(false);
            this.pnlSave.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.CheckBox playBox;
        public System.Windows.Forms.Panel pnlSave;
        public System.Windows.Forms.Button btnSend;
        public System.Windows.Forms.TextBox boxAnimName;
        public System.Windows.Forms.ComboBox cbFriends;
        public System.Windows.Forms.Label lblStatus;

    }
}
