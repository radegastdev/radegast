// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
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
    partial class Landmark
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
            this.pnlControls = new System.Windows.Forms.Panel();
            this.txtParcelDescription = new System.Windows.Forms.TextBox();
            this.btnShowOnMap = new System.Windows.Forms.Button();
            this.txtParcelName = new System.Windows.Forms.TextBox();
            this.btnTeleport = new System.Windows.Forms.Button();
            this.pnlDetail = new System.Windows.Forms.Panel();
            this.pnlControls.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.txtParcelDescription);
            this.pnlControls.Controls.Add(this.btnShowOnMap);
            this.pnlControls.Controls.Add(this.txtParcelName);
            this.pnlControls.Controls.Add(this.btnTeleport);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlControls.Location = new System.Drawing.Point(0, 216);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(320, 103);
            this.pnlControls.TabIndex = 0;
            // 
            // txtParcelDescription
            // 
            this.txtParcelDescription.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtParcelDescription.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtParcelDescription.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtParcelDescription.Location = new System.Drawing.Point(0, 13);
            this.txtParcelDescription.Multiline = true;
            this.txtParcelDescription.Name = "txtParcelDescription";
            this.txtParcelDescription.ReadOnly = true;
            this.txtParcelDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtParcelDescription.Size = new System.Drawing.Size(320, 58);
            this.txtParcelDescription.TabIndex = 1;
            // 
            // btnShowOnMap
            // 
            this.btnShowOnMap.Enabled = false;
            this.btnShowOnMap.Location = new System.Drawing.Point(84, 77);
            this.btnShowOnMap.Name = "btnShowOnMap";
            this.btnShowOnMap.Size = new System.Drawing.Size(97, 23);
            this.btnShowOnMap.TabIndex = 2;
            this.btnShowOnMap.Text = "Show on map";
            this.btnShowOnMap.UseVisualStyleBackColor = true;
            this.btnShowOnMap.Click += new System.EventHandler(this.btnShowOnMap_Click);
            // 
            // txtParcelName
            // 
            this.txtParcelName.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.txtParcelName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtParcelName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtParcelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtParcelName.Location = new System.Drawing.Point(0, 0);
            this.txtParcelName.Name = "txtParcelName";
            this.txtParcelName.ReadOnly = true;
            this.txtParcelName.Size = new System.Drawing.Size(320, 13);
            this.txtParcelName.TabIndex = 0;
            // 
            // btnTeleport
            // 
            this.btnTeleport.Enabled = false;
            this.btnTeleport.Location = new System.Drawing.Point(3, 77);
            this.btnTeleport.Name = "btnTeleport";
            this.btnTeleport.Size = new System.Drawing.Size(75, 23);
            this.btnTeleport.TabIndex = 1;
            this.btnTeleport.Text = "Teleport";
            this.btnTeleport.UseVisualStyleBackColor = true;
            this.btnTeleport.Click += new System.EventHandler(this.btnTeleport_Click);
            // 
            // pnlDetail
            // 
            this.pnlDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDetail.Location = new System.Drawing.Point(0, 0);
            this.pnlDetail.Name = "pnlDetail";
            this.pnlDetail.Size = new System.Drawing.Size(320, 216);
            this.pnlDetail.TabIndex = 1;
            this.pnlDetail.Visible = false;
            // 
            // Landmark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlDetail);
            this.Controls.Add(this.pnlControls);
            this.Name = "Landmark";
            this.Size = new System.Drawing.Size(320, 319);
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel pnlControls;
        public System.Windows.Forms.Button btnShowOnMap;
        public System.Windows.Forms.Button btnTeleport;
        public System.Windows.Forms.Panel pnlDetail;
        public System.Windows.Forms.TextBox txtParcelName;
        public System.Windows.Forms.TextBox txtParcelDescription;

    }
}
