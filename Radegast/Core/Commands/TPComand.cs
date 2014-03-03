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
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using OpenMetaverse;

namespace Radegast.Commands
{
    public class TPCommand : RadegastCommand
    {
        TabsConsole TC { get { return Instance.TabConsole; } }
        public static string FolderName = "Radegast Landmarks";
        Inventory Inv { get { return Client.Inventory.Store; } }
        ConsoleWriteLine wl;

        public TPCommand(RadegastInstance instance)
            : base(instance)
        {
            Name = "tp";
			Description = "Teleport to a named landmark from the \"" + FolderName + "\" inventory folder";
            Usage = "tp (list|landmark name|landmark number|help) (type \"" + CommandsManager.CmdPrefix + "tp help\" for full usage)";
            
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        void PrintUsage()
        {
            wl("Wrong arguments for \"tp\" command. For detailed description type: " + CommandsManager.CmdPrefix + "tp help");
        }

        void PrintFullUsage()
        {
            wl(@"Usage:
{0}tp (list|landmark name|landmark number|help)

List mode:
Lists landmarks in ""{1}"" folder
Example:
{0}tp list

Landmark name mode:
Specifies name of the landmark to teleport to
Example:
{0}tp Latif's place -- teleports to a landmark whose name begins with ""Latif's place""

Landmark number mode:
Teleport to landmark number specified
Example:
{0}tp 3 -- teleports to landmark number 3 as printed in {0}tp list output", CommandsManager.CmdPrefix, FolderName);
        }

        public override void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            wl = WriteLine;
            if (cmdArgs.Length == 0) { PrintUsage(); return; }

            if (cmdArgs[0] == "help")
            {
                PrintFullUsage();
                return;
            }

            InventoryFolder folder = (InventoryFolder)Inv.GetContents(Inv.RootFolder).Find((InventoryBase b) => { return b.Name == FolderName && b is InventoryFolder; });
            if (folder == null)
            {
                Client.Inventory.CreateFolder(Inv.RootFolder.UUID, FolderName);
                WriteLine(@"Could not find ""{0}"" folder in the inventory, creating one.", FolderName);
                return;
            }

            List<InventoryLandmark> landmarks = new List<InventoryLandmark>();
            Inv.GetContents(folder)
                .FindAll((InventoryBase b) => { return b is InventoryLandmark; })
                .ForEach((InventoryBase l) => { landmarks.Add((InventoryLandmark)l); });

            if (landmarks.Count == 0)
            {
                WriteLine(@"""{0}"" folder is empty, nothing to do.", FolderName);
                return;
            }

            string cmd = string.Join(" ", cmdArgs);

            landmarks.Sort(new LMSorter());

            if (cmd == "list")
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Landmarks:");
                for (int i = 0; i < landmarks.Count; i++)
                {
                    sb.AppendLine(string.Format("{0}. {1}", i + 1, landmarks[i].Name));
                }
                WriteLine(sb.ToString());
                return;
            }

            int lmnr;
            if (int.TryParse(cmd, out lmnr))
            {
                if (lmnr >= 1 && lmnr <= landmarks.Count)
                {
                    WriteLine("Teleporting to {0}", landmarks[lmnr - 1].Name);
                    Client.Self.RequestTeleport(landmarks[lmnr - 1].AssetUUID);
                }
                else
                {
                    WriteLine("Valid landmark number is between 1 and {0}", landmarks.Count);
                }
                return;
            }

            InventoryLandmark lm = landmarks.Find((InventoryLandmark l) => { return l.Name.ToLower().StartsWith(cmd.ToLower()); });
            if (lm == null)
            {
                WriteLine("Could not find landmark {0}, try {1}tp list", cmd, CommandsManager.CmdPrefix);
                return;
            }
            else
            {
                WriteLine("Teleporting to {0}", lm.Name);
                Client.Self.RequestTeleport(lm.AssetUUID);
                return;
            }
        }

        class LMSorter : IComparer<InventoryLandmark>
        {
            public int Compare(InventoryLandmark lm1, InventoryLandmark lm2)
            {
                return DateTime.Compare(lm1.CreationDate, lm2.CreationDate);
            }
        }
    }
}
