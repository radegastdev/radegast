using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenMetaverse;
using RadegastNc;
using System.Diagnostics;
using OpenMetaverse.Packets;

namespace Radegast
{
    public partial class SearchConsole : UserControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;

        private TabsConsole tabConsole;
        private FindPeopleConsole console;

        private string lastQuery = string.Empty;
        private int startResult = 0;

        private int totalResults = 0;

        public SearchConsole(RadegastInstance instance)
        {
            InitializeComponent();

            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;
            AddClientEvents();

            tabConsole = this.instance.TabConsole;

            console = new FindPeopleConsole(instance, UUID.Random());
            console.Dock = DockStyle.Fill;
            console.SelectedIndexChanged += new EventHandler(console_SelectedIndexChanged);
            client.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            pnlFindPeople.Controls.Add(console);
        }

        private void console_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnNewIM.Enabled = btnProfile.Enabled = btnLocate.Enabled = (console.SelectedName != null);
        }

        private void AddClientEvents()
        {
            client.Directory.OnDirPeopleReply += new DirectoryManager.DirPeopleReplyCallback(Directory_OnDirPeopleReply);
        }

        //Separate thread
        private void Directory_OnDirPeopleReply(UUID queryID, List<DirectoryManager.AgentSearchData> matchedPeople)
        {
            BeginInvoke(new DirectoryManager.DirPeopleReplyCallback(PeopleReply), new object[] { queryID, matchedPeople });
        }

        //UI thread
        private void PeopleReply(UUID queryID, List<DirectoryManager.AgentSearchData> matchedPeople)
        {
            if (console.QueryID != queryID) return;

            totalResults += matchedPeople.Count;
            lblResultCount.Text = totalResults.ToString() + " people found";

            txtPersonName.Enabled = true;
            btnFind.Enabled = true;

            btnNext.Enabled = (totalResults > 100);
            btnPrevious.Enabled = (startResult > 0);
        }

        private void txtPersonName_TextChanged(object sender, EventArgs e)
        {
            btnFind.Enabled = (txtPersonName.Text.Trim().Length > 2);
        }

        private void btnLocate_Click(object sender, EventArgs e)
        {
            client.Inventory.GiveItem(UUID.Zero, "", AssetType.Unknown, console.SelectedAgentUUID, false);
        }

        private string getHttp(string url)
        {
            // used to build entire input
            StringBuilder sb = new StringBuilder();

            // used on each read operation
            byte[] buf = new byte[8192];

            // prepare the web page we will be asking for
            HttpWebRequest request = (HttpWebRequest)
                WebRequest.Create(url);

            // execute the request
            HttpWebResponse response = (HttpWebResponse)
                request.GetResponse();

            // we will read data via the response stream
            Stream resStream = response.GetResponseStream();

            string tempString = null;
            int count = 0;

            do {
                // fill the buffer with data
                count = resStream.Read(buf, 0, buf.Length);

                // make sure we read some data
                if (count != 0) {
                    // translate from bytes to ASCII text
                    tempString = Encoding.UTF8.GetString(buf, 0, count);

                    // continue building the string
                    sb.Append(tempString);
                }
            }
            while (count > 0); // any more data to read?
            return sb.ToString();
        }

        /// <summary>
        /// Blocking <:O
        /// </summary>
        /// <param name="regionId"></param>
        /// <returns></returns>
        public string GetRegionName(UUID regionId)
        {
            RegionHandleRequestPacket handleRequest = new RegionHandleRequestPacket();
            handleRequest.Header.Reliable = true;
            handleRequest.RequestBlock = new RegionHandleRequestPacket.RequestBlockBlock();
            handleRequest.RequestBlock.RegionID = regionId;

            ulong handle = 0;
            ManualResetEvent evt = new ManualResetEvent(false);

            NetworkManager.PacketCallback handleReplyCallback = delegate(Packet packet, Simulator sender)
            {
                RegionIDAndHandleReplyPacket handleReply = (RegionIDAndHandleReplyPacket)packet;
                if (handleReply.ReplyBlock.RegionID == regionId) {
                    handle = handleReply.ReplyBlock.RegionHandle;
                    evt.Set();
                }
            };

            client.Network.RegisterCallback(PacketType.RegionIDAndHandleReply, handleReplyCallback);
            client.Network.SendPacket(handleRequest);

            bool ok = evt.WaitOne(10000, false);
            client.Network.UnregisterCallback(PacketType.RegionIDAndHandleReply, handleReplyCallback);
            if (!ok)
                return null;

            ushort X = (ushort)(handle >> 40);
            ushort Y = (ushort)((handle & 0xFFFFFFFF) >> 8);

            MapBlockRequestPacket mapRequest = new MapBlockRequestPacket();
            mapRequest.Header.Reliable = true;
            mapRequest.AgentData = new MapBlockRequestPacket.AgentDataBlock();
            mapRequest.AgentData.AgentID = client.Self.AgentID;
            mapRequest.AgentData.SessionID = client.Self.SessionID;
            mapRequest.AgentData.Flags = 0;
            mapRequest.AgentData.Godlike = false;
            mapRequest.PositionData = new MapBlockRequestPacket.PositionDataBlock();
            mapRequest.PositionData.MinX = X;
            mapRequest.PositionData.MaxX = X;
            mapRequest.PositionData.MinY = Y;
            mapRequest.PositionData.MaxY = Y;

            string name = null;
            evt.Reset();

            NetworkManager.PacketCallback mapReplyCallback = delegate(Packet packet, Simulator sender)
            {
                MapBlockReplyPacket mapReply = (MapBlockReplyPacket)packet;
                foreach (MapBlockReplyPacket.DataBlock block in mapReply.Data) {
                    if ((block.X == X) && (block.Y == Y)) {
                        name = Utils.BytesToString(block.Name);
                        evt.Set();
                    }
                }
            };

            client.Network.RegisterCallback(PacketType.MapBlockReply, mapReplyCallback);
            client.Network.SendPacket(mapRequest);

            ok = evt.WaitOne(10000, false);
            client.Network.UnregisterCallback(PacketType.MapBlockReply, mapReplyCallback);
            if (!ok)
                return null;

            return name;
        }

        void Self_OnInstantMessage(InstantMessage im, Simulator simulator)
        {
            if (InvokeRequired) {
                Invoke(new MethodInvoker(delegate()
                {
                    Self_OnInstantMessage(im, simulator);
                }));
            }

            if (im.Dialog == InstantMessageDialog.InventoryDeclined) {
                try {
                    /*
                    string reginfo = "";
                    reginfo = getHttp("http://world.GridClient.com/region/" + im.RegionID);
                    Regex r = new Regex("GridClient:///app/teleport/([^/]*)");
                    Match m = r.Match(reginfo);
                    */
                    string url = "GridClient:///" + GetRegionName(im.RegionID) + "/" + ((int)im.Position.X) + "/" + ((int)im.Position.Y) + "/" + ((int)im.Position.Z);
                    btnLink.Text = im.FromAgentName + " is at " + url;
                    btnLink.Tag = url;
                    btnLink.Visible = true;
                } catch (Exception ex) {
                    System.Console.WriteLine(ex.Message);
                }
                

            }
        }

        private void btnNewIM_Click(object sender, EventArgs e)
        {
            if (tabConsole.TabExists((client.Self.AgentID ^ console.SelectedAgentUUID).ToString()))
            {
                tabConsole.SelectTab((client.Self.AgentID ^ console.SelectedAgentUUID).ToString());
                return;
            }

            tabConsole.AddIMTab(console.SelectedAgentUUID, client.Self.AgentID ^ console.SelectedAgentUUID, console.SelectedName);
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            lastQuery = txtPersonName.Text;
            startResult = 0;
            StartFinding();
        }

        private void txtPersonName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;
            if (txtPersonName.Text.Trim().Length < 3) return;

            lastQuery = txtPersonName.Text;
            startResult = 0;
            StartFinding();
        }

        private void StartFinding()
        {
            totalResults = 0;
            lblResultCount.Text = "Searching for " + lastQuery;

            txtPersonName.Enabled = false;
            btnFind.Enabled = false;
            btnNewIM.Enabled = false;
            btnProfile.Enabled = false;
            btnPrevious.Enabled = false;
            btnNext.Enabled = false;

            console.ClearResults();
            console.QueryID = client.Directory.StartPeopleSearch(
                DirectoryManager.DirFindFlags.NameSort |
                DirectoryManager.DirFindFlags.SortAsc |
                DirectoryManager.DirFindFlags.People,
                lastQuery, startResult);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            startResult += 100;
            StartFinding();
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            startResult -= 100;
            StartFinding();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            (new frmProfile(instance, console.SelectedName, console.SelectedAgentUUID)).Show();
        }

        private void txtPersonName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
        }

        private void btnLink_Click(object sender, EventArgs e)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(((Button)sender).Tag.ToString());
            Process.Start(sInfo);
        }

    }
}
