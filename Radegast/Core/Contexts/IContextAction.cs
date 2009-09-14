using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Radegast
{
    public interface IContextAction : IDisposable
    {
        void IContextAction(RadegastInstance instance);
        IEnumerable<ToolStripMenuItem> GetToolItems(object target, Type type);
        IEnumerable<Control> GetControls(object target, Type type);
        bool IsEnabled(object target);
        string LabelFor(object target);
        bool Contributes(Object o, Type type);
        void OnInvoke(object sender, EventArgs e, object target);
    }
}