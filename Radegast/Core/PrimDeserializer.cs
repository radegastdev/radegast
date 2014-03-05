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
using System.Collections.Generic;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using ClientHelpers = OpenMetaverse.Helpers;

namespace Radegast
{
    public class PrimDeserializer
    {
        public static void ImportFromFile(GridClient client)
        {
            WindowWrapper mainWindow = new WindowWrapper(frmMain.ActiveForm.Handle);
            System.Windows.Forms.OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open object file";
            dlg.Filter = "XML file (*.xml)|*.xml";
            dlg.Multiselect = false;
            DialogResult res = dlg.ShowDialog();

            if (res == DialogResult.OK)
            {

                Thread t = new Thread(new System.Threading.ThreadStart(delegate()
                {
                    try
                    {
                        PrimDeserializer d = new PrimDeserializer(client);
                        string primsXmls = System.IO.File.ReadAllText(dlg.FileName);
                        d.CreateObjectFromXml(primsXmls);
                        d.CleanUp();
                        d = null;
                        MessageBox.Show(mainWindow, "Successfully imported " + dlg.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception excp)
                    {
                        MessageBox.Show(mainWindow, excp.Message, "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }));

                t.IsBackground = true;
                t.Start();

            }
        }

        private enum ImporterState
        {
            RezzingParent,
            RezzingChildren,
            Linking,
            Idle
        }

        private class Linkset
        {
            public Primitive RootPrim;
            public List<Primitive> Children = new List<Primitive>();

            public Linkset()
            {
                RootPrim = new Primitive();
            }

            public Linkset(Primitive rootPrim)
            {
                RootPrim = rootPrim;
            }
        }

        GridClient Client;
        Primitive currentPrim;
        Vector3 currentPosition;
        AutoResetEvent primDone = new AutoResetEvent(false);
        List<Primitive> primsCreated;
        List<uint> linkQueue;
        uint rootLocalID;
        ImporterState state = ImporterState.Idle;

        public PrimDeserializer(GridClient c)
        {
            Client = c;
            Client.Objects.ObjectUpdate += new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
        }

        public void CleanUp()
        {
            Client.Objects.ObjectUpdate -= new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
        }

        public bool CreateObjectFromXml(string xml)
        {
            List<Primitive> prims = ClientHelpers.OSDToPrimList(OSDParser.DeserializeLLSDXml(xml));
            return CreateObject(prims);
        }

        public bool CreateObject(List<Primitive> prims)
        {
            // Build an organized structure from the imported prims
            Dictionary<uint, Linkset> linksets = new Dictionary<uint, Linkset>();
            for (int i = 0; i < prims.Count; i++)
            {
                Primitive prim = prims[i];

                if (prim.ParentID == 0)
                {
                    if (linksets.ContainsKey(prim.LocalID))
                        linksets[prim.LocalID].RootPrim = prim;
                    else
                        linksets[prim.LocalID] = new Linkset(prim);
                }
                else
                {
                    if (!linksets.ContainsKey(prim.ParentID))
                        linksets[prim.ParentID] = new Linkset();

                    linksets[prim.ParentID].Children.Add(prim);
                }
            }

            primsCreated = new List<Primitive>();
            Console.WriteLine("Importing " + linksets.Count + " structures.");

            foreach (Linkset linkset in linksets.Values)
            {
                if (linkset.RootPrim.LocalID != 0)
                {
                    state = ImporterState.RezzingParent;
                    currentPrim = linkset.RootPrim;
                    // HACK: Import the structure just above our head
                    // We need a more elaborate solution for importing with relative or absolute offsets
                    linkset.RootPrim.Position = Client.Self.SimPosition;
                    linkset.RootPrim.Position.Z += 3.0f;
                    currentPosition = linkset.RootPrim.Position;

                    // Rez the root prim with no rotation
                    Quaternion rootRotation = linkset.RootPrim.Rotation;
                    linkset.RootPrim.Rotation = Quaternion.Identity;

                    Client.Objects.AddPrim(Client.Network.CurrentSim, linkset.RootPrim.PrimData, Client.Self.ActiveGroup,
                        linkset.RootPrim.Position, linkset.RootPrim.Scale, linkset.RootPrim.Rotation);

                    if (!primDone.WaitOne(25000, false))
                    {
                        throw new Exception("Rez failed, timed out while creating the root prim.");
                    }
                    Client.Objects.SetPosition(Client.Network.CurrentSim, primsCreated[primsCreated.Count - 1].LocalID, currentPosition);

                    state = ImporterState.RezzingChildren;

                    // Rez the child prims
                    foreach (Primitive prim in linkset.Children)
                    {
                        currentPrim = prim;
                        currentPosition = prim.Position + linkset.RootPrim.Position;

                        Client.Objects.AddPrim(Client.Network.CurrentSim, prim.PrimData, UUID.Zero, currentPosition,
                            prim.Scale, prim.Rotation);

                        if (!primDone.WaitOne(25000, false))
                        {
                            throw new Exception("Rez failed, timed out while creating child prim.");
                        }
                        Client.Objects.SetPosition(Client.Network.CurrentSim, primsCreated[primsCreated.Count - 1].LocalID, currentPosition);
                        // Client.Objects.SetRotation(Client.Network.CurrentSim, primsCreated[primsCreated.Count - 1].LocalID, prim.Rotation);
                    }

                    if (linkset.Children.Count != 0)
                    {
                        // Create a list of the local IDs of the newly created prims
                        List<uint> primIDs = new List<uint>(primsCreated.Count);
                        primIDs.Add(rootLocalID); // Root prim is first in list.
                        foreach (Primitive prim in primsCreated)
                        {
                            if (prim.LocalID != rootLocalID)
                                primIDs.Add(prim.LocalID);
                        }
                        linkQueue = new List<uint>(primIDs.Count);
                        linkQueue.AddRange(primIDs);

                        // Link and set the permissions + rotation
                        state = ImporterState.Linking;
                        Client.Objects.LinkPrims(Client.Network.CurrentSim, linkQueue);
                        Client.Objects.SetRotation(Client.Network.CurrentSim, rootLocalID, rootRotation);

                        if (!primDone.WaitOne(5000, false))
                        {
                            Logger.Log(String.Format("Warning: Failed to link {0} prims", linkQueue.Count), Helpers.LogLevel.Warning);
                        }

                        Client.Objects.SetPermissions(Client.Network.CurrentSim, primIDs,
                            PermissionWho.NextOwner,
                            PermissionMask.All, true);
                    }
                    else
                    {
                        List<uint> primsForPerms = new List<uint>();
                        primsForPerms.Add(rootLocalID);
                        Client.Objects.SetRotation(Client.Network.CurrentSim, rootLocalID, rootRotation);
                        Client.Objects.SetPermissions(Client.Network.CurrentSim, primsForPerms,
                            PermissionWho.NextOwner,
                            PermissionMask.All, true);
                    }
                    state = ImporterState.Idle;
                }
                else
                {
                    // Skip linksets with a missing root prim
                    Logger.Log("WARNING: Skipping a linkset with a missing root prim", Helpers.LogLevel.Warning);
                }

                // Reset everything for the next linkset
                primsCreated.Clear();
            }

            return true;
        }

        void Objects_ObjectUpdate(object sender, PrimEventArgs e)
        {
            if ((e.Prim.Flags & PrimFlags.CreateSelected) == 0)
                return; // We received an update for an object we didn't create

            switch (state)
            {
                case ImporterState.RezzingParent:
                    rootLocalID = e.Prim.LocalID;
                    goto case ImporterState.RezzingChildren;
                case ImporterState.RezzingChildren:
                    if (!primsCreated.Contains(e.Prim))
                    {
                        Console.WriteLine("Setting properties for " + e.Prim.LocalID);
                        // TODO: Is there a way to set all of this at once, and update more ObjectProperties stuff?
                        Client.Objects.SetPosition(e.Simulator, e.Prim.LocalID, currentPosition);
                        Client.Objects.SetTextures(e.Simulator, e.Prim.LocalID, currentPrim.Textures);

                        if (currentPrim.Light != null && currentPrim.Light.Intensity > 0)
                        {
                            Client.Objects.SetLight(e.Simulator, e.Prim.LocalID, currentPrim.Light);
                        }

                        if (currentPrim.Flexible != null)
                        {
                            Client.Objects.SetFlexible(e.Simulator, e.Prim.LocalID, currentPrim.Flexible);
                        }

                        if (currentPrim.Sculpt != null && currentPrim.Sculpt.SculptTexture != UUID.Zero)
                        {
                            Client.Objects.SetSculpt(e.Simulator, e.Prim.LocalID, currentPrim.Sculpt);
                        }

                        if (currentPrim.Properties != null && !String.IsNullOrEmpty(currentPrim.Properties.Name))
                        {
                            Client.Objects.SetName(e.Simulator, e.Prim.LocalID, currentPrim.Properties.Name);
                        }

                        if (currentPrim.Properties != null && !String.IsNullOrEmpty(currentPrim.Properties.Description))
                        {
                            Client.Objects.SetDescription(e.Simulator, e.Prim.LocalID, currentPrim.Properties.Description);
                        }

                        primsCreated.Add(e.Prim);
                        primDone.Set();
                    }
                    break;
                case ImporterState.Linking:
                    lock (linkQueue)
                    {
                        int index = linkQueue.IndexOf(e.Prim.LocalID);
                        if (index != -1)
                        {
                            linkQueue.RemoveAt(index);
                            if (linkQueue.Count == 0)
                                primDone.Set();
                        }
                    }
                    break;
            }
        }
    }
}
