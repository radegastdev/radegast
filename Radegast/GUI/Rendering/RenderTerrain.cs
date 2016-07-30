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
//       this software without specific prior CreateReflectionTexture permission.
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
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using OpenMetaverse;
using OpenMetaverse.Rendering;

namespace Radegast.Rendering
{
    public class RenderTerrain : SceneObject
    {
        RadegastInstance Instance;
        GridClient Client { get { return Instance.Client; } }

        public bool Modified = true;
        float[,] heightTable = new float[256, 256];
        Face terrainFace;
        uint[] terrainIndices;
        ColorVertex[] terrainVertices;
        int terrainTexture = -1;
        bool fetchingTerrainTexture = false;
        Bitmap terrainImage = null;
        int terrainVBO = -1;
        int terrainIndexVBO = -1;
        bool terrainVBOFailed = false;
        bool terrainInProgress = false;
        bool terrainTextureNeedsUpdate = false;
        float terrainTimeSinceUpdate = RenderSettings.MinimumTimeBetweenTerrainUpdated + 1f; // Update terrain om first run
        MeshmerizerR renderer;
        Simulator sim { get { return Instance.Client.Network.CurrentSim; } }

        public RenderTerrain(RadegastInstance instance)
        {
            this.Instance = instance;
            renderer = new MeshmerizerR();
        }

        public void ResetTerrain()
        {
            ResetTerrain(true);
        }

        public void ResetTerrain(bool removeImage)
        {
            if (terrainImage != null)
            {
                terrainImage.Dispose();
                terrainImage = null;
            }

            if (terrainVBO != -1)
            {
                Compat.DeleteBuffer(terrainVBO);
                terrainVBO = -1;
            }

            if (terrainIndexVBO != -1)
            {
                Compat.DeleteBuffer(terrainIndexVBO);
                terrainIndexVBO = -1;
            }

            if (removeImage)
            {
                if (terrainTexture != -1)
                {
                    GL.DeleteTexture(terrainTexture);
                    terrainTexture = -1;
                }
            }

            fetchingTerrainTexture = false;
            Modified = true;
        }

        private void UpdateTerrain()
        {
            if (sim == null || sim.Terrain == null) return;

            WorkPool.QueueUserWorkItem(sync =>
            {
                int step = 1;

                for (int x = 0; x < 256; x += step)
                {
                    for (int y = 0; y < 256; y += step)
                    {
                        float z = 0;
                        int patchNr = ((int)x / 16) * 16 + (int)y / 16;
                        if (sim.Terrain[patchNr] != null
                            && sim.Terrain[patchNr].Data != null)
                        {
                            float[] data = sim.Terrain[patchNr].Data;
                            z = data[(int)x % 16 * 16 + (int)y % 16];
                        }
                        heightTable[x, y] = z;
                    }
                }

                terrainFace = renderer.TerrainMesh(heightTable, 0f, 255f, 0f, 255f);
                terrainVertices = new ColorVertex[terrainFace.Vertices.Count];
                for (int i = 0; i < terrainFace.Vertices.Count; i++)
                {
                    byte[] part = Utils.IntToBytes(i);
                    terrainVertices[i] = new ColorVertex()
                    {
                        Vertex = terrainFace.Vertices[i],
                        Color = new Color4b()
                        {
                            R = part[0],
                            G = part[1],
                            B = part[2],
                            A = 253 // terrain picking
                        }
                    };
                }
                terrainIndices = new uint[terrainFace.Indices.Count];
                for (int i = 0; i < terrainIndices.Length; i++)
                {
                    terrainIndices[i] = terrainFace.Indices[i];
                }
                terrainInProgress = false;
                Modified = false;
                terrainTextureNeedsUpdate = true;
                terrainTimeSinceUpdate = 0f;
            });
        }

        void UpdateTerrainTexture()
        {
            if (!fetchingTerrainTexture)
            {
                fetchingTerrainTexture = true;
                WorkPool.QueueUserWorkItem(sync =>
                {
                    Simulator sim = Client.Network.CurrentSim;
                    terrainImage = TerrainSplat.Splat(Instance, heightTable,
                        new UUID[] { sim.TerrainDetail0, sim.TerrainDetail1, sim.TerrainDetail2, sim.TerrainDetail3 },
                        new float[] { sim.TerrainStartHeight00, sim.TerrainStartHeight01, sim.TerrainStartHeight10, sim.TerrainStartHeight11 },
                        new float[] { sim.TerrainHeightRange00, sim.TerrainHeightRange01, sim.TerrainHeightRange10, sim.TerrainHeightRange11 });

                    fetchingTerrainTexture = false;
                    terrainTextureNeedsUpdate = false;
                });
            }
        }

        public bool TryGetVertex(int indeex, out ColorVertex picked)
        {
            if (indeex < terrainVertices.Length)
            {
                picked = terrainVertices[indeex];
                return true;
            }
            picked = new ColorVertex();
            return false;
        }

        public override void Render(RenderPass pass, int pickingID, SceneWindow scene, float time)
        {
            terrainTimeSinceUpdate += time;

            if (Modified && terrainTimeSinceUpdate > RenderSettings.MinimumTimeBetweenTerrainUpdated)
            {
                if (!terrainInProgress)
                {
                    terrainInProgress = true;
                    ResetTerrain(false);
                    UpdateTerrain();
                }
            }

            if (terrainTextureNeedsUpdate)
            {
                UpdateTerrainTexture();
            }

            if (terrainIndices == null || terrainVertices == null) return;

            GL.Color3(1f, 1f, 1f);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.NormalArray);
            if (pass == RenderPass.Picking)
            {
                GL.EnableClientState(ArrayCap.ColorArray);
                GL.ShadeModel(ShadingModel.Flat);
            }

            if (terrainImage != null)
            {
                if (terrainTexture != -1)
                {
                    GL.DeleteTexture(terrainTexture);
                }

                terrainTexture = RHelp.GLLoadImage(terrainImage, false);
                terrainImage.Dispose();
                terrainImage = null;
            }

            if (pass != RenderPass.Picking && terrainTexture != -1)
            {
                GL.Enable(EnableCap.Texture2D);
                GL.BindTexture(TextureTarget.Texture2D, terrainTexture);
            }

            if (!RenderSettings.UseVBO || terrainVBOFailed)
            {
                unsafe
                {
                    fixed (float* normalPtr = &terrainVertices[0].Vertex.Normal.X)
                    fixed (float* texPtr = &terrainVertices[0].Vertex.TexCoord.X)
                    fixed (byte* colorPtr = &terrainVertices[0].Color.R)
                    {
                        GL.NormalPointer(NormalPointerType.Float, ColorVertex.Size, (IntPtr)normalPtr);
                        GL.TexCoordPointer(2, TexCoordPointerType.Float, ColorVertex.Size, (IntPtr)texPtr);
                        GL.VertexPointer(3, VertexPointerType.Float, ColorVertex.Size, terrainVertices);
                        if (pass == RenderPass.Picking)
                        {
                            GL.ColorPointer(4, ColorPointerType.UnsignedByte, ColorVertex.Size, (IntPtr)colorPtr);
                        }
                        GL.DrawElements(PrimitiveType.Triangles, terrainIndices.Length, DrawElementsType.UnsignedInt, terrainIndices);
                    }
                }
            }
            else
            {
                if (terrainVBO == -1)
                {
                    Compat.GenBuffers(out terrainVBO);
                    Compat.BindBuffer(BufferTarget.ArrayBuffer, terrainVBO);
                    Compat.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(terrainVertices.Length * ColorVertex.Size), terrainVertices, BufferUsageHint.StaticDraw);
                    if (Compat.BufferSize(BufferTarget.ArrayBuffer) != terrainVertices.Length * ColorVertex.Size)
                    {
                        terrainVBOFailed = true;
                        Compat.BindBuffer(BufferTarget.ArrayBuffer, 0);
                        terrainVBO = -1;
                    }
                }
                else
                {
                    Compat.BindBuffer(BufferTarget.ArrayBuffer, terrainVBO);
                }

                if (terrainIndexVBO == -1)
                {
                    Compat.GenBuffers(out terrainIndexVBO);
                    Compat.BindBuffer(BufferTarget.ElementArrayBuffer, terrainIndexVBO);
                    Compat.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(terrainIndices.Length * sizeof(uint)), terrainIndices, BufferUsageHint.StaticDraw);
                    if (Compat.BufferSize(BufferTarget.ElementArrayBuffer) != terrainIndices.Length * sizeof(uint))
                    {
                        terrainVBOFailed = true;
                        Compat.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                        terrainIndexVBO = -1;
                    }
                }
                else
                {
                    Compat.BindBuffer(BufferTarget.ElementArrayBuffer, terrainIndexVBO);
                }

                if (!terrainVBOFailed)
                {
                    GL.NormalPointer(NormalPointerType.Float, ColorVertex.Size, (IntPtr)12);
                    GL.TexCoordPointer(2, TexCoordPointerType.Float, ColorVertex.Size, (IntPtr)(24));
                    if (pass == RenderPass.Picking)
                    {
                        GL.ColorPointer(4, ColorPointerType.UnsignedByte, ColorVertex.Size, (IntPtr)32);
                    }
                    GL.VertexPointer(3, VertexPointerType.Float, ColorVertex.Size, (IntPtr)(0));

                    GL.DrawElements(PrimitiveType.Triangles, terrainIndices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
                }

                Compat.BindBuffer(BufferTarget.ArrayBuffer, 0);
                Compat.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            }

            if (pass == RenderPass.Picking)
            {
                GL.DisableClientState(ArrayCap.ColorArray);
                GL.ShadeModel(ShadingModel.Smooth);
            }
            else
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.TextureCoordArray);
            GL.DisableClientState(ArrayCap.NormalArray);
        }
    }
}
