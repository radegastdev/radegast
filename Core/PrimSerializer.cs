using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public class PrimSerializer
    {
        List<UUID> Textures = new List<UUID>();
        //Primitive.ObjectProperties Properties;
        UUID SelectedObject = UUID.Zero;

        Dictionary<UUID, Primitive> PrimsWaiting = new Dictionary<UUID, Primitive>();
        AutoResetEvent AllPropertiesReceived = new AutoResetEvent(false);

        private GridClient Client;

        public PrimSerializer(GridClient c)
        {
            Client = c;
            Client.Objects.OnObjectProperties += new ObjectManager.ObjectPropertiesCallback(Objects_OnObjectProperties);
        }


        public string GetSerializedAttachmentPrims(Simulator sim, uint localID)
        {
            List<Primitive> prims = sim.ObjectsPrimitives.FindAll(
                delegate(Primitive prim)
                {
                    return (prim.LocalID == localID || prim.ParentID == localID);
                }
            );

            bool complete = RequestObjectProperties(prims, 500);

            int i = prims.FindIndex(
                delegate(Primitive prim)
                {
                    return (prim.LocalID == localID);
                }
            );

            if (i >= 0) {
                prims[i].ParentID = 0;
            }

            return OSDParser.SerializeLLSDXmlString(Helpers.PrimListToOSD(prims));
        }

        public string GetSerializedPrims(Simulator sim, uint localID)
        {
            Primitive p;
            sim.ObjectsPrimitives.TryGetValue(localID, out p);

            if (p == null) {
                return "";
            }

            uint rootPrim = p.ParentID == 0 ? p.LocalID : p.ParentID;

            List<Primitive> prims = sim.ObjectsPrimitives.FindAll(
                delegate(Primitive prim)
                {
                    return (prim.LocalID == rootPrim || prim.ParentID == rootPrim);
                }
            );

            if (!RequestObjectProperties(prims, 500))
            {
                Logger.Log("Failed to retrieve object properties for " + PrimsWaiting.Count + " prims out of " + prims.Count, Helpers.LogLevel.Warning, Client);
            }

            return OSDParser.SerializeLLSDXmlString(Helpers.PrimListToOSD(prims));
        }

        private bool RequestObjectProperties(List<Primitive> objects, int msPerRequest)
        {
            // Create an array of the local IDs of all the prims we are requesting properties for
            uint[] localids = new uint[objects.Count];

            lock (PrimsWaiting) {
                PrimsWaiting.Clear();

                for (int i = 0; i < objects.Count; ++i) {
                    if (objects[i].Properties == null)
                    {
                        localids[i] = objects[i].LocalID;
                        PrimsWaiting.Add(objects[i].ID, objects[i]);
                    }
                }
            }

            if (localids.Length > 0)
            {
                Client.Objects.SelectObjects(Client.Network.CurrentSim, localids, false);
                return AllPropertiesReceived.WaitOne(2000 + msPerRequest * localids.Length, false);
            }
            return true;
        }

        void Objects_OnObjectProperties(Simulator simulator, Primitive.ObjectProperties properties)
        {
            lock (PrimsWaiting)
            {
                PrimsWaiting.Remove(properties.ObjectID);

                if (PrimsWaiting.Count == 0)
                    AllPropertiesReceived.Set();
            }
        }
    }
}
