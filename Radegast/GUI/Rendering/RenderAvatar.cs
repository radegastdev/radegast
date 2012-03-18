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
// $Id: RenderAvatar.cs 1137 2011-09-05 22:53:46Z latifer $
//

using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Xml;
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.Rendering;

namespace Radegast.Rendering
{
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

        public attachment_point(XmlNode node)
        {
            name = node.Attributes.GetNamedItem("name").Value;
            joint = node.Attributes.GetNamedItem("joint").Value;
            position = VisualParamEx.XmlParseVector(node.Attributes.GetNamedItem("position").Value);
            rotation = VisualParamEx.XmlParseRotation(node.Attributes.GetNamedItem("rotation").Value);
            id = Int32.Parse(node.Attributes.GetNamedItem("id").Value);
            group = Int32.Parse(node.Attributes.GetNamedItem("group").Value);
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
        public Dictionary<int, VisualParamEx> _evp = new Dictionary<int, VisualParamEx>();

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
            public float[] Normals;
            public ushort[] Indices;
            public float[] TexCoords;
            public Vector3 Center;
            public float[] weights; //strictly these are constant and don't need instancing with the GLMesh
            public string[] skinJoints;  //strictly these are constant and don't need instancing with the GLMesh
        }

        public static GLData baseRenderData;
        public GLData RenderData;
        public GLData OrigRenderData;
        public GLData MorphRenderData;

        public GLAvatar av;

        public GLMesh(string name)
            : base(name)
        {
        }

        public GLMesh(GLMesh source, GLAvatar av)
            : base(source.Name)
        {
            this.av = av;
            // Make a new GLMesh copy from the supplied source

            RenderData.Vertices = new float[source.RenderData.Vertices.Length];
            RenderData.Normals = new float[source.RenderData.Normals.Length];
            RenderData.TexCoords = new float[source.RenderData.TexCoords.Length];
            RenderData.Indices = new ushort[source.RenderData.Indices.Length];

            RenderData.weights = new float[source.RenderData.weights.Length];
            RenderData.skinJoints = new string[source.RenderData.skinJoints.Length];

            Array.Copy(source.RenderData.Vertices, RenderData.Vertices, source.RenderData.Vertices.Length);
            Array.Copy(source.RenderData.Normals, RenderData.Normals, source.RenderData.Normals.Length);

            Array.Copy(source.RenderData.TexCoords, RenderData.TexCoords, source.RenderData.TexCoords.Length);
            Array.Copy(source.RenderData.Indices, RenderData.Indices, source.RenderData.Indices.Length);
            Array.Copy(source.RenderData.weights, RenderData.weights, source.RenderData.weights.Length);
            Array.Copy(source.RenderData.skinJoints, RenderData.skinJoints, source.RenderData.skinJoints.Length);

            RenderData.Center = new Vector3(source.RenderData.Center);

            teFaceID = source.teFaceID;

            _rotationAngles = new Vector3(source.RotationAngles);
            _scale = new Vector3(source.Scale);
            _position = new Vector3(source.Position);

            // We should not need to instance these the reference from the top should be constant
            _evp = source._evp;
            _morphs = source._morphs;

            OrigRenderData.Indices = new ushort[source.RenderData.Indices.Length];
            OrigRenderData.TexCoords = new float[source.RenderData.TexCoords.Length];
            OrigRenderData.Vertices = new float[source.RenderData.Vertices.Length];

            MorphRenderData.Vertices = new float[source.RenderData.Vertices.Length];

            Array.Copy(source.RenderData.Vertices, OrigRenderData.Vertices, source.RenderData.Vertices.Length);
            Array.Copy(source.RenderData.Vertices, MorphRenderData.Vertices, source.RenderData.Vertices.Length);

            Array.Copy(source.RenderData.TexCoords, OrigRenderData.TexCoords, source.RenderData.TexCoords.Length);
            Array.Copy(source.RenderData.Indices, OrigRenderData.Indices, source.RenderData.Indices.Length);



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
            RenderData.Normals = new float[_numVertices * 3];

            Quaternion quat = Quaternion.CreateFromEulers(0, 0, (float)(Math.PI / 4.0));

            int current = 0;
            for (int i = 0; i < _numVertices; i++)
            {

                RenderData.Normals[current] = _vertices[i].Normal.X;
                RenderData.Vertices[current++] = _vertices[i].Coord.X;
                RenderData.Normals[current] = _vertices[i].Normal.Y;
                RenderData.Vertices[current++] = _vertices[i].Coord.Y;
                RenderData.Normals[current] = _vertices[i].Normal.Z;
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

            RenderData.weights = new float[_numVertices];
            for (int i = 0; i < _numVertices; i++)
            {
                RenderData.weights[i] = _vertices[i].Weight;
            }

            RenderData.skinJoints = new string[_skinJoints.Length + 3];
            for (int i = 1; i < _skinJoints.Length; i++)
            {
                RenderData.skinJoints[i] = _skinJoints[i];
            }


        }

        public override void LoadLODMesh(int level, string filename)
        {
            LODMesh lod = new LODMesh();
            lod.LoadMesh(filename);
            _lodMeshes[level] = lod;
        }

        public void applyjointweights()
        {

            /*Each weight actually contains two pieces of information. 
             * The number to the left of the decimal point is the index of the joint and also 
             * implicitly indexes to the following joint. The actual weight is to the right of 
             * the decimal point and interpolates between these two joints. The index is into an 
             * "expanded" list of joints, not just a linear array of the joints as defined in the 
             * skeleton file. In particular, any joint that has more than one child will be repeated 
             * in the list for each of its children.
             */

            float weight = -9999;
            int jointindex = 0;
            float factor;

            Bone ba = null;
            Bone bb = null;

            for (int v = 0, x = 0; v < RenderData.Vertices.Length; v = v + 3, x++)
            {
                if (weight != RenderData.weights[x])
                {

                    jointindex = (int)Math.Floor(weight = RenderData.weights[x]);
                    factor = RenderData.weights[x] - jointindex;
                    weight = weight - jointindex;

                    string jointname = "", jointname2 = "";

                    switch (Name)
                    {
                        case "upperBodyMesh":
                            jointname = skeleton.mUpperMeshMapping[jointindex];
                            jointindex++;
                            jointname2 = skeleton.mUpperMeshMapping[jointindex];
                            break;

                        case "lowerBodyMesh":
                            jointname = skeleton.mLowerMeshMapping[jointindex];
                            jointindex++;
                            jointname2 = skeleton.mLowerMeshMapping[jointindex];
                            break;

                        case "headMesh":
                            jointname = skeleton.mHeadMeshMapping[jointindex];
                            jointindex++;
                            jointname2 = skeleton.mHeadMeshMapping[jointindex];
                            break;

                        default:
                            return;

                    }

                 
                    if (jointname == "")
                    {
                        //Don't yet handle this, its a split joint to two children
                        ba = av.skel.mBones[jointname2];
                        bb = null;

                        //continue;
                    }
                    else
                    {
                        ba = av.skel.mBones[jointname];
                    }

                    if (jointname2 == "")
                    {
                        bb = null;
                    }
                    else
                    {
                        bb = av.skel.mBones[jointname2];
                    }
                }

                //Special cases 0 is not used
                // ON upper torso 5 and 10 are not used
                // 4 is neck and 6 and 11 are the left and right collar bones


                //TODO use the SRT matrix to do this!

                Vector3 lerpa;
                Vector3 offseta;
                Quaternion rota;

                Vector3 lerpb;
                Vector3 offsetb;
                Quaternion rotb;

                Vector3 pos;

                Vector3 posa = new Vector3(MorphRenderData.Vertices[v], MorphRenderData.Vertices[v + 1], MorphRenderData.Vertices[v + 2]);


                if (bb != null)
                {
                    lerpa = ba.getDeltaOffset();
                    offseta = ba.getTotalOffset();
                    rota = ba.getTotalRotation();

                    lerpb = bb.getDeltaOffset();
                    offsetb = bb.getTotalOffset();
                    rotb = bb.getTotalRotation();

                    Vector3 posb = new Vector3(MorphRenderData.Vertices[v], MorphRenderData.Vertices[v + 1], MorphRenderData.Vertices[v + 2]);

                    //move back to mesh local coords
                    posa = posa - offseta;
   
                    // apply rotated offset
                    posa = ((posa + lerpa) * ba.scale) * rota;
                    //move back to avatar local coords
                    posa = posa + offseta;

                    //move back to mesh local coords
                    posb = posb - offsetb;
                   
                    // apply rotated offset
                    posb = ((posb + lerpb) * bb.scale) * rotb;
                    //move back to avatar local coords
                    posb = posb + offsetb;

                    //LERP the two points to produce smooth skinning ;-)
                    pos = Vector3.Lerp(posa, posb, weight);

                }
                else
                {
                    lerpa = ba.getDeltaOffset();
                    offseta = ba.getTotalOffset(); 
                    rota = ba.getTotalRotation();
                  
                    //move back to mesh local coords
                    posa = posa - offseta;

                    // apply rotated offset
                    posa = ((posa + lerpa) *ba.scale) * rota;
                    //move back to avatar local coords
                    posa = posa + offseta;

                    //Only one bone contributing so its 100% that one.
                    pos = posa;

                }

                RenderData.Vertices[v] = pos.X;
                RenderData.Vertices[v + 1] = pos.Y;
                RenderData.Vertices[v + 2] = pos.Z;
            }
        }

        public void resetallmorphs()
        {
            for (int i = 0; i < OrigRenderData.Vertices.Length / 3; i++)
            {

                MorphRenderData.Vertices[i * 3] = OrigRenderData.Vertices[i * 3];
                MorphRenderData.Vertices[(i * 3) + 1] = OrigRenderData.Vertices[i * 3 + 1];
                MorphRenderData.Vertices[(i * 3) + 2] = OrigRenderData.Vertices[i * 3 + 2];

                //MorphRenderData.Normals[i * 3] = OrigRenderData.Normals[i * 3];
                //MorphRenderData.Normals[(i * 3) + 1] = OrigRenderData.Normals[i * 3 + 1];
                //MorphRenderData.Normals[(i * 3) + 2] = OrigRenderData.Normals[i * 3 + 2];

                RenderData.TexCoords[i * 2] = OrigRenderData.TexCoords[i * 2];
                RenderData.TexCoords[(i * 2) + 1] = OrigRenderData.TexCoords[i * 2 + 1];

            }

        }

        public void morphmesh(Morph morph, float weight)
        {
            Logger.Log(String.Format("Applying morph {0} weight {1}",morph.Name,weight),Helpers.LogLevel.Debug);

            for (int v = 0; v < morph.NumVertices; v++)
            {
                MorphVertex mvx = morph.Vertices[v];

                uint i = mvx.VertexIndex;

                MorphRenderData.Vertices[i * 3] = MorphRenderData.Vertices[i * 3] + mvx.Coord.X * weight;
                MorphRenderData.Vertices[(i * 3) + 1] = MorphRenderData.Vertices[i * 3 + 1] + mvx.Coord.Y * weight;
                MorphRenderData.Vertices[(i * 3) + 2] = MorphRenderData.Vertices[i * 3 + 2] + mvx.Coord.Z * weight;

                //MorphRenderData.Normals[i * 3] = MorphRenderData.Normals[i * 3] + mvx.Normal.X * weight;
                //MorphRenderData.Normals[(i * 3)+1] = MorphRenderData.Normals[(i * 3)+1] + mvx.Normal.Y * weight;
                //MorphRenderData.Normals[(i * 3)+2] = MorphRenderData.Normals[(i * 3)+2] + mvx.Normal.Z * weight;

                RenderData.TexCoords[i * 2] = OrigRenderData.TexCoords[i * 2] + mvx.TexCoord.X * weight;
                RenderData.TexCoords[(i * 2) + 1] = OrigRenderData.TexCoords[i * 2 + 1] + mvx.TexCoord.Y * weight;

            }
        }
    }

    public class GLAvatar
    {
        private static Dictionary<string, GLMesh> _defaultmeshes = new Dictionary<string, GLMesh>();
        public Dictionary<string, GLMesh> _meshes = new Dictionary<string, GLMesh>();

        public skeleton skel = new skeleton();
        public static Dictionary<int, attachment_point> attachment_points = new Dictionary<int, attachment_point>();

        public bool _wireframe = true;
        public bool _showSkirt = false;

        public VisualParamEx.EparamSex msex;

        public byte[] VisualAppearanceParameters = new byte[1024];
        bool vpsent = false;
        static bool lindenMeshesLoaded = false;

        public GLAvatar()
        {
            foreach (KeyValuePair<string, GLMesh> kvp in _defaultmeshes)
            {
                GLMesh mesh = new GLMesh(kvp.Value, this); // Instance our meshes
                _meshes.Add(kvp.Key, mesh);

            }
        }

        public static void dumptweaks()
        {

            for (int x = 0; x < VisualParamEx.tweakable_params.Count; x++)
            {
                VisualParamEx vpe = (VisualParamEx)VisualParamEx.tweakable_params.GetByIndex(x);
                Console.WriteLine(string.Format("{0} is {1}", x, vpe.Name));
            }


        }

        public static void loadlindenmeshes2(string LODfilename)
        {
            // Already have mashes loaded?
            if (lindenMeshesLoaded) return;

            attachment_points.Clear();


            string basedir = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + "character" + System.IO.Path.DirectorySeparatorChar;

            XmlDocument lad = new XmlDocument();
            lad.Load(basedir + LODfilename);

            //Firstly read the skeleton section this contains attachment point info and the bone deform info for visual params
            // And load the skeleton file in to the bones class

            XmlNodeList skeleton = lad.GetElementsByTagName("skeleton");
            string skeletonfilename = skeleton[0].Attributes.GetNamedItem("file_name").Value;
            Bone.loadbones(skeletonfilename);

            // Next read all the skeleton child nodes, we have attachment points and bone deform params
            // attachment points are an offset and rotation from a bone location
            // the name of the bone they reference is the joint paramater
            // params in the skeleton nodes are bone deforms, eg leg length changes the scale of the leg bones

            foreach (XmlNode skeletonnode in skeleton[0].ChildNodes)
            {
                if (skeletonnode.Name == "attachment_point")
                {
                    attachment_point point = new attachment_point(skeletonnode);
                    attachment_points.Add(point.id, point);
                }

                if (skeletonnode.Name == "param")
                {
                    //Bone deform param
                    VisualParamEx vp = new VisualParamEx(skeletonnode, VisualParamEx.ParamType.TYPE_BONEDEFORM);
                }
            }

            //Now we parse the mesh nodes, mesh nodes reference a particular LLM file with a LOD
            //and also list VisualParams for the various mesh morphs that can be applied

            XmlNodeList meshes = lad.GetElementsByTagName("mesh");
            foreach (XmlNode meshNode in meshes)
            {
                string type = meshNode.Attributes.GetNamedItem("type").Value;
                int lod = Int32.Parse(meshNode.Attributes.GetNamedItem("lod").Value);
                string fileName = meshNode.Attributes.GetNamedItem("file_name").Value;

                GLMesh mesh = (_defaultmeshes.ContainsKey(type) ? _defaultmeshes[type] : new GLMesh(type));

                if (meshNode.HasChildNodes)
                {
                    foreach (XmlNode paramnode in meshNode.ChildNodes)
                    {
                        if (paramnode.Name == "param")
                        {
                            VisualParamEx vp = new VisualParamEx(paramnode, VisualParamEx.ParamType.TYPE_MORPH);

                            mesh._evp.Add(vp.ParamID, vp); //Not sure we really need this may optimise out later
                            vp.morphmesh = mesh.Name;
                        }
                    }
                }

                // Set up the texture elemenets for each mesh
                // And hack the eyeball position
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
                        mesh.setMeshPos(Bone.mBones["mEyeLeft"].getTotalOffset());
                        mesh.teFaceID = (int)AvatarTextureIndex.EyesBaked;
                        break;

                    case "eyeBallLeftMesh":
                        mesh.setMeshPos(Bone.mBones["mEyeRight"].getTotalOffset());
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
                    mesh.LoadMesh(basedir + fileName);
                else
                    mesh.LoadLODMesh(lod, basedir + fileName);

                _defaultmeshes[type] = mesh;

            }

            // Next are the textureing params, skipping for the moment

            XmlNodeList colors = lad.GetElementsByTagName("global_color");
            {
                foreach (XmlNode globalcolornode in colors)
                {
                    foreach (XmlNode node in globalcolornode.ChildNodes)
                    {
                        if (node.Name == "param")
                        {
                            VisualParamEx vp = new VisualParamEx(node, VisualParamEx.ParamType.TYPE_COLOR);
                        }
                    }
                }
            }

            // Get layer paramaters, a bit of a verbose way to do it but we probably want to get access
            // to some of the other data not just the <param> tag

            XmlNodeList layer_sets = lad.GetElementsByTagName("layer_set");
            {
                foreach (XmlNode layer_set in layer_sets)
                {
                    foreach (XmlNode layer in layer_set.ChildNodes)
                    {
                        foreach (XmlNode layernode in layer.ChildNodes)
                        {
                            if (layernode.Name == "param")
                            {
                                VisualParamEx vp = new VisualParamEx(layernode, VisualParamEx.ParamType.TYPE_COLOR);
                            }
                        }
                    }
                }
            }

            // Next are the driver parameters, these are parameters that change multiple real parameters

            XmlNodeList drivers = lad.GetElementsByTagName("driver_parameters");

            foreach (XmlNode node in drivers[0].ChildNodes) //lazy 
            {
                if (node.Name == "param")
                {
                    VisualParamEx vp = new VisualParamEx(node, VisualParamEx.ParamType.TYPE_DRIVER);
                }
            }

            lindenMeshesLoaded = true;
        }

        public void applyMorph(Avatar av, int param, float weight)
        {
            VisualParamEx vpx;
            if (VisualParamEx.allParams.TryGetValue(param, out vpx))
            {

             
                if (weight < 0)
                    weight = 0;

                if (weight > 1.0)
                    weight = 1;

                float value = vpx.MinValue + ((vpx.MaxValue - vpx.MinValue) * weight);

                //Logger.Log(string.Format("Applying visual parameter {0} id {1} value {2} scalar {4} type {3}", vpx.Name, vpx.ParamID, weight, vpx.pType.ToString(),value.ToString()), Helpers.LogLevel.Debug);


                if (vpx.pType == VisualParamEx.ParamType.TYPE_MORPH)
                {
                    // Its a morph
                    GLMesh mesh;
                    if (_meshes.TryGetValue(vpx.morphmesh, out mesh))
                    {
                        foreach (LindenMesh.Morph morph in mesh.Morphs) //optimise me to a dictionary
                        {
                            if (morph.Name == vpx.Name)
                            {
                                if (mesh.Name == "skirtMesh" && _showSkirt == false)
                                    return;

                                mesh.morphmesh(morph, weight);

                                continue;
                            }
                        }
                    }
                    else
                    {
                        // Not a mesh morph 

                        // Its a volume deform, these appear to be related to collision volumes
                        /*
                        if (vpx.VolumeDeforms == null)
                        {
                            Logger.Log(String.Format("paramater {0} has invalid mesh {1}", param, vpx.morphmesh), Helpers.LogLevel.Warning);
                        }
                        else
                        {
                            foreach (KeyValuePair<string, VisualParamEx.VolumeDeform> kvp in vpx.VolumeDeforms)
                            {
                                skel.deformbone(kvp.Key, kvp.Value.pos, kvp.Value.scale);
                            }
                        }
                         * */

                    }

                }
                else
                {
                    // Its not a morph, it might be a driver though
                    if (vpx.pType == VisualParamEx.ParamType.TYPE_DRIVER)
                    {
                        foreach (VisualParamEx.driven child in vpx.childparams)
                        {
                            applyMorph(av, child.id, weight); //TO DO use minmax if they are present
                        }
                        return;
                    }

                    //Is this a bone deform?
                    if (vpx.pType == VisualParamEx.ParamType.TYPE_BONEDEFORM)
                    {
                        //  scale="0 0 .3" />
                        //   value_min="-1"
                        // value_max="1"

                        foreach (KeyValuePair<string, BoneDeform> kvp in vpx.BoneDeforms)
                        {
                            skel.scalebone(kvp.Key, Vector3.One + (kvp.Value.scale *value));
                            skel.offsetbone(kvp.Key, kvp.Value.offset * value);
                        }
                        return;
                    }
                    else
                    {
                        //Logger.Log(String.Format("paramater {0} is not a morph and not a driver", param), Helpers.LogLevel.Warning);
                    }
                }

            }
            else
            {
                Logger.Log("Invalid paramater " + param.ToString(), Helpers.LogLevel.Warning);
            }
        }

        public void morph(Avatar av)
        {

            if (av.VisualParameters == null)
                return;

            ThreadPool.QueueUserWorkItem(sync =>
            {
                int x = 0;

                // We need a lock here as we may get multiple packets thrown at us and we should at least
                // process them in turn not 1/2 process one then start to process the next. 
                // That said av might not be the best lock object, but it will do for the moment
                lock (av)
                {
                    if (av.VisualParameters.Length > 123)
                    {
                        if (av.VisualParameters[31] > 127)
                        {
                            msex = VisualParamEx.EparamSex.SEX_MALE;
                        }
                        else
                        {
                            msex = VisualParamEx.EparamSex.SEX_FEMALE;
                        }
                    }

                    foreach (GLMesh mesh in _meshes.Values)
                    {
                      mesh.resetallmorphs();
                    }

                    foreach (byte vpvalue in av.VisualParameters)
                    {
                        /*
                        if (vpsent == true && VisualAppearanceParameters[x] == vpvalue)
                        {
                     
                           x++;
                           continue;
                        }
                        */

                      

                        if (x >= VisualParamEx.tweakable_params.Count)
                        {
                            //Logger.Log("Two many visual paramaters in Avatar appearance", Helpers.LogLevel.Warning);
                            break;
                        }

                        VisualParamEx vpe = (VisualParamEx)VisualParamEx.tweakable_params.GetByIndex(x);

                        if (vpe.sex != VisualParamEx.EparamSex.SEX_BOTH && vpe.sex != msex)
                        {
                            x++;
                            continue;
                        }

                        
                        //if (VisualAppearanceParameters[x] == vpvalue)
                        //{
                        //  x++;
                        //   continue;
                        //}

                        //Console.WriteLine(String.Format("VP Change detected for byte {0} value {1} name {2}", x, vpvalue, vpe.Name));
                        

                        VisualAppearanceParameters[x] = vpvalue;

                        float value = (vpvalue / 255.0f);
                        this.applyMorph(av, vpe.ParamID, value);

                        x++;
                    }

                    vpsent = true;

                    // Don't update actual meshes here anymore, we do it every frame because of animation anyway
               
                }
            });
        }
    }

    public class skeleton
    {
        public Dictionary<string, Bone> mBones;
        public Dictionary<string, int> mPriority = new Dictionary<string, int>();
        public static Dictionary<int, string> mUpperMeshMapping = new Dictionary<int, string>();
        public static Dictionary<int, string> mLowerMeshMapping = new Dictionary<int, string>();
        public static Dictionary<int, string> mHeadMeshMapping = new Dictionary<int, string>();

        public List<BinBVHAnimationReader> mAnimations = new List<BinBVHAnimationReader>();

        public static Dictionary<UUID, RenderAvatar> mAnimationTransactions = new Dictionary<UUID, RenderAvatar>();

        public static Dictionary<UUID, BinBVHAnimationReader> mAnimationCache = new Dictionary<UUID, BinBVHAnimationReader>();

        public bool mNeedsUpdate = false;
        public bool mNeedsMeshRebuild = false;

        public Bone mLeftEye = null;
        public Bone mRightEye = null;

        public struct binBVHJointState
        {
            public float currenttime_rot;
            public int lastkeyframe_rot;
            public int nextkeyframe_rot;

            public float currenttime_pos;
            public int lastkeyframe_pos;
            public int nextkeyframe_pos;

            public int loopinframe;
            public int loopoutframe;
        }


        public skeleton()
        {



            mBones = new Dictionary<string, Bone>();

            foreach (Bone src in Bone.mBones.Values)
            {
                Bone newbone = new Bone(src);
                mBones.Add(newbone.name, newbone);
            }

            //rebuild the skeleton structure on the new copy
            foreach (Bone src in mBones.Values)
            {
                if (src.mParentBone != null)
                {
                    src.parent = mBones[src.mParentBone];
                    src.parent.children.Add(src);
                }
            }

            //FUDGE
            if (mUpperMeshMapping.Count == 0)
            {
                mUpperMeshMapping.Add(1, "mPelvis");
                mUpperMeshMapping.Add(2, "mTorso");
                mUpperMeshMapping.Add(3, "mChest");
                mUpperMeshMapping.Add(4, "mNeck");
                mUpperMeshMapping.Add(5, "");
                mUpperMeshMapping.Add(6, "mCollarLeft");
                mUpperMeshMapping.Add(7, "mShoulderLeft");
                mUpperMeshMapping.Add(8, "mElbowLeft");
                mUpperMeshMapping.Add(9, "mWristLeft");
                mUpperMeshMapping.Add(10, "");
                mUpperMeshMapping.Add(11, "mCollarRight");
                mUpperMeshMapping.Add(12, "mShoulderRight");
                mUpperMeshMapping.Add(13, "mElbowRight");
                mUpperMeshMapping.Add(14, "mWristRight");
                mUpperMeshMapping.Add(15, "");

                mLowerMeshMapping.Add(1, "mPelvis");
                mLowerMeshMapping.Add(2, "mHipRight");
                mLowerMeshMapping.Add(3, "mKneeRight");
                mLowerMeshMapping.Add(4, "mAnkleRight");
                mLowerMeshMapping.Add(5, "");
                mLowerMeshMapping.Add(6, "mHipLeft");
                mLowerMeshMapping.Add(7, "mKneeLeft");
                mLowerMeshMapping.Add(8, "mAnkleLeft");
                mLowerMeshMapping.Add(9, "");

                mHeadMeshMapping.Add(1, "mNeck");
                mHeadMeshMapping.Add(2, "mHead");
                mHeadMeshMapping.Add(3, "");

            }

            mLeftEye = mBones["mEyeLeft"];
            mRightEye = mBones["mEyeRight"];

        }

        public void deformbone(string name, Vector3 pos, Quaternion rotation)
        {
            Bone bone;
            if (mBones.TryGetValue(name, out bone))
            {
                bone.deformbone(pos, rotation);
            }
        }

        public void scalebone(string name, Vector3 scale)
        {
            Bone bone;
            if (mBones.TryGetValue(name, out bone))
            {
                bone.scalebone(scale);
            }
        }

        public void offsetbone(string name, Vector3 offset)
        {
            Bone bone;
            if (mBones.TryGetValue(name, out bone))
            {
                bone.offsetbone(offset);
            }
        }

        //TODO check offset and rot calcuations should each offset be multiplied by its parent rotation in
        // a standard child/parent rot/offset way?
        public Vector3 getOffset(string bonename)
        {
            Bone b;
            if (mBones.TryGetValue(bonename, out b))
            {
                return (b.getTotalOffset());
            }
            else
            {
                return Vector3.Zero;
            }
        }

        public Quaternion getRotation(string bonename)
        {
            Bone b;
            if (mBones.TryGetValue(bonename, out b))
            {
                return (b.getTotalRotation());
            }
            else
            {
                return Quaternion.Identity;
            }
        }


        public void flushanimations()
        {
            lock (mAnimations)
            {
                mAnimations.Clear();
            }
        }

        // Add animations to the global decoded list
        // TODO garbage collect unused animations somehow
        public static void addanimation(OpenMetaverse.Assets.Asset asset, UUID tid, BinBVHAnimationReader b)
        {
            RenderAvatar av;
            mAnimationTransactions.TryGetValue(tid, out av);
            if (av == null)
                return;

            mAnimationTransactions.Remove(tid);

            if (asset != null)
            {
                b = new BinBVHAnimationReader(asset.AssetData);
                mAnimationCache[asset.AssetID] = b;
                Logger.Log("Adding new decoded animaton known animations " + asset.AssetID.ToString(), Helpers.LogLevel.Info);
            }

            int pos = 0;
            foreach (binBVHJoint joint in b.joints)
            {
                binBVHJointState state;

                state.lastkeyframe_rot = 0;
                state.nextkeyframe_rot = 1;

                state.lastkeyframe_pos = 0;
                state.nextkeyframe_pos = 1;

                state.currenttime_rot = 0;
                state.currenttime_pos = 0;

                state.loopinframe = 0;
                state.loopoutframe = joint.rotationkeys.Length - 1;

                if (b.Loop == true)
                {
                    int frame = 0;
                    foreach (binBVHJointKey key in joint.rotationkeys)
                    {
                        if (key.time == b.InPoint)
                        {
                            state.loopinframe = frame;
                        }

                        if (key.time == b.OutPoint)
                        {
                            state.loopoutframe = frame;
                        }

                        frame++;

                    }

                }

                b.joints[pos].Tag = state;
                pos++;
            }

            lock (av.glavatar.skel.mAnimations)
            {
                av.glavatar.skel.mAnimations.Add(b);
            }
        }

        public void animate(float lastframetime)
        {
            try
            {
                lock (mPriority)
                {
                    mPriority.Clear();
                    lock (mAnimations)
                    {
                        foreach (BinBVHAnimationReader b in mAnimations)
                        {
                            if (b == null)
                                continue;

                            int jpos = 0;
                            foreach (binBVHJoint joint in b.joints)
                            {
                                int prio = 0;

                                //Quick hack to stack animations in the correct order
                                //TODO we need to do this per joint as they all have their own priorities as well ;-(
                                if (mPriority.TryGetValue(joint.Name, out prio))
                                {
                                    if (prio > b.Priority)
                                        continue;
                                }

                                mPriority[joint.Name] = b.Priority;

                                binBVHJointState state = (binBVHJointState)b.joints[jpos].Tag;

                                state.currenttime_rot += lastframetime;
                                state.currenttime_pos += lastframetime;

                                //fudge
                                if (b.joints[jpos].rotationkeys.Length == 1)
                                {
                                    state.nextkeyframe_rot = 0;
                                }

                                Vector3 poslerp = Vector3.Zero;

                                if (b.joints[jpos].positionkeys.Length > 2)
                                {
                                    binBVHJointKey pos2 = b.joints[jpos].positionkeys[state.nextkeyframe_pos];


                                    if (state.currenttime_pos > pos2.time)
                                    {
                                        state.lastkeyframe_pos++;
                                        state.nextkeyframe_pos++;

                                        if (state.nextkeyframe_pos >= b.joints[jpos].positionkeys.Length || (state.nextkeyframe_pos >= state.loopoutframe && b.Loop == true))
                                        {
                                            if (b.Loop == true)
                                            {
                                                state.nextkeyframe_pos = state.loopinframe;
                                                state.currenttime_pos = b.InPoint;

                                                if (state.lastkeyframe_pos >= b.joints[jpos].positionkeys.Length)
                                                {
                                                    state.lastkeyframe_pos = state.loopinframe;
                                                }
                                            }
                                            else
                                            {
                                                state.nextkeyframe_pos = joint.positionkeys.Length - 1;
                                            }
                                        }



                                        if (state.lastkeyframe_pos >= b.joints[jpos].positionkeys.Length)
                                        {
                                            if (b.Loop == true)
                                            {
                                                state.lastkeyframe_pos = 0;
                                                state.currenttime_pos = 0;

                                            }
                                            else
                                            {
                                                state.lastkeyframe_pos = joint.positionkeys.Length - 1;
                                                if (state.lastkeyframe_pos < 0)//eeww
                                                    state.lastkeyframe_pos = 0;
                                            }
                                        }
                                    }

                                    binBVHJointKey pos = b.joints[jpos].positionkeys[state.lastkeyframe_pos];


                                    if (state.currenttime_pos != ((pos.time - b.joints[jpos].positionkeys[0].time)))
                                    {

                                        float delta = (pos2.time - pos.time) / ((state.currenttime_pos) - (pos.time - b.joints[jpos].positionkeys[0].time));

                                        if (delta < 0)
                                            delta = 0;

                                        if (delta > 1)
                                            delta = 1;

                                        poslerp = Vector3.Lerp(pos.key_element, pos2.key_element, delta);
                                    }

                                }


                                Vector3 rotlerp = Vector3.Zero;
                                if (b.joints[jpos].rotationkeys.Length > 0)
                                {
                                    binBVHJointKey rot2 = b.joints[jpos].rotationkeys[state.nextkeyframe_rot];

                                    if (state.currenttime_rot > rot2.time)
                                    {
                                        state.lastkeyframe_rot++;
                                        state.nextkeyframe_rot++;

                                        if (state.nextkeyframe_rot >= b.joints[jpos].rotationkeys.Length || (state.nextkeyframe_rot >= state.loopoutframe && b.Loop == true))
                                        {
                                            if (b.Loop == true)
                                            {
                                                state.nextkeyframe_rot = state.loopinframe;
                                                state.currenttime_rot = b.InPoint;

                                                if (state.lastkeyframe_rot >= b.joints[jpos].rotationkeys.Length)
                                                {
                                                    state.lastkeyframe_rot = state.loopinframe;


                                                }

                                            }
                                            else
                                            {
                                                state.nextkeyframe_rot = joint.rotationkeys.Length - 1;
                                            }
                                        }

                                        if (state.lastkeyframe_rot >= b.joints[jpos].rotationkeys.Length)
                                        {
                                            if (b.Loop == true)
                                            {
                                                state.lastkeyframe_rot = 0;
                                                state.currenttime_rot = 0;

                                            }
                                            else
                                            {
                                                state.lastkeyframe_rot = joint.rotationkeys.Length - 1;
                                            }
                                        }
                                    }

                                    binBVHJointKey rot = b.joints[jpos].rotationkeys[state.lastkeyframe_rot];
                                    rot2 = b.joints[jpos].rotationkeys[state.nextkeyframe_rot];

                                    float deltarot = 0;
                                    if (state.currenttime_rot != (rot.time - b.joints[jpos].rotationkeys[0].time))
                                    {
                                        deltarot = (rot2.time - rot.time) / ((state.currenttime_rot) - (rot.time - b.joints[jpos].rotationkeys[0].time));
                                    }

                                    if (deltarot < 0)
                                        deltarot = 0;

                                    if (deltarot > 1)
                                        deltarot = 1;

                                    rotlerp = Vector3.Lerp(rot.key_element, rot2.key_element, deltarot);

                                }

                                b.joints[jpos].Tag = (object)state;

                                deformbone(joint.Name, poslerp, new Quaternion(rotlerp.X, rotlerp.Y, rotlerp.Z));

                                jpos++;
                            }

                            mNeedsMeshRebuild = true;
                        }

                        mNeedsUpdate = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log("Exception in animate()", Helpers.LogLevel.Warning, ex);
            }
        }
    }

    public class Bone
    {
        public string name;
        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;
        public Vector3 piviot;

        public Vector3 offset_pos;

        public Vector3 orig_pos;
        public Quaternion orig_rot;
        public Vector3 orig_scale;
        public Vector3 orig_piviot;

        Matrix4 mDeformMatrix = Matrix4.Identity;

        public Bone parent;

        public List<Bone> children = new List<Bone>();

        public static Dictionary<string, Bone> mBones = new Dictionary<string, Bone>();
        public static Dictionary<int, Bone> mIndexedBones = new Dictionary<int, Bone>();
        static int boneaddindex = 0;

        private bool rotdirty = true;
        private bool posdirty = true;

        //Inverse bind matrix with bone scale
        private Vector3 mTotalPos;

        private Quaternion mTotalRot;

        private Vector3 mDeltaPos;
        private Quaternion mDeltaRot;

        public string mParentBone = null;

        public Bone()
        {
        }

        public Bone(Bone source)
        {
            name = String.Copy(source.name);
            pos = new Vector3(source.pos);
            rot = new Quaternion(source.rot);
            scale = new Vector3(source.scale);
            piviot = new Vector3(source.piviot);
            offset_pos = new Vector3(source.offset_pos);

            orig_piviot = source.orig_piviot;
            orig_pos = source.orig_pos;
            orig_rot = source.orig_rot;
            orig_scale = source.orig_scale;

            mParentBone = source.mParentBone;

            mDeformMatrix = new Matrix4(source.mDeformMatrix);
        }

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

            if (bone.Name != "bone")
                return;

            Bone b = new Bone();
            b.name = bone.Attributes.GetNamedItem("name").Value;

            string pos = bone.Attributes.GetNamedItem("pos").Value;
            string[] posparts = pos.Split(' ');
            b.pos = new Vector3(float.Parse(posparts[0], Utils.EnUsCulture), float.Parse(posparts[1], Utils.EnUsCulture), float.Parse(posparts[2], Utils.EnUsCulture));
            b.orig_pos = new Vector3(b.pos);
            b.offset_pos = new Vector3(b.pos);

            string rot = bone.Attributes.GetNamedItem("rot").Value;
            string[] rotparts = rot.Split(' ');
            b.rot = Quaternion.CreateFromEulers((float)(float.Parse(rotparts[0], Utils.EnUsCulture) * Math.PI / 180f), (float)(float.Parse(rotparts[1], Utils.EnUsCulture) * Math.PI / 180f), (float)(float.Parse(rotparts[2], Utils.EnUsCulture) * Math.PI / 180f));
            b.orig_rot = new Quaternion(b.rot);

            string scale = bone.Attributes.GetNamedItem("scale").Value;
            string[] scaleparts = scale.Split(' ');
            b.scale = new Vector3(float.Parse(scaleparts[0], Utils.EnUsCulture), float.Parse(scaleparts[1], Utils.EnUsCulture), float.Parse(scaleparts[2], Utils.EnUsCulture));
            b.orig_scale = new Vector3(b.scale);
         

            float[] deform = Math3D.CreateSRTMatrix(new Vector3(1, 1, 1), b.rot, b.orig_pos);
            b.mDeformMatrix = new Matrix4(deform[0], deform[1], deform[2], deform[3], deform[4], deform[5], deform[6], deform[7], deform[8], deform[9], deform[10], deform[11], deform[12], deform[13], deform[14], deform[15]);

            //TODO piviot

            b.parent = parent;
            if (parent != null)
            {
                b.mParentBone = parent.name;
                parent.children.Add(b);
            }

            mBones.Add(b.name, b);
            mIndexedBones.Add(boneaddindex++, b);

            Logger.Log("Found bone " + b.name, Helpers.LogLevel.Info);

            foreach (XmlNode childbone in bone.ChildNodes)
            {
                addbone(childbone, b);
            }

        }

        public void deformbone(Vector3 pos, Quaternion rot)
        {
            //float[] deform = Math3D.CreateSRTMatrix(scale, rot, this.orig_pos);
            //mDeformMatrix = new Matrix4(deform[0], deform[1], deform[2], deform[3], deform[4], deform[5], deform[6], deform[7], deform[8], deform[9], deform[10], deform[11], deform[12], deform[13], deform[14], deform[15]);
            
            this.pos = Bone.mBones[name].offset_pos + pos;
            this.rot = Bone.mBones[name].orig_rot * rot;

            markdirty();
        }

        public void scalebone(Vector3 scale)
        {
            //this.scale = Bone.mBones[name].orig_scale + scale;
            this.scale = scale;
            markdirty();
        }

        public void offsetbone(Vector3 offset)
        {
            this.offset_pos = offset;
            markdirty();
        }

        // If we deform a bone mark this bone and all its children as dirty.  
        public void markdirty()
        {
            rotdirty = true;
            posdirty = true;
            foreach (Bone childbone in children)
            {
                childbone.markdirty();
            }
        }

        public Matrix4 getdeform()
        {
            if (this.parent != null)
            {
                return mDeformMatrix * parent.getdeform();
            }
            else
            {
                return mDeformMatrix;
            }
        }

        private Vector3 getOffset()
        {
            if (parent != null)
            {
                Quaternion totalrot = getParentRot(); // we don't want this joints rotation included
                Vector3 parento = parent.getOffset();
                mTotalPos = parento + pos * scale * totalrot;
                Vector3 orig = getOrigOffset();
                mDeltaPos = mTotalPos - orig;

                posdirty = false;

                return mTotalPos;
            }
            else
            {
                Vector3 orig = getOrigOffset();
                mTotalPos = (pos * scale)+offset_pos;
                mDeltaPos = mTotalPos - orig;
                posdirty = false;
                return mTotalPos;

            }
        }

        public Vector3 getMyOffset()
        {
            return pos * scale;
        }

        // Try to save some cycles by not recalculating positions and rotations every time
        public Vector3 getTotalOffset()
        {
            if (posdirty == false)
            {
                return mTotalPos;
            }
            else
            {
                return getOffset();
            }
        }

        public Vector3 getDeltaOffset()
        {
            if (posdirty == false)
            {
                return mDeltaPos;
            }
            else
            {
                getOffset();
                return mDeltaPos;
            }
        }

        private Vector3 getOrigOffset()
        {
            if (parent != null)
            {
                return (parent.getOrigOffset() + orig_pos);
            }
            else
            {
                return orig_pos;
            }
        }

        private static Quaternion getRotation(string bonename)
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


        private Quaternion getParentRot()
        {
            Quaternion totalrot = Quaternion.Identity;

            if (parent != null)
            {
                totalrot = parent.getRotation();
            }

            return totalrot;

        }

        private Quaternion getRotation()
        {
            Quaternion totalrot = rot;

            if (parent != null)
            {
                totalrot = parent.getRotation() * rot;
            }

            mTotalRot = totalrot;
            rotdirty = false;

            return totalrot;
        }

        public Quaternion getTotalRotation()
        {
            if (rotdirty == false)
            {
                return mTotalRot;
            }
            else
            {
                return getRotation();
            }
        }
    }

    public class BoneDeform
    {
        public BoneDeform(Vector3 scale, Vector3 offset)
        {
            this.scale = scale;
            this.offset = offset;
        }

        public Vector3 scale;
        public Vector3 offset;
    }

    public class VisualParamEx
    {

        static public Dictionary<int, VisualParamEx> allParams = new Dictionary<int, VisualParamEx>();
        static public Dictionary<int, VisualParamEx> deformParams = new Dictionary<int, VisualParamEx>();
        static public Dictionary<int, VisualParamEx> morphParams = new Dictionary<int, VisualParamEx>();
        static public Dictionary<int, VisualParamEx> drivenParams = new Dictionary<int, VisualParamEx>();
        static public SortedList tweakable_params = new SortedList();

        public Dictionary<string, BoneDeform> BoneDeforms = null;
        public Dictionary<string, VolumeDeform> VolumeDeforms = null;
        public List<driven> childparams = null;

        public string morphmesh = null;

        enum GroupType
        {
            VISUAL_PARAM_GROUP_TWEAKABLE = 0,
            VISUAL_PARAM_GROUP_ANIMATABLE,
            VISUAL_PARAM_GROUP_TWEAKABLE_NO_TRANSMIT,
        }

        public struct VolumeDeform
        {
            public string name;
            public Vector3 scale;
            public Vector3 pos;
        }

        public enum EparamSex
        {
            SEX_BOTH = 0,
            SEX_FEMALE = 1,
            SEX_MALE = 2
        }

        public enum ParamType
        {
            TYPE_BONEDEFORM,
            TYPE_MORPH,
            TYPE_DRIVER,
            TYPE_COLOR,
            TYPE_LAYER
        }

        public struct driven
        {
            public int id;
            public float max1;
            public float max2;
            public float min1;
            public float min2;
            public bool hasMinMax;
        }

        public string meshname;

        /// <summary>Index of this visual param</summary>
        public int ParamID;
        /// <summary>Internal name</summary>
        public string Name;
        /// <summary>Group ID this parameter belongs to</summary>
        public int Group;
        /// <summary>Name of the wearable this parameter belongs to</summary>
        public string Wearable;
        /// <summary>Displayable label of this characteristic</summary>
        public string Label;
        /// <summary>Displayable label for the minimum value of this characteristic</summary>
        public string LabelMin;
        /// <summary>Displayable label for the maximum value of this characteristic</summary>
        public string LabelMax;
        /// <summary>Default value</summary>
        public float DefaultValue;
        /// <summary>Minimum value</summary>
        public float MinValue;
        /// <summary>Maximum value</summary>
        public float MaxValue;
        /// <summary>Is this param used for creation of bump layer?</summary>
        public bool IsBumpAttribute;
        /// <summary>Alpha blending/bump info</summary>
        public VisualAlphaParam? AlphaParams;
        /// <summary>Color information</summary>
        public VisualColorParam? ColorParams;
        /// <summary>Array of param IDs that are drivers for this parameter</summary>
        public int[] Drivers;
        /// <summary>The Avatar Sex that this parameter applies to</summary>
        public EparamSex sex;

        public ParamType pType;

        public static int count = 0;

        /// <summary>
        /// Set all the values through the constructor
        /// </summary>
        /// <param name="paramID">Index of this visual param</param>
        /// <param name="name">Internal name</param>
        /// <param name="group"></param>
        /// <param name="wearable"></param>
        /// <param name="label">Displayable label of this characteristic</param>
        /// <param name="labelMin">Displayable label for the minimum value of this characteristic</param>
        /// <param name="labelMax">Displayable label for the maximum value of this characteristic</param>
        /// <param name="def">Default value</param>
        /// <param name="min">Minimum value</param>
        /// <param name="max">Maximum value</param>
        /// <param name="isBumpAttribute">Is this param used for creation of bump layer?</param>
        /// <param name="drivers">Array of param IDs that are drivers for this parameter</param>
        /// <param name="alpha">Alpha blending/bump info</param>
        /// <param name="colorParams">Color information</param>
        public VisualParamEx(int paramID, string name, int group, string wearable, string label, string labelMin, string labelMax, float def, float min, float max, bool isBumpAttribute, int[] drivers, VisualAlphaParam? alpha, VisualColorParam? colorParams)
        {
            ParamID = paramID;
            Name = name;
            Group = group;
            Wearable = wearable;
            Label = label;
            LabelMin = labelMin;
            LabelMax = labelMax;
            DefaultValue = def;
            MaxValue = max;
            MinValue = min;
            IsBumpAttribute = isBumpAttribute;
            Drivers = drivers;
            AlphaParams = alpha;
            ColorParams = colorParams;
            sex = EparamSex.SEX_BOTH;
        }

        public VisualParamEx(XmlNode node, ParamType pt)
        {
            pType = pt;

            ParamID = Int32.Parse(node.Attributes.GetNamedItem("id").Value);
            Name = node.Attributes.GetNamedItem("name").Value;
            Group = Int32.Parse(node.Attributes.GetNamedItem("group").Value);

            //These dont exist for facal expresion morphs
            if (node.Attributes.GetNamedItem("wearable") != null)
                Wearable = node.Attributes.GetNamedItem("wearable").Value;

            MinValue = float.Parse(node.Attributes.GetNamedItem("value_min").Value, Utils.EnUsCulture);
            MaxValue = float.Parse(node.Attributes.GetNamedItem("value_max").Value, Utils.EnUsCulture);

            // These do not exists for driven parameters
            if (node.Attributes.GetNamedItem("label_min") != null)
            {
                LabelMin = node.Attributes.GetNamedItem("label_min").Value;
            }

            if (node.Attributes.GetNamedItem("label_max") != null)
            {
                LabelMax = node.Attributes.GetNamedItem("label_max").Value;
            }

            XmlNode sexnode = node.Attributes.GetNamedItem("sex");

            if (sexnode != null)
            {
                if (sexnode.Value == "male")
                {
                    sex = EparamSex.SEX_MALE;
                }
                else
                {
                    sex = EparamSex.SEX_FEMALE;
                }

            }

            Group = int.Parse(node.Attributes.GetNamedItem("group").Value);

            if (Group == (int)GroupType.VISUAL_PARAM_GROUP_TWEAKABLE)
            {
                if (!tweakable_params.ContainsKey(ParamID)) //stupid duplicate shared params
                {
                    tweakable_params.Add(this.ParamID, this);
                }
                //Logger.Log(String.Format("Adding tweakable paramater ID {0} {1}", count, this.Name), Helpers.LogLevel.Info);
                count++;
            }

            //TODO other paramaters but these arew concerned with editing the GUI display so not too fussed at the moment

            try
            {
                allParams.Add(ParamID, this);
            }
            catch
            {
                Logger.Log("Duplicate VisualParam in allParams id " + ParamID.ToString(), Helpers.LogLevel.Info);
            }

            if (pt == ParamType.TYPE_BONEDEFORM)
            {
                // If we are in the skeleton section then we also have bone deforms to parse
                BoneDeforms = new Dictionary<string, BoneDeform>();
                if (node.HasChildNodes && node.ChildNodes[0].HasChildNodes)
                {
                    ParseBoneDeforms(node.ChildNodes[0].ChildNodes);
                }
                deformParams.Add(ParamID, this);
            }

            if (pt == ParamType.TYPE_MORPH)
            {
                VolumeDeforms = new Dictionary<string, VolumeDeform>();
                if (node.HasChildNodes && node.ChildNodes[0].HasChildNodes)
                {
                    ParseVolumeDeforms(node.ChildNodes[0].ChildNodes);
                }

                try
                {
                    morphParams.Add(ParamID, this);
                }
                catch
                {
                    Logger.Log("Duplicate VisualParam in morphParams id " + ParamID.ToString(), Helpers.LogLevel.Info);
                }

            }

            if (pt == ParamType.TYPE_DRIVER)
            {
                childparams = new List<driven>();
                if (node.HasChildNodes && node.ChildNodes[0].HasChildNodes) //LAZY
                {
                    ParseDrivers(node.ChildNodes[0].ChildNodes);
                }

                drivenParams.Add(ParamID, this);

            }

            if (pt == ParamType.TYPE_COLOR)
            {
                if (node.HasChildNodes)
                {
                    foreach (XmlNode colorchild in node.ChildNodes)
                    {
                        if (colorchild.Name == "param_color")
                        {
                            //TODO extract <value color="50, 25, 5, 255" />
                        }
                    }

                }
            }

        }

        void ParseBoneDeforms(XmlNodeList deforms)
        {
            foreach (XmlNode node in deforms)
            {
                if (node.Name == "bone")
                {
                    string name = node.Attributes.GetNamedItem("name").Value;
                    Vector3 scale = Vector3.One;
                    Vector3 offset = Vector3.One;

                    if(node.Attributes.GetNamedItem("scale")!=null)
                        scale = XmlParseVector(node.Attributes.GetNamedItem("scale").Value);

                    if (node.Attributes.GetNamedItem("offset") != null)
                        offset = XmlParseVector(node.Attributes.GetNamedItem("offset").Value);

                    BoneDeform bd = new BoneDeform(scale, offset);

                    BoneDeforms.Add(name, bd);
                }
            }
        }

        void ParseVolumeDeforms(XmlNodeList deforms)
        {
            foreach (XmlNode node in deforms)
            {
                if (node.Name == "volume_morph")
                {
                    VolumeDeform vd = new VolumeDeform();
                    vd.name = node.Attributes.GetNamedItem("name").Value;
                    vd.name = vd.name.ToLower();

                    if (node.Attributes.GetNamedItem("scale") != null)
                    {
                        vd.scale = XmlParseVector(node.Attributes.GetNamedItem("scale").Value);
                    }
                    else
                    {
                        vd.scale = new Vector3(0, 0, 0);
                    }

                    if (node.Attributes.GetNamedItem("pos") != null)
                    {
                        vd.pos = XmlParseVector(node.Attributes.GetNamedItem("pos").Value);
                    }
                    else
                    {
                        vd.pos = new Vector3(0f, 0f, 0f);
                    }

                    VolumeDeforms.Add(vd.name, vd);
                }
            }
        }

        void ParseDrivers(XmlNodeList drivennodes)
        {
            foreach (XmlNode node in drivennodes)
            {
                if (node.Name == "driven")
                {
                    driven d = new driven();

                    d.id = Int32.Parse(node.Attributes.GetNamedItem("id").Value);
                    XmlNode param = node.Attributes.GetNamedItem("max1");
                    if (param != null)
                    {
                        d.max1 = float.Parse(param.Value, Utils.EnUsCulture);
                        d.max2 = float.Parse(node.Attributes.GetNamedItem("max2").Value, Utils.EnUsCulture);
                        d.min1 = float.Parse(node.Attributes.GetNamedItem("min1").Value, Utils.EnUsCulture);
                        d.max2 = float.Parse(node.Attributes.GetNamedItem("min2").Value, Utils.EnUsCulture);
                        d.hasMinMax = true;
                    }
                    else
                    {
                        d.hasMinMax = false;
                    }

                    childparams.Add(d);

                }
            }
        }

        public static Vector3 XmlParseVector(string data)
        {
            string[] posparts = data.Split(' ');
            return new Vector3(float.Parse(posparts[0], Utils.EnUsCulture), float.Parse(posparts[1], Utils.EnUsCulture), float.Parse(posparts[2], Utils.EnUsCulture));
        }

        public static Quaternion XmlParseRotation(string data)
        {
            string[] rotparts = data.Split(' ');
            return Quaternion.CreateFromEulers((float)(float.Parse(rotparts[0]) * Math.PI / 180f), (float)(float.Parse(rotparts[1]) * Math.PI / 180f), (float)(float.Parse(rotparts[2]) * Math.PI / 180f));
        }
    }

    public class RenderAvatar : SceneObject
    {
        public static readonly BoundingVolume AvatarBoundingVolume;

        // Static constructor
        static RenderAvatar()
        {
            AvatarBoundingVolume = new BoundingVolume();
            AvatarBoundingVolume.FromScale(Vector3.One);
        }

        // Default constructor
        public RenderAvatar()
        {
            BoundingVolume = AvatarBoundingVolume;
            Type = SceneObjectType.Avatar;
        }

        public override Primitive BasePrim
        {
            get { return avatar; }
            set
            {
                if (value is Avatar)
                {
                    avatar = (Avatar)value;
                    AvatarBoundingVolume.CalcScaled(avatar.Scale);
                }
            }
        }

        public override void Step(float time)
        {
            glavatar.skel.animate(time);
            base.Step(time);
        }

        public GLAvatar glavatar = new GLAvatar();
        public Avatar avatar;
        public FaceData[] data = new FaceData[32];
        public Dictionary<UUID, Animation> animlist = new Dictionary<UUID, Animation>();
        public Dictionary<WearableType, AppearanceManager.WearableData> Wearables = new Dictionary<WearableType, AppearanceManager.WearableData>();

    }

}
