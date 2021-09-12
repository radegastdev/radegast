
namespace Radegast
{
    partial class ntfRegionRestart
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
            this.btnIgnore = new System.Windows.Forms.Button();
            this.btnElsewhere = new System.Windows.Forms.Button();
            this.btnHome = new System.Windows.Forms.Button();
            this.txtCountdownLabel = new System.Windows.Forms.TextBox();
            this.txtHead = new System.Windows.Forms.TextBox();
            this.txtCountdown = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(178, 97);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(75, 23);
            this.btnIgnore.TabIndex = 8;
            this.btnIgnore.Text = "&Ignore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // btnElsewhere
            // 
            this.btnElsewhere.Location = new System.Drawing.Point(93, 97);
            this.btnElsewhere.Name = "btnElsewhere";
            this.btnElsewhere.Size = new System.Drawing.Size(75, 23);
            this.btnElsewhere.TabIndex = 9;
            this.btnElsewhere.Text = "&Elsewhere";
            this.btnElsewhere.UseVisualStyleBackColor = true;
            this.btnElsewhere.Click += new System.EventHandler(this.btnElsewhere_Click);
            // 
            // btnHome
            // 
            this.btnHome.Location = new System.Drawing.Point(9, 97);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(75, 23);
            this.btnHome.TabIndex = 7;
            this.btnHome.Text = "Go &Home";
            this.btnHome.UseVisualStyleBackColor = true;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);
            // 
            // txtCountdownLabel
            // 
            this.txtCountdownLabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCountdownLabel.Location = new System.Drawing.Point(9, 47);
            this.txtCountdownLabel.Multiline = true;
            this.txtCountdownLabel.Name = "txtCountdownLabel";
            this.txtCountdownLabel.ReadOnly = true;
            this.txtCountdownLabel.Size = new System.Drawing.Size(244, 15);
            this.txtCountdownLabel.TabIndex = 11;
            this.txtCountdownLabel.TabStop = false;
            this.txtCountdownLabel.Text = "Time left until restart:";
            this.txtCountdownLabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtHead
            // 
            this.txtHead.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtHead.Location = new System.Drawing.Point(9, 14);
            this.txtHead.Multiline = true;
            this.txtHead.Name = "txtHead";
            this.txtHead.ReadOnly = true;
            this.txtHead.Size = new System.Drawing.Size(244, 34);
            this.txtHead.TabIndex = 10;
            this.txtHead.TabStop = false;
            this.txtHead.Text = "The region you are currently in is about to restart.\r\nIf you remain here, you will be logged out.";
            // 
            // txtCountdown
            // 
            this.txtCountdown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtCountdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCountdown.Location = new System.Drawing.Point(9, 65);
            this.txtCountdown.Multiline = true;
            this.txtCountdown.Name = "txtCountdown";
            this.txtCountdown.ReadOnly = true;
            this.txtCountdown.Size = new System.Drawing.Size(244, 26);
            this.txtCountdown.TabIndex = 12;
            this.txtCountdown.TabStop = false;
            this.txtCountdown.Text = "69";
            this.txtCountdown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ntfRegionRestart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtCountdown);
            this.Controls.Add(this.btnIgnore);
            this.Controls.Add(this.btnElsewhere);
            this.Controls.Add(this.btnHome);
            this.Controls.Add(this.txtCountdownLabel);
            this.Controls.Add(this.txtHead);
            this.Name = "ntfRegionRestart";
            this.Size = new System.Drawing.Size(263, 131);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Button btnIgnore;
        public System.Windows.Forms.Button btnElsewhere;
        public System.Windows.Forms.Button btnHome;
        public System.Windows.Forms.TextBox txtCountdownLabel;
        public System.Windows.Forms.TextBox txtHead;
        public System.Windows.Forms.TextBox txtCountdown;
    }
}
