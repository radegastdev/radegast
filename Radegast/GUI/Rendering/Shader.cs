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
using System.Linq;
using System.Text;
using System.IO;
using OpenMetaverse;
using OpenTK.Graphics.OpenGL;

namespace Radegast.Rendering
{
    public class Shader : IDisposable
    {
        public int ID = -1;

        public bool Load(string fileName)
        {
            if (!RenderSettings.HasShaders) return false;

            try
            {
                ShaderType type;

                string code = File.ReadAllText(fileName);
                if (fileName.EndsWith(".vert"))
                {
                    type = ShaderType.VertexShader;
                }
                else if (fileName.EndsWith(".frag"))
                {
                    type = ShaderType.FragmentShader;
                }
                else
                    return false;

                ID = GL.CreateShader(type);
                GL.ShaderSource(ID, code);
                GL.CompileShader(ID);
                string info = GL.GetShaderInfoLog(ID);
                int res;
                GL.GetShader(ID, ShaderParameter.CompileStatus, out res);
                if (res != 1)
                {
                    Logger.DebugLog("Compilation failed: " + info);
                    ID = -1;
                    return false;
                }
                Logger.DebugLog(string.Format("{0} {1} compiled successfully", type.ToString(), fileName));
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            if (!RenderSettings.HasShaders) return;

            if (ID == -1) return;
            GL.DeleteShader(ID);
        }
    }

    public class ShaderProgram : IDisposable
    {
        public static int CurrentProgram = 0;
        public int ID = -1;
        Shader[] shaders;

        public bool Load(params string[] shaderNames)
        {
            if (!RenderSettings.HasShaders) return false;

            shaders = new Shader[shaderNames.Length];
            for (int i = 0; i < shaderNames.Length; i++)
            {
                Shader s = new Shader();
                if (!s.Load(Path.Combine("shader_data", shaderNames[i])))
                    return false;
                shaders[i] = s;
            }

            ID = GL.CreateProgram();
            for (int i = 0; i < shaders.Length; i++)
            {
                GL.AttachShader(ID, shaders[i].ID);
            }
            GL.LinkProgram(ID);
            int res;
            GL.GetProgram(ID, GetProgramParameterName.LinkStatus, out res);
            if (res != 1)
            {
                Logger.DebugLog("Linking shader program failed!");
                return false;
            }

            Logger.DebugLog(string.Format("Linking shader program consitsting of {0} shaders successful", shaders.Length));
            return true;
        }

        public void Start()
        {
            if (!RenderSettings.HasShaders) return;

            if (ID != -1)
            {
                GL.UseProgram(ID);
                CurrentProgram = ID;
            }
        }

        public static void Stop()
        {
            if (!RenderSettings.HasShaders) return;
            
            if (CurrentProgram != 0)
            {
                GL.UseProgram(0);
            }
        }

        public int Uni(string var)
        {
            if (!RenderSettings.HasShaders) return -1;

            return GL.GetUniformLocation(ID, var);
        }

        public void SetUniform1(string var, int value)
        {
            if (!RenderSettings.HasShaders || ID < 1) return;
            int varID = Uni(var);
            if (varID == -1)
            {
                return;
            }
            GL.Uniform1(varID, value);
        }

        public void SetUniform1(string var, float value)
        {
            if (!RenderSettings.HasShaders || ID < 1) return;
            int varID = Uni(var);
            if (varID == -1)
            {
                return;
            }
            GL.Uniform1(varID, value);
        }

        public void Dispose()
        {
            if (!RenderSettings.HasShaders) return;

            if (ID != -1)
            {
                if (CurrentProgram == ID)
                {
                    GL.UseProgram(0);
                }
                GL.DeleteProgram(ID);
                ID = -1;
            }

            if (shaders != null)
            {
                for (int i = 0; i < shaders.Length; i++)
                {
                    shaders[i].Dispose();
                }
            }
            shaders = null;
        }
    }
}
