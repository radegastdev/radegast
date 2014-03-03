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
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Radegast
{
    [ToolStripItemDesignerAvailability(
        ToolStripItemDesignerAvailability.ToolStrip)]
    public partial class ToolStripCheckBox
        : ToolStripControlHost
    {
        public CheckBox CheckBoxControl
        {
            get
            {
                return Control as CheckBox;
            }
        }

        /// <summary>
        /// Is check box ticked
        /// </summary>
        [Category("Appearance")]
        public bool Checked
        {
            get
            {
                return CheckBoxControl.Checked;
            }
            set
            {
                CheckBoxControl.Checked = value;
            }
        }

        /// <summary>
        /// Checked state
        /// </summary>
        [Category("Appearance")]
        public CheckState CheckState
        {
            get
            {
                return CheckBoxControl.CheckState;
            }
            set
            {
                CheckBoxControl.CheckState = value;
            }
        }

        /// <summary>
        /// Label text
        /// </summary>
        [Category("Appearance")]
        public override string Text
        {
            get
            {
                return CheckBoxControl.Text;
            }
            set
            {
                CheckBoxControl.Text = value;
            }
        }

        /// <summary>
        /// Occurs when check property is changed
        /// </summary>
        [Category("Misc")]
        public event EventHandler CheckedChanged;

        /// <summary>
        /// Occurs when check state of the control changes
        /// </summary>
        [Category("Misc")]
        public event EventHandler CheckStateChanged;

        public ToolStripCheckBox()
            : base(new CheckBox())
        {
            CheckBoxControl.MouseHover += new EventHandler(chk_MouseHover);
        }

        void chk_MouseHover(object sender, EventArgs e)
        {
            this.OnMouseHover(e);
        }
 
        protected override void OnSubscribeControlEvents(Control c)
        {
            base.OnSubscribeControlEvents(c);
            ((CheckBox)c).CheckedChanged += new EventHandler(ToolStripCheckBox_CheckedChanged);
            ((CheckBox)c).CheckStateChanged += new EventHandler(ToolStripCheckBox_CheckStateChanged);
        }

        protected override void OnUnsubscribeControlEvents(Control c)
        {
            base.OnUnsubscribeControlEvents(c);
            ((CheckBox)c).CheckedChanged -= new EventHandler(ToolStripCheckBox_CheckedChanged);
            ((CheckBox)c).CheckStateChanged -= new EventHandler(ToolStripCheckBox_CheckStateChanged);
        }

        void ToolStripCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (CheckedChanged != null)
            {
                CheckedChanged(this, e);
            }
        }

        void ToolStripCheckBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (CheckStateChanged != null)
            {
                CheckStateChanged(this, e);
            }
        }
    }
}