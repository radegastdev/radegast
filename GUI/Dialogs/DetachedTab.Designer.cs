namespace Radegast
{
    partial class frmDetachedTab
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDetachedTab));
            this.tstMain = new System.Windows.Forms.ToolStrip();
            this.tbtnReattach = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tlblTyping = new System.Windows.Forms.ToolStripLabel();
            this.tstMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tstMain
            // 
            this.tstMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tstMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnReattach,
            this.toolStripSeparator1,
            this.tlblTyping});
            this.tstMain.Location = new System.Drawing.Point(0, 0);
            this.tstMain.Name = "tstMain";
            this.tstMain.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tstMain.Size = new System.Drawing.Size(508, 25);
            this.tstMain.TabIndex = 0;
            this.tstMain.Text = "toolStrip1";
            // 
            // tbtnReattach
            // 
            this.tbtnReattach.AutoToolTip = false;
            this.tbtnReattach.Image = global::Radegast.Properties.Resources.arrow_up_16;
            this.tbtnReattach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnReattach.Name = "tbtnReattach";
            this.tbtnReattach.Size = new System.Drawing.Size(71, 22);
            this.tbtnReattach.Text = "Reattach";
            this.tbtnReattach.Click += new System.EventHandler(this.tbtnReattach_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tlblTyping
            // 
            this.tlblTyping.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
            this.tlblTyping.Name = "tlblTyping";
            this.tlblTyping.Size = new System.Drawing.Size(106, 22);
            this.tlblTyping.Text = "Person is typing...";
            this.tlblTyping.Visible = false;
            // 
            // frmDetachedTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 362);
            this.Controls.Add(this.tstMain);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmDetachedTab";
            this.Text = "DetachedTab";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmDetachedTab_FormClosing);
            this.tstMain.ResumeLayout(false);
            this.tstMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip tstMain;
        private System.Windows.Forms.ToolStripButton tbtnReattach;
        private System.Windows.Forms.ToolStripLabel tlblTyping;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    }
}