// 
// Radegast Metaverse Client
// Copyright (c) 2009-2011, Radegast Development Team
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
using OpenTK.Graphics.OpenGL;
using OpenMetaverse;
using OpenMetaverse.Rendering;
using OpenMetaverse.Assets;
using OpenMetaverse.Imaging;
using OpenMetaverse.StructuredData;

// NOTE: Batches are divided by texture, fullbright, shiny, transparent, and glow

namespace Radegast
{

    public partial class frmPrimWorkshop : RadegastForm
    {
        #region Form Globals

        public OpenTK.GLControl glControl = null;

        List<FacetedMesh> Prims = null;

        bool Wireframe = false;
        public bool RenderingEnabled = false;

        int[] TexturePointers = new int[1];
        Dictionary<UUID, Image> Textures = new Dictionary<UUID, Image>();
        RadegastInstance instance;
        MeshmerizerR renderer;

        #endregion Form Globals

        public frmPrimWorkshop(RadegastInstance instance)
            : base(instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmPrimWorkshop_Disposed);
            AutoSavePosition = true;

            this.instance = instance;

            TexturePointers[0] = 0;

            renderer = new MeshmerizerR();

            Client.Objects.TerseObjectUpdate += new EventHandler<TerseObjectUpdateEventArgs>(Objects_TerseObjectUpdate);
            Client.Objects.ObjectUpdate += new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
        }

        public void SetupGLControl()
        {
            if (glControl != null)
                glControl.Dispose();
            glControl = null;

            OpenTK.Graphics.GraphicsMode mode = null;

            try
            {
                for (int aa = 0; aa <= 8; aa += 2)
                {
                    var testMode = new OpenTK.Graphics.GraphicsMode(OpenTK.DisplayDevice.Default.BitsPerPixel, 16, 0, aa);
                    if (testMode.Samples == aa)
                    {
                        mode = testMode;
                    }
                }
            }
            catch {}

            if (mode == null)
            {
                Logger.Log("Failed to initialize OpenGL control", Helpers.LogLevel.Error, Client);
                return;
            }

            Logger.Log("Initializing OpenGL mode: " + mode.ToString(), Helpers.LogLevel.Info);
            try
            {
                glControl = new OpenTK.GLControl(mode);
            }
            catch
            {
                glControl = null;
            }

            if (glControl == null) return;

            glControl.Paint += glControl_Paint;
            glControl.Resize += glControl_Resize;
            glControl.MouseDown += glControl_MouseDown;
            glControl.MouseUp += glControl_MouseUp;
            glControl.MouseMove += glControl_MouseMove;
            glControl.MouseWheel += glControl_MouseWheel;
            glControl.Load += new EventHandler(glControl_Load);
            glControl.Dock = DockStyle.Fill;
            Controls.Add(glControl);
        }

        void glControl_Load(object sender, EventArgs e)
        {
            try
            {
                GL.ShadeModel(ShadingModel.Smooth);
                GL.ClearColor(0f, 0f, 0f, 0f);

                //GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });

                GL.Enable(EnableCap.Lighting);
                GL.Enable(EnableCap.Light0);
                GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.5f, 0.5f, 0.5f, 1f });
                GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 0.3f, 0.3f, 0.3f, 1f });
                GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0.8f, 0.8f, 0.8f, 1.0f });
                GL.Light(LightName.Light0, LightParameter.Position, lightPos);

                GL.ClearDepth(1.0d);
                GL.Enable(EnableCap.DepthTest);
                GL.Enable(EnableCap.ColorMaterial);
                GL.Enable(EnableCap.CullFace);
                GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.AmbientAndDiffuse);
                GL.ColorMaterial(MaterialFace.Front, ColorMaterialParameter.Specular);

                GL.DepthMask(true);
                GL.DepthFunc(DepthFunction.Lequal);
                GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
                GL.MatrixMode(MatrixMode.Projection);

                GL.Enable(EnableCap.Blend);
                GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
                RenderingEnabled = true;
                // Call the resizing function which sets up the GL drawing window
                // and will also invalidate the GL control
                glControl_Resize(null, null);
            }
            catch (Exception ex)
            {
                RenderingEnabled = false;
                Logger.Log("Failed to initialize OpenGL control", Helpers.LogLevel.Warning, Client, ex);
            }
        }


        void frmPrimWorkshop_Disposed(object sender, EventArgs e)
        {
            Client.Objects.TerseObjectUpdate -= new EventHandler<TerseObjectUpdateEventArgs>(Objects_TerseObjectUpdate);
            Client.Objects.ObjectUpdate -= new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
        }

        void Objects_TerseObjectUpdate(object sender, TerseObjectUpdateEventArgs e)
        {
            if (Prims != null && null != Prims.Find(fm => fm.Prim.LocalID == e.Update.LocalID))
            {
                SafeInvalidate();
            }
        }

        void Objects_ObjectUpdate(object sender, PrimEventArgs e)
        {
            if (Prims != null && null != Prims.Find(fm => fm.Prim.LocalID == e.Prim.LocalID))
            {
                SafeInvalidate();
            }
        }

        void SafeInvalidate()
        {
            if (InvokeRequired)
            {
                if (!instance.MonoRuntime || IsHandleCreated)
                {
                    BeginInvoke(new MethodInvoker(() => SafeInvalidate()));
                }
                return;
            }

            glControl.Invalidate();

        }

        #region GLControl Callbacks

        Vector3 Center = Vector3.Zero;

        private void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            // Setup wireframe or solid fill drawing mode
            if (Wireframe)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            //var mLookAt = OpenTK.Matrix4d.LookAt(
            //        center.X, (double)scrollZoom.Value * 0.1d + center.Y, center.Z,
            //        center.X, center.Y, center.Z,
            //        0d, 0d, 1d);
            //GL.MultMatrix(ref mLookAt);
            OpenTK.Graphics.Glu.LookAt(
                    Center.X, (double)scrollZoom.Value * 0.1d + Center.Y, Center.Z,
                    Center.X, Center.Y, Center.Z,
                    0d, 0d, 1d);
            //GL.Light(LightName.Light0, LightParameter.Position, lightPos);

            // Push the world matrix
            GL.PushMatrix();

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            // World rotations
            GL.Rotate((float)scrollRoll.Value, 1f, 0f, 0f);
            GL.Rotate((float)scrollPitch.Value, 0f, 1f, 0f);
            GL.Rotate((float)scrollYaw.Value, 0f, 0f, 1f);

            if (Prims != null)
            {
                for (int i = 0; i < Prims.Count; i++)
                {
                    Primitive prim = Prims[i].Prim;

                    // Individual prim matrix
                    GL.PushMatrix();

                    if (Prims[i].Prim.ParentID != 0)
                    {
                        var parent = Prims.Find(fm => fm.Prim.LocalID == Prims[i].Prim.ParentID);

                        if (parent != null)
                        {
                            // Apply prim translation and rotation relative to the root prim
                            GL.MultMatrix(Math3D.CreateRotationMatrix(parent.Prim.Rotation));
                            //GL.MultMatrixf(Math3D.CreateTranslationMatrix(parent.Prim.Position));
                        }

                        // Prim roation relative to root
                        GL.MultMatrix(Math3D.CreateTranslationMatrix(prim.Position));
                    }

                    // Prim roation
                    GL.MultMatrix(Math3D.CreateRotationMatrix(prim.Rotation));

                    // Prim scaling
                    GL.Scale(prim.Scale.X, prim.Scale.Y, prim.Scale.Z);

                    // Draw the prim faces
                    for (int j = 0; j < Prims[i].Faces.Count; j++)
                    {
                        Primitive.TextureEntryFace teFace = Prims[i].Prim.Textures.FaceTextures[j];
                        if (teFace == null)
                            teFace = Prims[i].Prim.Textures.DefaultTexture;

                        switch (teFace.Shiny)
                        {
                            case Shininess.High:
                                GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 128f);
                                break;

                            case Shininess.Medium:
                                GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 96f);
                                break;

                            case Shininess.Low:
                                GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 24f);
                                break;


                            case Shininess.None:
                            default:
                                GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 0f);
                                break;
                        }

                        var faceColor = new float[] { teFace.RGBA.R, teFace.RGBA.G, teFace.RGBA.B, teFace.RGBA.A };

                        GL.Color4(faceColor);
                        GL.Material(MaterialFace.Front, MaterialParameter.AmbientAndDiffuse, faceColor);
                        GL.Material(MaterialFace.Front, MaterialParameter.Specular, faceColor);
                        #region Texturing

                        Face face = Prims[i].Faces[j];
                        FaceData data = (FaceData)face.UserData;

                        if (data.TexturePointer != 0)
                        {
                            GL.Enable(EnableCap.Texture2D);
                        }
                        else
                        {
                            GL.Disable(EnableCap.Texture2D);
                        }

                        // Bind the texture
                        GL.BindTexture(TextureTarget.Texture2D, data.TexturePointer);

                        #endregion Texturing
                        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, data.TexCoords);
                        GL.VertexPointer(3, VertexPointerType.Float, 0, data.Vertices);
                        GL.NormalPointer(NormalPointerType.Float, 0, data.Normals);
                        GL.DrawElements(BeginMode.Triangles, data.Indices.Length, DrawElementsType.UnsignedShort, data.Indices);
                    }

                    // Pop the prim matrix
                    GL.PopMatrix();
                }
            }

            // Pop the world matrix
            GL.PopMatrix();

            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);

            GL.Flush();
        }

        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            if (!RenderingEnabled) return;

            Render();

            glControl.SwapBuffers();
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            if (!RenderingEnabled) return;

            GL.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);

            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            double dAspRat = (double)glControl.Width / (double)glControl.Height;

#pragma warning disable 0618
            OpenTK.Graphics.Glu.Perspective(50.0d, dAspRat, 0.1d, 50d);
#pragma warning restore 0618
            //var mPerspective = OpenTK.Matrix4d.Perspective(50.0d, dAspRat, 0.1d, 50d);
            //GL.MultMatrix(ref mPerspective);


            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }

        void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            int newVal = scrollZoom.Value + e.Delta / 10;
            if (newVal < scrollZoom.Minimum) newVal = scrollZoom.Minimum;
            if (newVal > scrollZoom.Maximum) newVal = scrollZoom.Maximum;
            if (scrollZoom.Value != newVal)
            {
                scrollZoom.Value = newVal;
                glControl_Resize(null, null);
                SafeInvalidate();
            }
        }
        #endregion GLControl Callbacks

        public void loadPrims(List<Primitive> primList)
        {
            if (!RenderingEnabled) return;

            ThreadPool.QueueUserWorkItem((object sync) =>
                {
                    loadPrimsBlocking(primList);
                }
            );
        }

        public FacetedMesh GenerateFacetedMesh(Primitive prim, OSDMap MeshData, DetailLevel LOD)
        {
            FacetedMesh ret = new FacetedMesh();

            ret.Faces = new List<Face>();
            ret.Prim = prim;
            ret.Profile = new Profile();
            ret.Profile.Faces = new List<ProfileFace>();
            ret.Profile.Positions = new List<Vector3>();
            ret.Path = new OpenMetaverse.Rendering.Path();
            ret.Path.Points = new List<PathPoint>();

            try
            {
                OSD facesOSD = null;

                switch (LOD)
                {
                    default:
                    case DetailLevel.Highest:
                        facesOSD = MeshData["high_lod"];
                        break;

                    case DetailLevel.High:
                        facesOSD = MeshData["medium_lod"];
                        break;

                    case DetailLevel.Medium:
                        facesOSD = MeshData["low_lod"];
                        break;

                    case DetailLevel.Low:
                        facesOSD = MeshData["lowest_lod"];
                        break;
                }

                if (facesOSD == null || !(facesOSD is OSDArray))
                {
                    return ret;
                }

                OSDArray decodedMeshOsdArray = (OSDArray)facesOSD;



                for (int faceNr = 0; faceNr < decodedMeshOsdArray.Count; faceNr++)
                {
                    OSD subMeshOsd = decodedMeshOsdArray[faceNr];
                    Face oface = new Face();
                    oface.ID = faceNr;
                    oface.Vertices = new List<Vertex>();
                    oface.Indices = new List<ushort>();
                    oface.TextureFace = prim.Textures.GetFace((uint)faceNr);

                    if (subMeshOsd is OSDMap)
                    {
                        OSDMap subMeshMap = (OSDMap)subMeshOsd;

                        Vector3 posMax = ((OSDMap)subMeshMap["PositionDomain"])["Max"];
                        Vector3 posMin = ((OSDMap)subMeshMap["PositionDomain"])["Min"];

                        Vector2 texPosMax = ((OSDMap)subMeshMap["TexCoord0Domain"])["Max"];
                        Vector2 texPosMin = ((OSDMap)subMeshMap["TexCoord0Domain"])["Min"];


                        byte[] posBytes = subMeshMap["Position"];
                        byte[] norBytes = subMeshMap["Normal"];
                        byte[] texBytes = subMeshMap["TexCoord0"];

                        for (int i = 0; i < posBytes.Length; i += 6)
                        {
                            ushort uX = Utils.BytesToUInt16(posBytes, i);
                            ushort uY = Utils.BytesToUInt16(posBytes, i + 2);
                            ushort uZ = Utils.BytesToUInt16(posBytes, i + 4);

                            Vertex vx = new Vertex();

                            vx.Position = new Vector3(
                                Utils.UInt16ToFloat(uX, posMin.X, posMax.X),
                                Utils.UInt16ToFloat(uY, posMin.Y, posMax.Y),
                                Utils.UInt16ToFloat(uZ, posMin.Z, posMax.Z));

                            ushort nX = Utils.BytesToUInt16(norBytes, i);
                            ushort nY = Utils.BytesToUInt16(norBytes, i + 2);
                            ushort nZ = Utils.BytesToUInt16(norBytes, i + 4);

                            vx.Normal = new Vector3(
                                Utils.UInt16ToFloat(nX, posMin.X, posMax.X),
                                Utils.UInt16ToFloat(nY, posMin.Y, posMax.Y),
                                Utils.UInt16ToFloat(nZ, posMin.Z, posMax.Z));

                            var vertexIndexOffset = oface.Vertices.Count * 4;

                            if (texBytes != null && texBytes.Length >= vertexIndexOffset + 4)
                            {
                                ushort tX = Utils.BytesToUInt16(texBytes, vertexIndexOffset);
                                ushort tY = Utils.BytesToUInt16(texBytes, vertexIndexOffset + 2);

                                vx.TexCoord = new Vector2(
                                    Utils.UInt16ToFloat(tX, texPosMin.X, texPosMax.X),
                                    Utils.UInt16ToFloat(tY, texPosMin.Y, texPosMax.Y));
                            }

                            oface.Vertices.Add(vx);
                        }

                        byte[] triangleBytes = subMeshMap["TriangleList"];
                        for (int i = 0; i < triangleBytes.Length; i += 6)
                        {
                            ushort v1 = (ushort)(Utils.BytesToUInt16(triangleBytes, i));
                            oface.Indices.Add(v1);
                            ushort v2 = (ushort)(Utils.BytesToUInt16(triangleBytes, i + 2));
                            oface.Indices.Add(v2);
                            ushort v3 = (ushort)(Utils.BytesToUInt16(triangleBytes, i + 4));
                            oface.Indices.Add(v3);
                        }
                    }
                    ret.Faces.Add(oface);
                }

            }
            catch (Exception ex)
            {
                Logger.Log("Failed to decode mesh asset: " + ex.Message, Helpers.LogLevel.Warning, Client);
            }
            return ret;
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
                        if (prim.Sculpt.Type != SculptType.Mesh)
                        { // Regular sculptie
                            Image img = null;
                            if (!LoadTexture(primList[i].Sculpt.SculptTexture, ref img, true))
                                continue;
                            mesh = renderer.GenerateFacetedSculptMesh(prim, (Bitmap)img, DetailLevel.Highest);
                        }
                        else
                        { // Mesh
                            AutoResetEvent gotMesh = new AutoResetEvent(false);
                            bool meshSuccess = false;

                            Client.Assets.RequestMesh(prim.Sculpt.SculptTexture, (success, meshAsset) =>
                                {
                                    if (!success || !meshAsset.Decode())
                                    {
                                        Logger.Log("Failed to fetch or decode the mesh asset", Helpers.LogLevel.Warning, Client);
                                    }
                                    else
                                    {
                                        mesh = GenerateFacetedMesh(prim, meshAsset.MeshData, DetailLevel.Highest);
                                        meshSuccess = true;
                                    }
                                    gotMesh.Set();
                                });

                            if (!gotMesh.WaitOne(20 * 1000, false)) continue;
                            if (!meshSuccess) continue;
                        }
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
                // in a OpenGL friendly format
                for (int j = 0; j < mesh.Faces.Count; j++)
                {
                    Face face = mesh.Faces[j];
                    FaceData data = new FaceData();

                    // Vertices for this face
                    data.Vertices = new float[face.Vertices.Count * 3];
                    data.Normals = new float[face.Vertices.Count * 3];
                    for (int k = 0; k < face.Vertices.Count; k++)
                    {
                        data.Vertices[k * 3 + 0] = face.Vertices[k].Position.X;
                        data.Vertices[k * 3 + 1] = face.Vertices[k].Position.Y;
                        data.Vertices[k * 3 + 2] = face.Vertices[k].Position.Z;

                        data.Normals[k * 3 + 0] = face.Vertices[k].Normal.X;
                        data.Normals[k * 3 + 1] = face.Vertices[k].Normal.Y;
                        data.Normals[k * 3 + 2] = face.Vertices[k].Normal.Z;
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

                    ThreadPool.QueueUserWorkItem(sync =>
                        {
                            // Texture for this face
                            if (LoadTexture(teFace.TextureID, ref data.Texture, false))
                            {
                                if (IsHandleCreated)
                                {
                                    Invoke(new MethodInvoker(() =>
                                    {
                                        GL.GenTextures(1, out data.TexturePointer);
                                        GL.BindTexture(TextureTarget.Texture2D, data.TexturePointer);

                                        Bitmap bitmap = new Bitmap(data.Texture);
                                        bool hasAlpha;
                                        if (data.Texture.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                                        {
                                            hasAlpha = true;
                                        }
                                        else
                                        {
                                            hasAlpha = false;
                                        }

                                        bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
                                        Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);


                                        BitmapData bitmapData =
                                            bitmap.LockBits(
                                            rectangle,
                                            ImageLockMode.ReadOnly,
                                            hasAlpha ? System.Drawing.Imaging.PixelFormat.Format32bppArgb : System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                                        GL.TexImage2D(
                                            TextureTarget.Texture2D,
                                            0,
                                            hasAlpha ? PixelInternalFormat.Rgba : PixelInternalFormat.Rgb8,
                                            bitmap.Width,
                                            bitmap.Height,
                                            0,
                                            hasAlpha ? OpenTK.Graphics.OpenGL.PixelFormat.Bgra : OpenTK.Graphics.OpenGL.PixelFormat.Bgr,
                                            PixelType.UnsignedByte,
                                            bitmapData.Scan0);

                                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                                        //GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
                                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                                        bitmap.UnlockBits(bitmapData);
                                        bitmap.Dispose();

                                        SafeInvalidate();
                                    }
                                    ));
                                }
                            }
                        });

                    // Set the UserData for this face to our FaceData struct
                    face.UserData = data;
                    mesh.Faces[j] = face;
                }

                Prims.Add(mesh);
                SafeInvalidate();
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
                            Textures[textureID] = (System.Drawing.Image)img.Clone();
                        }
                        gotImage.Set();
                    }
                );
                gotImage.WaitOne(30 * 1000, false);
                if (img != null)
                {
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
            SafeInvalidate();
        }

        private void scrollZoom_ValueChanged(object sender, EventArgs e)
        {
            glControl_Resize(null, null);
            SafeInvalidate();
        }

        #endregion Scrollbar Callbacks


        private void chkWireFrame_CheckedChanged(object sender, EventArgs e)
        {
            Wireframe = chkWireFrame.Checked;
            SafeInvalidate();
        }

        bool dragging = false;
        int dragX, dragY, downX, downY;

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
            {
                dragging = true;
                downX = dragX = e.X;
                downY = dragY = e.Y;
            }

        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                int deltaX = e.X - dragX;
                int deltaY = e.Y - dragY;

                if (e.Button == MouseButtons.Left)
                {
                    if (ModifierKeys == Keys.Control || ModifierKeys == (Keys.Alt | Keys.Control | Keys.Shift))
                    {
                        Center.X -= deltaX / 100f;
                        Center.Z += deltaY / 100f;
                    }

                    if (ModifierKeys == Keys.Alt)
                    {
                        Center.Y -= deltaY / 25f;

                        int newYaw = scrollYaw.Value + deltaX;
                        if (newYaw < 0) newYaw += 360;
                        if (newYaw > 360) newYaw -= 360;

                        scrollYaw.Value = newYaw;

                    }

                    if (ModifierKeys == Keys.None || ModifierKeys == (Keys.Alt | Keys.Control))
                    {
                        int newRoll = scrollRoll.Value + deltaY;
                        if (newRoll < 0) newRoll += 360;
                        if (newRoll > 360) newRoll -= 360;

                        scrollRoll.Value = newRoll;


                        int newYaw = scrollYaw.Value + deltaX;
                        if (newYaw < 0) newYaw += 360;
                        if (newYaw > 360) newYaw -= 360;

                        scrollYaw.Value = newYaw;

                    }
                }
                else if (e.Button == MouseButtons.Middle)
                {
                    Center.X -= deltaX / 100f;
                    Center.Z += deltaY / 100f;

                }

                dragX = e.X;
                dragY = e.Y;

                SafeInvalidate();
            }
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = false;
                SafeInvalidate();
            }
        }
        float[] lightPos = new float[] { 0f, 0f, 1f, 0f };

        private void frmPrimWorkshop_Load(object sender, EventArgs e)
        {

        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            scrollYaw.Value = 0;
            scrollPitch.Value = 0;
            scrollRoll.Value = 0;
            scrollZoom.Value = -50;
            Center = Vector3.Zero;

            SafeInvalidate();
        }

    }

    public class FaceData
    {
        public float[] Vertices;
        public ushort[] Indices;
        public float[] TexCoords;
        public float[] Normals;
        public int TexturePointer;
        public System.Drawing.Image Texture;
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
