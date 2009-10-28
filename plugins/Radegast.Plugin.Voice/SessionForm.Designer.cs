namespace Radegast.Plugin.Voice
{
    partial class SessionForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SessionForm));
            this.participants = new System.Windows.Forms.ListView();
            this.nameheader = new System.Windows.Forms.ColumnHeader();
            this.TalkStates = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // participants
            // 
            this.participants.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameheader});
            this.participants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.participants.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.participants.LabelWrap = false;
            this.participants.Location = new System.Drawing.Point(0, 0);
            this.participants.Name = "participants";
            this.participants.Size = new System.Drawing.Size(202, 299);
            this.participants.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.participants.StateImageList = this.TalkStates;
            this.participants.TabIndex = 0;
            this.participants.UseCompatibleStateImageBehavior = false;
            this.participants.View = System.Windows.Forms.View.Details;
            this.participants.MouseUp += new System.Windows.Forms.MouseEventHandler(this.participants_MouseUp);
            // 
            // nameheader
            // 
            this.nameheader.Text = "Name";
            this.nameheader.Width = 147;
            // 
            // TalkStates
            // 
            this.TalkStates.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("TalkStates.ImageStream")));
            this.TalkStates.TransparentColor = System.Drawing.Color.Transparent;
            this.TalkStates.Images.SetKeyName(0, "TalkIdle.png");
            this.TalkStates.Images.SetKeyName(1, "Talking.png");
            this.TalkStates.Images.SetKeyName(2, "TalkMute.png");
            // 
            // SessionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(202, 299);
            this.Controls.Add(this.participants);
            this.MaximizeBox = false;
            this.Name = "SessionForm";
            this.Text = "Voice Session";
            this.Load += new System.EventHandler(this.SessionForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SessionForm_FormClosed);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SessionForm_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView participants;
        private System.Windows.Forms.ColumnHeader nameheader;
        private System.Windows.Forms.ImageList TalkStates;

    }
}