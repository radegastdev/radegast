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
    public struct Camera
    {
        Vector3 mPosition;
        Vector3 mFocalPoint;
        bool mModified;
        
        /// <summary>Camera position</summary>
        public Vector3 Position { get { return mPosition; } set { mPosition = value; mModified = true; } }
        /// <summary>Camera target</summary>
        public Vector3 FocalPoint { get { return mFocalPoint; } set { mFocalPoint = value; mModified = true; } }
        /// <summary>Zoom level</summary>
        public float Zoom;
        /// <summary>Draw distance</summary>
        public float Far;
        /// <summary>Has camera been modified</summary>
        public bool Modified { get { return mModified; } set { mModified = value; } }
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
        public Vector3 rotation;
        public int id;
        public int group;

        public GLMesh jointmesh;
        public int jointmeshindex;

        public Vector3 getposition()
        {
            Vector3 pos;
            pos = jointmesh.Position + position;
            return pos;
        }

        public Quaternion getrotation()
        {
            Vector3 rotvec = jointmesh.RotationAngles + rotation;
            rotvec.Normalize();
            Quaternion rot = new Quaternion(rotvec.X, rotvec.Y, rotvec.Z);
            return rot;
        }
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
                point.rotation = new Vector3(float.Parse(rotparts[0]), float.Parse(rotparts[1]), float.Parse(rotparts[2]));

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
                    case "eyeBallLeftMesh":
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

                if (lod == 0)
                {
                    //Associate each attachment point with the mesh that has its bones/joints
                    foreach (attachment_point apoint in attachment_points.Values)
                    {
                        int index = 0;

                        foreach (string jointname in mesh.SkinJoints)
                        {
                            if (jointname == apoint.joint)
                            {
                                apoint.jointmesh = mesh;
                                apoint.jointmeshindex = index;
                                Logger.Log("Adding " + apoint.name +"with joint "+ jointname + "to mesh " + mesh.Name,Helpers.LogLevel.Info);
                            }
                            index++;
                        }

                    }
                }

            }
        }

    }

    class RenderAvatar
    {
        public GLAvatar glavatar;
        public Avatar avatar;
        public FaceData[] data = new FaceData[32];

    }

}
