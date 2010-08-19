// 
// Radegast Metaverse Client
// Copyright (c) 2009-2010, Radegast Development Team
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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using Tao.OpenGl;
using OpenMetaverse;
using OpenMetaverse.Rendering;
using OpenMetaverse.Assets;
using OpenMetaverse.Imaging;

// NOTE: Batches are divided by texture, fullbright, shiny, transparent, and glow

namespace Radegast
{

    public partial class frmPrimWorkshop : RadegastForm
    {
        #region Form Globals

        List<FacetedMesh> Prims = null;
        FacetedMesh CurrentPrim = null;
        ProfileFace? CurrentFace = null;

        bool DraggingTexture = false;
        bool Wireframe = false;
        int[] TexturePointers = new int[1];
        Dictionary<UUID, Image> Textures = new Dictionary<UUID, Image>();
        RadegastInstance instance;
        MeshmerizerR renderer;

        #endregion Form Globals

        public frmPrimWorkshop(RadegastInstance instance)
        {
            InitializeComponent();

            this.instance = instance;

            glControl.InitializeContexts();
            glControl.MouseWheel += new MouseEventHandler(glControl_MouseWheel);

            Gl.glShadeModel(Gl.GL_SMOOTH);
            Gl.glClearColor(0f, 0f, 0f, 0f);

            Gl.glClearDepth(1.0f);
            Gl.glEnable(Gl.GL_DEPTH_TEST);
            Gl.glEnable(Gl.GL_COLOR_MATERIAL);
            Gl.glDepthMask(Gl.GL_TRUE);
            Gl.glDepthFunc(Gl.GL_LEQUAL);
            Gl.glHint(Gl.GL_PERSPECTIVE_CORRECTION_HINT, Gl.GL_NICEST);
            Gl.glMatrixMode(Gl.GL_PROJECTION);

            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            TexturePointers[0] = 0;

            // Call the resizing function which sets up the GL drawing window
            // and will also invalidate the GL control
            glControl_Resize(null, null);

            renderer = new MeshmerizerR();
        }

        #region GLControl Callbacks

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();

            // Setup wireframe or solid fill drawing mode
            if (Wireframe)
                Gl.glPolygonMode(Gl.GL_FRONT_AND_BACK, Gl.GL_LINE);
            else
                Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL);

            Vector3 center = Vector3.Zero;

            Glu.gluLookAt(
                    center.X, (double)scrollZoom.Value * 0.1d + center.Y, center.Z,
                    center.X, center.Y, center.Z,
                    0d, 0d, 1d);

            // Push the world matrix
            Gl.glPushMatrix();

            Gl.glEnableClientState(Gl.GL_VERTEX_ARRAY);
            Gl.glEnableClientState(Gl.GL_TEXTURE_COORD_ARRAY);

            // World rotations
            Gl.glRotatef((float)scrollRoll.Value, 1f, 0f, 0f);
            Gl.glRotatef((float)scrollPitch.Value, 0f, 1f, 0f);
            Gl.glRotatef((float)scrollYaw.Value, 0f, 0f, 1f);

            if (Prims != null)
            {
                for (int i = 0; i < Prims.Count; i++)
                {
                    Primitive prim = Prims[i].Prim;

                    // Individual prim matrix
                    Gl.glPushMatrix();

                    // The root prim position is sim-relative, while child prim positions are
                    // parent-relative. We want to apply parent-relative translations but not
                    // sim-relative ones
                    if (Prims[i].Prim.ParentID != 0)
                    {
                        // Apply prim translation and rotation
                        Gl.glMultMatrixf(Math3D.CreateTranslationMatrix(prim.Position));
                        Gl.glMultMatrixf(Math3D.CreateRotationMatrix(prim.Rotation));
                    }

                    // Prim scaling
                    Gl.glScalef(prim.Scale.X, prim.Scale.Y, prim.Scale.Z);

                    // Draw the prim faces
                    for (int j = 0; j < Prims[i].Faces.Count; j++)
                    {
                        Primitive.TextureEntryFace teFace = Prims[i].Prim.Textures.FaceTextures[j];
                        if (teFace == null)
                            teFace = Prims[i].Prim.Textures.DefaultTexture;

                        //switch (teFace.Shiny)
                        //{
                        //    case Shininess.High:
                        //        Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 128f);
                        //        break;

                        //    case Shininess.Medium:
                        //        Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 70f);
                        //        break;

                        //    case Shininess.Low:
                        //        Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 20f);
                        //        break;


                        //    case Shininess.None:
                        //    default:
                        //        Gl.glMaterialf(Gl.GL_FRONT, Gl.GL_SHININESS, 0f);
                        //        break;
                        //}

                        //Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_AMBIENT, new float[4] { 0.7f, 0.7f, 0.7f, 1.0f });
                        //Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_DIFFUSE, new float[4] { 0.1f, 0.5f, 0.8f, 1.0f });
                        //Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_SPECULAR, new float[4] {1.0f, 1.0f, 1.0f, 1.0f});
                        //Gl.glMaterialfv(Gl.GL_FRONT, Gl.GL_EMISSION, new float[4] { 0.0f, 0.0f, 0.0f, 1.0f });

                        Gl.glColor4f(teFace.RGBA.R, teFace.RGBA.G, teFace.RGBA.B, teFace.RGBA.A);
                        #region Texturing

                        Face face = Prims[i].Faces[j];
                        FaceData data = (FaceData)face.UserData;

                        if (data.TexturePointer != 0)
                        {
                            // Set the color to solid white so the texture is not altered
                            //Gl.glColor3f(1f, 1f, 1f);
                            // Enable texturing for this face
                            Gl.glEnable(Gl.GL_TEXTURE_2D);
                        }
                        else
                        {
                            Gl.glDisable(Gl.GL_TEXTURE_2D);
                        }

                        // Bind the texture
                        Gl.glBindTexture(Gl.GL_TEXTURE_2D, data.TexturePointer);

                        #endregion Texturing

                        Gl.glTexCoordPointer(2, Gl.GL_FLOAT, 0, data.TexCoords);
                        Gl.glVertexPointer(3, Gl.GL_FLOAT, 0, data.Vertices);
                        Gl.glDrawElements(Gl.GL_TRIANGLES, data.Indices.Length, Gl.GL_UNSIGNED_SHORT, data.Indices);
                    }

                    // Pop the prim matrix
                    Gl.glPopMatrix();
                }
            }

            // Pop the world matrix
            Gl.glPopMatrix();

            Gl.glDisableClientState(Gl.GL_TEXTURE_COORD_ARRAY);
            Gl.glDisableClientState(Gl.GL_VERTEX_ARRAY);

            Gl.glFlush();
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            Gl.glClearColor(0.39f, 0.58f, 0.93f, 1.0f);

            Gl.glViewport(0, 0, glControl.Width, glControl.Height);

            Gl.glPushMatrix();
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Glu.gluPerspective(50.0d, 1.0d, 0.1d, 50d);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glPopMatrix();
        }

        void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                scrollZoom.Value += e.Delta / 15;
            }
            catch (Exception) { }

            glControl_Resize(null, null);
            glControl.Invalidate();
        }
        #endregion GLControl Callbacks

        public void loadPrims(List<Primitive> primList)
        {
            ThreadPool.QueueUserWorkItem((object sync) =>
                {
                    loadPrimsBlocking(primList);
                }
            );
        }

        private void loadPrimsBlocking(List<Primitive> primList)
        {
            Prims = null;
            Prims = new List<FacetedMesh>(primList.Count);

            for (int i = 0; i < primList.Count; i++)
            {
                FacetedMesh mesh = null;
                Primitive prim = primList[i];
                if (prim.Textures == null)
                    continue;

                try
                {
                    if (prim.Sculpt != null && prim.Sculpt.SculptTexture != UUID.Zero)
                    {
                        Image img = null;
                        if (!LoadTexture(primList[i].Sculpt.SculptTexture, ref img, true))
                            continue;
                        mesh = renderer.GenerateFacetedSculptMesh(prim, (Bitmap)img, DetailLevel.Highest);
                        img.Dispose();
                    }
                    else
                    {
                        mesh = renderer.GenerateFacetedMesh(prim, DetailLevel.Highest);
                    }
                }
                catch
                {
                    continue;
                }

                // Create a FaceData struct for each face that stores the 3D data
                // in a Tao.OpenGL friendly format
                for (int j = 0; j < mesh.Faces.Count; j++)
                {
                    Face face = mesh.Faces[j];
                    FaceData data = new FaceData();

                    // Vertices for this face
                    data.Vertices = new float[face.Vertices.Count * 3];
                    for (int k = 0; k < face.Vertices.Count; k++)
                    {
                        data.Vertices[k * 3 + 0] = face.Vertices[k].Position.X;
                        data.Vertices[k * 3 + 1] = face.Vertices[k].Position.Y;
                        data.Vertices[k * 3 + 2] = face.Vertices[k].Position.Z;
                    }

                    // Indices for this face
                    data.Indices = face.Indices.ToArray();

                    // Texture transform for this face
                    Primitive.TextureEntryFace teFace = prim.Textures.GetFace((uint)j);
                    renderer.TransformTexCoords(face.Vertices, face.Center, teFace);

                    // Texcoords for this face
                    data.TexCoords = new float[face.Vertices.Count * 2];
                    for (int k = 0; k < face.Vertices.Count; k++)
                    {
                        data.TexCoords[k * 2 + 0] = face.Vertices[k].TexCoord.X;
                        data.TexCoords[k * 2 + 1] = face.Vertices[k].TexCoord.Y;
                    }

                    // Texture for this face
                    if (LoadTexture(teFace.TextureID, ref data.Texture, false))
                    {
                        if (IsHandleCreated)
                        {
                            Invoke(new MethodInvoker(() =>
                            {
                                Bitmap bitmap = new Bitmap(data.Texture);
                                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                                Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                                BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);

                                Gl.glGenTextures(1, out data.TexturePointer);
                                Gl.glBindTexture(Gl.GL_TEXTURE_2D, data.TexturePointer);

                                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MIN_FILTER, Gl.GL_LINEAR_MIPMAP_LINEAR);
                                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_MAG_FILTER, Gl.GL_LINEAR);
                                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_S, Gl.GL_REPEAT);
                                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_TEXTURE_WRAP_T, Gl.GL_REPEAT);
                                Gl.glTexParameteri(Gl.GL_TEXTURE_2D, Gl.GL_GENERATE_MIPMAP, Gl.GL_TRUE);
                                Glu.gluBuild2DMipmaps(Gl.GL_TEXTURE_2D, Gl.GL_RGB8, bitmap.Width, bitmap.Height, Gl.GL_BGR, Gl.GL_UNSIGNED_BYTE, bitmapData.Scan0);

                                bitmap.UnlockBits(bitmapData);
                                bitmap.Dispose();
                            }
                            ));
                        }
                    }

                    // Set the UserData for this face to our FaceData struct
                    face.UserData = data;
                    mesh.Faces[j] = face;
                }

                Prims.Add(mesh);

                if (IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() =>
                    {
                        glControl.Invalidate();
                    }));
                }

            }
        }

        private bool LoadTexture(UUID textureID, ref Image texture, bool removeAlpha)
        {
            ManualResetEvent gotImage = new ManualResetEvent(false);
            Image img = null;

            if (Textures.ContainsKey(textureID))
            {
                texture = Textures[textureID];
                return true;
            }

            try
            {
                gotImage.Reset();
                instance.Client.Assets.RequestImage(textureID, (TextureRequestState state, AssetTexture assetTexture) =>
                    {
                        if (state == TextureRequestState.Finished)
                        {
                            ManagedImage mi;
                            OpenJPEG.DecodeToImage(assetTexture.AssetData, out mi);
                            
                            if (removeAlpha)
                            {
                                if ((mi.Channels & ManagedImage.ImageChannels.Alpha) != 0)
                                {
                                    mi.ConvertChannels(mi.Channels & ~ManagedImage.ImageChannels.Alpha);
                                }
                            }

                            img = LoadTGAClass.LoadTGA(new MemoryStream(mi.ExportTGA()));
                        }
                        gotImage.Set();
                    }
                );
                gotImage.WaitOne(30 * 1000, false);
                if (img != null)
                {
                    Textures.Add(textureID, img);
                    texture = img;
                    Wireframe = false;
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Logger.Log(e.Message, Helpers.LogLevel.Error, instance.Client, e);
                return false;
            }
        }

        private void oBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "OBJ files (*.obj)|*.obj";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (!MeshToOBJ.MeshesToOBJ(Prims, dialog.FileName))
                {
                    MessageBox.Show("Failed to save file " + dialog.FileName +
                        ". Ensure that you have permission to write to that file and it is currently not in use");
                }
            }
        }

        #region Scrollbar Callbacks

        private void scroll_ValueChanged(object sender, EventArgs e)
        {
            glControl.Invalidate();
        }

        private void scrollZoom_ValueChanged(object sender, EventArgs e)
        {
            glControl_Resize(null, null);
            glControl.Invalidate();
        }

        #endregion Scrollbar Callbacks

        #region PictureBox Callbacks

        private void picTexture_MouseDown(object sender, MouseEventArgs e)
        {
            DraggingTexture = true;
        }

        private void picTexture_MouseUp(object sender, MouseEventArgs e)
        {
            DraggingTexture = false;
        }

        private void picTexture_MouseLeave(object sender, EventArgs e)
        {
            DraggingTexture = false;
        }

        private void picTexture_MouseMove(object sender, MouseEventArgs e)
        {
            if (DraggingTexture)
            {
                // What is the current action?
                // None, DraggingEdge, DraggingCorner, DraggingWhole
            }
            else
            {
                // Check if the mouse is close to the edge or corner of a selection
                // rectangle

                // If so, change the cursor accordingly
            }
        }

        private void picTexture_Paint(object sender, PaintEventArgs e)
        {
            // Draw the current selection rectangles
        }

        #endregion PictureBox Callbacks

        private void cboPrim_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPrim = (FacetedMesh)cboPrim.Items[cboPrim.SelectedIndex];
            PopulateFaceCombobox();

            glControl.Invalidate();
        }

        private void cboFace_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentFace = (ProfileFace)cboFace.Items[cboFace.SelectedIndex];

            glControl.Invalidate();
        }

        private void PopulatePrimCombobox()
        {
            cboPrim.Items.Clear();

            if (Prims != null)
            {
                for (int i = 0; i < Prims.Count; i++)
                    cboPrim.Items.Add(Prims[i]);
            }

            if (cboPrim.Items.Count > 0)
                cboPrim.SelectedIndex = 0;
        }

        private void PopulateFaceCombobox()
        {
            cboFace.Items.Clear();

            if (CurrentPrim != null)
            {
                for (int i = 0; i < CurrentPrim.Profile.Faces.Count; i++)
                    cboFace.Items.Add(CurrentPrim.Profile.Faces[i]);
            }

            if (cboFace.Items.Count > 0)
                cboFace.SelectedIndex = 0;
        }

    }

    public struct FaceData
    {
        public float[] Vertices;
        public ushort[] Indices;
        public float[] TexCoords;
        public int TexturePointer;
        public System.Drawing.Image Texture;
        // TODO: Normals / binormals?
    }

    public static class Render
    {
        public static IRendering Plugin;
    }

    public static class MeshToOBJ
    {
        public static bool MeshesToOBJ(List<FacetedMesh> meshes, string filename)
        {
            StringBuilder obj = new StringBuilder();
            StringBuilder mtl = new StringBuilder();

            FileInfo objFileInfo = new FileInfo(filename);

            string mtlFilename = objFileInfo.FullName.Substring(objFileInfo.DirectoryName.Length + 1,
                objFileInfo.FullName.Length - (objFileInfo.DirectoryName.Length + 1) - 4) + ".mtl";

            obj.AppendLine("# Created by libprimrender");
            obj.AppendLine("mtllib ./" + mtlFilename);
            obj.AppendLine();

            mtl.AppendLine("# Created by libprimrender");
            mtl.AppendLine();

            for (int i = 0; i < meshes.Count; i++)
            {
                FacetedMesh mesh = meshes[i];

                for (int j = 0; j < mesh.Faces.Count; j++)
                {
                    Face face = mesh.Faces[j];

                    if (face.Vertices.Count > 2)
                    {
                        string mtlName = String.Format("material{0}-{1}", i, face.ID);
                        Primitive.TextureEntryFace tex = face.TextureFace;
                        string texName = tex.TextureID.ToString() + ".tga";

                        // FIXME: Convert the source to TGA (if needed) and copy to the destination

                        float shiny = 0.00f;
                        switch (tex.Shiny)
                        {
                            case Shininess.High:
                                shiny = 1.00f;
                                break;
                            case Shininess.Medium:
                                shiny = 0.66f;
                                break;
                            case Shininess.Low:
                                shiny = 0.33f;
                                break;
                        }

                        obj.AppendFormat("g face{0}-{1}{2}", i, face.ID, Environment.NewLine);

                        mtl.AppendLine("newmtl " + mtlName);
                        mtl.AppendFormat("Ka {0} {1} {2}{3}", tex.RGBA.R, tex.RGBA.G, tex.RGBA.B, Environment.NewLine);
                        mtl.AppendFormat("Kd {0} {1} {2}{3}", tex.RGBA.R, tex.RGBA.G, tex.RGBA.B, Environment.NewLine);
                        //mtl.AppendFormat("Ks {0} {1} {2}{3}");
                        mtl.AppendLine("Tr " + tex.RGBA.A);
                        mtl.AppendLine("Ns " + shiny);
                        mtl.AppendLine("illum 1");
                        if (tex.TextureID != UUID.Zero && tex.TextureID != Primitive.TextureEntry.WHITE_TEXTURE)
                            mtl.AppendLine("map_Kd ./" + texName);
                        mtl.AppendLine();

                        // Write the vertices, texture coordinates, and vertex normals for this side
                        for (int k = 0; k < face.Vertices.Count; k++)
                        {
                            Vertex vertex = face.Vertices[k];

                            #region Vertex

                            Vector3 pos = vertex.Position;

                            // Apply scaling
                            pos *= mesh.Prim.Scale;

                            // Apply rotation
                            pos *= mesh.Prim.Rotation;

                            // The root prim position is sim-relative, while child prim positions are
                            // parent-relative. We want to apply parent-relative translations but not
                            // sim-relative ones
                            if (mesh.Prim.ParentID != 0)
                                pos += mesh.Prim.Position;

                            obj.AppendFormat("v {0} {1} {2}{3}", pos.X, pos.Y, pos.Z, Environment.NewLine);

                            #endregion Vertex

                            #region Texture Coord

                            obj.AppendFormat("vt {0} {1}{2}", vertex.TexCoord.X, vertex.TexCoord.Y,
                                Environment.NewLine);

                            #endregion Texture Coord

                            #region Vertex Normal

                            // HACK: Sometimes normals are getting set to <NaN,NaN,NaN>
                            if (!Single.IsNaN(vertex.Normal.X) && !Single.IsNaN(vertex.Normal.Y) && !Single.IsNaN(vertex.Normal.Z))
                                obj.AppendFormat("vn {0} {1} {2}{3}", vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z,
                                    Environment.NewLine);
                            else
                                obj.AppendLine("vn 0.0 1.0 0.0");

                            #endregion Vertex Normal
                        }

                        obj.AppendFormat("# {0} vertices{1}", face.Vertices.Count, Environment.NewLine);
                        obj.AppendLine();
                        obj.AppendLine("usemtl " + mtlName);

                        #region Elements

                        // Write all of the faces (triangles) for this side
                        for (int k = 0; k < face.Indices.Count / 3; k++)
                        {
                            obj.AppendFormat("f -{0}/-{0}/-{0} -{1}/-{1}/-{1} -{2}/-{2}/-{2}{3}",
                                face.Vertices.Count - face.Indices[k * 3 + 0],
                                face.Vertices.Count - face.Indices[k * 3 + 1],
                                face.Vertices.Count - face.Indices[k * 3 + 2],
                                Environment.NewLine);
                        }

                        obj.AppendFormat("# {0} elements{1}", face.Indices.Count / 3, Environment.NewLine);
                        obj.AppendLine();

                        #endregion Elements
                    }
                }
            }

            try
            {
                File.WriteAllText(filename, obj.ToString());
                File.WriteAllText(mtlFilename, mtl.ToString());
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }

    public static class Math3D
    {
        // Column-major:
        // |  0  4  8 12 |
        // |  1  5  9 13 |
        // |  2  6 10 14 |
        // |  3  7 11 15 |

        public static float[] CreateTranslationMatrix(Vector3 v)
        {
            float[] mat = new float[16];

            mat[12] = v.X;
            mat[13] = v.Y;
            mat[14] = v.Z;
            mat[0] = mat[5] = mat[10] = mat[15] = 1;

            return mat;
        }

        public static float[] CreateRotationMatrix(Quaternion q)
        {
            float[] mat = new float[16];

            // Transpose the quaternion (don't ask me why)
            q.X = q.X * -1f;
            q.Y = q.Y * -1f;
            q.Z = q.Z * -1f;

            float x2 = q.X + q.X;
            float y2 = q.Y + q.Y;
            float z2 = q.Z + q.Z;
            float xx = q.X * x2;
            float xy = q.X * y2;
            float xz = q.X * z2;
            float yy = q.Y * y2;
            float yz = q.Y * z2;
            float zz = q.Z * z2;
            float wx = q.W * x2;
            float wy = q.W * y2;
            float wz = q.W * z2;

            mat[0] = 1.0f - (yy + zz);
            mat[1] = xy - wz;
            mat[2] = xz + wy;
            mat[3] = 0.0f;

            mat[4] = xy + wz;
            mat[5] = 1.0f - (xx + zz);
            mat[6] = yz - wx;
            mat[7] = 0.0f;

            mat[8] = xz - wy;
            mat[9] = yz + wx;
            mat[10] = 1.0f - (xx + yy);
            mat[11] = 0.0f;

            mat[12] = 0.0f;
            mat[13] = 0.0f;
            mat[14] = 0.0f;
            mat[15] = 1.0f;

            return mat;
        }

        public static float[] CreateScaleMatrix(Vector3 v)
        {
            float[] mat = new float[16];

            mat[0] = v.X;
            mat[5] = v.Y;
            mat[10] = v.Z;
            mat[15] = 1;

            return mat;
        }
    }
}
