namespace Radegast
{
    partial class PrefGeneralConsole
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
            this.rdoSystemStyle = new System.Windows.Forms.RadioButton();
            this.rdoOfficeStyle = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdoSystemStyle
            // 
            this.rdoSystemStyle.AutoSize = true;
            this.rdoSystemStyle.Location = new System.Drawing.Point(6, 20);
            this.rdoSystemStyle.Name = "rdoSystemStyle";
            this.rdoSystemStyle.Size = new System.Drawing.Size(60, 17);
            this.rdoSystemStyle.TabIndex = 1;
            this.rdoSystemStyle.TabStop = true;
            this.rdoSystemStyle.Text = "System";
            this.rdoSystemStyle.UseVisualStyleBackColor = true;
            // 
            // rdoOfficeStyle
            // 
            this.rdoOfficeStyle.AutoSize = true;
            this.rdoOfficeStyle.Location = new System.Drawing.Point(72, 20);
            this.rdoOfficeStyle.Name = "rdoOfficeStyle";
            this.rdoOfficeStyle.Size = new System.Drawing.Size(81, 17);
            this.rdoOfficeStyle.TabIndex = 2;
            this.rdoOfficeStyle.TabStop = true;
            this.rdoOfficeStyle.Text = "Office 2003";
            this.rdoOfficeStyle.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rdoSystemStyle);
            this.groupBox1.Controls.Add(this.rdoOfficeStyle);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(338, 48);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Interface style";
            // 
            // PrefGeneralConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PrefGeneralConsole";
            this.Size = new System.Drawing.Size(344, 300);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rdoSystemStyle;
        private System.Windows.Forms.RadioButton rdoOfficeStyle;
        private System.Windows.Forms.GroupBox groupBox1;

    }
}
