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
// $Id$
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class ObjectsConsole : UserControl, IContextMenuProvider
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client;} }
        private Primitive currentPrim = new Primitive();
        private ListViewItem currentItem = new ListViewItem();
        private float searchRadius = 40.0f;
        PropertiesQueue propRequester;

        public ObjectsConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmObjects_Disposed);

            this.instance = instance;

            propRequester = new PropertiesQueue(instance);
            propRequester.OnTick += new PropertiesQueue.TickCallback(propRequester_OnTick);

            btnPointAt.Text = (this.instance.State.IsPointing ? "Unpoint" : "Point At");
            btnSitOn.Text = (this.instance.State.IsSitting ? "Stand Up" : "Sit On");
            
            nudRadius.Value = (decimal)searchRadius;
            nudRadius.ValueChanged += nudRadius_ValueChanged;

            lstPrims.ListViewItemSorter = new ObjectSorter(client.Self);

            if (instance.MonoRuntime)
            {
                btnView.Visible = false;
            }

            // Callbacks
            client.Network.OnDisconnected += new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Objects.OnNewPrim += new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
            client.Objects.OnObjectKilled += new ObjectManager.KillObjectCallback(Objects_OnObjectKilled);
            client.Objects.OnObjectProperties += new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);
            client.Network.OnCurrentSimChanged += new NetworkManager.CurrentSimChangedCallback(Network_OnCurrentSimChanged);
            client.Avatars.UUIDNameReply += new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
            instance.State.OnWalkStateCanged += new StateManager.WalkStateCanged(State_OnWalkStateCanged);
        }

        void frmObjects_Disposed(object sender, EventArgs e)
        {
            propRequester.Dispose();
            client.Network.OnDisconnected -= new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Objects.OnNewPrim -= new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
            client.Objects.OnObjectKilled -= new ObjectManager.KillObjectCallback(Objects_OnObjectKilled);
            client.Objects.OnObjectProperties -= new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);
            client.Network.OnCurrentSimChanged -= new NetworkManager.CurrentSimChangedCallback(Network_OnCurrentSimChanged);
            client.Avatars.UUIDNameReply -= new EventHandler<UUIDNameReplyEventArgs>(Avatars_UUIDNameReply);
            instance.State.OnWalkStateCanged -= new StateManager.WalkStateCanged(State_OnWalkStateCanged);
        }

        public void RefreshObjectList()
        {
            btnRefresh_Click(this, EventArgs.Empty);
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
            sb.AppendFormat("Tracking {0} objects", lstPrims.Items.Count);

            if (remaining > 10)
            {
                sb.AppendFormat(", fetching {0} object names.", remaining);
            }
            else
            {
                sb.Append(".");
            }

            lblStatus.Text = sb.ToString();
        }

        void Network_OnCurrentSimChanged(Simulator PreviousSimulator)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Network_OnCurrentSimChanged(PreviousSimulator);
                }
                ));
                return;
            }

            btnRefresh_Click(null, null);
        }

        void Avatars_UUIDNameReply(object sender, UUIDNameReplyEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { Avatars_UUIDNameReply(sender, e); }));
                return;
            }

            lstPrims.BeginUpdate();
            lock (lstPrims.Items)
            {
                foreach (ListViewItem item in lstPrims.Items)
                {
                    Primitive prim = item.Tag as Primitive;
                    if (prim.Properties != null && e.Names.ContainsKey(prim.Properties.OwnerID))
                    {
                        item.Text = GetObjectName(prim);
                    }
                }
            }
            lstPrims.EndUpdate();
        }

        void Objects_OnObjectProperties(Simulator simulator, Primitive.ObjectProperties props)
        {
            if (simulator.Handle != client.Network.CurrentSim.Handle)
            {
                return;
            }

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { Objects_OnObjectProperties(simulator, props); }));
                return;
            }

            lock (lstPrims.Items)
            {
                if (lstPrims.Items.ContainsKey(props.ObjectID.ToString()))
                {
                    Primitive prim = lstPrims.Items[props.ObjectID.ToString()].Tag as Primitive;
                    prim.Properties = props;
                    lstPrims.Items[props.ObjectID.ToString()].Text = GetObjectName(prim);
                }
            }

            if (props.ObjectID == currentPrim.ID)
            {
                UpdateCurrentObject();
            }
        }
        

        void UpdateCurrentObject()
        {
            if (currentPrim.Properties == null) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { UpdateCurrentObject(); }));
                return;
            }

            currentItem.Text = GetObjectName(currentPrim);

            txtObjectName.Text = currentPrim.Properties.Name;
            txtDescription.Text = currentPrim.Properties.Description;
            txtHover.Text = currentPrim.Text;
            txtOwner.AgentID = currentPrim.Properties.OwnerID;
            txtCreator.AgentID = currentPrim.Properties.CreatorID;

            Permissions p = currentPrim.Properties.Permissions;
            cbOwnerModify.Checked = (p.OwnerMask & PermissionMask.Modify) != 0;
            cbOwnerCopy.Checked = (p.OwnerMask & PermissionMask.Copy) != 0;
            cbOwnerTransfer.Checked = (p.OwnerMask & PermissionMask.Transfer) != 0;
            cbNextOwnModify.Checked = (p.NextOwnerMask & PermissionMask.Modify) != 0;
            cbNextOwnCopy.Checked = (p.NextOwnerMask & PermissionMask.Copy) != 0;
            cbNextOwnTransfer.Checked = (p.NextOwnerMask & PermissionMask.Transfer) != 0;

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
        }

        private void Network_OnDisconnected(NetworkManager.DisconnectType reason, string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        Network_OnDisconnected(reason, message);
                    }
                ));
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

            if (prim.Properties == null)
            {
                if (prim.ParentID != 0) throw new Exception("Requested properties for non root prim");
                propRequester.RequestProps(prim.LocalID);
            }
            else
            {
                name = prim.Properties.Name;
                ownerName = instance.getAvatarName(prim.Properties.OwnerID);
            }
            return String.Format("{0} ({1}m) owned by {2}", name, distance, ownerName);

        }

        private string GetObjectName(Primitive prim)
        {
            int distance = (int)Vector3.Distance(client.Self.SimPosition, prim.Position);
            return GetObjectName(prim, distance);
        }

        private void AddPrim(Primitive prim)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        AddPrim(prim);
                    }
                ));
                return;
            }

            ListViewItem item = null;

            lock (lstPrims.Items)
            {
                if (lstPrims.Items.ContainsKey(prim.ID.ToString()))
                {
                    item = lstPrims.Items[prim.ID.ToString()];
                }
            }

            if (item == null)
            {
                item = new ListViewItem();
                item.Text = GetObjectName(prim);
                item.Tag = prim;
                item.Name = prim.ID.ToString();

                if (txtSearch.Text.Length == 0 || item.Text.ToLower().Contains(txtSearch.Text.ToLower()))
                {
                    lock (lstPrims.Items)
                    {
                        lstPrims.Items.Add(item);
                    }
                }
            }
            else
            {
                item.Text = GetObjectName(prim);
            }
        }

        private void Objects_OnNewPrim(Simulator simulator, Primitive prim, ulong regionHandle, ushort timeDilation)
        {
            if (regionHandle != client.Network.CurrentSim.Handle || prim.Position == Vector3.Zero || prim is Avatar) return;

            if (prim.ParentID == 0)
            {
                int distance = (int)Vector3.Distance(client.Self.SimPosition, prim.Position);
                if (distance < searchRadius)
                {
                    if (prim.Properties == null)
                    {
                        propRequester.RequestProps(prim.LocalID);
                    }
                    AddPrim(prim);
                }
            }

            if (prim.ID == currentPrim.ID)
            {
                if (currentPrim.Properties != null)
                {
                    UpdateCurrentObject();
                }
                propRequester.RequestProps(prim.LocalID);
            }
        }

        private void Objects_OnObjectKilled(Simulator simulator, uint objectID)
        {
            if (simulator.Handle != client.Network.CurrentSim.Handle) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate() { Objects_OnObjectKilled(simulator, objectID); }));
                return;
            }

            Primitive prim;

            if (client.Network.CurrentSim.ObjectsPrimitives.TryGetValue(objectID, out prim))
            {
                lock (lstPrims.Items)
                {
                    if (lstPrims.Items.ContainsKey(prim.ID.ToString()))
                    {
                        lstPrims.Items[prim.ID.ToString()].Remove();
                    }
                }
            }
        }

        private void AddAllObjects()
        {
            Vector3 location = client.Self.SimPosition;
            List<ListViewItem> items = new List<ListViewItem>();

            client.Network.CurrentSim.ObjectsPrimitives.ForEach(
                new Action<Primitive>(
                delegate(Primitive prim)
                {
                    int distance = (int)Vector3.Distance(prim.Position, location);
                    if (prim.ParentID == 0 && (prim.Position != Vector3.Zero) && (distance < searchRadius) && (txtSearch.Text.Length == 0 || (prim.Properties != null && prim.Properties.Name.ToLower().Contains(txtSearch.Text.ToLower())))) //root prims only
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = GetObjectName(prim);
                        item.Tag = prim;
                        item.Name = prim.ID.ToString();
                        items.Add(item);
                    }
                }
                ));

            lock (lstPrims.Items)
            {
                lstPrims.Items.AddRange(items.ToArray());
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

        private void btnSource_Click(object sender, EventArgs e)
        {
            instance.State.EffectSource = currentPrim.ID;
        }


        private void btnSitOn_Click(object sender, EventArgs e)
        {
            if (btnSitOn.Text == "Sit On")
            {
                instance.State.SetSitting(true, currentPrim.ID);
                btnSitOn.Text = "Stand Up";
            }
            else if (btnSitOn.Text == "Stand Up")
            {
                instance.State.SetSitting(false, currentPrim.ID);
                btnSitOn.Text = "Sit On";
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
            lstPrims.BeginUpdate();
            Cursor.Current = Cursors.WaitCursor;
            lock (lstPrims.Items)
            {
                lstPrims.Items.Clear();
            }
            AddAllObjects();
            Cursor.Current = Cursors.Default;
            lstPrims.EndUpdate();
        }

        private void lstPrims_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPrims.SelectedItems.Count == 1)
            {
                gbxInworld.Enabled = true;
                currentItem = lstPrims.SelectedItems[0];
                currentPrim = currentItem.Tag as Primitive;
                btnBuy.Tag = currentPrim;

                if (currentPrim.Properties == null)
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

        private void btnPay_Click(object sender, EventArgs e)
        {
            (new frmPay(instance, currentPrim.ID, currentPrim.Properties.Name, true)).ShowDialog();
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            List<Primitive> prims = new List<Primitive>();

            client.Network.CurrentSim.ObjectsPrimitives.ForEach(delegate(KeyValuePair<uint, Primitive> kvp)
            {
                if (kvp.Key == currentPrim.LocalID || kvp.Value.ParentID == currentPrim.LocalID)
                {
                    prims.Add(kvp.Value);
                }
            });

            frmPrimWorkshop pw = new frmPrimWorkshop(instance);
            pw.loadPrims(prims);
            pw.Show();
        }

        private void nudRadius_ValueChanged(object sender, EventArgs e)
        {
            searchRadius = (float)nudRadius.Value;
            btnRefresh_Click(null, null);
        }

        private void btnBuy_Click(object sender, EventArgs e)
        {
            if (lstPrims.SelectedItems.Count != 1) return;
            btnBuy.Enabled = false;
            client.Objects.BuyObject(client.Network.CurrentSim, currentPrim.LocalID, currentPrim.Properties.SaleType, currentPrim.Properties.SalePrice, client.Self.ActiveGroup, client.Inventory.FindFolderForType(AssetType.Object));
        }

        private void rbDistance_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDistance.Checked)
            {
                lstPrims.BeginUpdate();
                ((ObjectSorter)lstPrims.ListViewItemSorter).SortByName = false;
                lstPrims.Sort();
                lstPrims.EndUpdate();
            }
        }

        private void rbName_CheckedChanged(object sender, EventArgs e)
        {
            if (rbName.Checked)
            {
                lstPrims.BeginUpdate();
                ((ObjectSorter)lstPrims.ListViewItemSorter).SortByName = true;
                lstPrims.Sort();
                lstPrims.EndUpdate();
            }
        }

        private void btnTurnTo_Click(object sender, EventArgs e)
        {
            if (lstPrims.SelectedItems.Count != 1) return;
            client.Self.Movement.TurnToward(currentPrim.Position);
        }

        private void btnWalkTo_Click(object sender, EventArgs e)
        {
            if (lstPrims.SelectedItems.Count != 1) return;

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
        private void lstPrims_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListView box = (ListView)sender;
                if (box.SelectedItems.Count > 0)
                {
                    ctxMenuObjects.Selection = box.SelectedItems[0];
                    ctxMenuObjects.HasSelection = true;
                    ctxMenuObjects.Show(lstPrims, new System.Drawing.Point(e.X, e.Y));
                } else
                {
                    ctxMenuObjects.Selection = null;
                    ctxMenuObjects.HasSelection = false;                    
                }
            }
        }
        private void ctxMenuObjects_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (lstPrims.SelectedItems.Count == 0)
            {
                ctxMenuObjects.Selection = null;
                ctxMenuObjects.HasSelection = false;
                e.Cancel = true;
                return;
            }
            instance.ContextActionManager.AddContributions(ctxMenuObjects, typeof(Primitive), lstPrims.SelectedItems[0].Tag, btnWalkTo.Parent);
        }

        public RadegastContextMenuStrip GetContextMenu()
        {
            return ctxMenuObjects;
        }

        private void lstPrims_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Apps)
            {
                if (lstPrims.SelectedItems.Count > 0)
                {
                    e.SuppressKeyPress = true;
                    ctxMenuObjects.Selection = lstPrims.SelectedItems[0];
                    ctxMenuObjects.HasSelection = true;

                    ctxMenuObjects.Show(lstPrims, new System.Drawing.Point(0, 0));
                }
            }
        }
    }

    public class ObjectSorter : IComparer
    {
        private AgentManager me;
        private bool sortByName = false;

        public bool SortByName { get { return sortByName; } set { sortByName = value; } }

        public ObjectSorter(AgentManager me)
        {
            this.me = me;
        }


        //this routine should return -1 if xy and 0 if x==y.
        // for our sample we'll just use string comparison
        public int Compare(object x, object y)
        {
            ListViewItem item1 = (ListViewItem)x;
            ListViewItem item2 = (ListViewItem)y;

            if (sortByName)
            {
                return string.Compare(item1.Text, item2.Text);
            }

            float dist1 = Vector3.Distance(me.SimPosition, ((Primitive)item1.Tag).Position);
            float dist2 = Vector3.Distance(me.SimPosition, ((Primitive)item2.Tag).Position);

            if (dist1 == dist2)
            {
                return String.Compare(item1.Text, item2.Text);
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
        Queue<uint> props = new Queue<uint>();
        List<uint> prims = new List<uint>();

        public delegate void TickCallback(int remaining);
        public event TickCallback OnTick;

        public PropertiesQueue(RadegastInstance instance)
        {
            this.instance = instance;
            qTimer = new System.Timers.Timer(1500);
            qTimer.Enabled = true;
            qTimer.Elapsed += new ElapsedEventHandler(qTimer_Elapsed);
        }

        public void RequestProps(uint localID)
        {
            lock (sync)
            {
                if (!props.Contains(localID))
                {
                    props.Enqueue(localID);
                }
            }
        }

        void qTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (sync)
            {
                if (prims.Count > 0)
                {
                    instance.Client.Objects.DeselectObjects(instance.Client.Network.CurrentSim, prims.ToArray());
                    prims.Clear();
                }

                for (int i = 0; i < 50 && props.Count > 0; i++)
                {
                    prims.Add(props.Dequeue());
                }

                if (prims.Count > 0)
                {
                    instance.Client.Objects.SelectObjects(instance.Client.Network.CurrentSim, prims.ToArray(), false);
                    prims.Clear();
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