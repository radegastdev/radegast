using System;
using System.Drawing;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public class ContextAction : IContextAction
    {
        public Type ContextType;
        public virtual string Label { get; set; }
        public EventHandler Handler;
        protected RadegastInstance instance;
        public bool Enabled { get; set; }
        public virtual object DeRef(object o)
        {
            if (o is Control)
            {
                Control control = (Control) o;
                if (control.Tag != null) return control.Tag;
                if (!string.IsNullOrEmpty(control.Text)) return control.Text;
                if (!string.IsNullOrEmpty(control.Name)) return control.Name;
            }
            else if (o is ListViewItem)
            {
                ListViewItem control = (ListViewItem) o;
                if (control.Tag != null) return control.Tag;
                if (!string.IsNullOrEmpty(control.Name)) return control.Name;
                if (!string.IsNullOrEmpty(control.Text)) return control.Text;
            }
            return o;
        }
        protected ContextActionsManager ContextActionManager
        {
            get { return Instance.ContextActionManager; }
        }
        public virtual RadegastInstance Instance
        {
            get
            {
                return instance;
            }
            set
            {
                if (instance == value)
                    return;
                instance = value;
            }
        }

        protected virtual GridClient Client
        {
            get
            {
                return Instance.Client;
            }
        }

        public ContextAction(RadegastInstance inst)
        {
            Enabled = true;
            instance = inst;
        }

        public virtual bool IsEnabled(object target)
        {
            return Enabled && Contributes(target);
        }

        public virtual ToolStripItem GetToolItem(object target)
        {
            return new ToolStripMenuItem(
                LabelFor(target), (Image) null,
                (sender, e) => TryCatch(() => OnInvoke(sender, e, target)))
                       {
                           Enabled = IsEnabled(target),
                       };
        }

        protected void TryCatch(MethodInvoker func)
        {
            try
            {
                func();
            }
            catch (Exception e)
            {
                DebugLog("Exception: " + e);
            }
        }


        public virtual string LabelFor(object target)
        {
            return Label;
        }

        public virtual Button GetButton(object target)
        {
            Button button = new Button { Text = LabelFor(target), Enabled = IsEnabled(target) };
            button.Click += (sender, e) => TryCatch(() => OnInvoke(sender, e, target));
            return button;
        }

        public virtual bool TypeContributes(Type o)
        {
            return ContextType.IsAssignableFrom(o);
        }

        public virtual bool Contributes(Object o)
        {
            if (o==null) return false;
            object oo = DeRef(o);
            return (oo != o && Contributes(oo)) || TypeContributes(o.GetType());
        }

        public virtual void OnInvoke(object sender, EventArgs e, object target)
        {
            if (Handler != null) Handler(DeRef(target ?? sender), e);
        }

        public virtual void IContextAction(RadegastInstance instance)
        {
            Instance = instance;
        }

        public virtual void Dispose()
        {           
        }

        public Primitive ToPrimitive(object target)
        {
            Primitive thePrim = ((target is Primitive) ? (Primitive)target : null);
            if (thePrim != null) return thePrim;
            object oo = DeRef(target);
            if (oo != target)
            {
                thePrim = ToAvatar(oo);
                if (thePrim != null) return thePrim;
            }
            UUID uuid = ((target is UUID) ? (UUID)target : UUID.Zero);
            if (uuid != UUID.Zero)
                thePrim = Client.Network.CurrentSim.ObjectsPrimitives.Find(prim => (prim.ID == uuid));
            return thePrim;
        }

        public Avatar ToAvatar(object target)
        {
            Primitive thePrim = ((target is Primitive) ? (Primitive)target : null);
            if (thePrim != null) return (Avatar)thePrim;
            object oo = DeRef(target);
            if (oo != target)
            {
                thePrim = ToAvatar(oo);
                if (thePrim != null) return (Avatar)thePrim;
            }
            UUID uuid = ((target is UUID) ? (UUID)target : UUID.Zero);
            if (uuid != UUID.Zero)
                thePrim = Client.Network.CurrentSim.ObjectsAvatars.Find(prim => (prim.ID == uuid));
            return (Avatar)thePrim;
        }

        public UUID ToUUID(object target)
        {
            UUID uuid = ((target is UUID) ? (UUID)target : UUID.Zero);
            if (target is FriendInfo)
            {
                return ((FriendInfo) target).UUID;
            }
            if (target is GroupMember)
            {
                return ((GroupMember)target).ID;
            }
            if (uuid != UUID.Zero) return uuid;
            object oo = DeRef(target);
            if (oo != target)
            {
                uuid = ToUUID(oo);
                if (uuid != UUID.Zero) return uuid;
            }
            string str = ((target is string) ? (string)target : null);
            if (string.IsNullOrEmpty(str))
            {
                if (UUID.TryParse(str, out uuid))
                {
                    return uuid;
                }
            }
            Primitive prim = ToPrimitive(target);
            if (prim != null) return prim.ID;
            Avatar avatar = ToAvatar(target);
            if (avatar != null) return avatar.ID;
            return uuid;
        }

        public void DebugLog(string s)
        {
            instance.TabConsole.DisplayNotificationInChat(string.Format("ContextAction {0}: {1}", Label, s));
        }
    }
}