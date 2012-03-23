using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RadegastSpeech;
using RadegastSpeech.Talk;
using Radegast;

namespace RadegastSpeech
{
    public class MacSpeech : IRadSpeech
    {
        private MacSynth synth;

        #pragma warning disable 67
        public event SpeechEventHandler OnRecognition;

        public MacSpeech()
        {

        }

        public void SpeechStart( PluginControl pc, string[] beeps)
        {
            synth = new MacSynth( pc, beeps);
            if (OnRecognition != null)
            {
            }
        }

        public void SpeechStop()
        {
            synth.Stop();
        }

        public void SpeechHalt()
        {
            synth.Halt();
        }
        public Dictionary<string, AvailableVoice> GetVoices()
        {
            return synth.GetVoices();
        }

        public void Speak(
            RadegastSpeech.Talk.QueuedSpeech utterance,
            string filename)
        {
            synth.Speak(utterance, filename);
        }

        public void RecogStart()
        {
            if (OnRecognition != null) // Supress compiler wanring until we have something for this
            {
            }
        }

        public void RecogStop()
        {
        }

        public void CreateGrammar(string name, string[] alternatives)
        {
        }

        public void ActivateGrammar(string name)
        {
        }

        public void DeactivateGrammar(string name)
        {
        }

    }
}
