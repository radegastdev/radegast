using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IdealistViewer;
using log4net;
using log4net.Config;
using Nini;
using Nini.Config;
using ArgvConfigSource = Nini.Config.ArgvConfigSource;
using Radegast;

namespace IdealistRadegastPlugin
{
    public class IdealistPlugin : IRadegastPlugin
    {
        private RadegastInstance Instance;
        private RadegastViewer IV;
        private IdealistUserControl idealistUserControl;

        public IdealistPlugin()
        {

        }
        public void StartPlugin(RadegastInstance inst)
        {
            return;
            Instance = inst;
            XmlConfigurator.Configure();


            ArgvConfigSource configSource = new ArgvConfigSource(new string[0]);
            configSource.Alias.AddAlias("On", true);
            configSource.Alias.AddAlias("Off", false);
            configSource.Alias.AddAlias("True", true);
            configSource.Alias.AddAlias("False", false);

            idealistUserControl = new IdealistUserControl();
            IV = new RadegastViewer(inst, configSource, idealistUserControl);
            IV.Startup();
            inst.TabConsole.AddTab("Idealist", "Idealist", idealistUserControl);
            //while (true)
            //{
            //    if (MainConsole.Instance != null)
            //    {
            //        MainConsole.Instance.Prompt();
            //        Thread.Sleep(100);
            //    }
            //}
        }

        public void StopPlugin(RadegastInstance inst)
        {
            // throw new NotImplementedException();
        }
    }
}
