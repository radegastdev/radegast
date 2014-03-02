// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
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
        /// Register a Context Action 
        ///  <code>           RegisterContextAction(typeof(Object), "Clueless Object!",
        ///                   (obj, hand)
        ///                   => Console.WriteLine("I am an Object!: {0} {1} {2}", obj, hand, obj.GetType()));
        /// </code>
        /// </summary>
        /// <param name="libomvType"></param>
        /// <param name="label"></param>
        /// <param name="handler">Action<object,EventArgs></param>
        public void RegisterContextAction(Type libomvType, String label, EventHandler handler)
        {
            RegisterContextAction(new ContextAction(instance)
                               {
                                   Label = label,
                                   Handler = handler,
                                   ContextType = libomvType,
                                   ExactContextType = true
                               });
        }

        /// <summary>
        /// Deregister a Context Action 
        ///  <code></code>DeregisterContextAction(typeof(Object), "Clueless Object!");</code>
        /// </summary>
        /// <param name="libomvType"></param>
        /// <param name="label"></param>
        public void DeregisterContextAction(Type libomvType, String label)
        {
            lock (contextEventHandlers)
            {
                contextEventHandlers.RemoveAll(e =>
                                                   {
                                                       ContextAction ca = e as ContextAction;
                                                       return ca != null && ca.Label == label &&
                                                              libomvType == ca.ContextType;
                                                   });
            }
        }

        public void RegisterContextAction(IContextAction handler)
        {
            lock (contextEventHandlers)
            {
                handler.IContextAction(instance);
                contextEventHandlers.Add(handler);
            }
        }

        /// <summary>
        /// Register a Context Action 
        /// </summary>
        /// <param name="handler"></param>
        public void DeregisterContextAction(IContextAction handler)
        {
            lock (contextEventHandlers)
            {
                contextEventHandlers.Remove(handler);
            }
        }

        /// <summary>
        /// Used by UI forms to add new ContextMenu Items to a Menu they are about to Display based on the Object
        /// </summary>
        /// <param name="strip">The form's menu</param>
        /// <param name="o">The target object</param>
        public void AddContributions(ToolStripDropDown strip, Object o)
        {
            SetCurrentItem(strip, o);
            AddContributions(strip, o != null ? o.GetType() : null, o);
        }

        /// <summary>
        /// Used by AddContributions to add new ContextMenu Items to a Menu they are about to Display
        /// </summary>
        /// <param name="strip">The form's menu</param>
        /// <param name="itemsIn">New Items to Add</param>
        public static void AddContributions_Helper(ToolStripDropDown strip, List<ToolStripMenuItem> itemsIn)
        {
            if (itemsIn == null || itemsIn.Count == 0) return;
            List<ToolStripItem> items = new List<ToolStripItem>();
            itemsIn.ForEach(o =>
                                {
                                    string txt = (o.Text ?? "").ToLower().Trim();
                                    foreach (var i in strip.Items)
                                    {
                                        ToolStripItem item = (ToolStripItem)i;
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

        /// <summary>
        /// Used by UI forms to add new ContextMenu Items to a Menu they are about to Display
        /// </summary>
        /// <param name="strip">The form's menu</param>        
        /// <param name="type">The type it will target</param>
        /// <param name="obj">the Target ofbject</param>
        /// <param name="controls">Control to search for extra contributions (like buttons)</param>
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
                if (v != null)
                {
                    foreach (ToolStripMenuItem item in v)
                    {
                        bool alreadyPresent = false;
                        foreach (object down in strip.Items)
                        {
                            if (down is ToolStripMenuItem)
                            {
                                ToolStripMenuItem mi = (ToolStripMenuItem) down;
                                if ((mi.Text ?? "").ToLower() == (item.Text ?? "").ToLower())
                                {
                                    alreadyPresent = true;
                                    break;
                                }
                            }
                        }
                        if (alreadyPresent) continue;
                        items.Add(item);
                    }
                }
            }
            items.Sort(CompareItems);
            AddContributions_Helper(strip, items);
            
#if DEBUG_CONTEXT
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

            AddContributions_Helper(strip, item1);
#endif
        }

        /// <summary>
        /// Used by UI forms to set the Context target (saved in the toplevel strip if it's a RadegastContextMenuStrip)
        /// </summary>
        /// <param name="strip"></param>
        /// <param name="o"></param>
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

        /// <summary>
        /// Used by UI forms to add new ContextMenu Items gleaned from Controls
        /// </summary>
        /// <param name="strip">The form's menu</param>        
        /// <param name="type">The type it will target</param>
        /// <param name="obj">the Target ofbject</param>
        /// <param name="controls">Control to search for extra contributions (like buttons)</param>
        public void GleanContributions(ToolStripDropDown strip, Type type, Object obj, params Control[] controls)
        {
            SetCurrentItem(strip, obj);
            List<ToolStripMenuItem> items = new List<ToolStripMenuItem>();
            foreach (Control control in controls) GleanContributions_Helper(items, type, control, obj);
            if (obj is Control)
            {
                Control control1 = (Control)obj;
                GleanContributions_Helper(items, type, control1.Parent, obj);
            }
            if (items.Count == 0) return;
            items.Sort(CompareItems);
            AddContributions_Helper(strip, items);
        }

        /// <summary>
        /// Used by GleanContributions to add new ContextMenu Items gleaned from Parent control that has buttons on it
        /// </summary>
        /// <param name="items">Collection of Items to be added to</param>        
        /// <param name="type">The type it will target</param>
        /// <param name="control">Parent control that has buttons on it</param>
        /// <param name="obj">Will becvome the button's target when </param>
        static public void GleanContributions_Helper(ICollection<ToolStripMenuItem> items, Type type, Control control, Object obj)
        {
            if (control == null) return;
            if (control is Button)
            {
                Button button = (Button)control;
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
                                  Visible = button.Visible,
                                  ToolTipText = "" + obj
                              });
                return;
            }
            foreach (object v in control.Controls)
                GleanContributions_Helper(items, type, (Control)v, obj);
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
        /// <summary>
        /// Loads context actions
        /// </summary>
        /// <param name="type">Type to try to load</param>
        /// <returns>True if useful types were found and loaded</returns>
        public bool LoadType(Type type)
        {
            if (typeof(IContextAction).IsAssignableFrom(type) && type != typeof(ContextAction))
            {
                try
                {
                    var c = type.GetConstructor(new Type[] { typeof(RadegastInstance) });
                    if (c != null)
                    {
                        IContextAction plug = (IContextAction)c.Invoke(new object[] { instance });
                        RegisterContextAction(plug);
                        return true;
                    }
                    c = type.GetConstructor(Type.EmptyTypes);
                    if (c != null)
                    {
                        IContextAction plug = (IContextAction)c.Invoke(new object[0]);
                        RegisterContextAction(plug);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Log("ERROR in Radegast Command: " + type + " because " + ex.Message + " " + ex.StackTrace,
                               Helpers.LogLevel.Debug);
                    throw ex;
                }
            }
            return false;
        }
    }
}