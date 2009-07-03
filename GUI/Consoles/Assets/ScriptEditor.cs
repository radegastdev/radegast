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
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{

    public partial class ScriptEditor : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private Dictionary<string, LSLKeyWord> keywords = LSLKeywordParser.KeyWords;
        private InventoryLSL script;
        private UUID requestID;
        private bool populating = true;
        private List<WordAndPosition> lineBuffer = new List<WordAndPosition>();
        private Color commentColor = Color.FromArgb(204, 76, 38);
        private string scriptName;
        private System.Threading.Timer ttTimer;
        
        public ScriptEditor(RadegastInstance instance)
            : this(instance, null)
        {
            scriptName = "New Script";
        }

        public ScriptEditor(RadegastInstance instance, InventoryLSL script)
        {
            InitializeComponent();
            Disposed += new EventHandler(SscriptEditor_Disposed);

            this.instance = instance;
            this.script = script;
            lblScripStatus.Text = string.Empty;
            ttTimer = new System.Threading.Timer(ttTimerElapsed, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);

            // Callbacks
            client.Assets.OnAssetReceived += new AssetManager.AssetReceivedCallback(Assets_OnAssetReceived);

            // Download script
            if (script != null)
            {
                scriptName = script.Name;
                requestID = client.Assets.RequestInventoryAsset(script, true);
                rtbCode.Text = lblScripStatus.Text = "Loading...";
            }
        }

        void SscriptEditor_Disposed(object sender, EventArgs e)
        {
            client.Assets.OnAssetReceived -= new AssetManager.AssetReceivedCallback(Assets_OnAssetReceived);
        }

        void Assets_OnAssetReceived(AssetDownload transfer, Asset asset)
        {
            if (transfer.ID != requestID) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { Assets_OnAssetReceived(transfer, asset); }));
                return;
            }

            if (!transfer.Success || asset.AssetType != AssetType.LSLText)
            {
                lblScripStatus.Text = rtbCode.Text = "Failed to download.";
                return;
            }

            asset.Decode();
            rtbCode.BeginUpdate();
            rtbCode.Text = ((AssetScriptText)asset).Source;
            MakeColorSyntaxForAllText();
            rtbCode.EndUpdate();
            lblScripStatus.Text = scriptName;
        }

        #region Syntax highlighting
        struct WordAndPosition
        {
            public string Word;
            public int Position;
            public int Length;
            public override string ToString()
            {
                string s = "Word = " + Word + ", Position = " + Position + ", Length = " + Length + "\n";
                return s;
            }
        };

        private int ParseLine(string s)
        {
            lineBuffer.Clear();
            Regex r = new Regex(@"\w+|[^A-Za-z0-9_ \f\t\v]", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            Match m;

            for (m = r.Match(s); m.Success; m = m.NextMatch())
            {
                WordAndPosition w = new WordAndPosition();
                w.Word = m.Value;
                w.Position = m.Index;
                w.Length = m.Length;
                lineBuffer.Add(w);
            }

            return lineBuffer.Count();
        }

        private Color Lookup(string s)
        {
            if (keywords.ContainsKey(s))
            {
                return keywords[s].Color;
            }
            else
            {
                return Color.Black;
            }
        }

        private bool TestComment(string s)
        {
            string testString = s.Trim();
            if ((testString.Length >= 2) &&
                 (testString[0] == '/') &&
                 (testString[1] == '/')
                )
                return true;

            return false;
        }

        private void MakeColorSyntaxForCurrentLine()
        {
            int CurrentSelectionStart = rtbCode.SelectionStart;
            int CurrentSelectionLength = rtbCode.SelectionLength;

            // find start of line
            int pos = CurrentSelectionStart;
            while ((pos > 0) && (rtbCode.Text[pos - 1] != '\n'))
                pos--;

            int pos2 = CurrentSelectionStart;
            while ((pos2 < rtbCode.Text.Length) &&
                    (rtbCode.Text[pos2] != '\n'))
                pos2++;

            string s = rtbCode.Text.Substring(pos, pos2 - pos);
            if (TestComment(s) == true)
            {
                rtbCode.Select(pos, pos2 - pos);
                rtbCode.SelectionColor = commentColor;
            }
            else
            {
                string previousWord = "";
                int count = ParseLine(s);
                for (int i = 0; i < count; i++)
                {
                    WordAndPosition wp = lineBuffer[i];

                    // check for comment
                    if (wp.Word == "/" && previousWord == "/")
                    {
                        // color until end of line
                        int posCommentStart = wp.Position - 1;
                        int posCommentEnd = pos2;
                        while (wp.Word != "\n" && i < count)
                        {
                            wp = lineBuffer[i];
                            i++;
                        }

                        i--;

                        posCommentEnd = pos2;
                        rtbCode.Select(posCommentStart + pos, posCommentEnd - (posCommentStart + pos));
                        rtbCode.SelectionColor = this.commentColor;

                    }
                    else
                    {

                        Color c = Lookup(wp.Word);
                        rtbCode.Select(wp.Position + pos, wp.Length);
                        rtbCode.SelectionColor = c;
                    }

                    previousWord = wp.Word;

                }
            }

            if (CurrentSelectionStart >= 0)
                rtbCode.Select(CurrentSelectionStart, CurrentSelectionLength);
        }

        private void MakeColorSyntaxForAllText()
        {
            populating = true;

            string s = rtbCode.Text;
            int CurrentSelectionStart = rtbCode.SelectionStart;
            int CurrentSelectionLength = rtbCode.SelectionLength;

            int count = ParseLine(s);
            string previousWord = "";
            for (int i = 0; i < count; i++)
            {
                WordAndPosition wp = lineBuffer[i];

                // check for comment
                if (wp.Word == "/" && previousWord == "/")
                {
                    // color until end of line
                    int posCommentStart = wp.Position - 1;
                    int posCommentEnd = i;
                    while (wp.Word != "\n" && i < count)
                    {
                        wp = lineBuffer[i];
                        i++;
                    }

                    i--;

                    posCommentEnd = wp.Position;
                    rtbCode.Select(posCommentStart, posCommentEnd - posCommentStart);
                    rtbCode.SelectionColor = this.commentColor;

                }
                else
                {

                    Color c = Lookup(wp.Word);
                    rtbCode.Select(wp.Position, wp.Length);
                    rtbCode.SelectionColor = c;
                }

                previousWord = wp.Word;

                //				Console.WriteLine(wp.ToString());
            }

            if (CurrentSelectionStart >= 0)
                rtbCode.Select(CurrentSelectionStart, CurrentSelectionLength);

            populating = false;

        }

        private void rtbCode_TextChanged(object sender, EventArgs e)
        {
            if (populating)
            {
                return;
            }
            rtbCode.BeginUpdate();
            MakeColorSyntaxForCurrentLine();
            rtbCode.EndUpdate();
        }
        #endregion

        #region Detach/Attach
        private Control originalParent;
        private frmDetachedControl detachedForm;
        private bool detached = false;

        public bool Detached
        {
            get
            {
                return detached;
            }

            set
            {
                if (detached == value) return; // no change

                detached = value;

                if (detached)
                {
                    Detach();
                }
                else
                {
                    Retach();
                }
            }
        }

        public void ShowDetached()
        {
            Detached = true;
        }

        private void Detach()
        {
            detached = true;

            if (detachedForm != null)
            {
                detachedForm.Dispose();
            }

            detachedForm = new frmDetachedControl();
            originalParent = Parent;
            Parent = detachedForm;
            detachedForm.ClientSize = new Size(873, 580);
            detachedForm.Text = scriptName + " - " + Properties.Resources.ProgramName + " script editor"; 
            detachedForm.Show();
            detachedForm.FormClosing += new FormClosingEventHandler(detachedForm_FormClosing);

            if (originalParent != null)
            {
                originalParent.ControlAdded += new ControlEventHandler(originalParent_ControlAdded);
            }
            else
            {
                tbtnAttach.Visible = false;
            }

            tbtnAttach.Text = "Retach";

            if (originalParent == null)
            {
                tSeparator1.Visible = true;
                tbtnExit.Visible = true;
            }
            else
            {
                tSeparator1.Visible = false;
                tbtnExit.Visible = false;
            }
        }

        void originalParent_ControlAdded(object sender, ControlEventArgs e)
        {
            if (detachedForm != null)
            {
                detachedForm.FormClosing -= new FormClosingEventHandler(detachedForm_FormClosing);
            }
            originalParent.ControlAdded -= new ControlEventHandler(originalParent_ControlAdded);
            tbtnAttach.Visible = false;
        }

        void detachedForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Retach();
        }

        private void Retach()
        {
            detached = false;

            if (originalParent != null)
            {
                originalParent.ControlAdded -= new ControlEventHandler(originalParent_ControlAdded);
                Parent = originalParent;
            }

            if (detachedForm != null)
            {
                detachedForm.Dispose();
                detachedForm = null;
            }

            tbtnAttach.Text = "Detach";

            if (originalParent == null)
            {
                Dispose();
            }
        }

        private void tbtnAttach_Click(object sender, EventArgs e)
        {
            if (detached)
            {
                Retach();
            }
            else
            {
                Detach();
            }
        }
        #endregion

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

            char trackedChar = rtbCode.GetCharFromPosition(trackedMousePos);
            
            if (!validWordChar(trackedChar))
            {
                return;
            }

            string trackedString = rtbCode.Text;
            int trackedPos = rtbCode.GetCharIndexFromPosition(trackedMousePos);
            int starPos;
            int endPos;

            for (starPos = trackedPos; starPos >= 0 && validWordChar(trackedString[starPos]); starPos--) ;
            for (endPos = trackedPos; endPos < trackedString.Length && validWordChar(trackedString[endPos]); endPos++) ;
            string word = trackedString.Substring(starPos + 1, endPos - starPos - 1);

            if (!keywords.ContainsKey(word) || keywords[word].ToolTip == string.Empty)
            {
                return;
            }

            ttKeyWords.Show(keywords[word].ToolTip, rtbCode, new Point(trackedMousePos.X, trackedMousePos.Y + 15), 120 * 1000);
        }

        private Point trackedMousePos = new Point(0, 0);

        private void rtbCode_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentMousePos = new Point(e.X, e.Y);

            if (currentMousePos != trackedMousePos)
            {
                trackedMousePos = currentMousePos;
                ttTimer.Change(500, System.Threading.Timeout.Infinite);
                ttKeyWords.Hide(rtbCode);
            }
        }
        #endregion

        private void tbtbSaveToDisk_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save script";
            dlg.Filter = "LSL script file (*.lsl)|*.lsl";
            dlg.FileName = RadegastMisc.SafeFileName(scriptName);
            DialogResult res = dlg.ShowDialog();

            if (res == DialogResult.OK)
            {
                File.WriteAllText(dlg.FileName, rtbCode.Text);
            }

        }

        private void tbtbLoadFromDisk_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open script";
            dlg.Filter = "LSL script file (*.lsl)|*.lsl";
            dlg.Multiselect = false;
            DialogResult res = dlg.ShowDialog();

            if (res == DialogResult.OK)
            {
                scriptName = dlg.FileName;
                rtbCode.Text = File.ReadAllText(dlg.FileName);
                rtbCode.BeginUpdate();
                MakeColorSyntaxForAllText();
                rtbCode.EndUpdate();
            }
        }

        private void tbtnExit_Click(object sender, EventArgs e)
        {
            FindForm().Close();
        }

        private void rtbCode_SelectionChanged(object sender, EventArgs e)
        {
            RRichTextBox.CursorLocation c = rtbCode.CursorPosition;
            lblLine.Text = string.Format("Ln {0}", c.Line + 1);
            lblCol.Text = string.Format("Col {0}", c.Column + 1);
        }
    }
}
