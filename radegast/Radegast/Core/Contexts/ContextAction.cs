// 
// Radegast Metaverse Client
// Copyright (c) 2009-2011, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id: 
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.Assets;

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
            return Enabled && Contributes(target,target!=null?target.GetType():null) || Enabled;
        }

        public virtual IEnumerable<ToolStripMenuItem> GetToolItems(object target, Type type)
        {
            return new List<ToolStripMenuItem>(){new ToolStripMenuItem(
                LabelFor(target), (Image) null,
                (sender, e) => TryCatch(() => OnInvoke(sender, e, target)))
                       {
                           Enabled = IsEnabled(target),
                       }};
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
            button.Click += (sender, e) => TryCatch(() => OnInvoke(sender, e, target));
            return new List<Control>() { button };
        }

        public virtual bool TypeContributes(Type o)
        {
            return ContextType.IsAssignableFrom(o);
        }

        public virtual bool Contributes(Object o, Type type)
        {
            if (o==null) return false;
            object oo = DeRef(o);
            return (oo != o && Contributes(oo,type)) || TypeContributes(type) || TypeContributes(o.GetType());
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
            if (uuid != UUID.Zero)
                thePrim = Client.Network.CurrentSim.ObjectsAvatars.Find(prim => (prim.ID == uuid));
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
           // instance.TabConsole.DisplayNotificationInChat(string.Format("ContextAction {0}: {1}", Label, s));
        }
    }
}