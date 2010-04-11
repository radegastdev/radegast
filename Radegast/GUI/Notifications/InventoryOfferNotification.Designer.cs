namespace Radegast
{
    partial class ntfInventoryOffer
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
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnDiscard = new System.Windows.Forms.Button();
            this.btnIgnore = new System.Windows.Forms.Button();
            this.txtInfo = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnAccept
            // 
            this.btnAccept.Location = new System.Drawing.Point(15, 95);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(75, 23);
            this.btnAccept.TabIndex = 0;
            this.btnAccept.Text = "Accept";
            this.btnAccept.UseVisualStyleBackColor = true;
            this.btnAccept.Click += new System.EventHandler(this.btnAccept_Click);
            // 
            // btnDiscard
            // 
            this.btnDiscard.Location = new System.Drawing.Point(96, 95);
            this.btnDiscard.Name = "btnDiscard";
            this.btnDiscard.Size = new System.Drawing.Size(75, 23);
            this.btnDiscard.TabIndex = 1;
            this.btnDiscard.Text = "Discard";
            this.btnDiscard.UseVisualStyleBackColor = true;
            this.btnDiscard.Click += new System.EventHandler(this.btnDiscard_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.Location = new System.Drawing.Point(177, 95);
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(75, 23);
            this.btnIgnore.TabIndex = 2;
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.UseVisualStyleBackColor = true;
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // txtInfo
            // 
            this.txtInfo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtInfo.Location = new System.Drawing.Point(15, 3);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.Size = new System.Drawing.Size(237, 86);
            this.txtInfo.TabIndex = 3;
            this.txtInfo.Text = "Cool guy has offered you a cool item.";
            this.txtInfo.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.txtInfo_LinkClicked);
            // 
            // ntfInventoryOffer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.btnIgnore);
            this.Controls.Add(this.btnDiscard);
            this.Controls.Add(this.btnAccept);
            this.Name = "ntfInventoryOffer";
            this.Size = new System.Drawing.Size(263, 131);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Button btnAccept;
        public System.Windows.Forms.Button btnDiscard;
        public System.Windows.Forms.Button btnIgnore;
        public System.Windows.Forms.RichTextBox txtInfo;

    }
}
