namespace Radegast
{
    partial class InventoryPanel
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panes = new System.Windows.Forms.TabControl();
            this.tabAll = new System.Windows.Forms.TabPage();
            this.tabRecent = new System.Windows.Forms.TabPage();
            this.tabWorn = new System.Windows.Forms.TabPage();
            this.panel1.SuspendLayout();
            this.panes.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtSearch);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(305, 28);
            this.panel1.TabIndex = 1;
            // 
            // txtSearch
            // 
            this.txtSearch.AccessibleName = "Inventory search input";
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.Location = new System.Drawing.Point(3, 3);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(267, 20);
            this.txtSearch.TabIndex = 0;
            // 
            // panes
            // 
            this.panes.Controls.Add(this.tabAll);
            this.panes.Controls.Add(this.tabRecent);
            this.panes.Controls.Add(this.tabWorn);
            this.panes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panes.Location = new System.Drawing.Point(0, 28);
            this.panes.Name = "panes";
            this.panes.SelectedIndex = 0;
            this.panes.Size = new System.Drawing.Size(305, 348);
            this.panes.TabIndex = 0;
            // 
            // tabAll
            // 
            this.tabAll.Location = new System.Drawing.Point(4, 22);
            this.tabAll.Name = "tabAll";
            this.tabAll.Size = new System.Drawing.Size(297, 322);
            this.tabAll.TabIndex = 0;
            this.tabAll.Text = "All";
            this.tabAll.UseVisualStyleBackColor = true;
            // 
            // tabRecent
            // 
            this.tabRecent.Location = new System.Drawing.Point(4, 22);
            this.tabRecent.Name = "tabRecent";
            this.tabRecent.Size = new System.Drawing.Size(297, 322);
            this.tabRecent.TabIndex = 1;
            this.tabRecent.Text = "Recent";
            this.tabRecent.UseVisualStyleBackColor = true;
            // 
            // tabWorn
            // 
            this.tabWorn.Location = new System.Drawing.Point(4, 22);
            this.tabWorn.Name = "tabWorn";
            this.tabWorn.Size = new System.Drawing.Size(297, 322);
            this.tabWorn.TabIndex = 2;
            this.tabWorn.Text = "Worn";
            this.tabWorn.UseVisualStyleBackColor = true;
            // 
            // InventoryPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panes);
            this.Controls.Add(this.panel1);
            this.Name = "InventoryPanel";
            this.Size = new System.Drawing.Size(305, 376);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panes.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.TextBox txtSearch;
        public System.Windows.Forms.TabControl panes;
        public System.Windows.Forms.TabPage tabAll;
        public System.Windows.Forms.TabPage tabRecent;
        public System.Windows.Forms.TabPage tabWorn;

    }
}
