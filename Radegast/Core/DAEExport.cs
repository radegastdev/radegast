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

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Drawing;
using OpenMetaverse;
using OpenMetaverse.Rendering;
using OpenMetaverse.Imaging;
using OpenMetaverse.Assets;

namespace Radegast
{
    public class DAEExport : IDisposable
    {
        public string FileName { get; set; }
        public Primitive RootPrim { get; set; }
        public List<Primitive> Prims = new List<Primitive>();
        public List<FacetedMesh> MeshedPrims = new List<FacetedMesh>();
        public List<UUID> Textures = new List<UUID>();
        public string[] TextureNames;
        public int ExportablePrims { get; set; }
        public int ExportableTextures { get; set; }
        public bool ExportTextures { get; set; }
        public string ImageFormat { get; set; }
        public bool ConsolidateMaterials { get; set; }
        public bool SkipTransparentFaces { get; set; }

        public static readonly List<UUID> BuiltInTextures = new List<UUID>() {
            new UUID("89556747-24cb-43ed-920b-47caed15465f"), // default
            new UUID("be293869-d0d9-0a69-5989-ad27f1946fd4"), // default sculpt 
            new UUID("5748decc-f629-461c-9a36-a35a221fe21f"), // blank
            new UUID("38b86f85-2575-52a9-a531-23108d8da837"), // invisible
            new UUID("8dcd4a48-2d37-4909-9f78-f7a9eb4ef903"), // tranparent
        };

        RadegastInstance Instance;
        GridClient Client { get { return Instance.Client; } }
        System.Globalization.CultureInfo invariant = System.Globalization.CultureInfo.InvariantCulture;
        MeshmerizerR Mesher;
        XmlDocument Doc;

        public event EventHandler<DAEStatutsEventArgs> Progress;

        void OnProgress(string message)
        {
            if (Progress != null)
            {
                Progress(this, new DAEStatutsEventArgs(message));
            }
        }

        public DAEExport(RadegastInstance instance, Primitive requestedPrim)
        {
            Instance = instance;
            ImageFormat = "PNG";
            ConsolidateMaterials = true;
            SkipTransparentFaces = true;
            Mesher = new MeshmerizerR();
            Init(Client.Network.CurrentSim, requestedPrim);
        }

        public void Dispose()
        {
        }

        public void Init(Simulator sim, Primitive requestedPrim)
        {
            if (requestedPrim == null || !Client.Network.Connected) return;

            RootPrim = requestedPrim;

            if (requestedPrim.ParentID != 0)
            {
                Primitive parent;
                if (sim.ObjectsPrimitives.TryGetValue(requestedPrim.ParentID, out parent))
                {
                    RootPrim = parent;
                }
            }

            Prims.Clear();
            Primitive root = new Primitive(RootPrim);
            root.Position = Vector3.Zero;
            Prims.Add(root);

            sim.ObjectsPrimitives
                .FindAll(p => p.ParentID == RootPrim.LocalID)
                .ForEach(p =>
                    {
                        var child = new Primitive(p);
                        child.Position = root.Position + child.Position * root.Rotation;
                        child.Rotation = root.Rotation * child.Rotation;
                        Prims.Add(child);
                    });
            List<uint> select = new List<uint>(Prims.Count);
            Prims.ForEach(p => select.Add(p.LocalID));
            Client.Objects.SelectObjects(sim, select.ToArray(), true);

            OnProgress("Checking permissions");
            ExportablePrims = 0;
            for (int i = 0; i < Prims.Count; i++)
            {
                if (!CanExport(Prims[i])) continue;
                ExportablePrims++;

                var defaultTexture = Prims[i].Textures.DefaultTexture;
                if (defaultTexture != null && !Textures.Contains(defaultTexture.TextureID))
                {
                    Textures.Add(defaultTexture.TextureID);
                }
                for (int j = 0; j < Prims[i].Textures.FaceTextures.Length; j++)
                {
                    var te = Prims[i].Textures.FaceTextures[j];
                    if (te == null) continue;
                    UUID id = te.TextureID;
                    if (!Textures.Contains(id))
                    {
                        Textures.Add(id);
                    }
                }
            }

            TextureNames = new string[Textures.Count];
            ExportableTextures = 0;
            for (int i = 0; i < Textures.Count; i++)
            {
                string name;
                if (CanExportTexture(Textures[i], out name))
                {
                    ExportableTextures++;
                    TextureNames[i] = RadegastInstance.SafeFileName(name).Replace(' ', '_');
                }
            }
        }

        public void Export(string Filename)
        {
            this.FileName = Filename;

            MeshedPrims.Clear();

            if (string.IsNullOrEmpty(FileName))
            {
                return;
            }

            WorkPool.QueueUserWorkItem(sync =>
            {
                if (ExportTextures)
                {
                    SaveTextures();
                }
                for (int i = 0; i < Prims.Count; i++)
                {
                    if (!CanExport(Prims[i])) continue;

                    FacetedMesh mesh = MeshPrim(Prims[i]);
                    if (mesh == null) continue;

                    for (int j = 0; j < mesh.Faces.Count; j++)
                    {
                        Face face = mesh.Faces[j];

                        Primitive.TextureEntryFace teFace = mesh.Faces[j].TextureFace;
                        if (teFace == null) continue;


                        // Sculpt UV vertically flipped compared to prims. Flip back
                        if (Prims[i].Sculpt != null && Prims[i].Sculpt.SculptTexture != UUID.Zero && Prims[i].Sculpt.Type != SculptType.Mesh)
                        {
                            teFace = (Primitive.TextureEntryFace)teFace.Clone();
                            teFace.RepeatV *= -1;
                        }

                        // Texture transform for this face
                        Mesher.TransformTexCoords(face.Vertices, face.Center, teFace, Prims[i].Scale);

                    }
                    MeshedPrims.Add(mesh);
                }

                string msg;
                if (MeshedPrims.Count == 0)
                {
                    msg = string.Format("Can export 0 out of {0} prims.{1}{1}Skipping.", Prims.Count, Environment.NewLine);
                }
                else
                {
                    msg = string.Format("Exported {0} out of {1} objects to{2}{2}{3}", MeshedPrims.Count, Prims.Count, Environment.NewLine, FileName);
                }
                GenerateCollada();
                File.WriteAllText(FileName, DocToString(Doc));
                OnProgress(msg);
            });
        }

        void SaveTextures()
        {
            OnProgress("Exporting textures...");
            for (int i = 0; i < Textures.Count; i++)
            {
                var id = Textures[i];
                if (TextureNames[i] == null)
                {
                    OnProgress("Skipping " + id.ToString() + " due to insufficient permissions");
                    continue;
                }

                string filename = TextureNames[i] + "." + ImageFormat.ToLower();

                OnProgress("Fetching texture" + id);

                ManagedImage mImage = null;
                Image wImage = null;
                byte[] jpegData = null;

                try
                {
                    System.Threading.AutoResetEvent gotImage = new System.Threading.AutoResetEvent(false);
                    Client.Assets.RequestImage(id, ImageType.Normal, (state, asset) =>
                    {
                        if (state == TextureRequestState.Finished && asset != null)
                        {
                            jpegData = asset.AssetData;
                            OpenJPEG.DecodeToImage(jpegData, out mImage, out wImage);
                            gotImage.Set();
                        }
                        else if (state != TextureRequestState.Pending && state != TextureRequestState.Started && state != TextureRequestState.Progress)
                        {
                            gotImage.Set();
                        }
                    });
                    gotImage.WaitOne(120 * 1000, false);

                    if (wImage != null)
                    {
                        string fullFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(FileName), filename);
                        switch (ImageFormat)
                        {
                            case "PNG":
                                wImage.Save(fullFileName, System.Drawing.Imaging.ImageFormat.Png);
                                break;
                            case "JPG":
                                wImage.Save(fullFileName, System.Drawing.Imaging.ImageFormat.Jpeg);
                                break;
                            case "BMP":
                                wImage.Save(fullFileName, System.Drawing.Imaging.ImageFormat.Bmp);
                                break;
                            case "J2C":
                                File.WriteAllBytes(fullFileName, jpegData);
                                break;
                            case "TGA":
                                File.WriteAllBytes(fullFileName, mImage.ExportTGA());
                                break;
                            default:
                                throw new Exception("Unsupported image format");
                        }
                        OnProgress("Saved to " + fullFileName);
                    }
                    else
                    {
                        throw new Exception("Failed to decode image");
                    }

                }
                catch (Exception ex)
                {
                    OnProgress("Failed: " + ex.ToString());
                    TextureNames[i] = null;
                }

            }
        }

        public static bool IsFullPerm(Permissions Permissions)
        {
            if (
                ((Permissions.OwnerMask & PermissionMask.Modify) != 0) &&
                ((Permissions.OwnerMask & PermissionMask.Copy) != 0) &&
                ((Permissions.OwnerMask & PermissionMask.Transfer) != 0)
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        bool CanExport(Primitive prim)
        {
            if (prim.Properties == null) return false;

            return (prim.Properties.OwnerID == Client.Self.AgentID) &&
                (prim.Properties.CreatorID == Client.Self.AgentID) ||
                (Instance.Netcom.LoginOptions.Grid.Platform != "SecondLife"
                && prim.Properties.OwnerID == Client.Self.AgentID
                && IsFullPerm(prim.Properties.Permissions)) ||
                Instance.advancedDebugging;
        }

        bool CanExportTexture(UUID id, out string name)
        {
            name = id.ToString();
            if (BuiltInTextures.Contains(id) || Instance.Netcom.LoginOptions.Grid.Platform != "SecondLife")
            {
                return true;
            }

            InventoryItem item = null;
            foreach (var pair in Client.Inventory.Store.Items)
            {
                if (pair.Value.Data is InventoryItem)
                {
                    var i = (InventoryItem)pair.Value.Data;
                    if (i.AssetUUID == id && (InventoryConsole.IsFullPerm(i) || Instance.advancedDebugging))
                    {
                        item = i;
                        break;
                    }
                }
            }

            if (item != null)
            {
                name = item.Name;
                return true;
            }
            else if (Instance.advancedDebugging)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        FacetedMesh MeshPrim(Primitive prim)
        {
            FacetedMesh mesh = null;
            if (prim.Sculpt == null || prim.Sculpt.SculptTexture == UUID.Zero)
            {
                mesh = Mesher.GenerateFacetedMesh(prim, DetailLevel.Highest);
            }
            else if (prim.Sculpt.Type != SculptType.Mesh)
            {
                Image img = null;
                if (LoadTexture(prim.Sculpt.SculptTexture, ref img, true))
                {
                    mesh = Mesher.GenerateFacetedSculptMesh(prim, (Bitmap)img, DetailLevel.Highest);
                }
            }
            else
            {
                var gotMesh = new System.Threading.AutoResetEvent(false);

                Client.Assets.RequestMesh(prim.Sculpt.SculptTexture, (success, meshAsset) =>
                {
                    if (!success || !FacetedMesh.TryDecodeFromAsset(prim, meshAsset, DetailLevel.Highest, out mesh))
                    {
                        Logger.Log("Failed to fetch or decode the mesh asset", Helpers.LogLevel.Warning, Client);
                    }
                    gotMesh.Set();
                });

                gotMesh.WaitOne(20 * 1000, false);
            }
            return mesh;
        }

        bool LoadTexture(UUID textureID, ref Image texture, bool removeAlpha)
        {
            var gotImage = new System.Threading.ManualResetEvent(false);
            Image img = null;

            try
            {
                gotImage.Reset();
                byte[] tgaData;
                Client.Assets.RequestImage(textureID, (TextureRequestState state, AssetTexture assetTexture) =>
                {
                    ManagedImage mi;
                    if (state == TextureRequestState.Finished && OpenJPEG.DecodeToImage(assetTexture.AssetData, out mi))
                    {

                        if (removeAlpha)
                        {
                            if ((mi.Channels & ManagedImage.ImageChannels.Alpha) != 0)
                            {
                                mi.ConvertChannels(mi.Channels & ~ManagedImage.ImageChannels.Alpha);
                            }
                        }
                        tgaData = mi.ExportTGA();
                        img = LoadTGAClass.LoadTGA(new MemoryStream(tgaData));
                    }
                    gotImage.Set();
                });
                gotImage.WaitOne(30 * 1000, false);

                if (img != null)
                {
                    texture = img;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, Helpers.LogLevel.Error, Client, e);
                return false;
            }
        }

        XmlNode ColladaInit()
        {
            Doc = new XmlDocument();
            var root = Doc.AppendChild(Doc.CreateElement("COLLADA"));
            root.Attributes.Append(Doc.CreateAttribute("xmlns")).Value = "http://www.collada.org/2005/11/COLLADASchema";
            root.Attributes.Append(Doc.CreateAttribute("version")).Value = "1.4.1";

            var asset = root.AppendChild(Doc.CreateElement("asset"));
            var contributor = asset.AppendChild(Doc.CreateElement("contributor"));
            contributor.AppendChild(Doc.CreateElement("author")).InnerText = "Radegast User";
            contributor.AppendChild(Doc.CreateElement("authoring_tool")).InnerText = "Radegast Collada Export";

            asset.AppendChild(Doc.CreateElement("created")).InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            asset.AppendChild(Doc.CreateElement("modified")).InnerText = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");

            var unit = asset.AppendChild(Doc.CreateElement("unit"));
            unit.Attributes.Append(Doc.CreateAttribute("name")).Value = "meter";
            unit.Attributes.Append(Doc.CreateAttribute("meter")).Value = "1";

            asset.AppendChild(Doc.CreateElement("up_axis")).InnerText = "Z_UP";

            return root;
        }

        void AddSource(XmlNode mesh, string src_id, string param, List<float> vals)
        {
            var source = mesh.AppendChild(Doc.CreateElement("source"));
            source.Attributes.Append(Doc.CreateAttribute("id")).InnerText = src_id;
            var src_array = source.AppendChild(Doc.CreateElement("float_array"));

            src_array.Attributes.Append(Doc.CreateAttribute("id")).InnerText = string.Format("{0}-{1}", src_id, "array");
            src_array.Attributes.Append(Doc.CreateAttribute("count")).InnerText = vals.Count.ToString();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vals.Count; i++)
            {
                sb.Append(vals[i].ToString(invariant));
                if (i != vals.Count - 1)
                {
                    sb.Append(" ");
                }
            }
            src_array.InnerText = sb.ToString();

            var acc = source.AppendChild(Doc.CreateElement("technique_common"))
                .AppendChild(Doc.CreateElement("accessor"));
            acc.Attributes.Append(Doc.CreateAttribute("source")).InnerText = string.Format("#{0}-{1}", src_id, "array");
            acc.Attributes.Append(Doc.CreateAttribute("count")).InnerText = ((int)(vals.Count / param.Length)).ToString();
            acc.Attributes.Append(Doc.CreateAttribute("stride")).InnerText = param.Length.ToString();

            foreach (char c in param)
            {
                var pX = acc.AppendChild(Doc.CreateElement("param"));
                pX.Attributes.Append(Doc.CreateAttribute("name")).InnerText = c.ToString();
                pX.Attributes.Append(Doc.CreateAttribute("type")).InnerText = "float";
            }

        }

        void AddPolygons(XmlNode mesh, string geomID, string materialID, FacetedMesh obj, List<int> faces_to_include)
        {
            var polylist = mesh.AppendChild(Doc.CreateElement("polylist"));
            polylist.Attributes.Append(Doc.CreateAttribute("material")).InnerText = materialID;

            // Vertices semantic
            {
                var input = polylist.AppendChild(Doc.CreateElement("input"));
                input.Attributes.Append(Doc.CreateAttribute("semantic")).InnerText = "VERTEX";
                input.Attributes.Append(Doc.CreateAttribute("offset")).InnerText = "0";
                input.Attributes.Append(Doc.CreateAttribute("source")).InnerText = string.Format("#{0}-{1}", geomID, "vertices");
            }

            // Normals semantic
            {
                var input = polylist.AppendChild(Doc.CreateElement("input"));
                input.Attributes.Append(Doc.CreateAttribute("semantic")).InnerText = "NORMAL";
                input.Attributes.Append(Doc.CreateAttribute("offset")).InnerText = "0";
                input.Attributes.Append(Doc.CreateAttribute("source")).InnerText = string.Format("#{0}-{1}", geomID, "normals");
            }

            // UV semantic
            {
                var input = polylist.AppendChild(Doc.CreateElement("input"));
                input.Attributes.Append(Doc.CreateAttribute("semantic")).InnerText = "TEXCOORD";
                input.Attributes.Append(Doc.CreateAttribute("offset")).InnerText = "0";
                input.Attributes.Append(Doc.CreateAttribute("source")).InnerText = string.Format("#{0}-{1}", geomID, "map0");
            }

            // Save indices
            var vcount = polylist.AppendChild(Doc.CreateElement("vcount"));
            var p = polylist.AppendChild(Doc.CreateElement("p"));
            int index_offset = 0;
            int num_tris = 0;
            StringBuilder pBuilder = new StringBuilder();
            StringBuilder vcountBuilder = new StringBuilder();

            for (int face_num = 0; face_num < obj.Faces.Count; face_num++)
            {
                var face = obj.Faces[face_num];
                if (faces_to_include == null || faces_to_include.Contains(face_num))
                {
                    for (int i = 0; i < face.Indices.Count; i++)
                    {
                        int index = index_offset + face.Indices[i];
                        pBuilder.Append(index);
                        pBuilder.Append(" ");
                        if (i % 3 == 0)
                        {
                            vcountBuilder.Append("3 ");
                            num_tris++;
                        }
                    }
                }
                index_offset += face.Vertices.Count;
            }

            p.InnerText = pBuilder.ToString().TrimEnd();
            vcount.InnerText = vcountBuilder.ToString().TrimEnd();
            polylist.Attributes.Append(Doc.CreateAttribute("count")).InnerText = num_tris.ToString();
        }

        void GenerateImagesSection(XmlNode images)
        {
            for (int i = 0; i < TextureNames.Length; i++)
            {
                string name = TextureNames[i];
                if (name == null) continue;
                string colladaName = name + "_" + ImageFormat.ToLower();
                var image = images.AppendChild(Doc.CreateElement("image"));
                image.Attributes.Append(Doc.CreateAttribute("id")).InnerText = colladaName;
                image.Attributes.Append(Doc.CreateAttribute("name")).InnerText = colladaName;
                image.AppendChild(Doc.CreateElement("init_from")).InnerText = System.Web.HttpUtility.UrlEncode(name + "." + ImageFormat.ToLower());
            }
        }

        class MaterialInfo
        {
            public UUID TextureID;
            public Color4 Color;
            public string Name;

            public bool Matches(Primitive.TextureEntryFace TextureEntry)
            {
                return TextureID == TextureEntry.TextureID && Color == TextureEntry.RGBA;
            }
        }

        List<MaterialInfo> AllMeterials = new List<MaterialInfo>();

        MaterialInfo GetMaterial(Primitive.TextureEntryFace te)
        {
            MaterialInfo ret = null;
            foreach (var mat in AllMeterials)
            {
                if (mat.Matches(te))
                {
                    ret = mat;
                    break;
                }
            }

            if (ret == null)
            {
                ret = new MaterialInfo()
                {
                    TextureID = te.TextureID,
                    Color = te.RGBA
                };
                ret.Name = string.Format("Material{0}", AllMeterials.Count);
                AllMeterials.Add(ret);
            }

            return ret;
        }

        List<MaterialInfo> GetMaterials(FacetedMesh obj)
        {
            var ret = new List<MaterialInfo>();

            for (int face_num = 0; face_num < obj.Faces.Count; face_num++)
            {
                var te = obj.Faces[(int)face_num].TextureFace;
                if (SkipTransparentFaces && te.RGBA.A < 0.01f)
                {
                    continue;
                }
                var mat = GetMaterial(te);
                if (!ret.Contains(mat))
                {
                    ret.Add(mat);
                }
            }
            return ret;
        }

        List<int> GetFacesWithMaterial(FacetedMesh obj, MaterialInfo mat)
        {
            var ret = new List<int>();
            for (int face_num = 0; face_num < obj.Faces.Count; face_num++)
            {
                if (mat == GetMaterial(obj.Faces[(int)face_num].TextureFace))
                {
                    ret.Add(face_num);
                }
            }
            return ret;
        }

        void GenerateEffects(XmlNode effects)
        {
            // Effects (face color, alpha)
            foreach (var mat in AllMeterials)
            {
                var color = mat.Color;
                var effect = effects.AppendChild(Doc.CreateElement("effect"));
                effect.Attributes.Append(Doc.CreateAttribute("id")).InnerText = mat.Name + "-fx";
                var profile = effect.AppendChild(Doc.CreateElement("profile_COMMON"));
                string colladaName = null;
                if (ExportTextures)
                {
                    UUID textID = UUID.Zero;
                    int i = 0;
                    for (; i < Textures.Count; i++)
                    {
                        if (mat.TextureID == Textures[i])
                        {
                            textID = mat.TextureID;
                            break;
                        }
                    }

                    if (textID != UUID.Zero && TextureNames[i] != null)
                    {
                        colladaName = TextureNames[i] + "_" + ImageFormat.ToLower();
                        var newparam = profile.AppendChild(Doc.CreateElement("newparam"));
                        newparam.Attributes.Append(Doc.CreateAttribute("sid")).InnerText = colladaName + "-surface";
                        var surface = newparam.AppendChild(Doc.CreateElement("surface"));
                        surface.Attributes.Append(Doc.CreateAttribute("type")).InnerText = "2D";
                        surface.AppendChild(Doc.CreateElement("init_from")).InnerText = colladaName;
                        newparam = profile.AppendChild(Doc.CreateElement("newparam"));
                        newparam.Attributes.Append(Doc.CreateAttribute("sid")).InnerText = colladaName + "-sampler";
                        newparam.AppendChild(Doc.CreateElement("sampler2D"))
                            .AppendChild(Doc.CreateElement("source"))
                            .InnerText = colladaName + "-surface";
                    }

                }
                var t = profile.AppendChild(Doc.CreateElement("technique"));
                t.Attributes.Append(Doc.CreateAttribute("sid")).InnerText = "common";
                var phong = t.AppendChild(Doc.CreateElement("phong"));

                var diffuse = phong.AppendChild(Doc.CreateElement("diffuse"));
                // Only one <color> or <texture> can appear inside diffuse element
                if (colladaName != null)
                {
                    var txtr = diffuse.AppendChild(Doc.CreateElement("texture"));
                    txtr.Attributes.Append(Doc.CreateAttribute("texture")).InnerText = colladaName + "-sampler";
                    txtr.Attributes.Append(Doc.CreateAttribute("texcoord")).InnerText = colladaName;
                }
                else
                {
                    var diffuseColor = diffuse.AppendChild(Doc.CreateElement("color"));
                    diffuseColor.Attributes.Append(Doc.CreateAttribute("sid")).InnerText = "diffuse";
                    diffuseColor.InnerText = string.Format("{0} {1} {2} {3}",
                        color.R.ToString(invariant),
                        color.G.ToString(invariant),
                        color.B.ToString(invariant),
                        color.A.ToString(invariant));
                }

                phong.AppendChild(Doc.CreateElement("transparency"))
                    .AppendChild(Doc.CreateElement("float"))
                    .InnerText = color.A.ToString(invariant);

            }
        }

        void GenerateCollada()
        {
            AllMeterials.Clear();
            var root = ColladaInit();
            var images = root.AppendChild(Doc.CreateElement("library_images"));
            var geomLib = root.AppendChild(Doc.CreateElement("library_geometries"));
            var effects = root.AppendChild(Doc.CreateElement("library_effects"));
            var materials = root.AppendChild(Doc.CreateElement("library_materials"));
            var scene = root.AppendChild(Doc.CreateElement("library_visual_scenes"))
                .AppendChild(Doc.CreateElement("visual_scene"));
            scene.Attributes.Append(Doc.CreateAttribute("id")).InnerText = "Scene";
            scene.Attributes.Append(Doc.CreateAttribute("name")).InnerText = "Scene";
            if (ExportTextures)
            {
                GenerateImagesSection(images);
            }

            int prim_nr = 0;
            foreach (var obj in MeshedPrims)
            {
                int total_num_vertices = 0;
                string name = string.Format("prim{0}", prim_nr++);
                string geomID = name;

                var geom = geomLib.AppendChild(Doc.CreateElement("geometry"));
                geom.Attributes.Append(Doc.CreateAttribute("id")).InnerText = string.Format("{0}-{1}", geomID, "mesh");
                var mesh = geom.AppendChild(Doc.CreateElement("mesh"));

                List<float> position_data = new List<float>();
                List<float> normal_data = new List<float>();
                List<float> uv_data = new List<float>();

                int num_faces = obj.Faces.Count;

                for (int face_num = 0; face_num < num_faces; face_num++)
                {
                    var face = obj.Faces[face_num];
                    total_num_vertices += face.Vertices.Count;

                    for (int i = 0; i < face.Vertices.Count; i++)
                    {
                        var v = face.Vertices[i];
                        position_data.Add(v.Position.X);
                        position_data.Add(v.Position.Y);
                        position_data.Add(v.Position.Z);

                        normal_data.Add(v.Normal.X);
                        normal_data.Add(v.Normal.Y);
                        normal_data.Add(v.Normal.Z);

                        uv_data.Add(v.TexCoord.X);
                        uv_data.Add(v.TexCoord.Y);
                    }
                }

                AddSource(mesh, string.Format("{0}-{1}", geomID, "positions"), "XYZ", position_data);
                AddSource(mesh, string.Format("{0}-{1}", geomID, "normals"), "XYZ", normal_data);
                AddSource(mesh, string.Format("{0}-{1}", geomID, "map0"), "ST", uv_data);

                // Add the <vertices> element
                {
                    var verticesNode = mesh.AppendChild(Doc.CreateElement("vertices"));
                    verticesNode.Attributes.Append(Doc.CreateAttribute("id")).InnerText = string.Format("{0}-{1}", geomID, "vertices");
                    var verticesInput = verticesNode.AppendChild(Doc.CreateElement("input"));
                    verticesInput.Attributes.Append(Doc.CreateAttribute("semantic")).InnerText = "POSITION";
                    verticesInput.Attributes.Append(Doc.CreateAttribute("source")).InnerText = string.Format("#{0}-{1}", geomID, "positions");
                }

                var objMaterials = GetMaterials(obj);

                // Add triangles
                foreach (var objMaterial in objMaterials)
                {
                    AddPolygons(mesh, geomID, objMaterial.Name + "-material", obj, GetFacesWithMaterial(obj, objMaterial));
                }

                var node = scene.AppendChild(Doc.CreateElement("node"));
                node.Attributes.Append(Doc.CreateAttribute("type")).InnerText = "NODE";
                node.Attributes.Append(Doc.CreateAttribute("id")).InnerText = geomID;
                node.Attributes.Append(Doc.CreateAttribute("name")).InnerText = geomID;

                // Set tranform matrix (node position, rotation and scale)
                var matrix = node.AppendChild(Doc.CreateElement("matrix"));

                var srt = Radegast.Rendering.Math3D.CreateSRTMatrix(obj.Prim.Scale, obj.Prim.Rotation, obj.Prim.Position);
                string matrixVal = "";
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        matrixVal += srt[j * 4 + i].ToString(invariant) + " ";
                    }
                }
                matrix.InnerText = matrixVal.TrimEnd();

                // Geometry of the node
                var nodeGeometry = node.AppendChild(Doc.CreateElement("instance_geometry"));

                // Bind materials
                var tq = nodeGeometry.AppendChild(Doc.CreateElement("bind_material"))
                    .AppendChild(Doc.CreateElement("technique_common"));
                foreach (var objMaterial in objMaterials)
                {
                    var instanceMaterial = tq.AppendChild(Doc.CreateElement("instance_material"));
                    instanceMaterial.Attributes.Append(Doc.CreateAttribute("symbol")).InnerText = string.Format("{0}-{1}", objMaterial.Name, "material");
                    instanceMaterial.Attributes.Append(Doc.CreateAttribute("target")).InnerText = string.Format("#{0}-{1}", objMaterial.Name, "material");
                }

                nodeGeometry.Attributes.Append(Doc.CreateAttribute("url")).InnerText = string.Format("#{0}-{1}", geomID, "mesh");
            }

            GenerateEffects(effects);

            // Materials
            foreach (var objMaterial in AllMeterials)
            {
                var mat = materials.AppendChild(Doc.CreateElement("material"));
                mat.Attributes.Append(Doc.CreateAttribute("id")).InnerText = objMaterial.Name + "-material";
                var matEffect = mat.AppendChild(Doc.CreateElement("instance_effect"));
                matEffect.Attributes.Append(Doc.CreateAttribute("url")).InnerText = string.Format("#{0}-{1}", objMaterial.Name, "fx");
            }

            root.AppendChild(Doc.CreateElement("scene"))
                .AppendChild(Doc.CreateElement("instance_visual_scene"))
                .Attributes.Append(Doc.CreateAttribute("url")).InnerText = "#Scene";

        }

        string DocToString(XmlDocument doc)
        {
            string ret = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + Environment.NewLine;
            try
            {
                using (MemoryStream outs = new MemoryStream())
                {
                    using (XmlTextWriter writter = new XmlTextWriter(outs, Encoding.UTF8))
                    {
                        writter.Formatting = Formatting.Indented;
                        doc.WriteContentTo(writter);
                        writter.Flush();
                        outs.Flush();
                        outs.Position = 0;
                        using (StreamReader sr = new StreamReader(outs))
                        {
                            ret += sr.ReadToEnd();
                        }
                    }
                }
            }
            catch { }

            return ret;
        }
    }

    public class DAEStatutsEventArgs : EventArgs
    {
        public string Message;

        public DAEStatutsEventArgs(string message)
        {
            Message = message;
        }
    }
}
