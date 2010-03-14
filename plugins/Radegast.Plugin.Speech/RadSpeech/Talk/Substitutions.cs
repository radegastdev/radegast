using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OpenMetaverse.StructuredData;

namespace RadegastSpeech.Talk
{
    internal class Substitutions
    {
        private List<Tuple> patterns;

        public Substitutions( PluginControl pc)
        {
            // TODO read this from the config file.
            patterns = new List<Tuple>();

            // Load standard substitutions
                Add("wb", "welcome back");
                Add(@"\^\^", "ha ha");
                Add(@"\^\s\^", "ha ha");
                Add(@"\^_\^", "ha ha");
                Add(@":\)", "ha ha");
                Add(@":-\)", "ha ha");
                Add("O.O", "wow");
                Add("afk", "away from keyboard");
                Add("lol", "ha ha");
                Add(@":\(", "frowns.");
                Add(@"\.\.+", ",");
                Add("lmao", "ha ha ha!");
                Add("w00t", "woot");
                Add("np", "no problem");
                Add("btw", "by the way");
                Add(":D", "ha ha");
                Add("brb", "Be right back.");
                Add("omg", "oh my god.");
                Add(@"http(s)*://[a-z0-9\./=%&?+-]+", ", a URL,");

            // Load custom substitutions
            OSDMap subs = pc.config["substitutions"] as OSDMap;
            if (subs != null)
            {
                foreach (string key in subs.Keys)
                {
                    Add( key, subs[key].AsString() );
                }
            }

            // Strip out non-pronouncable characters for now.
            // TODO more language support.
            // For some reason \P{IsBasicLatin} matches the letter "i"
           Add(@"\p{IsHiragana}|p\{IsKatakana}");
           Add(@"[*<>-]");
        }

        public string FixExpressions(string input)
        {
            if (input == null) return null;
            foreach (Tuple t in patterns)
            {
                input = t.pattern.Replace(input, t.replace);
            }
            return input.Trim();
        }

        /// <summary>
        /// Add a rule to replace a symbol with an expression
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        internal void Add(string a, string b)
        {
            patterns.Add(new Tuple(@"\b" + a + @"\b", " " + b + " "));
        }
        /// <summary>
        /// Add a rule to remove a pattern
        /// </summary>
        /// <param name="a"></param>
        internal void Add(string a)
        {
            patterns.Add(new Tuple(a, ""));

        }

        class Tuple
        {
            public Regex pattern;
            public string replace;
            /// <summary>
            /// Tuple to replace a pattern
            /// </summary>
            /// <param name="a"></param>
            /// <param name="b"></param>
            public Tuple(string a, string b)
            {
                pattern = new Regex(a, RegexOptions.IgnoreCase);
                replace = b;
            }

        }
    }

}
