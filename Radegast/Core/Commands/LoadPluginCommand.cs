using System;
using System.Reflection;
using OpenMetaverse;

namespace Radegast.Commands
{
    public class LoadPluginCommand : IRadegastCommand
    {
        private RadegastInstance instance;
        public string Name
        {
            get { return "loadplugin"; }
        }

        public string Description
        {
            get { return "Loads plugins from a path"; }
        }

        public string Usage
        {
            get { return "loadplugin c:\\myplugindir\\plugin.dll"; }
        }

        public void StartCommand(RadegastInstance inst)
        {
            instance = inst;
        }

        public void StopCommand(RadegastInstance inst)
        {
            instance = null;
        }

        public void Execute(string n, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            string loadfilename = String.Join(" ", cmdArgs);
            try
            {
                Assembly assembly = Assembly.LoadFile(loadfilename);
                instance.LoadAssembly(loadfilename, assembly);
            }
            catch (Exception ex)
            {
                WriteLine("ERROR in Radegast Plugin: {0} because {1} {2}", loadfilename, ex.Message, ex.StackTrace);
            }
        }
    }
}