namespace DemoTab
{
    partial class DemoTab
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
            this.btnSaySomething = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtChat = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnSaySomething
            // 
            this.btnSaySomething.Location = new System.Drawing.Point(18, 23);
            this.btnSaySomething.Name = "btnSaySomething";
            this.btnSaySomething.Size = new System.Drawing.Size(159, 23);
            this.btnSaySomething.TabIndex = 0;
            this.btnSaySomething.Text = "Say Something";
            this.btnSaySomething.UseVisualStyleBackColor = true;
            this.btnSaySomething.Click += new System.EventHandler(this.btnSaySomething_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Last chat line:";
            // 
            // txtChat
            // 
            this.txtChat.Location = new System.Drawing.Point(94, 59);
            this.txtChat.Name = "txtChat";
            this.txtChat.Size = new System.Drawing.Size(247, 20);
            this.txtChat.TabIndex = 2;
            // 
            // DemoTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtChat);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnSaySomething);
            this.Name = "DemoTab";
            this.Size = new System.Drawing.Size(458, 290);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSaySomething;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtChat;
    }
}
