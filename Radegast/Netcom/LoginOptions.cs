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

namespace Radegast.Netcom
{
    public class LoginOptions
    {
        private string firstName;
        private string lastName;
        private string password;
        private string version = string.Empty;
        private string channel = string.Empty;

        private StartLocationType startLocation = StartLocationType.Home;
        private string startLocationCustom = string.Empty;

        private Grid grid;
        private string gridCustomLoginUri = string.Empty;
        private LastExecStatus lastExecEvent = LastExecStatus.Normal;


        public LoginOptions()
        {

        }

        public static bool IsPasswordMD5(string pass)
        {
            return pass.Length == 35 && pass.StartsWith("$1$");
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string FullName
        {
            get
            {
                if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
                    return string.Empty;
                else
                    return firstName + " " + lastName;
            }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public StartLocationType StartLocation
        {
            get { return startLocation; }
            set { startLocation = value; }
        }

        public string StartLocationCustom
        {
            get { return startLocationCustom; }
            set { startLocationCustom = value; }
        }

        public string Channel
        {
            get { return channel; }
            set { channel = value; }
        }

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public Grid Grid
        {
            get { return grid; }
            set { grid = value; }
        }

        public string GridCustomLoginUri
        {
            get { return gridCustomLoginUri; }
            set { gridCustomLoginUri = value; }
        }

        public LastExecStatus LastExecEvent
        {
            get { return lastExecEvent; }
            set { lastExecEvent = value; }
        }
    }
}
