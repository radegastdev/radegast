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
