// 
// Radegast Metaverse Client
// Copyright (c) 2009-2012, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id$
//
namespace Radegast
{
    partial class frmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmSettings));
            this.tcGraphics = new System.Windows.Forms.TabControl();
            this.tbpGeneral = new System.Windows.Forms.TabPage();
            this.cbHighLight = new System.Windows.Forms.GroupBox();
            this.cbHighlightGroupIM = new System.Windows.Forms.CheckBox();
            this.cbHighlightIM = new System.Windows.Forms.CheckBox();
            this.cbHighlightChat = new System.Windows.Forms.CheckBox();
            this.cbFriendsHighlight = new System.Windows.Forms.CheckBox();
            this.cbMisc = new System.Windows.Forms.GroupBox();
            this.cbHideLoginGraphics = new System.Windows.Forms.CheckBox();
            this.cbDisableLookAt = new System.Windows.Forms.CheckBox();
            this.cbTrasactDialog = new System.Windows.Forms.CheckBox();
            this.cbRadegastLogToFile = new System.Windows.Forms.CheckBox();
            this.cbTrasactChat = new System.Windows.Forms.CheckBox();
            this.cbFriendsNotifications = new System.Windows.Forms.CheckBox();
            this.txtReconnectTime = new System.Windows.Forms.TextBox();
            this.cbAutoReconnect = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbRLV = new System.Windows.Forms.CheckBox();
            this.cbMinToTrey = new System.Windows.Forms.CheckBox();
            this.cbRadegastClientTag = new System.Windows.Forms.CheckBox();
            this.cbSyntaxHighlight = new System.Windows.Forms.CheckBox();
            this.Chat = new System.Windows.Forms.GroupBox();
            this.cbDisableChatIMLog = new System.Windows.Forms.CheckBox();
            this.cbChatTimestamps = new System.Windows.Forms.CheckBox();
            this.cbIMTimeStamps = new System.Windows.Forms.CheckBox();
            this.cbMUEmotes = new System.Windows.Forms.CheckBox();
            this.cbNoTyping = new System.Windows.Forms.CheckBox();
            this.cbTaskBarHighLight = new System.Windows.Forms.CheckBox();
            this.cbFontSize = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.gbDisplayNames = new System.Windows.Forms.GroupBox();
            this.rbDNOnlyDN = new System.Windows.Forms.RadioButton();
            this.rbDNDandUsernme = new System.Windows.Forms.RadioButton();
            this.rbDNSmart = new System.Windows.Forms.RadioButton();
            this.rbDNOff = new System.Windows.Forms.RadioButton();
            this.tbpAutoResponse = new System.Windows.Forms.TabPage();
            this.gnAutoInventory = new System.Windows.Forms.GroupBox();
            this.cbOnInvOffer = new System.Windows.Forms.ComboBox();
            this.txtAutoResponse = new System.Windows.Forms.TextBox();
            this.gbAutoResponse = new System.Windows.Forms.GroupBox();
            this.rbAutoAlways = new System.Windows.Forms.RadioButton();
            this.rbAutoNonFriend = new System.Windows.Forms.RadioButton();
            this.rbAutobusy = new System.Windows.Forms.RadioButton();
            this.tbpGraphics = new System.Windows.Forms.TabPage();
            this.tcGraphics.SuspendLayout();
            this.tbpGeneral.SuspendLayout();
            this.cbHighLight.SuspendLayout();
            this.cbMisc.SuspendLayout();
            this.Chat.SuspendLayout();
            this.gbDisplayNames.SuspendLayout();
            this.tbpAutoResponse.SuspendLayout();
            this.gnAutoInventory.SuspendLayout();
            this.gbAutoResponse.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcGraphics
            // 
            this.tcGraphics.Controls.Add(this.tbpGeneral);
            this.tcGraphics.Controls.Add(this.tbpAutoResponse);
            this.tcGraphics.Controls.Add(this.tbpGraphics);
            this.tcGraphics.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcGraphics.Location = new System.Drawing.Point(0, 0);
            this.tcGraphics.Multiline = true;
            this.tcGraphics.Name = "tcGraphics";
            this.tcGraphics.SelectedIndex = 0;
            this.tcGraphics.Size = new System.Drawing.Size(530, 403);
            this.tcGraphics.TabIndex = 0;
            // 
            // tbpGeneral
            // 
            this.tbpGeneral.Controls.Add(this.cbHighLight);
            this.tbpGeneral.Controls.Add(this.cbMisc);
            this.tbpGeneral.Controls.Add(this.Chat);
            this.tbpGeneral.Controls.Add(this.gbDisplayNames);
            this.tbpGeneral.Location = new System.Drawing.Point(4, 22);
            this.tbpGeneral.Name = "tbpGeneral";
            this.tbpGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tbpGeneral.Size = new System.Drawing.Size(522, 377);
            this.tbpGeneral.TabIndex = 1;
            this.tbpGeneral.Text = "General";
            this.tbpGeneral.UseVisualStyleBackColor = true;
            // 
            // cbHighLight
            // 
            this.cbHighLight.Controls.Add(this.cbHighlightGroupIM);
            this.cbHighLight.Controls.Add(this.cbHighlightIM);
            this.cbHighLight.Controls.Add(this.cbHighlightChat);
            this.cbHighLight.Controls.Add(this.cbFriendsHighlight);
            this.cbHighLight.Controls.Add(this.cbTaskBarHighLight);
            this.cbHighLight.Location = new System.Drawing.Point(8, 160);
            this.cbHighLight.Name = "cbHighLight";
            this.cbHighLight.Size = new System.Drawing.Size(256, 136);
            this.cbHighLight.TabIndex = 1;
            this.cbHighLight.TabStop = false;
            this.cbHighLight.Text = "Highlight when tab changes";
            // 
            // cbHighlightGroupIM
            // 
            this.cbHighlightGroupIM.AutoSize = true;
            this.cbHighlightGroupIM.Location = new System.Drawing.Point(27, 111);
            this.cbHighlightGroupIM.Name = "cbHighlightGroupIM";
            this.cbHighlightGroupIM.Size = new System.Drawing.Size(136, 17);
            this.cbHighlightGroupIM.TabIndex = 5;
            this.cbHighlightGroupIM.Text = "Group Instant Message";
            this.cbHighlightGroupIM.UseVisualStyleBackColor = true;
            // 
            // cbHighlightIM
            // 
            this.cbHighlightIM.AutoSize = true;
            this.cbHighlightIM.Location = new System.Drawing.Point(27, 88);
            this.cbHighlightIM.Name = "cbHighlightIM";
            this.cbHighlightIM.Size = new System.Drawing.Size(104, 17);
            this.cbHighlightIM.TabIndex = 4;
            this.cbHighlightIM.Text = "Instant Message";
            this.cbHighlightIM.UseVisualStyleBackColor = true;
            // 
            // cbHighlightChat
            // 
            this.cbHighlightChat.AutoSize = true;
            this.cbHighlightChat.Location = new System.Drawing.Point(27, 42);
            this.cbHighlightChat.Name = "cbHighlightChat";
            this.cbHighlightChat.Size = new System.Drawing.Size(48, 17);
            this.cbHighlightChat.TabIndex = 2;
            this.cbHighlightChat.Text = "Chat";
            this.cbHighlightChat.UseVisualStyleBackColor = true;
            // 
            // cbFriendsHighlight
            // 
            this.cbFriendsHighlight.AutoSize = true;
            this.cbFriendsHighlight.Location = new System.Drawing.Point(27, 65);
            this.cbFriendsHighlight.Name = "cbFriendsHighlight";
            this.cbFriendsHighlight.Size = new System.Drawing.Size(150, 17);
            this.cbFriendsHighlight.TabIndex = 3;
            this.cbFriendsHighlight.Text = "Friends online notifications";
            this.cbFriendsHighlight.UseVisualStyleBackColor = true;
            this.cbFriendsHighlight.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbMisc
            // 
            this.cbMisc.Controls.Add(this.cbHideLoginGraphics);
            this.cbMisc.Controls.Add(this.cbDisableLookAt);
            this.cbMisc.Controls.Add(this.cbTrasactDialog);
            this.cbMisc.Controls.Add(this.cbRadegastLogToFile);
            this.cbMisc.Controls.Add(this.cbTrasactChat);
            this.cbMisc.Controls.Add(this.cbFriendsNotifications);
            this.cbMisc.Controls.Add(this.txtReconnectTime);
            this.cbMisc.Controls.Add(this.cbAutoReconnect);
            this.cbMisc.Controls.Add(this.label2);
            this.cbMisc.Controls.Add(this.cbRLV);
            this.cbMisc.Controls.Add(this.cbMinToTrey);
            this.cbMisc.Controls.Add(this.cbRadegastClientTag);
            this.cbMisc.Controls.Add(this.cbSyntaxHighlight);
            this.cbMisc.Location = new System.Drawing.Point(270, 6);
            this.cbMisc.Name = "cbMisc";
            this.cbMisc.Size = new System.Drawing.Size(236, 260);
            this.cbMisc.TabIndex = 2;
            this.cbMisc.TabStop = false;
            // 
            // cbHideLoginGraphics
            // 
            this.cbHideLoginGraphics.AutoSize = true;
            this.cbHideLoginGraphics.Location = new System.Drawing.Point(6, 11);
            this.cbHideLoginGraphics.Name = "cbHideLoginGraphics";
            this.cbHideLoginGraphics.Size = new System.Drawing.Size(141, 17);
            this.cbHideLoginGraphics.TabIndex = 3;
            this.cbHideLoginGraphics.Text = "Hide login slpash screen";
            this.cbHideLoginGraphics.UseVisualStyleBackColor = true;
            this.cbHideLoginGraphics.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbDisableLookAt
            // 
            this.cbDisableLookAt.AutoSize = true;
            this.cbDisableLookAt.Location = new System.Drawing.Point(6, 234);
            this.cbDisableLookAt.Name = "cbDisableLookAt";
            this.cbDisableLookAt.Size = new System.Drawing.Size(140, 17);
            this.cbDisableLookAt.TabIndex = 15;
            this.cbDisableLookAt.Text = "Disable Look At beacon";
            this.cbDisableLookAt.UseVisualStyleBackColor = true;
            // 
            // cbTrasactDialog
            // 
            this.cbTrasactDialog.AutoSize = true;
            this.cbTrasactDialog.Location = new System.Drawing.Point(6, 111);
            this.cbTrasactDialog.Name = "cbTrasactDialog";
            this.cbTrasactDialog.Size = new System.Drawing.Size(176, 17);
            this.cbTrasactDialog.TabIndex = 9;
            this.cbTrasactDialog.Text = "Display dialog on L$ transaction";
            this.cbTrasactDialog.UseVisualStyleBackColor = true;
            this.cbTrasactDialog.CheckedChanged += new System.EventHandler(this.cbTrasactDialog_CheckedChanged);
            // 
            // cbRadegastLogToFile
            // 
            this.cbRadegastLogToFile.AutoSize = true;
            this.cbRadegastLogToFile.Location = new System.Drawing.Point(6, 211);
            this.cbRadegastLogToFile.Name = "cbRadegastLogToFile";
            this.cbRadegastLogToFile.Size = new System.Drawing.Size(72, 17);
            this.cbRadegastLogToFile.TabIndex = 14;
            this.cbRadegastLogToFile.Text = "Log to file";
            this.cbRadegastLogToFile.UseVisualStyleBackColor = true;
            this.cbRadegastLogToFile.CheckedChanged += new System.EventHandler(this.cbRadegastLogToFile_CheckedChanged);
            // 
            // cbTrasactChat
            // 
            this.cbTrasactChat.AutoSize = true;
            this.cbTrasactChat.Location = new System.Drawing.Point(6, 131);
            this.cbTrasactChat.Name = "cbTrasactChat";
            this.cbTrasactChat.Size = new System.Drawing.Size(170, 17);
            this.cbTrasactChat.TabIndex = 10;
            this.cbTrasactChat.Text = "Display L$ transactions in chat";
            this.cbTrasactChat.UseVisualStyleBackColor = true;
            this.cbTrasactChat.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbFriendsNotifications
            // 
            this.cbFriendsNotifications.AutoSize = true;
            this.cbFriendsNotifications.Location = new System.Drawing.Point(6, 151);
            this.cbFriendsNotifications.Name = "cbFriendsNotifications";
            this.cbFriendsNotifications.Size = new System.Drawing.Size(184, 17);
            this.cbFriendsNotifications.TabIndex = 11;
            this.cbFriendsNotifications.Text = "Display friends online notifications";
            this.cbFriendsNotifications.UseVisualStyleBackColor = true;
            this.cbFriendsNotifications.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // txtReconnectTime
            // 
            this.txtReconnectTime.Location = new System.Drawing.Point(151, 96);
            this.txtReconnectTime.Name = "txtReconnectTime";
            this.txtReconnectTime.Size = new System.Drawing.Size(53, 20);
            this.txtReconnectTime.TabIndex = 8;
            this.txtReconnectTime.Text = "120";
            this.txtReconnectTime.TextChanged += new System.EventHandler(this.txtReconnectTime_TextChanged);
            // 
            // cbAutoReconnect
            // 
            this.cbAutoReconnect.AutoSize = true;
            this.cbAutoReconnect.Location = new System.Drawing.Point(6, 71);
            this.cbAutoReconnect.Name = "cbAutoReconnect";
            this.cbAutoReconnect.Size = new System.Drawing.Size(169, 17);
            this.cbAutoReconnect.TabIndex = 6;
            this.cbAutoReconnect.Text = "Auto reconnect on disconnect";
            this.cbAutoReconnect.UseVisualStyleBackColor = true;
            this.cbAutoReconnect.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 91);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Auto reconnect time (sec)";
            // 
            // cbRLV
            // 
            this.cbRLV.AutoSize = true;
            this.cbRLV.Location = new System.Drawing.Point(6, 171);
            this.cbRLV.Name = "cbRLV";
            this.cbRLV.Size = new System.Drawing.Size(85, 17);
            this.cbRLV.TabIndex = 12;
            this.cbRLV.Text = "RLV support";
            this.cbRLV.UseVisualStyleBackColor = true;
            this.cbRLV.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbMinToTrey
            // 
            this.cbMinToTrey.AutoSize = true;
            this.cbMinToTrey.Location = new System.Drawing.Point(6, 31);
            this.cbMinToTrey.Name = "cbMinToTrey";
            this.cbMinToTrey.Size = new System.Drawing.Size(133, 17);
            this.cbMinToTrey.TabIndex = 4;
            this.cbMinToTrey.Text = "Minimize to system tray";
            this.cbMinToTrey.UseVisualStyleBackColor = true;
            this.cbMinToTrey.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbRadegastClientTag
            // 
            this.cbRadegastClientTag.AutoSize = true;
            this.cbRadegastClientTag.Location = new System.Drawing.Point(6, 191);
            this.cbRadegastClientTag.Name = "cbRadegastClientTag";
            this.cbRadegastClientTag.Size = new System.Drawing.Size(146, 17);
            this.cbRadegastClientTag.TabIndex = 13;
            this.cbRadegastClientTag.Text = "Send Radegast client tag";
            this.cbRadegastClientTag.UseVisualStyleBackColor = true;
            // 
            // cbSyntaxHighlight
            // 
            this.cbSyntaxHighlight.AutoSize = true;
            this.cbSyntaxHighlight.Location = new System.Drawing.Point(6, 51);
            this.cbSyntaxHighlight.Name = "cbSyntaxHighlight";
            this.cbSyntaxHighlight.Size = new System.Drawing.Size(134, 17);
            this.cbSyntaxHighlight.TabIndex = 5;
            this.cbSyntaxHighlight.Text = "LSL syntax highlighting";
            this.cbSyntaxHighlight.UseVisualStyleBackColor = true;
            // 
            // Chat
            // 
            this.Chat.Controls.Add(this.cbDisableChatIMLog);
            this.Chat.Controls.Add(this.cbChatTimestamps);
            this.Chat.Controls.Add(this.cbIMTimeStamps);
            this.Chat.Controls.Add(this.cbMUEmotes);
            this.Chat.Controls.Add(this.cbNoTyping);
            this.Chat.Controls.Add(this.cbFontSize);
            this.Chat.Controls.Add(this.label1);
            this.Chat.Location = new System.Drawing.Point(8, 5);
            this.Chat.Name = "Chat";
            this.Chat.Size = new System.Drawing.Size(256, 149);
            this.Chat.TabIndex = 0;
            this.Chat.TabStop = false;
            this.Chat.Text = "Chat";
            // 
            // cbDisableChatIMLog
            // 
            this.cbDisableChatIMLog.AutoSize = true;
            this.cbDisableChatIMLog.Location = new System.Drawing.Point(8, 121);
            this.cbDisableChatIMLog.Name = "cbDisableChatIMLog";
            this.cbDisableChatIMLog.Size = new System.Drawing.Size(138, 17);
            this.cbDisableChatIMLog.TabIndex = 8;
            this.cbDisableChatIMLog.Text = "Disable chat and IM log";
            this.cbDisableChatIMLog.UseVisualStyleBackColor = true;
            // 
            // cbChatTimestamps
            // 
            this.cbChatTimestamps.AutoSize = true;
            this.cbChatTimestamps.Location = new System.Drawing.Point(8, 12);
            this.cbChatTimestamps.Name = "cbChatTimestamps";
            this.cbChatTimestamps.Size = new System.Drawing.Size(143, 17);
            this.cbChatTimestamps.TabIndex = 0;
            this.cbChatTimestamps.Text = "Show timestamps in chat";
            this.cbChatTimestamps.UseVisualStyleBackColor = true;
            // 
            // cbIMTimeStamps
            // 
            this.cbIMTimeStamps.AutoSize = true;
            this.cbIMTimeStamps.Location = new System.Drawing.Point(8, 35);
            this.cbIMTimeStamps.Name = "cbIMTimeStamps";
            this.cbIMTimeStamps.Size = new System.Drawing.Size(137, 17);
            this.cbIMTimeStamps.TabIndex = 1;
            this.cbIMTimeStamps.Text = "Show timestamps in  IM";
            this.cbIMTimeStamps.UseVisualStyleBackColor = true;
            // 
            // cbMUEmotes
            // 
            this.cbMUEmotes.AutoSize = true;
            this.cbMUEmotes.Location = new System.Drawing.Point(8, 78);
            this.cbMUEmotes.Name = "cbMUEmotes";
            this.cbMUEmotes.Size = new System.Drawing.Size(108, 17);
            this.cbMUEmotes.TabIndex = 5;
            this.cbMUEmotes.Text = "MU* style emotes";
            this.cbMUEmotes.UseVisualStyleBackColor = true;
            this.cbMUEmotes.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbNoTyping
            // 
            this.cbNoTyping.AutoSize = true;
            this.cbNoTyping.Location = new System.Drawing.Point(8, 98);
            this.cbNoTyping.Name = "cbNoTyping";
            this.cbNoTyping.Size = new System.Drawing.Size(150, 17);
            this.cbNoTyping.TabIndex = 6;
            this.cbNoTyping.Text = "Don\'t use typing animation";
            this.cbNoTyping.UseVisualStyleBackColor = true;
            this.cbNoTyping.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbTaskBarHighLight
            // 
            this.cbTaskBarHighLight.AutoSize = true;
            this.cbTaskBarHighLight.Location = new System.Drawing.Point(6, 19);
            this.cbTaskBarHighLight.Name = "cbTaskBarHighLight";
            this.cbTaskBarHighLight.Size = new System.Drawing.Size(115, 17);
            this.cbTaskBarHighLight.TabIndex = 1;
            this.cbTaskBarHighLight.Text = "Enable highlighting";
            this.cbTaskBarHighLight.UseVisualStyleBackColor = true;
            // 
            // cbFontSize
            // 
            this.cbFontSize.FormatString = "N2";
            this.cbFontSize.FormattingEnabled = true;
            this.cbFontSize.Items.AddRange(new object[] {
            "8.25",
            "9",
            "10",
            "12",
            "14",
            "16",
            "20"});
            this.cbFontSize.Location = new System.Drawing.Point(97, 51);
            this.cbFontSize.Name = "cbFontSize";
            this.cbFontSize.Size = new System.Drawing.Size(54, 21);
            this.cbFontSize.TabIndex = 3;
            this.cbFontSize.Text = "8.25";
            this.cbFontSize.SelectedIndexChanged += new System.EventHandler(this.cbFontSize_SelectedIndexChanged);
            this.cbFontSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbFontSize_KeyDown);
            this.cbFontSize.Leave += new System.EventHandler(this.cbFontSize_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Chat text size:";
            // 
            // gbDisplayNames
            // 
            this.gbDisplayNames.Controls.Add(this.rbDNOnlyDN);
            this.gbDisplayNames.Controls.Add(this.rbDNDandUsernme);
            this.gbDisplayNames.Controls.Add(this.rbDNSmart);
            this.gbDisplayNames.Controls.Add(this.rbDNOff);
            this.gbDisplayNames.Location = new System.Drawing.Point(270, 272);
            this.gbDisplayNames.Name = "gbDisplayNames";
            this.gbDisplayNames.Size = new System.Drawing.Size(256, 100);
            this.gbDisplayNames.TabIndex = 3;
            this.gbDisplayNames.TabStop = false;
            this.gbDisplayNames.Text = "Display names";
            // 
            // rbDNOnlyDN
            // 
            this.rbDNOnlyDN.AutoSize = true;
            this.rbDNOnlyDN.Location = new System.Drawing.Point(6, 75);
            this.rbDNOnlyDN.Name = "rbDNOnlyDN";
            this.rbDNOnlyDN.Size = new System.Drawing.Size(110, 17);
            this.rbDNOnlyDN.TabIndex = 3;
            this.rbDNOnlyDN.TabStop = true;
            this.rbDNOnlyDN.Text = "Only display name";
            this.rbDNOnlyDN.UseVisualStyleBackColor = true;
            this.rbDNOnlyDN.CheckedChanged += new System.EventHandler(this.rbDNOnlyDN_CheckedChanged);
            // 
            // rbDNDandUsernme
            // 
            this.rbDNDandUsernme.AutoSize = true;
            this.rbDNDandUsernme.Location = new System.Drawing.Point(6, 55);
            this.rbDNDandUsernme.Name = "rbDNDandUsernme";
            this.rbDNDandUsernme.Size = new System.Drawing.Size(143, 17);
            this.rbDNDandUsernme.TabIndex = 2;
            this.rbDNDandUsernme.TabStop = true;
            this.rbDNDandUsernme.Text = "Display name (username)";
            this.rbDNDandUsernme.UseVisualStyleBackColor = true;
            this.rbDNDandUsernme.CheckedChanged += new System.EventHandler(this.rbDNDandUsernme_CheckedChanged);
            // 
            // rbDNSmart
            // 
            this.rbDNSmart.AutoSize = true;
            this.rbDNSmart.Location = new System.Drawing.Point(6, 35);
            this.rbDNSmart.Name = "rbDNSmart";
            this.rbDNSmart.Size = new System.Drawing.Size(52, 17);
            this.rbDNSmart.TabIndex = 1;
            this.rbDNSmart.TabStop = true;
            this.rbDNSmart.Text = "Smart";
            this.rbDNSmart.UseVisualStyleBackColor = true;
            this.rbDNSmart.CheckedChanged += new System.EventHandler(this.rbDNSmart_CheckedChanged);
            // 
            // rbDNOff
            // 
            this.rbDNOff.AutoSize = true;
            this.rbDNOff.Location = new System.Drawing.Point(6, 15);
            this.rbDNOff.Name = "rbDNOff";
            this.rbDNOff.Size = new System.Drawing.Size(39, 17);
            this.rbDNOff.TabIndex = 0;
            this.rbDNOff.TabStop = true;
            this.rbDNOff.Text = "Off";
            this.rbDNOff.UseVisualStyleBackColor = true;
            this.rbDNOff.CheckedChanged += new System.EventHandler(this.rbDNOff_CheckedChanged);
            // 
            // tbpAutoResponse
            // 
            this.tbpAutoResponse.Controls.Add(this.gnAutoInventory);
            this.tbpAutoResponse.Controls.Add(this.txtAutoResponse);
            this.tbpAutoResponse.Controls.Add(this.gbAutoResponse);
            this.tbpAutoResponse.Location = new System.Drawing.Point(4, 22);
            this.tbpAutoResponse.Name = "tbpAutoResponse";
            this.tbpAutoResponse.Padding = new System.Windows.Forms.Padding(3);
            this.tbpAutoResponse.Size = new System.Drawing.Size(522, 391);
            this.tbpAutoResponse.TabIndex = 2;
            this.tbpAutoResponse.Text = "Auto Response";
            this.tbpAutoResponse.UseVisualStyleBackColor = true;
            // 
            // gnAutoInventory
            // 
            this.gnAutoInventory.Controls.Add(this.cbOnInvOffer);
            this.gnAutoInventory.Location = new System.Drawing.Point(9, 156);
            this.gnAutoInventory.Name = "gnAutoInventory";
            this.gnAutoInventory.Size = new System.Drawing.Size(281, 54);
            this.gnAutoInventory.TabIndex = 2;
            this.gnAutoInventory.TabStop = false;
            this.gnAutoInventory.Text = "On inventory offers";
            // 
            // cbOnInvOffer
            // 
            this.cbOnInvOffer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOnInvOffer.FormattingEnabled = true;
            this.cbOnInvOffer.Items.AddRange(new object[] {
            "Ask",
            "Auto Accept",
            "Auto Discard"});
            this.cbOnInvOffer.Location = new System.Drawing.Point(6, 19);
            this.cbOnInvOffer.Name = "cbOnInvOffer";
            this.cbOnInvOffer.Size = new System.Drawing.Size(121, 21);
            this.cbOnInvOffer.TabIndex = 0;
            // 
            // txtAutoResponse
            // 
            this.txtAutoResponse.AccessibleName = "Auto response text";
            this.txtAutoResponse.Location = new System.Drawing.Point(8, 63);
            this.txtAutoResponse.Multiline = true;
            this.txtAutoResponse.Name = "txtAutoResponse";
            this.txtAutoResponse.Size = new System.Drawing.Size(282, 87);
            this.txtAutoResponse.TabIndex = 1;
            // 
            // gbAutoResponse
            // 
            this.gbAutoResponse.Controls.Add(this.rbAutoAlways);
            this.gbAutoResponse.Controls.Add(this.rbAutoNonFriend);
            this.gbAutoResponse.Controls.Add(this.rbAutobusy);
            this.gbAutoResponse.Location = new System.Drawing.Point(3, 6);
            this.gbAutoResponse.Name = "gbAutoResponse";
            this.gbAutoResponse.Size = new System.Drawing.Size(287, 51);
            this.gbAutoResponse.TabIndex = 0;
            this.gbAutoResponse.TabStop = false;
            this.gbAutoResponse.Text = "Send auto response when:";
            // 
            // rbAutoAlways
            // 
            this.rbAutoAlways.AutoSize = true;
            this.rbAutoAlways.Location = new System.Drawing.Point(216, 19);
            this.rbAutoAlways.Name = "rbAutoAlways";
            this.rbAutoAlways.Size = new System.Drawing.Size(58, 17);
            this.rbAutoAlways.TabIndex = 2;
            this.rbAutoAlways.Text = "Always";
            this.rbAutoAlways.UseVisualStyleBackColor = true;
            this.rbAutoAlways.CheckedChanged += new System.EventHandler(this.rbAutoAlways_CheckedChanged);
            // 
            // rbAutoNonFriend
            // 
            this.rbAutoNonFriend.AutoSize = true;
            this.rbAutoNonFriend.Location = new System.Drawing.Point(100, 19);
            this.rbAutoNonFriend.Name = "rbAutoNonFriend";
            this.rbAutoNonFriend.Size = new System.Drawing.Size(110, 17);
            this.rbAutoNonFriend.TabIndex = 1;
            this.rbAutoNonFriend.Text = "IM from non-friend";
            this.rbAutoNonFriend.UseVisualStyleBackColor = true;
            this.rbAutoNonFriend.CheckedChanged += new System.EventHandler(this.rbAutoNonFriend_CheckedChanged);
            // 
            // rbAutobusy
            // 
            this.rbAutobusy.AutoSize = true;
            this.rbAutobusy.Checked = true;
            this.rbAutobusy.Location = new System.Drawing.Point(6, 19);
            this.rbAutobusy.Name = "rbAutobusy";
            this.rbAutobusy.Size = new System.Drawing.Size(88, 17);
            this.rbAutobusy.TabIndex = 0;
            this.rbAutobusy.TabStop = true;
            this.rbAutobusy.Text = "In busy mode";
            this.rbAutobusy.UseVisualStyleBackColor = true;
            this.rbAutobusy.CheckedChanged += new System.EventHandler(this.rbAutobusy_CheckedChanged);
            // 
            // tbpGraphics
            // 
            this.tbpGraphics.Location = new System.Drawing.Point(4, 22);
            this.tbpGraphics.Name = "tbpGraphics";
            this.tbpGraphics.Padding = new System.Windows.Forms.Padding(3);
            this.tbpGraphics.Size = new System.Drawing.Size(522, 391);
            this.tbpGraphics.TabIndex = 3;
            this.tbpGraphics.Text = "Graphics Settings";
            this.tbpGraphics.UseVisualStyleBackColor = true;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 403);
            this.Controls.Add(this.tcGraphics);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSettings";
            this.Text = "Settings - Radegast";
            this.tcGraphics.ResumeLayout(false);
            this.tbpGeneral.ResumeLayout(false);
            this.cbHighLight.ResumeLayout(false);
            this.cbHighLight.PerformLayout();
            this.cbMisc.ResumeLayout(false);
            this.cbMisc.PerformLayout();
            this.Chat.ResumeLayout(false);
            this.Chat.PerformLayout();
            this.gbDisplayNames.ResumeLayout(false);
            this.gbDisplayNames.PerformLayout();
            this.tbpAutoResponse.ResumeLayout(false);
            this.tbpAutoResponse.PerformLayout();
            this.gnAutoInventory.ResumeLayout(false);
            this.gbAutoResponse.ResumeLayout(false);
            this.gbAutoResponse.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cbFontSize;
        public System.Windows.Forms.TabControl tcGraphics;
        public System.Windows.Forms.TabPage tbpGeneral;
        public System.Windows.Forms.CheckBox cbIMTimeStamps;
        public System.Windows.Forms.CheckBox cbChatTimestamps;
        public System.Windows.Forms.CheckBox cbTrasactChat;
        public System.Windows.Forms.CheckBox cbTrasactDialog;
        public System.Windows.Forms.CheckBox cbFriendsNotifications;
        public System.Windows.Forms.CheckBox cbAutoReconnect;
        public System.Windows.Forms.CheckBox cbHideLoginGraphics;
        public System.Windows.Forms.CheckBox cbRLV;
        public System.Windows.Forms.CheckBox cbMUEmotes;
        public System.Windows.Forms.CheckBox cbFriendsHighlight;
        public System.Windows.Forms.CheckBox cbMinToTrey;
        public System.Windows.Forms.CheckBox cbNoTyping;
        public System.Windows.Forms.TextBox txtReconnectTime;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.TabPage tbpAutoResponse;
        public System.Windows.Forms.GroupBox gbAutoResponse;
        public System.Windows.Forms.TextBox txtAutoResponse;
        public System.Windows.Forms.RadioButton rbAutoAlways;
        public System.Windows.Forms.RadioButton rbAutoNonFriend;
        public System.Windows.Forms.RadioButton rbAutobusy;
        public System.Windows.Forms.CheckBox cbSyntaxHighlight;
        public System.Windows.Forms.GroupBox gbDisplayNames;
        public System.Windows.Forms.RadioButton rbDNOnlyDN;
        public System.Windows.Forms.RadioButton rbDNDandUsernme;
        public System.Windows.Forms.RadioButton rbDNSmart;
        public System.Windows.Forms.RadioButton rbDNOff;
        public System.Windows.Forms.CheckBox cbTaskBarHighLight;
        public System.Windows.Forms.TabPage tbpGraphics;
        public System.Windows.Forms.CheckBox cbRadegastClientTag;
        public System.Windows.Forms.GroupBox gnAutoInventory;
        public System.Windows.Forms.ComboBox cbOnInvOffer;
        public System.Windows.Forms.CheckBox cbDisableChatIMLog;
        public System.Windows.Forms.GroupBox Chat;
        public System.Windows.Forms.CheckBox cbRadegastLogToFile;
        public System.Windows.Forms.GroupBox cbMisc;
        public System.Windows.Forms.CheckBox cbDisableLookAt;
        public System.Windows.Forms.GroupBox cbHighLight;
        public System.Windows.Forms.CheckBox cbHighlightGroupIM;
        public System.Windows.Forms.CheckBox cbHighlightIM;
        private System.Windows.Forms.CheckBox cbHighlightChat;


    }
}
