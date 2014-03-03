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
using System.Threading;
using System.Text;
using OpenMetaverse;
using OpenMetaverse.Messages.Linden;

namespace Radegast.Commands
{
    public class ScriptInfoCommand : RadegastCommand
    {
        private RadegastInstance instance;

        public ScriptInfoCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "scriptinfo";
            Description = "Prints out available information about the current script usage";
            Usage = "scriptinfo (avatar|parcel) - display script resource usage details about your avatar or parcel you are on. If not specified avatar is assumed";

            this.instance = instance;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            if (cmdArgs.Length == 0)
            {
                AttachmentInfo(WriteLine);
            }
            else if (cmdArgs[0] == "avatar")
            {
                AttachmentInfo(WriteLine);
            }
            else if (cmdArgs[0] == "parcel")
            {
                ParcelInfo(WriteLine);
            }

        }

        public void ParcelInfo(ConsoleWriteLine WriteLine)
        {
            WriteLine("Requesting script resources information...");
            UUID currectParcel = Client.Parcels.RequestRemoteParcelID(Client.Self.SimPosition, Client.Network.CurrentSim.Handle, Client.Network.CurrentSim.ID);
            Client.Parcels.GetParcelResouces(currectParcel, true, (bool success, LandResourcesInfo info) =>
            {
                if (!success || info == null) return;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Summary:");
                sb.AppendFormat("Memory used {0} KB out of {1} KB available.", info.SummaryUsed["memory"] / 1024, info.SummaryAvailable["memory"] / 1024);
                sb.AppendLine();
                sb.AppendFormat("URLs used {0} out of {1} available.", info.SummaryUsed["urls"], info.SummaryAvailable["urls"]);
                sb.AppendLine();

                if (info.Parcels != null)
                {
                    for (int i = 0; i < info.Parcels.Length; i++)
                    {
                        sb.AppendLine();
                        sb.AppendLine("Detailed usage for parcel " + info.Parcels[i].Name);
                        for (int j = 0; j < info.Parcels[i].Objects.Length; j++)
                        {
                            sb.AppendFormat("{0} KB - {1}", info.Parcels[i].Objects[j].Resources["memory"] / 1024, info.Parcels[i].Objects[j].Name);
                            sb.AppendLine();
                        }
                    }
                }
                WriteLine(sb.ToString());
            });
        }

        public void AttachmentInfo(ConsoleWriteLine WriteLine)
        {
            Client.Self.GetAttachmentResources((bool success, AttachmentResourcesMessage info) =>
            {
                if (!success || info == null)
                {
                    WriteLine("Failed to get the script info.");
                    return;
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Summary:");
                sb.AppendFormat("Memory used {0} KB out of {1} KB available.", info.SummaryUsed["memory"] / 1024, info.SummaryAvailable["memory"] / 1024);
                sb.AppendLine();
                sb.AppendFormat("URLs used {0} out of {1} available.", info.SummaryUsed["urls"], info.SummaryAvailable["urls"]);
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine("Details:");

                foreach (KeyValuePair<AttachmentPoint, ObjectResourcesDetail[]> kvp in info.Attachments)
                {
                    sb.AppendLine();
                    sb.AppendLine(string.Format("Attached to {0} ", Utils.EnumToText(kvp.Key)) + ":");
                    for (int i = 0; i < kvp.Value.Length; i++)
                    {
                        ObjectResourcesDetail obj = kvp.Value[i];
                        sb.AppendLine(obj.Name + " using " + obj.Resources["memory"] / 1024 + " KB");
                    }
                }

                WriteLine(sb.ToString());
            }
            );
        }
    }
}
