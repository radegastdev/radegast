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
    public partial class RadegastTab
    {
        public bool Floater = false;
        public bool CloseOnDetachedClose = false;

        private RadegastInstance instance;

        private string label;
        private string originalLabel;

        public RadegastTab(RadegastInstance instance, ToolStripButton button, Control control, string name, string label)
        {
            this.instance = instance;
            Button = button;
            Control = control;
            Name = name;
            this.label = label;
        }

        public void Close()
        {
            if (!AllowClose) return;

            if (Control != null)
            {
                if (Control.Parent is Form)
                {
                    Control.Parent.Dispose();
                }

                if (instance.TabConsole.toolStripContainer1.ContentPanel.Contains(Control))
                {
                    instance.TabConsole.toolStripContainer1.ContentPanel.Controls.Remove(Control);
                }
                Control.Dispose();
                Control = null;
            }

            if (Button != null)
            {
                if (instance.TabConsole.tstTabs.Items.Contains(Button))
                {
                    instance.TabConsole.tstTabs.Items.Remove(Button);
                }
                Button.Dispose();
                Button = null;
            }


            OnTabClosed(EventArgs.Empty);
        }

        public void Select()
        {
            if (Detached) return;

            if (Hidden)
            {
                Hidden = false;
            }

            Control.Visible = true;
            Control.BringToFront();

            if (!PartiallyHighlighted) Unhighlight();
            Button.Visible = true;
            Button.Checked = true;
            Selected = true;

            OnTabSelected(EventArgs.Empty);
        }

        public void Deselect()
        {
            if (Detached) return;

            if (Control != null) Control.Visible = false;
            if (Button != null) Button.Checked = false;

            Selected = false;

            OnTabDeselected(EventArgs.Empty);
        }

        public void Hide()
        {
            if (!AllowHide || Detached) return;

            if (Control != null) Control.Visible = false;
            if (Button != null) Button.Visible = false;

            Hidden = true;

            OnTabHidden(EventArgs.Empty);
        }

        public void Show()
        {
            if (Detached) return;

            if (Button != null) Button.Visible = true;
            Select();

            Hidden = false;

            OnTabShown(EventArgs.Empty);
        }

        public void PartialHighlight()
        {
            if (Detached)
            {
                //do nothing?!
            }
            else
            {
                Button.Image = null;
                Button.ForeColor = Color.Blue;
            }

            PartiallyHighlighted = true;
            OnTabPartiallyHighlighted(EventArgs.Empty);
        }

        public void Highlight()
        {
            if (instance.GlobalSettings["taskbar_highlight"])
            {
                if ((Control is ChatConsole && instance.GlobalSettings["highlight_on_chat"]) ||
                    (Control is IMTabWindow && instance.GlobalSettings["highlight_on_im"]) ||
                    (Control is GroupIMTabWindow && instance.GlobalSettings["highlight_on_group_chat"]) ||
                    (Control is ConferenceIMTabWindow && instance.GlobalSettings["highlight_on_group_chat"]))
                {
                    FormFlash.StartFlash(Control.FindForm());
                }
            }

            if (Selected) return;

            if (!Detached)
            {
                Button.Image = Properties.Resources.arrow_forward_16;
                Button.ForeColor = Color.Red;
            }

            Highlighted = true;
            OnTabHighlighted(EventArgs.Empty);
        }

        public void Unhighlight()
        {
            FormFlash.StopFlash(instance.MainForm);

            if (!Detached)
            {
                Button.Image = null;
                Button.ForeColor = Color.FromKnownColor(KnownColor.ControlText);
            }

            Highlighted = PartiallyHighlighted = false;
            OnTabUnhighlighted(EventArgs.Empty);
        }

        public void AttachTo(ToolStrip strip, Panel container)
        {
            if (!AllowDetach) return;
            if (!Detached) return;

            Button.Visible = true;
            foreach (Control c in container.Controls)
                c.Hide();
            container.Controls.Add(Control);

            Owner = null;
            Detached = false;
            OnTabAttached(EventArgs.Empty);
        }

        public void Detach(RadegastInstance instance)
        {
            if (!AllowDetach) return;
            if (Detached) return;
            Button.Visible = false;
            Owner = new frmDetachedTab(instance, this);
            Owner.Show();
            Owner.Focus();
            Detached = true;
            OnTabDetached(EventArgs.Empty);            
        }

        public void MergeWith(RadegastTab tab)
        {
            if (!AllowMerge) return;
            if (Merged) return;

            SplitContainer container = new SplitContainer();
            container.Dock = DockStyle.Fill;
            container.BorderStyle = BorderStyle.Fixed3D;
            container.SplitterDistance = container.Width / 2;
            container.Panel1.Controls.Add(Control);
            container.Panel2.Controls.Add(tab.Control);

            Control.Visible = true;
            tab.Control.Visible = true;

            Control = container;
            tab.Control = container;
            
            MergedTab = tab;
            tab.MergedTab = this;

            originalLabel = label;
            tab.originalLabel = tab.label;
            Label = label + "+" + tab.Label;
            
            Merged = tab.Merged = true;

            OnTabMerged(EventArgs.Empty);
        }

        public RadegastTab Split()
        {
            if (!AllowMerge) return null;
            if (!Merged) return null;

            RadegastTab returnTab = MergedTab;
            MergedTab = null;
            returnTab.MergedTab = null;

            SplitContainer container = (SplitContainer)Control;
            Control = container.Panel1.Controls[0];
            returnTab.Control = container.Panel2.Controls[0];
            Merged = returnTab.Merged = false;

            Label = originalLabel;
            OnTabSplit(EventArgs.Empty);

            return returnTab;
        }

        public ToolStripButton Button { get; set; }

        public Control Control { get; set; }

        public Button DefaultControlButton { get; set; }

        public string Name { get; }

        public string Label
        {
            get => label;
            set => label = Button.Text = value;
        }

        public RadegastTab MergedTab { get; private set; }

        public Form Owner { get; private set; }

        public bool AllowMerge { get; set; } = true;

        public bool AllowDetach { get; set; } = true;

        public bool AllowClose { get; set; } = true;

        public bool PartiallyHighlighted { get; private set; } = false;

        public bool Highlighted { get; private set; } = false;

        public bool Selected { get; private set; } = false;

        public bool Detached { get; private set; } = false;

        public bool Merged { get; private set; } = false;

        public bool AllowHide { get; set; } = true;

        public bool Hidden { get; private set; } = false;

        public bool Visible
        {
            get => !Hidden;

            set
            {
                if (value)
                {
                    Show();
                }
                else if (AllowHide)
                {
                    Hide();
                }
            }
        }
    }
}
