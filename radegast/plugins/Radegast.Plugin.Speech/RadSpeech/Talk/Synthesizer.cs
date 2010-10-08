using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

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

            if (control.osLayer == null) return;
 
            control.osLayer.SpeechStart(
                control,
                BeepNames);
		}

        internal void Start()
        {
        }

        internal void Stop()
        {
            if (control.osLayer != null)
                control.osLayer.SpeechHalt();
        }

        internal Dictionary<string, AvailableVoice> GetVoices()
        {
            if (control.osLayer == null) return null;
            return control.osLayer.GetVoices();
        }

        internal void Speak(QueuedSpeech q, string outputfile)
        {
            if (control.osLayer == null) return;
            control.osLayer.Speak(q, outputfile);
        }

        internal void Shutdown()
        {
            if (control.osLayer == null) return;
            control.osLayer.SpeechStop();
        }

    }
}
