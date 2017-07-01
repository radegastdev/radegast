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
        /// <summary>Alert</summary>
        public static UUID Alert = new UUID("ed124764-705d-d497-167a-182cd9fa2e6c");

        /// <summary>Click</summary>
        public static UUID Click = new UUID("4c8c3c77-de8d-bde2-b9b8-32635e0fd4a6");

        /// <summary>Error</summary>
        public static UUID Error = new UUID("cb58f920-5b52-8a49-b81c-e532adbbe6f1");

        /// <summary>Health Reduction (Female)</summary>
        public static UUID HealthReductionFemale = new UUID("219c5d93-6c09-31c5-fb3f-c5fe7495c115");

        /// <summary>Health Reduction (Male)</summary>
        public static UUID HealthReductionMale = new UUID("e057c244-5768-1056-c37e-1537454eeb62");

        /// <summary>Sound of incoming IM</summary>
        public static UUID IM = new UUID("67cc2844-00f3-2b3c-b991-6418d01e1bb7");
        
        /// <summary>When user opens new IM window</summary>
        public static UUID IMWindow = new UUID("c825dfbc-9827-7e02-6507-3713d18916c1");

        /// <summary>Invalid Operation</summary>
        public static UUID InvalidOperation = new UUID("4174f859-0d3d-c517-c424-72923dc21f65");

        /// <summary>Invalid Keystrole</summary>
        public static UUID InvalidKeystroke = new UUID("2ca849ba-2885-4bc3-90ef-d4987a5b983a");

        /// <summary>Keyboard Loop</summary>
        public static UUID KeyboardLoop = new UUID("5e191c7b-8996-9ced-a177-b2ac32bfea06");
        
        /// <summary>Pie Appear</summary>
        public static UUID PieAppear = new UUID("8eaed61f-92ff-6485-de83-4dcc938a478e");

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

        /// <summary>Snapshot sound</summary>
        public static UUID Snapshot = new UUID("3d09f582-3851-c0e0-f5ba-277ac5c73fb4");

        /// <summary>Teleport</summary>
        public static UUID Teleport = new UUID("d7a9a565-a013-2a69-797d-5332baa1a947");

        /// <summary></summary>
        public static UUID Thunder = new UUID("e95c96a5-293c-bb7a-57ad-ce2e785ad85f");

        /// <summary>Window Close</summary>
        public static UUID WindowClose = new UUID("2c346eda-b60c-ab33-1119-b8941916a499");

        /// <summary></summary>
        public static UUID WindowOpen = new UUID("c80260ba-41fd-8a46-768a-6bf236360e3a");

        /// <summary>Typing sound</summary>
        public static UUID Typing = new UUID("5e191c7b-8996-9ced-a177-b2ac32bfea06");

        /// <summary>Warning</summary>
        public static UUID Warning = new UUID("449bc80c-91b6-6365-8fd1-95bd91016624");
    }
}
