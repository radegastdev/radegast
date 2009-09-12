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
// $Id: DettachableControl.cs 211 2009-09-09 06:46:18Z latifer@gmail.com $
//
using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.ComponentModel;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
using Radegast.Netcom;

namespace Radegast
{
    [Browsable(false)]
    public class RadegastForm : Form
    {
        /// <summary>
        /// Indicates if position and size of the form should be saved
        /// </summary>
        [Browsable(true),
            Description("Indicates if position and size of the form should be saved"),
            Category("Behavior"),
            DefaultValue(false)
        ]
        public bool AutoSavePosition
        {
            get
            {
                return autoSavePosition;
            }

            set
            {
                if (value == false)
                {
                    ClearSavedPosition();
                }
                autoSavePosition = value;
            }
        }
        private bool autoSavePosition = false;

        /// <summary>
        /// Instance of Radegast
        /// </summary>
        protected RadegastInstance Instance { get { return instance; } }
        private RadegastInstance instance;

        /// <summary>
        /// Instance of OpenMetaverse's GridClient
        /// </summary>
        protected GridClient Client { get { return instance.Client; } }

        /// <summary>
        /// Instance of RadegastNetcom
        /// </summary>
        protected RadegastNetcom Netcom { get { return instance.Netcom; } }

        private System.Threading.Timer SettingsTimer;
        private const int SettingsTimerTimeout = 3000;

        public RadegastForm()
            : base()
        {
            InitForm();
        }

        public RadegastForm(RadegastInstance instance)
            : base()
        {
            this.instance = instance;
            InitForm();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (SettingsTimer != null)
                {
                    SettingsTimer.Dispose();
                    SettingsTimer = null;
                }
            }
            base.Dispose(disposing);
        }

        private void InitForm()
        {
            SettingsTimer = new System.Threading.Timer(
                SettingsTimer_Tick,
                null,
                System.Threading.Timeout.Infinite,
                System.Threading.Timeout.Infinite);
        }

        protected void SavePosition()
        {
            Instance.GlobalSettings[GetSettingsKey("left")] = OSD.FromInteger(Left);
            Instance.GlobalSettings[GetSettingsKey("top")] = OSD.FromInteger(Top);
            Instance.GlobalSettings[GetSettingsKey("width")] = OSD.FromInteger(Width);
            Instance.GlobalSettings[GetSettingsKey("height")] = OSD.FromInteger(Height);
        }

        protected void ClearSavedPosition()
        {
            Instance.GlobalSettings.Remove(GetSettingsKey("left"));
            Instance.GlobalSettings.Remove(GetSettingsKey("top"));
            Instance.GlobalSettings.Remove(GetSettingsKey("width"));
            Instance.GlobalSettings.Remove(GetSettingsKey("height"));
        }

        protected void RestoreSavedPosition()
        {
            if (Instance.GlobalSettings[GetSettingsKey("left")].Type != OSDType.Unknown)
                Left = Instance.GlobalSettings[GetSettingsKey("left")].AsInteger();

            if (Instance.GlobalSettings[GetSettingsKey("top")].Type != OSDType.Unknown)
                Top = Instance.GlobalSettings[GetSettingsKey("top")].AsInteger();

            if (Instance.GlobalSettings[GetSettingsKey("width")].Type != OSDType.Unknown)
                Width = Instance.GlobalSettings[GetSettingsKey("width")].AsInteger();

            if (Instance.GlobalSettings[GetSettingsKey("height")].Type != OSDType.Unknown)
                Height = Instance.GlobalSettings[GetSettingsKey("height")].AsInteger();
        }
        
        protected virtual string SettingsKeyBase()
        {
            return GetType().ToString();
        }

        protected virtual string GetSettingsKey(string arg)
        {
            return SettingsKeyBase() + "." + arg;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (AutoSavePosition)
                RestoreSavedPosition();
        }
        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            if (AutoSavePosition)
                TriggerSavePosition();
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            if (AutoSavePosition)
                TriggerSavePosition();
        }

        private void TriggerSavePosition()
        {
            if (SettingsTimer != null)
            {
                SettingsTimer.Change(
                    SettingsTimerTimeout,
                    System.Threading.Timeout.Infinite);
            }
        }

        private void SettingsTimer_Tick(object e)
        {
            SavePosition();
        }
    }
}
