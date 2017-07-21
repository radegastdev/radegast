using OpenMetaverse;

namespace RadegastSpeech
{
    internal abstract class AreaControl
    {
        protected PluginControl control;
        protected GridClient Client => control.instance.Client;
        protected Talk.Control Talker => control.talker;

        internal AreaControl(PluginControl pc)
        {
            control = pc;
        }

        internal abstract void Shutdown();
        internal abstract void Start();

    }
}
