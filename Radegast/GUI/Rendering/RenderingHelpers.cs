﻿// 
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
// $Id: RenderingHelpers.cs 1136 2011-09-05 22:45:11Z latifer $
//

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using OpenMetaverse;
using OpenMetaverse.Rendering;

namespace Radegast.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Color4b
    {
        public byte R;
        public byte G;
        public byte B;
        public byte A;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct ColorVertex
    {
        [FieldOffset(0)]
        public Vertex Vertex;
        [FieldOffset(32)]
        public Color4b Color;
        public static int Size = 36;
    }

    public class TextureInfo
    {
        public System.Drawing.Image Texture;
        public int TexturePointer;
        public bool HasAlpha;
        public bool FullAlpha;
        public bool IsMask;
        public bool IsInvisible;
        public UUID TextureID;
        public bool FetchFailed;
    }

    public class TextureLoadItem
    {
        public FaceData Data;
        public Primitive Prim;
        public Primitive.TextureEntryFace TeFace;
        public byte[] TextureData = null;
        public byte[] TGAData = null;
        public bool LoadAssetFromCache = false;
    }

    public enum RenderPass
    {
        Picking,
        Simple,
        Alpha,
        Invisible
    }

    public enum SceneObjectType
    {
        None,
        Primitive,
        Avatar,
    }

    /// <summary>
    /// Base class for all scene objects
    /// </summary>
    public abstract class SceneObject : IComparable, IDisposable
    {
        #region Public fields
        /// <summary>Interpolated local position of the object</summary>
        public Vector3 InterpolatedPosition;
        /// <summary>Interpolated local rotation of the object/summary>
        public Quaternion InterpolatedRotation;
        /// <summary>Rendered position of the object in the region</summary>
        public Vector3 RenderPosition;
        /// <summary>Rendered rotationm of the object in the region</summary>
        public Quaternion RenderRotation;
        /// <summary>Per frame calculated square of the distance from camera</summary>
        public float DistanceSquared;
        /// <summary>Bounding volume of the object</summary>
        public BoundingVolume BoundingVolume;
        /// <summary>Was the sim position and distance from camera calculated during this frame</summary>
        public bool PositionCalculated;
        /// <summary>Scene object type</summary>
        public SceneObjectType Type = SceneObjectType.None;
        /// <summary>Libomv primitive</summary>
        public virtual Primitive BasePrim { get; set; }
        /// <summary>Were initial initialization tasks done</summary>
        public bool Initialized;
        /// <summary>Is this object disposed</summary>
        public bool IsDisposed = false;
        public int AlphaQueryID = -1;
        public int SimpleQueryID = -1;
        public bool HasAlphaFaces;
        public bool HasSimpleFaces;
        public bool HasInvisibleFaces;

        #endregion Public fields

        uint previousParent = uint.MaxValue;

        /// <summary>
        /// Cleanup resources used
        /// </summary>
        public virtual void Dispose()
        {
            IsDisposed = true;
        }

        /// <summary>
        /// Task performed the fist time object is set for rendering
        /// </summary>
        public virtual void Initialize()
        {
            RenderPosition = InterpolatedPosition = BasePrim.Position;
            RenderRotation = InterpolatedRotation = BasePrim.Rotation;
            Initialized = true;
        }

        /// <summary>
        /// Perform per frame tasks
        /// </summary>
        /// <param name="time">Time since the last call (last frame time in seconds)</param>
        public virtual void Step(float time)
        {
            if (BasePrim == null) return;

            // Don't interpolate when parent changes (sit/stand link/unlink)
            if (previousParent != BasePrim.ParentID)
            {
                previousParent = BasePrim.ParentID;
                InterpolatedPosition = BasePrim.Position;
                InterpolatedRotation = BasePrim.Rotation;
                return;
            }

            // Linear velocity and acceleration
            if (BasePrim.Velocity != Vector3.Zero)
            {
                BasePrim.Position = InterpolatedPosition = BasePrim.Position + BasePrim.Velocity * time
                    * 0.98f * RadegastInstance.GlobalInstance.Client.Network.CurrentSim.Stats.Dilation;
                BasePrim.Velocity += BasePrim.Acceleration * time;
            }
            else if (InterpolatedPosition != BasePrim.Position)
            {
                InterpolatedPosition = RHelp.Smoothed1stOrder(InterpolatedPosition, BasePrim.Position, time);
            }

            // Angular velocity (target omega)
            if (BasePrim.AngularVelocity != Vector3.Zero)
            {
                Vector3 angVel = BasePrim.AngularVelocity;
                float angle = time * angVel.Length();
                Quaternion dQ = Quaternion.CreateFromAxisAngle(angVel, angle);
                InterpolatedRotation = dQ * InterpolatedRotation;
            }
            else if (InterpolatedRotation != BasePrim.Rotation && !(this is RenderAvatar))
            {
                InterpolatedRotation = Quaternion.Slerp(InterpolatedRotation, BasePrim.Rotation, time * 10f);
                if (1f - Math.Abs(Quaternion.Dot(InterpolatedRotation, BasePrim.Rotation)) < 0.0001)
                    InterpolatedRotation = BasePrim.Rotation;
            }
            else
            {
                InterpolatedRotation = BasePrim.Rotation;
            }
        }

        /// <summary>
        /// Render scene object
        /// </summary>
        /// <param name="pass">Which pass are we currently in</param>
        /// <param name="pickingID">ID used to identify which object was picked</param>
        /// <param name="scene">Main scene renderer</param>
        /// <param name="time">Time it took to render the last frame</param>
        public virtual void Render(RenderPass pass, int pickingID, SceneWindow scene, float time)
        {
        }

        /// <summary>
        /// Implementation of the IComparable interface
        /// used for sorting by distance
        /// </summary>
        /// <param name="other">Object we are comparing to</param>
        /// <returns>Result of the comparison</returns>
        public virtual int CompareTo(object other)
        {
            SceneObject o = (SceneObject)other;
            if (this.DistanceSquared < o.DistanceSquared)
                return -1;
            else if (this.DistanceSquared > o.DistanceSquared)
                return 1;
            else
                return 0;
        }

        #region Occlusion queries
        public void StartQuery(RenderPass pass)
        {
            if (!RenderSettings.OcclusionCullingEnabled) return;

            if (pass == RenderPass.Simple)
            {
                StartSimpleQuery();
            }
            else if (pass == RenderPass.Alpha)
            {
                StartAlphaQuery();
            }
        }

        public void EndQuery(RenderPass pass)
        {
            if (!RenderSettings.OcclusionCullingEnabled) return;

            if (pass == RenderPass.Simple)
            {
                EndSimpleQuery();
            }
            else if (pass == RenderPass.Alpha)
            {
                EndAlphaQuery();
            }
        }

        public void StartAlphaQuery()
        {
            if (!RenderSettings.OcclusionCullingEnabled) return;

            if (AlphaQueryID == -1)
            {
                Compat.GenQueries(out AlphaQueryID);
            }
            if (AlphaQueryID > 0)
            {
                Compat.BeginQuery(QueryTarget.SamplesPassed, AlphaQueryID);
            }
        }

        public void EndAlphaQuery()
        {
            if (!RenderSettings.OcclusionCullingEnabled) return;

            if (AlphaQueryID > 0)
            {
                Compat.EndQuery(QueryTarget.SamplesPassed);
            }
        }

        public void StartSimpleQuery()
        {
            if (!RenderSettings.OcclusionCullingEnabled) return;

            if (SimpleQueryID == -1)
            {
                Compat.GenQueries(out SimpleQueryID);
            }
            if (SimpleQueryID > 0)
            {
                Compat.BeginQuery(QueryTarget.SamplesPassed, SimpleQueryID);
            }
        }

        public void EndSimpleQuery()
        {
            if (!RenderSettings.OcclusionCullingEnabled) return;

            if (SimpleQueryID > 0)
            {
                Compat.EndQuery(QueryTarget.SamplesPassed);
            }
        }

        public bool Occluded()
        {
            if (!RenderSettings.OcclusionCullingEnabled) return false;

            if (HasInvisibleFaces) return false;

            if ((SimpleQueryID == -1 && AlphaQueryID == -1))
            {
                return false;
            }

            if ((!HasAlphaFaces && !HasSimpleFaces)) return true;

            int samples = 1;
            if (HasSimpleFaces && SimpleQueryID > 0)
            {
                Compat.GetQueryObject(SimpleQueryID, GetQueryObjectParam.QueryResult, out samples);
            }
            if (HasSimpleFaces && samples > 0)
            {
                return false;
            }

            samples = 1;
            if (HasAlphaFaces && AlphaQueryID > 0)
            {
                Compat.GetQueryObject(AlphaQueryID, GetQueryObjectParam.QueryResult, out samples);
            }
            if (HasAlphaFaces && samples > 0)
            {
                return false;
            }

            return true;
        }
        #endregion Occlusion queries
    }

    public static class RHelp
    {
        public static readonly Vector3 InvalidPosition = new Vector3(99999f, 99999f, 99999f);
        static float t1 = 0.075f;
        static float t2 = t1 / 5.7f;

        public static Vector3 Smoothed1stOrder(Vector3 curPos, Vector3 targetPos, float lastFrameTime)
        {
            int numIterations = (int)(lastFrameTime * 100);
            do
            {
                curPos += (targetPos - curPos) * t1;
                numIterations--;
            }
            while (numIterations > 0);
            if (Vector3.DistanceSquared(curPos, targetPos) < 0.00001f)
            {
                curPos = targetPos;
            }
            return curPos;
        }

        public static Vector3 Smoothed2ndOrder(Vector3 curPos, Vector3 targetPos, ref Vector3 accel, float lastFrameTime)
        {
            int numIterations = (int)(lastFrameTime * 100);
            do
            {
                accel += (targetPos - accel - curPos) * t1;
                curPos += accel * t2;
                numIterations--;
            }
            while (numIterations > 0);
            if (Vector3.DistanceSquared(curPos, targetPos) < 0.00001f)
            {
                curPos = targetPos;
            }
            return curPos;
        }

        public static OpenTK.Vector2 TKVector3(Vector2 v)
        {
            return new OpenTK.Vector2(v.X, v.Y);
        }

        public static OpenTK.Vector3 TKVector3(Vector3 v)
        {
            return new OpenTK.Vector3(v.X, v.Y, v.Z);
        }

        public static OpenTK.Vector4 TKVector3(Vector4 v)
        {
            return new OpenTK.Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Vector2 OMVVector2(OpenTK.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 OMVVector3(OpenTK.Vector3 v)
        {
            return new Vector3(v.X, v.Y, v.Z);
        }

        public static Vector4 OMVVector4(OpenTK.Vector4 v)
        {
            return new Vector4(v.X, v.Y, v.Z, v.W);
        }

        public static Color WinColor(OpenTK.Graphics.Color4 color)
        {
            return Color.FromArgb((int)(color.A * 255), (int)(color.R * 255), (int)(color.G * 255), (int)(color.B * 255));
        }

        public static Color WinColor(Color4 color)
        {
            return Color.FromArgb((int)(color.A * 255), (int)(color.R * 255), (int)(color.G * 255), (int)(color.B * 255));
        }

        public static int NextPow2(int start)
        {
            int pow = 1;
            while (pow < start) pow *= 2;
            return pow;
        }

        #region Cached image save and load
        public static readonly string RAD_IMG_MAGIC = "radegast_img";

        public static bool LoadCachedImage(UUID textureID, out byte[] tgaData, out bool hasAlpha, out bool fullAlpha, out bool isMask)
        {
            tgaData = null;
            hasAlpha = fullAlpha = isMask = false;

            try
            {
                string fname = System.IO.Path.Combine(RadegastInstance.GlobalInstance.Client.Settings.ASSET_CACHE_DIR, string.Format("{0}.rzi", textureID));
                //string fname = System.IO.Path.Combine(".", string.Format("{0}.rzi", textureID));

                using (var f = File.Open(fname, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] header = new byte[36];
                    int i = 0;
                    f.Read(header, 0, header.Length);

                    // check if the file is starting with magic string
                    if (RAD_IMG_MAGIC != Utils.BytesToString(header, 0, RAD_IMG_MAGIC.Length))
                        return false;
                    i += RAD_IMG_MAGIC.Length;

                    if (header[i++] != 1) // check version
                        return false;

                    hasAlpha = header[i++] == 1;
                    fullAlpha = header[i++] == 1;
                    isMask = header[i++] == 1;

                    int uncompressedSize = Utils.BytesToInt(header, i);
                    i += 4;

                    textureID = new UUID(header, i);
                    i += 16;

                    tgaData = new byte[uncompressedSize];
                    using (var compressed = new DeflateStream(f, CompressionMode.Decompress))
                    {
                        int read = 0;
                        while ((read = compressed.Read(tgaData, read, uncompressedSize - read)) > 0) ;
                    }
                }

                return true;
            }
            catch (FileNotFoundException) { }
            catch (Exception ex)
            {
                Logger.DebugLog(string.Format("Failed to load radegast cache file {0}: {1}", textureID, ex.Message));
            }
            return false;
        }

        public static bool SaveCachedImage(byte[] tgaData, UUID textureID, bool hasAlpha, bool fullAlpha, bool isMask)
        {
            try
            {
                string fname = System.IO.Path.Combine(RadegastInstance.GlobalInstance.Client.Settings.ASSET_CACHE_DIR, string.Format("{0}.rzi", textureID));
                //string fname = System.IO.Path.Combine(".", string.Format("{0}.rzi", textureID));

                using (var f = File.Open(fname, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    int i = 0;
                    // magic header
                    f.Write(Utils.StringToBytes(RAD_IMG_MAGIC), 0, RAD_IMG_MAGIC.Length);
                    i += RAD_IMG_MAGIC.Length;

                    // version
                    f.WriteByte((byte)1);
                    i++;

                    // texture info
                    f.WriteByte(hasAlpha ? (byte)1 : (byte)0);
                    f.WriteByte(fullAlpha ? (byte)1 : (byte)0);
                    f.WriteByte(isMask ? (byte)1 : (byte)0);
                    i += 3;

                    // texture size
                    byte[] uncompressedSize = Utils.IntToBytes(tgaData.Length);
                    f.Write(uncompressedSize, 0, uncompressedSize.Length);
                    i += uncompressedSize.Length;

                    // texture id
                    byte[] id = new byte[16];
                    textureID.ToBytes(id, 0);
                    f.Write(id, 0, 16);
                    i += 16;

                    // compressed texture data
                    using (var compressed = new DeflateStream(f, CompressionMode.Compress))
                    {
                        compressed.Write(tgaData, 0, tgaData.Length);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Logger.DebugLog(string.Format("Failed to save radegast cache file {0}: {1}", textureID, ex.Message));
                return false;
            }
        }
        #endregion Cached image save and load

        #region Static vertices and indices for a cube (used for bounding box drawing)
        /**********************************************
          5 --- 4
         /|    /|
        1 --- 0 |
        | 6 --| 7
        |/    |/
        2 --- 3
        ***********************************************/
        public static readonly float[] CubeVertices = new float[]
        {
             0.5f,  0.5f,  0.5f, // 0
	        -0.5f,  0.5f,  0.5f, // 1
	        -0.5f, -0.5f,  0.5f, // 2
	         0.5f, -0.5f,  0.5f, // 3
	         0.5f,  0.5f, -0.5f, // 4
	        -0.5f,  0.5f, -0.5f, // 5
	        -0.5f, -0.5f, -0.5f, // 6
	         0.5f, -0.5f, -0.5f  // 7
        };

        public static readonly ushort[] CubeIndices = new ushort[]
        {
            0, 1, 2, 3,     // Front Face
	        4, 5, 6, 7,     // Back Face
	        1, 2, 6, 5,     // Left Face
	        0, 3, 7, 4,     // Right Face
	        0, 1, 5, 4,     // Top Face
	        2, 3, 7, 6      // Bottom Face
        };
        #endregion Static vertices and indices for a cube (used for bounding box drawing)

        public static int GLLoadImage(Bitmap bitmap, bool hasAlpha)
        {
            return GLLoadImage(bitmap, hasAlpha, true);
        }

        public static int GLLoadImage(Bitmap bitmap, bool hasAlpha, bool useMipmap)
        {
            useMipmap = useMipmap && RenderSettings.HasMipmap;
            int ret = -1;
            GL.GenTextures(1, out ret);
            GL.BindTexture(TextureTarget.Texture2D, ret);

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
            if (useMipmap)
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.GenerateMipmap, 1);
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
            }
            else
            {
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            }

            bitmap.UnlockBits(bitmapData);
            return ret;
        }

        public static void Draw2DBox(float x, float y, float width, float height, float depth)
        {
            GL.Begin(BeginMode.Quads);
            {
                GL.TexCoord2(0, 1);
                GL.Vertex3(x, y, depth);
                GL.TexCoord2(1, 1);
                GL.Vertex3(x + width, y, depth);
                GL.TexCoord2(1, 0);
                GL.Vertex3(x + width, y + height, depth);
                GL.TexCoord2(0, 0);
                GL.Vertex3(x, y + height, depth);
            }
            GL.End();
        }

        public static void ResetMaterial()
        {
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, new float[] { 0.2f, 0.2f, 0.2f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, new float[] { 0.8f, 0.8f, 0.8f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, new float[] { 0f, 0f, 0f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Emission, new float[] { 0f, 0f, 0f, 1.0f });
            GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, 0f);
            ShaderProgram.Stop();
        }
    }

    /// <summary>
    /// Represents camera object
    /// </summary>
    public class Camera
    {
        /// <summary>
        /// Indicates that there was manual camera movement, stop tracking objects
        /// </summary>
        public bool Manual;
        Vector3 mPosition;
        Vector3 mFocalPoint;
        bool mModified;

        /// <summary>Camera position</summary>
        public Vector3 Position { get { return mPosition; } set { mPosition = value; Modify(); } }
        /// <summary>Camera target</summary>
        public Vector3 FocalPoint { get { return mFocalPoint; } set { mFocalPoint = value; Modify(); } }
        /// <summary>Zoom level</summary>
        public float Zoom;
        /// <summary>Draw distance</summary>
        public float Far;
        /// <summary>Has camera been modified</summary>
        public bool Modified { get { return mModified; } set { mModified = value; } }

        public float TimeToTarget = 0f;

        public Vector3 RenderPosition;
        public Vector3 RenderFocalPoint;

        void Modify()
        {
            mModified = true;
        }

        public void Step(float time)
        {
            if (RenderPosition != Position)
            {
                RenderPosition = RHelp.Smoothed1stOrder(RenderPosition, Position, time);
                Modified = true;
            }
            if (RenderFocalPoint != FocalPoint)
            {
                RenderFocalPoint = RHelp.Smoothed1stOrder(RenderFocalPoint, FocalPoint, time);
                Modified = true;
            }
        }

        [Obsolete("Use Step(), left in here for reference")]
        public void Step2(float time)
        {
            TimeToTarget -= time;
            if (TimeToTarget <= time)
            {
                EndMove();
                return;
            }

            mModified = true;

            float pctElapsed = time / TimeToTarget;

            if (RenderPosition != Position)
            {
                float distance = Vector3.Distance(RenderPosition, Position);
                RenderPosition = Vector3.Lerp(RenderPosition, Position, distance * pctElapsed);
            }

            if (RenderFocalPoint != FocalPoint)
            {
                RenderFocalPoint = Interpolate(RenderFocalPoint, FocalPoint, pctElapsed);
            }
        }

        Vector3 Interpolate(Vector3 start, Vector3 end, float fraction)
        {
            float distance = Vector3.Distance(start, end);
            Vector3 direction = end - start;
            return start + direction * fraction;
        }

        public void EndMove()
        {
            mModified = true;
            TimeToTarget = 0;
            RenderPosition = Position;
            RenderFocalPoint = FocalPoint;
        }

        public void Pan(float deltaX, float deltaY)
        {
            Manual = true;
            Vector3 direction = Position - FocalPoint;
            direction.Normalize();
            Vector3 vy = direction % Vector3.UnitZ;
            Vector3 vx = vy % direction;
            Vector3 vxy = vx * deltaY + vy * deltaX;
            Position += vxy;
            FocalPoint += vxy;
        }

        public void Rotate(float delta, bool horizontal)
        {
            Manual = true;
            Vector3 direction = Position - FocalPoint;
            if (horizontal)
            {
                Position = FocalPoint + direction * new Quaternion(0f, 0f, (float)Math.Sin(delta), (float)Math.Cos(delta));
            }
            else
            {
                Position = FocalPoint + direction * Quaternion.CreateFromAxisAngle(direction % Vector3.UnitZ, delta);
            }
        }

        public void MoveToTarget(float delta)
        {
            Manual = true;
            Position += (Position - FocalPoint) * delta;
        }

        /// <summary>
        /// Sets the world in perspective of the camera
        /// </summary>
        public void LookAt()
        {
            OpenTK.Matrix4 lookAt = OpenTK.Matrix4.LookAt(
                RenderPosition.X, RenderPosition.Y, RenderPosition.Z,
                RenderFocalPoint.X, RenderFocalPoint.Y, RenderFocalPoint.Z,
                0f, 0f, 1f);
            GL.MultMatrix(ref lookAt);
        }
    }

    public static class MeshToOBJ
    {
        public static bool MeshesToOBJ(Dictionary<uint, FacetedMesh> meshes, string filename)
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

            int primNr = 0;
            foreach (FacetedMesh mesh in meshes.Values)
            {
                for (int j = 0; j < mesh.Faces.Count; j++)
                {
                    Face face = mesh.Faces[j];

                    if (face.Vertices.Count > 2)
                    {
                        string mtlName = String.Format("material{0}-{1}", primNr, face.ID);
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

                        obj.AppendFormat("g face{0}-{1}{2}", primNr, face.ID, Environment.NewLine);

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
                primNr++;
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

        public static float[] CreateSRTMatrix(Vector3 scale, Quaternion q, Vector3 pos)
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

            mat[0] = (1.0f - (yy + zz)) * scale.X;
            mat[1] = (xy - wz) * scale.X;
            mat[2] = (xz + wy) * scale.X;
            mat[3] = 0.0f;

            mat[4] = (xy + wz) * scale.Y;
            mat[5] = (1.0f - (xx + zz)) * scale.Y;
            mat[6] = (yz - wx) * scale.Y;
            mat[7] = 0.0f;

            mat[8] = (xz - wy) * scale.Z;
            mat[9] = (yz + wx) * scale.Z;
            mat[10] = (1.0f - (xx + yy)) * scale.Z;
            mat[11] = 0.0f;

            //Positional parts
            mat[12] = pos.X;
            mat[13] = pos.Y;
            mat[14] = pos.Z;
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

        public static float[] Lerp(float[] matrix1, float[] matrix2, float amount)
        {

            float[] lerp = new float[16];
            //Probably not doing this as a loop is cheaper(unrolling)
            //also for performance we probably should not create new objects
            // but meh.
            for (int x = 0; x < 16; x++)
            {
                lerp[x] = matrix1[x] + ((matrix2[x] - matrix1[x]) * amount);
            }

            return lerp;
        }


        public static bool GluProject(OpenTK.Vector3 objPos, OpenTK.Matrix4 modelMatrix, OpenTK.Matrix4 projMatrix, int[] viewport, out OpenTK.Vector3 screenPos)
        {
            OpenTK.Vector4 _in;
            OpenTK.Vector4 _out;

            _in.X = objPos.X;
            _in.Y = objPos.Y;
            _in.Z = objPos.Z;
            _in.W = 1.0f;

            _out = OpenTK.Vector4.Transform(_in, modelMatrix);
            _in = OpenTK.Vector4.Transform(_out, projMatrix);

            if (_in.W <= 0.0)
            {
                screenPos = OpenTK.Vector3.Zero;
                return false;
            }

            _in.X /= _in.W;
            _in.Y /= _in.W;
            _in.Z /= _in.W;
            /* Map x, y and z to range 0-1 */
            _in.X = _in.X * 0.5f + 0.5f;
            _in.Y = _in.Y * 0.5f + 0.5f;
            _in.Z = _in.Z * 0.5f + 0.5f;

            /* Map x,y to viewport */
            _in.X = _in.X * viewport[2] + viewport[0];
            _in.Y = _in.Y * viewport[3] + viewport[1];

            screenPos.X = _in.X;
            screenPos.Y = _in.Y;
            screenPos.Z = _in.Z;

            return true;
        }

        public static bool GluUnProject(float winx, float winy, float winz, OpenTK.Matrix4 modelMatrix, OpenTK.Matrix4 projMatrix, int[] viewport, out OpenTK.Vector3 pos)
        {
            OpenTK.Matrix4 finalMatrix;
            OpenTK.Vector4 _in;
            OpenTK.Vector4 _out;

            finalMatrix = OpenTK.Matrix4.Mult(modelMatrix, projMatrix);

            finalMatrix.Invert();

            _in.X = winx;
            _in.Y = winy;
            _in.Z = winz;
            _in.W = 1.0f;

            /* Map x and y from window coordinates */
            _in.X = (_in.X - viewport[0]) / viewport[2];
            _in.Y = (_in.Y - viewport[1]) / viewport[3];

            pos = OpenTK.Vector3.Zero;

            /* Map to range -1 to 1 */
            _in.X = _in.X * 2 - 1;
            _in.Y = _in.Y * 2 - 1;
            _in.Z = _in.Z * 2 - 1;

            //__gluMultMatrixVecd(finalMatrix, _in, _out);
            // check if this works:
            _out = OpenTK.Vector4.Transform(_in, finalMatrix);

            if (_out.W == 0.0f)
                return false;
            _out.X /= _out.W;
            _out.Y /= _out.W;
            _out.Z /= _out.W;
            pos.X = _out.X;
            pos.Y = _out.Y;
            pos.Z = _out.Z;
            return true;
        }

        public static double[] AbovePlane(double height)
        {
            return new double[] { 0, 0, 1, -height };
        }

        public static double[] BelowPlane(double height)
        {
            return new double[] { 0, 0, -1, height };
        }
    }

    /*
     *  Helper classs for reading the static VFS file, call 
     *  staticVFS.readVFSheaders() with the path to the static_data.db2 and static_index.db2 files
     *  and it will pass and dump in to openmetaverse_data for you
     *  This should only be needed to be used if LL update the static VFS in order to refresh our data
     */

    class VFSblock
    {
        public int mLocation;
        public int mLength;
        public int mAccessTime;
        public UUID mFileID;
        public int mSize;
        public AssetType mAssetType;

        public int readblock(byte[] blockdata, int offset)
        {

            BitPack input = new BitPack(blockdata, offset);
            mLocation = input.UnpackInt();
            mLength = input.UnpackInt();
            mAccessTime = input.UnpackInt();
            mFileID = input.UnpackUUID();
            int filetype = input.UnpackShort();
            mAssetType = (AssetType)filetype;
            mSize = input.UnpackInt();
            offset += 34;

            Logger.Log(String.Format("Found header for {0} type {1} length {2} at {3}", mFileID, mAssetType, mSize, mLocation), Helpers.LogLevel.Info);

            return offset;
        }

    }

    public class staticVFS
    {
        public static void readVFSheaders(string datafile, string indexfile)
        {
            FileStream datastream;
            FileStream indexstream;

            datastream = File.Open(datafile, FileMode.Open);
            indexstream = File.Open(indexfile, FileMode.Open);

            int offset = 0;

            byte[] blockdata = new byte[indexstream.Length];
            indexstream.Read(blockdata, 0, (int)indexstream.Length);

            while (offset < indexstream.Length)
            {
                VFSblock block = new VFSblock();
                offset = block.readblock(blockdata, offset);

                FileStream writer = File.Open(OpenMetaverse.Settings.RESOURCE_DIR + System.IO.Path.DirectorySeparatorChar + block.mFileID.ToString(), FileMode.Create);
                byte[] data = new byte[block.mSize];
                datastream.Seek(block.mLocation, SeekOrigin.Begin);
                datastream.Read(data, 0, block.mSize);
                writer.Write(data, 0, block.mSize);
                writer.Close();
            }

        }
    }
}
