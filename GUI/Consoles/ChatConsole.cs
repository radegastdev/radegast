// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Radegast.Netcom;
using OpenMetaverse;
using AIMLbot;

namespace Radegast
{
    public partial class ChatConsole : UserControl
    {
        private RadegastInstance instance;
        private RadegastNetcom netcom;
        private GridClient client;
        private ChatTextManager chatManager;
        private TabsConsole tabConsole;
        private Avatar currentAvatar;
        private SleekMovement movement;
        private AIMLbot.Bot Alice;
        private Hashtable AliceUsers = new Hashtable();
        private Avatar.AvatarProperties myProfile;
        private Regex chatRegex = new Regex(@"^/(\d+)\s*(.*)", RegexOptions.Compiled);
        private Dictionary<uint, Avatar> avatars = new Dictionary<uint, Avatar>();
        private Dictionary<uint, bool> bots = new Dictionary<uint,bool>();

        public ChatConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(ChatConsole_Disposed);

            if (!instance.advancedDebugging)
            {
                tbtnAnim.Visible = false;
                tbtnTextures.Visible = false;

                ctxAnim.Visible = false;
                ctxTextures.Visible = false;
            }

            this.instance = instance;
            netcom = this.instance.Netcom;
            client = this.instance.Client;

            // Callbacks
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            netcom.ChatReceived += new EventHandler<ChatEventArgs>(netcom_ChatReceived);
            netcom.InstantMessageReceived += new EventHandler<InstantMessageEventArgs>(netcom_InstantMessageReceived);
            this.instance.Config.ConfigApplied += new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
            client.Grid.OnCoarseLocationUpdate += new GridManager.CoarseLocationUpdateCallback(Grid_OnCoarseLocationUpdate);
            client.Avatars.OnAvatarProperties += new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);

            movement = new SleekMovement(client);

            chatManager = new ChatTextManager(instance, new RichTextBoxPrinter(rtbChat));
            chatManager.PrintStartupMessage();

            this.instance.MainForm.Load += new EventHandler(MainForm_Load);

            SorterClass sc = new SorterClass();
            lvwObjects.ListViewItemSorter = sc;

            Alice = new AIMLbot.Bot();
            Alice.isAcceptingUserInput = false;

            try {
                Alice.loadSettings();
                AIMLbot.Utils.AIMLLoader loader = new AIMLbot.Utils.AIMLLoader(Alice);
                Alice.isAcceptingUserInput = false;
                loader.loadAIML(Alice.PathToAIML);
                Alice.isAcceptingUserInput = true;
            } catch (Exception ex) {
                System.Console.WriteLine("Failed loading ALICE: " + ex.Message);
            }

            ApplyConfig(this.instance.Config.CurrentConfig);
        }

        void ChatConsole_Disposed(object sender, EventArgs e)
        {
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
            netcom.ChatReceived -= new EventHandler<ChatEventArgs>(netcom_ChatReceived);
            netcom.InstantMessageReceived -= new EventHandler<InstantMessageEventArgs>(netcom_InstantMessageReceived);
            this.instance.Config.ConfigApplied -= new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
            client.Grid.OnCoarseLocationUpdate -= new GridManager.CoarseLocationUpdateCallback(Grid_OnCoarseLocationUpdate);
            client.Avatars.OnAvatarProperties -= new AvatarManager.AvatarPropertiesCallback(Avatars_OnAvatarProperties);
        }

        void Avatars_OnAvatarProperties(UUID avatarID, Avatar.AvatarProperties properties)
        {
            if (avatarID == client.Self.AgentID)
            {
                myProfile = properties;
                Alice.GlobalSettings.updateSetting("birthday", myProfile.BornOn);
                DateTime bd;
                if (DateTime.TryParse(myProfile.BornOn, Utils.EnUsCulture, System.Globalization.DateTimeStyles.None, out bd))
                {
                    DateTime now = DateTime.Today;
                    int age = now.Year - bd.Year;
                    if (now.Month < bd.Month || (now.Month == bd.Month && now.Day < bd.Day))
                    {
                        --age;
                    }
                    Alice.GlobalSettings.updateSetting("age", age.ToString());
                    Alice.GlobalSettings.updateSetting("birthday", bd.ToLongDateString());

                }
            }
        }

        void Grid_OnCoarseLocationUpdate(Simulator sim, List<UUID> newEntries, List<UUID> removedEntries)
        {
            if (client.Network.CurrentSim.ID != sim.ID || !sim.AvatarPositions.ContainsKey(client.Self.AgentID))
            {
                return;
            }

            if (InvokeRequired)
            {
               
                BeginInvoke(new MethodInvoker(delegate()
                {
                    Grid_OnCoarseLocationUpdate(sim, newEntries, removedEntries);
                }));
                return;
            }

            lvwObjects.BeginUpdate();
            try
            {
                Vector3 mypos = sim.AvatarPositions[client.Self.AgentID];
                List<UUID> existing = new List<UUID>();
                List<UUID> removed = new List<UUID>();

                sim.AvatarPositions.ForEach(delegate(KeyValuePair<UUID, Vector3> avi)
                {
                    existing.Add(avi.Key);
                    if (!lvwObjects.Items.ContainsKey(avi.Key.ToString()))
                    {
                        string name = instance.getAvatarName(avi.Key);
                        ListViewItem item = lvwObjects.Items.Add(avi.Key.ToString(), name, string.Empty);
                        if (avi.Key == client.Self.AgentID) item.Font = new Font(item.Font, FontStyle.Bold);
                        item.Tag = avi.Key;
                    }
                });

                foreach (ListViewItem item in lvwObjects.Items)
                {
                    UUID key = (UUID)item.Tag;
                    if (!existing.Contains(key))
                    {
                        removed.Add(key);
                        continue;
                    }
                    item.Text = instance.getAvatarName(key);
                    if (key == client.Self.AgentID)
                    {
                        continue;
                    }
                    int d = (int)Vector3.Distance(sim.AvatarPositions[key], mypos);
                    item.Text = instance.getAvatarName(key) + " (" + d + "m)";
                }

                foreach (UUID key in removed)
                {
                    lvwObjects.Items.RemoveByKey(key.ToString());
                }

                lvwObjects.Sort();
            }
            catch (Exception)
            {
            }
            lvwObjects.EndUpdate();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tabConsole = instance.TabConsole;
        }

        private void Config_ConfigApplied(object sender, ConfigAppliedEventArgs e)
        {
            ApplyConfig(e.AppliedConfig);
        }

        private void ApplyConfig(Config config)
        {
            if (config.InterfaceStyle == 0) //System
                toolStrip1.RenderMode = ToolStripRenderMode.System;
            else if (config.InterfaceStyle == 1) //Office 2003
                toolStrip1.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        }

        void netcom_InstantMessageReceived(object sender, InstantMessageEventArgs e)
        {
            if (e.IM.Dialog == InstantMessageDialog.MessageFromAgent 
                && Alice.isAcceptingUserInput
                && instance.Config.CurrentConfig.UseAlice
                && !instance.Groups.ContainsKey(e.IM.IMSessionID)
                && e.IM.BinaryBucket.Length < 2
                && e.IM.FromAgentName != "Second Life") 
            {
                Alice.GlobalSettings.updateSetting("location", "region " + client.Network.CurrentSim.Name);
                AIMLbot.User user;
                if (AliceUsers.ContainsKey(e.IM.FromAgentName)) {
                    user = (AIMLbot.User)AliceUsers[e.IM.FromAgentName];
                } else {
                    user = new User(e.IM.FromAgentName, Alice);
                    user.Predicates.removeSetting("name");
                    user.Predicates.addSetting("name", firstName(e.IM.FromAgentName));
                    AliceUsers[e.IM.FromAgentName] = user;
                }
                AIMLbot.Request req = new Request(e.IM.Message, user, Alice);
                AIMLbot.Result res = Alice.Chat(req);
                string msg = res.Output;
                if (msg.Length > 1000) {
                    msg = msg.Substring(0, 1000);
                }
                netcom.SendInstantMessage(msg, e.IM.FromAgentID, e.IM.IMSessionID);
            }
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;

            cbxInput.Enabled = true;
            btnSay.Enabled = true;
            btnShout.Enabled = true;
            client.Avatars.RequestAvatarProperties(client.Self.AgentID);
            Alice.GlobalSettings.updateSetting("name", firstName(client.Self.Name));
         }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            cbxInput.Enabled = false;
            btnSay.Enabled = false;
            btnShout.Enabled = false;

            lvwObjects.Items.Clear();
        }

        private string firstName(string name)
        {
            return name.Split(' ')[0];
        }

        private void netcom_ChatReceived(object sender, ChatEventArgs e)
        {
            if (e.SourceType != ChatSourceType.Agent) return;
            if (e.FromName == netcom.LoginOptions.FullName) return;

            if (Alice.isAcceptingUserInput && e.Message.ToLower().Contains(firstName(client.Self.Name).ToLower()) && instance.Config.CurrentConfig.UseAlice)
            {
                Alice.GlobalSettings.updateSetting("location", "region " + client.Network.CurrentSim.Name);
                string msg = e.Message.ToLower();
                msg = msg.Replace(firstName(client.Self.Name).ToLower(), "");
                AIMLbot.User user;
                if (AliceUsers.ContainsKey(e.FromName))
                {
                    user = (AIMLbot.User)AliceUsers[e.FromName];
                }
                else
                {
                    user = new User(e.FromName, Alice);
                    user.Predicates.removeSetting("name");
                    user.Predicates.addSetting("name", firstName(e.FromName));
                    AliceUsers[e.FromName] = user;
                }
                client.Self.Movement.TurnToward(e.Position);
                if (!instance.State.IsTyping)
                {
                    instance.State.SetTyping(true);
                }
                System.Threading.Thread.Sleep(1000);
                instance.State.SetTyping(false);
                AIMLbot.Request req = new Request(msg, user, Alice);
                AIMLbot.Result res = Alice.Chat(req);
                string outp = res.Output;
                if (outp.Length > 1000)
                {
                    outp = outp.Substring(0, 1000);
                }
                ProcessChatInput(outp, ChatType.Normal);
            }
        }

        private void cbxInput_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            if (e.Shift)
                ProcessChatInput(cbxInput.Text, ChatType.Whisper);
            else if (e.Control)
                ProcessChatInput(cbxInput.Text, ChatType.Shout);
            else
                ProcessChatInput(cbxInput.Text, ChatType.Normal);
        }

        private void ProcessChatInput(string input, ChatType type)
        {
            if (string.IsNullOrEmpty(input)) return;

            string msg;
            
            if (input.Length >= 1000)
            {
                msg = input.Substring(0, 1000);
            }
            else
            {
                msg = input;
            }

            int ch = 0;
            Match m = chatRegex.Match(msg);

            if (m.Groups.Count > 2)
            {
                ch = int.Parse(m.Groups[1].Value);
                msg = m.Groups[2].Value;
            }

            netcom.ChatOut(msg, type, ch);
            ClearChatInput();
        }

        private void ClearChatInput()
        {
            cbxInput.Items.Add(cbxInput.Text);
            cbxInput.Text = string.Empty;
        }

        private void btnSay_Click(object sender, EventArgs e)
        {
            ProcessChatInput(cbxInput.Text, ChatType.Normal);
        }

        private void btnShout_Click(object sender, EventArgs e)
        {
            ProcessChatInput(cbxInput.Text, ChatType.Shout);
        }

        private void cbxInput_TextChanged(object sender, EventArgs e)
        {
            if (cbxInput.Text.Length > 0)
            {
                btnSay.Enabled = btnShout.Enabled = true;

                if (!cbxInput.Text.StartsWith("/"))
                {
                    if (!instance.State.IsTyping)
                        instance.State.SetTyping(true);
                }
            }
            else
            {
                btnSay.Enabled = btnShout.Enabled = false;
                instance.State.SetTyping(false);
            }
        }

        public ChatTextManager ChatManager
        {
            get { return chatManager; }
        }

        private void tbtnStartIM_Click(object sender, EventArgs e)
        {
            if (lvwObjects.SelectedItems.Count == 0) return;
            UUID av = (UUID)lvwObjects.SelectedItems[0].Tag;
            string name = instance.getAvatarName(av);

            if (tabConsole.TabExists((client.Self.AgentID ^ av).ToString()))
            {
                tabConsole.SelectTab((client.Self.AgentID ^ av).ToString());
                return;
            }

            tabConsole.AddIMTab(av, client.Self.AgentID ^ av, name);
        }

        private void tbtnFollow_Click(object sender, EventArgs e)
        {
            Avatar av = currentAvatar;
            if (av == null) return;

            if (instance.State.FollowName != av.Name)
                instance.State.Follow(av.Name);
            else
                instance.State.Follow(string.Empty);
        }

        private void ctxPoint_Click(object sender, EventArgs e)
        {
            if (!instance.State.IsPointing)
            {
                Avatar av = currentAvatar;
                if (av == null) return;
                instance.State.SetPointing(av, 5);
                ctxPoint.Text = "Unpoint";
            }
            else
            {
                ctxPoint.Text = "Point at";
                instance.State.UnSetPointing();
            }
        }


        private void lvwObjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwObjects.SelectedItems.Count == 0)
            {
                currentAvatar = null;
                tbtnStartIM.Enabled = tbtnFollow.Enabled = tbtnProfile.Enabled = tbtnTextures.Enabled = tbtnMaster.Enabled = tbtnAttach.Enabled = tbtnAnim.Enabled = false;
                ctxPay.Enabled = ctxSource.Enabled = ctxPoint.Enabled = ctxStartIM.Enabled = ctxFollow.Enabled = ctxProfile.Enabled = ctxTextures.Enabled = ctxMaster.Enabled = ctxAttach.Enabled = ctxAnim.Enabled = false;
            }
            else
            {
                currentAvatar = client.Network.CurrentSim.ObjectsAvatars.Find(delegate(Avatar a)
                {
                    return a.ID == (UUID)lvwObjects.SelectedItems[0].Tag;
                });

                tbtnStartIM.Enabled = tbtnProfile.Enabled = true;
                tbtnFollow.Enabled = tbtnTextures.Enabled = tbtnMaster.Enabled = tbtnAttach.Enabled = tbtnAnim.Enabled = currentAvatar != null;

                ctxPay.Enabled = ctxSource.Enabled = ctxStartIM.Enabled = ctxProfile.Enabled = true;
                ctxPoint.Enabled = ctxFollow.Enabled = ctxTextures.Enabled = ctxMaster.Enabled = ctxAttach.Enabled = ctxAnim.Enabled = currentAvatar != null;

                if ((UUID)lvwObjects.SelectedItems[0].Tag == client.Self.AgentID)
                {
                    tbtnFollow.Enabled = tbtnStartIM.Enabled = false;
                    ctxPay.Enabled = ctxFollow.Enabled = ctxStartIM.Enabled = false;
                }
            }
            if (instance.State.IsPointing)
            {
                ctxPoint.Enabled = true;
            }
        }

        private void rtbChat_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            instance.MainForm.processLink(e.LinkText);
        }

        private void tbtnProfile_Click(object sender, EventArgs e)
        {
            if (lvwObjects.SelectedItems.Count == 0) return;
            UUID av = (UUID)lvwObjects.SelectedItems[0].Tag;
            string name = instance.getAvatarName(av);

            (new frmProfile(instance, name, av)).Show();
        }

        private void cbxInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) e.SuppressKeyPress = true;
        }

        private void dumpOufitBtn_Click(object sender, EventArgs e)
        {
            Avatar av = currentAvatar;
            if (av == null) return;

            if (!instance.TabConsole.TabExists("OT: " + av.Name)) {
                instance.TabConsole.AddOTTab(av);
            }
            instance.TabConsole.SelectTab("OT: " + av.Name);
        }

        private void tbtnMaster_Click(object sender, EventArgs e)
        {
            Avatar av = currentAvatar;
            if (av == null) return;

            if (!instance.TabConsole.TabExists("MS: " + av.Name))
            {
                instance.TabConsole.AddMSTab(av);
            }
            instance.TabConsole.SelectTab("MS: " + av.Name);
        }

        private void tbtnAttach_Click(object sender, EventArgs e)
        {
            Avatar av = currentAvatar;
            if (av == null) return;

            if (!instance.TabConsole.TabExists("AT: " + av.Name)) {
                instance.TabConsole.AddATTab(av);
            }
            instance.TabConsole.SelectTab("AT: " + av.Name);
        }

        private void tbtnAnim_Click(object sender, EventArgs e)
        {
            Avatar av = currentAvatar;
            if (av == null) return;

            if (!instance.TabConsole.TabExists("Anim: " + av.Name)) {
                instance.TabConsole.AddAnimTab(av);
            }
            instance.TabConsole.SelectTab("Anim: " + av.Name);


        }

        private void btnTurnLeft_MouseDown(object sender, MouseEventArgs e)
        {
            movement.TurningLeft = true;
        }

        private void btnTurnLeft_MouseUp(object sender, MouseEventArgs e)
        {
            movement.TurningLeft = false;
        }

        private void btnTurnRight_MouseDown(object sender, MouseEventArgs e)
        {
            movement.TurningRight = true;
        }

        private void btnTurnRight_MouseUp(object sender, MouseEventArgs e)
        {
            movement.TurningRight = false;
        }

        private void btnFwd_MouseDown(object sender, MouseEventArgs e)
        {
            movement.MovingForward = true;
        }

        private void btnFwd_MouseUp(object sender, MouseEventArgs e)
        {
            movement.MovingForward = false;
        }

        private void btnMoveBack_MouseDown(object sender, MouseEventArgs e)
        {
            movement.MovingBackward = true;
        }

        private void btnMoveBack_MouseUp(object sender, MouseEventArgs e)
        {
            movement.MovingBackward = false;
        }

        private void pnlMovement_Click(object sender, EventArgs e)
        {
            client.Self.Jump(true);
            System.Threading.Thread.Sleep(500);
            client.Self.Jump(false);
        }

        private void lvwObjects_DragDrop(object sender, DragEventArgs e)
        {
            Point local = lvwObjects.PointToClient(new Point(e.X, e.Y));
            ListViewItem litem = lvwObjects.GetItemAt(local.X, local.Y);
            if (litem == null) return;
            TreeNode node = e.Data.GetData(typeof(TreeNode)) as TreeNode;
            if (node == null) return;

            if (node.Tag is InventoryItem)
            {
                InventoryItem item = node.Tag as InventoryItem;
                client.Inventory.GiveItem(item.UUID, item.Name, item.AssetType, (UUID)litem.Tag, true);
                instance.TabConsole.DisplayNotificationInChat("Offered item " + item.Name + " to " + instance.getAvatarName((UUID)litem.Tag) + ".");
            }
            else if (node.Tag is InventoryFolder)
            {
                InventoryFolder folder = node.Tag as InventoryFolder;
                client.Inventory.GiveFolder(folder.UUID, folder.Name, AssetType.Folder, (UUID)litem.Tag, true);
                instance.TabConsole.DisplayNotificationInChat("Offered folder " + folder.Name + " to " + instance.getAvatarName((UUID)litem.Tag) + ".");
            }
        }

        private void lvwObjects_DragOver(object sender, DragEventArgs e)
        {
            Point local = lvwObjects.PointToClient(new Point(e.X, e.Y));
            ListViewItem litem = lvwObjects.GetItemAt(local.X, local.Y);
            if (litem == null) return;

            if (!e.Data.GetDataPresent(typeof(TreeNode))) return;

            e.Effect = DragDropEffects.Copy;
        }

        private void avatarContext_Opening(object sender, CancelEventArgs e)
        {
            if (lvwObjects.SelectedItems.Count == 0 && !instance.State.IsPointing)
            {
                e.Cancel = true;
            }
            else if (instance.State.IsPointing)
            {
                ctxPoint.Enabled = true;
                ctxPoint.Text = "Unpoint";
            }
        }

        private void ctxSource_Click(object sender, EventArgs e)
        {
            if (lvwObjects.SelectedItems.Count != 1) return;

            instance.State.EffectSource = (UUID)lvwObjects.SelectedItems[0].Tag;
        }

        private void ctxPay_Click(object sender, EventArgs e)
        {
            if (lvwObjects.SelectedItems.Count != 1) return;
            (new frmPay(instance, (UUID)lvwObjects.SelectedItems[0].Tag, instance.getAvatarName((UUID)lvwObjects.SelectedItems[0].Tag), false)).ShowDialog();
        }

    }
}

namespace System.Windows.Forms
{

    public class SorterClass : IComparer
    {

        public SorterClass()
        {

        }

        //this routine should return -1 if xy and 0 if x==y.
        // for our sample we'll just use string comparison
        public int Compare(object x, object y)
        {

            System.Windows.Forms.ListViewItem item1 = (System.Windows.Forms.ListViewItem)x;
            System.Windows.Forms.ListViewItem item2 = (System.Windows.Forms.ListViewItem)y;

            int distance1 = 0, distance2 = 0;

            int start1 = item1.Text.IndexOf('(');
            int end1 = item1.Text.IndexOf(')') - 2;
            if (start1 == -1)
                return -1;

            // data is "First Last (xyz m)"
            start1++;

            string substr1 = item1.Text.Substring(start1, end1 - start1);

            int.TryParse(substr1, out distance1);

            int start2 = item2.Text.IndexOf('(');
            int end2 = item2.Text.IndexOf(')') - 2;
            if (start2 == -1)
                return 1;

            // data is "First Last (xyz m)"
            start2++;

            string substr2 = item2.Text.Substring(start2, end2 - start2);

            int.TryParse(substr2, out distance2);

            if (distance1 < distance2)
                return -1;
            else if (distance1 > distance2)
                return 1;
            else
                return 0;

        }
    }

}
