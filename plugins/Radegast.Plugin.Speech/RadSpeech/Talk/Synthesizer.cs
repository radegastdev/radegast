using System.Collections.Generic;

namespace RadegastSpeech.Talk
{
    internal class Synthesizer
    {
        protected PluginControl control;
        protected readonly string[] BeepNames;

        internal Synthesizer(PluginControl pc)
        {
            control = pc;

            // The following list must be in order of BeepType.
            BeepNames = new string[] {
                "ConfirmBeep.wav",
                "CommBeep.wav",
                "DialogBeep.wav",
                "BadBeep.wav",
                "GoodBeep.wav",
                "MoneyBeep.wav",
                "OpenBeep.wav",
                "CloseBeep.wav"};

            control.osLayer?.SpeechStart(
                control,
                BeepNames);
		}

        internal void Start()
        {
        }

        internal void Stop()
        {
            control.osLayer?.SpeechHalt();
        }

        internal Dictionary<string, AvailableVoice> GetVoices()
        {
            return control.osLayer?.GetVoices();
        }

        internal void Speak(QueuedSpeech q, string outputfile)
        {
            control.osLayer?.Speak(q, outputfile);
        }

        internal void Shutdown()
        {
            control.osLayer?.SpeechStop();
        }

    }
}
