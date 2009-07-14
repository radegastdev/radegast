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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Radegast.Netcom;
using OpenMetaverse;

namespace Radegast
{
    public partial class TabsConsole : UserControl
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private ChatTextManager mainChatManger;

        public Dictionary<string, SleekTab> tabs = new Dictionary<string, SleekTab>();
        private ChatConsole chatConsole;

        private SleekTab selectedTab;
        private Form owner;

        public TabsConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(TabsConsole_Disposed);

            this.instance = instance;

            AddNetcomEvents();

            InitializeMainTab();
            InitializeChatTab();

            ApplyConfig(this.instance.Config.CurrentConfig);

            // Callbacks
            instance.Config.ConfigApplied += new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
            client.Self.OnScriptQuestion += new AgentManager.ScriptQuestionCallback(Self_OnScriptQuestion);
            client.Self.OnScriptDialog += new AgentManager.ScriptDialogCallback(Self_OnScriptDialog);
            client.Inventory.OnObjectOffered += new InventoryManager.ObjectOfferedCallback(Inventory_OnObjectOffered);
        }

        void TabsConsole_Disposed(object sender, EventArgs e)
        {
            RemoveNetcomEvents();
            instance.Config.ConfigApplied -= new EventHandler<ConfigAppliedEventArgs>(Config_ConfigApplied);
            client.Self.OnScriptQuestion -= new AgentManager.ScriptQuestionCallback(Self_OnScriptQuestion);
            client.Self.OnScriptDialog -= new AgentManager.ScriptDialogCallback(Self_OnScriptDialog);
            client.Inventory.OnObjectOffered -= new InventoryManager.ObjectOfferedCallback(Inventory_OnObjectOffered);
        }

        private void AddNetcomEvents()
        {
            netcom.ClientLoginStatus += new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected += new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            netcom.ChatReceived += new EventHandler<ChatEventArgs>(netcom_ChatReceived);
            netcom.ChatSent += new EventHandler<ChatSentEventArgs>(netcom_ChatSent);
            netcom.AlertMessageReceived += new EventHandler<AlertMessageEventArgs>(netcom_AlertMessageReceived);
            netcom.InstantMessageReceived += new EventHandler<InstantMessageEventArgs>(netcom_InstantMessageReceived);
        }

        private void RemoveNetcomEvents()
        {
            netcom.ClientLoginStatus -= new EventHandler<ClientLoginEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected -= new EventHandler<ClientDisconnectEventArgs>(netcom_ClientDisconnected);
            netcom.ChatReceived -= new EventHandler<ChatEventArgs>(netcom_ChatReceived);
            netcom.ChatSent -= new EventHandler<ChatSentEventArgs>(netcom_ChatSent);
            netcom.AlertMessageReceived -= new EventHandler<AlertMessageEventArgs>(netcom_AlertMessageReceived);
            netcom.InstantMessageReceived -= new EventHandler<InstantMessageEventArgs>(netcom_InstantMessageReceived);
        }

        bool Inventory_OnObjectOffered(InstantMessage offerDetails, AssetType type, UUID objectID, bool fromTask)
        {
            DisplayNotificationInChat(offerDetails.FromAgentName + " gave you " + offerDetails.Message);
            return true;
        }

        void Self_OnScriptDialog(string message, string objectName, UUID imageID, UUID objectID, string firstName, string lastName, int chatChannel, List<string> buttons)
        {
            instance.MainForm.AddNotification(new ntfScriptDialog(instance, message, objectName, imageID, objectID, firstName, lastName, chatChannel, buttons));
        }

        void Self_OnScriptQuestion(Simulator simulator, UUID taskID, UUID itemID, string objectName, string objectOwner, ScriptPermission questions)
        {
            instance.MainForm.AddNotification(new ntfPermissions(instance, simulator, taskID, itemID, objectName, objectOwner, questions));
        }

        private void Config_ConfigApplied(object sender, ConfigAppliedEventArgs e)
        {
            ApplyConfig(e.AppliedConfig);
        }

        private void ApplyConfig(Config config)
        {
            if (config.InterfaceStyle == 0) //System
                tstTabs.RenderMode = ToolStripRenderMode.System;
            else if (config.InterfaceStyle == 1) //Office 2003
                tstTabs.RenderMode = ToolStripRenderMode.ManagerRenderMode;
        }

        private void netcom_ClientLoginStatus(object sender, ClientLoginEventArgs e)
        {
            if (e.Status != LoginStatus.Success) return;

            InitializeFriendsTab();
            InitializeInventoryTab();
            InitializeSearchTab();

            if (selectedTab.Name == "main")
                tabs["chat"].Select();

            tabs["main"].AllowClose = true;
            tabs["main"].Close();

            client.Self.RetrieveInstantMessages();
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            DisposeSearchTab();
            DisposeInventoryTab();
            DisposeFriendsTab();

            tabs["chat"].Select();
            DisplayNotificationInChat("Logged out.");
            
        }

        private void netcom_ClientDisconnected(object sender, ClientDisconnectEventArgs e)
        {
            if (e.Type == NetworkManager.DisconnectType.ClientInitiated) return;

            DisposeSearchTab();
            DisposeInventoryTab();
            DisposeFriendsTab();

            tabs["chat"].Select();
            DisplayNotificationInChat("Disconnected.");
        }

        private void netcom_AlertMessageReceived(object sender, AlertMessageEventArgs e)
        {
            tabs["chat"].Highlight();
        }

        private void netcom_ChatSent(object sender, ChatSentEventArgs e)
        {
            tabs["chat"].Highlight();
        }

        private void netcom_ChatReceived(object sender, ChatEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Message)) return;

            tabs["chat"].Highlight();
        }

        private void netcom_InstantMessageReceived(object sender, InstantMessageEventArgs e)
        {
            switch (e.IM.Dialog)
            {
                case InstantMessageDialog.SessionSend:
                    if (instance.Groups.ContainsKey(e.IM.IMSessionID))
                    {
                        HandleGroupIM(e);
                    }
                    else
                    {
                        HandleConferenceIM(e);
                    }
                    break;

                case InstantMessageDialog.MessageFromAgent:
                    if (e.IM.FromAgentName == "Second Life")
                    {
                        HandleIMFromObject(e);
                    }
                    else if (e.IM.GroupIM || instance.Groups.ContainsKey(e.IM.IMSessionID))
                    {
                        HandleGroupIM(e);
                    }
                    else if (e.IM.BinaryBucket.Length > 1)
                    { // conference
                        HandleConferenceIM(e);
                    }
                    else
                    {
                        HandleIM(e);
                    }
                    break;

                case InstantMessageDialog.MessageFromObject:
                    HandleIMFromObject(e);
                    break;

                case InstantMessageDialog.StartTyping:
                    if (TabExists(e.IM.FromAgentName))
                    {
                        SleekTab tab = tabs[e.IM.FromAgentName.ToLower()];
                        if (!tab.Highlighted) tab.PartialHighlight();
                    }

                    break;

                case InstantMessageDialog.StopTyping:
                    if (TabExists(e.IM.FromAgentName))
                    {
                        SleekTab tab = tabs[e.IM.FromAgentName.ToLower()];
                        if (!tab.Highlighted) tab.Unhighlight();
                    }

                    break;

                case InstantMessageDialog.MessageBox:
                    instance.MainForm.AddNotification(new ntfGeneric(instance, e.IM.Message));
                    break;

                case InstantMessageDialog.RequestTeleport:
                    instance.MainForm.AddNotification(new ntfTeleport(instance, e.IM));
                    break;

                case InstantMessageDialog.GroupInvitation:
                    instance.MainForm.AddNotification(new ntfGroupInvitation(instance, e.IM));
                    break;

                case InstantMessageDialog.FriendshipOffered:
                    instance.MainForm.AddNotification(new ntfFriendshipOffer(instance, e.IM));
                    break;

                case InstantMessageDialog.InventoryAccepted:
                    DisplayNotificationInChat(e.IM.FromAgentName + " accepted your inventory offer.");
                    break;
            }
        }

        public void DisplayNotificationInChat(string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(delegate()
                    {
                        DisplayNotificationInChat(msg);
                    }));
                return;
            }

            ChatBufferItem line = new ChatBufferItem(
                DateTime.Now,
                msg,
                ChatBufferTextStyle.ObjectChat
            );
            try
            {
                mainChatManger.ProcessBufferItem(line, true);
                tabs["chat"].Highlight();
            }
            catch (Exception) { }
        }

        private void HandleIMFromObject(InstantMessageEventArgs e)
        {
            DisplayNotificationInChat(e.IM.FromAgentName + ": " + e.IM.Message);
        }

        private void HandleIM(InstantMessageEventArgs e)
        {
            if (TabExists(e.IM.IMSessionID.ToString()))
            {
                SleekTab tab = tabs[e.IM.IMSessionID.ToString()];
                if (!tab.Selected) tab.Highlight();
                return;
            }

            IMTabWindow imTab = AddIMTab(e);
            tabs[e.IM.IMSessionID.ToString()].Highlight();
        }

        private void HandleConferenceIM(InstantMessageEventArgs e)
        {
            if (TabExists(e.IM.IMSessionID.ToString()))
            {
                SleekTab tab = tabs[e.IM.IMSessionID.ToString()];
                if (!tab.Selected) tab.Highlight();
                return;
            }

            ConferenceIMTabWindow imTab = AddConferenceIMTab(e);
            tabs[e.IM.IMSessionID.ToString()].Highlight();
        }

        private void HandleGroupIM(InstantMessageEventArgs e)
        {
            if (TabExists(e.IM.IMSessionID.ToString())) {
                SleekTab tab = tabs[e.IM.IMSessionID.ToString()];
                if (!tab.Selected) tab.Highlight();
                return;
            }

            GroupIMTabWindow imTab = AddGroupIMTab(e);
            tabs[e.IM.IMSessionID.ToString()].Highlight();
        }

        private void InitializeMainTab()
        {
            MainConsole mainConsole = new MainConsole(instance);
            mainConsole.Dock = DockStyle.Fill;
            mainConsole.Visible = false;

            toolStripContainer1.ContentPanel.Controls.Add(mainConsole);

            SleekTab tab = AddTab("main", "Main", mainConsole);
            tab.AllowClose = false;
            tab.AllowDetach = false;
            tab.AllowMerge = false;

            mainConsole.RegisterTab(tab);
        }

        private void InitializeChatTab()
        {
            chatConsole = new ChatConsole(instance);
            mainChatManger = chatConsole.ChatManager;
            chatConsole.Dock = DockStyle.Fill;
            chatConsole.Visible = false;

            toolStripContainer1.ContentPanel.Controls.Add(chatConsole);

            SleekTab tab = AddTab("chat", "Chat", chatConsole);
            tab.AllowClose = false;
            tab.AllowDetach = false;
        }

        private void InitializeFriendsTab()
        {
            FriendsConsole friendsConsole = new FriendsConsole(instance);
            friendsConsole.Dock = DockStyle.Fill;
            friendsConsole.Visible = false;

            toolStripContainer1.ContentPanel.Controls.Add(friendsConsole);

            SleekTab tab = AddTab("friends", "Friends", friendsConsole);
            tab.AllowClose = false;
            tab.AllowDetach = false;
        }

        private void InitializeSearchTab()
        {
            SearchConsole searchConsole = new SearchConsole(instance);
            searchConsole.Dock = DockStyle.Fill;
            searchConsole.Visible = false;

            toolStripContainer1.ContentPanel.Controls.Add(searchConsole);

            SleekTab tab = AddTab("search", "Search", searchConsole);
            tab.AllowClose = false;
            tab.AllowDetach = false;
        }

        private void InitializeInventoryTab()
        {
            InventoryConsole invConsole = new InventoryConsole(instance);
            invConsole.Dock = DockStyle.Fill;
            invConsole.Visible = true;

            toolStripContainer1.ContentPanel.Controls.Add(invConsole);

            SleekTab tab = AddTab("inventory", "Inventory", invConsole);
            tab.AllowClose = false;
            tab.AllowDetach = true;
        }

        private void DisposeFriendsTab()
        {
            ForceCloseTab("friends");
        }

        private void DisposeSearchTab()
        {
            ForceCloseTab("search");
        }

        private void DisposeInventoryTab()
        {
            ForceCloseTab("inventory");
        }

        private void ForceCloseTab(string name)
        {
            if (!TabExists(name)) return;

            SleekTab tab = tabs[name];
            if (tab.Merged) SplitTab(tab);

            tab.AllowClose = true;
            tab.Close();
            tab = null;
        }

        public void AddTab(SleekTab tab)
        {
            ToolStripButton button = (ToolStripButton)tstTabs.Items.Add(tab.Label);
            button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            button.Image = null;
            button.AutoToolTip = false;
            button.Tag = tab.Name;
            button.Click += new EventHandler(TabButtonClick);
            tab.Button = button;
            tabs.Add(tab.Name, tab);
        }

        public SleekTab AddTab(string name, string label, Control control)
        {
            ToolStripButton button = (ToolStripButton)tstTabs.Items.Add(label);
            button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            button.Image = null;
            button.AutoToolTip = false;
            button.Tag = name.ToLower();
            button.Click += new EventHandler(TabButtonClick);

            SleekTab tab = new SleekTab(instance, button, control, name.ToLower(), label);
            tab.TabAttached += new EventHandler(tab_TabAttached);
            tab.TabDetached += new EventHandler(tab_TabDetached);
            tab.TabSelected += new EventHandler(tab_TabSelected);
            tab.TabClosed += new EventHandler(tab_TabClosed);
            tabs.Add(name.ToLower(), tab);

            return tab;
        }

        private void tab_TabAttached(object sender, EventArgs e)
        {
            SleekTab tab = (SleekTab)sender;
            tab.Select();
        }

        private void tab_TabDetached(object sender, EventArgs e)
        {
            SleekTab tab = (SleekTab)sender;
            frmDetachedTab form = (frmDetachedTab)tab.Owner;

            form.ReattachStrip = tstTabs;
            form.ReattachContainer = toolStripContainer1.ContentPanel;
        }

        private void tab_TabSelected(object sender, EventArgs e)
        {
            SleekTab tab = (SleekTab)sender;

            if (selectedTab != null &&
                selectedTab != tab)
            { selectedTab.Deselect(); }
            
            selectedTab = tab;

            tbtnCloseTab.Enabled = tab.AllowClose;
            owner.AcceptButton = tab.DefaultControlButton;
        }

        private void tab_TabClosed(object sender, EventArgs e)
        {
            SleekTab tab = (SleekTab)sender;
            
            tabs.Remove(tab.Name);
            tab = null;
        }

        private void TabButtonClick(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;

            SleekTab tab = tabs[button.Tag.ToString()];
            tab.Select();
        }

        public void RemoveTabEntry(SleekTab tab)
        {
            if (tstTabs.Items.Contains(tab.Button))
            {
                tstTabs.Items.Remove(tab.Button);
            }

            tab.Button.Dispose();
            tabs.Remove(tab.Name);
        }

        public void RemoveTab(string name)
        {
            if (tabs.ContainsKey(name))
            {
                tabs.Remove(name);
            }
        }

        //Used for outside classes that have a reference to TabsConsole
        public void SelectTab(string name)
        {
            tabs[name.ToLower()].Select();
        }

        public bool TabExists(string name)
        {
            return tabs.ContainsKey(name.ToLower());
        }

        public SleekTab GetTab(string name)
        {
            return tabs[name.ToLower()];
        }

        public List<SleekTab> GetOtherTabs()
        {
            List<SleekTab> otherTabs = new List<SleekTab>();

            foreach (ToolStripItem item in tstTabs.Items)
            {
                if (item.Tag == null) continue;
                if ((ToolStripButton)item == selectedTab.Button) continue;

                SleekTab tab = tabs[item.Tag.ToString()];
                if (!tab.AllowMerge) continue;
                if (tab.Merged) continue;
                
                otherTabs.Add(tab);
            }

            return otherTabs;
        }

        public IMTabWindow AddIMTab(UUID target, UUID session, string targetName)
        {
            IMTabWindow imTab = new IMTabWindow(instance, target, session, targetName);
            imTab.Dock = DockStyle.Fill;

            toolStripContainer1.ContentPanel.Controls.Add(imTab);
            SleekTab tab = AddTab(session.ToString(), "IM: " + targetName, imTab);
            SelectTab(session.ToString());
            imTab.SelectIMInput();

            return imTab;
        }

        public ConferenceIMTabWindow AddConferenceIMTab(UUID session, string name)
        {
            ConferenceIMTabWindow imTab = new ConferenceIMTabWindow(instance, session, name);
            imTab.Dock = DockStyle.Fill;

            toolStripContainer1.ContentPanel.Controls.Add(imTab);
            SleekTab tab = AddTab(session.ToString(), name, imTab);
            imTab.SelectIMInput();

            return imTab;
        }


        public ConferenceIMTabWindow AddConferenceIMTab(InstantMessageEventArgs e)
        {
            ConferenceIMTabWindow imTab = AddConferenceIMTab(e.IM.IMSessionID, Utils.BytesToString(e.IM.BinaryBucket));
            imTab.TextManager.ProcessIM(e);
            return imTab;
        }

        public GroupIMTabWindow AddGroupIMTab(UUID session, string name)
        {
            GroupIMTabWindow imTab = new GroupIMTabWindow(instance, session, name);
            imTab.Dock = DockStyle.Fill;

            toolStripContainer1.ContentPanel.Controls.Add(imTab);
            SleekTab tab = AddTab(session.ToString(), name, imTab);
            imTab.SelectIMInput();

            return imTab;
        }

        public GroupIMTabWindow AddGroupIMTab(InstantMessageEventArgs e)
        {
            GroupIMTabWindow imTab = AddGroupIMTab(e.IM.IMSessionID, Utils.BytesToString(e.IM.BinaryBucket));
            imTab.TextManager.ProcessIM(e);
            return imTab;
        }
        
        public IMTabWindow AddIMTab(InstantMessageEventArgs e)
        {
            IMTabWindow imTab = AddIMTab(e.IM.FromAgentID, e.IM.IMSessionID, e.IM.FromAgentName);
            imTab.TextManager.ProcessIM(e);
            return imTab;
        }

        public OutfitTextures AddOTTab(Avatar avatar)
        {
            OutfitTextures otTab = new OutfitTextures(instance, avatar);
            otTab.Dock = DockStyle.Fill;
            toolStripContainer1.ContentPanel.Controls.Add(otTab);
            SleekTab tab = AddTab("OT: " + avatar.Name, "OT: " + avatar.Name, otTab);
            otTab.GetTextures();
            return otTab;
        }

        public MasterTab AddMSTab(Avatar avatar)
        {
            MasterTab msTab = new MasterTab(instance, avatar);
            msTab.Dock = DockStyle.Fill;
            toolStripContainer1.ContentPanel.Controls.Add(msTab);
            SleekTab tab = AddTab("MS: " + avatar.Name, "MS: " + avatar.Name, msTab);
            // msTab.GetTextures();
            return msTab;
        }

        public AttachmentTab AddATTab(Avatar avatar)
        {
            AttachmentTab atTab = new AttachmentTab(instance, avatar);
            atTab.Dock = DockStyle.Fill;
            toolStripContainer1.ContentPanel.Controls.Add(atTab);
            SleekTab tab = AddTab("AT: " + avatar.Name, "AT: " + avatar.Name, atTab);
            return atTab;
        }

        public AnimTab AddAnimTab(Avatar avatar)
        {
            AnimTab animTab = new AnimTab(instance, avatar);
            animTab.Dock = DockStyle.Fill;
            toolStripContainer1.ContentPanel.Controls.Add(animTab);
            SleekTab tab = AddTab("Anim: " + avatar.Name, "Anim: " + avatar.Name, animTab);
            return animTab;
        }

        private void tbtnTabOptions_Click(object sender, EventArgs e)
        {
            tmnuMergeWith.Enabled = selectedTab.AllowMerge;
            tmnuDetachTab.Enabled = selectedTab.AllowDetach;

            tmnuMergeWith.DropDown.Items.Clear();

            if (!selectedTab.AllowMerge) return;
            if (!selectedTab.Merged)
            {
                tmnuMergeWith.Text = "Merge With";

                List<SleekTab> otherTabs = GetOtherTabs();

                tmnuMergeWith.Enabled = (otherTabs.Count > 0);
                if (!tmnuMergeWith.Enabled) return;

                foreach (SleekTab tab in otherTabs)
                {
                    ToolStripItem item = tmnuMergeWith.DropDown.Items.Add(tab.Label);
                    item.Tag = tab.Name;
                    item.Click += new EventHandler(MergeItemClick);
                }
            }
            else
            {
                tmnuMergeWith.Text = "Split";
                tmnuMergeWith.Click += new EventHandler(SplitClick);
            }
        }

        private void MergeItemClick(object sender, EventArgs e)
        {
            ToolStripItem item = (ToolStripItem)sender;
            SleekTab tab = tabs[item.Tag.ToString()];

            selectedTab.MergeWith(tab);

            SplitContainer container = (SplitContainer)selectedTab.Control;
            toolStripContainer1.ContentPanel.Controls.Add(container);

            selectedTab.Select();
            RemoveTabEntry(tab);

            tabs.Add(tab.Name, selectedTab);
        }

        private void SplitClick(object sender, EventArgs e)
        {
            SplitTab(selectedTab);
            selectedTab.Select();
        }

        public void SplitTab(SleekTab tab)
        {
            SleekTab otherTab = tab.Split();
            if (otherTab == null) return;

            toolStripContainer1.ContentPanel.Controls.Add(tab.Control);
            toolStripContainer1.ContentPanel.Controls.Add(otherTab.Control);

            tabs.Remove(otherTab.Name);
            AddTab(otherTab);
        }

        private void tmnuDetachTab_Click(object sender, EventArgs e)
        {
            if (!selectedTab.AllowDetach) return;

            tstTabs.Items.Remove(selectedTab.Button);
            selectedTab.Detach(instance);
            selectedTab.Owner.Show();

            tabs["chat"].Select();
        }

        private void tbtnCloseTab_Click(object sender, EventArgs e)
        {
            SleekTab tab = selectedTab;

            tabs["chat"].Select();
            tab.Close();
            
            tab = null;
        }

        private void TabsConsole_Load(object sender, EventArgs e)
        {
            owner = this.FindForm();
        }

        private void ctxTabs_Opening(object sender, CancelEventArgs e)
        {
            Point pt = this.PointToClient(Cursor.Position);
            ToolStripItem stripItem = tstTabs.GetItemAt(pt);

            if (stripItem == null)
            {
                e.Cancel = true;
            }
            else
            {
                tabs[stripItem.Tag.ToString()].Select();

                ctxBtnClose.Enabled = selectedTab.AllowClose;
                ctxBtnDetach.Enabled = selectedTab.AllowDetach;
                ctxBtnMerge.Enabled = selectedTab.AllowMerge;
                ctxBtnMerge.DropDown.Items.Clear();

                if (!ctxBtnClose.Enabled && !ctxBtnDetach.Enabled && !ctxBtnMerge.Enabled)
                {
                    e.Cancel = true;
                    return;
                }

                if (!selectedTab.AllowMerge) return;
                if (!selectedTab.Merged)
                {
                    ctxBtnMerge.Text = "Merge With";

                    List<SleekTab> otherTabs = GetOtherTabs();

                    ctxBtnMerge.Enabled = (otherTabs.Count > 0);
                    if (!ctxBtnMerge.Enabled) return;

                    foreach (SleekTab tab in otherTabs)
                    {
                        ToolStripItem item = ctxBtnMerge.DropDown.Items.Add(tab.Label);
                        item.Tag = tab.Name;
                        item.Click += new EventHandler(MergeItemClick);
                    }
                }
                else
                {
                    ctxBtnMerge.Text = "Split";
                    ctxBtnMerge.Click += new EventHandler(SplitClick);
                }

            }
        }
    }
}
