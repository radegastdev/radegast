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
using System.Collections;
using System.IO;
using System.Xml;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using System.Drawing;
using System.Web.Script.Serialization;
using System.ComponentModel;

namespace Radegast
{
    public class Settings : IDictionary<string, OSD>
    {
        private string SettingsFile;
        private OSDMap SettingsData;

        public delegate void SettingChangedCallback(object sender, SettingsEventArgs e);
        public event SettingChangedCallback OnSettingChanged;

        public static readonly Dictionary<string, FontSetting> DefaultFontSettings = new Dictionary<string, FontSetting>()
        {
            {"Normal", new FontSetting {
                Name = "Normal",
                ForeColor = SystemColors.ControlText,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"StatusBlue", new FontSetting {
                Name = "StatusBlue",
                ForeColor = Color.Blue,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"StatusDarkBlue", new FontSetting {
                Name = "StatusDarkBlue",
                ForeColor = Color.DarkBlue,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"LindenChat", new FontSetting {
                Name = "LindenChat",
                ForeColor = Color.DarkGreen,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"ObjectChat", new FontSetting {
                Name = "ObjectChat",
                ForeColor = Color.DarkCyan,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"StartupTitle", new FontSetting {
                Name = "StartupTitle",
                ForeColor = SystemColors.ControlText,
                BackColor = Color.Transparent,
                Font = new Font(FontSetting.DefaultFont, FontStyle.Bold),
            }},
            {"Alert", new FontSetting {
                Name = "Alert",
                ForeColor = Color.DarkRed,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"Error", new FontSetting {
                Name = "Error",
                ForeColor = Color.Red,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"OwnerSay", new FontSetting {
                Name = "OwnerSay",
                ForeColor = Color.DarkGoldenrod,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"Timestamp", new FontSetting {
                Name = "Timestamp",
                ForeColor = SystemColors.GrayText,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"Name", new FontSetting {
                Name = "Name",
                ForeColor = SystemColors.ControlText,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"Notification", new FontSetting {
                Name = "Notification",
                ForeColor = SystemColors.ControlText,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"IncomingIM", new FontSetting {
                Name = "IncomingIM",
                ForeColor = SystemColors.ControlText,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"OutgoingIM", new FontSetting {
                Name = "OutgoingIM",
                ForeColor = SystemColors.ControlText,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"Emote", new FontSetting {
                Name = "Emote",
                ForeColor = SystemColors.ControlText,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
            {"Self", new FontSetting {
                Name = "Self",
                ForeColor = SystemColors.ControlText,
                BackColor = Color.Transparent,
                Font = FontSetting.DefaultFont,
            }},
        };

        public class FontSetting
        {
            [ScriptIgnoreAttribute]
            public static readonly Font DefaultFont = new Font(FontFamily.GenericSansSerif, 8.0f);

            [ScriptIgnoreAttribute]
            public Font Font;

            [ScriptIgnoreAttribute]
            public Color ForeColor;

            [ScriptIgnoreAttribute]
            public Color BackColor;


            public String Name;

            public string ForeColorString
            {
                get
                {
                    if (ForeColor != null)
                    {
                        return ColorTranslator.ToHtml(ForeColor);
                    }
                    else
                    {
                        return ColorTranslator.ToHtml(SystemColors.ControlText);
                    }
                }
                set
                {
                    ForeColor = ColorTranslator.FromHtml(value);
                }
            }

            public string BackColorString
            {
                get
                {
                    if (BackColor != null)
                    {
                        return ColorTranslator.ToHtml(BackColor);
                    }
                    else
                    {
                        return ColorTranslator.ToHtml(SystemColors.ControlText);
                    }
                }
                set
                {
                    BackColor = ColorTranslator.FromHtml(value);
                }
            }

            public string FontString
            {
                get
                {
                    if (this.Font != null)
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                        return converter.ConvertToString(this.Font);
                    }
                    else
                    {
                        return null;
                    }
                }
                set
                {
                    try
                    {
                        TypeConverter converter = TypeDescriptor.GetConverter(typeof(Font));
                        this.Font = converter.ConvertFromString(value) as Font;
                    }
                    catch (Exception)
                    {
                        this.Font = DefaultFont;
                    }

                }
            }

            public override string ToString()
            {
                return Name;
            }
        }

        public Settings(string fileName)
        {
            SettingsFile = fileName;

            try
            {
                string xml = File.ReadAllText(SettingsFile);
                SettingsData = (OSDMap)OSDParser.DeserializeLLSDXml(xml);
            }
            catch
            {
                Logger.DebugLog("Failed openning setting file: " + fileName);
                SettingsData = new OSDMap();
                Save();
            }
        }

        public void Save()
        {
            try
            {
                File.WriteAllText(SettingsFile, SerializeLLSDXmlStringFormated(SettingsData));
            }
            catch (Exception ex)
            {
                Logger.Log("Failed saving settings", Helpers.LogLevel.Warning, ex);
            }
        }

        public static string SerializeLLSDXmlStringFormated(OSD data)
        {
            using (StringWriter sw = new StringWriter())
            {
                using (XmlTextWriter writer = new XmlTextWriter(sw))
                {
                    writer.Formatting = Formatting.Indented;
                    writer.Indentation = 4;
                    writer.IndentChar = ' ';

                    writer.WriteStartElement(String.Empty, "llsd", String.Empty);
                    OSDParser.SerializeLLSDXmlElement(writer, data);
                    writer.WriteEndElement();

                    writer.Close();

                    return sw.ToString();
                }
            }
        }

        private void FireEvent(string key, OSD val)
        {
            if (OnSettingChanged != null)
            {
                try { OnSettingChanged(this, new SettingsEventArgs(key, val)); }
                catch (Exception) {}
            }
        }

        #region IDictionary Implementation

        public int Count { get { return SettingsData.Count; } }
        public bool IsReadOnly { get { return false; } }
        public ICollection<string> Keys { get { return SettingsData.Keys; } }
        public ICollection<OSD> Values { get { return SettingsData.Values; } }
        public OSD this[string key]
        {
            get
            {
                return SettingsData[key];
            }
            set 
            {
                if (string.IsNullOrEmpty(key))
                {
                    Logger.DebugLog("Warning: trying to set an emprty setting: " + Environment.StackTrace.ToString());
                }
                else
                {
                    SettingsData[key] = value;
                    FireEvent(key, value);
                    Save();
                }
            }
        }

        public bool ContainsKey(string key)
        {
            return SettingsData.ContainsKey(key);
        }

        public void Add(string key, OSD llsd)
        {
            SettingsData.Add(key, llsd);
            FireEvent(key, llsd);
            Save();
        }

        public void Add(KeyValuePair<string, OSD> kvp)
        {
            SettingsData.Add(kvp.Key, kvp.Value);
            FireEvent(kvp.Key, kvp.Value);
            Save();
        }

        public bool Remove(string key)
        {
            bool ret = SettingsData.Remove(key);
            FireEvent(key, null);
            Save();
            return ret;
        }

        public bool TryGetValue(string key, out OSD llsd)
        {
            return SettingsData.TryGetValue(key, out llsd);
        }

        public void Clear()
        {
            SettingsData.Clear();
            Save();
        }

        public bool Contains(KeyValuePair<string, OSD> kvp)
        {
            // This is a bizarre function... we don't really implement it
            // properly, hopefully no one wants to use it
            return SettingsData.ContainsKey(kvp.Key);
        }

        public void CopyTo(KeyValuePair<string, OSD>[] array, int index)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, OSD> kvp)
        {
            bool ret = SettingsData.Remove(kvp.Key);
            FireEvent(kvp.Key, null);
            Save();
            return ret;
        }

        public System.Collections.IDictionaryEnumerator GetEnumerator()
        {
            return SettingsData.GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, OSD>> IEnumerable<KeyValuePair<string, OSD>>.GetEnumerator()
        {
            return null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return SettingsData.GetEnumerator();
        }

        #endregion IDictionary Implementation

    }

    public class SettingsEventArgs : EventArgs
    {
        public string Key = string.Empty;
        public OSD Value = new OSD();

        public SettingsEventArgs(string key, OSD val)
        {
            Key = key;
            Value = val;
        }
    }
}
