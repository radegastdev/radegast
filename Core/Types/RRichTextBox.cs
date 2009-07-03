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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radegast
{

    public class RRichTextBox : RichTextBox
    {

        private const short WM_PAINT = 0x00f;
        private bool _Paint = true;

        public RRichTextBox()
            : base()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            // sometimes we want to eat the paint message so we don't have to see all the 
            //  flicker from when we select the text to change the color.
            if (m.Msg == WM_PAINT)
            {
                if (_Paint)
                    base.WndProc(ref m);
                else
                    m.Result = IntPtr.Zero;
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_Paint)
            {
                base.OnPaint(e);
            }
        }

        public virtual void BeginUpdate()
        {
            _Paint = false;
            SuspendLayout();
        }

        public virtual void EndUpdate()
        {
            ResumeLayout();
            _Paint = true;
        }

        public struct CursorLocation
        {
            public CursorLocation(int Line, int Column)
            {
                this.Line = Line;
                this.Column = Column;
            }

            public int Line;
            public int Column;

            public override string ToString()
            {
                return string.Format("Ln {0}  Col {1}", Line + 1, Column + 1);
            }
        }

        public CursorLocation CursorPosition
        {
            get
            {
                int currentLine = GetLineFromCharIndex(SelectionStart);
                int currentCol = 0;
                int offset = 0;
                int i = 0;

                foreach (string line in Lines)
                {
                    if (i < currentLine)
                    {
                        offset += line.Length + 1;
                    }
                    else
                    {
                        break;
                    }
                    i++;
                }

                currentCol = SelectionStart - offset;
                return new CursorLocation(currentLine, currentCol);
            }

            set
            {
                int Offset = 0;
                int i = 0;

                foreach (String L in Lines)
                {
                    if (i < value.Line)
                    {
                        Offset += L.Length + 1;
                    }
                    else
                    {
                        break;
                    }

                    i++;
                }

                Select(Offset + value.Column, 0);
            }
        }
    }
}