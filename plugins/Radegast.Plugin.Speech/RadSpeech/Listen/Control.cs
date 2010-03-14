using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;
using Radegast;

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
            if (recog == null) return;
            recog.CreateGrammar(name, options);
        }
        internal override void Start()
        {
            // If we have a recognizer, start it.
            if (recog != null)
                recog.Start();
        }
        internal override void Shutdown()
        {
            if (recog == null) return;
            recog.Stop();
            recog = null;
        }
        internal void ActivateGrammar(string name)
        {
            if (recog == null) return;
            recog.ActivateGrammar(name);
        }
        internal void DeactivateGrammar(string name)
        {
            if (recog == null) return;
            recog.DeactivateGrammar(name);
        }

    }
}
