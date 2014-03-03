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

using OpenMetaverse;

namespace Radegast
{
    /// <summary>
    /// Predefined UI sounds
    /// Source: https://wiki.secondlife.com/wiki/Client_sounds
    /// </summary>
    public static class UISounds
    {
        /// <summary>Sound of incoming IM</summary>
        public static UUID IM = new UUID("67cc2844-00f3-2b3c-b991-6418d01e1bb7");
        
        /// <summary>Typing sound</summary>
        public static UUID Typing = new UUID("5e191c7b-8996-9ced-a177-b2ac32bfea06");

        /// <summary>When user opens new IM window</summary>
        public static UUID IMWindow = new UUID("c825dfbc-9827-7e02-6507-3713d18916c1");

        /// <summary>When money balance is increased</summary>
        public static UUID MoneyIn = new UUID("104974e3-dfda-428b-99ee-b0d4e748d3a3");

        /// <summary>When money balance is decreased</summary>
        public static UUID MoneyOut = new UUID("77a018af-098e-c037-51a6-178f05877c6f");

        /// <summary>Object rezzed from inventory</summary>
        public static UUID ObjectRez = new UUID("f4a0660f-5446-dea2-80b7-6482a082803c");

        /// <summary>Object create</summary>
        public static UUID ObjectCreate = new UUID("3c8fc726-1fd6-862d-fa01-16c5b2568db6");

        /// <summary>Object deleted</summary>
        public static UUID ObjectDelete = new UUID("0cb7b00a-4c10-6948-84de-a93c09af2ba9");

    }
}
