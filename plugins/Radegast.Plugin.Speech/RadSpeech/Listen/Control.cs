namespace RadegastSpeech.Listen
{
    class Control : AreaControl
    {
        private Recognizer recog = null;
        internal Control(PluginControl pc)
            : base(pc)
        {
            recog = new Recognizer(control);
        }

        internal void CreateGrammar(string name, string[] options)
        {
            recog?.CreateGrammar(name, options);
        }
        internal override void Start()
        {
            // If we have a recognizer, start it.
            recog?.Start();
        }
        internal override void Shutdown()
        {
            if (recog == null) return;
            recog.Stop();
            recog = null;
        }
        internal void ActivateGrammar(string name)
        {
            recog?.ActivateGrammar(name);
        }
        internal void DeactivateGrammar(string name)
        {
            recog?.DeactivateGrammar(name);
        }

    }
}
