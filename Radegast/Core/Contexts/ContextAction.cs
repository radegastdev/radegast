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
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public class ContextAction : IContextAction
    {
        public Type ContextType;
        public bool ExactContextType = false;
        public virtual string Label { get; set; }
        public EventHandler Handler;
        protected RadegastInstance instance;
        public bool ExecAsync = true;
        private Thread AsyncThread;
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
        protected ContextActionsManager ContextActionManager => Instance.ContextActionManager;

        public virtual RadegastInstance Instance
        {
            get => instance;
            set => instance = value;
        }

        protected virtual GridClient Client => Instance.Client;

        public ContextAction(RadegastInstance inst)
        {
            Enabled = true;
            instance = inst;
        }

        public virtual bool IsEnabled(object target)
        {
            return Enabled && Contributes(target,target?.GetType()) || Enabled;
        }

        public virtual string ToolTipText(object target)
        {
            return LabelFor(target) + " " + target;
        }

        public virtual IEnumerable<ToolStripMenuItem> GetToolItems(object target, Type type)
        {
            return new List<ToolStripMenuItem>()
                       {
                           new ToolStripMenuItem(
                LabelFor(target), (Image) null,
                (sender, e) => TCI(sender, e, target))
                       {
                           Enabled = IsEnabled(target),
                                   ToolTipText = ToolTipText(target)
                               }
                       };
        }

        private void TCI(object sender, EventArgs e, object target)
        {
            if (!ExecAsync)
            {
                TryCatch(() => OnInvoke(sender, e, target));
                return;
            }
            if (AsyncThread != null && AsyncThread.IsAlive)
            {
                AsyncThread.Abort();
            }
            AsyncThread = new Thread(() => TryCatch(() => OnInvoke(sender, e, target)))
                              {
                                  Name = Label + " Async command"
                              };
            AsyncThread.Start();                       
        }

        protected void InvokeSync(MethodInvoker func)
        {
            try
            {
                if (instance.MainForm.InvokeRequired)
                {
                    instance.MainForm.Invoke(func);
                    return;
                }
                func();
            }
            catch (Exception e)
            {
                DebugLog("Exception: " + e);
            }
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

        public virtual IEnumerable<Control> GetControls(object target, Type type)
        {
            Button button = new Button { Text = LabelFor(target), Enabled = IsEnabled(target) };
            button.Click += (sender, e) => TCI(sender, e, target);
            return new List<Control>() { button };
        }

        public virtual bool TypeContributes(Type o)
        {
            if (ExactContextType) return o.IsAssignableFrom(ContextType);
            return ContextType.IsAssignableFrom(o);
        }

        public virtual bool Contributes(Object o, Type type)
        {
            if (o==null) return false;
            if(TypeContributes(type)) return true;
            object oo = DeRef(o);
            return (oo != null && oo != o && Contributes(oo, oo.GetType()));
        }

        public virtual void OnInvoke(object sender, EventArgs e, object target)
        {

            object oneOf = target ?? sender;
            oneOf = GetValue(ContextType, oneOf);
            if (!ContextType.IsInstanceOfType(oneOf))
            {
                oneOf = GetValue(ContextType, DeRef(oneOf));
            }
            Handler?.Invoke(oneOf, e);
        }

        public virtual void IContextAction(RadegastInstance instance)
        {
            Instance = instance;
        }

        public virtual void Dispose()
        {           
        }
        public object GetValue(Type type, object lastObject)
        {
            if (type.IsInstanceOfType(lastObject)) return lastObject;
            if (type.IsAssignableFrom(typeof(Primitive))) return ToPrimitive(lastObject);
            if (type.IsAssignableFrom(typeof(Avatar))) return ToAvatar(lastObject);
            if (type.IsAssignableFrom(typeof(UUID))) return ToUUID(lastObject);
            return lastObject;
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
            if (uuid != UUID.Zero)
                thePrim = Client.Network.CurrentSim.ObjectsAvatars.Find(prim => (prim.ID == uuid));
            return thePrim;
        }

        public Avatar ToAvatar(object target)
        {
            Primitive thePrim = ((target is Primitive) ? (Primitive)target : null);
            if (thePrim is Avatar) return (Avatar)thePrim;
            if (thePrim != null && thePrim.Properties != null && thePrim.Properties.OwnerID != UUID.Zero)
            {
                target = thePrim.Properties.OwnerID;
            }
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
            if (target is Group)
            {
                return ((Group)target).ID;
            }
            if (target is Primitive)
            {
                return ((Primitive)target).ID;
            }
            if (target is Asset)
            {
                return ((Asset) target).AssetID;
            }
            if (target is InventoryItem)
            {
                return ((InventoryItem)target).AssetUUID;
            }
            if (uuid != UUID.Zero) return uuid;
            object oo = DeRef(target);
            if (oo != target)
            {
                uuid = ToUUID(oo);
                if (uuid != UUID.Zero) return uuid;
            }
            string str = ((target is string) ? (string)target : null);
            if (!string.IsNullOrEmpty(str))
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
           // instance.TabConsole.DisplayNotificationInChat(string.Format("ContextAction {0}: {1}", Label, s));
        }

        protected bool TryFindPos(object initial, out Simulator simulator, out Vector3 vector3)
        {
            simulator = Client.Network.CurrentSim;
            vector3 = Vector3.Zero;
            if (initial is Vector3)
            {
                vector3 = (Vector3) initial;
                return true;
            }
            if (initial is Vector3d)
            {
                var v3d = (Vector3d) initial;
                float lx, ly;
                ulong handle = Helpers.GlobalPosToRegionHandle((float) v3d.X, (float) v3d.Y, out lx, out ly);
                Simulator[] Simulators = null;
                lock (Client.Network.Simulators)
                {
                    Simulators = Client.Network.Simulators.ToArray();
                }
                foreach (Simulator s in Simulators)
                {
                    if (handle == s.Handle)
                    {
                        simulator = s;
                    }
                }
                vector3 = new Vector3(lx, ly, (float) v3d.Z);
                return true;
            }
            if (initial is UUID)
            {
                return instance.State.TryFindPrim((UUID)initial, out simulator, out vector3, false);
            }
            if (initial is Primitive)
            {
                return instance.State.TryLocatePrim((Primitive)initial, out simulator, out vector3);
            }
            UUID toUUID = ToUUID(initial);
            if (toUUID == UUID.Zero) return false;
            return instance.State.TryFindPrim(toUUID, out simulator, out vector3, false);
        }
    }
}