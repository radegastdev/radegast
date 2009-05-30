namespace Radegast
{
    partial class frmObjects
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmObjects));
            this.lbxPrims = new System.Windows.Forms.ListBox();
            this.gbxInworld = new System.Windows.Forms.GroupBox();
            this.btnTouch = new System.Windows.Forms.Button();
            this.btnSitOn = new System.Windows.Forms.Button();
            this.btnPointAt = new System.Windows.Forms.Button();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.gbxInworld.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbxPrims
            // 
            this.lbxPrims.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbxPrims.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lbxPrims.FormattingEnabled = true;
            this.lbxPrims.IntegralHeight = false;
            this.lbxPrims.ItemHeight = 18;
            this.lbxPrims.Location = new System.Drawing.Point(12, 39);
            this.lbxPrims.Name = "lbxPrims";
            this.lbxPrims.Size = new System.Drawing.Size(362, 375);
            this.lbxPrims.Sorted = true;
            this.lbxPrims.TabIndex = 0;
            this.lbxPrims.Visible = false;
            this.lbxPrims.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.lbxPrims_DrawItem);
            this.lbxPrims.SelectedIndexChanged += new System.EventHandler(this.lbxPrims_SelectedIndexChanged);
            // 
            // gbxInworld
            // 
            this.gbxInworld.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gbxInworld.Controls.Add(this.btnTouch);
            this.gbxInworld.Controls.Add(this.btnSitOn);
            this.gbxInworld.Controls.Add(this.btnPointAt);
            this.gbxInworld.Enabled = false;
            this.gbxInworld.Location = new System.Drawing.Point(380, 10);
            this.gbxInworld.Name = "gbxInworld";
            this.gbxInworld.Size = new System.Drawing.Size(100, 110);
            this.gbxInworld.TabIndex = 2;
            this.gbxInworld.TabStop = false;
            this.gbxInworld.Text = "In-world";
            // 
            // btnTouch
            // 
            this.btnTouch.Location = new System.Drawing.Point(6, 78);
            this.btnTouch.Name = "btnTouch";
            this.btnTouch.Size = new System.Drawing.Size(88, 23);
            this.btnTouch.TabIndex = 2;
            this.btnTouch.Text = "Touch/Click";
            this.btnTouch.UseVisualStyleBackColor = true;
            this.btnTouch.Click += new System.EventHandler(this.btnTouch_Click);
            // 
            // btnSitOn
            // 
            this.btnSitOn.Location = new System.Drawing.Point(6, 49);
            this.btnSitOn.Name = "btnSitOn";
            this.btnSitOn.Size = new System.Drawing.Size(88, 23);
            this.btnSitOn.TabIndex = 1;
            this.btnSitOn.Text = "Sit On";
            this.btnSitOn.UseVisualStyleBackColor = true;
            this.btnSitOn.Click += new System.EventHandler(this.btnSitOn_Click);
            // 
            // btnPointAt
            // 
            this.btnPointAt.Location = new System.Drawing.Point(6, 20);
            this.btnPointAt.Name = "btnPointAt";
            this.btnPointAt.Size = new System.Drawing.Size(88, 23);
            this.btnPointAt.TabIndex = 0;
            this.btnPointAt.Text = "Point At";
            this.btnPointAt.UseVisualStyleBackColor = true;
            this.btnPointAt.Click += new System.EventHandler(this.btnPointAt_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Enabled = false;
            this.txtSearch.Location = new System.Drawing.Point(62, 12);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(246, 21);
            this.txtSearch.TabIndex = 4;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(12, 36);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(103, 13);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "Getting objects...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Search:";
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(314, 10);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(60, 23);
            this.btnClear.TabIndex = 8;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.Location = new System.Drawing.Point(380, 391);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(100, 23);
            this.btnClose.TabIndex = 9;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmObjects
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(492, 426);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.txtSearch);
            this.Controls.Add(this.gbxInworld);
            this.Controls.Add(this.lbxPrims);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmObjects";
            this.Text = "Objects - SLeek";
            this.Load += new System.EventHandler(this.frmObjects_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmObjects_FormClosing);
            this.gbxInworld.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxPrims;
        private System.Windows.Forms.GroupBox gbxInworld;
        private System.Windows.Forms.Button btnSitOn;
        private System.Windows.Forms.Button btnPointAt;
        private System.Windows.Forms.Button btnTouch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnClose;
    }
}