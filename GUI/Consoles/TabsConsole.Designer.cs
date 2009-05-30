namespace Radegast
{
    partial class TabsConsole
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
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tstTabs = new System.Windows.Forms.ToolStrip();
            this.tbtnCloseTab = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnTabOptions = new System.Windows.Forms.ToolStripDropDownButton();
            this.tmnuMergeWith = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tmnuDetachTab = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tstTabs.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(623, 436);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // toolStripContainer1.LeftToolStripPanel
            // 
            this.toolStripContainer1.LeftToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            // 
            // toolStripContainer1.RightToolStripPanel
            // 
            this.toolStripContainer1.RightToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripContainer1.Size = new System.Drawing.Size(623, 461);
            this.toolStripContainer1.TabIndex = 9;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.tstTabs);
            this.toolStripContainer1.TopToolStripPanel.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // tstTabs
            // 
            this.tstTabs.Dock = System.Windows.Forms.DockStyle.None;
            this.tstTabs.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnCloseTab,
            this.toolStripSeparator1,
            this.tbtnTabOptions});
            this.tstTabs.Location = new System.Drawing.Point(0, 0);
            this.tstTabs.Name = "tstTabs";
            this.tstTabs.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tstTabs.Size = new System.Drawing.Size(623, 25);
            this.tstTabs.Stretch = true;
            this.tstTabs.TabIndex = 0;
            this.tstTabs.Text = "toolStrip1";
            // 
            // tbtnCloseTab
            // 
            this.tbtnCloseTab.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tbtnCloseTab.AutoToolTip = false;
            this.tbtnCloseTab.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnCloseTab.Enabled = false;
            this.tbtnCloseTab.Image = global::Radegast.Properties.Resources.delete_16;
            this.tbtnCloseTab.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnCloseTab.Name = "tbtnCloseTab";
            this.tbtnCloseTab.Size = new System.Drawing.Size(23, 22);
            this.tbtnCloseTab.ToolTipText = "Close Tab";
            this.tbtnCloseTab.Click += new System.EventHandler(this.tbtnCloseTab_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tbtnTabOptions
            // 
            this.tbtnTabOptions.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tbtnTabOptions.AutoToolTip = false;
            this.tbtnTabOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnTabOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tmnuMergeWith,
            this.toolStripMenuItem1,
            this.tmnuDetachTab});
            this.tbtnTabOptions.Image = global::Radegast.Properties.Resources.applications_16;
            this.tbtnTabOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnTabOptions.Name = "tbtnTabOptions";
            this.tbtnTabOptions.Size = new System.Drawing.Size(29, 22);
            this.tbtnTabOptions.Click += new System.EventHandler(this.tbtnTabOptions_Click);
            // 
            // tmnuMergeWith
            // 
            this.tmnuMergeWith.Name = "tmnuMergeWith";
            this.tmnuMergeWith.Size = new System.Drawing.Size(140, 22);
            this.tmnuMergeWith.Text = "Merge With";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(137, 6);
            // 
            // tmnuDetachTab
            // 
            this.tmnuDetachTab.Image = global::Radegast.Properties.Resources.copy_16;
            this.tmnuDetachTab.Name = "tmnuDetachTab";
            this.tmnuDetachTab.Size = new System.Drawing.Size(140, 22);
            this.tmnuDetachTab.Text = "Detach Tab";
            this.tmnuDetachTab.Click += new System.EventHandler(this.tmnuDetachTab_Click);
            // 
            // TabsConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStripContainer1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TabsConsole";
            this.Size = new System.Drawing.Size(623, 461);
            this.Load += new System.EventHandler(this.TabsConsole_Load);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tstTabs.ResumeLayout(false);
            this.tstTabs.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip tstTabs;
        private System.Windows.Forms.ToolStripDropDownButton tbtnTabOptions;
        private System.Windows.Forms.ToolStripMenuItem tmnuMergeWith;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tmnuDetachTab;
        private System.Windows.Forms.ToolStripButton tbtnCloseTab;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}
