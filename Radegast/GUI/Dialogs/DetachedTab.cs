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
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Radegast
{
    public partial class frmDetachedTab : RadegastForm
    {
        public bool DockedToMain;

        private RadegastInstance instance;
        private RadegastTab tab;

        //For reattachment
        private ToolStrip strip;
        private Panel container;

        int mainTop;
        int mainLeft;
        Size mainSize;

        public frmDetachedTab(RadegastInstance instance, RadegastTab tab)
            :base(instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmDetachedTab_Disposed);

            this.instance = instance;
            this.tab = tab;
            ClientSize = tab.Control.Size;
            Controls.Add(tab.Control);
            tab.Control.Visible = true;
            tab.Control.BringToFront();
            tab.Control.TextChanged += new EventHandler(Control_TextChanged);
            SettingsKeyBase = "tab_window_" + tab.Control.GetType().Name;
            AutoSavePosition = true;
            instance.MainForm.Move += new EventHandler(MainForm_ResizeEnd);
            SaveMainFormPos();
            if (tab.Floater)
            {
                Owner = instance.MainForm;
            }

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void Control_TextChanged(object sender, EventArgs e)
        {
            Text = tab.Control.Text;
        }

        void frmDetachedTab_Disposed(object sender, EventArgs e)
        {
            instance.MainForm.Move -= new EventHandler(MainForm_ResizeEnd);
        }

        void SaveMainFormPos()
        {
            mainTop = instance.MainForm.Top;
            mainLeft = instance.MainForm.Left;
            mainSize = instance.MainForm.Size;
        }

        void MainForm_ResizeEnd(object sender, EventArgs e)
        {
            if (DockedToMain)
            {
                Left += (instance.MainForm.Left - mainLeft);
                Top += (instance.MainForm.Top - mainTop);
            }
            SaveMainFormPos();
        }

        private void frmDetachedTab_FormClosing(object sender, FormClosingEventArgs e)
        {
            tab.Control.TextChanged -= new EventHandler(Control_TextChanged);
            if (tab.Detached)
            {
                if (tab.CloseOnDetachedClose)
                    tab.Close();
                else
                    tab.AttachTo(strip, container);
            }
        }

        public ToolStrip ReattachStrip
        {
            get { return strip; }
            set { strip = value; }
        }

        public Panel ReattachContainer
        {
            get { return container; }
            set { container = value; }
        }

        private void UpdatePos()
        {
            Rectangle mainRect = new Rectangle(new Point(mainLeft, mainTop), mainSize);
            Rectangle myRect = new Rectangle(new Point(Left, Top), Size);
            
            bool oldDocked = DockedToMain; 

            if (tab.Floater && mainRect == Rectangle.Union(mainRect, myRect))
            {
                DockedToMain = true;
                ShowInTaskbar = false;
                Text = tab.Label;
            }
            else
            {
                DockedToMain = false;
                if (!ShowInTaskbar)
                    ShowInTaskbar = true;
                this.Text = tab.Label + " - " + Properties.Resources.ProgramName;
            }
        }

        private void frmDetachedTab_Shown(object sender, EventArgs e)
        {
            UpdatePos();
        }

        private void frmDetachedTab_ResizeEnd(object sender, EventArgs e)
        {
            UpdatePos();
        }

        private void frmDetachedTab_Deactivate(object sender, EventArgs e)
        {
            if (tab.Floater)
            {
                Opacity = 0.5;
            }
        }

        private void frmDetachedTab_Activated(object sender, EventArgs e)
        {
            if (tab.Floater)
            {
                Opacity = 1;
            }
        }
    }
}