namespace Radegast
{
    partial class ScriptDialog
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
            this.ignoreBtn = new System.Windows.Forms.Button();
            this.descBox = new System.Windows.Forms.TextBox();
            this.btnsPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // ignoreBtn
            // 
            this.ignoreBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ignoreBtn.Location = new System.Drawing.Point(293, 314);
            this.ignoreBtn.Name = "ignoreBtn";
            this.ignoreBtn.Size = new System.Drawing.Size(75, 23);
            this.ignoreBtn.TabIndex = 0;
            this.ignoreBtn.Text = "&Ignore";
            this.ignoreBtn.UseVisualStyleBackColor = true;
            this.ignoreBtn.Click += new System.EventHandler(this.ignoreBtn_Click);
            // 
            // descBox
            // 
            this.descBox.BackColor = System.Drawing.SystemColors.Control;
            this.descBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.descBox.Location = new System.Drawing.Point(6, 3);
            this.descBox.Multiline = true;
            this.descBox.Name = "descBox";
            this.descBox.Size = new System.Drawing.Size(360, 146);
            this.descBox.TabIndex = 1;
            this.descBox.TabStop = false;
            // 
            // btnsPanel
            // 
            this.btnsPanel.Location = new System.Drawing.Point(6, 155);
            this.btnsPanel.Name = "btnsPanel";
            this.btnsPanel.Size = new System.Drawing.Size(362, 153);
            this.btnsPanel.TabIndex = 2;
            // 
            // ScriptDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ignoreBtn;
            this.ClientSize = new System.Drawing.Size(374, 349);
            this.Controls.Add(this.btnsPanel);
            this.Controls.Add(this.descBox);
            this.Controls.Add(this.ignoreBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScriptDialog";
            this.ShowIcon = false;
            this.Text = "Script Dialog";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ignoreBtn;
        private System.Windows.Forms.TextBox descBox;
        private System.Windows.Forms.Panel btnsPanel;
    }
}