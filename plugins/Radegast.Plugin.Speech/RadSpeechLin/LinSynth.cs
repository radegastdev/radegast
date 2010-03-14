
#define USEFILE
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;
using RadegastSpeech.Talk;
using System.Text.RegularExpressions;
using OpenMetaverse;
using Radegast;
using OpenMetaverse.StructuredData;

namespace RadegastSpeech
{
    /// <summary>
    /// Linux Speech Synthesis based on Festival
    /// </summary>
    class LinSynth
    {
        private string[] BeepNames;
		private string serverhost = "localhost";
		private string serverport = "1314";
        private const string SynthCommand = "/usr/bin/festival_client"; //"text2wave";
        private const string SynthArgs = " --server %S --port %P --output %F"; 
        private string ActualArgs;
        private const string LIBRARYPATH =
            "/usr/share/festival/voices/";
        private System.Text.ASCIIEncoding ToASCII;
        private int rateBias = 0;
        private OSDMap voiceProperties;

        internal LinSynth( PluginControl pc, string[] beeps)
        {
            BeepNames = beeps;
            ToASCII = new System.Text.ASCIIEncoding();
			
			OSDMap speech = pc.config["synthesizer"] as OSDMap;
            if (speech == null)
            {
                speech = new OSDMap();
                speech["server"] = new OSDString("localhost:1314");
                speech["speed"] = new OSDInteger(0);
                pc.config["synthesizer"] = speech;
                pc.SaveSpeechSettings();
            }

			string server = speech["server"].AsString();
            if (server==null)
                server = "localhost:1314";
			string[] parts = server.Split(':');
			serverhost = parts[0];
			if (parts.Length>1)
				serverport = parts[1];
			else
				serverport = "1314";
            rateBias = speech["speed"].AsInteger();
 
            // Build the festival command line args
            ActualArgs = Regex.Replace( SynthArgs, @"%S", serverhost );
			ActualArgs = Regex.Replace( ActualArgs, @"%P", serverport );
            voiceProperties = pc.config["properties"] as OSDMap;

        }

        /// <summary>
        /// Initialize resources
        /// </summary>
        internal void SpeechStart()
        {
       }

        /// <summary>
        /// Abort a synthesis run
        /// </summary>
        internal void Stop()
        {
        }

        internal void Halt()
        {
        }

        /// <summary>
        /// Release resources
        /// </summary>
        internal void SpeechStop()
        {

        }

        /// <summary>
        /// Synthesize a speech with Festival
        /// </summary>
        /// <param name="utterance"></param>
        /// <param name="outputfile"></param>
        internal void Speak(QueuedSpeech utterance, string outputfile)
        {
			string args;
            // Construct the exact sequence to say in SABLE notation.
            // SABLE is a simple XML syntax for describing how to speak something.
            // First set voice name.
			string message = "<?xml version=\"1.0\"?>\n" +
				"<!DOCTYPE SABLE PUBLIC \"-//SABLE//DTD SABLE speech mark up//EN\"\n"+
				"\"Sable.v0_2.dtd\" []>\n" +
				"<SABLE><SPEAKER NAME=\"" + utterance.voice.root.Name + "\">\n";
			
            // Then pitch and rate variations on that for more variety.
			if (utterance.voice.pitchModification > 0)
				message += "<PITCH BASE=\"+30%\">";
			else if (utterance.voice.pitchModification < 0)
				message += "<PITCH BASE=\"-30%\">";

            int effectiveSpeed = utterance.voice.rateModification + rateBias;
			if (effectiveSpeed != 0)
				message += "<RATE SPEED=\"" + effectiveSpeed.ToString("+#;-#;0") + "%\">";

			// For an action it all comes out at a constant speed.
			// For chat, the name is said quickly, with a break after it.
            if (utterance.isAction)
                message += utterance.speaker + " ";
            else
                message += "<RATE SPEED=\"+25%\">" + utterance.speaker + " </RATE><BREAK/>\n";

			// Supress any embedded tags which would confuse SABLE.
            message += Regex.Replace(utterance.message, @"[<>]", "") + "\n";
			
            // Close any modifications, in reverse order.
			if (utterance.voice.rateModification != 0)
				message += "</RATE>";
			if (utterance.voice.pitchModification != 0)
				message += "</PITCH>";

			message += "</SPEAKER></SABLE>\n";

            // Write this to a temporary file.  Filename is the output
            // filename with ".wav" changed to ".sable".
            // TODO talk directly to Festival server.
            string textfilename = Regex.Replace(outputfile, @"\.wav$", ".sable");

            FileStream tstream =
                new FileStream(textfilename, FileMode.Create, FileAccess.Write);
            byte[] msgBytes = ToASCII.GetBytes(message);

            tstream.Write(msgBytes, 0, msgBytes.Length);
            tstream.Close();
//			args = Regex.Replace( ActualArgs, @"%T", textfilename );

			// Put the desired WAV file name in the command.
            args = Regex.Replace( ActualArgs, @"%F", outputfile );

			// Run synthesizer externally
            // TODO Talk directly to the Festival Server on port 1314.
			Process proc = new Process( );
			proc.StartInfo.FileName = SynthCommand;
			proc.StartInfo.Arguments = args;
			proc.StartInfo.UseShellExecute = false;
			proc.StartInfo.RedirectStandardInput = true;
			proc.StartInfo.RedirectStandardError = false;
			proc.StartInfo.RedirectStandardOutput = false;

			try
			{
				if( proc.Start( ) )
				{
                    // Send the SCHEME command to redirect output back here
                    proc.StandardInput.WriteLine("(tts_return_to_client)");
                    // Send the SCHEME command to voice the SABLE file.
					proc.StandardInput.WriteLine("(tts \"" + textfilename + "\" 'sable)");
//					proc.StandardInput.WriteLine("(tts_textall \"" + saythis + "\" 'nil)");

					proc.StandardInput.Close();
					proc.WaitForExit( );
				}
			}
			catch( Exception e )
			{
				System.Console.WriteLine( "Festival process error " + e.Message );
				return;
			}
 
			// All done with the intermediate file.
            File.Delete(textfilename);
        }

        /// <summary>
        /// Get the list of available installed voices.
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, AvailableVoice> GetVoices()
        {
            Dictionary<string, AvailableVoice> names = new Dictionary<string, AvailableVoice>();

            // Festival organizes voices by their language.
            // TODO deal with other languages then English
            // TODO Use (voice.list) to fetch the list from Festival
            string path = LIBRARYPATH + "english/";

            // If directory not there, maybe Festival is not installed?
            if (!Directory.Exists(path)) return names;

            // Voice names are simply the names of directories.
            foreach (string voicename in Directory.GetDirectories(path))
            {
				string[] terms = voicename.Split('/');
				string name = terms[terms.Length - 1];
                bool male = true;
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
                                case "male":
                                    male = true;
                                    break;
                                case "female":
                                    male = false;
                                    break;
                                case "ignore":
                                    skip = true;
                                    break;
                            }
                        }
                    }
                }

                // If this voice is not blocked add it to the list.
                if (!skip)
                    names[name] = new LinAvailableVoice(name, male);
            }

            return names;
        }

        private void RemoveIf(Dictionary<string, AvailableVoice> d, string key)
        {
            if (d.ContainsKey(key))
                d.Remove(key);
        }

		/// <summary>
		/// The Linux representation of an installed voice. 
		/// </summary>
        class LinAvailableVoice : AvailableVoice
        {
            internal LinAvailableVoice(string name, bool m)
            {
                Name = name;
                Male = m;
            }
        }
    }
}
