namespace Radegast
{
    partial class PluginsTab
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
            this.lvwPlugins = new Radegast.ListViewNoFlicker();
            this.chName = new System.Windows.Forms.ColumnHeader();
            this.chDesciption = new System.Windows.Forms.ColumnHeader();
            this.chVersion = new System.Windows.Forms.ColumnHeader();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnUnload = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnReload = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lvwPlugins
            // 
            this.lvwPlugins.AccessibleName = "Plugins";
            this.lvwPlugins.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lvwPlugins.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chName,
            this.chDesciption,
            this.chVersion});
            this.lvwPlugins.FullRowSelect = true;
            this.lvwPlugins.GridLines = true;
            this.lvwPlugins.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwPlugins.HideSelection = false;
            this.lvwPlugins.Location = new System.Drawing.Point(16, 20);
            this.lvwPlugins.Name = "lvwPlugins";
            this.lvwPlugins.Size = new System.Drawing.Size(443, 334);
            this.lvwPlugins.TabIndex = 0;
            this.lvwPlugins.UseCompatibleStateImageBehavior = false;
            this.lvwPlugins.View = System.Windows.Forms.View.Details;
            // 
            // chName
            // 
            this.chName.Text = "Name";
            // 
            // chDesciption
            // 
            this.chDesciption.Text = "Description";
            // 
            // chVersion
            // 
            this.chVersion.Text = "Version";
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(470, 20);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(93, 23);
            this.btnLoad.TabIndex = 1;
            this.btnLoad.Text = "&Load...";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnUnload
            // 
            this.btnUnload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUnload.Location = new System.Drawing.Point(470, 49);
            this.btnUnload.Name = "btnUnload";
            this.btnUnload.Size = new System.Drawing.Size(93, 23);
            this.btnUnload.TabIndex = 2;
            this.btnUnload.Text = "&Unload";
            this.btnUnload.UseVisualStyleBackColor = true;
            this.btnUnload.Click += new System.EventHandler(this.btnUnload_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(470, 107);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(93, 23);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnReload
            // 
            this.btnReload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReload.Location = new System.Drawing.Point(470, 78);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(93, 23);
            this.btnReload.TabIndex = 3;
            this.btnReload.Text = "&Reload";
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // PluginsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnUnload);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.lvwPlugins);
            this.Name = "PluginsTab";
            this.Size = new System.Drawing.Size(576, 385);
            this.SizeChanged += new System.EventHandler(this.PluginsTab_SizeChanged);
            this.ResumeLayout(false);

        }

        #endregion

        private ListViewNoFlicker lvwPlugins;
        private System.Windows.Forms.ColumnHeader chName;
        private System.Windows.Forms.ColumnHeader chDesciption;
        private System.Windows.Forms.ColumnHeader chVersion;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Button btnUnload;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnReload;
    }
}
