// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public class ContextActionsManager : IDisposable
    {
        List<IContextAction> contextEventHandlers = new List<IContextAction>();
        private readonly RadegastInstance instance;
        public ContextActionsManager(RadegastInstance instance)
        {
            this.instance = instance;
        }

        /// <summary>
        ///             AddContextMenu(typeof(Object), "Clueless Object!",
        ///                   (obj, hand)
        ///                   => Console.WriteLine("I am an Object!: {0} {1} {2}", obj, hand, obj.GetType()));
        /// </summary>
        /// <param name="libomvType"></param>
        /// <param name="label"></param>
        /// <param name="handler"></param>
        public void AddContextMenu(Type libomvType, String label, EventHandler handler)
        {
            AddContextMenu(new ContextAction(instance)
                               {
                                   Label = label,
                                   Handler = handler,
                                   ContextType = libomvType
                               });
        }

        public void AddContextMenu(IContextAction handler)
        {
            lock (contextEventHandlers)
            {
                handler.IContextAction(instance);
                contextEventHandlers.Add(handler);
            }
        }

        public void AddContributions(ToolStripDropDown strip, Object o)
        {
            SetCurrentItem(strip, o);
            AddContributions(strip, o != null ? o.GetType() : null, o);
        }

        public void AddContributions(ToolStripDropDown strip, List<ToolStripMenuItem> itemsIn)
        {
            if (itemsIn == null || itemsIn.Count == 0) return;
            List<ToolStripItem> items = new List<ToolStripItem>();
            itemsIn.ForEach(o =>
                                {
                                    string txt = (o.Text ?? "").ToLower().Trim();
                                    foreach (var i in strip.Items)
                                    {
                                        ToolStripItem item = (ToolStripItem) i;
                                        if (txt != (item.Text ?? "").ToLower().Trim()) continue;
                                        txt = null;
                                        break;
                                    }
                                    foreach (var item in items)
                                    {
                                        if (txt != (item.Text ?? "").ToLower().Trim()) continue;
                                        txt = null;
                                        break;
                                    }
                                    if (txt != null) items.Add(o);
                                });
            if (items.Count == 0) return;
            if (strip.Items.Count > 0)
                items.Insert(0, new ToolStripSeparator());
            strip.Items.AddRange(items.ToArray());
            strip.Closing += ((sender, args) => items.ForEach((o) => strip.Items.Remove(o)));
        }

        public void AddContributions(ToolStripDropDown strip, Type type, Object obj, params Control[] controls)
        {
            SetCurrentItem(strip, obj);
            List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
            GleanContributions(strip, type, obj, controls);
            if (strip.Parent != null) GleanContributions(strip, type, obj, strip.Parent);
            foreach (IContextAction i in contextEventHandlers)
            {
                if (!i.Contributes(obj, type)) continue;
                var v = i.GetToolItems(obj, type);
                if (v != null) items.AddRange(v);
            }
            items.Sort(CompareItems);
            AddContributions(strip, items);
            if (!instance.advancedDebugging) return;
            List<ToolStripMenuItem> item1 = new List<ToolStripMenuItem>();
            string newVariable = obj.GetType() == type
                                     ? type.Name
                                     : string.Format("{0} -> {1}", obj.GetType().Name, type.Name);
            item1.Add(new ToolStripMenuItem(newVariable, null,
                                            (sender, e) =>
                                            instance.TabConsole.DisplayNotificationInChat(
                                                string.Format(" sender={0}\ntarget={1}", ToString(sender), ToString(obj)))

                          )
            {
                Enabled = true,
                ToolTipText = "" + obj
            });

            AddContributions(strip, item1);
        }

        public void SetCurrentItem(ToolStripDropDown strip, object o)
        {
            RadegastContextMenuStrip rmenu = strip as RadegastContextMenuStrip;
            if (rmenu != null) rmenu.Selection = o;
        }

        private string ToString(object sender)
        {
            string t = sender.GetType().Name + ":";
            if (sender is Control)
            {
                Control control = (Control)sender;
                return string.Format("{0}{1} {2} {3}", t, control.Text, control.Name, ToString(control.Tag));
            }
            if (sender is ListViewItem)
            {
                ListViewItem control = (ListViewItem)sender;
                return string.Format("{0}{1} {2} {3}", t, control.Text, control.Name, ToString(control.Tag));
            }
            return t + sender;
        }

        public void GleanContributions(ToolStripDropDown strip, Type type, Object obj, params Control[] controls)
        {
            SetCurrentItem(strip, obj);
            List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
            foreach (Control control in controls) GleanContributions(items, type, control, obj);                
            if (obj is Control)
            {
                Control control1 = (Control) obj;
                GleanContributions(items, type, control1.Parent, obj);
            }
            if (items.Count == 0) return;
            items.Sort(CompareItems);
            AddContributions(strip, items);
        }

        public void GleanContributions(List<ToolStripMenuItem> items, Type type, Control control, Object obj)
        {
            if (control == null) return;
            if (control is Button)
            {
                Button button = (Button) control;
                string newVariable = obj.GetType() == type
                                         ? type.Name
                                         : string.Format("{0} -> {1}", obj.GetType().Name, type.Name);
                items.Add(new ToolStripMenuItem(button.Text, null,
                                                (sender, e) =>
                                                    {
                                                        button.PerformClick();
                                                        Logger.DebugLog(String.Format("Button: {0} {1} {2} {3}",
                                                                                      newVariable, e, sender, obj));
                                                    })
                              {
                                  Enabled = button.Enabled,
                                  ToolTipText = "" + obj
                              });
                return;
            }
            foreach (object v in control.Controls)
                GleanContributions(items, type, (Control) v, obj);
        }

        static int CompareItems(ToolStripItem i1, ToolStripItem i2)
        {
            if (i1 == i2) return 0;
            if (i1 is ToolStripSeparator)
                return (i2 is ToolStripSeparator) ? 0 : -1;
            int i = string.Compare(i1.Text, i2.Text);
            return i == 0 ? i1.GetHashCode().CompareTo(i2.GetHashCode()) : i;
        }

        public void Dispose()
        {
            lock (contextEventHandlers)
            {
                foreach (IContextAction handler in contextEventHandlers)
                {
                    try
                    {
                        handler.Dispose();
                    }
                    catch (Exception) { }
                }
                contextEventHandlers.Clear();
            }

        }
        public void LoadType(Type type)
        {
            if (typeof(IContextAction).IsAssignableFrom(type) && type != typeof(ContextAction))
            {
                try
                {
                    var c = type.GetConstructor(new Type[] { typeof(RadegastInstance) });
                    if (c != null)
                    {
                        IContextAction plug = (IContextAction)c.Invoke(new object[] { instance });
                        AddContextMenu(plug);
                        return;
                    }
                    c = type.GetConstructor(Type.EmptyTypes);
                    if (c != null)
                    {
                        IContextAction plug = (IContextAction)c.Invoke(new object[0]);
                        AddContextMenu(plug);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("ERROR in Radegast Command: " + type + " because " + ex.Message + " " + ex.StackTrace,
                               Helpers.LogLevel.Debug);
                    throw ex;
                }
                return;
            }
        }
    }
}