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
    partial class ImageUploadConsole
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
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.chkLossless = new System.Windows.Forms.CheckBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtAssetID = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkTemp = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoad
            // 
            this.btnLoad.Location = new System.Drawing.Point(3, 15);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(116, 23);
            this.btnLoad.TabIndex = 0;
            this.btnLoad.Text = "Load Image...";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Enabled = false;
            this.btnUpload.Location = new System.Drawing.Point(3, 73);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(116, 23);
            this.btnUpload.TabIndex = 2;
            this.btnUpload.Text = "Upload Image";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // pbPreview
            // 
            this.pbPreview.BackgroundImage = global::Radegast.Properties.Resources.checkerboard;
            this.pbPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbPreview.Location = new System.Drawing.Point(125, 15);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(256, 256);
            this.pbPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbPreview.TabIndex = 1;
            this.pbPreview.TabStop = false;
            // 
            // txtStatus
            // 
            this.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStatus.Location = new System.Drawing.Point(3, 306);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(378, 60);
            this.txtStatus.TabIndex = 10;
            this.txtStatus.Text = "Ready to load the image.\r\n";
            // 
            // btnSave
            // 
            this.btnSave.Enabled = false;
            this.btnSave.Location = new System.Drawing.Point(3, 44);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(116, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "Save Image As...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // chkLossless
            // 
            this.chkLossless.AutoSize = true;
            this.chkLossless.Location = new System.Drawing.Point(3, 102);
            this.chkLossless.Name = "chkLossless";
            this.chkLossless.Size = new System.Drawing.Size(66, 17);
            this.chkLossless.TabIndex = 4;
            this.chkLossless.Text = "Lossless";
            this.chkLossless.UseVisualStyleBackColor = true;
            // 
            // lblSize
            // 
            this.lblSize.Location = new System.Drawing.Point(9, 258);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(110, 13);
            this.lblSize.TabIndex = 11;
            this.lblSize.Text = "0 KB";
            this.lblSize.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(9, 279);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Asset UUID";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtAssetID
            // 
            this.txtAssetID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAssetID.Location = new System.Drawing.Point(125, 277);
            this.txtAssetID.Name = "txtAssetID";
            this.txtAssetID.ReadOnly = true;
            this.txtAssetID.Size = new System.Drawing.Size(256, 20);
            this.txtAssetID.TabIndex = 6;
            this.txtAssetID.Text = "00000000-0000-0000-0000-000000000000";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 78);
            this.label2.TabIndex = 13;
            this.label2.Text = "Set this checkbox\r\nbefore clicking on\r\nLoad image...\r\nOnly useful for\r\npixel perf" +
                "ect small\r\nimages (sculpties).\r\n";
            // 
            // chkTemp
            // 
            this.chkTemp.AutoSize = true;
            this.chkTemp.Location = new System.Drawing.Point(3, 214);
            this.chkTemp.Name = "chkTemp";
            this.chkTemp.Size = new System.Drawing.Size(76, 17);
            this.chkTemp.TabIndex = 5;
            this.chkTemp.Text = "Temporary";
            this.chkTemp.UseVisualStyleBackColor = true;
            // 
            // ImageUploadConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkTemp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAssetID);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.chkLossless);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.pbPreview);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnLoad);
            this.Name = "ImageUploadConsole";
            this.Size = new System.Drawing.Size(606, 419);
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtAssetID;
        public System.Windows.Forms.Button btnLoad;
        public System.Windows.Forms.Button btnUpload;
        public System.Windows.Forms.PictureBox pbPreview;
        public System.Windows.Forms.TextBox txtStatus;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.CheckBox chkLossless;
        public System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkTemp;

    }
}
