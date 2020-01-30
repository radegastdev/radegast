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
using System.Reflection;
using System.Net;
using System.Threading;
using OpenMetaverse;
using OpenMetaverse.StructuredData;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif


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
            if (client == null) return;

            client.Dispose();
            client = null;
        }

        public void StartCheck()
        {
            if (client == null)
            {
                client = new WebClient();
                client.DownloadStringCompleted += OnDownloadStringCompleted;
            }

            ThreadPool.QueueUserWorkItem(state =>
                {
                    client.DownloadStringAsync(new Uri(Properties.Resources.UpdateCheckUri));
                }
            );
        }

        private void OnDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Logger.Log("Failed fetching updated information: ", Helpers.LogLevel.Warning, e.Error);
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
                    Logger.Log("Failed decoding updated information: ", Helpers.LogLevel.Warning, ex);
                    FireCallback(new UpdateCheckerArgs() { Success = false });
                }
            }
        }

        private void FireCallback(UpdateCheckerArgs e)
        {
            if (OnUpdateInfoReceived == null) return;
            try
            {
                OnUpdateInfoReceived(this, e);
            }
            catch { }
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
