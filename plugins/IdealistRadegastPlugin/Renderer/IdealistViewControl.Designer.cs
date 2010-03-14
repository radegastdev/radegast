using System.Windows.Forms;

namespace IdealistRadegastPlugin
{
    partial class IdealistViewControl
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
            this.IdealistView = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.IdealistView)).BeginInit();
            this.SuspendLayout();
            // 
            // IdealistView
            // 
            this.IdealistView.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.IdealistView.Image = global::IdealistRadegastPlugin.Properties.Resources.radegast;
            this.IdealistView.Location = new System.Drawing.Point(0, 0);
            this.IdealistView.Name = "IdealistView";
            this.IdealistView.Size = new System.Drawing.Size(893, 835);
            this.IdealistView.TabIndex = 0;
            this.IdealistView.TabStop = false;
            this.IdealistView.Click += new System.EventHandler(this.IdealistView_Click);
            // 
            // IdealistViewControl
            // 
            this.Controls.Add(this.IdealistView);
            this.Name = "IdealistViewControl";
            this.Size = new System.Drawing.Size(893, 835);
            this.Click += new System.EventHandler(this.UIFace_Click);
            ((System.ComponentModel.ISupportInitialize)(this.IdealistView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public Control IdealistView;



    }
}