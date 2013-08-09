/*
 * Created by SharpDevelop.
 * User: eumario
 * Date: 7/18/2013
 * Time: 4:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Radegast
{
    partial class ExportCollada
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the control.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnExport = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtFileName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.exportableTextures = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.exportablePrims = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textureCount = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.primCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.objectUUID = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.objectName = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbExportTextures = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.texturesPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.cbImageType = new System.Windows.Forms.ComboBox();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Enabled = false;
            this.btnExport.Location = new System.Drawing.Point(540, 377);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 3;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.BtnExportClick);
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.btnBrowse);
            this.groupBox2.Controls.Add(this.txtFileName);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(621, 63);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Export To File";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowse.Location = new System.Drawing.Point(540, 21);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.BtnBrowseClick);
            // 
            // txtFileName
            // 
            this.txtFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFileName.Location = new System.Drawing.Point(8, 24);
            this.txtFileName.Name = "txtFileName";
            this.txtFileName.Size = new System.Drawing.Size(526, 20);
            this.txtFileName.TabIndex = 0;
            this.txtFileName.TextChanged += new System.EventHandler(this.TxtFileNameTextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.Controls.Add(this.exportableTextures);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.exportablePrims);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textureCount);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.primCount);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.objectUUID);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.objectName);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(0, 63);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(275, 129);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Object Information";
            // 
            // exportableTextures
            // 
            this.exportableTextures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exportableTextures.Location = new System.Drawing.Point(94, 96);
            this.exportableTextures.Name = "exportableTextures";
            this.exportableTextures.Size = new System.Drawing.Size(175, 17);
            this.exportableTextures.TabIndex = 11;
            this.exportableTextures.Text = "0";
            this.exportableTextures.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(6, 96);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(82, 17);
            this.label7.TabIndex = 10;
            this.label7.Text = "Exportable:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // exportablePrims
            // 
            this.exportablePrims.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exportablePrims.Location = new System.Drawing.Point(94, 64);
            this.exportablePrims.Name = "exportablePrims";
            this.exportablePrims.Size = new System.Drawing.Size(175, 17);
            this.exportablePrims.TabIndex = 9;
            this.exportablePrims.Text = "label4";
            this.exportablePrims.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(6, 64);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 17);
            this.label6.TabIndex = 8;
            this.label6.Text = "Exportable:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textureCount
            // 
            this.textureCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureCount.Location = new System.Drawing.Point(94, 81);
            this.textureCount.Name = "textureCount";
            this.textureCount.Size = new System.Drawing.Size(175, 15);
            this.textureCount.TabIndex = 7;
            this.textureCount.Text = "label5";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(6, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Texture Count:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // primCount
            // 
            this.primCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.primCount.Location = new System.Drawing.Point(94, 47);
            this.primCount.Name = "primCount";
            this.primCount.Size = new System.Drawing.Size(175, 17);
            this.primCount.TabIndex = 5;
            this.primCount.Text = "label4";
            this.primCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(6, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Prim Count:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // objectUUID
            // 
            this.objectUUID.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectUUID.Location = new System.Drawing.Point(94, 32);
            this.objectUUID.Name = "objectUUID";
            this.objectUUID.Size = new System.Drawing.Size(175, 15);
            this.objectUUID.TabIndex = 3;
            this.objectUUID.Text = "label3";
            this.objectUUID.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(6, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Object UUID:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // objectName
            // 
            this.objectName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.objectName.Location = new System.Drawing.Point(94, 16);
            this.objectName.Name = "objectName";
            this.objectName.Size = new System.Drawing.Size(175, 16);
            this.objectName.TabIndex = 1;
            this.objectName.Text = "label2";
            this.objectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Object Name:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cbExportTextures
            // 
            this.cbExportTextures.AutoSize = true;
            this.cbExportTextures.Location = new System.Drawing.Point(6, 19);
            this.cbExportTextures.Name = "cbExportTextures";
            this.cbExportTextures.Size = new System.Drawing.Size(91, 17);
            this.cbExportTextures.TabIndex = 11;
            this.cbExportTextures.Text = "Enable export";
            this.cbExportTextures.UseVisualStyleBackColor = true;
            this.cbExportTextures.CheckedChanged += new System.EventHandler(this.cbExportTextures_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.txtLog);
            this.groupBox3.Location = new System.Drawing.Point(0, 289);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(621, 82);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Log";
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(3, 16);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(615, 63);
            this.txtLog.TabIndex = 1;
            // 
            // texturesPanel
            // 
            this.texturesPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.texturesPanel.AutoScroll = true;
            this.texturesPanel.Location = new System.Drawing.Point(278, 69);
            this.texturesPanel.Margin = new System.Windows.Forms.Padding(0);
            this.texturesPanel.Name = "texturesPanel";
            this.texturesPanel.Size = new System.Drawing.Size(337, 214);
            this.texturesPanel.TabIndex = 8;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.cbImageType);
            this.groupBox4.Controls.Add(this.cbExportTextures);
            this.groupBox4.Location = new System.Drawing.Point(9, 198);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(266, 85);
            this.groupBox4.TabIndex = 9;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Texture Options";
            // 
            // cbImageType
            // 
            this.cbImageType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbImageType.Enabled = false;
            this.cbImageType.FormattingEnabled = true;
            this.cbImageType.Items.AddRange(new object[] {
            "TGA",
            "PNG",
            "JPG",
            "TGA",
            "J2C",
            "BMP"});
            this.cbImageType.Location = new System.Drawing.Point(6, 42);
            this.cbImageType.Name = "cbImageType";
            this.cbImageType.Size = new System.Drawing.Size(128, 21);
            this.cbImageType.TabIndex = 12;
            // 
            // ExportCollada
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.texturesPanel);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnExport);
            this.Name = "ExportCollada";
            this.Size = new System.Drawing.Size(621, 410);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        public System.Windows.Forms.CheckBox cbExportTextures;
        public System.Windows.Forms.GroupBox groupBox4;
        public System.Windows.Forms.ComboBox cbImageType;
        public System.Windows.Forms.Button btnExport;
        public System.Windows.Forms.TextBox txtLog;
        public System.Windows.Forms.GroupBox groupBox3;
        public System.Windows.Forms.TextBox txtFileName;
        public System.Windows.Forms.Button btnBrowse;
        public System.Windows.Forms.GroupBox groupBox2;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label objectName;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label objectUUID;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label primCount;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label textureCount;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Label exportablePrims;
        public System.Windows.Forms.Label label6;
        public System.Windows.Forms.FlowLayoutPanel texturesPanel;
        public System.Windows.Forms.Label exportableTextures;
        public System.Windows.Forms.Label label7;
	}
}
