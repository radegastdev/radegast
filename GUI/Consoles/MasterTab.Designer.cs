namespace Radegast
{
    partial class MasterTab
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
            this.primInfoPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.payAmount = new System.Windows.Forms.TextBox();
            this.standBtn = new System.Windows.Forms.Button();
            this.sitBitn = new System.Windows.Forms.Button();
            this.touchBtn = new System.Windows.Forms.Button();
            this.texturesBtn = new System.Windows.Forms.Button();
            this.loadBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.payBtn = new System.Windows.Forms.Button();
            this.objInfoBtn = new System.Windows.Forms.Button();
            this.lastPrimLLUUID = new System.Windows.Forms.TextBox();
            this.lastPrimName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pnlImages = new System.Windows.Forms.Panel();
            this.primInfoPanel.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // primInfoPanel
            // 
            this.primInfoPanel.Controls.Add(this.groupBox1);
            this.primInfoPanel.Controls.Add(this.label1);
            this.primInfoPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.primInfoPanel.Location = new System.Drawing.Point(0, 0);
            this.primInfoPanel.Name = "primInfoPanel";
            this.primInfoPanel.Size = new System.Drawing.Size(529, 162);
            this.primInfoPanel.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.payAmount);
            this.groupBox1.Controls.Add(this.standBtn);
            this.groupBox1.Controls.Add(this.sitBitn);
            this.groupBox1.Controls.Add(this.touchBtn);
            this.groupBox1.Controls.Add(this.texturesBtn);
            this.groupBox1.Controls.Add(this.loadBtn);
            this.groupBox1.Controls.Add(this.saveBtn);
            this.groupBox1.Controls.Add(this.payBtn);
            this.groupBox1.Controls.Add(this.objInfoBtn);
            this.groupBox1.Controls.Add(this.lastPrimLLUUID);
            this.groupBox1.Controls.Add(this.lastPrimName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(6, 16);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(456, 135);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Prim";
            // 
            // payAmount
            // 
            this.payAmount.Enabled = false;
            this.payAmount.Location = new System.Drawing.Point(6, 102);
            this.payAmount.Name = "payAmount";
            this.payAmount.Size = new System.Drawing.Size(77, 20);
            this.payAmount.TabIndex = 7;
            // 
            // standBtn
            // 
            this.standBtn.Location = new System.Drawing.Point(170, 71);
            this.standBtn.Name = "standBtn";
            this.standBtn.Size = new System.Drawing.Size(75, 23);
            this.standBtn.TabIndex = 5;
            this.standBtn.Text = "S&tand";
            this.standBtn.UseVisualStyleBackColor = true;
            this.standBtn.Click += new System.EventHandler(this.standBtn_Click);
            // 
            // sitBitn
            // 
            this.sitBitn.Enabled = false;
            this.sitBitn.Location = new System.Drawing.Point(89, 71);
            this.sitBitn.Name = "sitBitn";
            this.sitBitn.Size = new System.Drawing.Size(75, 23);
            this.sitBitn.TabIndex = 4;
            this.sitBitn.Text = "S&it";
            this.sitBitn.UseVisualStyleBackColor = true;
            this.sitBitn.Click += new System.EventHandler(this.sitBitn_Click);
            // 
            // touchBtn
            // 
            this.touchBtn.Enabled = false;
            this.touchBtn.Location = new System.Drawing.Point(251, 71);
            this.touchBtn.Name = "touchBtn";
            this.touchBtn.Size = new System.Drawing.Size(75, 23);
            this.touchBtn.TabIndex = 6;
            this.touchBtn.Text = "&Touch";
            this.touchBtn.UseVisualStyleBackColor = true;
            this.touchBtn.Click += new System.EventHandler(this.touchBtn_Click);
            // 
            // texturesBtn
            // 
            this.texturesBtn.Enabled = false;
            this.texturesBtn.Location = new System.Drawing.Point(170, 100);
            this.texturesBtn.Name = "texturesBtn";
            this.texturesBtn.Size = new System.Drawing.Size(75, 23);
            this.texturesBtn.TabIndex = 9;
            this.texturesBtn.Text = "Te&xtures";
            this.texturesBtn.UseVisualStyleBackColor = true;
            this.texturesBtn.Click += new System.EventHandler(this.texturesBtn_Click);
            // 
            // loadBtn
            // 
            this.loadBtn.Location = new System.Drawing.Point(332, 99);
            this.loadBtn.Name = "loadBtn";
            this.loadBtn.Size = new System.Drawing.Size(75, 23);
            this.loadBtn.TabIndex = 10;
            this.loadBtn.Text = "&Load...";
            this.loadBtn.UseVisualStyleBackColor = true;
            this.loadBtn.Visible = false;
            this.loadBtn.Click += new System.EventHandler(this.loadBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Enabled = false;
            this.saveBtn.Location = new System.Drawing.Point(251, 99);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 10;
            this.saveBtn.Text = "&Save...";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // payBtn
            // 
            this.payBtn.Enabled = false;
            this.payBtn.Location = new System.Drawing.Point(89, 100);
            this.payBtn.Name = "payBtn";
            this.payBtn.Size = new System.Drawing.Size(75, 23);
            this.payBtn.TabIndex = 8;
            this.payBtn.Text = "&Pay";
            this.payBtn.UseVisualStyleBackColor = true;
            this.payBtn.Click += new System.EventHandler(this.payBtn_Click);
            // 
            // objInfoBtn
            // 
            this.objInfoBtn.Enabled = false;
            this.objInfoBtn.Location = new System.Drawing.Point(8, 71);
            this.objInfoBtn.Name = "objInfoBtn";
            this.objInfoBtn.Size = new System.Drawing.Size(75, 23);
            this.objInfoBtn.TabIndex = 3;
            this.objInfoBtn.Text = "&Object";
            this.objInfoBtn.UseVisualStyleBackColor = true;
            this.objInfoBtn.Click += new System.EventHandler(this.objInfoBtn_Click);
            // 
            // lastPrimLLUUID
            // 
            this.lastPrimLLUUID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lastPrimLLUUID.Location = new System.Drawing.Point(46, 45);
            this.lastPrimLLUUID.Name = "lastPrimLLUUID";
            this.lastPrimLLUUID.ReadOnly = true;
            this.lastPrimLLUUID.Size = new System.Drawing.Size(280, 20);
            this.lastPrimLLUUID.TabIndex = 2;
            // 
            // lastPrimName
            // 
            this.lastPrimName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lastPrimName.Location = new System.Drawing.Point(46, 19);
            this.lastPrimName.Name = "lastPrimName";
            this.lastPrimName.ReadOnly = true;
            this.lastPrimName.Size = new System.Drawing.Size(280, 20);
            this.lastPrimName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "ID";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Name";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Master Selected";
            // 
            // pnlImages
            // 
            this.pnlImages.AutoScroll = true;
            this.pnlImages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImages.Location = new System.Drawing.Point(0, 162);
            this.pnlImages.MaximumSize = new System.Drawing.Size(5000, 0);
            this.pnlImages.Name = "pnlImages";
            this.pnlImages.Size = new System.Drawing.Size(529, 195);
            this.pnlImages.TabIndex = 4;
            // 
            // MasterTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlImages);
            this.Controls.Add(this.primInfoPanel);
            this.Name = "MasterTab";
            this.Size = new System.Drawing.Size(529, 357);
            this.primInfoPanel.ResumeLayout(false);
            this.primInfoPanel.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel primInfoPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox payAmount;
        private System.Windows.Forms.Button standBtn;
        private System.Windows.Forms.Button sitBitn;
        private System.Windows.Forms.Button touchBtn;
        private System.Windows.Forms.Button texturesBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Button payBtn;
        private System.Windows.Forms.Button objInfoBtn;
        private System.Windows.Forms.TextBox lastPrimLLUUID;
        private System.Windows.Forms.TextBox lastPrimName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel pnlImages;
        private System.Windows.Forms.Button loadBtn;
    }
}
