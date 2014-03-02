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
// $Id: RadegastInstance.cs 234 2009-09-13 12:45:52Z logicmoo $
//
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Net;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;


namespace Radegast
{
    public class UpdateChecker : IDisposable
    {
        public AssemblyName MyAssembly;

        public delegate void UpdateInfoCallback(object sender, UpdateCheckerArgs e);

        public event UpdateInfoCallback OnUpdateInfoReceived;

        private WebClient client;

        public UpdateChecker()
        {
            MyAssembly = Assembly.GetExecutingAssembly().GetName();
        }

        public void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
        }

        public void StartCheck()
        {
            if (client == null)
            {
                client = new WebClient();
                client.DownloadStringCompleted += new DownloadStringCompletedEventHandler(OnDownloadStringCompleted);
            }

            WorkPool.QueueUserWorkItem((object state) =>
                {
                    client.DownloadStringAsync(new Uri(Properties.Resources.UpdateCheckUri));
                }
            );
        }

        private void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Logger.Log("Failed fetching updatede information: ", Helpers.LogLevel.Warning, e.Error);
                FireCallback(new UpdateCheckerArgs() { Success = false });
            }
            else
            {
                try
                {
                    OSDMap upd = OSDParser.DeserializeJson(e.Result) as OSDMap;
                    UpdateInfo inf = new UpdateInfo()
                    {
                        Error = upd["Error"].AsBoolean(),
                        ErrMessage = upd["ErrMessage"].AsString(),
                        CurrentVersion = upd["CurrentVersion"].AsString(),
                        DownloadSite = upd["DownloadSite"].AsString(),
                        DisplayMOTD = upd["DisplayMOTD"].AsBoolean(),
                        MOTD = upd["MOTD"].AsString(),
                        UpdateAvailable = MyAssembly.Version < new Version(upd["CurrentVersion"].AsString())
                    };

                    FireCallback(new UpdateCheckerArgs() { Success = !inf.Error, Info = inf });
                }
                catch (Exception ex)
                {
                    Logger.Log("Failed decoding updatede information: ", Helpers.LogLevel.Warning, ex);
                    FireCallback(new UpdateCheckerArgs() { Success = false });
                }
            }
        }

        private void FireCallback(UpdateCheckerArgs e)
        {
            if (OnUpdateInfoReceived != null)
            {
                try { OnUpdateInfoReceived(this, e); }
                catch { }
            }
        }
    }

    public class UpdateInfo
    {
        public bool Error { get; set; }
        public string ErrMessage { get; set; }
        public string CurrentVersion { get; set; }
        public string DownloadSite { get; set; }
        public bool DisplayMOTD { get; set; }
        public string MOTD { get; set; }
        public bool UpdateAvailable { get; set; }
    }

    public class UpdateCheckerArgs : EventArgs
    {
        public bool Success { get; set; }
        public UpdateInfo Info { get; set; }
    }

}
