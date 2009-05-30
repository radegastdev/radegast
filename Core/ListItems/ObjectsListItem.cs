using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using OpenMetaverse;

namespace Radegast
{
    public class ObjectsListItem
    {
        private Primitive prim;
        private GridClient client;
        private ListBox listBox;
        private bool gotProperties = false;
        private bool gettingProperties = false;

        public ObjectsListItem(Primitive prim, GridClient client, ListBox listBox)
        {
            this.prim = prim;
            this.client = client;
            this.listBox = listBox;
        }

        public void RequestProperties()
        {
            if (prim.Properties == null)
            {
                gettingProperties = true;
                client.Objects.OnObjectPropertiesFamily += new ObjectManager.ObjectPropertiesFamilyCallback(Objects_OnObjectPropertiesFamily);
                client.Objects.RequestObjectPropertiesFamily(client.Network.CurrentSim, prim.ID);
            }
            else
            {
                gotProperties = true;
                OnPropertiesReceived(EventArgs.Empty);
            }
        }

        void Objects_OnObjectPropertiesFamily(Simulator simulator, Primitive.ObjectProperties props, ReportType type)
        {
            if (props.ObjectID != prim.ID) return;
            gettingProperties = false;
            gotProperties = true;
            prim.Properties = props;
            listBox.BeginInvoke(
                new OnPropReceivedRaise(OnPropertiesReceived),
                new object[] { EventArgs.Empty });
        }

        public override string ToString()
        {
            return (String.IsNullOrEmpty(prim.Properties.Name) ? "..." : prim.Properties.Name);
        }

        public event EventHandler PropertiesReceived;
        private delegate void OnPropReceivedRaise(EventArgs e);
        protected virtual void OnPropertiesReceived(EventArgs e)
        {
            if (PropertiesReceived != null) PropertiesReceived(this, e);
        }

        public Primitive Prim
        {
            get { return prim; }
        }

        public bool GotProperties
        {
            get { return gotProperties; }
        }

        public bool GettingProperties
        {
            get { return gettingProperties; }
        }
    }
}
