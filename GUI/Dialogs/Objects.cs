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
    public partial class frmObjects : Form
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client;} }
        private Primitive currentPrim;
        private float searchRadius = 40.0f;
        PropertiesQueue propRequester;

        public frmObjects(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmObjects_Disposed);

            this.instance = instance;
            
            btnPointAt.Text = (this.instance.State.IsPointing ? "Unpoint" : "Point At");
            btnSitOn.Text = (this.instance.State.IsSitting ? "Stand Up" : "Sit On");
            nudRadius.Value = (decimal)searchRadius;

            propRequester = new PropertiesQueue(instance);
            propRequester.OnTick += new PropertiesQueue.TickCallback(propRequester_OnTick);
            lstPrims.ListViewItemSorter = new ObjectSorter(client.Self);

            if (instance.MonoRuntime)
            {
                btnView.Visible = false;
            }

            // Callbacks
            client.Network.OnDisconnected += new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Objects.OnNewPrim += new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
            client.Objects.OnObjectKilled += new ObjectManager.KillObjectCallback(Objects_OnObjectKilled);
            client.Objects.OnObjectPropertiesFamily += new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
            client.Network.OnCurrentSimChanged += new NetworkManager.CurrentSimChangedCallback(Network_OnCurrentSimChanged);
            instance.OnAvatarName += new RadegastInstance.OnAvatarNameCallBack(instance_OnAvatarName);
        }

        void frmObjects_Disposed(object sender, EventArgs e)
        {
            propRequester.Dispose();
            client.Network.OnDisconnected -= new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Objects.OnNewPrim -= new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
            client.Objects.OnObjectKilled -= new ObjectManager.KillObjectCallback(Objects_OnObjectKilled);
            client.Objects.OnObjectPropertiesFamily -= new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
            client.Network.OnCurrentSimChanged -= new NetworkManager.CurrentSimChangedCallback(Network_OnCurrentSimChanged);

            instance.OnAvatarName -= new RadegastInstance.OnAvatarNameCallBack(instance_OnAvatarName);
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

        void instance_OnAvatarName(UUID agentID, string agentName)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    instance_OnAvatarName(agentID, agentName);
                }));
                return;
            }

            foreach (ListViewItem item in lstPrims.Items)
            {
                Primitive prim = item.Tag as Primitive;
                if (prim.Properties != null && prim.Properties.OwnerID == agentID)
                {
                    item.Text = GetObjectName(prim);
                }
            }
  
        }

        void Objects_OnObjectPropertiesFamily(Simulator simulator, Primitive.ObjectProperties props, ReportType type)
        {
            if (simulator.Handle != client.Network.CurrentSim.Handle) return;
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Objects_OnObjectPropertiesFamily(simulator, props, type);
                }));
                return;
            }
            foreach (ListViewItem item in lstPrims.Items)
            {
                Primitive prim = item.Tag as Primitive;
                if (prim.ID == props.ObjectID)
                {
                    prim.Properties = props;
                    item.Text = GetObjectName(prim);
                    break;
                }
            }

            #region Update buy button
            Primitive p = btnBuy.Tag as Primitive;
            if (p != null && p.ID == props.ObjectID)
            {
                if (props.SaleType != SaleType.Not)
                {
                    btnBuy.Text = string.Format("Buy $L{0}", props.SalePrice);
                    btnBuy.Enabled = true;
                }
                else
                {
                    btnBuy.Text = "Buy";
                    btnBuy.Enabled = false;
                }
            }
            #endregion

            #region Update selected prim text box
            p = txtCurrentPrim.Tag as Primitive;
            if (p != null && p.ID == props.ObjectID)
            {
                p.Properties = props;
                txtCurrentPrim.Text = GetObjectName(p);
            }
            #endregion

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
            this.Close();
        }

        private string GetObjectName(Primitive prim, int distance)
        {
            string name = "Loading...";
            string ownerName = "Loading...";

            if (prim.Properties == null)
            {
                if (prim.ParentID != 0) throw new Exception("Requested properties for non root prim");
                propRequester.RequestProps(prim.ID);
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

            foreach (ListViewItem sitem in lstPrims.Items)
            {
                if (((Primitive)sitem.Tag).LocalID == prim.LocalID)
                {
                    item = sitem;
                    break;
                }
            }

            if (item == null)
            {
                item = new ListViewItem(prim.LocalID.ToString());
                item.Text = GetObjectName(prim);
                item.Tag = prim;
                if (txtSearch.Text.Length == 0 || item.Text.ToLower().Contains(txtSearch.Text.ToLower()))
                {
                    lstPrims.Items.Add(item);
                }
            }
            else
            {
                item.Text = GetObjectName(prim);
            }
        }

        private void Objects_OnNewPrim(Simulator simulator, Primitive prim, ulong regionHandle, ushort timeDilation)
        {
            if (regionHandle != client.Network.CurrentSim.Handle || prim.Position == Vector3.Zero || prim.ParentID != 0 || prim is Avatar) return;
            int distance = (int)Vector3.Distance(client.Self.SimPosition, prim.Position);
            if (distance < searchRadius)
            {
                if (prim.ParentID != 0) throw new Exception("Requested properties for non root prim");
                propRequester.RequestProps(prim.ID);
                AddPrim(prim);
            }
        }

        private void Objects_OnObjectKilled(Simulator simulator, uint objectID)
        {
            return;
            if (simulator.Handle != client.Network.CurrentSim.Handle) return;

            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Objects_OnObjectKilled(simulator, objectID);
                }
                ));
                return;
            }

            ListViewItem item = null;

            foreach (ListViewItem sitem in lstPrims.Items)
            {
                if (((Primitive)sitem.Tag).LocalID == objectID)
                {
                    item = sitem;
                    break;
                }
            }

            if (item != null)
            {
                lstPrims.Items.Remove(item);
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
                        ListViewItem item = new ListViewItem(prim.LocalID.ToString());
                        item.Text = GetObjectName(prim);
                        item.Tag = prim;
                        items.Add(item);
                    }
                }
                ));
            lstPrims.Items.AddRange(items.ToArray());
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lstPrims.BeginUpdate();
            Cursor.Current = Cursors.WaitCursor;
            lstPrims.Items.Clear();
            AddAllObjects();
            Cursor.Current = Cursors.Default;
            lstPrims.EndUpdate();
        }

        private void lstPrims_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPrims.SelectedItems.Count == 1)
            {
                gbxInworld.Enabled = true;
                currentPrim = lstPrims.SelectedItems[0].Tag as Primitive;
                btnBuy.Tag = currentPrim;
                txtCurrentPrim.Text = GetObjectName(currentPrim);
                txtCurrentPrim.Tag = currentPrim;

                if ((currentPrim.Flags & PrimFlags.Money) != 0)
                {
                    btnPay.Enabled = true;
                }
                else
                {
                    btnPay.Enabled = false;
                }

                if (currentPrim.Properties != null && currentPrim.Properties.SaleType != SaleType.Not)
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

        private void frmObjects_Activated(object sender, EventArgs e)
        {
            lstPrims.Focus();
            if (lstPrims.Items.Count > 0)
            {
                lstPrims.FocusedItem = lstPrims.Items[0];
            }
        }

        private void frmObjects_Shown(object sender, EventArgs e)
        {
            btnRefresh_Click(null, null);
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
        Queue<UUID> props = new Queue<UUID>();

        public delegate void TickCallback(int remaining);
        public event TickCallback OnTick;

        public PropertiesQueue(RadegastInstance instance)
        {
            this.instance = instance;
            qTimer = new System.Timers.Timer(1500);
            qTimer.Enabled = true;
            qTimer.Elapsed += new ElapsedEventHandler(qTimer_Elapsed);
        }

        public void RequestProps(UUID objectID)
        {
            lock (sync)
            {
                if (!props.Contains(objectID))
                {
                    props.Enqueue(objectID);
                }
            }
        }

        void qTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (sync)
            {
                for (int i = 0; i < 50 && props.Count > 0; i++)
                {
                    instance.Client.Objects.RequestObjectPropertiesFamily(instance.Client.Network.CurrentSim, props.Dequeue());
                }

                if (OnTick != null)
                {
                    OnTick(props.Count);
                }
            }
        }

        public void Dispose()
        {
            qTimer.Elapsed -= new ElapsedEventHandler(qTimer_Elapsed);
            qTimer.Enabled = false;
            qTimer = null;
            props = null;
            instance = null;
        }
    }

}