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
// $Id: MainForm.cs 223 2009-09-12 04:33:49Z latifer@gmail.com $
//
namespace Radegast
{
    partial class frmAbout
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.txtTitle = new System.Windows.Forms.Label();
            this.txtLicense = new System.Windows.Forms.TextBox();
            this.txtOtherLicenses = new System.Windows.Forms.TextBox();
            this.txtWrittenBy = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblHomepage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::Radegast.Properties.Resources.radegast_large;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pictureBox1.InitialImage = null;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(143, 108);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // txtTitle
            // 
            this.txtTitle.AutoSize = true;
            this.txtTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(161, 9);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(137, 17);
            this.txtTitle.TabIndex = 1;
            this.txtTitle.Text = "Radegast 2.0.000";
            // 
            // txtLicense
            // 
            this.txtLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLicense.Location = new System.Drawing.Point(12, 125);
            this.txtLicense.MaximumSize = new System.Drawing.Size(315, 113);
            this.txtLicense.Multiline = true;
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.ReadOnly = true;
            this.txtLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLicense.Size = new System.Drawing.Size(315, 113);
            this.txtLicense.TabIndex = 2;
            this.txtLicense.TabStop = false;
            this.txtLicense.Text = resources.GetString("txtLicense.Text");
            // 
            // txtOtherLicenses
            // 
            this.txtOtherLicenses.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtOtherLicenses.Location = new System.Drawing.Point(12, 244);
            this.txtOtherLicenses.MaximumSize = new System.Drawing.Size(315, 92);
            this.txtOtherLicenses.Multiline = true;
            this.txtOtherLicenses.Name = "txtOtherLicenses";
            this.txtOtherLicenses.ReadOnly = true;
            this.txtOtherLicenses.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOtherLicenses.Size = new System.Drawing.Size(315, 92);
            this.txtOtherLicenses.TabIndex = 3;
            this.txtOtherLicenses.TabStop = false;
            this.txtOtherLicenses.Text = resources.GetString("txtOtherLicenses.Text");
            // 
            // txtWrittenBy
            // 
            this.txtWrittenBy.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtWrittenBy.Location = new System.Drawing.Point(165, 27);
            this.txtWrittenBy.Multiline = true;
            this.txtWrittenBy.Name = "txtWrittenBy";
            this.txtWrittenBy.ReadOnly = true;
            this.txtWrittenBy.Size = new System.Drawing.Size(162, 102);
            this.txtWrittenBy.TabIndex = 1;
            this.txtWrittenBy.TabStop = false;
            this.txtWrittenBy.Text = "Written by Latif Khalifa\r\nwith contributions from\r\n- Douglas R. Miles\r\n- Mojito S" +
    "orbet\r\n- Robin Cornelius\r\n- Revolution Smythe\r\n- Signpost Marv";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(251, 342);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // lblHomepage
            // 
            this.lblHomepage.AutoSize = true;
            this.lblHomepage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblHomepage.ForeColor = System.Drawing.Color.Blue;
            this.lblHomepage.Location = new System.Drawing.Point(12, 342);
            this.lblHomepage.Name = "lblHomepage";
            this.lblHomepage.Size = new System.Drawing.Size(93, 13);
            this.lblHomepage.TabIndex = 4;
            this.lblHomepage.Text = "www.radegast.org";
            this.lblHomepage.Click += new System.EventHandler(this.lblHomepage_Click);
            // 
            // frmAbout
            // 
            this.AcceptButton = this.btnClose;
            this.AutoSavePosition = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(339, 375);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.txtLicense);
            this.Controls.Add(this.lblHomepage);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.txtOtherLicenses);
            this.Controls.Add(this.txtWrittenBy);
            this.Controls.Add(this.pictureBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(351, 409);
            this.Name = "frmAbout";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "About Radegast";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label txtTitle;
        public System.Windows.Forms.TextBox txtLicense;
        public System.Windows.Forms.TextBox txtOtherLicenses;
        public System.Windows.Forms.TextBox txtWrittenBy;
        public System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblHomepage;

    }
}