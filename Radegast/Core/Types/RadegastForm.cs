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
// $Id: DettachableControl.cs 211 2009-09-09 06:46:18Z latifer@gmail.com $
//
using System;
using System.Threading;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.ComponentModel;
using System.Drawing;
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

        [Browsable(true),
            Description("Key used for saving this particular windows settings"),
            Category("Behavior"),
            DefaultValue(false)
        ]
        public virtual string SettingsKeyBase
        {
            get
            {
                if (settingsKeyBase == string.Empty)
                    return GetType().ToString();
                else
                    return settingsKeyBase;
            }

            set
            {
                settingsKeyBase = value;
            }
        }
        private string settingsKeyBase = string.Empty;

        /// <summary>
        /// Instance of Radegast
        /// </summary>
        protected RadegastInstance Instance { get { return instance; } }
        private RadegastInstance instance = null;

        /// <summary>
        /// Instance of OpenMetaverse's GridClient
        /// </summary>
        protected GridClient Client { get { return instance.Client; } }

        /// <summary>
        /// Instance of RadegastNetcom
        /// </summary>
        protected RadegastNetcom Netcom { get { return instance.Netcom; } }

        private System.Threading.Timer SettingsTimer;
        private const int SettingsTimerTimeout = 500;

        public RadegastForm()
            : base()
        {
        }

        public RadegastForm(RadegastInstance instance)
            : base()
        {
            this.instance = instance;
            instance.OnRadegastFormCreated(this);
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

        private void InitTimer()
        {
            SettingsTimer = new System.Threading.Timer(
                SettingsTimer_Tick,
                null,
                System.Threading.Timeout.Infinite,
                System.Threading.Timeout.Infinite);
        }

        protected void SavePosition()
        {
            if (instance == null) return;

            Instance.GlobalSettings[GetSettingsKey("left")] = OSD.FromInteger(Left);
            Instance.GlobalSettings[GetSettingsKey("top")] = OSD.FromInteger(Top);
            Instance.GlobalSettings[GetSettingsKey("width")] = OSD.FromInteger(Width);
            Instance.GlobalSettings[GetSettingsKey("height")] = OSD.FromInteger(Height);
        }

        protected void ClearSavedPosition()
        {
            if (instance == null) return;

            Instance.GlobalSettings.Remove(GetSettingsKey("left"));
            Instance.GlobalSettings.Remove(GetSettingsKey("top"));
            Instance.GlobalSettings.Remove(GetSettingsKey("width"));
            Instance.GlobalSettings.Remove(GetSettingsKey("height"));
        }

        protected void RestoreSavedPosition()
        {
            if (instance == null) return;

            int left = Left, top = Top, width = Width, height = Height;

            if (Instance.GlobalSettings[GetSettingsKey("left")].Type != OSDType.Unknown)
                left = Instance.GlobalSettings[GetSettingsKey("left")].AsInteger();

            if (Instance.GlobalSettings[GetSettingsKey("top")].Type != OSDType.Unknown)
                top = Instance.GlobalSettings[GetSettingsKey("top")].AsInteger();

            if (Instance.GlobalSettings[GetSettingsKey("width")].Type != OSDType.Unknown)
                width = Instance.GlobalSettings[GetSettingsKey("width")].AsInteger();

            if (Instance.GlobalSettings[GetSettingsKey("height")].Type != OSDType.Unknown)
                height = Instance.GlobalSettings[GetSettingsKey("height")].AsInteger();

            Rectangle rec = Screen.GetWorkingArea(new Rectangle(left, top, width, height));

            if (left > rec.X + rec.Width - width) left = rec.X + rec.Width - width;
            if (top > rec.Y + rec.Height - height) top = rec.Y + rec.Height - height;
            if (left < rec.X) left = rec.X;
            if (top < rec.Y) top = rec.Y;
            if (width > rec.Width) width = rec.Width;
            if (height > rec.Height) height = rec.Height;

            Left = left;
            Top = top;
            Width = width;
            Height = height;
        }

        protected virtual string GetSettingsKey(string arg)
        {
            return SettingsKeyBase + "." + arg;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (AutoSavePosition)
                RestoreSavedPosition();
            InitTimer();
        }

        protected override void OnMove(EventArgs e)
        {
            base.OnMove(e);
            if (AutoSavePosition && WindowState != FormWindowState.Minimized)
                TriggerSavePosition();
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            base.OnResizeEnd(e);
            if (AutoSavePosition && WindowState != FormWindowState.Minimized)
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
