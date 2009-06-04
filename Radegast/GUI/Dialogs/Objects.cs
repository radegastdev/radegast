using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public partial class frmObjects : Form
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client;} }
        private Primitive currentPrim;
        private float searchRadius = 35.0f;

        public frmObjects(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmObjects_Disposed);

            this.instance = instance;
            
            btnPointAt.Text = (this.instance.State.IsPointing ? "Unpoint" : "Point At");
            btnSitOn.Text = (this.instance.State.IsSitting ? "Stand Up" : "Sit On");

            lstPrims.ListViewItemSorter = new ObjectSorter(client.Self);

            // Callbacks
            client.Network.OnDisconnected += new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Objects.OnNewPrim += new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
            client.Objects.OnObjectKilled += new ObjectManager.KillObjectCallback(Objects_OnObjectKilled);
            client.Objects.OnObjectPropertiesFamily += new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
            instance.OnAvatarName += new RadegastInstance.OnAvatarNameCallBack(instance_OnAvatarName);
        }

        void frmObjects_Disposed(object sender, EventArgs e)
        {
            client.Network.OnDisconnected -= new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Objects.OnNewPrim -= new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
            client.Objects.OnObjectKilled -= new ObjectManager.KillObjectCallback(Objects_OnObjectKilled);
            client.Objects.OnObjectPropertiesFamily -= new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);

            instance.OnAvatarName -= new RadegastInstance.OnAvatarNameCallBack(instance_OnAvatarName);
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
                    item.Text = GetObjectName(prim);
                    break;
                }
            }
        }

        private void Network_OnDisconnected(NetworkManager.DisconnectType reason, string message)
        {
            this.Close();
        }

        private string GetObjectName(Primitive prim, int distance)
        {
            string name = "Loading...";
            string ownerName = "Loading...";

            if (prim.Properties == null)
            {
                client.Objects.RequestObjectPropertiesFamily(client.Network.CurrentSim, prim.ID);
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
            if (regionHandle != client.Network.CurrentSim.Handle || prim.Position == Vector3.Zero) return;
            int distance = (int)Vector3.Distance(client.Self.SimPosition, prim.Position);
            if (distance < searchRadius)
            {
                AddPrim(prim);
            }
        }

        private void Objects_OnObjectKilled(Simulator simulator, uint objectID)
        {
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

            client.Network.CurrentSim.ObjectsPrimitives.ForEach(
                new Action<Primitive>(
                delegate(Primitive prim)
                {
                    int distance = (int)Vector3.Distance(prim.Position, location);
                    if (prim.ParentID == 0 && (prim.Position != Vector3.Zero) && (distance < searchRadius)) //root prims only
                    {
                        AddPrim(prim);
                    }
                }
                ));
        }

        private void frmObjects_Load(object sender, EventArgs e)
        {
            AddAllObjects();
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
            ApplySearch();
        }

        private void ApplySearch()
        {
            List<ListViewItem> toRemove = new List<ListViewItem>();
            foreach (ListViewItem item in lstPrims.Items)
            {
                if (!item.Text.ToLower().Contains(txtSearch.Text.ToLower()))
                {
                    toRemove.Add(item);
                }
            }
            foreach (ListViewItem item in toRemove)
            {
                lstPrims.Items.Remove(item);
            }
            AddAllObjects();

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            txtSearch.Select();
            ApplySearch();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            lstPrims.Items.Clear();
            AddAllObjects();
        }

        private void lstPrims_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPrims.SelectedItems.Count == 1)
            {
                gbxInworld.Enabled = true;
                currentPrim = lstPrims.SelectedItems[0].Tag as Primitive;
            }
            else
            {
                gbxInworld.Enabled = false;
            }
        }
    }

    public class ObjectSorter : IComparer
    {
        private AgentManager me;

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
}