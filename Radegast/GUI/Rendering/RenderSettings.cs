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
using OpenMetaverse;
using OpenMetaverse.Rendering;

namespace Radegast.Rendering
{
    public static class RenderSettings
    {
        #region VBO support
        public static bool UseVBO;
        public static bool CoreVBOPresent;
        public static bool ARBVBOPresent;
        #endregion VBO support

        #region Occlusion queries
        /// <summary>Should we try to optimize by not drawing objects occluded behind other objects</summary>
        public static bool OcclusionCullingEnabled;
        public static bool CoreQuerySupported;
        public static bool ARBQuerySupported;
        #endregion Occlusion queries

        public static bool HasMultiTexturing;
        public static bool UseFBO;
        public static bool HasMipmap;
        public static bool HasShaders;
        public static DetailLevel PrimRenderDetail = DetailLevel.High;
        public static DetailLevel SculptRenderDetail = DetailLevel.High;
        public static DetailLevel MeshRenderDetail = DetailLevel.Highest;
        public static bool AllowQuickAndDirtyMeshing = true;
        public static int MeshesPerFrame = 16;
        public static int TexturesToDownloadPerFrame = 2;
        /// <summary>Should we try to make sure that large prims that are > our draw distance are in view when we are standing on them</summary>
        public static bool HeavierDistanceChecking = true;
        /// <summary>Minimum time between rebuilding terrain mesh and texture</summary>
        public static float MinimumTimeBetweenTerrainUpdated = 15f;
        /// <summary>Are textures that don't have dimensions that are powers of two supported</summary>
        public static bool TextureNonPowerOfTwoSupported;

        /// <summary>
        /// Render avatars
        /// </summary>
        public static bool AvatarRenderingEnabled = true;

        /// <summary>
        /// Render prims
        /// </summary>
        public static bool PrimitiveRenderingEnabled = true;

        /// <summary>
        /// Show avatar skeloton
        /// </summary>
        public static bool RenderAvatarSkeleton = false;

        /// <summary>
        /// Enable shader for shiny
        /// </summary>
        public static bool EnableShiny = false;

        #region Water
        public static bool WaterReflections = false;
        #endregion Water
    }
}
