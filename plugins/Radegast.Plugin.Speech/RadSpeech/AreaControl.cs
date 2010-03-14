using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

namespace RadegastSpeech
{
    internal abstract class AreaControl
    {
        protected PluginControl control;
        protected GridClient Client { get { return control.instance.Client; } }
        protected Talk.Control Talker { get { return control.talker; } }

        internal AreaControl(PluginControl pc)
        {
            control = pc;
        }

        internal abstract void Shutdown();
        internal abstract void Start();

    }
}
