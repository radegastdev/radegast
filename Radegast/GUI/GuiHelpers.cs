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
            var instance = Radegast.RadegastInstance.GlobalInstance;
            if (instance.GlobalSettings["theme_compatibility_mode"])
            {
                ApplyThemeCompatibilityModeRecursive(root);
            }
        }

        private static void ApplyThemeCompatibilityModeRecursive(Object root)
        {
            if (root is ToolStrip)
            {
                var toolstrip = root as ToolStrip;
                toolstrip.RenderMode = ToolStripRenderMode.System;

                foreach (var item in toolstrip.Items)
                {
                    ApplyThemeCompatibilityModeRecursive(item);
                }
            }
            else if (root is ListBox)
            {
                var item = root as ListBox;
                item.BackColor = System.Drawing.SystemColors.Window;
                item.ForeColor = System.Drawing.SystemColors.WindowText;
            }
            else if (root is TextBox)
            {
                var item = root as TextBox;
                item.BackColor = System.Drawing.SystemColors.Control;
                item.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            else if (root is Label)
            {
                var item = root as Label;
                item.BackColor = System.Drawing.SystemColors.Control;
                item.ForeColor = System.Drawing.SystemColors.ControlText;
            }
            else if (root is Control)
            {
                var control = root as Control;
                foreach (var item in control.Controls)
                {
                    ApplyThemeCompatibilityModeRecursive(item);
                }
            }
        }
    }
}
