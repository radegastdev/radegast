namespace Radegast
{
    partial class ChatConsole
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChatConsole));
            this.rtbChat = new System.Windows.Forms.RichTextBox();
            this.cbxInput = new System.Windows.Forms.ComboBox();
            this.btnSay = new System.Windows.Forms.Button();
            this.btnShout = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lvwObjects = new System.Windows.Forms.ListView();
            this.avatarContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ctxProfile = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxPay = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxStartIM = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxFollow = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxTextures = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxAttach = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxMaster = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxAnim = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSource = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tbtnStartIM = new System.Windows.Forms.ToolStripButton();
            this.tbtnProfile = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnFollow = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnTextures = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnAttach = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnMaster = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnAnim = new System.Windows.Forms.ToolStripButton();
            this.pnlMovement = new System.Windows.Forms.Panel();
            this.btnMoveBack = new System.Windows.Forms.Button();
            this.btnFwd = new System.Windows.Forms.Button();
            this.btnTurnRight = new System.Windows.Forms.Button();
            this.btnTurnLeft = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.avatarContext.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pnlMovement.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtbChat
            // 
            this.rtbChat.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbChat.BackColor = System.Drawing.Color.White;
            this.rtbChat.HideSelection = false;
            this.rtbChat.Location = new System.Drawing.Point(3, 0);
            this.rtbChat.Name = "rtbChat";
            this.rtbChat.ReadOnly = true;
            this.rtbChat.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            this.rtbChat.ShowSelectionMargin = true;
            this.rtbChat.Size = new System.Drawing.Size(400, 310);
            this.rtbChat.TabIndex = 5;
            this.rtbChat.Text = "";
            this.rtbChat.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbChat_LinkClicked);
            // 
            // cbxInput
            // 
            this.cbxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbxInput.Enabled = false;
            this.cbxInput.FormattingEnabled = true;
            this.cbxInput.Location = new System.Drawing.Point(0, 0);
            this.cbxInput.Name = "cbxInput";
            this.cbxInput.Size = new System.Drawing.Size(352, 21);
            this.cbxInput.TabIndex = 3;
            this.cbxInput.KeyUp += new System.Windows.Forms.KeyEventHandler(this.cbxInput_KeyUp);
            this.cbxInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbxInput_KeyDown);
            this.cbxInput.TextChanged += new System.EventHandler(this.cbxInput_TextChanged);
            // 
            // btnSay
            // 
            this.btnSay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSay.Enabled = false;
            this.btnSay.Location = new System.Drawing.Point(358, 0);
            this.btnSay.Name = "btnSay";
            this.btnSay.Size = new System.Drawing.Size(76, 24);
            this.btnSay.TabIndex = 5;
            this.btnSay.Text = "Say";
            this.btnSay.UseVisualStyleBackColor = true;
            this.btnSay.Click += new System.EventHandler(this.btnSay_Click);
            // 
            // btnShout
            // 
            this.btnShout.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnShout.Enabled = false;
            this.btnShout.Location = new System.Drawing.Point(440, 0);
            this.btnShout.Name = "btnShout";
            this.btnShout.Size = new System.Drawing.Size(76, 24);
            this.btnShout.TabIndex = 4;
            this.btnShout.Text = "Shout";
            this.btnShout.UseVisualStyleBackColor = true;
            this.btnShout.Click += new System.EventHandler(this.btnShout_Click);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.rtbChat);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvwObjects);
            this.splitContainer1.Panel2.Controls.Add(this.toolStrip1);
            this.splitContainer1.Panel2.Controls.Add(this.pnlMovement);
            this.splitContainer1.Size = new System.Drawing.Size(516, 310);
            this.splitContainer1.SplitterDistance = 400;
            this.splitContainer1.TabIndex = 7;
            // 
            // lvwObjects
            // 
            this.lvwObjects.AllowDrop = true;
            this.lvwObjects.ContextMenuStrip = this.avatarContext;
            this.lvwObjects.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwObjects.FullRowSelect = true;
            this.lvwObjects.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvwObjects.LabelWrap = false;
            this.lvwObjects.Location = new System.Drawing.Point(0, 0);
            this.lvwObjects.MultiSelect = false;
            this.lvwObjects.Name = "lvwObjects";
            this.lvwObjects.Size = new System.Drawing.Size(71, 273);
            this.lvwObjects.TabIndex = 10;
            this.lvwObjects.UseCompatibleStateImageBehavior = false;
            this.lvwObjects.View = System.Windows.Forms.View.List;
            this.lvwObjects.SelectedIndexChanged += new System.EventHandler(this.lvwObjects_SelectedIndexChanged);
            this.lvwObjects.DragDrop += new System.Windows.Forms.DragEventHandler(this.lvwObjects_DragDrop);
            this.lvwObjects.DragOver += new System.Windows.Forms.DragEventHandler(this.lvwObjects_DragOver);
            // 
            // avatarContext
            // 
            this.avatarContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ctxProfile,
            this.ctxPay,
            this.ctxStartIM,
            this.ctxFollow,
            this.ctxTextures,
            this.ctxAttach,
            this.ctxMaster,
            this.ctxAnim,
            this.ctxPoint,
            this.ctxSource});
            this.avatarContext.Name = "avatarContext";
            this.avatarContext.Size = new System.Drawing.Size(157, 246);
            this.avatarContext.Opening += new System.ComponentModel.CancelEventHandler(this.avatarContext_Opening);
            // 
            // ctxProfile
            // 
            this.ctxProfile.Name = "ctxProfile";
            this.ctxProfile.Size = new System.Drawing.Size(156, 22);
            this.ctxProfile.Text = "Profile";
            this.ctxProfile.Click += new System.EventHandler(this.tbtnProfile_Click);
            // 
            // ctxPay
            // 
            this.ctxPay.Enabled = false;
            this.ctxPay.Name = "ctxPay";
            this.ctxPay.Size = new System.Drawing.Size(156, 22);
            this.ctxPay.Text = "Pay";
            this.ctxPay.Click += new System.EventHandler(this.ctxPay_Click);
            // 
            // ctxStartIM
            // 
            this.ctxStartIM.Name = "ctxStartIM";
            this.ctxStartIM.Size = new System.Drawing.Size(156, 22);
            this.ctxStartIM.Text = "Start IM";
            this.ctxStartIM.Click += new System.EventHandler(this.tbtnStartIM_Click);
            // 
            // ctxFollow
            // 
            this.ctxFollow.Name = "ctxFollow";
            this.ctxFollow.Size = new System.Drawing.Size(156, 22);
            this.ctxFollow.Text = "Follow";
            this.ctxFollow.Click += new System.EventHandler(this.tbtnFollow_Click);
            // 
            // ctxTextures
            // 
            this.ctxTextures.Name = "ctxTextures";
            this.ctxTextures.Size = new System.Drawing.Size(156, 22);
            this.ctxTextures.Text = "Textures";
            this.ctxTextures.Click += new System.EventHandler(this.dumpOufitBtn_Click);
            // 
            // ctxAttach
            // 
            this.ctxAttach.Name = "ctxAttach";
            this.ctxAttach.Size = new System.Drawing.Size(156, 22);
            this.ctxAttach.Text = "Attachments";
            this.ctxAttach.Click += new System.EventHandler(this.tbtnAttach_Click);
            // 
            // ctxMaster
            // 
            this.ctxMaster.Name = "ctxMaster";
            this.ctxMaster.Size = new System.Drawing.Size(156, 22);
            this.ctxMaster.Text = "Master controls";
            this.ctxMaster.Click += new System.EventHandler(this.tbtnMaster_Click);
            // 
            // ctxAnim
            // 
            this.ctxAnim.Name = "ctxAnim";
            this.ctxAnim.Size = new System.Drawing.Size(156, 22);
            this.ctxAnim.Text = "Animations";
            this.ctxAnim.Click += new System.EventHandler(this.tbtnAnim_Click);
            // 
            // ctxPoint
            // 
            this.ctxPoint.Name = "ctxPoint";
            this.ctxPoint.Size = new System.Drawing.Size(156, 22);
            this.ctxPoint.Text = "Point at";
            this.ctxPoint.Click += new System.EventHandler(this.ctxPoint_Click);
            // 
            // ctxSource
            // 
            this.ctxSource.Name = "ctxSource";
            this.ctxSource.Size = new System.Drawing.Size(156, 22);
            this.ctxSource.Text = "Set as source";
            this.ctxSource.Click += new System.EventHandler(this.ctxSource_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtnStartIM,
            this.tbtnProfile,
            this.toolStripSeparator1,
            this.tbtnFollow,
            this.toolStripSeparator2,
            this.tbtnTextures,
            this.toolStripSeparator3,
            this.tbtnAttach,
            this.toolStripSeparator4,
            this.tbtnMaster,
            this.toolStripSeparator5,
            this.tbtnAnim});
            this.toolStrip1.Location = new System.Drawing.Point(71, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(41, 273);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tbtnStartIM
            // 
            this.tbtnStartIM.AutoToolTip = false;
            this.tbtnStartIM.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnStartIM.Enabled = false;
            this.tbtnStartIM.Image = global::Radegast.Properties.Resources.computer_16;
            this.tbtnStartIM.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnStartIM.Name = "tbtnStartIM";
            this.tbtnStartIM.Size = new System.Drawing.Size(38, 20);
            this.tbtnStartIM.ToolTipText = "Start IM";
            this.tbtnStartIM.Click += new System.EventHandler(this.tbtnStartIM_Click);
            // 
            // tbtnProfile
            // 
            this.tbtnProfile.AutoToolTip = false;
            this.tbtnProfile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnProfile.Enabled = false;
            this.tbtnProfile.Image = global::Radegast.Properties.Resources.applications_16;
            this.tbtnProfile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnProfile.Name = "tbtnProfile";
            this.tbtnProfile.Size = new System.Drawing.Size(38, 20);
            this.tbtnProfile.ToolTipText = "View Profile";
            this.tbtnProfile.Click += new System.EventHandler(this.tbtnProfile_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(38, 6);
            // 
            // tbtnFollow
            // 
            this.tbtnFollow.AutoToolTip = false;
            this.tbtnFollow.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tbtnFollow.Enabled = false;
            this.tbtnFollow.Image = global::Radegast.Properties.Resources.arrow_forward_16;
            this.tbtnFollow.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnFollow.Name = "tbtnFollow";
            this.tbtnFollow.Size = new System.Drawing.Size(38, 20);
            this.tbtnFollow.ToolTipText = "Follow";
            this.tbtnFollow.Click += new System.EventHandler(this.tbtnFollow_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(38, 6);
            // 
            // tbtnTextures
            // 
            this.tbtnTextures.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnTextures.Enabled = false;
            this.tbtnTextures.Image = ((System.Drawing.Image)(resources.GetObject("tbtnTextures.Image")));
            this.tbtnTextures.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnTextures.Name = "tbtnTextures";
            this.tbtnTextures.Size = new System.Drawing.Size(38, 19);
            this.tbtnTextures.Text = "Txtr";
            this.tbtnTextures.Click += new System.EventHandler(this.dumpOufitBtn_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(38, 6);
            // 
            // tbtnAttach
            // 
            this.tbtnAttach.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnAttach.Enabled = false;
            this.tbtnAttach.Image = ((System.Drawing.Image)(resources.GetObject("tbtnAttach.Image")));
            this.tbtnAttach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnAttach.Name = "tbtnAttach";
            this.tbtnAttach.Size = new System.Drawing.Size(38, 19);
            this.tbtnAttach.Text = "Attn";
            this.tbtnAttach.ToolTipText = "List avatar attachments";
            this.tbtnAttach.Click += new System.EventHandler(this.tbtnAttach_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(38, 6);
            // 
            // tbtnMaster
            // 
            this.tbtnMaster.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnMaster.Enabled = false;
            this.tbtnMaster.Image = ((System.Drawing.Image)(resources.GetObject("tbtnMaster.Image")));
            this.tbtnMaster.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnMaster.Name = "tbtnMaster";
            this.tbtnMaster.Size = new System.Drawing.Size(38, 19);
            this.tbtnMaster.Text = "Mstr";
            this.tbtnMaster.Click += new System.EventHandler(this.tbtnMaster_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(38, 6);
            // 
            // tbtnAnim
            // 
            this.tbtnAnim.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnAnim.Enabled = false;
            this.tbtnAnim.Image = ((System.Drawing.Image)(resources.GetObject("tbtnAnim.Image")));
            this.tbtnAnim.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnAnim.Name = "tbtnAnim";
            this.tbtnAnim.Size = new System.Drawing.Size(38, 19);
            this.tbtnAnim.Text = "Anim";
            this.tbtnAnim.ToolTipText = "List Avatar Animatoions";
            this.tbtnAnim.Click += new System.EventHandler(this.tbtnAnim_Click);
            // 
            // pnlMovement
            // 
            this.pnlMovement.Controls.Add(this.btnMoveBack);
            this.pnlMovement.Controls.Add(this.btnFwd);
            this.pnlMovement.Controls.Add(this.btnTurnRight);
            this.pnlMovement.Controls.Add(this.btnTurnLeft);
            this.pnlMovement.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlMovement.Location = new System.Drawing.Point(0, 273);
            this.pnlMovement.Name = "pnlMovement";
            this.pnlMovement.Size = new System.Drawing.Size(112, 37);
            this.pnlMovement.TabIndex = 11;
            this.pnlMovement.Click += new System.EventHandler(this.pnlMovement_Click);
            // 
            // btnMoveBack
            // 
            this.btnMoveBack.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMoveBack.Location = new System.Drawing.Point(36, 15);
            this.btnMoveBack.Margin = new System.Windows.Forms.Padding(0);
            this.btnMoveBack.Name = "btnMoveBack";
            this.btnMoveBack.Size = new System.Drawing.Size(31, 19);
            this.btnMoveBack.TabIndex = 0;
            this.btnMoveBack.Text = "R";
            this.btnMoveBack.UseVisualStyleBackColor = true;
            this.btnMoveBack.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnMoveBack_MouseDown);
            this.btnMoveBack.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnMoveBack_MouseUp);
            // 
            // btnFwd
            // 
            this.btnFwd.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFwd.Location = new System.Drawing.Point(36, 0);
            this.btnFwd.Margin = new System.Windows.Forms.Padding(0);
            this.btnFwd.Name = "btnFwd";
            this.btnFwd.Size = new System.Drawing.Size(31, 19);
            this.btnFwd.TabIndex = 0;
            this.btnFwd.Text = "^";
            this.btnFwd.UseVisualStyleBackColor = true;
            this.btnFwd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnFwd_MouseDown);
            this.btnFwd.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnFwd_MouseUp);
            // 
            // btnTurnRight
            // 
            this.btnTurnRight.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTurnRight.Location = new System.Drawing.Point(67, 15);
            this.btnTurnRight.Margin = new System.Windows.Forms.Padding(0);
            this.btnTurnRight.Name = "btnTurnRight";
            this.btnTurnRight.Size = new System.Drawing.Size(31, 19);
            this.btnTurnRight.TabIndex = 0;
            this.btnTurnRight.Text = ">>";
            this.btnTurnRight.UseVisualStyleBackColor = true;
            this.btnTurnRight.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnTurnRight_MouseDown);
            this.btnTurnRight.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnTurnRight_MouseUp);
            // 
            // btnTurnLeft
            // 
            this.btnTurnLeft.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTurnLeft.Location = new System.Drawing.Point(5, 15);
            this.btnTurnLeft.Margin = new System.Windows.Forms.Padding(0);
            this.btnTurnLeft.Name = "btnTurnLeft";
            this.btnTurnLeft.Size = new System.Drawing.Size(31, 19);
            this.btnTurnLeft.TabIndex = 0;
            this.btnTurnLeft.Text = "<<";
            this.btnTurnLeft.UseVisualStyleBackColor = true;
            this.btnTurnLeft.MouseDown += new System.Windows.Forms.MouseEventHandler(this.btnTurnLeft_MouseDown);
            this.btnTurnLeft.MouseUp += new System.Windows.Forms.MouseEventHandler(this.btnTurnLeft_MouseUp);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cbxInput);
            this.panel1.Controls.Add(this.btnSay);
            this.panel1.Controls.Add(this.btnShout);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 310);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(516, 24);
            this.panel1.TabIndex = 8;
            // 
            // ChatConsole
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ChatConsole";
            this.Size = new System.Drawing.Size(516, 334);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            this.splitContainer1.ResumeLayout(false);
            this.avatarContext.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnlMovement.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbChat;
        private System.Windows.Forms.ComboBox cbxInput;
        private System.Windows.Forms.Button btnSay;
        private System.Windows.Forms.Button btnShout;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton tbtnStartIM;
        private System.Windows.Forms.ToolStripButton tbtnFollow;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ListView lvwObjects;
        private System.Windows.Forms.ToolStripButton tbtnProfile;
        private System.Windows.Forms.ToolStripButton tbtnTextures;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tbtnMaster;
        private System.Windows.Forms.ToolStripButton tbtnAttach;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.Panel pnlMovement;
        private System.Windows.Forms.Button btnTurnLeft;
        private System.Windows.Forms.Button btnTurnRight;
        private System.Windows.Forms.Button btnFwd;
        private System.Windows.Forms.Button btnMoveBack;
        private System.Windows.Forms.ToolStripButton tbtnAnim;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ContextMenuStrip avatarContext;
        private System.Windows.Forms.ToolStripMenuItem ctxProfile;
        private System.Windows.Forms.ToolStripMenuItem ctxStartIM;
        private System.Windows.Forms.ToolStripMenuItem ctxFollow;
        private System.Windows.Forms.ToolStripMenuItem ctxTextures;
        private System.Windows.Forms.ToolStripMenuItem ctxAttach;
        private System.Windows.Forms.ToolStripMenuItem ctxMaster;
        private System.Windows.Forms.ToolStripMenuItem ctxAnim;
        private System.Windows.Forms.ToolStripMenuItem ctxPoint;
        private System.Windows.Forms.ToolStripMenuItem ctxSource;
        private System.Windows.Forms.ToolStripMenuItem ctxPay;
    }
}
