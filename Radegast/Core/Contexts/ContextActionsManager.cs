using System;
using System.Collections.Generic;
using System.Drawing;
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

        internal void AddContributions(ToolStripDropDown strip, Object o)
        {
            AddContributions(strip, o.GetType(), o);
        }

        public void AddContributions(ToolStripDropDown strip, List<ToolStripItem> items)
        {
            if (items==null || items.Count == 0) return; 
            foreach(var i in strip.Items)
            {
               if (i is ToolStripItem)
               {
                   ToolStripItem item = (ToolStripItem) i;
                   if (string.IsNullOrEmpty(item.Text)) continue;
                   ToolStripItem dup = items.Find((o)=> !string.IsNullOrEmpty(o.Text) && o.Text.ToLower() == item.Text.ToLower());
                   if (dup!=null)
                       items.Remove(dup);
               }
            }
            if (items.Count == 0) return; 
            strip.Items.AddRange(items.ToArray());
            strip.Closing += ((sender, args) => items.ForEach((o) => strip.Items.Remove(o)));
        }

        internal void AddContributions(ToolStripDropDown strip, Type type, Object obj, params Control[] controls)
        {
            List<ToolStripItem> items = new List<ToolStripItem>();
            GleanContributions(strip, type, obj, controls);
            if (strip.Parent != null) GleanContributions(strip, type, obj, strip.Parent);
            foreach (IContextAction i in contextEventHandlers)
            {
                if (!i.TypeContributes(type) && !i.Contributes(obj)) continue;
                items.Add(i.GetToolItem(obj));
            }
            items.Sort(CompareItems);
            if (strip.Items.Count > 0) items.Insert(0, new ToolStripSeparator());
            //if (!instance.advancedDebugging) return;
            if (items.Count > 1) items.Add(new ToolStripSeparator());
            string newVariable = obj.GetType() == type
                                     ? type.Name
                                     : string.Format("{0} -> {1}", obj.GetType().Name, type.Name);
            items.Add(new ToolStripMenuItem(newVariable, null,
                                            (sender, e) =>
                                            instance.TabConsole.DisplayNotificationInChat(
                                                string.Format(" sender={0}\ntarget={1}", ToString(sender), ToString(obj)))

                          )
            {
                Enabled = true,
                ToolTipText = "" + obj
            });

            AddContributions(strip, items);
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
            List<ToolStripItem> items = new List<ToolStripItem>();
            foreach (Control control in controls) GleanContributions(items, type, control, obj);                
            if (obj is Control)
            {
                Control control1 = (Control) obj;
                GleanContributions(items, type, control1.Parent, obj);
            }
            if (items.Count == 0) return;
            items.Sort(CompareItems);
            if (strip.Items.Count > 0) items.Insert(0, new ToolStripSeparator());
            AddContributions(strip, items);
        }

        public void GleanContributions(List<ToolStripItem> items, Type type, Control control, Object obj)
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