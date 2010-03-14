// Define CARBON to use the lower level interface
//#define CARBON

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using RadegastSpeech.Talk;
using Radegast;
using Monobjc;
using Monobjc.Cocoa;
using System.Runtime.InteropServices;
using OpenMetaverse.StructuredData;

namespace RadegastSpeech
{
    /// <summary>
    /// Low level interface to Mac OSX speech synthesis functions.
    /// </summary>
    class MacSynth
    {
        private string[] BeepNames;
        private NSSpeechSynthesizer syn;
        private OSDMap voiceProperties;
        internal MacSynth(PluginControl pc, string[] beeps )
        {
			BeepNames = beeps;
            voiceProperties = pc.config["properties"] as OSDMap;
        }

        public void SpeechStart( )
        {
            syn = new NSSpeechSynthesizer();
        }

        internal void SpeechStop()
        {
            syn.StopSpeaking();
            syn.Dispose();
        }

        internal void Halt()
        {
            syn.StopSpeaking();
        }

        /// <summary>
        /// Tell OSX Speech Synthesizer to speak some text.
        /// </summary>
        /// <param name="utterance"></param>
        /// <param name="outputfile"></param>
        internal void Speak(QueuedSpeech utterance, string outputfile)
        {
            string message;

            // Remove any embedded command delimiters.
            string sayThis = Regex.Replace( utterance.message, @"\[\[", "" );

            if (utterance.isAction)
                // Action statements are spoken all together.  Such as
                // "/me looks nervous."  The "/me" will have been substituted
                // with the correct name earlier.
                message = utterance.speaker + " " + sayThis;
            else
                // Normal speech has the name spoken quickly and slightly softer.
                message = "[[rate +1.0;volm -10.0]]" +
                    utterance.speaker +
                    "[[rate -1.0;volm +10.0;slnc 200]]" +       // 200ms pause after name
                    sayThis;

            syn.SetVoice(utterance.voice.root.Name);

            NSURL fileURL = new NSURL("file://" + outputfile);
            syn.StartSpeakingStringToURL(message, fileURL);

            // Wait for it to finish.  This proceeds at faster than
            // speaking speed because output is to a file.
            // TODO use a callback to detect this.
            while (syn.IsSpeaking)
            {
                Thread.Sleep(200);  // Check 5x per second.
            }
        }

        internal void Stop()
        {
//            syn.DidFinishSpeaking -= new SpeechSynthesizer.DidFinishSpeakingEventHandler(syn_DidFinishSpeaking);
            syn.Dispose();
        }

        /// <summary>
        /// Get the list of available installed voices.
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, AvailableVoice> GetVoices()
        {
            Dictionary<string, AvailableVoice> names = new Dictionary<string, AvailableVoice>();

            NSArray insVoices = NSSpeechSynthesizer.AvailableVoices;
            foreach (NSString name in insVoices)
            {
                bool skip = false;

                // Check for additional information about this voice
                if (voiceProperties != null)
                {
                    string propString = voiceProperties[name].AsString();
                    if (propString != null)
                    {
                        // Properties are a series of blank-separated keywords
                        string[] props = propString.Split(' ');

                        foreach (string key in props)
                        {
                            switch (key)
                            {
                                case "ignore":
                                    skip = true;
                                    break;
                            }
                        }
                    }
                }

                // If this voice is not blocked add it to the list.
                if (!skip)
                {
                    names[name] = new MacAvailableVoice(name);
                }
            }

            return names;
        }

        private void RemoveIf(Dictionary<string, AvailableVoice> d, string key)
        {
            if (d.ContainsKey(key))
                d.Remove(key);
        }


        class MacAvailableVoice : AvailableVoice
        {
            internal MacAvailableVoice(string name)
            {
                Name = name;

                // Ask OSX for properties of this voice
                NSDictionary props = NSSpeechSynthesizer.AttributesForVoice(name);
                NSString gender = (NSString)props.ValueForKey(NSSpeechSynthesizer.NSVoiceGender);

                // TODO Deal with neuter voices
                // TODO Deal with multiple languages.
                if (gender == NSSpeechSynthesizer.NSVoiceGenderMale)
                    Male = true;
                else
                    Male = false;
            }
        }
    }
}
