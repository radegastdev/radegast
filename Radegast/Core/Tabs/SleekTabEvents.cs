using System;
using System.Collections.Generic;
using System.Text;

namespace Radegast
{
    public partial class SleekTab
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
    }
}
