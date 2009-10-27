namespace Radegast.Plugin.Voice
{
    partial class SettingsForm
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
            this.cStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sStatus = new System.Windows.Forms.Label();
            this.micMute = new System.Windows.Forms.CheckBox();
            this.micLevel = new System.Windows.Forms.TrackBar();
            this.spkrLevel = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.spkrDevice = new System.Windows.Forms.ComboBox();
            this.micDevice = new System.Windows.Forms.ComboBox();
            this.spkrMute = new System.Windows.Forms.CheckBox();
            this.sendlevel = new System.Windows.Forms.ProgressBar();
            this.rcvLevel = new System.Windows.Forms.ProgressBar();
            this.talkMode = new System.Windows.Forms.RadioButton();
            this.testMode = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.micLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.spkrLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Connection";
            // 
            // cStatus
            // 
            this.cStatus.AutoSize = true;
            this.cStatus.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.cStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cStatus.Location = new System.Drawing.Point(83, 11);
            this.cStatus.Name = "cStatus";
            this.cStatus.Size = new System.Drawing.Size(47, 15);
            this.cStatus.TabIndex = 1;
            this.cStatus.Text = "offline";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Session";
            // 
            // sStatus
            // 
            this.sStatus.AutoSize = true;
            this.sStatus.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.sStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.sStatus.Location = new System.Drawing.Point(83, 37);
            this.sStatus.Name = "sStatus";
            this.sStatus.Size = new System.Drawing.Size(39, 15);
            this.sStatus.TabIndex = 3;
            this.sStatus.Text = "none";
            // 
            // micMute
            // 
            this.micMute.AutoSize = true;
            this.micMute.Checked = true;
            this.micMute.CheckState = System.Windows.Forms.CheckState.Checked;
            this.micMute.Location = new System.Drawing.Point(86, 71);
            this.micMute.Name = "micMute";
            this.micMute.Size = new System.Drawing.Size(50, 17);
            this.micMute.TabIndex = 4;
            this.micMute.Text = "Mute";
            this.micMute.UseVisualStyleBackColor = true;
            this.micMute.CheckedChanged += new System.EventHandler(this.micMute_CheckedChanged);
            // 
            // micLevel
            // 
            this.micLevel.BackColor = System.Drawing.SystemColors.Control;
            this.micLevel.LargeChange = 20;
            this.micLevel.Location = new System.Drawing.Point(142, 62);
            this.micLevel.Maximum = 100;
            this.micLevel.Minimum = -100;
            this.micLevel.Name = "micLevel";
            this.micLevel.Size = new System.Drawing.Size(104, 42);
            this.micLevel.TabIndex = 5;
            this.micLevel.TickFrequency = 20;
            this.micLevel.ValueChanged += new System.EventHandler(this.micLevel_ValueChanged);
            // 
            // spkrLevel
            // 
            this.spkrLevel.Location = new System.Drawing.Point(142, 106);
            this.spkrLevel.Maximum = 100;
            this.spkrLevel.Minimum = -100;
            this.spkrLevel.Name = "spkrLevel";
            this.spkrLevel.Size = new System.Drawing.Size(104, 42);
            this.spkrLevel.TabIndex = 6;
            this.spkrLevel.TickFrequency = 20;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Microphone";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Speaker";
            // 
            // spkrDevice
            // 
            this.spkrDevice.FormattingEnabled = true;
            this.spkrDevice.Location = new System.Drawing.Point(252, 110);
            this.spkrDevice.Name = "spkrDevice";
            this.spkrDevice.Size = new System.Drawing.Size(369, 21);
            this.spkrDevice.TabIndex = 9;
            this.spkrDevice.SelectedIndexChanged += new System.EventHandler(this.spkrDevice_SelectedIndexChanged);
            // 
            // micDevice
            // 
            this.micDevice.FormattingEnabled = true;
            this.micDevice.Location = new System.Drawing.Point(252, 69);
            this.micDevice.Name = "micDevice";
            this.micDevice.Size = new System.Drawing.Size(369, 21);
            this.micDevice.TabIndex = 10;
            this.micDevice.SelectedIndexChanged += new System.EventHandler(this.micDevice_SelectedIndexChanged);
            // 
            // spkrMute
            // 
            this.spkrMute.AutoSize = true;
            this.spkrMute.Location = new System.Drawing.Point(86, 114);
            this.spkrMute.Name = "spkrMute";
            this.spkrMute.Size = new System.Drawing.Size(50, 17);
            this.spkrMute.TabIndex = 11;
            this.spkrMute.Text = "Mute";
            this.spkrMute.UseVisualStyleBackColor = true;
            this.spkrMute.CheckedChanged += new System.EventHandler(this.spkrMute_CheckedChanged);
            // 
            // sendlevel
            // 
            this.sendlevel.ForeColor = System.Drawing.Color.Red;
            this.sendlevel.Location = new System.Drawing.Point(526, 13);
            this.sendlevel.Name = "sendlevel";
            this.sendlevel.Size = new System.Drawing.Size(100, 13);
            this.sendlevel.TabIndex = 12;
            // 
            // rcvLevel
            // 
            this.rcvLevel.ForeColor = System.Drawing.Color.Red;
            this.rcvLevel.Location = new System.Drawing.Point(526, 37);
            this.rcvLevel.Name = "rcvLevel";
            this.rcvLevel.Size = new System.Drawing.Size(100, 15);
            this.rcvLevel.TabIndex = 13;
            // 
            // talkMode
            // 
            this.talkMode.AutoSize = true;
            this.talkMode.Checked = true;
            this.talkMode.Location = new System.Drawing.Point(474, 11);
            this.talkMode.Name = "talkMode";
            this.talkMode.Size = new System.Drawing.Size(46, 17);
            this.talkMode.TabIndex = 16;
            this.talkMode.TabStop = true;
            this.talkMode.Text = "Talk";
            this.talkMode.UseVisualStyleBackColor = true;
            this.talkMode.Click += new System.EventHandler(this.talkMode_Click);
            // 
            // testMode
            // 
            this.testMode.AutoSize = true;
            this.testMode.Location = new System.Drawing.Point(474, 37);
            this.testMode.Name = "testMode";
            this.testMode.Size = new System.Drawing.Size(46, 17);
            this.testMode.TabIndex = 17;
            this.testMode.Text = "Test";
            this.testMode.UseVisualStyleBackColor = true;
            this.testMode.Click += new System.EventHandler(this.testMode_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(638, 153);
            this.Controls.Add(this.testMode);
            this.Controls.Add(this.talkMode);
            this.Controls.Add(this.rcvLevel);
            this.Controls.Add(this.sendlevel);
            this.Controls.Add(this.spkrMute);
            this.Controls.Add(this.micDevice);
            this.Controls.Add(this.spkrDevice);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.spkrLevel);
            this.Controls.Add(this.micLevel);
            this.Controls.Add(this.micMute);
            this.Controls.Add(this.sStatus);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cStatus);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Voice Console";
            ((System.ComponentModel.ISupportInitialize)(this.micLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.spkrLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label sStatus;
        private System.Windows.Forms.Label cStatus;
        private System.Windows.Forms.CheckBox micMute;
        private System.Windows.Forms.TrackBar micLevel;
        private System.Windows.Forms.TrackBar spkrLevel;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox spkrDevice;
        private System.Windows.Forms.ComboBox micDevice;
        private System.Windows.Forms.CheckBox spkrMute;
        private System.Windows.Forms.ProgressBar sendlevel;
        private System.Windows.Forms.ProgressBar rcvLevel;
        private System.Windows.Forms.RadioButton talkMode;
        private System.Windows.Forms.RadioButton testMode;
    }
}