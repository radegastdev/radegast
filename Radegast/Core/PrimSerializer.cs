/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using ClientHelpers = OpenMetaverse.Helpers;

namespace Radegast
{
    public class PrimSerializer
    {
        List<UUID> Textures = new List<UUID>();
        UUID SelectedObject = UUID.Zero;

        Dictionary<UUID, Primitive> PrimsWaiting = new Dictionary<UUID, Primitive>();
        AutoResetEvent AllPropertiesReceived = new AutoResetEvent(false);

        private GridClient Client;

        public PrimSerializer(GridClient c)
        {
            Client = c;
            Client.Objects.ObjectProperties += new System.EventHandler<ObjectPropertiesEventArgs>(Objects_ObjectProperties);
        }

        public void CleanUp()
        {
            Client.Objects.ObjectProperties -= new System.EventHandler<ObjectPropertiesEventArgs>(Objects_ObjectProperties);
        }

        public string GetSerializedAttachmentPrims(Simulator sim, uint localID)
        {
            List<Primitive> prims = sim.ObjectsPrimitives.FindAll(
                prim => (prim.LocalID == localID || prim.ParentID == localID)
            );

            RequestObjectProperties(prims, 500);

            int i = prims.FindIndex(
                prim => (prim.LocalID == localID)
            );

            if (i >= 0) {
                prims[i].ParentID = 0;
            }

            return OSDParser.SerializeLLSDXmlString(ClientHelpers.PrimListToOSD(prims));
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
                prim => (prim.LocalID == rootPrim || prim.ParentID == rootPrim)
            );

            RequestObjectProperties(prims, 500);

            return OSDParser.SerializeLLSDXmlString(ClientHelpers.PrimListToOSD(prims));
        }

        private bool RequestObjectProperties(List<Primitive> objects, int msPerRequest)
        {
            // Create an array of the local IDs of all the prims we are requesting properties for
            uint[] localids = new uint[objects.Count];

            lock (PrimsWaiting) {
                PrimsWaiting.Clear();

                for (int i = 0; i < objects.Count; ++i)
                {
                    localids[i] = objects[i].LocalID;
                    PrimsWaiting.Add(objects[i].ID, objects[i]);
                }
            }

            if (localids.Length > 0)
            {
                Client.Objects.SelectObjects(Client.Network.CurrentSim, localids, false);
                bool success = AllPropertiesReceived.WaitOne(2000 + msPerRequest * localids.Length, false);
                if (PrimsWaiting.Count > 0)
                {
                    Logger.Log("Failed to retrieve object properties for " + PrimsWaiting.Count + " prims out of " + localids.Length, Helpers.LogLevel.Warning, Client);

                }
                Client.Objects.DeselectObjects(Client.Network.CurrentSim, localids);
                return success;
            }
            return true;
        }

        void Objects_ObjectProperties(object sender, ObjectPropertiesEventArgs e)
        {
            lock (PrimsWaiting)
            {
                PrimsWaiting.Remove(e.Properties.ObjectID);

                if (PrimsWaiting.Count == 0)
                    AllPropertiesReceived.Set();
            }
        }
    }
}
