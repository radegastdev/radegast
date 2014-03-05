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
    partial class frmPay
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
            this.lblResident = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnPay = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnFastPay1 = new System.Windows.Forms.Button();
            this.btnFastPay2 = new System.Windows.Forms.Button();
            this.btnFastPay3 = new System.Windows.Forms.Button();
            this.btnFastPay4 = new System.Windows.Forms.Button();
            this.txtAmount = new System.Windows.Forms.TextBox();
            this.lblObject = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblResident
            // 
            this.lblResident.AutoSize = true;
            this.lblResident.Location = new System.Drawing.Point(12, 9);
            this.lblResident.Name = "lblResident";
            this.lblResident.Size = new System.Drawing.Size(74, 13);
            this.lblResident.TabIndex = 0;
            this.lblResident.Text = "Pay resident: ";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(202, 107);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(54, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnPay
            // 
            this.btnPay.Enabled = false;
            this.btnPay.Location = new System.Drawing.Point(153, 107);
            this.btnPay.Name = "btnPay";
            this.btnPay.Size = new System.Drawing.Size(43, 23);
            this.btnPay.TabIndex = 2;
            this.btnPay.Text = "Pay";
            this.btnPay.UseVisualStyleBackColor = true;
            this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 112);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Amount";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 57);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Fast pay";
            // 
            // btnFastPay1
            // 
            this.btnFastPay1.Location = new System.Drawing.Point(69, 52);
            this.btnFastPay1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.btnFastPay1.Name = "btnFastPay1";
            this.btnFastPay1.Size = new System.Drawing.Size(92, 23);
            this.btnFastPay1.TabIndex = 4;
            this.btnFastPay1.Text = "L$1";
            this.btnFastPay1.UseVisualStyleBackColor = true;
            // 
            // btnFastPay2
            // 
            this.btnFastPay2.Location = new System.Drawing.Point(164, 52);
            this.btnFastPay2.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.btnFastPay2.Name = "btnFastPay2";
            this.btnFastPay2.Size = new System.Drawing.Size(92, 23);
            this.btnFastPay2.TabIndex = 5;
            this.btnFastPay2.Text = "L$5";
            this.btnFastPay2.UseVisualStyleBackColor = true;
            // 
            // btnFastPay3
            // 
            this.btnFastPay3.Location = new System.Drawing.Point(69, 81);
            this.btnFastPay3.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.btnFastPay3.Name = "btnFastPay3";
            this.btnFastPay3.Size = new System.Drawing.Size(92, 23);
            this.btnFastPay3.TabIndex = 6;
            this.btnFastPay3.Text = "L$10";
            this.btnFastPay3.UseVisualStyleBackColor = true;
            // 
            // btnFastPay4
            // 
            this.btnFastPay4.Location = new System.Drawing.Point(164, 81);
            this.btnFastPay4.Margin = new System.Windows.Forms.Padding(0, 3, 3, 0);
            this.btnFastPay4.Name = "btnFastPay4";
            this.btnFastPay4.Size = new System.Drawing.Size(92, 23);
            this.btnFastPay4.TabIndex = 7;
            this.btnFastPay4.Text = "L$20";
            this.btnFastPay4.UseVisualStyleBackColor = true;
            // 
            // txtAmount
            // 
            this.txtAmount.Location = new System.Drawing.Point(69, 107);
            this.txtAmount.Name = "txtAmount";
            this.txtAmount.Size = new System.Drawing.Size(78, 21);
            this.txtAmount.TabIndex = 1;
            this.txtAmount.TextChanged += new System.EventHandler(this.txtAmount_TextChanged);
            this.txtAmount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAmount_KeyPress);
            // 
            // lblObject
            // 
            this.lblObject.AutoSize = true;
            this.lblObject.Location = new System.Drawing.Point(12, 30);
            this.lblObject.Name = "lblObject";
            this.lblObject.Size = new System.Drawing.Size(58, 13);
            this.lblObject.TabIndex = 0;
            this.lblObject.Text = "Via object:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.BackColor = System.Drawing.SystemColors.Highlight;
            this.lblStatus.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.lblStatus.Location = new System.Drawing.Point(12, 134);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(91, 13);
            this.lblStatus.TabIndex = 8;
            this.lblStatus.Text = "Insufficient funds";
            this.lblStatus.Visible = false;
            // 
            // frmPay
            // 
            this.AcceptButton = this.btnPay;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(268, 153);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtAmount);
            this.Controls.Add(this.btnFastPay4);
            this.Controls.Add(this.btnFastPay3);
            this.Controls.Add(this.btnFastPay2);
            this.Controls.Add(this.btnFastPay1);
            this.Controls.Add(this.btnPay);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblObject);
            this.Controls.Add(this.lblResident);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmPay";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Pay";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label lblResident;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnPay;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button btnFastPay1;
        public System.Windows.Forms.Button btnFastPay2;
        public System.Windows.Forms.Button btnFastPay3;
        public System.Windows.Forms.Button btnFastPay4;
        public System.Windows.Forms.TextBox txtAmount;
        public System.Windows.Forms.Label lblObject;
        public System.Windows.Forms.Label lblStatus;

    }
}