namespace Radegast
{
    partial class MediaConsole
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
            this.components = new System.ComponentModel.Container();
            this.volAudioStream = new System.Windows.Forms.TrackBar();
            this.pnlParcelAudio = new System.Windows.Forms.GroupBox();
            this.pbAudioVolume = new System.Windows.Forms.ProgressBar();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPlay = new System.Windows.Forms.Button();
            this.cbKeep = new System.Windows.Forms.CheckBox();
            this.cbPlayAudioStream = new System.Windows.Forms.CheckBox();
            this.txtSongTitle = new System.Windows.Forms.TextBox();
            this.txtAudioURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.volAudioStream)).BeginInit();
            this.pnlParcelAudio.SuspendLayout();
            this.SuspendLayout();
            // 
            // volAudioStream
            // 
            this.volAudioStream.LargeChange = 25;
            this.volAudioStream.Location = new System.Drawing.Point(6, 19);
            this.volAudioStream.Maximum = 50;
            this.volAudioStream.Name = "volAudioStream";
            this.volAudioStream.Size = new System.Drawing.Size(347, 45);
            this.volAudioStream.SmallChange = 10;
            this.volAudioStream.TabIndex = 1;
            this.volAudioStream.Value = 25;
            // 
            // pnlParcelAudio
            // 
            this.pnlParcelAudio.Controls.Add(this.pbAudioVolume);
            this.pnlParcelAudio.Controls.Add(this.btnStop);
            this.pnlParcelAudio.Controls.Add(this.btnPlay);
            this.pnlParcelAudio.Controls.Add(this.cbKeep);
            this.pnlParcelAudio.Controls.Add(this.cbPlayAudioStream);
            this.pnlParcelAudio.Controls.Add(this.txtSongTitle);
            this.pnlParcelAudio.Controls.Add(this.txtAudioURL);
            this.pnlParcelAudio.Controls.Add(this.label2);
            this.pnlParcelAudio.Controls.Add(this.label1);
            this.pnlParcelAudio.Controls.Add(this.volAudioStream);
            this.pnlParcelAudio.Location = new System.Drawing.Point(3, 3);
            this.pnlParcelAudio.Name = "pnlParcelAudio";
            this.pnlParcelAudio.Size = new System.Drawing.Size(359, 157);
            this.pnlParcelAudio.TabIndex = 2;
            this.pnlParcelAudio.TabStop = false;
            this.pnlParcelAudio.Text = "Audio Stream";
            // 
            // pbAudioVolume
            // 
            this.pbAudioVolume.Location = new System.Drawing.Point(9, 96);
            this.pbAudioVolume.Name = "pbAudioVolume";
            this.pbAudioVolume.Size = new System.Drawing.Size(344, 14);
            this.pbAudioVolume.TabIndex = 6;
            // 
            // btnStop
            // 
            this.btnStop.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnStop.Image = global::Radegast.Properties.Resources.btn_stop;
            this.btnStop.Location = new System.Drawing.Point(36, 119);
            this.btnStop.Margin = new System.Windows.Forms.Padding(0);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(25, 25);
            this.btnStop.TabIndex = 5;
            this.btnStop.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnPlay
            // 
            this.btnPlay.Image = global::Radegast.Properties.Resources.btn_play;
            this.btnPlay.Location = new System.Drawing.Point(9, 119);
            this.btnPlay.Margin = new System.Windows.Forms.Padding(0);
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(25, 25);
            this.btnPlay.TabIndex = 5;
            this.btnPlay.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnPlay.UseVisualStyleBackColor = true;
            // 
            // cbKeep
            // 
            this.cbKeep.AutoSize = true;
            this.cbKeep.Location = new System.Drawing.Point(232, 124);
            this.cbKeep.Name = "cbKeep";
            this.cbKeep.Size = new System.Drawing.Size(121, 17);
            this.cbKeep.TabIndex = 4;
            this.cbKeep.Text = "Keep current stream";
            this.toolTip1.SetToolTip(this.cbKeep, "When checked the audio player will keep the current audio stream and not change w" +
                    "hen parcel audio changes");
            this.cbKeep.UseVisualStyleBackColor = true;
            // 
            // cbPlayAudioStream
            // 
            this.cbPlayAudioStream.AutoSize = true;
            this.cbPlayAudioStream.Location = new System.Drawing.Point(87, 124);
            this.cbPlayAudioStream.Name = "cbPlayAudioStream";
            this.cbPlayAudioStream.Size = new System.Drawing.Size(141, 17);
            this.cbPlayAudioStream.TabIndex = 4;
            this.cbPlayAudioStream.Text = "Play parcel audio stream";
            this.cbPlayAudioStream.UseVisualStyleBackColor = true;
            // 
            // txtSongTitle
            // 
            this.txtSongTitle.BackColor = System.Drawing.SystemColors.Control;
            this.txtSongTitle.Location = new System.Drawing.Point(41, 70);
            this.txtSongTitle.Name = "txtSongTitle";
            this.txtSongTitle.ReadOnly = true;
            this.txtSongTitle.Size = new System.Drawing.Size(312, 20);
            this.txtSongTitle.TabIndex = 3;
            // 
            // txtAudioURL
            // 
            this.txtAudioURL.BackColor = System.Drawing.SystemColors.Control;
            this.txtAudioURL.Location = new System.Drawing.Point(41, 48);
            this.txtAudioURL.Name = "txtAudioURL";
            this.txtAudioURL.Size = new System.Drawing.Size(312, 20);
            this.txtAudioURL.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Song";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "URL";
            // 
            // MediaConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlParcelAudio);
            this.Name = "MediaConsole";
            this.Size = new System.Drawing.Size(423, 279);
            ((System.ComponentModel.ISupportInitialize)(this.volAudioStream)).EndInit();
            this.pnlParcelAudio.ResumeLayout(false);
            this.pnlParcelAudio.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar volAudioStream;
        private System.Windows.Forms.GroupBox pnlParcelAudio;
        private System.Windows.Forms.TextBox txtAudioURL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbKeep;
        private System.Windows.Forms.CheckBox cbPlayAudioStream;
        private System.Windows.Forms.Button btnPlay;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.TextBox txtSongTitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar pbAudioVolume;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
