namespace Radegast
{
    partial class frmDebugLog
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
            this.tabLogs = new System.Windows.Forms.TabControl();
            this.tpgInfo = new System.Windows.Forms.TabPage();
            this.rtbInfo = new System.Windows.Forms.RichTextBox();
            this.tpgWarning = new System.Windows.Forms.TabPage();
            this.rtbWarning = new System.Windows.Forms.RichTextBox();
            this.tpgError = new System.Windows.Forms.TabPage();
            this.rtbError = new System.Windows.Forms.RichTextBox();
            this.tpgDebug = new System.Windows.Forms.TabPage();
            this.rtbDebug = new System.Windows.Forms.RichTextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.tabLogs.SuspendLayout();
            this.tpgInfo.SuspendLayout();
            this.tpgWarning.SuspendLayout();
            this.tpgError.SuspendLayout();
            this.tpgDebug.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabLogs
            // 
            this.tabLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabLogs.Controls.Add(this.tpgInfo);
            this.tabLogs.Controls.Add(this.tpgWarning);
            this.tabLogs.Controls.Add(this.tpgError);
            this.tabLogs.Controls.Add(this.tpgDebug);
            this.tabLogs.Location = new System.Drawing.Point(12, 12);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.SelectedIndex = 0;
            this.tabLogs.Size = new System.Drawing.Size(548, 385);
            this.tabLogs.TabIndex = 0;
            // 
            // tpgInfo
            // 
            this.tpgInfo.Controls.Add(this.rtbInfo);
            this.tpgInfo.Location = new System.Drawing.Point(4, 22);
            this.tpgInfo.Name = "tpgInfo";
            this.tpgInfo.Padding = new System.Windows.Forms.Padding(3);
            this.tpgInfo.Size = new System.Drawing.Size(540, 359);
            this.tpgInfo.TabIndex = 0;
            this.tpgInfo.Text = "Info";
            this.tpgInfo.UseVisualStyleBackColor = true;
            // 
            // rtbInfo
            // 
            this.rtbInfo.BackColor = System.Drawing.Color.White;
            this.rtbInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbInfo.Location = new System.Drawing.Point(3, 3);
            this.rtbInfo.Name = "rtbInfo";
            this.rtbInfo.ReadOnly = true;
            this.rtbInfo.Size = new System.Drawing.Size(534, 353);
            this.rtbInfo.TabIndex = 0;
            this.rtbInfo.Text = "";
            this.rtbInfo.WordWrap = false;
            // 
            // tpgWarning
            // 
            this.tpgWarning.Controls.Add(this.rtbWarning);
            this.tpgWarning.Location = new System.Drawing.Point(4, 22);
            this.tpgWarning.Name = "tpgWarning";
            this.tpgWarning.Padding = new System.Windows.Forms.Padding(3);
            this.tpgWarning.Size = new System.Drawing.Size(540, 359);
            this.tpgWarning.TabIndex = 1;
            this.tpgWarning.Text = "Warning";
            this.tpgWarning.UseVisualStyleBackColor = true;
            // 
            // rtbWarning
            // 
            this.rtbWarning.BackColor = System.Drawing.Color.White;
            this.rtbWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbWarning.Location = new System.Drawing.Point(3, 3);
            this.rtbWarning.Name = "rtbWarning";
            this.rtbWarning.ReadOnly = true;
            this.rtbWarning.Size = new System.Drawing.Size(534, 353);
            this.rtbWarning.TabIndex = 1;
            this.rtbWarning.Text = "";
            this.rtbWarning.WordWrap = false;
            // 
            // tpgError
            // 
            this.tpgError.Controls.Add(this.rtbError);
            this.tpgError.Location = new System.Drawing.Point(4, 22);
            this.tpgError.Name = "tpgError";
            this.tpgError.Padding = new System.Windows.Forms.Padding(3);
            this.tpgError.Size = new System.Drawing.Size(540, 359);
            this.tpgError.TabIndex = 2;
            this.tpgError.Text = "Error";
            this.tpgError.UseVisualStyleBackColor = true;
            // 
            // rtbError
            // 
            this.rtbError.BackColor = System.Drawing.Color.White;
            this.rtbError.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbError.Location = new System.Drawing.Point(3, 3);
            this.rtbError.Name = "rtbError";
            this.rtbError.ReadOnly = true;
            this.rtbError.Size = new System.Drawing.Size(534, 353);
            this.rtbError.TabIndex = 1;
            this.rtbError.Text = "";
            this.rtbError.WordWrap = false;
            // 
            // tpgDebug
            // 
            this.tpgDebug.Controls.Add(this.rtbDebug);
            this.tpgDebug.Location = new System.Drawing.Point(4, 22);
            this.tpgDebug.Name = "tpgDebug";
            this.tpgDebug.Padding = new System.Windows.Forms.Padding(3);
            this.tpgDebug.Size = new System.Drawing.Size(540, 359);
            this.tpgDebug.TabIndex = 3;
            this.tpgDebug.Text = "Debug";
            this.tpgDebug.UseVisualStyleBackColor = true;
            // 
            // rtbDebug
            // 
            this.rtbDebug.BackColor = System.Drawing.Color.White;
            this.rtbDebug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbDebug.Location = new System.Drawing.Point(3, 3);
            this.rtbDebug.Name = "rtbDebug";
            this.rtbDebug.ReadOnly = true;
            this.rtbDebug.Size = new System.Drawing.Size(534, 353);
            this.rtbDebug.TabIndex = 1;
            this.rtbDebug.Text = "";
            this.rtbDebug.WordWrap = false;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(485, 403);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmDebugLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 438);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tabLogs);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "frmDebugLog";
            this.Text = "Debug Log - SLeek";
            this.Shown += new System.EventHandler(this.frmDebugLog_Shown);
            this.tabLogs.ResumeLayout(false);
            this.tpgInfo.ResumeLayout(false);
            this.tpgWarning.ResumeLayout(false);
            this.tpgError.ResumeLayout(false);
            this.tpgDebug.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabLogs;
        private System.Windows.Forms.TabPage tpgInfo;
        private System.Windows.Forms.TabPage tpgWarning;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TabPage tpgError;
        private System.Windows.Forms.TabPage tpgDebug;
        private System.Windows.Forms.RichTextBox rtbInfo;
        private System.Windows.Forms.RichTextBox rtbWarning;
        private System.Windows.Forms.RichTextBox rtbError;
        private System.Windows.Forms.RichTextBox rtbDebug;
    }
}