// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id$
//
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Radegast
{
    public struct LSLKeyWord
    {
        public string KeyWord;
        public string ToolTip;
        public Color Color;

        public override string ToString()
        {
            return KeyWord;
        }
    }

    public class LSLKeywordParser
    {
        private static Dictionary<string, LSLKeyWord> keyWords;
        
        public static Dictionary<string, LSLKeyWord> KeyWords
        {
            get
            {
                if (keyWords != null)
                    return keyWords;
                else
                {
                    keyWords = Parse();
                    return keyWords;
                }
            }
        }

        private static Regex sectionRegex = new Regex(@"\[(\w+)\s+([\d.]+)\s*,\s*([\d.]+)\s*,\s*([\d.]+)\s*\]", RegexOptions.Compiled);
        private static Regex keyWordRegex = new Regex(@"^([^\s]+)(\s+(.*))?", RegexOptions.Compiled);

        public static Dictionary<string, LSLKeyWord> Parse()
        {
            Dictionary<string, LSLKeyWord> ret = new Dictionary<string, LSLKeyWord>();
            Color currentColor = Color.Black;

            bool valid = false; 
            string section = string.Empty;

            foreach (String line in Properties.Resources.lsl_keywords.Split('\n'))
            {
                Match m;

                string l = line.Trim();
                // Skip comments
                if (l.StartsWith("#"))
                {
                    continue;
                }

                // Looking for line like [word 0, .3, .5]
                if ((m = sectionRegex.Match(l)).Success)
                {
                    section = m.Groups[1].Value;
                    if (section == "word")
                    {
                        valid = true;
                        float r = float.Parse(m.Groups[2].Value, System.Globalization.CultureInfo.InvariantCulture);
                        float g = float.Parse(m.Groups[3].Value, System.Globalization.CultureInfo.InvariantCulture);
                        float b = float.Parse(m.Groups[4].Value, System.Globalization.CultureInfo.InvariantCulture);
                        currentColor = Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
                    }
                    else
                    {
                        valid = false;
                    }
                    continue;
                }

                if ((m = keyWordRegex.Match(l)).Success)
                {
                    LSLKeyWord kw = new LSLKeyWord();
                    kw.KeyWord = m.Groups[1].Value;
                    kw.ToolTip = m.Groups[3].Value.Replace(@"\n", "\n");
                    kw.Color = currentColor;

                    if (valid)
                    {
                        ret.Add(kw.KeyWord, kw);
                    }
                    continue;
                }
            }
            return ret;
        }
    }
}
