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
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Windows.Forms;

namespace Radegast
{
    public class RichTextBoxPrinter : ITextPrinter
    {
        private RRichTextBox rtb;
        private bool mono;
        private static readonly string urlRegexString = @"(https?://[^ \r\n]+)|(\[secondlife://[^ \]\r\n]* ?(?:[^\]\r\n]*)])|(secondlife://[^ \r\n]*)";
        Regex urlRegex;
        private SlUriParser uriParser;

        public RichTextBoxPrinter(RRichTextBox textBox)
        {
            rtb = textBox;

            // Are we running mono?
            mono = Type.GetType("Mono.Runtime") != null;
            if (mono)
            {
                // On Linux we cannot do manual links
                // so we keep using built in URL detection
                rtb.DetectUrls = true;
            }

            uriParser = new SlUriParser();
            urlRegex = new Regex(urlRegexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);

        }

        public void InsertLink(string text)
        {
            rtb.InsertLink(text);
        }

        public void InsertLink(string text, string hyperlink)
        {
            rtb.InsertLink(text, hyperlink);
        }

        private void FindURLs(string text)
        {
            StringBuilder sb = new StringBuilder();
            string[] lineParts = urlRegex.Split(text);
            int linePartIndex;

            // 'text' will be split into 1 + NumLinks*2 parts...
            // If 'text' has no links in it:
            //    lineParts[0] = text
            // If 'text' has one link in it:
            //    lineParts[0] = <Text before first link>
            //    lineParts[1] = <first link>
            //    lineParts[2] = <text after first link>
            // If 'text' has two links in it:
            //    lineParts[0] = <Text before first link>
            //    lineParts[1] = <first link>
            //    lineParts[2] = <text after first link>
            //    lineParts[3] = <second link>
            //    lineParts[4] = <text after second link>
            // ...
            for (linePartIndex = 0; linePartIndex < lineParts.Length - 1; linePartIndex += 2)
            {
                rtb.AppendText(lineParts[linePartIndex]);
                Color c = ForeColor;
                rtb.InsertLink(uriParser.GetLinkName(lineParts[linePartIndex + 1]), lineParts[linePartIndex+1]);
                ForeColor = c;
            }
            if (linePartIndex != lineParts.Length)
            {
                rtb.AppendText(lineParts[linePartIndex]);
            }
        }

        #region ITextPrinter Members

        public void PrintText(string text)
        {
            if (rtb.InvokeRequired)
            {
                rtb.Invoke(new MethodInvoker(() => rtb.AppendText(text)));
                return;
            }

            if (mono)
            {
                rtb.AppendText(text);
            }
            else
            {
                FindURLs(text);
            }
        }

        public void PrintTextLine(string text)
        {
            PrintText(text + Environment.NewLine);
        }

        public void PrintTextLine(string text, Color color)
        {
            if (rtb.InvokeRequired)
            {
                rtb.Invoke(new MethodInvoker(() => PrintTextLine(text, color)));
                return;
            }

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
