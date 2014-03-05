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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FriendsConsole));
            this.lblFriendName = new System.Windows.Forms.Label();
            this.btnIM = new System.Windows.Forms.Button();
            this.btnProfile = new System.Windows.Forms.Button();
            this.pnlActions = new System.Windows.Forms.GroupBox();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnPay = new System.Windows.Forms.Button();
            this.btnOfferTeleport = new System.Windows.Forms.Button();
            this.chkSeeMeOnline = new System.Windows.Forms.CheckBox();
            this.chkSeeMeOnMap = new System.Windows.Forms.CheckBox();
            this.chkModifyMyObjects = new System.Windows.Forms.CheckBox();
            this.pnlFriendsRights = new System.Windows.Forms.GroupBox();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.listFriends = new System.Windows.Forms.ListBox();
            this.btnRequestTeleport = new System.Windows.Forms.Button();
            this.pnlActions.SuspendLayout();
            this.pnlFriendsRights.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblFriendName
            // 
            this.lblFriendName.AutoSize = true;
            this.lblFriendName.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFriendName.Location = new System.Drawing.Point(6, 17);
            this.lblFriendName.Name = "lblFriendName";
            this.lblFriendName.Size = new System.Drawing.Size(0, 13);
            this.lblFriendName.TabIndex = 1;
            // 
            // btnIM
            // 
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
            this.btnProfile.Location = new System.Drawing.Point(87, 46);
            this.btnProfile.Name = "btnProfile";
            this.btnProfile.Size = new System.Drawing.Size(75, 23);
            this.btnProfile.TabIndex = 3;
            this.btnProfile.Text = "Profile";
            this.btnProfile.UseVisualStyleBackColor = true;
            this.btnProfile.Click += new System.EventHandler(this.btnProfile_Click);
            // 
            // pnlActions
            // 
            this.pnlActions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlActions.Controls.Add(this.btnRequestTeleport);
            this.pnlActions.Controls.Add(this.btnRemove);
            this.pnlActions.Controls.Add(this.btnPay);
            this.pnlActions.Controls.Add(this.btnOfferTeleport);
            this.pnlActions.Controls.Add(this.lblFriendName);
            this.pnlActions.Controls.Add(this.btnProfile);
            this.pnlActions.Controls.Add(this.btnIM);
            this.pnlActions.Location = new System.Drawing.Point(209, 3);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Size = new System.Drawing.Size(470, 118);
            this.pnlActions.TabIndex = 1;
            this.pnlActions.TabStop = false;
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(87, 75);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnPay
            // 
            this.btnPay.Location = new System.Drawing.Point(9, 76);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(75, 23);
            this.btnPay.TabIndex = 5;
            this.btnPay.Text = "Pay...";
            this.btnPay.UseVisualStyleBackColor = true;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // btnOfferTeleport
            // 
            this.btnOfferTeleport.Location = new System.Drawing.Point(168, 46);
            this.btnOfferTeleport.Name = "btnOfferTeleport";
            this.btnOfferTeleport.Size = new System.Drawing.Size(112, 23);
            this.btnOfferTeleport.TabIndex = 4;
            this.btnOfferTeleport.Text = "Offer Teleport";
            this.btnOfferTeleport.UseVisualStyleBackColor = true;
            this.btnOfferTeleport.Click += new System.EventHandler(this.btnOfferTeleport_Click);
            // 
            // chkSeeMeOnline
            // 
            this.chkSeeMeOnline.AutoSize = true;
            this.chkSeeMeOnline.Location = new System.Drawing.Point(6, 20);
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
            this.chkSeeMeOnMap.Location = new System.Drawing.Point(6, 43);
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
            this.chkModifyMyObjects.Location = new System.Drawing.Point(6, 66);
            this.chkModifyMyObjects.Name = "chkModifyMyObjects";
            this.chkModifyMyObjects.Size = new System.Drawing.Size(113, 17);
            this.chkModifyMyObjects.TabIndex = 8;
            this.chkModifyMyObjects.Text = "Modify my objects";
            this.chkModifyMyObjects.UseVisualStyleBackColor = true;
            this.chkModifyMyObjects.CheckedChanged += new System.EventHandler(this.chkModifyMyObjects_CheckedChanged);
            // 
            // pnlFriendsRights
            // 
            this.pnlFriendsRights.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlFriendsRights.Controls.Add(this.chkModifyMyObjects);
            this.pnlFriendsRights.Controls.Add(this.chkSeeMeOnline);
            this.pnlFriendsRights.Controls.Add(this.chkSeeMeOnMap);
            this.pnlFriendsRights.Enabled = false;
            this.pnlFriendsRights.Location = new System.Drawing.Point(215, 136);
            this.pnlFriendsRights.Name = "pnlFriendsRights";
            this.pnlFriendsRights.Size = new System.Drawing.Size(461, 92);
            this.pnlFriendsRights.TabIndex = 2;
            this.pnlFriendsRights.TabStop = false;
            this.pnlFriendsRights.Text = "This friend can";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "GreenOrbFaded_16.png");
            this.imageList1.Images.SetKeyName(1, "GreenOrb_16.png");
            // 
            // listFriends
            // 
            this.listFriends.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listFriends.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.listFriends.FormattingEnabled = true;
            this.listFriends.ItemHeight = 18;
            this.listFriends.Location = new System.Drawing.Point(3, 9);
            this.listFriends.Name = "listFriends";
            this.listFriends.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listFriends.Size = new System.Drawing.Size(200, 454);
            this.listFriends.TabIndex = 0;
            this.listFriends.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.listFriends_DrawItem);
            this.listFriends.SelectedIndexChanged += new System.EventHandler(this.listFriends_SelectedIndexChanged);
            this.listFriends.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listFriends_KeyDown);
            this.listFriends.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listFriends_MouseDown);
            // 
            // btnRequestTeleport
            // 
            this.btnRequestTeleport.AccessibleDescription = "Request from a friend to teleport you to their location";
            this.btnRequestTeleport.Location = new System.Drawing.Point(168, 75);
            this.btnRequestTeleport.Name = "btnRequestTeleport";
            this.btnRequestTeleport.Size = new System.Drawing.Size(112, 23);
            this.btnRequestTeleport.TabIndex = 7;
            this.btnRequestTeleport.Text = "Request Teleport";
            this.btnRequestTeleport.UseVisualStyleBackColor = true;
            this.btnRequestTeleport.Click += new System.EventHandler(this.btnRequestTeleport_Click);
            // 
            // FriendsConsole
            // 
            this.Controls.Add(this.listFriends);
            this.Controls.Add(this.pnlFriendsRights);
            this.Controls.Add(this.pnlActions);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FriendsConsole";
            this.Size = new System.Drawing.Size(682, 466);
            this.pnlActions.ResumeLayout(false);
            this.pnlActions.PerformLayout();
            this.pnlFriendsRights.ResumeLayout(false);
            this.pnlFriendsRights.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public Label lblFriendName;
        public Button btnIM;
        public Button btnProfile;
        public GroupBox pnlActions;
        public CheckBox chkSeeMeOnline;
        public CheckBox chkSeeMeOnMap;
        public CheckBox chkModifyMyObjects;
        public Button btnOfferTeleport;
        public Button btnPay;
        public Button btnRemove;
        public GroupBox pnlFriendsRights;
        public ImageList imageList1;
        public ListBox listFriends;
        public Button btnRequestTeleport;
    }
}
