namespace Radegast.Plugin.IRC
{
    partial class RelayConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RelayConsole));
            this.pnlChatLine = new System.Windows.Forms.Panel();
            this.btnSend = new System.Windows.Forms.Button();
            this.cbxInput = new Radegast.ChatInputBox();
            this.pnlConnectionSettings = new System.Windows.Forms.Panel();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtChan = new System.Windows.Forms.TextBox();
            this.txtNick = new System.Windows.Forms.TextBox();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.scChat = new System.Windows.Forms.SplitContainer();
            this.rtbChatText = new System.Windows.Forms.RichTextBox();
            this.Participants = new Radegast.ListViewNoFlicker();
            this.tsChatMenu = new System.Windows.Forms.ToolStrip();
            this.lblConnected = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnConnectPannel = new System.Windows.Forms.ToolStripButton();
            this.btnDisconnect = new System.Windows.Forms.ToolStripButton();
            this.ddGroup = new System.Windows.Forms.ToolStripDropDownButton();
            this.pnlChatLine.SuspendLayout();
            this.pnlConnectionSettings.SuspendLayout();
            this.scChat.Panel1.SuspendLayout();
            this.scChat.Panel2.SuspendLayout();
            this.scChat.SuspendLayout();
            this.tsChatMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlChatLine
            // 
            this.pnlChatLine.Controls.Add(this.btnSend);
            this.pnlChatLine.Controls.Add(this.cbxInput);
            this.pnlChatLine.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlChatLine.Location = new System.Drawing.Point(0, 380);
            this.pnlChatLine.Name = "pnlChatLine";
            this.pnlChatLine.Size = new System.Drawing.Size(705, 27);
            this.pnlChatLine.TabIndex = 0;
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Enabled = false;
            this.btnSend.Location = new System.Drawing.Point(646, 2);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(55, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // cbxInput
            // 
            this.cbxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxInput.Location = new System.Drawing.Point(3, 3);
            this.cbxInput.Name = "cbxInput";
            this.cbxInput.Size = new System.Drawing.Size(638, 20);
            this.cbxInput.TabIndex = 1;
            this.cbxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbxInput_KeyDown);
            // 
            // pnlConnectionSettings
            // 
            this.pnlConnectionSettings.Controls.Add(this.btnConnect);
            this.pnlConnectionSettings.Controls.Add(this.txtChan);
            this.pnlConnectionSettings.Controls.Add(this.txtNick);
            this.pnlConnectionSettings.Controls.Add(this.txtPort);
            this.pnlConnectionSettings.Controls.Add(this.txtServer);
            this.pnlConnectionSettings.Controls.Add(this.label4);
            this.pnlConnectionSettings.Controls.Add(this.label3);
            this.pnlConnectionSettings.Controls.Add(this.label2);
            this.pnlConnectionSettings.Controls.Add(this.label1);
            this.pnlConnectionSettings.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlConnectionSettings.Location = new System.Drawing.Point(0, 25);
            this.pnlConnectionSettings.Name = "pnlConnectionSettings";
            this.pnlConnectionSettings.Size = new System.Drawing.Size(705, 30);
            this.pnlConnectionSettings.TabIndex = 1;
            this.pnlConnectionSettings.Visible = false;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(9, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 10;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtChan
            // 
            this.txtChan.Location = new System.Drawing.Point(540, 6);
            this.txtChan.Name = "txtChan";
            this.txtChan.Size = new System.Drawing.Size(80, 20);
            this.txtChan.TabIndex = 4;
            // 
            // txtNick
            // 
            this.txtNick.Location = new System.Drawing.Point(435, 6);
            this.txtNick.Name = "txtNick";
            this.txtNick.Size = new System.Drawing.Size(61, 20);
            this.txtNick.TabIndex = 3;
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(333, 6);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(61, 20);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "6667";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(155, 6);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(140, 20);
            this.txtServer.TabIndex = 1;
            this.txtServer.Text = "irc.freenode.net";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(502, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Chan";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(400, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Nick";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(301, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Port";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(90, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IRC Server";
            // 
            // scChat
            // 
            this.scChat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scChat.Location = new System.Drawing.Point(0, 55);
            this.scChat.Name = "scChat";
            // 
            // scChat.Panel1
            // 
            this.scChat.Panel1.Controls.Add(this.rtbChatText);
            // 
            // scChat.Panel2
            // 
            this.scChat.Panel2.Controls.Add(this.Participants);
            this.scChat.Panel2.Enabled = false;
            this.scChat.Size = new System.Drawing.Size(705, 325);
            this.scChat.SplitterDistance = 533;
            this.scChat.TabIndex = 2;
            // 
            // rtbChatText
            // 
            this.rtbChatText.BackColor = System.Drawing.Color.White;
            this.rtbChatText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbChatText.HideSelection = false;
            this.rtbChatText.Location = new System.Drawing.Point(0, 0);
            this.rtbChatText.Name = "rtbChatText";
            this.rtbChatText.ReadOnly = true;
            this.rtbChatText.Size = new System.Drawing.Size(533, 325);
            this.rtbChatText.TabIndex = 4;
            this.rtbChatText.Text = "";
            // 
            // Participants
            // 
            this.Participants.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.Participants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Participants.HideSelection = false;
            this.Participants.Location = new System.Drawing.Point(0, 0);
            this.Participants.MultiSelect = false;
            this.Participants.Name = "Participants";
            this.Participants.ShowGroups = false;
            this.Participants.Size = new System.Drawing.Size(168, 325);
            this.Participants.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.Participants.TabIndex = 5;
            this.Participants.UseCompatibleStateImageBehavior = false;
            this.Participants.View = System.Windows.Forms.View.List;
            // 
            // tsChatMenu
            // 
            this.tsChatMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsChatMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblConnected,
            this.toolStripSeparator2,
            this.btnConnectPannel,
            this.btnDisconnect,
            this.ddGroup});
            this.tsChatMenu.Location = new System.Drawing.Point(0, 0);
            this.tsChatMenu.Name = "tsChatMenu";
            this.tsChatMenu.Size = new System.Drawing.Size(705, 25);
            this.tsChatMenu.TabIndex = 3;
            this.tsChatMenu.Text = "toolStrip1";
            // 
            // lblConnected
            // 
            this.lblConnected.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblConnected.Name = "lblConnected";
            this.lblConnected.Size = new System.Drawing.Size(84, 22);
            this.lblConnected.Text = "not connected";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnConnectPannel
            // 
            this.btnConnectPannel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnConnectPannel.Image = ((System.Drawing.Image)(resources.GetObject("btnConnectPannel.Image")));
            this.btnConnectPannel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConnectPannel.Name = "btnConnectPannel";
            this.btnConnectPannel.Size = new System.Drawing.Size(56, 22);
            this.btnConnectPannel.Text = "Connect";
            this.btnConnectPannel.Click += new System.EventHandler(this.btnConnectPannel_Click);
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDisconnect.Image = ((System.Drawing.Image)(resources.GetObject("btnDisconnect.Image")));
            this.btnDisconnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(70, 22);
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // ddGroup
            // 
            this.ddGroup.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.ddGroup.Image = ((System.Drawing.Image)(resources.GetObject("ddGroup.Image")));
            this.ddGroup.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ddGroup.Name = "ddGroup";
            this.ddGroup.Size = new System.Drawing.Size(91, 22);
            this.ddGroup.Text = "Group (none)";
            // 
            // RelayConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.scChat);
            this.Controls.Add(this.pnlConnectionSettings);
            this.Controls.Add(this.pnlChatLine);
            this.Controls.Add(this.tsChatMenu);
            this.Name = "RelayConsole";
            this.Size = new System.Drawing.Size(705, 407);
            this.pnlChatLine.ResumeLayout(false);
            this.pnlChatLine.PerformLayout();
            this.pnlConnectionSettings.ResumeLayout(false);
            this.pnlConnectionSettings.PerformLayout();
            this.scChat.Panel1.ResumeLayout(false);
            this.scChat.Panel2.ResumeLayout(false);
            this.scChat.ResumeLayout(false);
            this.tsChatMenu.ResumeLayout(false);
            this.tsChatMenu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlChatLine;
        private System.Windows.Forms.Panel pnlConnectionSettings;
        private System.Windows.Forms.SplitContainer scChat;
        private System.Windows.Forms.ToolStrip tsChatMenu;
        private System.Windows.Forms.ToolStripLabel lblConnected;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.RichTextBox rtbChatText;
        public ListViewNoFlicker Participants;
        public ChatInputBox cbxInput;
        public System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ToolStripButton btnConnectPannel;
        private System.Windows.Forms.TextBox txtNick;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtChan;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripButton btnDisconnect;
        private System.Windows.Forms.ToolStripDropDownButton ddGroup;
    }
}
