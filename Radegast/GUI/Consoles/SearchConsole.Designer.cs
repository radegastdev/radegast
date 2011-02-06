// 
// Radegast Metaverse Client
// Copyright (c) 2009-2011, Radegast Development Team
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
            this.tpgPlaces = new System.Windows.Forms.TabPage();
            this.pnlPlaceDetail = new System.Windows.Forms.Panel();
            this.btnSearchPlace = new System.Windows.Forms.Button();
            this.txtSearchPlace = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnNextPlace = new System.Windows.Forms.Button();
            this.btnPrevPlace = new System.Windows.Forms.Button();
            this.lblNrPlaces = new System.Windows.Forms.Label();
            this.tpgGroups = new System.Windows.Forms.TabPage();
            this.pnlGroupDetail = new System.Windows.Forms.Panel();
            this.btnSearchGroup = new System.Windows.Forms.Button();
            this.txtSearchGroup = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnNextGroup = new System.Windows.Forms.Button();
            this.btnPrevGroup = new System.Windows.Forms.Button();
            this.lblNrGroups = new System.Windows.Forms.Label();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.lvwPlaces = new Radegast.ListViewNoFlicker();
            this.Place = new System.Windows.Forms.ColumnHeader();
            this.Traffic = new System.Windows.Forms.ColumnHeader();
            this.lvwGroups = new Radegast.ListViewNoFlicker();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.tabControl1.SuspendLayout();
            this.tpgPeople.SuspendLayout();
            this.tpgPlaces.SuspendLayout();
            this.tpgGroups.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.tpgPlaces);
            this.tabControl1.Controls.Add(this.tpgGroups);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(680, 400);
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
            this.tpgPeople.Size = new System.Drawing.Size(672, 374);
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
            // tpgPlaces
            // 
            this.tpgPlaces.Controls.Add(this.pnlPlaceDetail);
            this.tpgPlaces.Controls.Add(this.lvwPlaces);
            this.tpgPlaces.Controls.Add(this.btnSearchPlace);
            this.tpgPlaces.Controls.Add(this.txtSearchPlace);
            this.tpgPlaces.Controls.Add(this.label2);
            this.tpgPlaces.Controls.Add(this.btnNextPlace);
            this.tpgPlaces.Controls.Add(this.btnPrevPlace);
            this.tpgPlaces.Controls.Add(this.lblNrPlaces);
            this.tpgPlaces.Location = new System.Drawing.Point(4, 22);
            this.tpgPlaces.Name = "tpgPlaces";
            this.tpgPlaces.Size = new System.Drawing.Size(672, 374);
            this.tpgPlaces.TabIndex = 1;
            this.tpgPlaces.Text = "Places";
            this.tpgPlaces.UseVisualStyleBackColor = true;
            // 
            // pnlPlaceDetail
            // 
            this.pnlPlaceDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlPlaceDetail.Location = new System.Drawing.Point(315, 10);
            this.pnlPlaceDetail.Name = "pnlPlaceDetail";
            this.pnlPlaceDetail.Size = new System.Drawing.Size(353, 338);
            this.pnlPlaceDetail.TabIndex = 15;
            // 
            // btnSearchPlace
            // 
            this.btnSearchPlace.Enabled = false;
            this.btnSearchPlace.Location = new System.Drawing.Point(231, 6);
            this.btnSearchPlace.Name = "btnSearchPlace";
            this.btnSearchPlace.Size = new System.Drawing.Size(75, 23);
            this.btnSearchPlace.TabIndex = 13;
            this.btnSearchPlace.Text = "Search";
            this.btnSearchPlace.UseVisualStyleBackColor = true;
            this.btnSearchPlace.Click += new System.EventHandler(this.btnSearchPlace_Click);
            // 
            // txtSearchPlace
            // 
            this.txtSearchPlace.Location = new System.Drawing.Point(47, 8);
            this.txtSearchPlace.Name = "txtSearchPlace";
            this.txtSearchPlace.Size = new System.Drawing.Size(178, 21);
            this.txtSearchPlace.TabIndex = 12;
            this.txtSearchPlace.TextChanged += new System.EventHandler(this.txtSearchPlace_TextChanged);
            this.txtSearchPlace.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchPlace_KeyDown);
            this.txtSearchPlace.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txtSearchPlace_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Place";
            // 
            // btnNextPlace
            // 
            this.btnNextPlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextPlace.Enabled = false;
            this.btnNextPlace.Location = new System.Drawing.Point(594, 348);
            this.btnNextPlace.Name = "btnNextPlace";
            this.btnNextPlace.Size = new System.Drawing.Size(75, 23);
            this.btnNextPlace.TabIndex = 10;
            this.btnNextPlace.Text = "Next >";
            this.btnNextPlace.UseVisualStyleBackColor = true;
            this.btnNextPlace.Click += new System.EventHandler(this.btnNextPlace_Click);
            // 
            // btnPrevPlace
            // 
            this.btnPrevPlace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevPlace.Enabled = false;
            this.btnPrevPlace.Location = new System.Drawing.Point(513, 348);
            this.btnPrevPlace.Name = "btnPrevPlace";
            this.btnPrevPlace.Size = new System.Drawing.Size(75, 23);
            this.btnPrevPlace.TabIndex = 9;
            this.btnPrevPlace.Text = "< Previous";
            this.btnPrevPlace.UseVisualStyleBackColor = true;
            this.btnPrevPlace.Click += new System.EventHandler(this.btnPrevPlace_Click);
            // 
            // lblNrPlaces
            // 
            this.lblNrPlaces.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNrPlaces.AutoSize = true;
            this.lblNrPlaces.Location = new System.Drawing.Point(9, 353);
            this.lblNrPlaces.Name = "lblNrPlaces";
            this.lblNrPlaces.Size = new System.Drawing.Size(77, 13);
            this.lblNrPlaces.TabIndex = 8;
            this.lblNrPlaces.Text = "0 places found";
            // 
            // tpgGroups
            // 
            this.tpgGroups.Controls.Add(this.pnlGroupDetail);
            this.tpgGroups.Controls.Add(this.lvwGroups);
            this.tpgGroups.Controls.Add(this.btnSearchGroup);
            this.tpgGroups.Controls.Add(this.txtSearchGroup);
            this.tpgGroups.Controls.Add(this.label3);
            this.tpgGroups.Controls.Add(this.btnNextGroup);
            this.tpgGroups.Controls.Add(this.btnPrevGroup);
            this.tpgGroups.Controls.Add(this.lblNrGroups);
            this.tpgGroups.Location = new System.Drawing.Point(4, 22);
            this.tpgGroups.Name = "tpgGroups";
            this.tpgGroups.Padding = new System.Windows.Forms.Padding(3);
            this.tpgGroups.Size = new System.Drawing.Size(672, 374);
            this.tpgGroups.TabIndex = 2;
            this.tpgGroups.Text = "Groups";
            this.tpgGroups.UseVisualStyleBackColor = true;
            // 
            // pnlGroupDetail
            // 
            this.pnlGroupDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGroupDetail.AutoScroll = true;
            this.pnlGroupDetail.Location = new System.Drawing.Point(312, 9);
            this.pnlGroupDetail.Name = "pnlGroupDetail";
            this.pnlGroupDetail.Size = new System.Drawing.Size(353, 338);
            this.pnlGroupDetail.TabIndex = 23;
            // 
            // btnSearchGroup
            // 
            this.btnSearchGroup.Enabled = false;
            this.btnSearchGroup.Location = new System.Drawing.Point(228, 5);
            this.btnSearchGroup.Name = "btnSearchGroup";
            this.btnSearchGroup.Size = new System.Drawing.Size(75, 23);
            this.btnSearchGroup.TabIndex = 21;
            this.btnSearchGroup.Text = "Search";
            this.btnSearchGroup.UseVisualStyleBackColor = true;
            this.btnSearchGroup.Click += new System.EventHandler(this.btnSearchGroup_Click);
            // 
            // txtSearchGroup
            // 
            this.txtSearchGroup.Location = new System.Drawing.Point(44, 7);
            this.txtSearchGroup.Name = "txtSearchGroup";
            this.txtSearchGroup.Size = new System.Drawing.Size(178, 21);
            this.txtSearchGroup.TabIndex = 20;
            this.txtSearchGroup.TextChanged += new System.EventHandler(this.txtSearchGroup_TextChanged);
            this.txtSearchGroup.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearchGroup_KeyDown);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(36, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "Group";
            // 
            // btnNextGroup
            // 
            this.btnNextGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextGroup.Enabled = false;
            this.btnNextGroup.Location = new System.Drawing.Point(591, 347);
            this.btnNextGroup.Name = "btnNextGroup";
            this.btnNextGroup.Size = new System.Drawing.Size(75, 23);
            this.btnNextGroup.TabIndex = 18;
            this.btnNextGroup.Text = "Next >";
            this.btnNextGroup.UseVisualStyleBackColor = true;
            this.btnNextGroup.Click += new System.EventHandler(this.btnNextGroup_Click);
            // 
            // btnPrevGroup
            // 
            this.btnPrevGroup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrevGroup.Enabled = false;
            this.btnPrevGroup.Location = new System.Drawing.Point(510, 347);
            this.btnPrevGroup.Name = "btnPrevGroup";
            this.btnPrevGroup.Size = new System.Drawing.Size(75, 23);
            this.btnPrevGroup.TabIndex = 17;
            this.btnPrevGroup.Text = "< Previous";
            this.btnPrevGroup.UseVisualStyleBackColor = true;
            this.btnPrevGroup.Click += new System.EventHandler(this.btnPrevGroup_Click);
            // 
            // lblNrGroups
            // 
            this.lblNrGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblNrGroups.AutoSize = true;
            this.lblNrGroups.Location = new System.Drawing.Point(6, 352);
            this.lblNrGroups.Name = "lblNrGroups";
            this.lblNrGroups.Size = new System.Drawing.Size(80, 13);
            this.lblNrGroups.TabIndex = 16;
            this.lblNrGroups.Text = "0 groups found";
            this.lblNrGroups.Visible = false;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Place";
            this.columnHeader1.Width = 200;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Traffic";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 50;
            // 
            // lvwPlaces
            // 
            this.lvwPlaces.AllowColumnReorder = true;
            this.lvwPlaces.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvwPlaces.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Place,
            this.Traffic});
            this.lvwPlaces.FullRowSelect = true;
            this.lvwPlaces.GridLines = true;
            this.lvwPlaces.HideSelection = false;
            this.lvwPlaces.Location = new System.Drawing.Point(12, 35);
            this.lvwPlaces.MultiSelect = false;
            this.lvwPlaces.Name = "lvwPlaces";
            this.lvwPlaces.ShowGroups = false;
            this.lvwPlaces.ShowItemToolTips = true;
            this.lvwPlaces.Size = new System.Drawing.Size(294, 315);
            this.lvwPlaces.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwPlaces.TabIndex = 14;
            this.lvwPlaces.UseCompatibleStateImageBehavior = false;
            this.lvwPlaces.View = System.Windows.Forms.View.Details;
            this.lvwPlaces.SelectedIndexChanged += new System.EventHandler(this.lvwPlaces_SelectedIndexChanged);
            this.lvwPlaces.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwPlaces_ColumnClick);
            // 
            // Place
            // 
            this.Place.Text = "Place";
            this.Place.Width = 200;
            // 
            // Traffic
            // 
            this.Traffic.Text = "Traffic";
            this.Traffic.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.Traffic.Width = 50;
            // 
            // lvwGroups
            // 
            this.lvwGroups.AllowColumnReorder = true;
            this.lvwGroups.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lvwGroups.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvwGroups.FullRowSelect = true;
            this.lvwGroups.GridLines = true;
            this.lvwGroups.HideSelection = false;
            this.lvwGroups.Location = new System.Drawing.Point(9, 34);
            this.lvwGroups.MultiSelect = false;
            this.lvwGroups.Name = "lvwGroups";
            this.lvwGroups.ShowGroups = false;
            this.lvwGroups.ShowItemToolTips = true;
            this.lvwGroups.Size = new System.Drawing.Size(294, 315);
            this.lvwGroups.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvwGroups.TabIndex = 22;
            this.lvwGroups.UseCompatibleStateImageBehavior = false;
            this.lvwGroups.View = System.Windows.Forms.View.Details;
            this.lvwGroups.SelectedIndexChanged += new System.EventHandler(this.lvwGroups_SelectedIndexChanged);
            this.lvwGroups.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.lvwGroups_ColumnClick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Group";
            this.columnHeader3.Width = 200;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Members";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 50;
            // 
            // SearchConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(680, 0);
            this.Name = "SearchConsole";
            this.Size = new System.Drawing.Size(680, 400);
            this.tabControl1.ResumeLayout(false);
            this.tpgPeople.ResumeLayout(false);
            this.tpgPeople.PerformLayout();
            this.tpgPlaces.ResumeLayout(false);
            this.tpgPlaces.PerformLayout();
            this.tpgGroups.ResumeLayout(false);
            this.tpgGroups.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel pnlFindPeople;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtPersonName;
        public System.Windows.Forms.Button btnFind;
        public System.Windows.Forms.Button btnNewIM;
        public System.Windows.Forms.Label lblResultCount;
        public System.Windows.Forms.Button btnPrevious;
        public System.Windows.Forms.Button btnNext;
        public System.Windows.Forms.Button btnProfile;
        public System.Windows.Forms.TabControl tabControl1;
        public System.Windows.Forms.TabPage tpgPeople;
        public System.Windows.Forms.Button btnLink;
        public System.Windows.Forms.TabPage tpgPlaces;
        public System.Windows.Forms.Button btnSearchPlace;
        public System.Windows.Forms.TextBox txtSearchPlace;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button btnNextPlace;
        public System.Windows.Forms.Button btnPrevPlace;
        public System.Windows.Forms.Label lblNrPlaces;
        public ListViewNoFlicker lvwPlaces;
        public System.Windows.Forms.ColumnHeader Place;
        public System.Windows.Forms.ColumnHeader Traffic;
        public System.Windows.Forms.Panel pnlPlaceDetail;
        public System.Windows.Forms.TabPage tpgGroups;
        public System.Windows.Forms.Panel pnlGroupDetail;
        public ListViewNoFlicker lvwGroups;
        public System.Windows.Forms.ColumnHeader columnHeader3;
        public System.Windows.Forms.ColumnHeader columnHeader4;
        public System.Windows.Forms.Button btnSearchGroup;
        public System.Windows.Forms.TextBox txtSearchGroup;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button btnNextGroup;
        public System.Windows.Forms.Button btnPrevGroup;
        public System.Windows.Forms.Label lblNrGroups;
        public System.Windows.Forms.ColumnHeader columnHeader1;
        public System.Windows.Forms.ColumnHeader columnHeader2;

    }
}
