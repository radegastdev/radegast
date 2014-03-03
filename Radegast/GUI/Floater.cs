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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Radegast
{
    public class Floater : RadegastForm
    {
        public Control Control, TrackedControl;
        public bool DockedToMain;

        int mainTop;
        int mainLeft;
        Size mainSize;
        Form parentForm;

        public Floater(RadegastInstance instance, Control control, Control trackedControl)
            : base(instance)
        {
            this.Control = control;
            this.TrackedControl = trackedControl;
            SettingsKeyBase = "tab_window_" + control.GetType().Name;
            AutoSavePosition = true;

            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Text = Control.Text;
            
            TrackedControl.ParentChanged += new EventHandler(Control_ParentChanged);
            Control.TextChanged += new EventHandler(Control_TextChanged);
            Activated += new EventHandler(Floater_Activated);
            Deactivate += new EventHandler(Floater_Deactivate);
            ResizeEnd += new EventHandler(Floater_ResizeEnd);
            Shown += new EventHandler(Floater_Shown);
            Load += new EventHandler(Floater_Load);

        }

        void Floater_Load(object sender, EventArgs e)
        {
            Control.Dock = DockStyle.Fill;
            ClientSize = Control.Size;
            Controls.Add(Control);
            SaveMainFormPos();
            parentForm.Move += new EventHandler(parentForm_Move);
        }

        void Floater_Shown(object sender, EventArgs e)
        {
            UpdatePos();
        }

        void Floater_ResizeEnd(object sender, EventArgs e)
        {
            UpdatePos();
        }

        void Floater_Activated(object sender, EventArgs e)
        {
            Opacity = 1;
        }

        void Floater_Deactivate(object sender, EventArgs e)
        {
            if (DockedToMain)
            {
                Opacity = 0.75;
            }
        }

        void Control_TextChanged(object sender, EventArgs e)
        {
            Text = Control.Text;
        }

        void parentForm_Move(object sender, EventArgs e)
        {
            if (DockedToMain)
            {
                Left += (parentForm.Left - mainLeft);
                Top += (parentForm.Top - mainTop);
            }
            SaveMainFormPos();
            UpdatePos();
        }

        void Control_ParentChanged(object sender, EventArgs e)
        {
            if (parentForm != null)
            {
                parentForm.Move -= new EventHandler(parentForm_Move);

            }
            SaveMainFormPos();
            if (parentForm != null)
            {
                Owner = parentForm;
                parentForm.Move += new EventHandler(parentForm_Move);
                UpdatePos();
            }
        }

        void SaveMainFormPos()
        {
            parentForm = TrackedControl.FindForm();
            if (parentForm != null)
            {
                mainTop = parentForm.Top;
                mainLeft = parentForm.Left;
                mainSize = parentForm.Size;
            }
        }

        private void UpdatePos()
        {
            Rectangle mainRect = new Rectangle(new Point(mainLeft, mainTop), mainSize);
            Rectangle myRect = new Rectangle(new Point(Left, Top), Size);

            if (mainRect == Rectangle.Union(mainRect, myRect))
            {
                DockedToMain = true;
            }
            else
            {
                DockedToMain = false;
            }
        }
    }
}
