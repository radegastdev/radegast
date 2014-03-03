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
                delegate(Primitive prim)
                {
                    return (prim.LocalID == localID || prim.ParentID == localID);
                }
            );

            RequestObjectProperties(prims, 500);

            int i = prims.FindIndex(
                delegate(Primitive prim)
                {
                    return (prim.LocalID == localID);
                }
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
                delegate(Primitive prim)
                {
                    return (prim.LocalID == rootPrim || prim.ParentID == rootPrim);
                }
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
