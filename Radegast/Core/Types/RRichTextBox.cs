// 
// Radegast Metaverse Client
// Copyright (c) 2009-2013, Radegast Development Team
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
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Tools;

namespace Radegast
{

    public class RRichTextBox : ExtendedRichTextBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private bool supressOnTextChanged = false;
        private bool syntaxHighLightEnabled = false;
        private bool monoRuntime = false;
        private string rtfHeader;

        //  Tool tip related private members
        private System.Threading.Timer ttTimer;
        private ToolTip ttKeyWords;

        #region Public properties
        [Browsable(true), Category("Behavior"), DefaultValue(false)]
        public bool SyntaxHighlightEnabled
        {
            get { return syntaxHighLightEnabled; }

            set
            {
                if (value != syntaxHighLightEnabled)
                {
                    syntaxHighLightEnabled = value;
                    BeginUpdate();
                    SaveState(true);
                    string oldText = Text;
                    Clear();
                    Text = oldText;

                    RestoreState(true);
                    EndUpdate();
                }
            }
        }
        #endregion

        public RRichTextBox()
            : base()
        {
            InitializeComponent();

            // Are we running mono?
            if (Type.GetType("Mono.Runtime") != null)
            {
                monoRuntime = true;
            }

            rtfHeader = this.Rtf.Substring(0, this.Rtf.IndexOf('{', 2)) + " ";

        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ttKeyWords = new ToolTip(this.components);
            this.ttTimer = new System.Threading.Timer(ttTimerElapsed, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components.Dispose();
                ttTimer.Dispose();
            }
            base.Dispose(disposing);
        }


        #region Update supression
        private int savedSelStart;
        private int savedSelLen;
        private Win32.POINT savedScrollPos;

        public void SaveState(bool saveScrollBars)
        {
            savedSelStart = SelectionStart;
            savedSelLen = SelectionLength;
            if (saveScrollBars)
                savedScrollPos = GetScrollPos();
        }

        public void RestoreState(bool saveScrollBars)
        {
            SelectionStart = savedSelStart;
            SelectionLength = savedSelLen;
            if (saveScrollBars)
                SetScrollPos(savedScrollPos);
        }

        #endregion

        #region Copy/Paste
        public new void Paste(DataFormats.Format format)
        {
            Paste();
        }

        public new void Copy()
        {
            if (SelectionLength > 0)
            {
                Clipboard.SetText(SelectedText);
            }
        }

        public new void Cut()
        {
            if (SelectionLength > 0)
            {
                Clipboard.SetText(SelectedText);
                SelectedText = string.Empty;
            }
        }

        public new void Paste()
        {
            string toPaste = Clipboard.GetText();
            if (syntaxHighLightEnabled && toPaste.Contains("\n") && !monoRuntime)
            {
                SelectedRtf = ReHighlight(toPaste);
            }
            else
            {
                SelectedText = toPaste;
            }
        }
        #endregion

        #region Syntax highligting
        private string rtfEscaped(string s)
        {
            return s.Replace(@"\", @"\\").Replace("{", @"\{").Replace("}", @"\}").Replace("\n", "\\par\n");
        }

        public Color CommentColor = Color.FromArgb(204, 76, 38);
        public Color StringColor = Color.FromArgb(0, 51, 0);

        private List<Color> usedColors;
        private List<Color> UsedColors
        {
            get
            {
                if (usedColors == null)
                {
                    usedColors = new List<Color>();
                    usedColors.Add(ForeColor);
                    usedColors.Add(CommentColor);
                    usedColors.Add(StringColor);
                    
                    foreach (LSLKeyWord w in KeyWords.Values)
                    {
                        if (!usedColors.Contains(w.Color))
                        {
                            usedColors.Add(w.Color);
                        }
                    }
                }
                return usedColors;
            }
        }

        private string colorTag(Color c, string s)
        {
            return "\\cf" + (UsedColors.IndexOf(c) +1) + " " + rtfEscaped(s) + "\\cf1 ";
        }

        public Dictionary<string, LSLKeyWord> KeyWords = LSLKeywordParser.KeyWords;

        private string ReHighlight(string text)
        {
            StringTokenizer tokenizer = new StringTokenizer(text);
            Token token;
            StringBuilder body = new StringBuilder();

            do
            {
                token = tokenizer.Next();

                switch (token.Kind)
                {
                    case TokenKind.Word:
                        if (KeyWords.ContainsKey(token.Value))
                        {
                            body.Append(colorTag(KeyWords[token.Value].Color, token.Value));
                        }
                        else
                        {
                            goto default;
                        }
                        break;

                    case TokenKind.QuotedString:
                        body.Append(colorTag(StringColor, token.Value));
                        break;

                    case TokenKind.Comment:
                        body.Append(colorTag(CommentColor, token.Value));
                        break;

                    case TokenKind.EOL:
                        body.Append("\\par\n\\cf1 ");
                        break;

                    default:
                        body.Append(rtfEscaped(token.Value));
                        break;
                }

            } while (token.Kind != TokenKind.EOF);

            StringBuilder colorTable = new StringBuilder();
            colorTable.Append(@"{\colortbl;");

            foreach (Color color in UsedColors)
            {
                colorTable.AppendFormat("\\red{0}\\green{1}\\blue{2};", color.R, color.G, color.B);
            }

            colorTable.Append("}");

            // Construct final rtf
            StringBuilder rtf = new StringBuilder();
            rtf.AppendLine(@"{\rtf1\ansi\deff0{\fonttbl{\f0\fnil\fcharset0 " + rtfEscaped(Font.Name) + ";}}");
            rtf.AppendLine(colorTable.ToString());
            rtf.Append(@"\pard\f0\fs" + (int)(Font.SizeInPoints * 2)+ " ");
            rtf.Append(body);
            rtf.AppendLine(@"}");

            return rtf.ToString();
        }

        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                if (syntaxHighLightEnabled && value != null)
                {
                    BeginUpdate();
                    base.Rtf = ReHighlight(value);
                    EndUpdate();
                }
                else
                {
                    base.Text = value;
                }
            }
        }

        protected override void OnTextChanged(EventArgs e)
        {
            if (supressOnTextChanged || Updating) return;

            if (!syntaxHighLightEnabled || monoRuntime || Lines.Length == 0)
            {
                base.OnTextChanged(e);
                return;
            }

            supressOnTextChanged = true;
            BeginUpdate();

            // Save selection
            int selectionStart = SelectionStart;
            int selectionLength = SelectionLength;

            // Rehighlught line
            int currentLineNr = GetLineFromCharIndex(selectionStart);
            string currentLine = Lines[currentLineNr];
            int firstCharIndex = GetFirstCharIndexOfCurrentLine();

            SelectionStart = firstCharIndex;
            SelectionLength = currentLine.Length;
            SelectedRtf = ReHighlight(currentLine);

            // Restore selection
            SelectionStart = selectionStart;
            SelectionLength = selectionLength;

            base.OnTextChanged(e);

            EndUpdate();
            supressOnTextChanged = false;
        }

        #endregion

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

        [Browsable(false)]
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
                if (currentCol < 0) currentCol = 0;
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

        public override void InsertImage(Image _image)
        {
            supressOnTextChanged = true;
            if (!monoRuntime)
                base.InsertImage(_image);
            supressOnTextChanged = false;
        }

        #region ToolTips
        private bool validWordChar(char c)
        {
            return
                (c >= 'a' && c <= 'z') ||
                (c >= 'A' && c <= 'Z') ||
                (c >= '0' && c <= '9') ||
                c == '_';
        }

        private void ttTimerElapsed(Object sender)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { ttTimerElapsed(sender); }));
                return;
            }

            char trackedChar = GetCharFromPosition(trackedMousePos);

            if (!validWordChar(trackedChar))
            {
                return;
            }

            string trackedString = Text;
            int trackedPos = GetCharIndexFromPosition(trackedMousePos);
            int starPos;
            int endPos;

			// Yes we want empty statement here
			#pragma warning disable 642
            for (starPos = trackedPos; starPos >= 0 && validWordChar(trackedString[starPos]); starPos--) ;
            for (endPos = trackedPos; endPos < trackedString.Length && validWordChar(trackedString[endPos]); endPos++) ;
            string word = trackedString.Substring(starPos + 1, endPos - starPos - 1);

            if (!KeyWords.ContainsKey(word) || KeyWords[word].ToolTip == string.Empty)
            {
                return;
            }

            ttKeyWords.Show(KeyWords[word].ToolTip, this, new Point(trackedMousePos.X, trackedMousePos.Y + 15), 120 * 1000);
        }

        private Point trackedMousePos = new Point(0, 0);

        protected override void OnMouseMove(MouseEventArgs e)
        {
            Point currentMousePos = new Point(e.X, e.Y);

            if (currentMousePos != trackedMousePos)
            {
                trackedMousePos = currentMousePos;
                ttTimer.Change(500, System.Threading.Timeout.Infinite);
                ttKeyWords.Hide(this);
            }
            base.OnMouseMove(e);
        }
        #endregion

        #region Links
        /// <summary>
        /// Insert a given text as a link into the RichTextBox at the current insert position.
        /// </summary>
        /// <param name="text">Text to be inserted</param>
        public void InsertLink(string text)
        {
            InsertLink(text, this.SelectionStart);
        }

        /// <summary>
        /// Insert a given text at a given position as a link. 
        /// </summary>
        /// <param name="text">Text to be inserted</param>
        /// <param name="position">Insert position</param>
        public void InsertLink(string text, int position)
        {
            if (position < 0 || position > this.Text.Length)
                throw new ArgumentOutOfRangeException("position");

            this.SelectionStart = position;
            this.SelectedText = text;
            this.Select(position, text.Length);
            this.SetSelectionLink(true);
            this.Select(position + text.Length, 0);
        }

        /// <summary>
        /// Insert a given text at at the current input position as a link.
        /// The link text is followed by a hash (#) and the given hyperlink text, both of
        /// them invisible.
        /// When clicked on, the whole link text and hyperlink string are given in the
        /// LinkClickedEventArgs.
        /// </summary>
        /// <param name="text">Text to be inserted</param>
        /// <param name="hyperlink">Invisible hyperlink string to be inserted</param>
        public void InsertLink(string text, string hyperlink)
        {
            InsertLink(text, hyperlink, this.SelectionStart);
        }

        //public const char LinkSeparator = (char)0x1970;
        public const char LinkSeparator = (char)0x8D;

        private string RtfUnicode(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] > (char)255)
                {
                    sb.Append(string.Format("\\u{0}?", (short)s[i]));
                }
                else
                {
                    sb.Append(s[i]);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Insert a given text at a given position as a link. The link text is followed by
        /// a hash (#) and the given hyperlink text, both of them invisible.
        /// When clicked on, the whole link text and hyperlink string are given in the
        /// LinkClickedEventArgs.
        /// </summary>
        /// <param name="text">Text to be inserted</param>
        /// <param name="hyperlink">Invisible hyperlink string to be inserted</param>
        /// <param name="position">Insert position</param>
        public void InsertLink(string text, string hyperlink, int position)
        {
            if (position < 0 || position > this.Text.Length)
                throw new ArgumentOutOfRangeException("position");

            this.SelectionStart = position;

            if (monoRuntime)
            {
                SelectedText = text;
                SelectionStart = position + text.Length;
            }
            else
            {
                this.SelectedRtf = rtfHeader + RtfUnicode(text) + @"\v " + LinkSeparator + hyperlink + @"\v0}";
                this.Select(position, text.Length + hyperlink.Length + 1);
                this.SetSelectionLink(true);
                this.Select(position + text.Length + hyperlink.Length + 1, 0);
            }
        }

        /// <summary>
        /// Set the current selection's link style
        /// </summary>
        /// <param name="link">true: set link style, false: clear link style</param>
        public void SetSelectionLink(bool link)
        {
            SetSelectionStyle(Win32.CFM_LINK, link ? Win32.CFE_LINK : 0);
        }
        /// <summary>
        /// Get the link style for the current selection
        /// </summary>
        /// <returns>0: link style not set, 1: link style set, -1: mixed</returns>
        public int GetSelectionLink()
        {
            return GetSelectionStyle(Win32.CFM_LINK, Win32.CFE_LINK);
        }

        private void SetSelectionStyle(UInt32 mask, UInt32 effect)
        {
            Win32.CHARFORMAT2_STRUCT cf = new Win32.CHARFORMAT2_STRUCT();
            cf.cbSize = (UInt32)Marshal.SizeOf(cf);
            cf.dwMask = mask;
            cf.dwEffects = effect;

            IntPtr wpar = new IntPtr(Win32.SCF_SELECTION);
            IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
            Marshal.StructureToPtr(cf, lpar, false);

            IntPtr res = Win32.SendMessage(Handle, Win32.EM_SETCHARFORMAT, wpar, lpar);

            Marshal.FreeCoTaskMem(lpar);
        }

        private int GetSelectionStyle(UInt32 mask, UInt32 effect)
        {
            Win32.CHARFORMAT2_STRUCT cf = new Win32.CHARFORMAT2_STRUCT();
            cf.cbSize = (UInt32)Marshal.SizeOf(cf);
            cf.szFaceName = new char[32];

            IntPtr wpar = new IntPtr(Win32.SCF_SELECTION);
            IntPtr lpar = Marshal.AllocCoTaskMem(Marshal.SizeOf(cf));
            Marshal.StructureToPtr(cf, lpar, false);

            IntPtr res = Win32.SendMessage(Handle, Win32.EM_GETCHARFORMAT, wpar, lpar);

            cf = (Win32.CHARFORMAT2_STRUCT)Marshal.PtrToStructure(lpar, typeof(Win32.CHARFORMAT2_STRUCT));

            int state;
            // dwMask holds the information which properties are consistent throughout the selection:
            if ((cf.dwMask & mask) == mask)
            {
                if ((cf.dwEffects & effect) == effect)
                    state = 1;
                else
                    state = 0;
            }
            else
            {
                state = -1;
            }

            Marshal.FreeCoTaskMem(lpar);
            return state;
        }
        #endregion

        #region Scrollbar positions functions
        /// <summary>
        /// Sends a win32 message to get the scrollbars' position.
        /// </summary>
        /// <returns>a POINT structre containing horizontal
        ///       and vertical scrollbar position.</returns>
        private unsafe Win32.POINT GetScrollPos()
        {
            Win32.POINT res = new Win32.POINT();
            IntPtr ptr = new IntPtr(&res);
            Win32.SendMessage(Handle, Win32.EM_GETSCROLLPOS, 0, ptr);
            return res;

        }

        /// <summary>
        /// Sends a win32 message to set scrollbars position.
        /// </summary>
        /// <param name="point">a POINT
        ///        conatining H/Vscrollbar scrollpos.</param>
        private unsafe void SetScrollPos(Win32.POINT point)
        {
            IntPtr ptr = new IntPtr(&point);
            Win32.SendMessage(Handle, Win32.EM_SETSCROLLPOS, 0, ptr);

        }

        #endregion
    }

    class LSLErrorHandler : ErrorHandler
    {
        public LSLErrorHandler()
            : base(false)
        {
        }
        public override void Error(CSToolsException e)
        {
            Report(e);
        }
    }
}