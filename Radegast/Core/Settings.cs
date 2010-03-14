// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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

namespace Radegast
{
    public class Settings : IDictionary<string, OSD>
    {
        private string SettingsFile;
        private OSDMap SettingsData;

        public delegate void SettingChangedCallback(object sender, SettingsEventArgs e);
        public event SettingChangedCallback OnSettingChanged;

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
                SettingsData[key] = value;
                FireEvent(key, value);
                Save();
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
