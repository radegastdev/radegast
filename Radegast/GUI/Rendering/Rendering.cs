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

#region Usings
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Text;
using System.Threading;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using OpenMetaverse;
using OpenMetaverse.Rendering;
using OpenMetaverse.Assets;
using OpenMetaverse.Imaging;
using OpenMetaverse.StructuredData;
#endregion Usings

namespace Radegast
{

    public partial class SceneWindow : RadegastForm
    {
        #region Public fields
        /// <summary>
        /// The OpenGL surface
        /// </summary>
        public OpenTK.GLControl glControl = null;

        /// <summary>
        /// Use multi sampling (anti aliasing)
        /// </summary>
        public bool UseMultiSampling = true;

        /// <summary>
        /// Is rendering engine ready and enabled
        /// </summary>
        public bool RenderingEnabled = false;

        /// <summary>
        /// Rednder in wireframe mode
        /// </summary>
        public bool Wireframe = false;

        /// <summary>
        /// List of prims in the scene
        /// </summary>
        Dictionary<uint, FacetedMesh> Prims = new Dictionary<uint, FacetedMesh>();

        #endregion Public fields

        #region Private fields

        Camera Camera;
        Dictionary<UUID, TextureInfo> TexturesPtrMap = new Dictionary<UUID, TextureInfo>();
        RadegastInstance instance;
        MeshmerizerR renderer;
        OpenTK.Graphics.GraphicsMode GLMode = null;
        AutoResetEvent TextureThreadContextReady = new AutoResetEvent(false);
        BlockingQueue<TextureLoadItem> PendingTextures = new BlockingQueue<TextureLoadItem>();
        float[] lightPos = new float[] { 0f, 0f, 1f, 0f };
        bool hasMipmap;
        Font HoverTextFont = new Font(FontFamily.GenericSansSerif, 10f, FontStyle.Regular);
        Dictionary<UUID, Bitmap> sculptCache = new Dictionary<UUID, Bitmap>();
        OpenTK.Matrix4 ModelViewMatrix;
        OpenTK.Matrix4 ProjectionMatrix;
        int[] Viewport = new int[4];
        #endregion Private fields

        #region Construction and disposal
        public SceneWindow(RadegastInstance instance)
            : base(instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(frmPrimWorkshop_Disposed);
            AutoSavePosition = true;
            UseMultiSampling = cbAA.Checked = instance.GlobalSettings["use_multi_sampling"];
            cbAA.CheckedChanged += cbAA_CheckedChanged;

            this.instance = instance;

            renderer = new MeshmerizerR();

            // Camera initial setting
            Camera = new Camera();
            InitCamera();

            Client.Objects.TerseObjectUpdate += new EventHandler<TerseObjectUpdateEventArgs>(Objects_TerseObjectUpdate);
            Client.Objects.ObjectUpdate += new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
            Client.Objects.ObjectDataBlockUpdate += new EventHandler<ObjectDataBlockUpdateEventArgs>(Objects_ObjectDataBlockUpdate);
            Client.Objects.KillObject += new EventHandler<KillObjectEventArgs>(Objects_KillObject);
            Client.Network.SimChanged += new EventHandler<SimChangedEventArgs>(Network_SimChanged);
            Client.Self.TeleportProgress += new EventHandler<TeleportEventArgs>(Self_TeleportProgress);
            Instance.Netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);
            Application.Idle += new EventHandler(Application_Idle);
        }

        void frmPrimWorkshop_Disposed(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(Application_Idle);
            Client.Objects.TerseObjectUpdate -= new EventHandler<TerseObjectUpdateEventArgs>(Objects_TerseObjectUpdate);
            Client.Objects.ObjectUpdate -= new EventHandler<PrimEventArgs>(Objects_ObjectUpdate);
            Client.Objects.ObjectDataBlockUpdate -= new EventHandler<ObjectDataBlockUpdateEventArgs>(Objects_ObjectDataBlockUpdate);
            Client.Objects.KillObject -= new EventHandler<KillObjectEventArgs>(Objects_KillObject);
            Client.Network.SimChanged -= new EventHandler<SimChangedEventArgs>(Network_SimChanged);
            Client.Self.TeleportProgress -= new EventHandler<TeleportEventArgs>(Self_TeleportProgress);
            Instance.Netcom.ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);

            if (glControl != null)
            {
                glControl.Dispose();
            }
            glControl = null;

            lock (sculptCache)
            {
                foreach (var img in sculptCache.Values)
                    img.Dispose();
                sculptCache.Clear();
            }

            lock (Prims) Prims.Clear();
            TexturesPtrMap.Clear();
            GC.Collect();
        }

        void Application_Idle(object sender, EventArgs e)
        {
            if (glControl != null && !glControl.IsDisposed && RenderingEnabled)
            {
                while (glControl.IsIdle)
                {
                    MainRenderLoop();
                }
            }
        }
        #endregion Construction and disposal

        #region Network messaage handlers
        void Netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated || !instance.MonoRuntime)
                {
                    BeginInvoke(new MethodInvoker(() => Netcom_ClientDisconnected(sender, e)));
                }
                return;
            }

            Dispose();
        }

        void Self_TeleportProgress(object sender, TeleportEventArgs e)
        {
            switch (e.Status)
            {
                case TeleportStatus.Progress:
                case TeleportStatus.Start:
                    RenderingEnabled = false;
                    break;

                case TeleportStatus.Cancelled:
                case TeleportStatus.Failed:
                    RenderingEnabled = true;
                    break;

                case TeleportStatus.Finished:
                    ThreadPool.QueueUserWorkItem(sync =>
                    {
                        Thread.Sleep(3000);
                        InitCamera();
                        LoadCurrentPrims();
                        RenderingEnabled = true;
                    });
                    break;
            }
        }

        void Network_SimChanged(object sender, SimChangedEventArgs e)
        {
            lock (sculptCache)
            {
                foreach (var img in sculptCache.Values)
                    img.Dispose();
                sculptCache.Clear();
            }
            lock (Prims) Prims.Clear();
        }

        void Objects_KillObject(object sender, KillObjectEventArgs e)
        {
            if (e.Simulator.Handle != Client.Network.CurrentSim.Handle) return;
            lock (Prims) Prims.Remove(e.ObjectLocalID);
        }

        void Objects_TerseObjectUpdate(object sender, TerseObjectUpdateEventArgs e)
        {
            if (e.Simulator.Handle != Client.Network.CurrentSim.Handle) return;
            UpdatePrimBlocking(e.Prim);
        }

        void Objects_ObjectUpdate(object sender, PrimEventArgs e)
        {
            if (e.Simulator.Handle != Client.Network.CurrentSim.Handle) return;
            UpdatePrimBlocking(e.Prim);
        }

        void Objects_ObjectDataBlockUpdate(object sender, ObjectDataBlockUpdateEventArgs e)
        {
            if (e.Simulator.Handle != Client.Network.CurrentSim.Handle) return;
            UpdatePrimBlocking(e.Prim);
        }
        #endregion Network messaage handlers

        #region glControl setup and disposal
        public void SetupGLControl()
        {
            RenderingEnabled = false;

            if (glControl != null)
                glControl.Dispose();
            glControl = null;

            GLMode = null;

            try
            {
                if (!UseMultiSampling)
                {
                    GLMode = new OpenTK.Graphics.GraphicsMode(OpenTK.DisplayDevice.Default.BitsPerPixel, 24, 8, 0);
                }
                else
                {
                    for (int aa = 0; aa <= 4; aa += 2)
                    {
                        var testMode = new OpenTK.Graphics.GraphicsMode(OpenTK.DisplayDevice.Default.BitsPerPixel, 24, 8, aa);
                        if (testMode.Samples == aa)
                        {
                            GLMode = testMode;
                        }
                    }
                }
            }
            catch
            {
                GLMode = null;
            }


            try
            {
                if (GLMode == null)
                {
                    // Try default mode
                    glControl = new OpenTK.GLControl();
                }
                else
                {
                    glControl = new OpenTK.GLControl(GLMode);
                }
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message, Helpers.LogLevel.Warning, Client);
                glControl = null;
            }

            if (glControl == null)
            {
                Logger.Log("Failed to initialize OpenGL control, cannot continue", Helpers.LogLevel.Error, Client);
                return;
            }

            Logger.Log("Initializing OpenGL mode: " + GLMode.ToString(), Helpers.LogLevel.Info);

            glControl.Paint += glControl_Paint;
            glControl.Resize += glControl_Resize;
            glControl.MouseDown += glControl_MouseDown;
            glControl.MouseUp += glControl_MouseUp;
            glControl.MouseMove += glControl_MouseMove;
            glControl.MouseWheel += glControl_MouseWheel;
            glControl.Load += new EventHandler(glControl_Load);
            glControl.Disposed += new EventHandler(glControl_Disposed);
            glControl.Dock = DockStyle.Fill;
            glControl.VSync = false;
            Controls.Add(glControl);
            glControl.BringToFront();
        }

        void glControl_Disposed(object sender, EventArgs e)
        {
            TextureThreadRunning = false;
            PendingTextures.Close();
            glControl.Paint -= glControl_Paint;
            glControl.Resize -= glControl_Resize;
            glControl.MouseDown -= glControl_MouseDown;
            glControl.MouseUp -= glControl_MouseUp;
            glControl.MouseMove -= glControl_MouseMove;
            glControl.MouseWheel -= glControl_MouseWheel;
            glControl.Load -= new EventHandler(glControl_Load);
            glControl.Disposed -= glControl_Disposed;
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
                hasMipmap = GL.GetString(StringName.Extensions).Contains("GL_SGIS_generate_mipmap");

                RenderingEnabled = true;
                // Call the resizing function which sets up the GL drawing window
                // and will also invalidate the GL control
                glControl_Resize(null, null);

                glControl.Context.MakeCurrent(null);
                TextureThreadContextReady.Reset();
                var textureThread = new Thread(() => TextureThread())
                {
                    IsBackground = true,
                    Name = "TextureLoadingThread"
                };
                textureThread.Start();
                TextureThreadContextReady.WaitOne(1000, false);
                glControl.MakeCurrent();
            }
            catch (Exception ex)
            {
                RenderingEnabled = false;
                Logger.Log("Failed to initialize OpenGL control", Helpers.LogLevel.Warning, Client, ex);
            }
        }
        #endregion glControl setup and disposal

        #region glControl paint and resize events
        private void MainRenderLoop()
        {
            if (!RenderingEnabled) return;

            Render(false);

            glControl.SwapBuffers();
        }

        void glControl_Paint(object sender, EventArgs e)
        {
            MainRenderLoop();
        }

        private void glControl_Resize(object sender, EventArgs e)
        {
            if (!RenderingEnabled) return;
            glControl.MakeCurrent();

            GL.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);

            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            GL.PushMatrix();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();

            SetPerspective();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }
        #endregion glControl paint and resize events

        #region Mouse handling
        bool dragging = false;
        int dragX, dragY, downX, downY;

        private void glControl_MouseWheel(object sender, MouseEventArgs e)
        {
            int newVal = Utils.Clamp(scrollZoom.Value + e.Delta / 10, scrollZoom.Minimum, scrollZoom.Maximum);

            if (scrollZoom.Value != newVal)
            {
                Camera.Zoom = 1f - (float)newVal / (float)scrollZoom.Minimum;
                scrollZoom.Value = newVal;
                glControl_Resize(null, null);
            }
        }

        FacetedMesh RightclickedPrim;
        int RightclickedFaceID;

        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                downX = dragX = e.X;
                downY = dragY = e.Y;
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (TryPick(e.X, e.Y, out RightclickedPrim, out RightclickedFaceID))
                {
                    ctxObjects.Show(glControl, e.X, e.Y);
                }
            }

        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                int deltaX = e.X - dragX;
                int deltaY = e.Y - dragY;
                float pixelToM = 1f / 75f;

                if (e.Button == MouseButtons.Left)
                {
                    // Pan
                    if (ModifierKeys == Keys.Control || ModifierKeys == (Keys.Alt | Keys.Control | Keys.Shift))
                    {
                        Vector3 direction = Camera.Position - Camera.FocalPoint;
                        direction.Normalize();
                        Vector3 vy = direction % new Vector3(0f, 0f, 1f);
                        Vector3 vx = vy % direction;
                        Vector3 vxy = vx * deltaY * pixelToM + vy * deltaX * pixelToM;
                        Camera.Position += vxy;
                        Camera.FocalPoint += vxy;
                    }

                    // Alt-zoom (up down move camera closer to target, left right rotate around target)
                    if (ModifierKeys == Keys.Alt)
                    {
                        Camera.Position += (Camera.Position - Camera.FocalPoint) * deltaY * pixelToM;
                        var dx = -(deltaX * pixelToM);
                        Camera.Position = Camera.FocalPoint + (Camera.Position - Camera.FocalPoint) * new Quaternion(0f, 0f, (float)Math.Sin(dx), (float)Math.Cos(dx));
                    }

                    // Rotate camera in a vertical circle around target on up down mouse movement
                    if (ModifierKeys == (Keys.Alt | Keys.Control))
                    {
                        Camera.Position = Camera.FocalPoint +
                            (Camera.Position - Camera.FocalPoint)
                            * Quaternion.CreateFromAxisAngle((Camera.Position - Camera.FocalPoint) % new Vector3(0f, 0f, 1f), deltaY * pixelToM);
                        var dx = -(deltaX * pixelToM);
                        Camera.Position = Camera.FocalPoint + (Camera.Position - Camera.FocalPoint) * new Quaternion(0f, 0f, (float)Math.Sin(dx), (float)Math.Cos(dx));
                    }

                }

                dragX = e.X;
                dragY = e.Y;
            }
        }

        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = false;

                if (e.X == downX && e.Y == downY) // click
                {
                    FacetedMesh picked;
                    int faceID;
                    if (TryPick(e.X, e.Y, out picked, out faceID))
                    {
                        if (ModifierKeys == Keys.None)
                        {
                            Client.Self.Grab(picked.Prim.LocalID, Vector3.Zero, Vector3.Zero, Vector3.Zero, faceID, Vector3.Zero, Vector3.Zero, Vector3.Zero);
                            Client.Self.GrabUpdate(picked.Prim.ID, Vector3.Zero, Vector3.Zero, Vector3.Zero, Vector3.Zero, faceID, Vector3.Zero, Vector3.Zero, Vector3.Zero);
                            Client.Self.DeGrab(picked.Prim.LocalID);
                        }
                        else if (ModifierKeys == Keys.Alt)
                        {
                            Camera.FocalPoint = PrimPos(picked.Prim);
                            Cursor.Position = glControl.PointToScreen(new Point(glControl.Width / 2, glControl.Height / 2));
                            UpdateCamera();
                        }
                    }
                }
            }
        }
        #endregion Mouse handling

        #region Texture thread
        bool TextureThreadRunning = true;

        void TextureThread()
        {
            OpenTK.INativeWindow window = new OpenTK.NativeWindow();
            OpenTK.Graphics.IGraphicsContext context = new OpenTK.Graphics.GraphicsContext(GLMode, window.WindowInfo);
            context.MakeCurrent(window.WindowInfo);
            TextureThreadContextReady.Set();
            PendingTextures.Open();
            Logger.DebugLog("Started Texture Thread");

            while (window.Exists && TextureThreadRunning)
            {
                window.ProcessEvents();

                TextureLoadItem item = null;

                if (!PendingTextures.Dequeue(Timeout.Infinite, ref item)) continue;

                if (TexturesPtrMap.ContainsKey(item.TeFace.TextureID))
                {
                    item.Data.TextureInfo = TexturesPtrMap[item.TeFace.TextureID];
                    GL.BindTexture(TextureTarget.Texture2D, item.Data.TextureInfo.TexturePointer);
                    continue;
                }

                if (LoadTexture(item.TeFace.TextureID, ref item.Data.TextureInfo.Texture, false))
                {
                    GL.GenTextures(1, out item.Data.TextureInfo.TexturePointer);
                    GL.BindTexture(TextureTarget.Texture2D, item.Data.TextureInfo.TexturePointer);

                    Bitmap bitmap = (Bitmap)item.Data.TextureInfo.Texture;

                    bool hasAlpha;
                    if (item.Data.TextureInfo.Texture.PixelFormat == System.Drawing.Imaging.PixelFormat.Format32bppArgb)
                    {
                        hasAlpha = true;
                    }
                    else
                    {
                        hasAlpha = false;
                    }
                    item.Data.TextureInfo.HasAlpha = hasAlpha;

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

                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                    if (hasMipmap)
                    {
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                    }
                    else
                    {
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                    }

                    TexturesPtrMap[item.TeFace.TextureID] = item.Data.TextureInfo;

                    bitmap.UnlockBits(bitmapData);
                    bitmap.Dispose();
                    item.Data.TextureInfo.Texture = null;

                    GL.Flush();
                }
            }
            Logger.DebugLog("Texture thread exited");
        }
        #endregion Texture thread

        void LoadCurrentPrims()
        {
            ThreadPool.QueueUserWorkItem(sync =>
                {
                    Client.Network.CurrentSim.ObjectsPrimitives.FindAll((Primitive root) => root.ParentID == 0).ForEach((Primitive mainPrim) =>
                    {
                        UpdatePrimBlocking(mainPrim);
                        Client.Network.CurrentSim.ObjectsPrimitives.FindAll((Primitive p) => { return p.ParentID == mainPrim.LocalID; })
                            .FindAll((Primitive child) => child.ParentID == mainPrim.LocalID)
                            .ForEach((Primitive subPrim) => UpdatePrimBlocking(subPrim));
                    });
                }
            );
        }

        private void frmPrimWorkshop_Shown(object sender, EventArgs e)
        {
            SetupGLControl();
            UpdateCamera();
            LoadCurrentPrims();
        }

        #region Private methods (the meat)
        private void UpdateCamera()
        {
            if (Client != null)
            {
                Client.Self.Movement.Camera.LookAt(Camera.Position, Camera.FocalPoint);
                //Client.Self.Movement.Camera.Far = (float)Camera.Far;
            }

            if (RenderingEnabled)
            {
                GL.PushMatrix();
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();

                SetPerspective();

                GL.MatrixMode(MatrixMode.Modelview);
                GL.PopMatrix();
            }
        }

        void InitCamera()
        {
            Camera.Position = Client.Self.SimPosition + new Vector3(-2, 0, 0) * Client.Self.Movement.BodyRotation;
            Camera.Position.Z += 2f;
            Camera.FocalPoint = Client.Self.SimPosition + new Vector3(5, 0, 0) * Client.Self.Movement.BodyRotation;
            Camera.Zoom = 1.0f;
            Camera.Far = 128.0f;
        }

        Vector3 PrimPos(Primitive prim)
        {
            if (prim.ParentID == 0)
            {
                return prim.Position;
            }
            else
            {
                FacetedMesh parent;
                if (Prims.TryGetValue(prim.ParentID, out parent))
                {
                    return parent.Prim.Position + prim.Position * Matrix4.CreateFromQuaternion(parent.Prim.Rotation);
                    //return parent.Position * prim.Position * prim.Rotation;
                }
                else
                {
                    return Vector3.Zero;
                }
            }
        }

        private void SetPerspective()
        {
            float dAspRat = (float)glControl.Width / (float)glControl.Height;
            GluPerspective(50.0f * Camera.Zoom, dAspRat, 0.1f, (float)Camera.Far);
        }


#pragma warning disable 0612
        OpenTK.Graphics.TextPrinter Printer = new OpenTK.Graphics.TextPrinter(OpenTK.Graphics.TextQuality.High);
#pragma warning restore 0612
        private void RenderText()
        {
            lock (Prims)
            {
                int primNr = 0;
                foreach (FacetedMesh mesh in Prims.Values)
                {
                    primNr++;
                    Primitive prim = mesh.Prim;
                    if (!string.IsNullOrEmpty(prim.Text))
                    {
                        string text = System.Text.RegularExpressions.Regex.Replace(prim.Text, "(\r?\n)+", "\n");
                        var newPrimPos = PrimPos(prim);
                        OpenTK.Vector3 primPos = new OpenTK.Vector3(newPrimPos.X, newPrimPos.Y, newPrimPos.Z);

                        if (Vector3.Distance(newPrimPos, Camera.Position) > 15) continue;

                        primPos.Z += prim.Scale.Z * 0.8f;

                        OpenTK.Vector3 screenPos;
                        if (!Math3D.GluProject(primPos, ModelViewMatrix, ProjectionMatrix, Viewport, out screenPos)) continue;
                        screenPos.Y = glControl.Height - screenPos.Y;

                        Printer.Begin();

                        Color color = Color.FromArgb((int)(prim.TextColor.A * 255), (int)(prim.TextColor.R * 255), (int)(prim.TextColor.G * 255), (int)(prim.TextColor.B * 255));

                        var size = Printer.Measure(text, HoverTextFont);
                        screenPos.X -= size.BoundingBox.Width / 2;
                        screenPos.Y -= size.BoundingBox.Height;

                        // Shadow
                        if (color != Color.Black)
                        {
                            Printer.Print(text, HoverTextFont, Color.Black, new RectangleF(screenPos.X + 1, screenPos.Y + 1, size.BoundingBox.Width, size.BoundingBox.Height), OpenTK.Graphics.TextPrinterOptions.Default, OpenTK.Graphics.TextAlignment.Center);
                        }
                        Printer.Print(text, HoverTextFont, color, new RectangleF(screenPos.X, screenPos.Y, size.BoundingBox.Width, size.BoundingBox.Height), OpenTK.Graphics.TextPrinterOptions.Default, OpenTK.Graphics.TextAlignment.Center);

                        Printer.End();
                    }
                }
            }
        }

        private void RenderObjects(RenderPass pass)
        {
            lock (Prims)
            {
                int primNr = 0;
                foreach (FacetedMesh mesh in Prims.Values)
                {
                    primNr++;
                    Primitive prim = mesh.Prim;
                    FacetedMesh parent = null;
                    if (prim.ParentID != 0 && !Prims.TryGetValue(prim.ParentID, out parent)) continue;

                    // Individual prim matrix
                    GL.PushMatrix();

                    if (prim.ParentID != 0)
                    {
                        // Apply prim translation and rotation relative to the root prim
                        GL.MultMatrix(Math3D.CreateTranslationMatrix(parent.Prim.Position));
                        GL.MultMatrix(Math3D.CreateRotationMatrix(parent.Prim.Rotation));
                    }

                    // Prim roation and position
                    GL.MultMatrix(Math3D.CreateTranslationMatrix(prim.Position));
                    GL.MultMatrix(Math3D.CreateRotationMatrix(prim.Rotation));

                    // Prim scaling
                    GL.Scale(prim.Scale.X, prim.Scale.Y, prim.Scale.Z);

                    // Draw the prim faces
                    for (int j = 0; j < mesh.Faces.Count; j++)
                    {
                        Primitive.TextureEntryFace teFace = mesh.Prim.Textures.FaceTextures[j];
                        Face face = mesh.Faces[j];
                        FaceData data = (FaceData)face.UserData;

                        if (teFace == null)
                            teFace = mesh.Prim.Textures.DefaultTexture;

                        if (teFace == null)
                            continue;

                        if (pass != RenderPass.Picking)
                        {
                            bool belongToAlphaPass = (teFace.RGBA.A < 0.99) || data.TextureInfo.HasAlpha;

                            if (belongToAlphaPass && pass != RenderPass.Alpha) continue;
                            if (!belongToAlphaPass && pass == RenderPass.Alpha) continue;

                            // Don't render transparent faces
                            if (teFace.RGBA.A <= 0.01f) continue;

                            switch (teFace.Shiny)
                            {
                                case Shininess.High:
                                    GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 94f);
                                    break;

                                case Shininess.Medium:
                                    GL.Material(MaterialFace.Front, MaterialParameter.Shininess, 64f);
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

                            if (data.TextureInfo.TexturePointer != 0)
                            {
                                GL.Enable(EnableCap.Texture2D);
                            }
                            else
                            {
                                GL.Disable(EnableCap.Texture2D);
                            }

                            // Bind the texture
                            GL.BindTexture(TextureTarget.Texture2D, data.TextureInfo.TexturePointer);
                        }
                        else
                        {
                            data.PickingID = primNr;
                            var primNrBytes = Utils.Int16ToBytes((short)primNr);
                            var faceColor = new byte[] { primNrBytes[0], primNrBytes[1], (byte)j, 255 };

                            GL.Color4(faceColor);
                        }

                        GL.TexCoordPointer(2, TexCoordPointerType.Float, 0, data.TexCoords);
                        GL.VertexPointer(3, VertexPointerType.Float, 0, data.Vertices);
                        GL.NormalPointer(NormalPointerType.Float, 0, data.Normals);
                        GL.DrawElements(BeginMode.Triangles, data.Indices.Length, DrawElementsType.UnsignedShort, data.Indices);

                    }

                    // Pop the prim matrix
                    GL.PopMatrix();
                }
            }
        }

        private void Render(bool picking)
        {
            glControl.MakeCurrent();
            if (picking)
            {
                GL.ClearColor(1f, 1f, 1f, 1f);
            }
            else
            {
                GL.ClearColor(0.39f, 0.58f, 0.93f, 1.0f);
            }

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.LoadIdentity();

            // Setup wireframe or solid fill drawing mode
            if (Wireframe && !picking)
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            }
            else
            {
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }

            var mLookAt = OpenTK.Matrix4d.LookAt(
                    Camera.Position.X, Camera.Position.Y, Camera.Position.Z,
                    Camera.FocalPoint.X, Camera.FocalPoint.Y, Camera.FocalPoint.Z,
                    0d, 0d, 1d);
            GL.MultMatrix(ref mLookAt);

            //GL.Light(LightName.Light0, LightParameter.Position, lightPos);

            // Push the world matrix
            GL.PushMatrix();

            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.NormalArray);

            GL.GetFloat(GetPName.ProjectionMatrix, out ProjectionMatrix);
            GL.GetFloat(GetPName.ModelviewMatrix, out ModelViewMatrix);
            GL.GetInteger(GetPName.Viewport, Viewport);

            if (picking)
            {
                RenderObjects(RenderPass.Picking);
            }
            else
            {
                RenderObjects(RenderPass.Simple);
                RenderObjects(RenderPass.Alpha);
                RenderText();
            }

            // Pop the world matrix
            GL.PopMatrix();

            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);

            GL.Flush();
        }

        private void GluPerspective(float fovy, float aspect, float zNear, float zFar)
        {
            float fH = (float)Math.Tan(fovy / 360 * (float)Math.PI) * zNear;
            float fW = fH * aspect;
            GL.Frustum(-fW, fW, -fH, fH, zNear, zFar);
        }

        private bool TryPick(int x, int y, out FacetedMesh picked, out int faceID)
        {
            // Save old attributes
            GL.PushAttrib(AttribMask.AllAttribBits);

            // Disable some attributes to make the objects flat / solid color when they are drawn
            GL.Disable(EnableCap.Fog);
            GL.Disable(EnableCap.Texture2D);
            GL.Disable(EnableCap.Dither);
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.LineStipple);
            GL.Disable(EnableCap.PolygonStipple);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.AlphaTest);

            Render(true);

            byte[] color = new byte[4];
            GL.ReadPixels(x, glControl.Height - y, 1, 1, OpenTK.Graphics.OpenGL.PixelFormat.Rgba, PixelType.UnsignedByte, color);

            GL.PopAttrib();

            int primID = Utils.BytesToUInt16(color, 0);
            faceID = color[2];

            picked = null;

            lock (Prims)
            {
                foreach (var mesh in Prims.Values)
                {
                    foreach (var face in mesh.Faces)
                    {
                        if (face.UserData == null) continue;
                        if (((FaceData)face.UserData).PickingID == primID)
                        {
                            picked = mesh;
                            break;
                        }
                    }

                    if (picked != null) break;
                }
            }

            return picked != null;
        }


        private void UpdatePrimBlocking(Primitive prim)
        {
            // Don't render avatars for now
            if (Client.Network.CurrentSim.ObjectsAvatars.ContainsKey(prim.LocalID)) return;

            if (Vector3.Distance(PrimPos(prim), Client.Self.SimPosition) > 32 && !Prims.ContainsKey(prim.ParentID)) return;
            if (prim.Textures == null) return;

            FacetedMesh mesh = null;
            FacetedMesh existingMesh = null;

            lock (Prims)
            {
                if (Prims.ContainsKey(prim.LocalID))
                {
                    existingMesh = Prims[prim.LocalID];
                }
            }

            try
            {
                if (prim.Sculpt != null && prim.Sculpt.SculptTexture != UUID.Zero)
                {
                    if (prim.Sculpt.Type != SculptType.Mesh)
                    { // Regular sculptie
                        Image img = null;
                        
                        lock (sculptCache)
                        {
                            if (sculptCache.ContainsKey(prim.Sculpt.SculptTexture))
                            {
                                img = sculptCache[prim.Sculpt.SculptTexture];
                            }
                        }

                        if (img == null)
                        {
                            if (LoadTexture(prim.Sculpt.SculptTexture, ref img, true))
                            {
                                sculptCache[prim.Sculpt.SculptTexture] = (Bitmap)img;
                            }
                            else
                            {
                                return;
                            }
                        }

                        mesh = renderer.GenerateFacetedSculptMesh(prim, (Bitmap)img, DetailLevel.High);
                    }
                    else
                    { // Mesh
                        AutoResetEvent gotMesh = new AutoResetEvent(false);
                        bool meshSuccess = false;

                        Client.Assets.RequestMesh(prim.Sculpt.SculptTexture, (success, meshAsset) =>
                            {
                                if (!success || !FacetedMesh.TryDecodeFromAsset(prim, meshAsset, DetailLevel.Highest, out mesh))
                                {
                                    Logger.Log("Failed to fetch or decode the mesh asset", Helpers.LogLevel.Warning, Client);
                                }
                                else
                                {
                                    meshSuccess = true;
                                }
                                gotMesh.Set();
                            });

                        if (!gotMesh.WaitOne(20 * 1000, false)) return;
                        if (!meshSuccess) return;
                    }
                }
                else
                {
                    mesh = renderer.GenerateFacetedMesh(prim, DetailLevel.High);
                }
            }
            catch
            {
                return;
            }

            // Create a FaceData struct for each face that stores the 3D data
            // in a OpenGL friendly format
            for (int j = 0; j < mesh.Faces.Count; j++)
            {
                Primitive.TextureEntryFace teFace = prim.Textures.GetFace((uint)j);
                if (teFace == null) continue;

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
                renderer.TransformTexCoords(face.Vertices, face.Center, teFace);

                // Texcoords for this face
                data.TexCoords = new float[face.Vertices.Count * 2];
                for (int k = 0; k < face.Vertices.Count; k++)
                {
                    data.TexCoords[k * 2 + 0] = face.Vertices[k].TexCoord.X;
                    data.TexCoords[k * 2 + 1] = face.Vertices[k].TexCoord.Y;
                }

                // Set the UserData for this face to our FaceData struct
                face.UserData = data;
                mesh.Faces[j] = face;


                if (existingMesh != null &&
                    j < existingMesh.Faces.Count &&
                    existingMesh.Faces[j].TextureFace.TextureID == teFace.TextureID &&
                    ((FaceData)existingMesh.Faces[j].UserData).TextureInfo.TexturePointer != 0
                    )
                {
                    FaceData existingData = (FaceData)existingMesh.Faces[j].UserData;
                    data.TextureInfo.TexturePointer = existingData.TextureInfo.TexturePointer;
                }
                else
                {

                    var textureItem = new TextureLoadItem()
                    {
                        Data = data,
                        Prim = prim,
                        TeFace = teFace
                    };

                    PendingTextures.Enqueue(textureItem);
                }

            }

            lock (Prims)
            {
                Prims[prim.LocalID] = mesh;
            }
        }

        private bool LoadTexture(UUID textureID, ref Image texture, bool removeAlpha)
        {
            ManualResetEvent gotImage = new ManualResetEvent(false);
            Image img = null;

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
                    texture = img;
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
        #endregion Private methods (the meat)

        #region Form controls handlers
        private void scrollZoom_ValueChanged(object sender, EventArgs e)
        {
            Camera.Zoom = 1f - (float)scrollZoom.Value / (float)scrollZoom.Minimum;
            glControl_Resize(null, null);
        }

        private void chkWireFrame_CheckedChanged(object sender, EventArgs e)
        {
            Wireframe = chkWireFrame.Checked;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            InitCamera();
            UpdateCamera();
            scrollZoom.Value = 0;
        }

        private void cbAA_CheckedChanged(object sender, EventArgs e)
        {
            instance.GlobalSettings["use_multi_sampling"] = UseMultiSampling = cbAA.Checked;
            SetupGLControl();
        }

        #endregion Form controls handlers

        #region Context menu
        private void ctxObjects_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (instance.State.IsSitting)
            {
                sitToolStripMenuItem.Text = "Stand up";
            }
            else if (RightclickedPrim.Prim.Properties != null
                && !string.IsNullOrEmpty(RightclickedPrim.Prim.Properties.SitName))
            {
                sitToolStripMenuItem.Text = RightclickedPrim.Prim.Properties.SitName;
            }
            else
            {
                sitToolStripMenuItem.Text = "Sit";
            }

            if (RightclickedPrim.Prim.Properties != null
                && !string.IsNullOrEmpty(RightclickedPrim.Prim.Properties.TouchName))
            {
                touchToolStripMenuItem.Text = RightclickedPrim.Prim.Properties.TouchName;
            }
            else
            {
                touchToolStripMenuItem.Text = "Touch";
            }
        }

        private void touchToolStripMenuItem_Click(object sender, EventArgs e)
        {

            Client.Self.Grab(RightclickedPrim.Prim.LocalID, Vector3.Zero, Vector3.Zero, Vector3.Zero, RightclickedFaceID, Vector3.Zero, Vector3.Zero, Vector3.Zero);
            Thread.Sleep(100);
            Client.Self.DeGrab(RightclickedPrim.Prim.LocalID);
        }

        private void sitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!instance.State.IsSitting)
            {
                instance.State.SetSitting(true, RightclickedPrim.Prim.ID);
            }
            else
            {
                instance.State.SetSitting(false, UUID.Zero);
            }
        }

        private void takeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            instance.MediaManager.PlayUISound(UISounds.ObjectDelete);
            Client.Inventory.RequestDeRezToInventory(RightclickedPrim.Prim.LocalID);
            Close();
        }

        private void returnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            instance.MediaManager.PlayUISound(UISounds.ObjectDelete);
            Client.Inventory.RequestDeRezToInventory(RightclickedPrim.Prim.LocalID, DeRezDestination.ReturnToOwner, UUID.Zero, UUID.Random());
            Close();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (RightclickedPrim.Prim.Properties != null && RightclickedPrim.Prim.Properties.OwnerID != Client.Self.AgentID)
                returnToolStripMenuItem_Click(sender, e);
            else
            {
                instance.MediaManager.PlayUISound(UISounds.ObjectDelete);
                Client.Inventory.RequestDeRezToInventory(RightclickedPrim.Prim.LocalID, DeRezDestination.AgentInventoryTake, Client.Inventory.FindFolderForType(AssetType.TrashFolder), UUID.Random());
            }
            Close();
        }
        #endregion Context menu
    }
}
