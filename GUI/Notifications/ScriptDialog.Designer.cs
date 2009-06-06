namespace Radegast
{
    partial class ntfScriptDialog
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
            this.btnsPanel = new System.Windows.Forms.Panel();
            this.descBox = new System.Windows.Forms.TextBox();
            this.ignoreBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnsPanel
            // 
            this.btnsPanel.Location = new System.Drawing.Point(0, 123);
            this.btnsPanel.Name = "btnsPanel";
            this.btnsPanel.Size = new System.Drawing.Size(287, 112);
            this.btnsPanel.TabIndex = 5;
            // 
            // descBox
            // 
            this.descBox.BackColor = System.Drawing.SystemColors.Control;
            this.descBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.descBox.Location = new System.Drawing.Point(3, 3);
            this.descBox.Multiline = true;
            this.descBox.Name = "descBox";
            this.descBox.ReadOnly = true;
            this.descBox.Size = new System.Drawing.Size(284, 114);
            this.descBox.TabIndex = 4;
            this.descBox.TabStop = false;
            // 
            // ignoreBtn
            // 
            this.ignoreBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ignoreBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ignoreBtn.Location = new System.Drawing.Point(212, 231);
            this.ignoreBtn.Name = "ignoreBtn";
            this.ignoreBtn.Size = new System.Drawing.Size(75, 23);
            this.ignoreBtn.TabIndex = 3;
            this.ignoreBtn.Text = "&Ignore";
            this.ignoreBtn.UseVisualStyleBackColor = true;
            this.ignoreBtn.Click += new System.EventHandler(this.ignoreBtn_Click);
            // 
            // ntfScriptDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ignoreBtn);
            this.Controls.Add(this.btnsPanel);
            this.Controls.Add(this.descBox);
            this.Name = "ntfScriptDialog";
            this.Size = new System.Drawing.Size(289, 254);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel btnsPanel;
        private System.Windows.Forms.TextBox descBox;
        private System.Windows.Forms.Button ignoreBtn;
    }
}
