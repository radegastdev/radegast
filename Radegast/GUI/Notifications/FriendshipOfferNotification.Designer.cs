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
    partial class ntfFriendshipOffer
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
            this.btnNo = new System.Windows.Forms.Button();
            this.btnYes = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.txtHead = new System.Windows.Forms.TextBox();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnNo
            // 
            this.btnNo.Location = new System.Drawing.Point(88, 94);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(75, 23);
            this.btnNo.TabIndex = 4;
            this.btnNo.Text = "&No";
            this.btnNo.UseVisualStyleBackColor = true;
            this.btnNo.Click += new System.EventHandler(this.btnNo_Click);
            // 
            // btnYes
            // 
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.Location = new System.Drawing.Point(7, 94);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(75, 23);
            this.btnYes.TabIndex = 3;
            this.btnYes.Text = "&Yes";
            this.btnYes.UseVisualStyleBackColor = true;
            this.btnYes.Click += new System.EventHandler(this.btnYes_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessage.Location = new System.Drawing.Point(7, 54);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(244, 34);
            this.txtMessage.TabIndex = 6;
            this.txtMessage.TabStop = false;
            this.txtMessage.Text = "Will you be my friend?";
            // 
            // txtHead
            // 
            this.txtHead.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHead.Location = new System.Drawing.Point(7, 14);
            this.txtHead.Multiline = true;
            this.txtHead.Name = "txtHead";
            this.txtHead.ReadOnly = true;
            this.txtHead.Size = new System.Drawing.Size(244, 34);
            this.txtHead.TabIndex = 5;
            this.txtHead.TabStop = false;
            this.txtHead.Text = "First Last has offered you friendship.";
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(169, 94);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(75, 23);
            this.btnIgnore.TabIndex = 4;
            this.btnIgnore.Text = "&Ignore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // ntfFriendshipOffer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnIgnore);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.txtHead);
            this.Name = "ntfFriendshipOffer";
            this.Size = new System.Drawing.Size(263, 131);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button btnNo;
        public System.Windows.Forms.Button btnYes;
        public System.Windows.Forms.TextBox txtMessage;
        public System.Windows.Forms.TextBox txtHead;
        public System.Windows.Forms.Button btnIgnore;

    }
}
