namespace Radegast
{
    partial class GroupsDialog
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
            if (disposing && (components != null)) {
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
            this.grpsCombo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.activateBtn = new System.Windows.Forms.Button();
            this.leaveBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.imBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // grpsCombo
            // 
            this.grpsCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.grpsCombo.FormattingEnabled = true;
            this.grpsCombo.Location = new System.Drawing.Point(12, 37);
            this.grpsCombo.Name = "grpsCombo";
            this.grpsCombo.Size = new System.Drawing.Size(266, 21);
            this.grpsCombo.Sorted = true;
            this.grpsCombo.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Select group";
            // 
            // activateBtn
            // 
            this.activateBtn.Location = new System.Drawing.Point(12, 76);
            this.activateBtn.Name = "activateBtn";
            this.activateBtn.Size = new System.Drawing.Size(67, 23);
            this.activateBtn.TabIndex = 2;
            this.activateBtn.Text = "&Activate";
            this.activateBtn.UseVisualStyleBackColor = true;
            this.activateBtn.Click += new System.EventHandler(this.activateBtn_Click);
            // 
            // leaveBtn
            // 
            this.leaveBtn.Location = new System.Drawing.Point(85, 76);
            this.leaveBtn.Name = "leaveBtn";
            this.leaveBtn.Size = new System.Drawing.Size(75, 23);
            this.leaveBtn.TabIndex = 2;
            this.leaveBtn.Text = "&Leave";
            this.leaveBtn.UseVisualStyleBackColor = true;
            this.leaveBtn.Click += new System.EventHandler(this.leaveBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(212, 76);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(66, 23);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "&Close";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // imBtn
            // 
            this.imBtn.Location = new System.Drawing.Point(170, 76);
            this.imBtn.Name = "imBtn";
            this.imBtn.Size = new System.Drawing.Size(36, 23);
            this.imBtn.TabIndex = 2;
            this.imBtn.Text = "&IM";
            this.imBtn.UseVisualStyleBackColor = true;
            this.imBtn.Click += new System.EventHandler(this.imBtn_Click);
            // 
            // GroupsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(300, 138);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.imBtn);
            this.Controls.Add(this.leaveBtn);
            this.Controls.Add(this.activateBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.grpsCombo);
            this.Name = "GroupsDialog";
            this.ShowIcon = false;
            this.Text = "Groups";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox grpsCombo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button activateBtn;
        private System.Windows.Forms.Button leaveBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button imBtn;
    }
}