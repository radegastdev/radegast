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
using System.Drawing;
using System.Windows.Forms;

namespace Radegast
{
    public class DettachableControl : UserControl
    {
        private bool detached = false;
        private Form detachedForm;

        public delegate void DetachedCallback();
        public event DetachedCallback OnDetached;

        public delegate void RetachedCallback();
        public event RetachedCallback OnRetached;

        protected Control OriginalParent;
        protected Size AttachedSize;

        /// <summary>
        /// If in detached state and detached form is closing and we have no parent
        /// do we dispose ourselves</summary>
        public bool DisposeOnDetachedClose = true;

        public DettachableControl()
            : base()
        {
            SizeChanged += new System.EventHandler(DettachableControl_SizeChanged);
        }

        void DettachableControl_SizeChanged(object sender, System.EventArgs e)
        {
            if (detached)
            {
                detachedForm.ClientSize = Size;
            }
        }

        public virtual bool Detached
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

        public virtual void ShowDetached()
        {
            Detached = true;
        }

        protected virtual void Detach()
        {
            detached = true;
            AttachedSize = Size;
            
            if (detachedForm != null)
            {
                detachedForm.Dispose();
            }

            detachedForm = new RadegastForm(RadegastInstance.GlobalInstance) { SettingsKeyBase = GetType().ToString(), AutoSavePosition = true };
            detachedForm.Icon = Properties.Resources.radegast_icon;
            OriginalParent = Parent;
            Parent = detachedForm;
            Dock = DockStyle.Fill;
            detachedForm.ClientSize = Size;
            SetTitle();
            detachedForm.ActiveControl = this;
            detachedForm.Show();
            detachedForm.FormClosing += new FormClosingEventHandler(detachedForm_FormClosing);

            if (OriginalParent == null)
            {
                ControlIsNotRetachable();
            }
            else
            {
                OriginalParent.ControlAdded += new ControlEventHandler(originalParent_ControlAdded);
            }

            if (OnDetached != null)
            {
                OnDetached();
            }
        }

        protected virtual void ControlIsNotRetachable() { }

        void originalParent_ControlAdded(object sender, ControlEventArgs e)
        {
            if (detachedForm != null)
            {
                detachedForm.FormClosing -= new FormClosingEventHandler(detachedForm_FormClosing);
            }
            OriginalParent.ControlAdded -= new ControlEventHandler(originalParent_ControlAdded);
            ControlIsNotRetachable();
        }

        void detachedForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Retach();
        }

        protected virtual void Retach()
        {
            detached = false;

            if (OriginalParent != null)
            {
                OriginalParent.ControlAdded -= new ControlEventHandler(originalParent_ControlAdded);
                Size = AttachedSize;
            }

            Parent = OriginalParent;

            if (detachedForm != null)
            {
                detachedForm.Dispose();
                detachedForm = null;
            }

            if (OriginalParent == null && DisposeOnDetachedClose)
            {
                Dispose();
            }

            if (OnRetached != null)
            {
                OnRetached();
            }
        }

        protected virtual void SetTitle()
        {
            if (detached)
            {
                FindForm().Text = Text + " - " + Properties.Resources.ProgramName;
            }
        }
    }
}