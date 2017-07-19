namespace Radegast
{
    partial class frmHoverHeight
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
            this.tbHoverHeight = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.txtHoverHeight = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.tbHoverHeight)).BeginInit();
            this.SuspendLayout();
            // 
            // tbHoverHeight
            // 
            this.tbHoverHeight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
                                                                              | System.Windows.Forms.AnchorStyles.Right)));
            this.tbHoverHeight.Location = new System.Drawing.Point(56, 9);
            this.tbHoverHeight.Maximum = 200;
            this.tbHoverHeight.Minimum = -200;
            this.tbHoverHeight.Name = "tbHoverHeight";
            this.tbHoverHeight.Size = new System.Drawing.Size(290, 45);
            this.tbHoverHeight.TabIndex = 0;
            this.tbHoverHeight.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbHoverHeight.Scroll += new System.EventHandler(this.tbHoverHeight_Scroll);
            this.tbHoverHeight.MouseCaptureChanged += new System.EventHandler(this.tbHoverHeight_MouseCaptureChanged);
            this.tbHoverHeight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tbHoverHeight_MouseDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Height";
            // 
            // txtHoverHeight
            // 
            this.txtHoverHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHoverHeight.Location = new System.Drawing.Point(353, 9);
            this.txtHoverHeight.Name = "txtHoverHeight";
            this.txtHoverHeight.Size = new System.Drawing.Size(68, 21);
            this.txtHoverHeight.TabIndex = 2;
            this.txtHoverHeight.Tag = "";
            // 
            // frmHoverHeight
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(433, 43);
            this.Controls.Add(this.txtHoverHeight);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbHoverHeight);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmHoverHeight";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Hover Height";
            ((System.ComponentModel.ISupportInitialize)(this.tbHoverHeight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar tbHoverHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHoverHeight;
    }
}