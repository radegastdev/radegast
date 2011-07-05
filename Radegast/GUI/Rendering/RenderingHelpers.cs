using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using OpenMetaverse;
using OpenMetaverse.Rendering;

namespace Radegast.Rendering
{
    public class FaceData
    {
        public float[] Vertices;
        public ushort[] Indices;
        public float[] TexCoords;
        public float[] Normals;
        public int PickingID = -1;
        public int VertexVBO = -1;
        public int IndexVBO = -1;
        public TextureInfo TextureInfo = new TextureInfo();
        public BoundingSphere BoundingSphere = new BoundingSphere();
        public static int VertexSize = 32; // sizeof (vertex), 2  x vector3 + 1 x vector2 = 8 floats x 4 bytes = 32 bytes 
        public TextureAnimationInfo AnimInfo;

        public void CheckVBO(Face face)
        {
            if (VertexVBO == -1)
            {
                Vertex[] vArray = face.Vertices.ToArray();
                GL.GenBuffers(1, out VertexVBO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VertexVBO);
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vArray.Length * VertexSize), vArray, BufferUsageHint.StreamDraw);
            }

            if (IndexVBO == -1)
            {
                ushort[] iArray = face.Indices.ToArray();
                GL.GenBuffers(1, out IndexVBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexVBO);
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(iArray.Length * sizeof(ushort)), iArray, BufferUsageHint.StreamDraw);
            }
        }
    }

    public class TextureAnimationInfo
    {
        public Primitive.TextureAnimation PrimAnimInfo;
        public float CurrentFrame;
        public double CurrentTime;
        public bool PingPong;
        float LastTime = 0f;
        float TotalTime = 0f;

        public void Step(double lastFrameTime)
        {
            float numFrames = 1f;
            float fullLength = 1f;

            if (PrimAnimInfo.Length > 0)
            {
                numFrames = PrimAnimInfo.Length;
            }
            else
            {
                numFrames = Math.Max(1f, (float)(PrimAnimInfo.SizeX * PrimAnimInfo.SizeY));
            }

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.PING_PONG) != 0)
            {
                if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.SMOOTH) != 0)
                {
                    fullLength = 2f * numFrames;
                }
                else if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.LOOP) != 0)
                {
                    fullLength = 2f * numFrames - 2f;
                    fullLength = Math.Max(1f, fullLength);
                }
                else
                {
                    fullLength = 2f * numFrames - 1f;
                    fullLength = Math.Max(1f, fullLength);
                }
            }
            else
            {
                fullLength = numFrames;
            }

            float frameCounter;
            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.SMOOTH) != 0)
            {
                frameCounter = (float)lastFrameTime * PrimAnimInfo.Rate + LastTime;
            }
            else
            {
                TotalTime += (float)lastFrameTime;
                frameCounter = TotalTime * PrimAnimInfo.Rate;
            }
            LastTime = frameCounter;

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.LOOP) != 0)
            {
                frameCounter %= fullLength;
            }
            else
            {
                frameCounter = Math.Min(fullLength - 1f, frameCounter);
            }

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.SMOOTH) == 0)
            {
                frameCounter = (float)Math.Floor(frameCounter + 0.01f);
            }

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.PING_PONG) != 0)
            {
                if (frameCounter > numFrames)
                {
                    if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.SMOOTH) != 0)
                    {
                        frameCounter = numFrames - (frameCounter - numFrames);
                    }
                    else
                    {
                        frameCounter = (numFrames - 1.99f) - (frameCounter - numFrames);
                    }
                }
            }

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.REVERSE) != 0)
            {
                if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.SMOOTH) != 0)
                {
                    frameCounter = numFrames - frameCounter;
                }
                else
                {
                    frameCounter = (numFrames - 0.99f) - frameCounter;
                }
            }

            frameCounter += PrimAnimInfo.Start;

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.SMOOTH) == 0)
            {
                frameCounter = (float)Math.Round(frameCounter);
            }


            GL.MatrixMode(MatrixMode.Texture);
            GL.LoadIdentity();

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.ROTATE) != 0)
            {
                GL.Translate(0.5f, 0.5f, 0f);
                GL.Rotate(Utils.RAD_TO_DEG * frameCounter, OpenTK.Vector3d.UnitZ);
                GL.Translate(-0.5f, -0.5f, 0f);
            }
            else if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.SCALE) != 0)
            {
                GL.Scale(frameCounter, frameCounter, 0);
            }
            else // Translate
            {
                float sizeX = Math.Max(1f, (float)PrimAnimInfo.SizeX);
                float sizeY = Math.Max(1f, (float)PrimAnimInfo.SizeY);

                GL.Scale(1f / sizeX, 1f / sizeY, 0);
                GL.Translate(frameCounter % sizeX, Math.Floor(frameCounter / sizeY), 0);
            }

            GL.MatrixMode(MatrixMode.Modelview);
        }

        [Obsolete("Use Step() instead")]
        public void ExperimentalStep(double time)
        {
            int reverseFactor = 1;
            float rate = PrimAnimInfo.Rate;

            if (rate < 0)
            {
                rate = -rate;
                reverseFactor = -reverseFactor;
            }

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.REVERSE) != 0)
            {
                reverseFactor = -reverseFactor;
            }

            CurrentTime += time;
            double totalTime = 1 / rate;

            uint x = Math.Max(1, PrimAnimInfo.SizeX);
            uint y = Math.Max(1, PrimAnimInfo.SizeY);
            uint nrFrames = x * y;

            if (PrimAnimInfo.Length > 0 && PrimAnimInfo.Length < nrFrames)
            {
                nrFrames = (uint)PrimAnimInfo.Length;
            }

            GL.MatrixMode(MatrixMode.Texture);
            GL.LoadIdentity();

            if (CurrentTime >= totalTime)
            {
                CurrentTime = 0;
                CurrentFrame++;
                if (CurrentFrame > nrFrames) CurrentFrame = (uint)PrimAnimInfo.Start;
                if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.PING_PONG) != 0)
                {
                    PingPong = !PingPong;
                }
            }

            float smoothOffset = 0f;

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.SMOOTH) != 0)
            {
                smoothOffset = (float)(CurrentTime / totalTime) * reverseFactor;
            }

            float f = CurrentFrame;
            if (reverseFactor < 0)
            {
                f = nrFrames - CurrentFrame;
            }

            if ((PrimAnimInfo.Flags & Primitive.TextureAnimMode.ROTATE) == 0) // not rotaating
            {
                GL.Scale(1f / x, 1f / y, 0f);
                GL.Translate((f % x) + smoothOffset, f / y, 0);
            }
            else
            {
                smoothOffset = (float)(CurrentTime * PrimAnimInfo.Rate);
                double startAngle = PrimAnimInfo.Start;
                double endAngle = PrimAnimInfo.Length;
                double angle = startAngle + (endAngle - startAngle) * smoothOffset;
                GL.Translate(0.5f, 0.5f, 0f);
                GL.Rotate(Utils.RAD_TO_DEG * angle, OpenTK.Vector3d.UnitZ);
                GL.Translate(-0.5f, -0.5f, 0f);
            }

            GL.MatrixMode(MatrixMode.Modelview);
        }

    }


    public class TextureInfo
    {
        public System.Drawing.Image Texture;
        public int TexturePointer;
        public bool HasAlpha;
        public UUID TextureID;
    }

    public class TextureLoadItem
    {
        public FaceData Data;
        public Primitive Prim;
        public Primitive.TextureEntryFace TeFace;
    }

    public enum RenderPass
    {
        Picking,
        Simple,
        Alpha
    }

    public static class Render
    {
        public static IRendering Plugin;
    }

    public static class RHelp
    {
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
    }

    /// <summary>
    /// Represents camera object
    /// </summary>
    public class Camera
    {
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

        public double TimeToTarget = 0d;

        public Vector3 RenderPosition;
        public Vector3 RenderFocalPoint;

        void Modify()
        {
            mModified = true;
            if (TimeToTarget <= 0)
            {
                RenderPosition = Position;
                RenderFocalPoint = FocalPoint;
            }
        }

        public void Step(double time)
        {
            TimeToTarget -= time;
            if (TimeToTarget <= time)
            {
                EndMove();
                return;
            }

            mModified = true;

            float pctElapsed = (float)(time / TimeToTarget);

            if (RenderPosition != Position)
            {
                float distance = Vector3.Distance(RenderPosition, Position);
                RenderPosition = Vector3.Lerp(RenderPosition, Position, (float)(distance * pctElapsed));
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

        public static float[] CreateScaleMatrix(Vector3 v)
        {
            float[] mat = new float[16];

            mat[0] = v.X;
            mat[5] = v.Y;
            mat[10] = v.Z;
            mat[15] = 1;

            return mat;
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
    }

    public class attachment_point
    {
        public string name;
        public string joint;
        public Vector3 position;
        public Quaternion rotation;
        public int id;
        public int group;

        public GLMesh jointmesh;
        public int jointmeshindex;

    }

    /// <summary>
    /// Subclass of LindenMesh that adds vertex, index, and texture coordinate
    /// arrays suitable for pushing direct to OpenGL
    /// </summary>
    public class GLMesh : LindenMesh
    {
        /// <summary>
        /// Subclass of LODMesh that adds an index array suitable for pushing
        /// direct to OpenGL
        /// </summary>
        /// 

        public int teFaceID;

        new public class LODMesh : LindenMesh.LODMesh
        {
            public ushort[] Indices;

            public override void LoadMesh(string filename)
            {
                base.LoadMesh(filename);

                // Generate the index array
                Indices = new ushort[_numFaces * 3];
                int current = 0;
                for (int i = 0; i < _numFaces; i++)
                {
                    Indices[current++] = (ushort)_faces[i].Indices[0];
                    Indices[current++] = (ushort)_faces[i].Indices[1];
                    Indices[current++] = (ushort)_faces[i].Indices[2];
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public struct GLData
        {
            public float[] Vertices;
            public ushort[] Indices;
            public float[] TexCoords;
            public Vector3 Center;
        }

        public static GLData baseRenderData;
        public GLData RenderData;

        public GLMesh(string name)
            : base(name)
        {
        }

        public void setMeshPos(Vector3 pos)
        {
            _position = pos;
        }

        public void setMeshRot(Vector3 rot)
        {
            _rotationAngles = rot;
        }

        public override void LoadMesh(string filename)
        {
            base.LoadMesh(filename);

            float minX, minY, minZ;
            minX = minY = minZ = Single.MaxValue;
            float maxX, maxY, maxZ;
            maxX = maxY = maxZ = Single.MinValue;

            // Generate the vertex array
            RenderData.Vertices = new float[_numVertices * 3];
            int current = 0;
            for (int i = 0; i < _numVertices; i++)
            {
                RenderData.Vertices[current++] = _vertices[i].Coord.X;
                RenderData.Vertices[current++] = _vertices[i].Coord.Y;
                RenderData.Vertices[current++] = _vertices[i].Coord.Z;

                if (_vertices[i].Coord.X < minX)
                    minX = _vertices[i].Coord.X;
                else if (_vertices[i].Coord.X > maxX)
                    maxX = _vertices[i].Coord.X;

                if (_vertices[i].Coord.Y < minY)
                    minY = _vertices[i].Coord.Y;
                else if (_vertices[i].Coord.Y > maxY)
                    maxY = _vertices[i].Coord.Y;

                if (_vertices[i].Coord.Z < minZ)
                    minZ = _vertices[i].Coord.Z;
                else if (_vertices[i].Coord.Z > maxZ)
                    maxZ = _vertices[i].Coord.Z;
            }

            // Calculate the center-point from the bounding box edges
            RenderData.Center = new Vector3((minX + maxX) / 2, (minY + maxY) / 2, (minZ + maxZ) / 2);

            // Generate the index array
            RenderData.Indices = new ushort[_numFaces * 3];
            current = 0;
            for (int i = 0; i < _numFaces; i++)
            {
                RenderData.Indices[current++] = (ushort)_faces[i].Indices[0];
                RenderData.Indices[current++] = (ushort)_faces[i].Indices[1];
                RenderData.Indices[current++] = (ushort)_faces[i].Indices[2];
            }

            // Generate the texcoord array
            RenderData.TexCoords = new float[_numVertices * 2];
            current = 0;
            for (int i = 0; i < _numVertices; i++)
            {
                RenderData.TexCoords[current++] = _vertices[i].TexCoord.X;
                RenderData.TexCoords[current++] = _vertices[i].TexCoord.Y;
            }
        }

        public override void LoadLODMesh(int level, string filename)
        {
            LODMesh lod = new LODMesh();
            lod.LoadMesh(filename);
            _lodMeshes[level] = lod;
        }
    }

    public class GLAvatar
    {
        public static Dictionary<string, GLMesh> _meshes = new Dictionary<string, GLMesh>();
        public static bool _wireframe = true;
        public static bool _showSkirt = false;
        public static Dictionary<int, attachment_point> attachment_points = new Dictionary<int, attachment_point>();

        public static void loadlindenmeshes(string LODfilename)
        {
            Bone.loadbones("avatar_skeleton.xml");

            string basedir = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "character" + System.IO.Path.DirectorySeparatorChar;

            // Parse through avatar_lad.xml to find all of the mesh references
            XmlDocument lad = new XmlDocument();
            lad.Load(basedir + LODfilename);

            attachment_points.Clear();

            XmlNodeList attach_points = lad.GetElementsByTagName("attachment_point");
            foreach (XmlNode apoint in attach_points)
            {
                attachment_point point = new attachment_point();
                point.name = apoint.Attributes.GetNamedItem("name").Value;
                point.joint = apoint.Attributes.GetNamedItem("joint").Value;

                string pos = apoint.Attributes.GetNamedItem("position").Value;
                string[] posparts = pos.Split(' ');
                point.position = new Vector3(float.Parse(posparts[0]), float.Parse(posparts[1]), float.Parse(posparts[2]));

                string rot = apoint.Attributes.GetNamedItem("rotation").Value;
                string[] rotparts = rot.Split(' ');
                point.rotation = Quaternion.CreateFromEulers((float)(float.Parse(rotparts[0]) * Math.PI / 180f), (float)(float.Parse(rotparts[1]) * Math.PI / 180f), (float)(float.Parse(rotparts[2]) * Math.PI / 180f));

                point.id = Int32.Parse(apoint.Attributes.GetNamedItem("id").Value);
                point.group = Int32.Parse(apoint.Attributes.GetNamedItem("group").Value);

                attachment_points.Add(point.id, point);

            }

            XmlNodeList bones = lad.GetElementsByTagName("bone");

            XmlNodeList meshes = lad.GetElementsByTagName("mesh");

            foreach (XmlNode meshNode in meshes)
            {
                string type = meshNode.Attributes.GetNamedItem("type").Value;
                int lod = Int32.Parse(meshNode.Attributes.GetNamedItem("lod").Value);
                string fileName = meshNode.Attributes.GetNamedItem("file_name").Value;
                //string minPixelWidth = meshNode.Attributes.GetNamedItem("min_pixel_width").Value;

                GLMesh mesh = (_meshes.ContainsKey(type) ? _meshes[type] : new GLMesh(type));

                switch (mesh.Name)
                {
                    case "lowerBodyMesh":
                        mesh.teFaceID = (int)AvatarTextureIndex.LowerBaked;
                        break;

                    case "upperBodyMesh":
                        mesh.teFaceID = (int)AvatarTextureIndex.UpperBaked;
                        break;

                    case "headMesh":
                        mesh.teFaceID = (int)AvatarTextureIndex.HeadBaked;
                        break;

                    case "hairMesh":
                        mesh.teFaceID = (int)AvatarTextureIndex.HairBaked;
                        break;

                    case "eyelashMesh":
                        mesh.teFaceID = (int)AvatarTextureIndex.HeadBaked;
                        break;

                    case "eyeBallRightMesh":
                        mesh.setMeshPos(Bone.getOffset("mEyeLeft"));
                        //mesh.setMeshRot(Bone.getRotation("mEyeLeft"));
                        mesh.teFaceID = (int)AvatarTextureIndex.EyesBaked;
                        break;

                    case "eyeBallLeftMesh":
                        mesh.setMeshPos(Bone.getOffset("mEyeRight"));
                        //mesh.setMeshRot(Bone.getRotation("mEyeRight"));
                        mesh.teFaceID = (int)AvatarTextureIndex.EyesBaked;
                        break;

                    case "skirtMesh":
                        mesh.teFaceID = (int)AvatarTextureIndex.SkirtBaked;
                        break;

                    default:
                        mesh.teFaceID = 0;
                        break;
                }

                if (lod == 0)
                {
                    mesh.LoadMesh(basedir + fileName);
                }
                else
                {
                    mesh.LoadLODMesh(lod, basedir + fileName);
                }

                _meshes[type] = mesh;

            }
        }

    }

    class RenderAvatar
    {
        public GLAvatar glavatar;
        public Avatar avatar;
        public FaceData[] data = new FaceData[32];

    }

    public class Bone
    {
        public string name;
        public Vector3 pos;
        //public Vector3 rot;
        public Quaternion rot;
        public Vector3 scale;
        public Vector3 piviot;

        public Bone parent;

        public static Dictionary<string, Bone> mBones = new Dictionary<string, Bone>();

        public static void loadbones(string skeletonfilename)
        {
            mBones.Clear();
            string basedir = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "character" + System.IO.Path.DirectorySeparatorChar;
            XmlDocument skeleton = new XmlDocument();
            skeleton.Load(basedir + skeletonfilename);
            XmlNode boneslist = skeleton.GetElementsByTagName("linden_skeleton")[0];
            addbone(boneslist.ChildNodes[0], null);
        }

        public static void addbone(XmlNode bone, Bone parent)
        {
            Bone b = new Bone();
            b.name = bone.Attributes.GetNamedItem("name").Value;

            string pos = bone.Attributes.GetNamedItem("pos").Value;
            string[] posparts = pos.Split(' ');
            b.pos = new Vector3(float.Parse(posparts[0]), float.Parse(posparts[1]), float.Parse(posparts[2]));

            string rot = bone.Attributes.GetNamedItem("rot").Value;
            string[] rotparts = pos.Split(' ');
            b.rot = Quaternion.CreateFromEulers((float)(float.Parse(rotparts[0]) * Math.PI / 180f), (float)(float.Parse(rotparts[1]) * Math.PI / 180f), (float)(float.Parse(rotparts[2]) * Math.PI / 180f));

            string scale = bone.Attributes.GetNamedItem("scale").Value;
            string[] scaleparts = pos.Split(' ');
            b.scale = new Vector3(float.Parse(scaleparts[0]), float.Parse(scaleparts[1]), float.Parse(scaleparts[2]));

            //TODO piviot

            b.parent = parent;

            mBones.Add(b.name, b);

            Logger.Log("Found bone " + b.name, Helpers.LogLevel.Info);

            foreach (XmlNode childbone in bone.ChildNodes)
            {
                addbone(childbone, b);
            }

        }

        //TODO check offset and rot calcuations should each offset be multiplied by its parent rotation in
        // a standard child/parent rot/offset way?
        public static Vector3 getOffset(string bonename)
        {
            Bone b;
            if (mBones.TryGetValue(bonename, out b))
            {
                return (b.getOffset());
            }
            else
            {
                return Vector3.Zero;
            }
        }

        public Vector3 getOffset()
        {
            Vector3 totalpos = pos;

            if (parent != null)
            {
                totalpos = parent.getOffset() + pos;
            }

            return totalpos;
        }

        public static Quaternion getRotation(string bonename)
        {
            Bone b;
            if (mBones.TryGetValue(bonename, out b))
            {
                return (b.getRotation());
            }
            else
            {
                return Quaternion.Identity;
            }
        }

        public Quaternion getRotation()
        {
            Quaternion totalrot = rot;

            if (parent != null)
            {
                totalrot = parent.getRotation() * rot;
            }

            return totalrot;
        }

    }

}
