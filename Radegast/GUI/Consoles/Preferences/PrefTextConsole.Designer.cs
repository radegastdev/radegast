namespace Radegast
{
    partial class PrefTextConsole
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkIMTimestamps = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkChatTimestamps = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkIMTimestamps);
            this.groupBox1.Location = new System.Drawing.Point(3, 57);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(338, 48);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Instant messages";
            // 
            // chkIMTimestamps
            // 
            this.chkIMTimestamps.AutoSize = true;
            this.chkIMTimestamps.Location = new System.Drawing.Point(6, 20);
            this.chkIMTimestamps.Name = "chkIMTimestamps";
            this.chkIMTimestamps.Size = new System.Drawing.Size(115, 17);
            this.chkIMTimestamps.TabIndex = 0;
            this.chkIMTimestamps.Text = "Enable timestamps";
            this.chkIMTimestamps.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkChatTimestamps);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(338, 48);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Chat";
            // 
            // chkChatTimestamps
            // 
            this.chkChatTimestamps.AutoSize = true;
            this.chkChatTimestamps.Location = new System.Drawing.Point(6, 20);
            this.chkChatTimestamps.Name = "chkChatTimestamps";
            this.chkChatTimestamps.Size = new System.Drawing.Size(115, 17);
            this.chkChatTimestamps.TabIndex = 0;
            this.chkChatTimestamps.Text = "Enable timestamps";
            this.chkChatTimestamps.UseVisualStyleBackColor = true;
            // 
            // PrefTextConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "PrefTextConsole";
            this.Size = new System.Drawing.Size(344, 300);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkIMTimestamps;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkChatTimestamps;
    }
}
