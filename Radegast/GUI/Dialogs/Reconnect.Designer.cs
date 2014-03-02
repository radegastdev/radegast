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
// $Id: TabsConsole.cs 361 2009-10-24 15:04:57Z latifer $
//
namespace Radegast
{
    partial class frmReconnect
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmReconnect));
            this.label1 = new System.Windows.Forms.Label();
            this.lblAutoReconnect = new System.Windows.Forms.Label();
            this.btnReconnectNow = new System.Windows.Forms.Button();
            this.btnDisable = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tmrReconnect = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(300, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "You have been disconnected, and auto reconnect is enabled.";
            // 
            // lblAutoReconnect
            // 
            this.lblAutoReconnect.AutoSize = true;
            this.lblAutoReconnect.Location = new System.Drawing.Point(12, 32);
            this.lblAutoReconnect.Name = "lblAutoReconnect";
            this.lblAutoReconnect.Size = new System.Drawing.Size(152, 13);
            this.lblAutoReconnect.TabIndex = 1;
            this.lblAutoReconnect.Text = "Auto reconnect in 60 seconds.";
            // 
            // btnReconnectNow
            // 
            this.btnReconnectNow.Location = new System.Drawing.Point(15, 71);
            this.btnReconnectNow.Name = "btnReconnectNow";
            this.btnReconnectNow.Size = new System.Drawing.Size(119, 23);
            this.btnReconnectNow.TabIndex = 2;
            this.btnReconnectNow.Text = "Reconnect now";
            this.btnReconnectNow.UseVisualStyleBackColor = true;
            this.btnReconnectNow.Click += new System.EventHandler(this.btnReconnectNow_Click);
            // 
            // btnDisable
            // 
            this.btnDisable.Location = new System.Drawing.Point(140, 71);
            this.btnDisable.Name = "btnDisable";
            this.btnDisable.Size = new System.Drawing.Size(155, 23);
            this.btnDisable.TabIndex = 3;
            this.btnDisable.Text = "Disable auto reconnect";
            this.btnDisable.UseVisualStyleBackColor = true;
            this.btnDisable.Click += new System.EventHandler(this.btnDisable_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(311, 71);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tmrReconnect
            // 
            this.tmrReconnect.Interval = 1000;
            this.tmrReconnect.Tick += new System.EventHandler(this.tmrReconnect_Tick);
            // 
            // frmReconnect
            // 
            this.AcceptButton = this.btnReconnectNow;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(410, 114);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDisable);
            this.Controls.Add(this.btnReconnectNow);
            this.Controls.Add(this.lblAutoReconnect);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmReconnect";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Radegast - Auto Reconnect";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label lblAutoReconnect;
        public System.Windows.Forms.Button btnReconnectNow;
        public System.Windows.Forms.Button btnDisable;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Timer tmrReconnect;
    }
}