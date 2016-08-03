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
using OpenTK.Graphics.OpenGL;

namespace Radegast.Rendering
{
    /// <summary>
    /// Compatibility functions for some OpenGL variants
    /// </summary>
    public class Compat
    {
        #region VBO functions
        public static void GenBuffers(out int buffer)
        {
            if (RenderSettings.CoreVBOPresent)
            {
                GL.GenBuffers(1, out buffer);
            }
            else
            {
                GL.Arb.GenBuffers(1, out buffer);
            }
        }

        public static void BindBuffer(BufferTarget target, int id)
        {
            if (RenderSettings.CoreVBOPresent)
            {
                GL.BindBuffer(target, id);
            }
            else
            {
                GL.Arb.BindBuffer((BufferTargetArb)(int)target, id);
            }
        }

        public static void BufferData<T2>(BufferTarget target, IntPtr size, T2[] data, BufferUsageHint usage) where T2 : struct
        {
            if (RenderSettings.CoreVBOPresent)
            {
                GL.BufferData<T2>(target, size, data, usage);
            }
            else
            {
                GL.Arb.BufferData<T2>((BufferTargetArb)(int)target, size, data, (BufferUsageArb)(int)usage);
            }
        }

        public static int BufferSize(BufferTarget target)
        {
            int ret = 0;
            if (RenderSettings.CoreVBOPresent)
            {
                GL.GetBufferParameter(target, BufferParameterName.BufferSize, out ret);
            }
            else
            {
                GL.Arb.GetBufferParameter((BufferTargetArb)(int)target, BufferParameterNameArb.BufferSize, out ret);
            }
            return ret;
        }

        public static void DeleteBuffer(int id)
        {
            if (RenderSettings.CoreVBOPresent)
            {
                GL.DeleteBuffers(1, ref id);
            }
            else
            {
                GL.Arb.DeleteBuffers(1, ref id);
            }
        }

        #endregion VBO functions

        #region Occlusion query functions
        public static void GenQueries(out int id)
        {
            if (RenderSettings.CoreQuerySupported)
            {
                GL.GenQueries(1, out id);
            }
            else
            {
                GL.Arb.GenQueries(1, out id);
            }
        }

        public static void BeginQuery(QueryTarget target, int id)
        {
            if (RenderSettings.CoreQuerySupported)
            {
                GL.BeginQuery(target, id);
            }
            else
            {
                GL.Arb.BeginQuery((ArbOcclusionQuery)(int)target, id);
            }
        }

        public static void EndQuery(QueryTarget target)
        {
            if (RenderSettings.CoreQuerySupported)
            {
                GL.EndQuery(target);
            }
            else
            {
                GL.Arb.EndQuery((ArbOcclusionQuery)(int)target);
            }
        }

        public static void GetQueryObject(int id, GetQueryObjectParam param, out int res)
        {
            if (RenderSettings.CoreQuerySupported)
            {
                GL.GetQueryObject(id, param, out res);
            }
            else
            {
                GL.Arb.GetQueryObject(id, (ArbOcclusionQuery)(int)param, out res);
            }
        }
        #endregion Occlusion query functions
    }
}
