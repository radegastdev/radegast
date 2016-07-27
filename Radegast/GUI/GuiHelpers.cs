using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Radegast.GUI
{
    public static class GuiHelpers
    {
        public static void ApplyGuiFixes(Object root)
        {
            try
            {
                var instance = Radegast.RadegastInstance.GlobalInstance;
                if (instance.GlobalSettings["theme_compatibility_mode"])
                {
                    ApplyThemeCompatibilityModeRecursive(root);
                }
            }
            catch (Exception)
            {
                // Suppress exceptions that will be raised above in designer mode.
            }

        }

        private static void ApplyThemeCompatibilityModeRecursive(Object root)
        {
            if (root is ToolStrip)
            {
                var toolstrip = root as ToolStrip;
                toolstrip.RenderMode = ToolStripRenderMode.System;
                toolstrip.BackColor = System.Drawing.SystemColors.Control;
                toolstrip.ForeColor = System.Drawing.SystemColors.ControlText;
                toolstrip.BackColor = System.Drawing.Color.Transparent;

                foreach (var item in toolstrip.Items)
                {
                    ApplyThemeCompatibilityModeRecursive(item);
                }
            }
            else if (root is ToolStripDropDownItem)
            {
                var dropDownItem = root as ToolStripDropDownItem;
                dropDownItem.BackColor = System.Drawing.SystemColors.Control;
                dropDownItem.ForeColor = System.Drawing.SystemColors.ControlText;
                dropDownItem.DropDown.RenderMode = ToolStripRenderMode.System;
                dropDownItem.DropDown.BackColor = System.Drawing.SystemColors.Control;
                dropDownItem.DropDown.ForeColor = System.Drawing.SystemColors.ControlText;

                foreach (var item in dropDownItem.DropDownItems)
                {
                    ApplyThemeCompatibilityModeRecursive(item);
                }
            }
            else if (root is Control && !(root is ProgressBar))
            {
                var control = root as Control;
                control.BackColor = System.Drawing.SystemColors.Control;
                control.ForeColor = System.Drawing.SystemColors.ControlText;

                foreach (var item in control.Controls)
                {
                    ApplyThemeCompatibilityModeRecursive(item);
                }
            }
        }
    }
}
