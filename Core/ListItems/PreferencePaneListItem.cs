using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Radegast
{
    public class PreferencePaneListItem
    {
        private string name;
        private Image icon;

        public PreferencePaneListItem(string name, Image icon)
        {
            this.name = name;
            this.icon = icon;
        }

        public string Name
        {
            get { return name; }
        }

        public Image Icon
        {
            get { return icon; }
        }
    }
}
