using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RadegastSpeech;
using RadegastSpeech.Talk;

namespace RadegastSpeech
{
    /// <summary>
    /// Main interface to the Speech OS Layer for Microsoft Windows.
    /// </summary>
    public class WinSpeech : IRadSpeech
    {
        private WinSynth synth;
        private WinRecog recog;

        public event SpeechEventHandler OnRecognition;

        public WinSpeech()
        {

        }

        #region Synthesis
        public void SpeechStart( PluginControl pc, string[] beeps)
        {
            synth = new WinSynth( pc, beeps);
            synth.SpeechStart();
        }

        public void SpeechStop()
        {
            if (synth != null)
            {
                synth.SpeechStop();
                synth = null;
            }

            if (recog != null)
            {
                recog.Stop();
                recog = null;
            }
        }

        public void SpeechHalt()
        {
            if (synth != null)
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
        #region Recognition
        public void RecogStart()
        {
            try
            {
                recog = new WinRecog();
                recog.Start();

                // Hook up event relay
                recog.OnWinRecognition +=
                    new SpeechEventHandler(recog_OnWinRecognition);
            }
            catch (Exception)
            {
                // No recognition on this platform
                recog = null;
            }
        }

        private void recog_OnWinRecognition(string text)
        {
            if (OnRecognition != null)
                OnRecognition(text);
        }

        public void RecogStop()
        {
            if (recog != null)
                recog.Stop();
        }

        public void CreateGrammar( string name, string[] alternatives )
        {
            if (recog != null)
                recog.CreateGrammar(name, alternatives);
        }

        public void ActivateGrammar(string name)
        {
            if (recog != null)
                recog.ActivateGrammar(name);
        }

        public void DeactivateGrammar(string name)
        {
            if (recog != null)
                recog.DeactivateGrammar(name);
        }
        #endregion
    }
}
