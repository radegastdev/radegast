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

using System.Collections.Generic;
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

        public override void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            if (cmdArgs.Length == 0)
            {
                AttachmentInfo(WriteLine);
            }
            else switch (cmdArgs[0])
            {
                case "avatar":
                    AttachmentInfo(WriteLine);
                    break;
                case "parcel":
                    ParcelInfo(WriteLine);
                    break;
            }

        }

        public void ParcelInfo(ConsoleWriteLine WriteLine)
        {
            WriteLine("Requesting script resources information...");
            UUID currectParcel = Client.Parcels.RequestRemoteParcelID(Client.Self.SimPosition, Client.Network.CurrentSim.Handle, Client.Network.CurrentSim.ID);
            Client.Parcels.GetParcelResouces(currectParcel, true, (success, info) =>
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
                    foreach (ParcelResourcesDetail parcelresource in info.Parcels)
                    {
                        sb.AppendLine();
                        sb.AppendLine("Detailed usage for parcel " + parcelresource.Name);
                        foreach (ObjectResourcesDetail ord in parcelresource.Objects)
                        {
                            sb.AppendFormat("{0} KB - {1}", ord.Resources["memory"] / 1024, ord.Name);
                            sb.AppendLine();
                        }
                    }
                }
                WriteLine(sb.ToString());
            });
        }

        public void AttachmentInfo(ConsoleWriteLine WriteLine)
        {
            Client.Self.GetAttachmentResources((success, info) =>
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
                    sb.AppendLine($"Attached to {Utils.EnumToText(kvp.Key)} " + ":");
                    foreach (var obj in kvp.Value)
                    {
                        sb.AppendLine(obj.Name + " using " + obj.Resources["memory"] / 1024 + " KB");
                    }
                }

                WriteLine(sb.ToString());
            }
            );
        }
    }
}
