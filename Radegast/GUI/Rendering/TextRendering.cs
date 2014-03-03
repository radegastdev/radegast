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
using System.Drawing;
using System.Windows.Forms;
using OpenTK.Graphics.OpenGL;
using OpenMetaverse;

namespace Radegast.Rendering
{
    public class TextRendering : IDisposable
    {
        class CachedInfo
        {
            public int TextureID;
            public int LastUsed;
            public int Width;
            public int Height;
        }

        class TextItem
        {
            public String Text;
            public Font Font;
            public Color Color;
            public Rectangle Box;
            public TextFormatFlags Flags;

            public int ImgWidth;
            public int ImgHeight;

            public int TextureID = -1;

            public TextItem(string text, Font font, Color color, Rectangle box, TextFormatFlags flags)
            {
                this.Text = text;
                this.Font = font;
                this.Color = color;
                this.Box = box;
                this.Flags = flags | TextFormatFlags.NoPrefix;
            }
        }

        public static Size MaxSize = new Size(8192, 8192);

        RadegastInstance Instance;
        List<TextItem> textItems;
        int[] Viewport = new int[4];
        int ScreenWidth { get; set; }
        int ScreenHeight { get; set; }
        Dictionary<int, CachedInfo> Cache = new Dictionary<int, CachedInfo>();

        public TextRendering(RadegastInstance instance)
        {
            this.Instance = instance;
            textItems = new List<TextItem>();
        }

        public void Dispose()
        {
        }

        public void Print(string text, Font font, Color color, Rectangle box, TextFormatFlags flags)
        {
            textItems.Add(new TextItem(text, font, color, box, flags));
        }

        public static Size Measure(string text, Font font, TextFormatFlags flags)
        {
            return TextRenderer.MeasureText(text, font, TextRendering.MaxSize, flags);
        }

        public void Begin()
        {
        }

        public void End()
        {
            GL.GetInteger(GetPName.Viewport, Viewport);
            ScreenWidth = Viewport[2];
            ScreenHeight = Viewport[3];
            int stamp = Environment.TickCount;

            GL.Enable(EnableCap.Texture2D);
            GLHUDBegin();
            {
                foreach (TextItem item in textItems)
                {
                    int hash = GetItemHash(item);
                    CachedInfo tex = new CachedInfo() { TextureID = -1 };
                    if (Cache.ContainsKey(hash))
                    {
                        tex = Cache[hash];
                        tex.LastUsed = stamp;
                    }
                    else
                    {
                        PrepareText(item);
                        if (item.TextureID != -1)
                        {
                            Cache[hash] = tex = new CachedInfo()
                            {
                                TextureID = item.TextureID,
                                Width = item.ImgWidth,
                                Height = item.ImgHeight,
                                LastUsed = stamp
                            };
                        }
                    }
                    if (tex.TextureID == -1) continue;
                    GL.Color4(item.Color);
                    GL.BindTexture(TextureTarget.Texture2D, tex.TextureID);
                    RHelp.Draw2DBox(item.Box.X, ScreenHeight - item.Box.Y - tex.Height, tex.Width, tex.Height, 0f);
                }

                GL.BindTexture(TextureTarget.Texture2D, 0);
                GL.Disable(EnableCap.Texture2D);
                GL.Color4(1f, 1f, 1f, 1f);
            }
            GLHUDEnd();

            textItems.Clear();
        }

        int GetItemHash(TextItem item)
        {
            int ret = 17;
            ret = ret * 31 + item.Text.GetHashCode();
            ret = ret * 31 + item.Font.GetHashCode();
            ret = ret * 31 + (int)item.Flags;
            return ret;
        }


        void PrepareText(TextItem item)
        {

            // If we're modified and have texture already delete it from graphics card
            if (item.TextureID > 0)
            {
                //GL.DeleteTexture(item.TextureID);
                item.TextureID = -1;
            }

            Size s;
            
            try
            {
                 s = TextRenderer.MeasureText(
                    item.Text,
                    item.Font,
                    TextRendering.MaxSize,
                    item.Flags);
            }
            catch
            {
                return;
            }

            item.ImgWidth = s.Width;
            item.ImgHeight = s.Height;

            if (!RenderSettings.TextureNonPowerOfTwoSupported)
            {
                item.ImgWidth = RHelp.NextPow2(s.Width);
                item.ImgHeight = RHelp.NextPow2(s.Height);
            }

            Bitmap img = new Bitmap(
                item.ImgWidth,
                item.ImgHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(img);
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;

            TextRenderer.DrawText(
                g,
                item.Text,
                item.Font,
                new Rectangle(0, 0, s.Width + 2, s.Height + 2),
                Color.White,
                Color.Transparent,
                item.Flags);

            item.TextureID = RHelp.GLLoadImage(img, true, false);
            g.Dispose();
            img.Dispose();
        }


        bool depthTestEnabled;
        bool lightningEnabled;

        // Switch to ortho display mode for drawing hud
        void GLHUDBegin()
        {
            depthTestEnabled = GL.IsEnabled(EnableCap.DepthTest);
            lightningEnabled = GL.IsEnabled(EnableCap.Lighting);

            GL.Disable(EnableCap.DepthTest);
            GL.Disable(EnableCap.Lighting);
            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Ortho(0, ScreenWidth, 0, ScreenHeight, 1, -1);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
        }

        // Switch back to frustrum display mode
        void GLHUDEnd()
        {
            if (depthTestEnabled)
            {
                GL.Enable(EnableCap.DepthTest);
            }
            if (lightningEnabled)
            {
                GL.Enable(EnableCap.Lighting);
            }
            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();
            GL.MatrixMode(MatrixMode.Modelview);
        }

    }
}
