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
