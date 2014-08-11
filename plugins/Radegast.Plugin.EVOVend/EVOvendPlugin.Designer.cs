namespace Radegast.Plugin.EVOVend
{
    partial class EVOvendPlugin
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
            this.numDeliveryInterval = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numStartupDelay = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numDeliveryInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStartupDelay)).BeginInit();
            this.SuspendLayout();
            // 
            // numDeliveryInterval
            // 
            this.numDeliveryInterval.Location = new System.Drawing.Point(396, 19);
            this.numDeliveryInterval.Maximum = new decimal(new int[] {
            3600,
            0,
            0,
            0});
            this.numDeliveryInterval.Name = "numDeliveryInterval";
            this.numDeliveryInterval.Size = new System.Drawing.Size(120, 20);
            this.numDeliveryInterval.TabIndex = 7;
            this.numDeliveryInterval.ValueChanged += new System.EventHandler(this.numDeliveryInterval_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(281, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Delivery Interval [sec]";
            // 
            // numStartupDelay
            // 
            this.numStartupDelay.Location = new System.Drawing.Point(118, 19);
            this.numStartupDelay.Name = "numStartupDelay";
            this.numStartupDelay.Size = new System.Drawing.Size(120, 20);
            this.numStartupDelay.TabIndex = 5;
            this.numStartupDelay.ValueChanged += new System.EventHandler(this.numStartupDelay_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Startup delay [sec]";
            // 
            // EVOvendPlugin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numDeliveryInterval);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numStartupDelay);
            this.Controls.Add(this.label1);
            this.Name = "EVOvendPlugin";
            this.Size = new System.Drawing.Size(538, 138);
            ((System.ComponentModel.ISupportInitialize)(this.numDeliveryInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStartupDelay)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numDeliveryInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numStartupDelay;
        private System.Windows.Forms.Label label1;
    }
}
