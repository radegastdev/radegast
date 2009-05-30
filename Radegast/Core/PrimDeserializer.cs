using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Windows.Forms;
using System.Text;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

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

            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(delegate()
            {
                try {
                    if (res == DialogResult.OK) {
                        PrimDeserializer d = new PrimDeserializer(client);
                        string primsXmls = System.IO.File.ReadAllText(dlg.FileName);
                        d.CreateObjectFromXml(primsXmls);
                        MessageBox.Show(mainWindow, "Successfully imported " + dlg.FileName, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                } catch (Exception excp) {
                    MessageBox.Show(mainWindow, excp.Message, "Saving failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }));

            t.IsBackground = true;
            t.Start();
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
        ObjectManager.NewPrimCallback newPrimCallback = null;

        public PrimDeserializer(GridClient c)
        {
            Client = c;
            if (newPrimCallback == null) {
                newPrimCallback = new ObjectManager.NewPrimCallback(Objects_OnNewPrim);
                Client.Objects.OnNewPrim += newPrimCallback;
            }
        }

        ~PrimDeserializer()
        {
            if (newPrimCallback != null) {
                Client.Objects.OnNewPrim -= newPrimCallback;
            }
        }

        public bool CreateObjectFromXml(string xml)
        {
            List<Primitive> prims = Helpers.OSDToPrimList(OSDParser.DeserializeLLSDXml(xml));
            return CreateObject(prims);
        }

        public bool CreateObject(List<Primitive> prims)
        {
            // Build an organized structure from the imported prims
            Dictionary<uint, Linkset> linksets = new Dictionary<uint, Linkset>();
            for (int i = 0; i < prims.Count; i++) {
                Primitive prim = prims[i];

                if (prim.ParentID == 0) {
                    if (linksets.ContainsKey(prim.LocalID))
                        linksets[prim.LocalID].RootPrim = prim;
                    else
                        linksets[prim.LocalID] = new Linkset(prim);
                } else {
                    if (!linksets.ContainsKey(prim.ParentID))
                        linksets[prim.ParentID] = new Linkset();

                    linksets[prim.ParentID].Children.Add(prim);
                }
            }

            primsCreated = new List<Primitive>();
            Console.WriteLine("Importing " + linksets.Count + " structures.");

            foreach (Linkset linkset in linksets.Values) {
                if (linkset.RootPrim.LocalID != 0) {
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

                    if (!primDone.WaitOne(5000, false)) {
                        throw new Exception("Rez failed, timed out while creating the root prim.");
                    }
                    Client.Objects.SetPosition(Client.Network.CurrentSim, primsCreated[primsCreated.Count - 1].LocalID, currentPosition);

                    state = ImporterState.RezzingChildren;

                    // Rez the child prims
                    foreach (Primitive prim in linkset.Children) {
                        currentPrim = prim;
                        currentPosition = prim.Position + linkset.RootPrim.Position;

                        Client.Objects.AddPrim(Client.Network.CurrentSim, prim.PrimData, UUID.Zero, currentPosition,
                            prim.Scale, prim.Rotation);

                        if (!primDone.WaitOne(5000, false)) {
                            throw new Exception("Rez failed, timed out while creating child prim.");
                        }
                        Client.Objects.SetPosition(Client.Network.CurrentSim, primsCreated[primsCreated.Count - 1].LocalID, currentPosition);
                        // Client.Objects.SetRotation(Client.Network.CurrentSim, primsCreated[primsCreated.Count - 1].LocalID, prim.Rotation);
                    }

                    if (linkset.Children.Count != 0) {
                        // Create a list of the local IDs of the newly created prims
                        List<uint> primIDs = new List<uint>(primsCreated.Count);
                        primIDs.Add(rootLocalID); // Root prim is first in list.
                        foreach (Primitive prim in primsCreated) {
                            if (prim.LocalID != rootLocalID)
                                primIDs.Add(prim.LocalID);
                        }
                        linkQueue = new List<uint>(primIDs.Count);
                        linkQueue.AddRange(primIDs);

                        // Link and set the permissions + rotation
                        state = ImporterState.Linking;
                        Client.Objects.LinkPrims(Client.Network.CurrentSim, linkQueue);
                        Client.Objects.SetRotation(Client.Network.CurrentSim, rootLocalID, rootRotation);

                        if (!primDone.WaitOne(1000, false)) {
                            Logger.Log(String.Format("Warning: Failed to link {0} prims", linkQueue.Count), Helpers.LogLevel.Warning);
                        }

                        Client.Objects.SetPermissions(Client.Network.CurrentSim, primIDs,
                            PermissionWho.NextOwner,
                            PermissionMask.All, true);
                    } else {
                        List<uint> primsForPerms = new List<uint>();
                        primsForPerms.Add(rootLocalID);
                        Client.Objects.SetRotation(Client.Network.CurrentSim, rootLocalID, rootRotation);
                        Client.Objects.SetPermissions(Client.Network.CurrentSim, primsForPerms,
                            PermissionWho.NextOwner,
                            PermissionMask.All, true);
                    }
                    state = ImporterState.Idle;
                } else {
                    // Skip linksets with a missing root prim
                    Logger.Log("WARNING: Skipping a linkset with a missing root prim", Helpers.LogLevel.Warning);
                }

                // Reset everything for the next linkset
                primsCreated.Clear();
            }

            return true;
        }

        void Objects_OnNewPrim(Simulator simulator, Primitive prim, ulong regionHandle, ushort timeDilation)
        {
            if ((prim.Flags & PrimFlags.CreateSelected) == 0)
                return; // We received an update for an object we didn't create

            if (prim.OwnerID != Client.Self.AgentID) {
                //return;
            }

            switch (state) {
                case ImporterState.RezzingParent:
                    rootLocalID = prim.LocalID;
                    goto case ImporterState.RezzingChildren;
                case ImporterState.RezzingChildren:
                    if (!primsCreated.Contains(prim)) {
                        Console.WriteLine("Setting properties for " + prim.LocalID);
                        // TODO: Is there a way to set all of this at once, and update more ObjectProperties stuff?
                        Client.Objects.SetPosition(simulator, prim.LocalID, currentPosition);
                        Client.Objects.SetTextures(simulator, prim.LocalID, currentPrim.Textures);

                        if (currentPrim.Light.Intensity > 0) {
                            Client.Objects.SetLight(simulator, prim.LocalID, currentPrim.Light);
                        }

                        Client.Objects.SetFlexible(simulator, prim.LocalID, currentPrim.Flexible);

                        if (currentPrim.Sculpt.SculptTexture != UUID.Zero) {
                            Client.Objects.SetSculpt(simulator, prim.LocalID, currentPrim.Sculpt);
                        }

                        if (!String.IsNullOrEmpty(currentPrim.Properties.Name)) {
                            Client.Objects.SetName(simulator, prim.LocalID, currentPrim.Properties.Name);
                        }

                        if (!String.IsNullOrEmpty(currentPrim.Properties.Description)) {
                            Client.Objects.SetDescription(simulator, prim.LocalID, currentPrim.Properties.Description);
                        }

                        primsCreated.Add(prim);
                        primDone.Set();
                    }
                    break;
                case ImporterState.Linking:
                    lock (linkQueue)
                    {
                        int index = linkQueue.IndexOf(prim.LocalID);
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
