
namespace Radegast.GUI
{
    partial class MfaPrompt
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
            this.tokenBox = new System.Windows.Forms.TextBox();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tokenBox
            // 
            this.tokenBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tokenBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tokenBox.Location = new System.Drawing.Point(14, 42);
            this.tokenBox.Name = "tokenBox";
            this.tokenBox.Size = new System.Drawing.Size(277, 62);
            this.tokenBox.TabIndex = 0;
            this.tokenBox.AcceptsReturn = true;
            // 
            // btnSubmit
            // 
            this.btnSubmit.AccessibleName = "Submit";
            this.btnSubmit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnSubmit.Location = new System.Drawing.Point(0, 110);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(300, 30);
            this.btnSubmit.TabIndex = 1;
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.UseVisualStyleBackColor = true;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(279, 26);
            this.label1.TabIndex = 2;
            this.label1.Text = "The login service has requested MFA authentication. \r\nPlease enter the MFA token " +
    "from your authentication app.";
            // 
            // MfaPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(300, 140);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.tokenBox);
            this.Name = "MfaPrompt";
            this.Text = "Multi-Factor Authentication Requested";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tokenBox;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Label label1;
    }
}