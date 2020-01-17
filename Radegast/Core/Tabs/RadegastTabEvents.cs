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
            TabSelected?.Invoke(this, e);
        }

        protected virtual void OnTabDeselected(EventArgs e)
        {
            TabDeselected?.Invoke(this, e);
        }

        protected virtual void OnTabHighlighted(EventArgs e)
        {
            TabHighlighted?.Invoke(this, e);
        }

        protected virtual void OnTabUnhighlighted(EventArgs e)
        {
            TabUnhighlighted?.Invoke(this, e);
        }

        protected virtual void OnTabPartiallyHighlighted(EventArgs e)
        {
            TabPartiallyHighlighted?.Invoke(this, e);
        }

        protected virtual void OnTabMerged(EventArgs e)
        {
            TabMerged?.Invoke(this, e);
        }

        protected virtual void OnTabSplit(EventArgs e)
        {
            TabSplit?.Invoke(this, e);
        }

        protected virtual void OnTabDetached(EventArgs e)
        {
            TabDetached?.Invoke(this, e);
        }

        protected virtual void OnTabAttached(EventArgs e)
        {
            TabAttached?.Invoke(this, e);
        }

        protected virtual void OnTabClosed(EventArgs e)
        {
            TabClosed?.Invoke(this, e);
        }

        protected virtual void OnTabHidden(EventArgs e)
        {
            TabHidden?.Invoke(this, e);
        }

        protected virtual void OnTabShown(EventArgs e)
        {
            TabShown?.Invoke(this, e);
        }
    }
}
