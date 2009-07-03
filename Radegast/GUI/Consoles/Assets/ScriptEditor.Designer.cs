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
    partial class ScriptEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditor));
            this.ttKeyWords = new System.Windows.Forms.ToolTip(this.components);
            this.strpMenu = new System.Windows.Forms.ToolStrip();
            this.tbtbFile = new System.Windows.Forms.ToolStripDropDownButton();
            this.tbtbLoadFromDisk = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtbSave = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtbSaveToDisk = new System.Windows.Forms.ToolStripMenuItem();
            this.tSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tbtnExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tbtnAttach = new System.Windows.Forms.ToolStripButton();
            this.strpStatus = new System.Windows.Forms.ToolStrip();
            this.lblScripStatus = new System.Windows.Forms.ToolStripLabel();
            this.lblLine = new System.Windows.Forms.ToolStripLabel();
            this.lblCol = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.rtbCode = new Radegast.RRichTextBox();
            this.strpMenu.SuspendLayout();
            this.strpStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // strpMenu
            // 
            this.strpMenu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.strpMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtbFile,
            this.tbtnAttach});
            this.strpMenu.Location = new System.Drawing.Point(0, 0);
            this.strpMenu.Name = "strpMenu";
            this.strpMenu.Size = new System.Drawing.Size(661, 25);
            this.strpMenu.TabIndex = 2;
            // 
            // tbtbFile
            // 
            this.tbtbFile.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtbFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tbtbLoadFromDisk,
            this.tbtbSave,
            this.tbtbSaveToDisk,
            this.tSeparator1,
            this.tbtnExit});
            this.tbtbFile.Image = ((System.Drawing.Image)(resources.GetObject("tbtbFile.Image")));
            this.tbtbFile.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtbFile.Name = "tbtbFile";
            this.tbtbFile.Size = new System.Drawing.Size(38, 22);
            this.tbtbFile.Text = "File";
            // 
            // tbtbLoadFromDisk
            // 
            this.tbtbLoadFromDisk.Name = "tbtbLoadFromDisk";
            this.tbtbLoadFromDisk.Size = new System.Drawing.Size(165, 22);
            this.tbtbLoadFromDisk.Text = "Open...";
            this.tbtbLoadFromDisk.Click += new System.EventHandler(this.tbtbLoadFromDisk_Click);
            // 
            // tbtbSave
            // 
            this.tbtbSave.Enabled = false;
            this.tbtbSave.Name = "tbtbSave";
            this.tbtbSave.Size = new System.Drawing.Size(165, 22);
            this.tbtbSave.Text = "Save to inventory";
            // 
            // tbtbSaveToDisk
            // 
            this.tbtbSaveToDisk.Name = "tbtbSaveToDisk";
            this.tbtbSaveToDisk.Size = new System.Drawing.Size(165, 22);
            this.tbtbSaveToDisk.Text = "Save to disk...";
            this.tbtbSaveToDisk.Click += new System.EventHandler(this.tbtbSaveToDisk_Click);
            // 
            // tSeparator1
            // 
            this.tSeparator1.Name = "tSeparator1";
            this.tSeparator1.Size = new System.Drawing.Size(162, 6);
            this.tSeparator1.Visible = false;
            // 
            // tbtnExit
            // 
            this.tbtnExit.Name = "tbtnExit";
            this.tbtnExit.Size = new System.Drawing.Size(165, 22);
            this.tbtnExit.Text = "Close";
            this.tbtnExit.Visible = false;
            this.tbtnExit.Click += new System.EventHandler(this.tbtnExit_Click);
            // 
            // tbtnAttach
            // 
            this.tbtnAttach.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tbtnAttach.Image = ((System.Drawing.Image)(resources.GetObject("tbtnAttach.Image")));
            this.tbtnAttach.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tbtnAttach.Name = "tbtnAttach";
            this.tbtnAttach.Size = new System.Drawing.Size(48, 22);
            this.tbtnAttach.Text = "Detach";
            this.tbtnAttach.Click += new System.EventHandler(this.tbtnAttach_Click);
            // 
            // strpStatus
            // 
            this.strpStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.strpStatus.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.strpStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblScripStatus,
            this.lblCol,
            this.toolStripSeparator1,
            this.lblLine,
            this.toolStripSeparator2});
            this.strpStatus.Location = new System.Drawing.Point(0, 420);
            this.strpStatus.Name = "strpStatus";
            this.strpStatus.Size = new System.Drawing.Size(661, 25);
            this.strpStatus.TabIndex = 3;
            // 
            // lblScripStatus
            // 
            this.lblScripStatus.Name = "lblScripStatus";
            this.lblScripStatus.Size = new System.Drawing.Size(59, 22);
            this.lblScripStatus.Text = "Loading...";
            // 
            // lblLine
            // 
            this.lblLine.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblLine.AutoSize = false;
            this.lblLine.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(55, 22);
            this.lblLine.Text = "Ln 1";
            this.lblLine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCol
            // 
            this.lblCol.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblCol.AutoSize = false;
            this.lblCol.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblCol.Name = "lblCol";
            this.lblCol.Size = new System.Drawing.Size(55, 22);
            this.lblCol.Text = "Col 1";
            this.lblCol.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // rtbCode
            // 
            this.rtbCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(255)))), ((int)(((byte)(250)))));
            this.rtbCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbCode.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbCode.HideSelection = false;
            this.rtbCode.Location = new System.Drawing.Point(0, 25);
            this.rtbCode.Name = "rtbCode";
            this.rtbCode.Size = new System.Drawing.Size(661, 395);
            this.rtbCode.TabIndex = 1;
            this.rtbCode.Text = "";
            this.rtbCode.WordWrap = false;
            this.rtbCode.SelectionChanged += new System.EventHandler(this.rtbCode_SelectionChanged);
            this.rtbCode.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rtbCode_MouseMove);
            this.rtbCode.TextChanged += new System.EventHandler(this.rtbCode_TextChanged);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtbCode);
            this.Controls.Add(this.strpStatus);
            this.Controls.Add(this.strpMenu);
            this.Name = "ScriptEditor";
            this.Size = new System.Drawing.Size(661, 445);
            this.strpMenu.ResumeLayout(false);
            this.strpMenu.PerformLayout();
            this.strpStatus.ResumeLayout(false);
            this.strpStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip ttKeyWords;
        private RRichTextBox rtbCode;
        private System.Windows.Forms.ToolStrip strpMenu;
        private System.Windows.Forms.ToolStripDropDownButton tbtbFile;
        private System.Windows.Forms.ToolStripMenuItem tbtbSave;
        private System.Windows.Forms.ToolStripMenuItem tbtbSaveToDisk;
        private System.Windows.Forms.ToolStripMenuItem tbtbLoadFromDisk;
        private System.Windows.Forms.ToolStripButton tbtnAttach;
        private System.Windows.Forms.ToolStripSeparator tSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tbtnExit;
        private System.Windows.Forms.ToolStrip strpStatus;
        private System.Windows.Forms.ToolStripLabel lblScripStatus;
        private System.Windows.Forms.ToolStripLabel lblCol;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel lblLine;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}
