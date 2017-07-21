namespace RadegastSpeech.Listen
{
    internal class Recognizer
    {
        protected PluginControl control;

        internal Recognizer(PluginControl pc)
        {
            control = pc;
        }

        internal void Start()
        {
            if (control.osLayer == null) return;
            control.osLayer.RecogStart();
            control.osLayer.OnRecognition += new SpeechEventHandler(OnRecognition);
        }
        internal void Stop()
        {
            if (control.osLayer == null) return;
            control.osLayer.OnRecognition -= new SpeechEventHandler(OnRecognition);
            control.osLayer.RecogStop();
        }
        protected void OnRecognition(string message)
        {
            control.converse.Hear(message);
        }

        internal void CreateGrammar(string name, string[] options)
        {
            control.osLayer?.CreateGrammar(name, options);
        }
        internal void ActivateGrammar(string name)
        {
            control.osLayer?.ActivateGrammar(name);
        }
        internal void DeactivateGrammar(string name)
        {
            control.osLayer?.DeactivateGrammar(name);
        }
    }
}
