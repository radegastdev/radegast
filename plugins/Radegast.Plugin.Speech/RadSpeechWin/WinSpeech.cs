using System;
using System.Collections.Generic;
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
            synth?.Halt();
        }

        public Dictionary<string, AvailableVoice> GetVoices()
        {
            return synth.GetVoices();
        }

        public void Speak(
            QueuedSpeech utterance,
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
            OnRecognition?.Invoke(text);
        }

        public void RecogStop()
        {
            recog?.Stop();
        }

        public void CreateGrammar( string name, string[] alternatives )
        {
            recog?.CreateGrammar(name, alternatives);
        }

        public void ActivateGrammar(string name)
        {
            recog?.ActivateGrammar(name);
        }

        public void DeactivateGrammar(string name)
        {
            recog?.DeactivateGrammar(name);
        }
        #endregion
    }
}
