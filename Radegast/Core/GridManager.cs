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
using System.IO;
using System.Text;
using System.Reflection;
using OpenMetaverse;
using OpenMetaverse.StructuredData;

namespace Radegast
{
    public class Grid
    {
        public string ID = string.Empty;
        public string Name = string.Empty;
        public string Platform = string.Empty;
        public string LoginURI = string.Empty;
        public string LoginPage = string.Empty;
        public string HelperURI = string.Empty;
        public string Website = string.Empty;
        public string Support = string.Empty;
        public string Register = string.Empty;
        public string PasswordURL = string.Empty;
        public string Version = "1";

        public Grid() { }

        public Grid(string ID, string name, string loginURI)
        {
            this.ID = ID;
            this.Name = name;
            this.LoginURI = loginURI;
        }

        public override string ToString()
        {
            return Name;
        }

        public static Grid FromOSD(OSD data)
        {
            if (data == null || data.Type != OSDType.Map) return null;
            Grid grid = new Grid();
            OSDMap map = (OSDMap)data;

            grid.ID = map["gridnick"].AsString();
            grid.Name = map["gridname"].AsString();
            grid.Platform = map["platform"].AsString();
            grid.LoginURI = map["loginuri"].AsString();
            grid.LoginPage = map["loginpage"].AsString();
            grid.HelperURI = map["helperuri"].AsString();
            grid.Website = map["website"].AsString();
            grid.Support = map["support"].AsString();
            grid.Register = map["register"].AsString();
            grid.PasswordURL = map["password"].AsString();
            grid.Version = map["version"].AsString();

            return grid;
        }
    }

    public class GridManager : IDisposable
    {
        public List<Grid> Grids;

        public GridManager()
        {
            Grids = new List<Grid>();
        }

        public void Dispose()
        {
        }

        public void LoadGrids()
        {
            Grids.Clear();
            Grids.Add(new Grid("agni", "Second Life (agni)", "https://login.agni.lindenlab.com/cgi-bin/login.cgi"));
            Grids.Add(new Grid("aditi", "Second Life Beta (aditi)", "https://login.aditi.lindenlab.com/cgi-bin/login.cgi"));

            try
            {
                string sysGridsFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "grids.xml");
                OSDArray sysGrids = (OSDArray)OSDParser.DeserializeLLSDXml(File.ReadAllText(sysGridsFile));
                for (int i = 0; i < sysGrids.Count; i++)
                {
                    RegisterGrid(Grid.FromOSD(sysGrids[i]));
                }
            }
            catch (Exception ex)
            {
                Logger.Log(string.Format("Error loading grid information: {0}", ex.Message), Helpers.LogLevel.Warning);
            }
        }

        public void RegisterGrid(Grid grid)
        {
            int ix = Grids.FindIndex((Grid g) => { return g.ID == grid.ID; });
            if (ix < 0)
            {
                Grids.Add(grid);
            }
            else
            {
                Grids[ix] = grid;
            }
        }

        public Grid this[int ix]
        {
            get { return Grids[ix]; }
        }

        public Grid this[string gridID]
        {
            get
            {
                foreach (Grid grid in Grids)
                {
                    if (grid.ID == gridID) return grid;
                }
                throw new KeyNotFoundException();
            }
        }

        public bool KeyExists(string gridID)
        {
            foreach (Grid grid in Grids)
            {
                if (grid.ID == gridID) return true;
            }
            return false;
        }

        public int Count
        {
            get { return Grids.Count; }
        }
    }
}
