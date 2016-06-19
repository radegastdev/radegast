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
// $Id$
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows.Forms;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;
using System.Drawing;
using OpenMetaverse;
using OpenMetaverse.Assets;

namespace Radegast
{
    public partial class ObjectsConsole : UserControl, IContextMenuProvider
    {
        public List<Primitive> Prims = new List<Primitive>();

        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private Primitive currentPrim = new Primitive();
        private float searchRadius = 40.0f;
        //public List<InventoryBase> subitems;
        PropertiesQueue propRequester;
        private Thread ContentsThread;
        private ObjectConsoleFilter filter;
        private ObjectSorter PrimSorter;

        public Primitive CurrentPrim { get { return currentPrim; } }

        public ObjectsConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmObjects_Disposed);

            this.instance = instance;

            propRequester = new PropertiesQueue(instance);
            propRequester.OnTick += new PropertiesQueue.TickCallback(propRequester_OnTick);

            btnPointAt.Text = (this.instance.State.IsPointing ? "Unpoint" : "Point At");
            State_SitStateChanged(this, new SitEventArgs(instance.State.IsSitting));

            nudRadius.Value = (decimal)searchRadius;
            nudRadius.ValueChanged += nudRadius_ValueChanged;

            PrimSorter = new ObjectSorter(client.Self);

            lstContents.LargeImageList = frmMain.ResourceImages;
            lstContents.SmallImageList = frmMain.ResourceImages;

            filter = (ObjectConsoleFilter)instance.GlobalSettings["object_console_filter"].AsInteger();
            comboFilter.SelectedIndex = 0;
            try
            {
                comboFilter.SelectedIndex = (int)filter;
            }
            catch { }
            comboFilter.SelectedIndexChanged += (ssender, se) =>
            {
                instance.GlobalSettings["object_console_filter"] = comboFilter.SelectedIndex;
                filter = (ObjectConsoleFilter)comboFilter.SelectedIndex;
                btnRefresh_Click(null, null);
            };

            //if (instance.MonoRuntime)
            //{
            //    btnView.Visible = false;
            //}

            // Callbacks
            instance.Netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);
            instance.State.SitStateChanged += new EventHandler<SitEventArgs>(State_SitStateChanged);
            client.Objects.ObjectUpdate += new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
            client.Objects.KillObjects += new EventHandler<KillObjectsEventArgs>(Objects_KillObjects);
            client.Objects.ObjectProperties += new EventHandler<ObjectPropertiesEventArgs>(Objects_ObjectProperties);
            client.Objects.ObjectPropertiesFamily += new EventHandler<ObjectPropertiesFamilyEventArgs>(Objects_ObjectPropertiesFamily);
            client.Network.SimChanged += new EventHandler<SimChangedEventArgs>(Network_SimChanged);
            client.Self.MuteListUpdated += new EventHandler<EventArgs>(Self_MuteListUpdated);
            instance.Names.NameUpdated += new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
            instance.State.OnWalkStateCanged += new StateManager.WalkStateCanged(State_OnWalkStateCanged);

            Radegast.GUI.GuiHelpers.ApplyGuiFixes(this);
        }

        void frmObjects_Disposed(object sender, EventArgs e)
        {
            if (ContentsThread != null)
            {
                if (ContentsThread.IsAlive) ContentsThread.Abort();
                ContentsThread = null;
            }

            propRequester.Dispose();
            instance.Netcom.ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);
            instance.State.SitStateChanged -= new EventHandler<SitEventArgs>(State_SitStateChanged);
            client.Objects.ObjectUpdate -= new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
            client.Objects.KillObjects -= new EventHandler<KillObjectsEventArgs>(Objects_KillObjects);
            client.Objects.ObjectProperties -= new EventHandler<ObjectPropertiesEventArgs>(Objects_ObjectProperties);
            client.Objects.ObjectPropertiesFamily -= new EventHandler<ObjectPropertiesFamilyEventArgs>(Objects_ObjectPropertiesFamily);
            client.Network.SimChanged -= new EventHandler<SimChangedEventArgs>(Network_SimChanged);
            client.Self.MuteListUpdated -= new EventHandler<EventArgs>(Self_MuteListUpdated);
            instance.Names.NameUpdated -= new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
            instance.State.OnWalkStateCanged -= new StateManager.WalkStateCanged(State_OnWalkStateCanged);
        }

        void State_SitStateChanged(object sender, SitEventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => State_SitStateChanged(sender, e)));
                }
                return;
            }

            btnSitOn.Text = (this.instance.State.IsSitting ? "Stand Up" : "Sit On");
        }

        public void RefreshObjectList()
        {
            btnRefresh_Click(this, EventArgs.Empty);
        }

        public List<Primitive> GetObjectList()
        {
            return new List<Primitive>(Prims);
        }

        void propRequester_OnTick(int remaining)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        propRequester_OnTick(remaining);
                    }
                ));
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Tracking {0} objects", Prims.Count);

            if (remaining > 10)
            {
                sb.AppendFormat(", fetching {0} object names.", remaining);
            }
            else
            {
                sb.Append(".");
            }

            lock (Prims)
            {
                Prims.Sort(PrimSorter);
                lstPrims.VirtualListSize = Prims.Count;
            }
            lstPrims.Invalidate();
            lblStatus.Text = sb.ToString();
        }

        void Network_SimChanged(object sender, SimChangedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => Network_SimChanged(sender, e)));
                return;
            }

            btnRefresh_Click(null, null);
        }

        void Avatars_UUIDNameReply(object sender, UUIDNameReplyEventArgs e)
        {
            int minIndex = -1;
            int maxIndex = -1;
            bool updated = false;

            lock (Prims)
            {
                for (int i = 0; i < Prims.Count; i++)
                {
                    Primitive prim = Prims[i];
                    if (prim.Properties != null && e.Names.ContainsKey(prim.Properties.OwnerID))
                    {
                        if (minIndex == -1 || i < minIndex) minIndex = i;
                        if (i > maxIndex) maxIndex = i;
                        updated = true;
                    }
                }
            }

            if (updated)
            {
                if (InvokeRequired)
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        try
                        {
                            lstPrims.RedrawItems(minIndex, maxIndex, true);
                        }
                        catch { }
                    }));
                }
                else
                {
                    lstPrims.RedrawItems(minIndex, maxIndex, true);
                }
            }
        }

        void UpdateProperties(Primitive.ObjectProperties props)
        {
            lock (Prims)
            {
                for (int i = 0; i < Prims.Count; i++)
                {
                    if (Prims[i].ID == props.ObjectID)
                    {
                        Prims[i].Properties = props;
                        try
                        {
                            lstPrims.RedrawItems(i, i, true);
                        }
                        catch { }
                        break;
                    }
                }
            }

            lock (lstChildren.Items)
            {
                if (lstChildren.Items.ContainsKey(props.ObjectID.ToString()))
                {
                    Primitive prim = lstChildren.Items[props.ObjectID.ToString()].Tag as Primitive;
                    prim.Properties = props;
                    lstChildren.Items[props.ObjectID.ToString()].Text = prim.Properties.Name;
                }
            }

            if (props.ObjectID == currentPrim.ID)
            {
                UpdateCurrentObject(false);
            }
        }

        void Objects_ObjectProperties(object sender, ObjectPropertiesEventArgs e)
        {
            if (e.Simulator.Handle != client.Network.CurrentSim.Handle)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { Objects_ObjectProperties(sender, e); }));
                return;
            }

            UpdateProperties(e.Properties);
        }

        void Objects_ObjectPropertiesFamily(object sender, ObjectPropertiesFamilyEventArgs e)
        {
            if (e.Simulator.Handle != client.Network.CurrentSim.Handle)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { Objects_ObjectPropertiesFamily(sender, e); }));
                return;
            }

            UpdateProperties(e.Properties);
        }


        void UpdateObjectContents()
        {
            if (ContentsThread != null)
            {
                if (ContentsThread.IsAlive) ContentsThread.Abort();
                ContentsThread = null;
            }

            lstContents.Items.Clear();
            ListViewItem entry = new ListViewItem();
            entry.SubItems.Add("Loading...");
            lstContents.Items.Add(entry);

            ContentsThread = new Thread(new ThreadStart(() =>
                {
                    lstContents.Tag = currentPrim;
                    List<InventoryBase> items = client.Inventory.GetTaskInventory(currentPrim.ID, currentPrim.LocalID, 1000 * 30);
                    lstContents.Invoke(new MethodInvoker(() => UpdateContentsList(items)));
                }));

            ContentsThread.IsBackground = true;
            ContentsThread.Start();

        }

        void UpdateContentsList(List<InventoryBase> items)
        {
            //object inventory in liste reinlesen
            lstContents.Items.Clear();
            btnOpen.Enabled = false;
            Primitive prim = (Primitive)lstContents.Tag;

            //subitems = items;
            if (items == null)
            {
                ListViewItem entry = new ListViewItem();
                entry.SubItems.Add("(failied to fetch contents)");
                entry.SubItems[0].Font = new System.Drawing.Font(entry.SubItems[0].Font, System.Drawing.FontStyle.Italic);
                lstContents.Items.Add(entry);
            }
            else
            {
                if (items.Count == 0)
                {
                    ListViewItem entry = new ListViewItem();
                    entry.SubItems.Add("(empty object)");
                    entry.SubItems[0].Font = new System.Drawing.Font(entry.SubItems[0].Font, System.Drawing.FontStyle.Italic);
                    lstContents.Items.Add(entry);
                }
                else
                {
                    btnOpen.Enabled = prim.Properties != null && prim.Properties.OwnerID == client.Self.AgentID;

                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i] is InventoryItem)
                        {
                            InventoryItem item = (InventoryItem)items[i];
                            ListViewItem entry = new ListViewItem();

                            entry.ImageIndex = InventoryConsole.GetItemImageIndex(item.AssetType.ToString().ToLower());
                            entry.Tag = item;

                            ListViewItem.ListViewSubItem sub;

                            entry.SubItems.Add(sub = new ListViewItem.ListViewSubItem()
                            {
                                Name = item.UUID.ToString(),
                                Text = item.Name
                            });

                            if ((item.Permissions.OwnerMask & PermissionMask.Modify) == 0)
                                sub.Text += " (no modify)";

                            if ((item.Permissions.OwnerMask & PermissionMask.Copy) == 0)
                                sub.Text += " (no copy)";

                            if ((item.Permissions.OwnerMask & PermissionMask.Transfer) == 0)
                                sub.Text += " (no transfer)";

                            sub.Text += " (" + item.InventoryType.ToString() + ")";
                            entry.ToolTipText = sub.Text;

                            lstContents.Items.Add(entry);
                        }
                    }
                }
            }
        }

        private void ctxContents_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;

            if (!(lstContents.Tag is Primitive))
            {
                e.Cancel = true;
                return;
            }

            ctxContents.Items.Clear();
            Primitive prim = (Primitive)lstContents.Tag;
            bool canModify = (prim.Flags & PrimFlags.ObjectModify) == PrimFlags.ObjectModify;

            if (lstContents.SelectedItems.Count == 1)
            {
                ListViewItem entry = lstContents.SelectedItems[0];

                if (entry.Tag is InventoryItem)
                {
                    InventoryItem item = (InventoryItem)entry.Tag;

                    if (canModify)
                    {
                        switch (item.InventoryType)
                        {
                            case InventoryType.Notecard:
                                ctxContents.Items.Add("Edit Notecard", null, (object sd, EventArgs ev) =>
                                    {
                                        InventoryNotecard inv = (InventoryNotecard)entry.Tag;
                                        new Notecard(instance, inv, prim) { Detached = true };
                                    }
                                );
                                break;

                            case InventoryType.LSL:
                                ctxContents.Items.Add("Edit Script", null, (object sd, EventArgs ev) =>
                                    {
                                        InventoryLSL inv = (InventoryLSL)entry.Tag;
                                        new ScriptEditor(instance, inv, prim) { Detached = true };
                                    }
                                );
                                break;
                        }

                    }
                }
            }

            if (lstContents.SelectedItems.Count > 0)
            {
                ctxContents.Items.Add("Delete", null, (object sd, EventArgs ev) =>
                {
                    foreach (ListViewItem i in lstContents.SelectedItems)
                        if (i.Tag is InventoryItem)
                            client.Inventory.RemoveTaskInventory(prim.LocalID, ((InventoryItem)i.Tag).UUID, client.Network.CurrentSim);
                });
            }

            if (ctxContents.Items.Count != 0)
            {
                ctxContents.Items.Add(new ToolStripSeparator());
            }

            if ((canModify || (prim.Flags & PrimFlags.AllowInventoryDrop) == PrimFlags.AllowInventoryDrop) && instance.InventoryClipboard != null)
            {
                if (instance.InventoryClipboard.Item is InventoryItem)
                {
                    ctxContents.Items.Add("Paste from Inventory", null, (object sd, EventArgs ev) =>
                    {
                        if (instance.InventoryClipboard.Item is InventoryLSL)
                        {
                            client.Inventory.CopyScriptToTask(prim.LocalID, (InventoryItem)instance.InventoryClipboard.Item, true);
                        }
                        else
                        {
                            client.Inventory.UpdateTaskInventory(prim.LocalID, (InventoryItem)instance.InventoryClipboard.Item);
                        }
                    });
                }
                else if (instance.InventoryClipboard.Item is InventoryFolder)
                {
                    ctxContents.Items.Add("Paste Folder Contents", null, (object sd, EventArgs ev) =>
                    {
                        foreach (InventoryBase oldItem in client.Inventory.Store.GetContents((InventoryFolder)instance.InventoryClipboard.Item))
                        {
                            if (oldItem is InventoryItem)
                            {
                                if (oldItem is InventoryLSL)
                                {
                                    client.Inventory.CopyScriptToTask(prim.LocalID, (InventoryItem)oldItem, true);
                                }
                                else
                                {
                                    client.Inventory.UpdateTaskInventory(prim.LocalID, (InventoryItem)oldItem);
                                }
                            }
                        }
                    });
                }
            }

            if (canModify)
            {
                ctxContents.Items.Add("Open (copy all to inventory)", null, OpenObject);
            }

            ctxContents.Items.Add("Close", null, btnCloseContents_Click);
        }

        void OpenObject(object sender, EventArgs e)
        {
            if (!(lstContents.Tag is Primitive)) return;

            Primitive prim = (Primitive)lstContents.Tag;
            if (prim.Properties == null) return;

            List<InventoryItem> items = new List<InventoryItem>();

            foreach (ListViewItem item in lstContents.Items)
            {
                if (item.Tag is InventoryItem)
                    items.Add(item.Tag as InventoryItem);
            }

            if (items.Count == 0) return;

            UUID folderID = client.Inventory.CreateFolder(client.Inventory.Store.RootFolder.UUID, prim.Properties.Name);

            for (int i = 0; i < items.Count; i++)
            {
                client.Inventory.MoveTaskInventory(prim.LocalID, items[i].UUID, folderID, client.Network.CurrentSim);
            }

            instance.TabConsole.DisplayNotificationInChat("Items from object contents copied to new inventory folder " + prim.Properties.Name);

        }

        void UpdateCurrentObject()
        {
            UpdateCurrentObject(true);
        }

        void UpdateCurrentObject(bool updateContents)
        {
            if (currentPrim.Properties == null) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { UpdateCurrentObject(updateContents); }));
                return;
            }

            // currentItem.Text = GetObjectName(currentPrim);

            txtObjectName.Text = currentPrim.Properties.Name;
            txtDescription.Text = currentPrim.Properties.Description;
            if ((currentPrim.Flags & PrimFlags.ObjectModify) == PrimFlags.ObjectModify)
            {
                txtObjectName.ReadOnly = txtDescription.ReadOnly = false;
                gbxObjectDetails.Text = "Object details (you can modify)";
            }
            else
            {
                txtObjectName.ReadOnly = txtDescription.ReadOnly = true;
                gbxObjectDetails.Text = "Object details";
            }

            txtHover.Text = currentPrim.Text;
            txtOwner.AgentID = currentPrim.Properties.OwnerID;
            txtCreator.AgentID = currentPrim.Properties.CreatorID;

            Permissions p = currentPrim.Properties.Permissions;
            cbOwnerModify.Checked = (p.OwnerMask & PermissionMask.Modify) != 0;
            cbOwnerCopy.Checked = (p.OwnerMask & PermissionMask.Copy) != 0;
            cbOwnerTransfer.Checked = (p.OwnerMask & PermissionMask.Transfer) != 0;

            cbNextOwnModify.CheckedChanged -= cbNextOwnerUpdate_CheckedChanged;
            cbNextOwnCopy.CheckedChanged -= cbNextOwnerUpdate_CheckedChanged;
            cbNextOwnTransfer.CheckedChanged -= cbNextOwnerUpdate_CheckedChanged;

            cbNextOwnModify.Checked = (p.NextOwnerMask & PermissionMask.Modify) != 0;
            cbNextOwnCopy.Checked = (p.NextOwnerMask & PermissionMask.Copy) != 0;
            cbNextOwnTransfer.Checked = (p.NextOwnerMask & PermissionMask.Transfer) != 0;

            cbNextOwnModify.CheckedChanged += cbNextOwnerUpdate_CheckedChanged;
            cbNextOwnCopy.CheckedChanged += cbNextOwnerUpdate_CheckedChanged;
            cbNextOwnTransfer.CheckedChanged += cbNextOwnerUpdate_CheckedChanged;

            if (currentPrim.Properties.OwnerID == client.Self.AgentID)
            {
                cbNextOwnModify.Enabled = cbNextOwnCopy.Enabled = cbNextOwnTransfer.Enabled = true;
            }
            else
            {
                cbNextOwnModify.Enabled = cbNextOwnCopy.Enabled = cbNextOwnTransfer.Enabled = false;
            }

            txtPrims.Text = (client.Network.CurrentSim.ObjectsPrimitives.FindAll(
                delegate(Primitive prim)
                {
                    return prim.ParentID == currentPrim.LocalID || prim.LocalID == currentPrim.LocalID;
                })).Count.ToString();

            if ((currentPrim.Flags & PrimFlags.Money) != 0)
            {
                btnPay.Enabled = true;
            }
            else
            {
                btnPay.Enabled = false;
            }

            if (currentPrim.Properties.SaleType != SaleType.Not)
            {
                btnBuy.Text = string.Format("Buy $L{0}", currentPrim.Properties.SalePrice);
                btnBuy.Enabled = true;
            }
            else
            {
                btnBuy.Text = "Buy";
                btnBuy.Enabled = false;
            }

            if (gbxContents.Visible /*&& updateContents*/)
            {
                UpdateObjectContents();
            }

            UpdateMuteButton();
        }

        void Netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Netcom_ClientDisconnected(sender, e)));
                }
                return;
            }

            if (instance.TabConsole.TabExists("objects"))
            {
                instance.TabConsole.Tabs["objects"].Close();
            }
        }

        private string GetObjectName(Primitive prim, int distance)
        {
            string name = "Loading...";
            string ownerName = "Loading...";

            if (prim.Properties != null)
            {
                name = prim.Properties.Name;
                // prim.Properties.GroupID is the actual group when group owned, not prim.GroupID
                if (UUID.Zero == prim.Properties.OwnerID &&
                    PrimFlags.ObjectGroupOwned == (prim.Flags & PrimFlags.ObjectGroupOwned) &&
                    UUID.Zero != prim.Properties.GroupID)
                {
                    System.Threading.AutoResetEvent nameReceivedSignal = new System.Threading.AutoResetEvent(false);
                    EventHandler<GroupNamesEventArgs> cbGroupName = new EventHandler<GroupNamesEventArgs>(
                        delegate(object sender, GroupNamesEventArgs e)
                        {
                            if (e.GroupNames.ContainsKey(prim.Properties.GroupID))
                            {
                                e.GroupNames.TryGetValue(prim.Properties.GroupID, out ownerName);
                                if (string.IsNullOrEmpty(ownerName))
                                    ownerName = "Loading...";
                                if (null != nameReceivedSignal)
                                    nameReceivedSignal.Set();
                            }
                        });
                    client.Groups.GroupNamesReply += cbGroupName;
                    client.Groups.RequestGroupName(prim.Properties.GroupID);
                    nameReceivedSignal.WaitOne(5000, false);
                    nameReceivedSignal.Close();
                    client.Groups.GroupNamesReply -= cbGroupName;
                }
                else
                    ownerName = instance.Names.Get(prim.Properties.OwnerID);
            }

            if (prim.ParentID == client.Self.LocalID)
            {
                return string.Format("{0} attached to {1}", name, prim.PrimData.AttachmentPoint.ToString());
            }
            else if (ownerName != "Loading...")
            {
                return String.Format("{0} ({1}m) owned by {2}", name, distance, ownerName);
            }
            else
            {
                return String.Format("{0} ({1}m)", name, distance);
            }

        }

        private string GetObjectName(Primitive prim)
        {
            int distance = (int)Vector3.Distance(client.Self.SimPosition, prim.Position);
            if (prim.ParentID == client.Self.LocalID) distance = 0;
            return GetObjectName(prim, distance);
        }

        private void AddPrim(Primitive prim)
        {
            lock (Prims)
            {
                string name = GetObjectName(prim);
                if (!Prims.Contains(prim) && (txtSearch.Text.Length == 0 || name.ToLower().Contains(txtSearch.Text.ToLower())))
                {
                    Prims.Add(prim);
                    if (prim.Properties == null)
                    {
                        propRequester.RequestProps(prim);
                    }
                }
            }
        }

        void Objects_ObjectUpdate(object sender, PrimEventArgs e)
        {
            if (e.Simulator.Handle != client.Network.CurrentSim.Handle || e.Prim.Position == Vector3.Zero || e.Prim is Avatar) return;

            if (IncludePrim(e.Prim))
            {
                if (e.Prim.ParentID == 0)
                {
                    int distance = (int)Vector3.Distance(client.Self.SimPosition, e.Prim.Position);
                    if (distance < searchRadius)
                    {
                        AddPrim(e.Prim);
                    }
                }
                else if (e.Prim.ParentID == client.Self.LocalID)
                {
                    AddPrim(e.Prim);
                }
            }

            if (e.Prim.ID == currentPrim.ID)
            {
                if (currentPrim.Properties != null)
                {
                    UpdateCurrentObject(false);
                }
            }
        }

        void Objects_KillObjects(object sender, KillObjectsEventArgs e)
        {
            if (e.Simulator.Handle != client.Network.CurrentSim.Handle) return;

            lock (Prims)
            {
                List<Primitive> killed = Prims.FindAll((p) =>
                {
                    for (int i = 0; i < e.ObjectLocalIDs.Length; i++)
                    {
                        if (p.LocalID == e.ObjectLocalIDs[i])
                        {
                            return true;
                        }
                    }
                    return false;
                });

                foreach (Primitive prim in killed)
                {
                    Prims.Remove(prim);
                }
            }

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() =>
                {
                    lstPrims.VirtualListSize = Prims.Count;
                    lstPrims.Invalidate();
                }));
            }
            else
            {
                lstPrims.VirtualListSize = Prims.Count;
                lstPrims.Invalidate();
            }

        }

        private bool IncludePrim(Primitive prim)
        {
            if ((prim.ParentID == client.Self.LocalID) && (filter == ObjectConsoleFilter.Attached || filter == ObjectConsoleFilter.Both))
            {
                return true;
            }
            else if ((prim.ParentID == 0) && (filter == ObjectConsoleFilter.Rezzed || filter == ObjectConsoleFilter.Both))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddAllObjects()
        {
            Vector3 location = client.Self.SimPosition;

            lock (Prims)
            {
                /*
                var prims = client.Network.CurrentSim.ObjectsPrimitives.FindAll(prim =>
                {
                    return ((prim.ParentID == client.Self.LocalID) && (filter == ObjectConsoleFilter.Attached || filter == ObjectConsoleFilter.Both)) ||
                        ((prim.ParentID == 0) && (filter == ObjectConsoleFilter.Rezzed || filter == ObjectConsoleFilter.Both));
                });
                */
                client.Network.CurrentSim.ObjectsPrimitives.ForEach(prim =>
                {
                    int distance = (int)Vector3.Distance(prim.Position, location);
                    if (prim.ParentID == client.Self.LocalID)
                    {
                        distance = 0;
                    }
                    if (IncludePrim(prim) &&
                        (prim.Position != Vector3.Zero) &&
                        (distance < searchRadius) &&
                        (txtSearch.Text.Length == 0 || (prim.Properties != null && prim.Properties.Name.ToLower().Contains(txtSearch.Text.ToLower()))) && //root prims and attachments only
                        !Prims.Contains(prim))
                    {
                        Prims.Add(prim);
                        if (prim.Properties == null)
                        {
                            propRequester.RequestProps(prim);
                        }
                    }
                });
                Prims.Sort(PrimSorter);
                lstPrims.VirtualListSize = Prims.Count;
                lstPrims.Invalidate();
            }
        }

        private void btnPointAt_Click(object sender, EventArgs e)
        {
            if (btnPointAt.Text == "Point At")
            {
                instance.State.SetPointing(currentPrim, 3);
                btnPointAt.Text = "Unpoint";
            }
            else if (btnPointAt.Text == "Unpoint")
            {
                instance.State.UnSetPointing();
                btnPointAt.Text = "Point At";
            }
        }

        private void btnSitOn_Click(object sender, EventArgs e)
        {
            if (!instance.State.IsSitting)
            {
                instance.State.SetSitting(true, currentPrim.ID);
            }
            else
            {
                instance.State.SetSitting(false, currentPrim.ID);
            }
        }

        private void btnTouch_Click(object sender, EventArgs e)
        {
            client.Self.Touch(currentPrim.LocalID);
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            btnRefresh_Click(null, null);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            txtSearch.Select();
            btnRefresh_Click(null, null);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            instance.State.SetDefaultCamera();
            Cursor.Current = Cursors.WaitCursor;
            Prims.Clear();
            AddAllObjects();

            Cursor.Current = Cursors.Default;
        }

        private void lstPrims_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPrims.SelectedIndices.Count == 1)
            {
                lock (Prims)
                {
                    try
                    {
                        currentPrim = Prims[lstPrims.SelectedIndices[0]];
                    }
                    catch
                    {
                        gbxInworld.Enabled = false;
                        return;
                    }
                }

                gbxInworld.Enabled = true;
                btnBuy.Tag = currentPrim;

                if (currentPrim.Properties == null || currentPrim.OwnerID == UUID.Zero || (currentPrim.Properties != null && currentPrim.Properties.CreatorID == UUID.Zero))
                {
                    client.Objects.SelectObject(client.Network.CurrentSim, currentPrim.LocalID);
                }

                UpdateCurrentObject();
                UpdateChildren();
            }
            else
            {
                lstChildren.Visible = false;
                gbxInworld.Enabled = false;
            }
        }

        private void lstChildren_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstChildren.SelectedItems.Count == 1)
            {
                gbxInworld.Enabled = true;
                currentPrim = lstChildren.SelectedItems[0].Tag as Primitive;
                btnBuy.Tag = currentPrim;

                if (currentPrim.Properties == null || (currentPrim.Properties != null && currentPrim.Properties.CreatorID == UUID.Zero))
                {
                    client.Objects.SelectObject(client.Network.CurrentSim, currentPrim.LocalID);
                }

                UpdateCurrentObject();
            }
            else
            {
                gbxInworld.Enabled = false;
            }
        }

        void UpdateChildren()
        {
            if (currentPrim == null) return;
            var prims = client.Network.CurrentSim.ObjectsPrimitives.FindAll((Primitive p) => p.ParentID == currentPrim.LocalID);
            if (prims == null || prims.Count == 0) return;
            List<uint> toGetNames = new List<uint>();

            lstChildren.BeginUpdate();
            lock (lstChildren.Items)
            {
                lstChildren.Items.Clear();
                foreach (var prim in prims)
                {
                    var item = new ListViewItem();

                    if (prim.Properties != null)
                    {
                        item.Text = prim.Properties.Name;
                    }
                    else
                    {
                        item.Text = "Loading...";
                        toGetNames.Add(prim.LocalID);
                    }

                    item.Tag = prim;
                    item.Name = prim.ID.ToString();
                    lstChildren.Items.Add(item);
                }
            }
            lstChildren.EndUpdate();
            lstChildren.Visible = true;
            if (toGetNames.Count > 0)
            {
                client.Objects.SelectObjects(client.Network.CurrentSim, toGetNames.ToArray(), true);
            }
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            (new frmPay(instance, currentPrim.ID, currentPrim.Properties.Name, true)).ShowDialog();
        }

        private void btnDetach_Click(object sender, EventArgs e)
        {
            var toDetach = CurrentOutfitFolder.GetAttachmentItem(currentPrim);
            if (toDetach != UUID.Zero)
            {
                if (client.Inventory.Store.Contains(toDetach))
                {
                    instance.COF.Detach(client.Inventory.Store[toDetach] as InventoryItem);
                }
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (currentPrim.PrimData.PCode != PCode.Prim)
            {
                instance.TabConsole.DisplayNotificationInChat("Cannot display objects of that type", ChatBufferTextStyle.Error);
                return;
            }

            Rendering.frmPrimWorkshop pw = new Rendering.frmPrimWorkshop(instance, currentPrim.LocalID);
            pw.Show();
        }

        private void nudRadius_ValueChanged(object sender, EventArgs e)
        {
            searchRadius = (float)nudRadius.Value;
            btnRefresh_Click(null, null);
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            if (lstPrims.SelectedIndices.Count != 1) return;
            btnBuy.Enabled = false;
            client.Objects.BuyObject(client.Network.CurrentSim, currentPrim.LocalID, currentPrim.Properties.SaleType, currentPrim.Properties.SalePrice, client.Self.ActiveGroup, client.Inventory.FindFolderForType(AssetType.Object));
        }

        private void rbDistance_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDistance.Checked)
            {
                PrimSorter.SortByName = false;
                lock (Prims) Prims.Sort(PrimSorter);
                lstPrims.Invalidate();
            }
        }

        private void rbName_CheckedChanged(object sender, EventArgs e)
        {
            if (rbName.Checked)
            {
                PrimSorter.SortByName = true;
                lock (Prims) Prims.Sort(PrimSorter);
                lstPrims.Invalidate();
            }
        }

        private void btnTurnTo_Click(object sender, EventArgs e)
        {
            if (lstPrims.SelectedIndices.Count != 1) return;
            client.Self.Movement.TurnToward(currentPrim.Position);
        }

        private void btnWalkTo_Click(object sender, EventArgs e)
        {
            if (lstPrims.SelectedIndices.Count != 1) return;

            if (instance.State.IsWalking)
            {
                instance.State.EndWalking();
            }
            else
            {
                instance.State.WalkTo(currentPrim);
            }
        }

        void State_OnWalkStateCanged(bool walking)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { State_OnWalkStateCanged(walking); }));
                return;
            }

            if (walking)
            {
                btnWalkTo.Text = "Stop";
            }
            else
            {
                btnWalkTo.Text = "Walk to";
                btnRefresh_Click(null, null);
            }
        }

        private void nudRadius_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void nudRadius_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
            }
        }

        private void ctxMenuObjects_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;

            if (lstPrims.SelectedIndices.Count != 1)
            {
                e.Cancel = true;
                return;
            }

            ctxMenuObjects.Items.Clear();
            ctxMenuObjects.Items.Add("Click/Touch", null, btnTouch_Click);

            if (currentPrim.ParentID == client.Self.LocalID)
                ctxMenuObjects.Items.Add("Detach", null, btnDetach_Click);

            if ((currentPrim.Flags & PrimFlags.Money) != 0)
                ctxMenuObjects.Items.Add("Pay", null, btnPay_Click);

            if (currentPrim.Properties != null && currentPrim.Properties.SaleType != SaleType.Not)
                ctxMenuObjects.Items.Add(string.Format("Buy for ${0}", currentPrim.Properties.SalePrice), null, btnBuy_Click);

            if (gbxInworld.Visible)
                ctxMenuObjects.Items.Add("Show Contents", null, btnContents_Click);
            else
                ctxMenuObjects.Items.Add("Hide Contents", null, btnCloseContents_Click);

            ctxMenuObjects.Items.Add(this.instance.State.IsSitting ? "Stand Up" : "Sit On", null, btnSitOn_Click);
            ctxMenuObjects.Items.Add("Turn To", null, btnTurnTo_Click);
            ctxMenuObjects.Items.Add("Walk To", null, btnWalkTo_Click);
            ctxMenuObjects.Items.Add(this.instance.State.IsPointing ? "Unpoint" : "Point At", null, btnPointAt_Click);
            ctxMenuObjects.Items.Add("3D View", null, btnView_Click);
            ctxMenuObjects.Items.Add("Take", null, btnTake_Click);
            ctxMenuObjects.Items.Add("Delete", null, btnDelete_Click);
            ctxMenuObjects.Items.Add("Return", null, btnReturn_Click);

            if (currentPrim.Properties != null)
            {
                if (currentPrim.Properties.CreatorID == client.Self.AgentID &&
                    currentPrim.Properties.OwnerID == client.Self.AgentID)
                {
                    ctxMenuObjects.Items.Add("Export", null, btnExport_Click);
                }
            }

            if (currentPrim.Properties != null)
            {
                bool isMuted = null != client.Self.MuteList.Find(me => me.Type == MuteType.Object && me.ID == currentPrim.ID);

                if (isMuted)
                {
                    ctxMenuObjects.Items.Add("Unmute", null, (a, b) =>
                        {
                            client.Self.RemoveMuteListEntry(currentPrim.ID, currentPrim.Properties.Name);
                        });
                }
                else
                {
                    ctxMenuObjects.Items.Add("Mute", null, (a, b) =>
                    {
                        client.Self.UpdateMuteListEntry(MuteType.Object, currentPrim.ID, currentPrim.Properties.Name);
                    });
                }
            }

            //if (currentPrim.MediaURL != null && currentPrim.MediaURL.StartsWith("x-mv:"))
            //{
            //    ctxMenuObjects.Items.Add("Test", null, (object menuSender, EventArgs menuE) =>
            //        {
            //            client.Objects.RequestObjectMedia(currentPrim.ID, client.Network.CurrentSim, (bool success, string version, MediaEntry[] faceMedia) =>
            //                {
            //                    int foo = 1;
            //                }
            //            );
            //        }
            //    );
            //}

            instance.ContextActionManager.AddContributions(ctxMenuObjects, currentPrim);
        }

        public RadegastContextMenuStrip GetContextMenu()
        {
            return ctxMenuObjects;
        }

        private void btnTake_Click(object sender, EventArgs e)
        {
            instance.MediaManager.PlayUISound(UISounds.ObjectDelete);
            client.Inventory.RequestDeRezToInventory(currentPrim.LocalID);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (currentPrim.Properties != null && currentPrim.Properties.OwnerID != client.Self.AgentID)
                btnReturn_Click(sender, e);
            else
            {
                instance.MediaManager.PlayUISound(UISounds.ObjectDelete);
                client.Inventory.RequestDeRezToInventory(currentPrim.LocalID, DeRezDestination.AgentInventoryTake, client.Inventory.FindFolderForType(FolderType.Trash), UUID.Random());
            }
        }

        private void btnReturn_Click(object sender, EventArgs e)
        {
            instance.MediaManager.PlayUISound(UISounds.ObjectDelete);
            client.Inventory.RequestDeRezToInventory(currentPrim.LocalID, DeRezDestination.ReturnToOwner, UUID.Zero, UUID.Random());
        }

        private void btnCloseContents_Click(object sender, EventArgs e)
        {
            gbxContents.Hide();
            gbxInworld.Show();
            lstPrims.Focus();
        }

        private void btnContents_Click(object sender, EventArgs e)
        {
            gbxInworld.Hide();
            gbxContents.Show();
            UpdateObjectContents();
            lstContents.Focus();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            instance.MainForm.DisplayExportConsole(currentPrim.LocalID);
        }

        private void lstContents_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstContents.SelectedItems.Count != 1) return;

            ListViewItem contentItem = lstContents.SelectedItems[0];

            if (contentItem.Tag is InventoryLSL)
            {
                InventoryLSL inv = (InventoryLSL)contentItem.Tag;
                Primitive prim = (Primitive)lstContents.Tag;
                new ScriptEditor(instance, inv, prim) { Detached = true };
            }
            else if (contentItem.Tag is InventoryNotecard)
            {
                InventoryNotecard inv = (InventoryNotecard)contentItem.Tag;
                Primitive prim = (Primitive)lstContents.Tag;
                new Notecard(instance, inv, prim) { Detached = true };
            }

        }

        private void lstContents_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                lstContents_MouseDoubleClick(null, null);

                e.SuppressKeyPress = e.Handled = true;
                return;
            }
            //else if (e.KeyCode == Keys.Apps)
            //{
            //    Point pos = new Point(50, 30);

            //    if (lstContents.SelectedItems.Count > 0)
            //    {
            //        pos = lstContents.SelectedItems[0].Position;
            //        pos.Y += 10;
            //        pos.X += 120;
            //    }

            //    ctxContents.Show(lstContents, pos);

            //    e.SuppressKeyPress = e.Handled = true;
            //    return;
            //}
        }

        void Self_MuteListUpdated(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => Self_MuteListUpdated(sender, e)));
                }
                return;
            }

            if (lstPrims.SelectedIndices.Count != 1) return;

            UpdateMuteButton();
        }

        void UpdateMuteButton()
        {
            bool isMuted = null != client.Self.MuteList.Find(me => me.Type == MuteType.Object && me.ID == currentPrim.ID);

            if (isMuted)
            {
                btnMute.Text = "Unmute";
            }
            else
            {
                btnMute.Text = "Mute";
            }
        }

        private void btnMute_Click(object sender, EventArgs e)
        {
            if (lstPrims.SelectedIndices.Count != 1) return;

            if (currentPrim.Properties == null) return;

            if (btnMute.Text == "Mute")
            {
                client.Self.UpdateMuteListEntry(MuteType.Object, currentPrim.ID, currentPrim.Properties.Name);
            }
            else
            {
                client.Self.RemoveMuteListEntry(currentPrim.ID, currentPrim.Properties.Name);
            }
        }

        private void txtObjectName_Leave(object sender, EventArgs e)
        {
            if (currentPrim == null) return;
            if (currentPrim.Properties == null || (currentPrim.Properties != null && currentPrim.Properties.Name != txtObjectName.Text))
            {
                client.Objects.SetName(client.Network.CurrentSim, currentPrim.LocalID, txtObjectName.Text);
            }
        }

        private void txtDescription_Leave(object sender, EventArgs e)
        {
            if (currentPrim == null) return;
            if (currentPrim.Properties == null || (currentPrim.Properties != null && currentPrim.Properties.Description != txtDescription.Text))
            {
                client.Objects.SetDescription(client.Network.CurrentSim, currentPrim.LocalID, txtDescription.Text);
            }
        }

        private void cbNextOwnerUpdate_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            PermissionMask pm = PermissionMask.None;

            if (cb == cbNextOwnCopy) pm = PermissionMask.Copy;
            if (cb == cbNextOwnModify) pm = PermissionMask.Modify;
            if (cb == cbNextOwnTransfer) pm = PermissionMask.Transfer;

            if (pm == PermissionMask.None) return;

            client.Objects.SetPermissions(client.Network.CurrentSim, new List<uint>() { currentPrim.LocalID }, PermissionWho.NextOwner, pm, cb.Checked);
        }

        private void lstPrims_Enter(object sender, EventArgs e)
        {
            lstPrims_SelectedIndexChanged(sender, EventArgs.Empty);
        }

        private void lstChildren_Enter(object sender, EventArgs e)
        {
            lstChildren_SelectedIndexChanged(sender, EventArgs.Empty);
        }

        private void lstPrims_DoubleClick(object sender, EventArgs e)
        {
            btnView.PerformClick();
        }

        private void lstPrims_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            Primitive prim = null;
            try
            {
                lock (Prims)
                {
                    prim = Prims[e.ItemIndex];
                }
            }
            catch
            {
                e.Item = new ListViewItem();
                return;
            }

            string name = GetObjectName(prim);
            var item = new ListViewItem(name);
            item.Tag = prim;
            item.Name = prim.ID.ToString();
            e.Item = item;
        }

    }

    public enum ObjectConsoleFilter : int
    {
        Rezzed = 0,
        Attached = 1,
        Both = 2
    }

    public class ObjectSorter : IComparer<Primitive>
    {
        private AgentManager me;
        private bool sortByName = false;

        public bool SortByName { get { return sortByName; } set { sortByName = value; } }

        public ObjectSorter(AgentManager me)
        {
            this.me = me;
        }

        private int NameCompare(Primitive prim1, Primitive prim2)
        {
            if (prim1.Properties == null && prim2.Properties == null)
            {
                return 0;
            }
            else if (prim1.Properties != null && prim2.Properties == null)
            {
                return -1;
            }
            else if (prim1.Properties == null && prim2.Properties != null)
            {
                return 1;
            }

            return string.Compare(prim1.Properties.Name, prim2.Properties.Name);
        }
        //this routine should return -1 if xy and 0 if x==y.
        // for our sample we'll just use string comparison
        public int Compare(Primitive prim1, Primitive prim2)
        {
            if (sortByName)
            {
                return NameCompare(prim1, prim2);
            }

            float dist1 = prim1.ParentID == me.LocalID ? 0 : Vector3.Distance(me.SimPosition, prim1.Position);
            float dist2 = prim2.ParentID == me.LocalID ? 0 : Vector3.Distance(me.SimPosition, prim2.Position);

            if (dist1 == dist2)
            {
                return NameCompare(prim1, prim2);
            }
            else
            {
                if (dist1 < dist2)
                {
                    return -1;
                }
                return 1;
            }
        }
    }

    public class PropertiesQueue : IDisposable
    {
        Object sync = new Object();
        RadegastInstance instance;
        System.Timers.Timer qTimer;
        Queue<Primitive> props = new Queue<Primitive>();

        public delegate void TickCallback(int remaining);
        public event TickCallback OnTick;

        public PropertiesQueue(RadegastInstance instance)
        {
            this.instance = instance;
            qTimer = new System.Timers.Timer(2500);
            qTimer.Enabled = true;
            qTimer.Elapsed += new ElapsedEventHandler(qTimer_Elapsed);
        }

        public void RequestProps(Primitive prim)
        {
            lock (sync)
            {
                if (!props.Contains(prim))
                {
                    props.Enqueue(prim);
                }
            }
        }

        void qTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (sync)
            {
                for (int i = 0; i < 25 && props.Count > 0; i++)
                {
                    Primitive prim = props.Dequeue();
                    if (prim.ParentID == 0)
                    {
                        instance.Client.Objects.RequestObjectPropertiesFamily(
                            instance.Client.Network.CurrentSim, prim.ID, true);
                    }
                    else
                    {
                        instance.Client.Objects.SelectObject(instance.Client.Network.CurrentSim,
                            prim.LocalID, true);
                    }
                }

                if (OnTick != null)
                {
                    OnTick(props.Count);
                }
            }
        }

        public void Dispose()
        {
            if (qTimer != null)
            {
                qTimer.Elapsed -= new ElapsedEventHandler(qTimer_Elapsed);
                qTimer.Enabled = false;
                qTimer = null;
            }
            props = null;
            instance = null;
        }
    }

}