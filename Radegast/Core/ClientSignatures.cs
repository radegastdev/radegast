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
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public class ClientSignatures
    {
        public static Dictionary<UUID, string> Signatures { get; }

        static ClientSignatures()
        {
            Signatures = new Dictionary<UUID, string>();

            try
            {
                OSDMap clients = (OSDMap)OSDParser.DeserializeLLSDXml(Properties.Resources.client_signatures);

                foreach (KeyValuePair<string, OSD> kvp in clients)
                {
                    UUID sig;
                    
                    if (UUID.TryParse(kvp.Key, out sig))
                    {
                        if (kvp.Value.Type == OSDType.Map)
                        {
                            OSDMap client = (OSDMap)kvp.Value;
                            if (client.ContainsKey("name"))
                            {
                                Signatures.Add(sig, client["name"].AsString());
                            }
                        }
                    }
                }
            }
            catch 
            {
                Logger.Log("Failed to parse client signatures.", Helpers.LogLevel.Warning);
            }
        }
    }
}
