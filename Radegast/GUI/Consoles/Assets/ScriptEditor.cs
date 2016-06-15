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
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;
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
        private string scriptName;
        private string fileName;
        private Primitive prim;

        public ScriptEditor(RadegastInstance instance)
            : this(instance, null, null)
        {
            scriptName = "New Script";
        }

        public ScriptEditor(RadegastInstance instance, InventoryLSL script)
            : this(instance, script, null)
        {
        }

        public ScriptEditor(RadegastInstance instance, InventoryLSL script, Primitive prim)
        {
            InitializeComponent();
            Disposed += new EventHandler(SscriptEditor_Disposed);

            this.instance = instance;
            this.script = script;
            this.prim = prim;

            rtb.SyntaxHighlightEnabled = instance.GlobalSettings["script_syntax_highlight"].AsBoolean();
            lblScripStatus.Text = string.Empty;
            lblScripStatus.TextChanged += (object sender, EventArgs e) =>
                instance.TabConsole.DisplayNotificationInChat(lblScripStatus.Text, ChatBufferTextStyle.Invisible);
            Dock = DockStyle.Fill;
            this.TabStop = false;

            if (prim == null)
            {
                cbRunning.Visible = false;
            }

            // Download script
            if (script != null)
            {
                scriptName = script.Name;
                if (prim != null)
                {
                    client.Assets.RequestInventoryAsset(script.AssetUUID, script.UUID, prim.ID, prim.OwnerID, script.AssetType, true, Assets_OnAssetReceived);
                    client.Inventory.RequestGetScriptRunning(prim.ID, script.UUID);
                    client.Inventory.ScriptRunningReply += OnScriptRunningReplyReceived;
                }
                else
                {
                    client.Assets.RequestInventoryAsset(script, true, Assets_OnAssetReceived);
                }
                rtb.Text = lblScripStatus.Text = "Loading script...";
            }
            else
            {
                rtb.Text = " "; //bugs in control grrrr
                rtb.SelectionStart = 0;
            }

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void SscriptEditor_Disposed(object sender, EventArgs e)
        {
            client.Inventory.ScriptRunningReply -= OnScriptRunningReplyReceived;
        }

        void OnScriptRunningReplyReceived(object sender, ScriptRunningReplyEventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => OnScriptRunningReplyReceived(sender, e)));
                return;
            }

            cbRunning.Checked = e.IsRunning;
            cbMono.Checked = e.IsMono;
        }

        void Assets_OnAssetReceived(AssetDownload transfer, Asset asset)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                    BeginInvoke(new MethodInvoker(() => Assets_OnAssetReceived(transfer, asset)));
                return;
            }

            if (!transfer.Success || asset.AssetType != AssetType.LSLText)
            {
                lblScripStatus.Text = rtb.Text = "Failed to download.";
                return;
            }
            else
                lblScripStatus.Text = rtb.Text = "OK";

            asset.Decode();
            rtb.Text = ((AssetScriptText)asset).Source;
            lineNubersForRtb.Invalidate();
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
        private Form detachedForm;
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

            detachedForm = new Form();
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
            File.WriteAllText(fileName, rtb.Text);
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
                File.WriteAllText(fileName, rtb.Text);
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
                rtb.Text = File.ReadAllText(fileName);
                lineNubersForRtb.Invalidate();
            }
        }
        #endregion

        private void tbtnExit_Click(object sender, EventArgs e)
        {
            FindForm().Close();
        }

        private void rtb_SelectionChanged(object sender, EventArgs e)
        {
            RRichTextBox.CursorLocation c = rtb.CursorPosition;
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

        //private RRichTextBox.CursorLocation prevCursor;

        private void rtb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control && !e.Shift)
            {
                tbtbSave_Click(this, EventArgs.Empty);
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            if (e.KeyCode == Keys.L && e.Control && !e.Shift)
            {
                ReadCursorPosition();
                e.Handled = e.SuppressKeyPress = true;
                return;
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.F)
            {
                findToolStripMenuItem_Click(null, null);
            }
            if (e.KeyCode == Keys.Tab)
            {
                rtb.SelectedText = "    ";
                e.Handled = true;
            }
            /*
            else if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;

                // RRichTextBox.CursorLocation cl = prevCursor;
                RRichTextBox.CursorLocation cl = rtb.CursorPosition;
                string prevLine = rtb.Lines[cl.Line];
                string addIndent = "\n";
                int pos;

                for (int spaces = 0;
                    spaces < prevLine.Length && spaceOrTab(prevLine[spaces]);
                    addIndent += prevLine[spaces++]) ;

                System.Console.WriteLine("In identer");
                if ((pos = lastPos(prevLine.Substring(0, cl.Column), '{')) != -1)
                {
                    addIndent += "    ";
                }

                rtb.SelectedText = addIndent;
                int eat = 0;
                for (eat = 0; (eat + rtb.SelectionStart) < rtb.Text.Length && rtb.Text[rtb.SelectionStart + eat] == ' '; eat++) ;
                rtb.SelectionLength = eat;
                rtb.SelectedText = "";
            }
            */

        }


        private void rtb_KeyPress(object sender, KeyPressEventArgs e)
        {/*
            if (rtb.Lines.Length > 0)
            {
                if (e.KeyChar == '}')
                {
                    string li = rtb.Lines[rtb.GetLineFromCharIndex(rtb.SelectionStart)];

                    if (li.Trim() == "")
                    {
                        rtb.BeginUpdate();
                        int toDelete = li.Length;

                        if (toDelete > 4)
                        {
                            toDelete = 4;
                        }

                        rtb.SelectionStart -= toDelete;
                        rtb.SelectionLength = toDelete;
                        rtb.SelectedText = "";
                        rtb.EndUpdate();
                    }
                }
            }
          */
        }

        #region Edit menu handlers
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtb.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtb.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtb.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtb.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtb.Paste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtb.SelectedText = string.Empty;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            rtb.SelectionStart = 0;
            rtb.SelectionLength = rtb.Text.Length;
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
        int searchFrom = 0;

        private void tfindFindText_TextChanged(object sender, EventArgs e)
        {
            string st = tfindFindText.Text;

            if (startPos == null)
            {
                startPos = new FindHistoryItem(string.Empty, rtb.SelectionStart, rtb.SelectionLength);
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
                FindHistoryItem h = FindHistory[st];
                rtb.BeginUpdate();
                rtb.Select(h.SelStart, h.SelLength);
                rtb.ScrollToCaret();
                rtb.EndUpdate();
                searchFrom = h.SelStart;

                if (st == string.Empty)
                {
                    FindHistory.Clear();
                    tfindFindText.BackColor = Color.FromKnownColor(KnownColor.Window);
                }

                return;
            }

            if (st == string.Empty)
            {
                FindHistory.Clear();
                tfindFindText.BackColor = Color.FromKnownColor(KnownColor.Window);
                return;
            }

            int pos = rtb.Text.IndexOf(st, searchFrom, type);

            if (pos != -1)
            {
                tfindFindText.BackColor = Color.FromKnownColor(KnownColor.Window);
                FindHistory[st] = new FindHistoryItem(st, pos, st.Length);
                rtb.BeginUpdate();
                rtb.Select(pos, st.Length);
                rtb.ScrollToCaret();
                rtb.EndUpdate();
                searchFrom = pos;
            }
            else
            {
                searchFrom = 0;
                tfindFindText.BackColor = Color.FromArgb(200, 0, 0);
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
            searchFrom += rtb.SelectionLength;
            tfindFindText_TextChanged(sender, e);
        }

        private void tfindFindText_Leave(object sender, EventArgs e)
        {
            startPos = null;
            FindHistory.Clear();
        }

        private void tfindFindText_Enter(object sender, EventArgs e)
        {
            searchFrom = rtb.SelectionStart;
        }


        private void tfindFindNextReplace_Click(object sender, EventArgs e)
        {
            tfindDoFind_Click(null, null);
        }

        private void tfindReplace_Click(object sender, EventArgs e)
        {
            if (tfindFindText.Text.Length > 0 && rtb.SelectionLength > 0)
            {
                rtb.SelectedText = tfindReplaceText.Text;
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
                    rtb.Text.Replace(tfindFindText.Text, tfindReplaceText.Text);
                }
                else
                {
                    rtb.Text = ReplaceEx(rtb.Text, tfindFindText.Text, tfindReplaceText.Text);
                }
            }
        }

        private void syntaxHiglightingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rtb.SyntaxHighlightEnabled == true)
            {
                rtb.SyntaxHighlightEnabled = false;
            }
            else
            {
                rtb.SyntaxHighlightEnabled = true;
            }
            syntaxHiglightingToolStripMenuItem.Checked = rtb.SyntaxHighlightEnabled;
        }

        private void ReadCursorPosition()
        {
            instance.TabConsole.DisplayNotificationInChat(
                string.Format("Cursor at line {0}, column {1}", rtb.CursorPosition.Line + 1, rtb.CursorPosition.Column + 1),
                ChatBufferTextStyle.Invisible);
        }

        private void tbtbSave_Click(object sender, EventArgs e)
        {
            InventoryManager.ScriptUpdatedCallback handler = (bool uploadSuccess, string uploadStatus, bool compileSuccess, List<string> compileMessages, UUID itemID, UUID assetID) =>
            {
                if (!IsHandleCreated && instance.MonoRuntime) return;

                BeginInvoke(new MethodInvoker(() =>
                {
                    if (uploadSuccess && compileSuccess)
                    {
                        lblScripStatus.Text = "Saved OK";
                    }
                    else
                    {
                        if (!compileSuccess)
                        {
                            lblScripStatus.Text = "Compilation failed";
                            if (compileMessages != null)
                            {
                                txtStatus.Show();
                                txtStatus.Text = string.Empty;
                                for (int i = 0; i < compileMessages.Count; i++)
                                {
                                    Match m = Regex.Match(compileMessages[i], @"\((?<line>\d+),\s*(?<column>\d+)\s*\)\s*:\s*(?<kind>\w+)\s*:\s*(?<msg>.*)", RegexOptions.IgnoreCase);

                                    if (m.Success)
                                    {
                                        int line = 1 + int.Parse(m.Groups["line"].Value, Utils.EnUsCulture);
                                        int column = 1 + int.Parse(m.Groups["column"].Value, Utils.EnUsCulture);
                                        string kind = m.Groups["kind"].Value;
                                        string msg = m.Groups["msg"].Value;
                                        instance.TabConsole.DisplayNotificationInChat(
                                            string.Format("{0} on line {1}, column {2}: {3}", kind, line, column, msg),
                                            ChatBufferTextStyle.Invisible);
                                        txtStatus.Text += string.Format("{0} (Ln {1}, Col {2}): {3}", kind, line, column, msg);

                                        if (i == 0)
                                        {
                                            rtb.CursorPosition = new RRichTextBox.CursorLocation(line - 1, column - 1);
                                            ReadCursorPosition();
                                            rtb.Focus();
                                        }
                                    }
                                    else
                                    {
                                        txtStatus.Text += compileMessages[i] + Environment.NewLine;
                                        instance.TabConsole.DisplayNotificationInChat(compileMessages[i]);
                                    }
                                }
                            }
                        }
                        else
                        {
                            lblScripStatus.Text = rtb.Text = "Failed to download.";
                        }

                    }
                }
                ));
            };


            lblScripStatus.Text = "Saving...";
            txtStatus.Hide();
            txtStatus.Text = string.Empty;

            AssetScriptText n = new AssetScriptText();
            n.Source = rtb.Text;
            n.Encode();

            if (prim != null)
            {
                client.Inventory.RequestUpdateScriptTask(n.AssetData, script.UUID, prim.ID, cbMono.Checked, cbRunning.Checked, handler);
            }
            else
            {
                client.Inventory.RequestUpdateScriptAgentInventory(n.AssetData, script.UUID, cbMono.Checked, handler);
            }
        }

    }
}
