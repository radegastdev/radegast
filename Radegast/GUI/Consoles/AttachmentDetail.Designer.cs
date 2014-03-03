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
    partial class AttachmentDetail
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
            this.lblAttachment = new System.Windows.Forms.Label();
            this.boxID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.boxName = new System.Windows.Forms.TextBox();
            this.btnTouch = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblPrimCount = new System.Windows.Forms.Label();
            this.lblAttachmentPoint = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblAttachment
            // 
            this.lblAttachment.AutoSize = true;
            this.lblAttachment.Location = new System.Drawing.Point(207, 5);
            this.lblAttachment.Name = "lblAttachment";
            this.lblAttachment.Size = new System.Drawing.Size(75, 13);
            this.lblAttachment.TabIndex = 0;
            this.lblAttachment.Text = "Attachment ID";
            // 
            // boxID
            // 
            this.boxID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.boxID.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.boxID.Location = new System.Drawing.Point(288, 5);
            this.boxID.Name = "boxID";
            this.boxID.ReadOnly = true;
            this.boxID.Size = new System.Drawing.Size(227, 13);
            this.boxID.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // boxName
            // 
            this.boxName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.boxName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.boxName.Location = new System.Drawing.Point(85, 25);
            this.boxName.Name = "boxName";
            this.boxName.ReadOnly = true;
            this.boxName.Size = new System.Drawing.Size(410, 13);
            this.boxName.TabIndex = 1;
            this.boxName.Text = "Loading...";
            // 
            // btnTouch
            // 
            this.btnTouch.Location = new System.Drawing.Point(6, 45);
            this.btnTouch.Name = "btnTouch";
            this.btnTouch.Size = new System.Drawing.Size(75, 23);
            this.btnTouch.TabIndex = 2;
            this.btnTouch.Text = "Touch";
            this.btnTouch.UseVisualStyleBackColor = true;
            this.btnTouch.Click += new System.EventHandler(this.btnTouch_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(168, 44);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Save...";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblPrimCount
            // 
            this.lblPrimCount.AutoSize = true;
            this.lblPrimCount.Location = new System.Drawing.Point(123, 5);
            this.lblPrimCount.Name = "lblPrimCount";
            this.lblPrimCount.Size = new System.Drawing.Size(81, 13);
            this.lblPrimCount.TabIndex = 3;
            this.lblPrimCount.Text = "Prims: loading...";
            // 
            // lblAttachmentPoint
            // 
            this.lblAttachmentPoint.AutoSize = true;
            this.lblAttachmentPoint.Location = new System.Drawing.Point(3, 5);
            this.lblAttachmentPoint.Name = "lblAttachmentPoint";
            this.lblAttachmentPoint.Size = new System.Drawing.Size(31, 13);
            this.lblAttachmentPoint.TabIndex = 0;
            this.lblAttachmentPoint.Text = "Point";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(87, 44);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "View";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // AttachmentDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblPrimCount);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnTouch);
            this.Controls.Add(this.boxName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.boxID);
            this.Controls.Add(this.lblAttachmentPoint);
            this.Controls.Add(this.lblAttachment);
            this.Name = "AttachmentDetail";
            this.Size = new System.Drawing.Size(518, 74);
            this.Load += new System.EventHandler(this.AttachmentDetail_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblAttachment;
        public System.Windows.Forms.TextBox boxID;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox boxName;
        public System.Windows.Forms.Button btnTouch;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.Label lblPrimCount;
        public System.Windows.Forms.Label lblAttachmentPoint;
        private System.Windows.Forms.Button button1;

    }
}
