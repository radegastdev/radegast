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
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Radegast
{
    public class RichTextBoxPrinter : ITextPrinter
    {
        private RichTextBox rtb;

        public RichTextBoxPrinter(RichTextBox textBox)
        {
            rtb = textBox;
        }

        #region ITextPrinter Members

        public void PrintText(string text)
        {
            rtb.AppendText(text);
        }

        public void PrintTextLine(string text)
        {
            rtb.AppendText(text + Environment.NewLine);
        }

        public void PrintTextLine(string text, Color color)
        {
            Color c = ForeColor;
            ForeColor = color;
            PrintTextLine(text);
            ForeColor = c;
        }

        public void ClearText()
        {
            rtb.Clear();
        }

        public string Content
        {
            get
            {
                return rtb.Text;
            }
            set
            {
                rtb.Text = value;
            }
        }

        public System.Drawing.Color ForeColor
        {
            get
            {
                return rtb.SelectionColor;
            }
            set
            {
                rtb.SelectionColor = value;
            }
        }

        public System.Drawing.Color BackColor
        {
            get
            {
                return rtb.SelectionBackColor;
            }
            set
            {
                rtb.SelectionBackColor = value;
            }
        }

        public System.Drawing.Font Font
        {
            get
            {
                return rtb.SelectionFont;
            }
            set
            {
                rtb.SelectionFont = value;
            }
        }

        #endregion
    }
}
