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
// $Id: CommandsManager.cs 226 2009-09-12 18:10:30Z logicmoo $
//
using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using OpenMetaverse;

namespace Radegast.Commands
{
    public class ParcelInfoCommand : RadegastCommand
    {
        private ManualResetEvent ParcelsDownloaded = new ManualResetEvent(false);
        private RadegastInstance instance;

        public ParcelInfoCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "parcelinfo";
            Description = "Prints out info about all the parcels in this simulator";
            Usage = Name;

            this.instance = instance;
            this.instance.Netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);
        }

        public override void Dispose()
        {
            base.Dispose();
            instance.Netcom.ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(Netcom_ClientDisconnected);
        }

        public override void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            StringBuilder sb = new StringBuilder();
            string result;

            EventHandler<SimParcelsDownloadedEventArgs> del = delegate(object sender, SimParcelsDownloadedEventArgs e)
            {
                ParcelsDownloaded.Set();
            };

            instance.MainForm.PreventParcelUpdate = true;

            ParcelsDownloaded.Reset();
            Client.Parcels.SimParcelsDownloaded += del;
            Client.Parcels.RequestAllSimParcels(Client.Network.CurrentSim, true, 750);

            if (Client.Network.CurrentSim.IsParcelMapFull())
                ParcelsDownloaded.Set();

            if (ParcelsDownloaded.WaitOne(30000, false) && Client.Network.Connected)
            {
                sb.AppendFormat("Downloaded {0} Parcels in {1} " + System.Environment.NewLine,
                    Client.Network.CurrentSim.Parcels.Count, Client.Network.CurrentSim.Name);

                Client.Network.CurrentSim.Parcels.ForEach(delegate(Parcel parcel)
                {
                    sb.AppendFormat("Parcel[{0}]: Name: \"{1}\", Description: \"{2}\" ACLBlacklist Count: {3}, ACLWhiteList Count: {5} Traffic: {4}" + System.Environment.NewLine,
                        parcel.LocalID, parcel.Name, parcel.Desc, parcel.AccessBlackList.Count, parcel.Dwell, parcel.AccessWhiteList.Count);
                });

                result = sb.ToString();
            }
            else
                result = "Failed to retrieve information on all the simulator parcels";

            Client.Parcels.SimParcelsDownloaded -= del;

            Thread.Sleep(2000);
            instance.MainForm.PreventParcelUpdate = false;

            WriteLine("Parcel Infro results:\n{0}", result);
        }

        void Netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            ParcelsDownloaded.Set();
        }
    }
}
