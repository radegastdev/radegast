namespace RadegastSpeech.GUI
{
    partial class VoiceAssignment
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
            this.label1 = new System.Windows.Forms.Label();
            this.avName = new System.Windows.Forms.TextBox();
            this.voiceList = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.saveBtn = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.gender = new System.Windows.Forms.TextBox();
            this.age = new System.Windows.Forms.TextBox();
            this.pitchSelector = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.rateSelector = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Avatar";
            // 
            // avName
            // 
            this.avName.Enabled = false;
            this.avName.Location = new System.Drawing.Point(51, 4);
            this.avName.Name = "avName";
            this.avName.ReadOnly = true;
            this.avName.Size = new System.Drawing.Size(212, 20);
            this.avName.TabIndex = 1;
            // 
            // voiceList
            // 
            this.voiceList.FormattingEnabled = true;
            this.voiceList.Location = new System.Drawing.Point(11, 56);
            this.voiceList.MultiColumn = true;
            this.voiceList.Name = "voiceList";
            this.voiceList.Size = new System.Drawing.Size(227, 82);
            this.voiceList.TabIndex = 2;
            this.voiceList.SelectedIndexChanged += new System.EventHandler(this.voiceList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Available voices";
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(278, 174);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(63, 23);
            this.cancelBtn.TabIndex = 4;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // saveBtn
            // 
            this.saveBtn.Location = new System.Drawing.Point(188, 174);
            this.saveBtn.Name = "saveBtn";
            this.saveBtn.Size = new System.Drawing.Size(75, 23);
            this.saveBtn.TabIndex = 5;
            this.saveBtn.Text = "Save";
            this.saveBtn.UseVisualStyleBackColor = true;
            this.saveBtn.Click += new System.EventHandler(this.saveBtn_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(258, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Gender";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(260, 84);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Age";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gender
            // 
            this.gender.Location = new System.Drawing.Point(306, 53);
            this.gender.Name = "gender";
            this.gender.ReadOnly = true;
            this.gender.Size = new System.Drawing.Size(24, 20);
            this.gender.TabIndex = 8;
            // 
            // age
            // 
            this.age.Location = new System.Drawing.Point(306, 84);
            this.age.Name = "age";
            this.age.ReadOnly = true;
            this.age.Size = new System.Drawing.Size(24, 20);
            this.age.TabIndex = 9;
            // 
            // pitchSelector
            // 
            this.pitchSelector.FormattingEnabled = true;
            this.pitchSelector.Items.AddRange(new object[] {
            "Low",
            "Normal",
            "High"});
            this.pitchSelector.Location = new System.Drawing.Point(53, 147);
            this.pitchSelector.Name = "pitchSelector";
            this.pitchSelector.Size = new System.Drawing.Size(75, 21);
            this.pitchSelector.TabIndex = 10;
            this.pitchSelector.SelectedIndexChanged += new System.EventHandler(this.pitchSelector_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 150);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Pitch";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(134, 150);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Rate";
            // 
            // rateSelector
            // 
            this.rateSelector.FormattingEnabled = true;
            this.rateSelector.Items.AddRange(new object[] {
            "Slow",
            "Normal",
            "Fast"});
            this.rateSelector.Location = new System.Drawing.Point(170, 147);
            this.rateSelector.Name = "rateSelector";
            this.rateSelector.Size = new System.Drawing.Size(71, 21);
            this.rateSelector.TabIndex = 13;
            this.rateSelector.SelectedIndexChanged += new System.EventHandler(this.rateSelector_SelectedIndexChanged);
            // 
            // VoiceAssignment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 209);
            this.Controls.Add(this.rateSelector);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pitchSelector);
            this.Controls.Add(this.age);
            this.Controls.Add(this.gender);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.saveBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.voiceList);
            this.Controls.Add(this.avName);
            this.Controls.Add(this.label1);
            this.Name = "VoiceAssignment";
            this.Text = "Voice Assignment";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox avName;
        private System.Windows.Forms.ListBox voiceList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button saveBtn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox gender;
        private System.Windows.Forms.TextBox age;
        private System.Windows.Forms.ComboBox pitchSelector;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox rateSelector;
    }
}