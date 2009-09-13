using System;
using System.Windows.Forms;

namespace Radegast
{
    public interface IContextAction : IDisposable
    {
        void IContextAction(RadegastInstance instance);
        ToolStripItem GetToolItem(object target);
        Button GetButton(object target);
        bool IsEnabled(object target);
        string LabelFor(object target);
        bool TypeContributes(Type o);
        bool Contributes(Object o);
        void OnInvoke(object sender, EventArgs e, object target);
    }
}