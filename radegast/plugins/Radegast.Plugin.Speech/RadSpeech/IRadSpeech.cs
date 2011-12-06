using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadegastSpeech
{
    public delegate void SpeechEventHandler(string text);
    public interface IRadSpeech
    {
        // Speech Synthesis methods
        void SpeechStart( PluginControl pc, string[] beeps);
        void SpeechStop();
        void SpeechHalt();
        void Speak(Talk.QueuedSpeech utterance, string filename);
        Dictionary<string, Talk.AvailableVoice> GetVoices();

        // Speech Recognition methods
        void RecogStart();
        void RecogStop();
        void CreateGrammar(string name, string[] options);
        void ActivateGrammar(string name);
        void DeactivateGrammar(string name);
        event SpeechEventHandler OnRecognition;

    }
}
