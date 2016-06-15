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
                toolstrip.BackColor = System.Drawing.Color.FromArgb(toolstrip.BackColor.ToArgb());
                toolstrip.ForeColor = System.Drawing.Color.FromArgb(toolstrip.ForeColor.ToArgb());
                toolstrip.RenderMode = ToolStripRenderMode.System;

                foreach (var item in toolstrip.Items)
                {
                    ApplyThemeCompatibilityModeRecursive(item);
                }
            }
            else if (root is ToolStripItem)
            {
                var toolstripitem = root as ToolStripItem;
                toolstripitem.BackColor = System.Drawing.Color.FromArgb(toolstripitem.BackColor.ToArgb());
                toolstripitem.ForeColor = System.Drawing.Color.FromArgb(toolstripitem.ForeColor.ToArgb());
            }
            else if (root is Control)
            {

                var control = root as Control;
                control.BackColor = System.Drawing.Color.FromArgb(control.BackColor.ToArgb());
                control.ForeColor = System.Drawing.Color.FromArgb(control.ForeColor.ToArgb());

                foreach (var item in control.Controls)
                {
                    var sub_control = item as Control;
                    if (sub_control != null)
                    {
                        ApplyThemeCompatibilityModeRecursive(sub_control);
                    }
                }
            }
        }
    }
}
