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

namespace Radegast.Rendering
{
    public class ChatOverlay : IDisposable
    {
        public static Font ChatFont = new Font(FontFamily.GenericSansSerif, 11f, FontStyle.Regular, GraphicsUnit.Point);
        public static float ChatLineTimeOnScreen = 20f; // time in seconds chat line appears on the screen
        public static float ChatLineFade = 1.5f; // number of seconds before expiry to start fading chat off
        public static OpenTK.Graphics.Color4 ChatBackground = new OpenTK.Graphics.Color4(0f, 0f, 0f, 0.75f);

        RadegastInstance Instance;
        SceneWindow Window;
        float runningTime;
        
        Queue<ChatLine> chatLines;
        int screenWidth, screenHeight;

        public ChatOverlay(RadegastInstance instance, SceneWindow window)
        {
            this.Instance = instance;
            this.Window = window;
            Instance.TabConsole.MainChatManger.ChatLineAdded += new EventHandler<ChatLineAddedArgs>(MainChatManger_ChatLineAdded);
            chatLines = new Queue<ChatLine>();
        }

        public void Dispose()
        {
            Instance.TabConsole.MainChatManger.ChatLineAdded -= new EventHandler<ChatLineAddedArgs>(MainChatManger_ChatLineAdded);

            if (chatLines != null)
            {
                lock (chatLines)
                {
                    foreach (var line in chatLines)
                    {
                        line.Dispose();
                    }
                }
                chatLines = null;
            }
        }

        void MainChatManger_ChatLineAdded(object sender, ChatLineAddedArgs e)
        {
            lock (chatLines)
            {
                chatLines.Enqueue(new ChatLine(e.Item, runningTime));
            }
        }

        public static OpenTK.Graphics.Color4 GetColorForStyle(ChatBufferTextStyle style)
        {
            switch (style)
            {
                case ChatBufferTextStyle.StatusBlue:
                    return new OpenTK.Graphics.Color4(0.2f, 0.2f, 1f, 1f);
                case ChatBufferTextStyle.StatusDarkBlue:
                    return new OpenTK.Graphics.Color4(0f, 0f, 1f, 1f);
                case ChatBufferTextStyle.ObjectChat:
                    return new OpenTK.Graphics.Color4(0.7f, 0.9f, 0.7f, 1f);
                case ChatBufferTextStyle.OwnerSay:
                    return new OpenTK.Graphics.Color4(0.99f, 0.99f, 0.69f, 1f);
                case ChatBufferTextStyle.Alert:
                    return new OpenTK.Graphics.Color4(0.8f, 1f, 1f, 1f);
                case ChatBufferTextStyle.Error:
                    return new OpenTK.Graphics.Color4(0.82f, 0.27f, 0.27f, 1f);

                default:
                    return new OpenTK.Graphics.Color4(1f, 1f, 1f, 1f);

            }
        }


        public void RenderChat(float time, RenderPass pass)
        {
            runningTime += time;
            if (chatLines.Count == 0) return;

            int c = 0;
            int expired = 0;
            screenWidth = Window.Viewport[2];
            screenHeight = Window.Viewport[3];
            ChatLine[] lines;

            lock (chatLines)
            {
                lines = chatLines.ToArray();
                c = lines.Length;
                for (int i = 0; i < c; i++)
                {
                    if ((runningTime - lines[i].TimeAdded) > ChatLineTimeOnScreen)
                    {
                        expired++;
                        ChatLine goner = chatLines.Dequeue();
                        goner.Dispose();
                    }
                    else
                    {
                        break;
                    }
                }
                if (expired > 0)
                {
                    lines = chatLines.ToArray();
                    c = lines.Length;
                }
            }

            if (c == 0)
            {
                runningTime = 0f;
                return;
            }

            int maxWidth = (int)((float)screenWidth * 0.7f);

            int actualMaxWidth = 0;
            int height = 0;
            for (int i = 0; i < c; i++)
            {
                lines[i].PrepareText(maxWidth);
                if (lines[i].TextWidth > actualMaxWidth) actualMaxWidth = lines[i].TextWidth;
                height += lines[i].TextHeight;
            }

            int x = 5;
            int y = 5;
            GL.Enable(EnableCap.Texture2D);
            GL.Color4(ChatBackground);
            RHelp.Draw2DBox(x, y, actualMaxWidth + 6, height + 10, 1f);
            for (int i = c - 1; i >= 0; i--)
            {
                ChatLine line = lines[i];
                OpenTK.Graphics.Color4 color = GetColorForStyle(line.Style);
                float remain = ChatLineTimeOnScreen - (runningTime - line.TimeAdded);
                if (remain < ChatLineFade)
                {
                    color.A = remain / ChatLineFade;
                }
                GL.Color4(color);
                line.Render(x + 3, y + 5);
                y += line.TextHeight;
            }
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.Disable(EnableCap.Texture2D);
            GL.Color4(1f, 1f, 1f, 1f);
        }
    }

    public class ChatLine : IDisposable
    {
        public float TimeAdded;
        
        public int TextWidth;
        public int TextHeight;
        public int ImgWidth;
        public int ImgHeight;

        public ChatBufferTextStyle Style { get { return item.Style; } }

        int textureID = -1;
        int widthForTextureGenerated = -1;
        ChatBufferItem item;
       
        public ChatLine(ChatBufferItem item, float timeAdded)
        {
            this.item = item;
            this.TimeAdded = timeAdded;
        }

        public void Dispose()
        {
            if (textureID > 0)
            {
                GL.DeleteTexture(textureID);
                textureID = -1;
            }
        }

        public void PrepareText(int maxWidth)
        {
            if (maxWidth != widthForTextureGenerated)
            {
                string txt = item.From + item.Text;

                // If we're modified and have texture already delete it from graphics card
                if (textureID > 0)
                {
                    GL.DeleteTexture(textureID);
                    textureID = -1;
                }

                TextFormatFlags flags = TextFormatFlags.Top | TextFormatFlags.Left | TextFormatFlags.WordBreak;

                Size s = TextRenderer.MeasureText(
                    txt,
                    ChatOverlay.ChatFont,
                    new Size(maxWidth, 2000), flags);

                ImgWidth = TextWidth = s.Width;
                ImgHeight = TextHeight = s.Height;

                if (!RenderSettings.TextureNonPowerOfTwoSupported)
                {
                    ImgWidth = RHelp.NextPow2(TextWidth);
                    ImgHeight = RHelp.NextPow2(TextHeight);
                }

                Bitmap img = new Bitmap(
                    ImgWidth,
                    ImgHeight,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                
                Graphics g = Graphics.FromImage(img);

                TextRenderer.DrawText(
                    g,
                    txt,
                    ChatOverlay.ChatFont,
                    new Rectangle(0, ImgHeight - TextHeight, TextWidth + 2, TextHeight + 2),
                    Color.White,
                    Color.Transparent,
                    flags);

                widthForTextureGenerated = maxWidth;
                textureID = RHelp.GLLoadImage(img, true, false);
                g.Dispose();
                img.Dispose();
            }
        }

        public void Render(int x, int y)
        {
            if (textureID == -1) return;
            GL.BindTexture(TextureTarget.Texture2D, textureID);
            RHelp.Draw2DBox(x, y, ImgWidth, ImgHeight, 0f);
        }
    }
}
