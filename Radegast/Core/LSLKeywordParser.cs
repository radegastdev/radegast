/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

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
                    LSLKeyWord kw = new LSLKeyWord
                    {
                        KeyWord = m.Groups[1].Value,
                        ToolTip = m.Groups[3].Value.Replace(@"\n", "\n"),
                        Color = currentColor
                    };

                    if (valid)
                    {
                        ret.Add(kw.KeyWord, kw);
                    }
                }
            }
            return ret;
        }
    }
}
