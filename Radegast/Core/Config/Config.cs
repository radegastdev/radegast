// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
using System.Collections.Generic;
using System.Text;
using Nini.Config;

namespace Radegast
{
    public class Config
    {
        private int configVersion = 1;

        private int mainWindowState = 0;
        private int interfaceStyle = 1; //0 = System, 1 = Office 2003
        
        private string firstName = string.Empty;
        private string lastName = string.Empty;
        private string passwordMD5 = string.Empty;
        private int loginLocationType = 0;
        private string loginLocation = string.Empty;
        private int loginGrid = 0;
        private string loginUri = string.Empty;

        private bool chatTimestamps = true;
        private bool imTimestamps = true;
        private bool useAlice = false;
        public Config()
        {

        }

        public static Config LoadFrom(string filename)
        {
            Config config = new Config();
            IConfigSource conf = new IniConfigSource(filename);

            config.Version = conf.Configs["General"].GetInt("Version");

            config.MainWindowState = conf.Configs["Interface"].GetInt("MainWindowState");
            config.InterfaceStyle = conf.Configs["Interface"].GetInt("Style");
            
            config.FirstName = conf.Configs["Login"].GetString("FirstName");
            config.LastName = conf.Configs["Login"].GetString("LastName");
            config.PasswordMD5 = conf.Configs["Login"].GetString("Password");
            config.LoginGrid = conf.Configs["Login"].GetInt("Grid");
            config.LoginUri = conf.Configs["Login"].GetString("Uri");
            config.LoginLocationType = conf.Configs["Login"].GetInt("LocationType");
            config.LoginLocation = conf.Configs["Login"].GetString("Location");

            config.ChatTimestamps = conf.Configs["Text"].GetBoolean("ChatTimestamps");
            config.IMTimestamps = conf.Configs["Text"].GetBoolean("IMTimestamps");

            config.useAlice = conf.Configs["Misc"].GetBoolean("UseAlice");
            return config;
        }

        public void Save(string filename)
        {
            IniConfigSource source = new IniConfigSource();

            IConfig config = source.AddConfig("General");
            config.Set("Version", configVersion);

            config = source.AddConfig("Interface");
            config.Set("MainWindowState", mainWindowState);
            config.Set("Style", interfaceStyle);

            config = source.AddConfig("Login");
            config.Set("FirstName", firstName);
            config.Set("LastName", lastName);
            config.Set("Password", passwordMD5);
            config.Set("Grid", loginGrid);
            config.Set("Uri", loginUri);
            config.Set("LocationType", loginLocationType);
            config.Set("Location", loginLocation);

            config = source.AddConfig("Text");
            config.Set("ChatTimestamps", chatTimestamps);
            config.Set("IMTimestamps", imTimestamps);

            config = source.AddConfig("Misc");
            config.Set("UseAlice", useAlice);
            source.Save(filename);
        }

        public bool UseAlice
        {
            get { return useAlice; }
            set { useAlice = value; }
        }

        public int Version
        {
            get { return configVersion; }
            set { configVersion = value; }
        }

        public int MainWindowState
        {
            get { return mainWindowState; }
            set { mainWindowState = value; }
        }

        public int InterfaceStyle
        {
            get { return interfaceStyle; }
            set { interfaceStyle = value; }
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

        public string PasswordMD5
        {
            get { return passwordMD5; }
            set { passwordMD5 = value; }
        }

        public int LoginLocationType
        {
            get { return loginLocationType; }
            set { loginLocationType = value; }
        }

        public string LoginLocation
        {
            get { return loginLocation; }
            set { loginLocation = value; }
        }

        public int LoginGrid
        {
            get { return loginGrid; }
            set { loginGrid = value; }
        }

        public string LoginUri
        {
            get { return loginUri; }
            set { loginUri = value; }
        }

        public bool ChatTimestamps
        {
            get { return chatTimestamps; }
            set { chatTimestamps = value; }
        }

        public bool IMTimestamps
        {
            get { return imTimestamps; }
            set { imTimestamps = value; }
        }
    }
}
