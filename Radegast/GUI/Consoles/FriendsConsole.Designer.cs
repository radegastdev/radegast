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
using System.Windows.Forms;

namespace Radegast
{
    partial class FriendsConsole
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
            this.components = new System.ComponentModel.Container();
            this.lbxFriends = new System.Windows.Forms.ListBox();
            this.lblFriendName = new System.Windows.Forms.Label();
            this.btnIM = new System.Windows.Forms.Button();
            this.btnProfile = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnPay = new System.Windows.Forms.Button();
            this.btnOfferTeleport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.chkSeeMeOnline = new System.Windows.Forms.CheckBox();
            this.chkSeeMeOnMap = new System.Windows.Forms.CheckBox();
            this.chkModifyMyObjects = new System.Windows.Forms.CheckBox();
            this.timInitDelay = new System.Windows.Forms.Timer(this.components);
            this.btnRemove = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbxFriends
            // 
            this.lbxFriends.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbxFriends.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbxFriends.IntegralHeight = false;
            this.lbxFriends.ItemHeight = 20;
            this.lbxFriends.Location = new System.Drawing.Point(3, 3);
            this.lbxFriends.Name = "lbxFriends";
            this.lbxFriends.Size = new System.Drawing.Size(200, 460);
            this.lbxFriends.Sorted = true;
            this.lbxFriends.TabIndex = 0;
            this.lbxFriends.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbxFriends_DrawItem);
            this.lbxFriends.SelectedIndexChanged += new System.EventHandler(this.lbxFriends_SelectedIndexChanged);
            this.lbxFriends.MouseUp += new MouseEventHandler(this.lbxFriends_MouseUp);
            // 
            // lblFriendName
            // 
            this.lblFriendName.AutoSize = true;
            this.lblFriendName.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFriendName.Location = new System.Drawing.Point(6, 17);
            this.lblFriendName.Name = "lblFriendName";
            this.lblFriendName.Size = new System.Drawing.Size(120, 13);
            this.lblFriendName.TabIndex = 1;
            this.lblFriendName.Text = "Getting friends list...";
            // 
            // btnIM
            // 
            this.btnIM.Enabled = false;
            this.btnIM.Location = new System.Drawing.Point(6, 47);
            this.btnIM.Name = "btnIM";
            this.btnIM.Size = new System.Drawing.Size(75, 23);
            this.btnIM.TabIndex = 2;
            this.btnIM.Text = "IM";
            this.btnIM.UseVisualStyleBackColor = true;
            this.btnIM.Click += new System.EventHandler(this.btnIM_Click);
            // 
            // btnProfile
            // 
            this.btnProfile.Enabled = false;
            this.btnProfile.Location = new System.Drawing.Point(87, 46);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(75, 23);
            this.btnProfile.TabIndex = 3;
            this.btnProfile.Text = "Profile";
            this.btnProfile.UseVisualStyleBackColor = true;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnRemove);
            this.groupBox1.Controls.Add(this.btnPay);
            this.groupBox1.Controls.Add(this.btnOfferTeleport);
            this.groupBox1.Controls.Add(this.lblFriendName);
            this.groupBox1.Controls.Add(this.btnProfile);
            this.groupBox1.Controls.Add(this.btnIM);
            this.groupBox1.Location = new System.Drawing.Point(209, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(470, 76);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            // 
            // btnPay
            // 
            this.btnPay.Enabled = false;
            this.btnPay.Location = new System.Drawing.Point(286, 46);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(75, 23);
            this.btnPay.TabIndex = 5;
            this.btnPay.Text = "Pay...";
            this.btnPay.UseVisualStyleBackColor = true;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // btnOfferTeleport
            // 
            this.btnOfferTeleport.Enabled = false;
            this.btnOfferTeleport.Location = new System.Drawing.Point(168, 46);
            this.btnOfferTeleport.Name = "btnOfferTeleport";
            this.btnOfferTeleport.Size = new System.Drawing.Size(112, 23);
            this.btnOfferTeleport.TabIndex = 4;
            this.btnOfferTeleport.Text = "Offer Teleport";
            this.btnOfferTeleport.UseVisualStyleBackColor = true;
            this.btnOfferTeleport.Click += new System.EventHandler(this.btnOfferTeleport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(209, 92);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "This friend can:";
            // 
            // chkSeeMeOnline
            // 
            this.chkSeeMeOnline.AutoSize = true;
            this.chkSeeMeOnline.Enabled = false;
            this.chkSeeMeOnline.Location = new System.Drawing.Point(218, 117);
            this.chkSeeMeOnline.Name = "chkSeeMeOnline";
            this.chkSeeMeOnline.Size = new System.Drawing.Size(125, 17);
            this.chkSeeMeOnline.TabIndex = 6;
            this.chkSeeMeOnline.Text = "See my online status";
            this.chkSeeMeOnline.UseVisualStyleBackColor = true;
            this.chkSeeMeOnline.CheckedChanged += new System.EventHandler(this.chkSeeMeOnline_CheckedChanged);
            // 
            // chkSeeMeOnMap
            // 
            this.chkSeeMeOnMap.AutoSize = true;
            this.chkSeeMeOnMap.Enabled = false;
            this.chkSeeMeOnMap.Location = new System.Drawing.Point(218, 140);
            this.chkSeeMeOnMap.Name = "chkSeeMeOnMap";
            this.chkSeeMeOnMap.Size = new System.Drawing.Size(118, 17);
            this.chkSeeMeOnMap.TabIndex = 7;
            this.chkSeeMeOnMap.Text = "See me on the map";
            this.chkSeeMeOnMap.UseVisualStyleBackColor = true;
            this.chkSeeMeOnMap.CheckedChanged += new System.EventHandler(this.chkSeeMeOnMap_CheckedChanged);
            // 
            // chkModifyMyObjects
            // 
            this.chkModifyMyObjects.AutoSize = true;
            this.chkModifyMyObjects.Enabled = false;
            this.chkModifyMyObjects.Location = new System.Drawing.Point(218, 163);
            this.chkModifyMyObjects.Name = "chkModifyMyObjects";
            this.chkModifyMyObjects.Size = new System.Drawing.Size(113, 17);
            this.chkModifyMyObjects.TabIndex = 8;
            this.chkModifyMyObjects.Text = "Modify my objects";
            this.chkModifyMyObjects.UseVisualStyleBackColor = true;
            this.chkModifyMyObjects.CheckedChanged += new System.EventHandler(this.chkModifyMyObjects_CheckedChanged);
            // 
            // timInitDelay
            // 
            this.timInitDelay.Enabled = true;
            this.timInitDelay.Interval = 3000;
            this.timInitDelay.Tick += new System.EventHandler(this.timInitDelay_Tick);
            // 
            // btnRemove
            // 
            this.btnRemove.Enabled = false;
            this.btnRemove.Location = new System.Drawing.Point(367, 46);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // FriendsConsole
            // 
            this.Controls.Add(this.chkModifyMyObjects);
            this.Controls.Add(this.chkSeeMeOnMap);
            this.Controls.Add(this.chkSeeMeOnline);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lbxFriends);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FriendsConsole";
            this.Size = new System.Drawing.Size(682, 466);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxFriends;
        private System.Windows.Forms.Label lblFriendName;
        private System.Windows.Forms.Button btnIM;
        private System.Windows.Forms.Button btnProfile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkSeeMeOnline;
        private System.Windows.Forms.CheckBox chkSeeMeOnMap;
        private System.Windows.Forms.CheckBox chkModifyMyObjects;
        private System.Windows.Forms.Button btnOfferTeleport;
        private System.Windows.Forms.Timer timInitDelay;
        private System.Windows.Forms.Button btnPay;
        private System.Windows.Forms.Button btnRemove;
    }
}
