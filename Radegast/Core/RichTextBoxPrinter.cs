/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;
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
            get => rtb.Text;
            set => rtb.Text = value;
        }

        public Color ForeColor
        {
            get => rtb.SelectionColor;
            set => rtb.SelectionColor = value;
        }

        public Color BackColor
        {
            get => rtb.SelectionBackColor;
            set => rtb.SelectionBackColor = value;
        }

        public Font Font
        {
            get => rtb.SelectionFont;
            set => rtb.SelectionFont = value;
        }

        #endregion
    }
}
