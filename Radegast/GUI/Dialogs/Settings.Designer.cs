// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tbpGeneral = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.cbFontSize = new System.Windows.Forms.ComboBox();
            this.cbMinToTrey = new System.Windows.Forms.CheckBox();
            this.cbMUEmotes = new System.Windows.Forms.CheckBox();
            this.cbRLV = new System.Windows.Forms.CheckBox();
            this.cbHideLoginGraphics = new System.Windows.Forms.CheckBox();
            this.cbAutoReconnect = new System.Windows.Forms.CheckBox();
            this.cbFriendsHighlight = new System.Windows.Forms.CheckBox();
            this.cbFriendsNotifications = new System.Windows.Forms.CheckBox();
            this.cbTrasactChat = new System.Windows.Forms.CheckBox();
            this.cbTrasactDialog = new System.Windows.Forms.CheckBox();
            this.cbIMTimeStamps = new System.Windows.Forms.CheckBox();
            this.cbChatTimestamps = new System.Windows.Forms.CheckBox();
            this.tabControl1.SuspendLayout();
            this.tbpGeneral.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tbpGeneral);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(464, 333);
            this.tabControl1.TabIndex = 0;
            // 
            // tbpGeneral
            // 
            this.tbpGeneral.Controls.Add(this.label1);
            this.tbpGeneral.Controls.Add(this.cbFontSize);
            this.tbpGeneral.Controls.Add(this.cbMinToTrey);
            this.tbpGeneral.Controls.Add(this.cbMUEmotes);
            this.tbpGeneral.Controls.Add(this.cbRLV);
            this.tbpGeneral.Controls.Add(this.cbHideLoginGraphics);
            this.tbpGeneral.Controls.Add(this.cbAutoReconnect);
            this.tbpGeneral.Controls.Add(this.cbFriendsHighlight);
            this.tbpGeneral.Controls.Add(this.cbFriendsNotifications);
            this.tbpGeneral.Controls.Add(this.cbTrasactChat);
            this.tbpGeneral.Controls.Add(this.cbTrasactDialog);
            this.tbpGeneral.Controls.Add(this.cbIMTimeStamps);
            this.tbpGeneral.Controls.Add(this.cbChatTimestamps);
            this.tbpGeneral.Location = new System.Drawing.Point(4, 22);
            this.tbpGeneral.Name = "tbpGeneral";
            this.tbpGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tbpGeneral.Size = new System.Drawing.Size(456, 307);
            this.tbpGeneral.TabIndex = 1;
            this.tbpGeneral.Text = "General";
            this.tbpGeneral.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 239);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Chat text size:";
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
            this.cbFontSize.Location = new System.Drawing.Point(87, 236);
            this.cbFontSize.Name = "cbFontSize";
            this.cbFontSize.Size = new System.Drawing.Size(54, 21);
            this.cbFontSize.TabIndex = 11;
            this.cbFontSize.Text = "8.25";
            this.cbFontSize.SelectedIndexChanged += new System.EventHandler(this.cbFontSize_SelectedIndexChanged);
            this.cbFontSize.Leave += new System.EventHandler(this.cbFontSize_Leave);
            this.cbFontSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cbFontSize_KeyDown);
            // 
            // cbMinToTrey
            // 
            this.cbMinToTrey.AutoSize = true;
            this.cbMinToTrey.Location = new System.Drawing.Point(8, 263);
            this.cbMinToTrey.Name = "cbMinToTrey";
            this.cbMinToTrey.Size = new System.Drawing.Size(133, 17);
            this.cbMinToTrey.TabIndex = 12;
            this.cbMinToTrey.Text = "Minimize to system trey";
            this.cbMinToTrey.UseVisualStyleBackColor = true;
            this.cbMinToTrey.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbMUEmotes
            // 
            this.cbMUEmotes.AutoSize = true;
            this.cbMUEmotes.Location = new System.Drawing.Point(8, 213);
            this.cbMUEmotes.Name = "cbMUEmotes";
            this.cbMUEmotes.Size = new System.Drawing.Size(108, 17);
            this.cbMUEmotes.TabIndex = 9;
            this.cbMUEmotes.Text = "MU* style emotes";
            this.cbMUEmotes.UseVisualStyleBackColor = true;
            this.cbMUEmotes.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbRLV
            // 
            this.cbRLV.AutoSize = true;
            this.cbRLV.Location = new System.Drawing.Point(8, 190);
            this.cbRLV.Name = "cbRLV";
            this.cbRLV.Size = new System.Drawing.Size(85, 17);
            this.cbRLV.TabIndex = 8;
            this.cbRLV.Text = "RLV support";
            this.cbRLV.UseVisualStyleBackColor = true;
            this.cbRLV.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbHideLoginGraphics
            // 
            this.cbHideLoginGraphics.AutoSize = true;
            this.cbHideLoginGraphics.Location = new System.Drawing.Point(8, 167);
            this.cbHideLoginGraphics.Name = "cbHideLoginGraphics";
            this.cbHideLoginGraphics.Size = new System.Drawing.Size(141, 17);
            this.cbHideLoginGraphics.TabIndex = 7;
            this.cbHideLoginGraphics.Text = "Hide login slpash screen";
            this.cbHideLoginGraphics.UseVisualStyleBackColor = true;
            this.cbHideLoginGraphics.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbAutoReconnect
            // 
            this.cbAutoReconnect.AutoSize = true;
            this.cbAutoReconnect.Location = new System.Drawing.Point(8, 144);
            this.cbAutoReconnect.Name = "cbAutoReconnect";
            this.cbAutoReconnect.Size = new System.Drawing.Size(169, 17);
            this.cbAutoReconnect.TabIndex = 6;
            this.cbAutoReconnect.Text = "Auto reconnect on disconnect";
            this.cbAutoReconnect.UseVisualStyleBackColor = true;
            this.cbAutoReconnect.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbFriendsHighlight
            // 
            this.cbFriendsHighlight.AutoSize = true;
            this.cbFriendsHighlight.Location = new System.Drawing.Point(8, 121);
            this.cbFriendsHighlight.Name = "cbFriendsHighlight";
            this.cbFriendsHighlight.Size = new System.Drawing.Size(248, 17);
            this.cbFriendsHighlight.TabIndex = 5;
            this.cbFriendsHighlight.Text = "Highlight chat tab on friends online notifications";
            this.cbFriendsHighlight.UseVisualStyleBackColor = true;
            this.cbFriendsHighlight.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbFriendsNotifications
            // 
            this.cbFriendsNotifications.AutoSize = true;
            this.cbFriendsNotifications.Location = new System.Drawing.Point(8, 98);
            this.cbFriendsNotifications.Name = "cbFriendsNotifications";
            this.cbFriendsNotifications.Size = new System.Drawing.Size(184, 17);
            this.cbFriendsNotifications.TabIndex = 4;
            this.cbFriendsNotifications.Text = "Display friends online notifications";
            this.cbFriendsNotifications.UseVisualStyleBackColor = true;
            this.cbFriendsNotifications.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbTrasactChat
            // 
            this.cbTrasactChat.AutoSize = true;
            this.cbTrasactChat.Location = new System.Drawing.Point(8, 75);
            this.cbTrasactChat.Name = "cbTrasactChat";
            this.cbTrasactChat.Size = new System.Drawing.Size(170, 17);
            this.cbTrasactChat.TabIndex = 3;
            this.cbTrasactChat.Text = "Display L$ transactions in chat";
            this.cbTrasactChat.UseVisualStyleBackColor = true;
            this.cbTrasactChat.CheckedChanged += new System.EventHandler(this.cbTrasactChat_CheckedChanged);
            // 
            // cbTrasactDialog
            // 
            this.cbTrasactDialog.AutoSize = true;
            this.cbTrasactDialog.Location = new System.Drawing.Point(8, 52);
            this.cbTrasactDialog.Name = "cbTrasactDialog";
            this.cbTrasactDialog.Size = new System.Drawing.Size(176, 17);
            this.cbTrasactDialog.TabIndex = 2;
            this.cbTrasactDialog.Text = "Display dialog on L$ transaction";
            this.cbTrasactDialog.UseVisualStyleBackColor = true;
            this.cbTrasactDialog.CheckedChanged += new System.EventHandler(this.cbTrasactDialog_CheckedChanged);
            // 
            // cbIMTimeStamps
            // 
            this.cbIMTimeStamps.AutoSize = true;
            this.cbIMTimeStamps.Location = new System.Drawing.Point(8, 29);
            this.cbIMTimeStamps.Name = "cbIMTimeStamps";
            this.cbIMTimeStamps.Size = new System.Drawing.Size(137, 17);
            this.cbIMTimeStamps.TabIndex = 1;
            this.cbIMTimeStamps.Text = "Show timestamps in  IM";
            this.cbIMTimeStamps.UseVisualStyleBackColor = true;
            // 
            // cbChatTimestamps
            // 
            this.cbChatTimestamps.AutoSize = true;
            this.cbChatTimestamps.Location = new System.Drawing.Point(8, 6);
            this.cbChatTimestamps.Name = "cbChatTimestamps";
            this.cbChatTimestamps.Size = new System.Drawing.Size(143, 17);
            this.cbChatTimestamps.TabIndex = 0;
            this.cbChatTimestamps.Text = "Show timestamps in chat";
            this.cbChatTimestamps.UseVisualStyleBackColor = true;
            // 
            // frmSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 333);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmSettings";
            this.Text = "Settings - Radegast";
            this.tabControl1.ResumeLayout(false);
            this.tbpGeneral.ResumeLayout(false);
            this.tbpGeneral.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbFontSize;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tbpGeneral;
        private System.Windows.Forms.CheckBox cbIMTimeStamps;
        private System.Windows.Forms.CheckBox cbChatTimestamps;
        private System.Windows.Forms.CheckBox cbTrasactChat;
        private System.Windows.Forms.CheckBox cbTrasactDialog;
        private System.Windows.Forms.CheckBox cbFriendsNotifications;
        private System.Windows.Forms.CheckBox cbAutoReconnect;
        private System.Windows.Forms.CheckBox cbHideLoginGraphics;
        private System.Windows.Forms.CheckBox cbRLV;
        private System.Windows.Forms.CheckBox cbMUEmotes;
        private System.Windows.Forms.CheckBox cbFriendsHighlight;
        private System.Windows.Forms.CheckBox cbMinToTrey;

    }
}