namespace Radegast
{
    partial class PermissionsDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.descBox = new System.Windows.Forms.TextBox();
            this.yesBtn = new System.Windows.Forms.Button();
            this.noBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // descBox
            // 
            this.descBox.BackColor = System.Drawing.SystemColors.Control;
            this.descBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.descBox.Location = new System.Drawing.Point(12, 12);
            this.descBox.Multiline = true;
            this.descBox.Name = "descBox";
            this.descBox.Size = new System.Drawing.Size(268, 88);
            this.descBox.TabIndex = 99;
            this.descBox.TabStop = false;
            // 
            // yesBtn
            // 
            this.yesBtn.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.yesBtn.Location = new System.Drawing.Point(53, 116);
            this.yesBtn.Name = "yesBtn";
            this.yesBtn.Size = new System.Drawing.Size(75, 23);
            this.yesBtn.TabIndex = 1;
            this.yesBtn.Text = "&Yes";
            this.yesBtn.UseVisualStyleBackColor = true;
            this.yesBtn.Click += new System.EventHandler(this.yesBtn_Click);
            // 
            // noBtn
            // 
            this.noBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.noBtn.Location = new System.Drawing.Point(159, 116);
            this.noBtn.Name = "noBtn";
            this.noBtn.Size = new System.Drawing.Size(75, 23);
            this.noBtn.TabIndex = 2;
            this.noBtn.Text = "&No";
            this.noBtn.UseVisualStyleBackColor = true;
            this.noBtn.Click += new System.EventHandler(this.noBtn_Click);
            // 
            // PermissionsDialog
            // 
            this.AcceptButton = this.noBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.noBtn;
            this.ClientSize = new System.Drawing.Size(292, 154);
            this.Controls.Add(this.noBtn);
            this.Controls.Add(this.yesBtn);
            this.Controls.Add(this.descBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PermissionsDialog";
            this.ShowIcon = false;
            this.Text = "Permissions Request";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox descBox;
        private System.Windows.Forms.Button yesBtn;
        private System.Windows.Forms.Button noBtn;

    }
}