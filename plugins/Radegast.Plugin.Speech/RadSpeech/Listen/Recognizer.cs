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

namespace RadegastSpeech.Listen
{
    internal class Recognizer
    {
        protected PluginControl control;

        internal Recognizer(PluginControl pc)
        {
            control = pc;
        }

        internal void Start()
        {
            if (control.osLayer == null) return;
            control.osLayer.RecogStart();
            control.osLayer.OnRecognition += new SpeechEventHandler(OnRecognition);
        }
        internal void Stop()
        {
            if (control.osLayer == null) return;
            control.osLayer.OnRecognition -= new SpeechEventHandler(OnRecognition);
            control.osLayer.RecogStop();
        }
        protected void OnRecognition(string message)
        {
            control.converse.Hear(message);
        }

        internal void CreateGrammar(string name, string[] options)
        {
            control.osLayer?.CreateGrammar(name, options);
        }
        internal void ActivateGrammar(string name)
        {
            control.osLayer?.ActivateGrammar(name);
        }
        internal void DeactivateGrammar(string name)
        {
            control.osLayer?.DeactivateGrammar(name);
        }
    }
}
