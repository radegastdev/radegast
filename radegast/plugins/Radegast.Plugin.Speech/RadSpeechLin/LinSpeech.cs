using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RadegastSpeech;
using RadegastSpeech.Talk;

namespace RadegastSpeech
{
    public class LinSpeech : IRadSpeech
    {
        private LinSynth synth;

        public event SpeechEventHandler OnRecognition;

        public LinSpeech(  )
        {
        }

 
        #region Recognition
        // Speech recognition is not yet available on Linux
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
        #endregion
        #region Speech
        public void SpeechStart( PluginControl pc, string[] beeps)
        {
            synth = new LinSynth( pc, beeps);
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
        #endregion

    }
}
