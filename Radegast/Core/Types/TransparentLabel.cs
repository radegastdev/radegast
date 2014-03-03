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
    class TransparentLabel : Label
    {
        public TransparentLabel()
            : base()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        //private void PaintParentBackground(Graphics g)
        //{
        //    if (Parent != null)
        //    {
        //        Rectangle rect = new Rectangle(Left, Top, Width, Height);

        //        g.TranslateTransform(-rect.X, -rect.Y);

        //        try
        //        {
        //            using (PaintEventArgs pea =
        //                        new PaintEventArgs(g, rect))
        //            {
        //                pea.Graphics.SetClip(rect);
        //                InvokePaintBackground(Parent, pea);
        //                InvokePaint(Parent, pea);
        //            }
        //        }
        //        finally
        //        {
        //           g.TranslateTransform(rect.X, rect.Y);
        //        }
        //    }
        //    else
        //    {
        //        g.FillRectangle(SystemBrushes.Control, ClientRectangle);
        //    }
        //}

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    PaintParentBackground(e.Graphics);
        //    e.Graphics.DrawString("foo", Font, new SolidBrush(ForeColor), 0, 0);
        //}

        //protected override void OnPaintBackground(PaintEventArgs pevent)
        //{
        //    PaintParentBackground(pevent.Graphics);
        //}

    }
}
