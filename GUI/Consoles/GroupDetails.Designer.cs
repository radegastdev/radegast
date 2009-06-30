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
    partial class GroupDetails
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
            this.tcGroupDetails = new System.Windows.Forms.TabControl();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.tpMembersRoles = new System.Windows.Forms.TabPage();
            this.tpNotices = new System.Windows.Forms.TabPage();
            this.tcGroupDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // tcGroupDetails
            // 
            this.tcGroupDetails.Controls.Add(this.tpGeneral);
            this.tcGroupDetails.Controls.Add(this.tpMembersRoles);
            this.tcGroupDetails.Controls.Add(this.tpNotices);
            this.tcGroupDetails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcGroupDetails.Location = new System.Drawing.Point(0, 0);
            this.tcGroupDetails.Name = "tcGroupDetails";
            this.tcGroupDetails.SelectedIndex = 0;
            this.tcGroupDetails.Size = new System.Drawing.Size(418, 405);
            this.tcGroupDetails.TabIndex = 0;
            // 
            // tpGeneral
            // 
            this.tpGeneral.Location = new System.Drawing.Point(4, 22);
            this.tpGeneral.Name = "tpGeneral";
            this.tpGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tpGeneral.Size = new System.Drawing.Size(410, 379);
            this.tpGeneral.TabIndex = 0;
            this.tpGeneral.Text = "General";
            this.tpGeneral.UseVisualStyleBackColor = true;
            // 
            // tpMembersRoles
            // 
            this.tpMembersRoles.Location = new System.Drawing.Point(4, 22);
            this.tpMembersRoles.Name = "tpMembersRoles";
            this.tpMembersRoles.Padding = new System.Windows.Forms.Padding(3);
            this.tpMembersRoles.Size = new System.Drawing.Size(410, 379);
            this.tpMembersRoles.TabIndex = 1;
            this.tpMembersRoles.Text = "Members & Roles";
            this.tpMembersRoles.UseVisualStyleBackColor = true;
            // 
            // tpNotices
            // 
            this.tpNotices.Location = new System.Drawing.Point(4, 22);
            this.tpNotices.Name = "tpNotices";
            this.tpNotices.Padding = new System.Windows.Forms.Padding(3);
            this.tpNotices.Size = new System.Drawing.Size(410, 379);
            this.tpNotices.TabIndex = 2;
            this.tpNotices.Text = "Notices";
            this.tpNotices.UseVisualStyleBackColor = true;
            // 
            // GroupDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcGroupDetails);
            this.Name = "GroupDetails";
            this.Size = new System.Drawing.Size(418, 405);
            this.tcGroupDetails.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tcGroupDetails;
        private System.Windows.Forms.TabPage tpGeneral;
        private System.Windows.Forms.TabPage tpMembersRoles;
        private System.Windows.Forms.TabPage tpNotices;
    }
}
