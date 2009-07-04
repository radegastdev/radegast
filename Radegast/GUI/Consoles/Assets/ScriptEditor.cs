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
        private InventoryLSL script;
        private UUID requestID;
        private bool populating = true;
        private string scriptName;
        private string fileName;
        
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
            Dock = DockStyle.Fill;
            this.TabStop = false;
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
            rtbCode.Text = ((AssetScriptText)asset).Source;
            SetTitle();
        }

        private void SetTitle()
        {
            if (detached)
            {
                FindForm().Text = scriptName + " - " + Properties.Resources.ProgramName + " script editor";
            }
        }

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
            SetTitle();
            detachedForm.ActiveControl = this;
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

 
        #region File I/O
        private void tbtbSaveToDisk_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                tbtbSaveToDisk_Click(sender, e);
                return;
            }
            File.WriteAllText(fileName, rtbCode.Text);
        }

        private void tbtbSaveToDisk_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save script";
            dlg.Filter = "LSL script file (*.lsl)|*.lsl|Plain text file (*.txt)|*.txt";
            dlg.FileName = RadegastMisc.SafeFileName(scriptName);
            DialogResult res = dlg.ShowDialog();

            if (res == DialogResult.OK)
            {
                fileName = dlg.FileName;
                scriptName = Path.GetFileName(fileName);
                SetTitle();
                File.WriteAllText(fileName, rtbCode.Text);
            }

        }

        private void tbtbLoadFromDisk_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open script";
            dlg.Filter = "LSL script files (*.lsl)|*.lsl|Plain text files (*.txt)|*.txt|All files (*.*)|*.*";
            dlg.Multiselect = false;
            DialogResult res = dlg.ShowDialog();

            if (res == DialogResult.OK)
            {
                fileName = dlg.FileName;
                scriptName = Path.GetFileName(fileName);
                SetTitle();
                rtbCode.ClearUndoRedo();
                rtbCode.Text = File.ReadAllText(fileName);
            }
        }
        #endregion

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

        private bool spaceOrTab(char c)
        {
            return c == ' ' || c == '\t';
        }

        private int lastPos(string s, char c)
        {
            s = s.TrimEnd();
            
            if (s == string.Empty)
                return -1;

            if (s[s.Length - 1] == c)
            {
                return s.LastIndexOf(c);
            }
            return -1;
        }

        private void rtbCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
            {
                findToolStripMenuItem_Click(null, null);
            }
            if (e.KeyCode == Keys.Tab)
            {
                rtbCode.SelectedText = "    ";
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                if (rtbCode.Lines.Length > 0)
                {
                    RRichTextBox.CursorLocation cl = rtbCode.CursorPosition;
                    string prevLine = rtbCode.Lines[cl.Line];
                    string addIndent = "\n";
                    int pos;

                    for (int spaces = 0;
                        spaces < prevLine.Length && spaceOrTab(prevLine[spaces]);
                        addIndent += prevLine[spaces++]) ;

                    if ((pos = lastPos(prevLine.Substring(0, cl.Column), '{')) != -1)
                    {
                        addIndent += "    ";
                    }

                    rtbCode.SelectedText = addIndent;
                    int eat = 0;
                    for (eat = 0; (eat + rtbCode.SelectionStart) < rtbCode.Text.Length && rtbCode.Text[rtbCode.SelectionStart + eat] == ' '; eat++) ;
                    rtbCode.SelectionLength = eat;
                    rtbCode.SelectedText = "";
                    e.Handled = true;
                }
            }
        }

        private void rtbCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '}' && rtbCode.Lines.Length > 0)
            {
                string li = rtbCode.Lines[rtbCode.GetLineFromCharIndex(rtbCode.SelectionStart)];

                if (li.Trim() == "")
                {
                    rtbCode.BeginUpdate();
                    int toDelete = li.Length;

                    if (toDelete > 4)
                    {
                        toDelete = 4;
                    }

                    rtbCode.SelectionStart -= toDelete;
                    rtbCode.SelectionLength = toDelete;
                    rtbCode.SelectedText = "}";
                    rtbCode.EndUpdate();
                    e.Handled = true;
                }
            }
        }

        #region Edit menu handlers
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbCode.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbCode.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbCode.SelectionLength > 0)
            {
                Clipboard.SetText(rtbCode.SelectedText);
                rtbCode.SelectedText = string.Empty;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtbCode.SelectionLength > 0)
            {
                Clipboard.SetText(rtbCode.SelectedText);
            }
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string clip = Clipboard.GetText();

            if (!string.IsNullOrEmpty(clip))
            {
                rtbCode.SelectedText = clip;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbCode.SelectedText = string.Empty;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtbCode.SelectionStart = 0;
            rtbCode.SelectionLength = rtbCode.Text.Length;
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tsFindReplace.Show();
            tfindFindText.Focus();
        }
        #endregion

        private void tfindClose_Click(object sender, EventArgs e)
        {
            tsFindReplace.Hide();
        }

        private class FindHistoryItem
        {
            public FindHistoryItem(string term, int ss, int sl)
            {
                Term = term;
                SelStart = ss;
                SelLength = sl;
            }

            public string Term;
            public int SelStart;
            public int SelLength;

            public override string ToString()
            {
                return Term;
            }
        }

        Dictionary<string, FindHistoryItem> FindHistory = new Dictionary<string, FindHistoryItem>();
        FindHistoryItem startPos;

        private void tfindFindText_TextChanged(object sender, EventArgs e)
        {
            string st = tfindFindText.Text;

            if (startPos == null)
            {
                startPos = new FindHistoryItem(string.Empty, rtbCode.SelectionStart, rtbCode.SelectionLength);
                FindHistory[startPos.Term] = startPos;
            }

            StringComparison type;

            if (tfindMatchCase.Checked)
            {
                type = StringComparison.Ordinal;
            }
            else
            {
                type = StringComparison.OrdinalIgnoreCase;
            }

            if (FindHistory.ContainsKey(st))
            {
                tfindFindText.BackColor = Color.FromKnownColor(KnownColor.Window);
                tfindDoFind.Enabled = true;
                FindHistoryItem h = FindHistory[st];
                rtbCode.BeginUpdate();
                rtbCode.Select(h.SelStart, h.SelLength);
                rtbCode.ScrollToCaret();
                rtbCode.EndUpdate();
            }

            if (st == string.Empty)
            {
                FindHistory.Clear();
                tfindFindText.BackColor = Color.FromKnownColor(KnownColor.Window);
                tfindDoFind.Enabled = true;
                startPos = null;
                return;
            }

            int pos = rtbCode.Text.IndexOf(st, rtbCode.SelectionStart, type);

            if (pos != -1)
            {
                tfindFindText.BackColor = Color.FromKnownColor(KnownColor.Window);
                tfindDoFind.Enabled = true;
                FindHistory[st] = new FindHistoryItem(st, pos, st.Length);
                rtbCode.BeginUpdate();
                rtbCode.Select(pos, st.Length);
                rtbCode.ScrollToCaret();
                rtbCode.EndUpdate();
            }
            else
            {
                tfindFindText.BackColor = Color.FromArgb(200, 0, 0);
                tfindDoFind.Enabled = false;
            }
        }

        private void tfindFindText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void tfindFindText_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    tfindDoFind_Click(null, null);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }

        private void tfindDoFind_Click(object sender, EventArgs e)
        {
            FindHistory.Clear();
            startPos = null;
            int len = rtbCode.SelectionLength;
            rtbCode.SelectionLength = 0;
            rtbCode.SelectionStart += len;
            tfindFindText_TextChanged(sender, e);
        }

        private void tfindFindText_Leave(object sender, EventArgs e)
        {
            startPos = null;
            FindHistory.Clear();
        }

        private void tfindFindNextReplace_Click(object sender, EventArgs e)
        {
            tfindDoFind_Click(null, null);
        }

        private void tfindReplace_Click(object sender, EventArgs e)
        {
            if (tfindFindText.Text.Length > 0 && rtbCode.SelectionLength > 0)
            {
                rtbCode.SelectedText = tfindReplaceText.Text;
                tfindDoFind_Click(null, null);
            }
        }

        private static string ReplaceEx(string original, string pattern, string replacement)
        {
            int count, position0, position1;
            count = position0 = position1 = 0;
            string upperString = original.ToUpper();
            string upperPattern = pattern.ToUpper();
            int inc = (original.Length / pattern.Length) * (replacement.Length - pattern.Length);
            char[] chars = new char[original.Length + Math.Max(0, inc)];
            while ((position1 = upperString.IndexOf(upperPattern, position0)) != -1)
            {
                for (int i = position0; i < position1; ++i)
                    chars[count++] = original[i];
                for (int i = 0; i < replacement.Length; ++i)
                    chars[count++] = replacement[i];
                position0 = position1 + pattern.Length;
            }
            if (position0 == 0) return original;
            for (int i = position0; i < original.Length; ++i)
                chars[count++] = original[i];
            return new string(chars, 0, count);
        }

        private void tfindReplaceAll_Click(object sender, EventArgs e)
        {
            if (tfindFindText.Text.Length > 0)
            {
                if (tfindMatchCase.Checked)
                {
                    rtbCode.Text.Replace(tfindFindText.Text, tfindReplaceText.Text);
                }
                else
                {
                    rtbCode.Text = ReplaceEx(rtbCode.Text, tfindFindText.Text, tfindReplaceText.Text);
                }
            }
        }

    }
}
