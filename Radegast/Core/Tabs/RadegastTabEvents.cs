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

namespace Radegast
{
    public partial class RadegastTab
    {
        public event EventHandler TabSelected;
        public event EventHandler TabDeselected;
        public event EventHandler TabHighlighted;
        public event EventHandler TabUnhighlighted;
        public event EventHandler TabPartiallyHighlighted;
        public event EventHandler TabMerged;
        public event EventHandler TabSplit;
        public event EventHandler TabDetached;
        public event EventHandler TabAttached;
        public event EventHandler TabClosed;
        public event EventHandler TabHidden;
        public event EventHandler TabShown;

        protected virtual void OnTabSelected(EventArgs e)
        {
            if (TabSelected != null) TabSelected(this, e);
        }

        protected virtual void OnTabDeselected(EventArgs e)
        {
            if (TabDeselected != null) TabDeselected(this, e);
        }

        protected virtual void OnTabHighlighted(EventArgs e)
        {
            if (TabHighlighted != null) TabHighlighted(this, e);
        }

        protected virtual void OnTabUnhighlighted(EventArgs e)
        {
            if (TabUnhighlighted != null) TabUnhighlighted(this, e);
        }

        protected virtual void OnTabPartiallyHighlighted(EventArgs e)
        {
            if (TabPartiallyHighlighted != null) TabPartiallyHighlighted(this, e);
        }

        protected virtual void OnTabMerged(EventArgs e)
        {
            if (TabMerged != null) TabMerged(this, e);
        }

        protected virtual void OnTabSplit(EventArgs e)
        {
            if (TabSplit != null) TabSplit(this, e);
        }

        protected virtual void OnTabDetached(EventArgs e)
        {
            if (TabDetached != null) TabDetached(this, e);
        }

        protected virtual void OnTabAttached(EventArgs e)
        {
            if (TabAttached != null) TabAttached(this, e);
        }

        protected virtual void OnTabClosed(EventArgs e)
        {
            if (TabClosed != null) TabClosed(this, e);
        }

        protected virtual void OnTabHidden(EventArgs e)
        {
            if (TabHidden != null) TabHidden(this, e);
        }

        protected virtual void OnTabShown(EventArgs e)
        {
            if (TabShown != null) TabShown(this, e);
        }
    }
}
