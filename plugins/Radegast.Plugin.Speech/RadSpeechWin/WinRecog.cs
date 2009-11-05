using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Speech.Recognition;
using OpenMetaverse;

namespace RadegastSpeech
{
    internal class WinRecog
    {
        private SpeechRecognizer rec;
        private DictationGrammar dGrammar;
        private Dictionary<string, Grammar> cGrammars;
        internal event SpeechEventHandler OnWinRecognition;

        internal WinRecog( )
        {
            try
            {
                rec = new SpeechRecognizer();
            }
            catch (Exception e)
            {
                rec = null;
                Logger.Log("Speech recognition disabled, " + e.Message,
                    Helpers.LogLevel.Warning);
                return;
            }
            dGrammar = new DictationGrammar();
            rec.LoadGrammar(dGrammar);
            cGrammars = new Dictionary<string, Grammar>();
        }

        internal void Start()
        {
            if (rec == null) return;
            rec.Enabled = true;
            rec.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(onSpeech);
        }

        internal void Stop()
        {
            if (rec == null) return;
            rec.Enabled = false;
            rec.SpeechRecognized -= new EventHandler<SpeechRecognizedEventArgs>(onSpeech);
            rec.Dispose();
            rec = null;
        }

        /// <summary>
        /// Add a grammar to the active list
        /// </summary>
        /// <param name="name"></param>
        internal void ActivateGrammar(string name)
        {
            if (rec.Grammars.Contains(cGrammars[name]))
                return;
            rec.LoadGrammar(cGrammars[name]);
        }

        /// <summary>
        /// Remove a grammar from active list
        /// </summary>
        /// <param name="name"></param>
        internal void DeactivateGrammar(string name)
        {
            // Avoid exceptions from deactiovating what is not active
            Grammar v = cGrammars[name];
            if (!rec.Grammars.Contains(v))
                return;
            rec.UnloadGrammar(v);
        }

        void onSpeech(object sender, SpeechRecognizedEventArgs e)
        {
            if (OnWinRecognition == null) return;

            RecognitionResult r = e.Result;
            OnWinRecognition(r.Text);
        }

        internal void CreateGrammar(string name, string[] options)
        {
            GrammarBuilder gb = new GrammarBuilder();
            gb.Append(new Choices(options));
            cGrammars[name] = new Grammar(gb);
        }

    }
}
