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
using System.Windows.Forms;
using Radegast.Netcom;
using OpenMetaverse;

namespace Radegast
{
    public partial class TabsConsole : UserControl
    {

        /// <summary>
        /// List of nearby avatars (radar data)
        /// </summary>
        public List<NearbyAvatar> NearbyAvatars
        {
            get
            {
                List<NearbyAvatar> res = new List<NearbyAvatar>();

                if (TabExists("chat"))
                {
                    ChatConsole c = (ChatConsole)Tabs["chat"].Control;
                    lock (c.agentSimHandle)
                    {
                        foreach (ListViewItem item in c.lvwObjects.Items)
                        {
                            if (item.Name != client.Self.AgentID.ToString())
                            {
                                res.Add(new NearbyAvatar() { ID = new UUID(item.Name), Name = item.Text });
                            }
                        }
                    }
                }

                return res;
            }
        }

        /// <summary>
        /// Delegate invoked on tab operations
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public delegate void TabCallback(object sender, TabEventArgs e);

        /// <summary>
        /// Fired when a tab is selected
        /// </summary>
        public event TabCallback OnTabSelected;


        /// <summary>
        /// Delegate invoked when chat notification is printed
        /// </summary>
        /// <param name="sender">Event sender</param>
        /// <param name="e">Event arguments</param>
        public delegate void ChatNotificationCallback(object sender, ChatNotificationEventArgs e);

        /// <summary>
        /// Fired when a tab is selected
        /// </summary>
        public event ChatNotificationCallback OnChatNotification;

        /// <summary>
        /// Fired when a new tab is added
        /// </summary>
        public event TabCallback OnTabAdded;

        /// <summary>
        /// Fired when a tab is removed
        /// </summary>
        public event TabCallback OnTabRemoved;

        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        private RadegastNetcom netcom { get { return instance.Netcom; } }
        private ChatTextManager mainChatManger;

        private Dictionary<string, RadegastTab> tabs = new Dictionary<string, RadegastTab>();
        public Dictionary<string, RadegastTab> Tabs { get { return tabs; } }

        private ChatConsole chatConsole;

        private RadegastTab selectedTab;

        /// <summary>
        /// Currently selected tab
        /// </summary>
        public RadegastTab SelectedTab
        {
            get
            {
                return selectedTab;
            }
        }

        private Form owner;

        public TabsConsole(RadegastInstance instance)
        {
            InitializeComponent();
            Disposed += new EventHandler(TabsConsole_Disposed);

            this.instance = instance;
            this.instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);

            AddNetcomEvents();

            InitializeMainTab();
            InitializeChatTab();

            // Callbacks
            RegisterClientEvents(client);
        }

        private void RegisterClientEvents(GridClient client)
        {
            client.Self.ScriptQuestion += new EventHandler<ScriptQuestionEventArgs>(Self_ScriptQuestion);
            client.Self.ScriptDialog += new EventHandler<ScriptDialogEventArgs>(Self_ScriptDialog);
        }

        private void UnregisterClientEvents(GridClient client)
        {
            client.Self.ScriptQuestion -= new EventHandler<ScriptQuestionEventArgs>(Self_ScriptQuestion);
            client.Self.ScriptDialog -= new EventHandler<ScriptDialogEventArgs>(Self_ScriptDialog);
        }

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(client);
        }

        void TabsConsole_Disposed(object sender, EventArgs e)
        {
            RemoveNetcomEvents();
            UnregisterClientEvents(client);
        }

        private void AddNetcomEvents()
        {
            netcom.ClientLoginStatus += new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut += new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected += new EventHandler<DisconnectedEventArgs>(netcom_ClientDisconnected);
            netcom.ChatReceived += new EventHandler<ChatEventArgs>(netcom_ChatReceived);
            netcom.ChatSent += new EventHandler<ChatSentEventArgs>(netcom_ChatSent);
            netcom.AlertMessageReceived += new EventHandler<AlertMessageEventArgs>(netcom_AlertMessageReceived);
            netcom.InstantMessageReceived += new EventHandler<InstantMessageEventArgs>(netcom_InstantMessageReceived);
        }

        private void RemoveNetcomEvents()
        {
            netcom.ClientLoginStatus -= new EventHandler<LoginProgressEventArgs>(netcom_ClientLoginStatus);
            netcom.ClientLoggedOut -= new EventHandler(netcom_ClientLoggedOut);
            netcom.ClientDisconnected -= new EventHandler<DisconnectedEventArgs>(netcom_ClientDisconnected);
            netcom.ChatReceived -= new EventHandler<ChatEventArgs>(netcom_ChatReceived);
            netcom.ChatSent -= new EventHandler<ChatSentEventArgs>(netcom_ChatSent);
            netcom.AlertMessageReceived -= new EventHandler<AlertMessageEventArgs>(netcom_AlertMessageReceived);
            netcom.InstantMessageReceived -= new EventHandler<InstantMessageEventArgs>(netcom_InstantMessageReceived);
        }

        void Self_ScriptDialog(object sender, ScriptDialogEventArgs e)
        {
            instance.MainForm.AddNotification(new ntfScriptDialog(instance, e.Message, e.ObjectName, e.ImageID, e.ObjectID, e.FirstName, e.LastName, e.Channel, e.ButtonLabels));
        }

        void Self_ScriptQuestion(object sender, ScriptQuestionEventArgs e)
        {
            instance.MainForm.AddNotification(new ntfPermissions(instance, e.Simulator, e.TaskID, e.ItemID, e.ObjectName, e.ObjectOwnerName, e.Questions));
        }

        private void netcom_ClientLoginStatus(object sender, LoginProgressEventArgs e)
        {
            if (e.Status == LoginStatus.Failed)
            {
                DisplayNotificationInChat("Login error: " + e.Message, ChatBufferTextStyle.Error);
            }
            else if (e.Status == LoginStatus.Success)
            {
                DisplayNotificationInChat("Logged in as " + netcom.LoginOptions.FullName + ".", ChatBufferTextStyle.StatusDarkBlue);
                DisplayNotificationInChat("Login reply: " + e.Message, ChatBufferTextStyle.StatusDarkBlue);

                InitializeOnlineTabs();

                if (tabs.ContainsKey("login"))
                {
                    if (selectedTab.Name == "login")
                        SelectDefaultTab();
                    ForceCloseTab("login");
                }

                client.Self.RetrieveInstantMessages();
            }
        }

        private void netcom_ClientLoggedOut(object sender, EventArgs e)
        {
            DisposeOnlineTabs();

            SelectDefaultTab();
            DisplayNotificationInChat("Logged out.");

        }

        private void netcom_ClientDisconnected(object sender, DisconnectedEventArgs e)
        {
            if (e.Reason == NetworkManager.DisconnectType.ClientInitiated) return;

            DisposeOnlineTabs();
            SelectDefaultTab();
            DisplayNotificationInChat("Disconnected: " + e.Message, ChatBufferTextStyle.Error);
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
                        RadegastTab tab = tabs[e.IM.FromAgentName.ToLower()];
                        if (!tab.Highlighted) tab.PartialHighlight();
                    }

                    break;

                case InstantMessageDialog.StopTyping:
                    if (TabExists(e.IM.FromAgentName))
                    {
                        RadegastTab tab = tabs[e.IM.FromAgentName.ToLower()];
                        if (!tab.Highlighted) tab.Unhighlight();
                    }

                    break;

                case InstantMessageDialog.MessageBox:
                    instance.MainForm.AddNotification(new ntfGeneric(instance, e.IM.Message));
                    break;

                case InstantMessageDialog.RequestTeleport:
                    if (instance.RLV.AutoAcceptTP(e.IM.FromAgentID))
                    {
                        DisplayNotificationInChat("Auto accepting teleprot from " + e.IM.FromAgentName);
                        instance.Client.Self.TeleportLureRespond(e.IM.FromAgentID, true);
                    }
                    else
                    {
                        instance.MainForm.AddNotification(new ntfTeleport(instance, e.IM));
                    }
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

                case InstantMessageDialog.InventoryDeclined:
                    DisplayNotificationInChat(e.IM.FromAgentName + " declined your inventory offer.");
                    break;

                case InstantMessageDialog.GroupNotice:
                    instance.MainForm.AddNotification(new ntfGroupNotice(instance, e.IM));
                    break;

                case InstantMessageDialog.InventoryOffered:
                case InstantMessageDialog.TaskInventoryOffered:
                    instance.MainForm.AddNotification(new ntfInventoryOffer(instance, e.IM));
                    break;
            }
        }

        /// <summary>
        /// Make default tab (chat) active
        /// </summary>
        public void SelectDefaultTab()
        {
            if (TabExists("chat"))
                tabs["chat"].Select();
        }

        /// <summary>
        /// Displays notification in the main chat tab
        /// </summary>
        /// <param name="msg">Message to be printed in the chat tab</param>
        public void DisplayNotificationInChat(string msg)
        {
            DisplayNotificationInChat(msg, ChatBufferTextStyle.ObjectChat);
        }

        /// <summary>
        /// Displays notification in the main chat tab
        /// </summary>
        /// <param name="msg">Message to be printed in the chat tab</param>
        /// <param name="style">Style of the message to be printed, normal, object, etc.</param>
        public void DisplayNotificationInChat(string msg, ChatBufferTextStyle style)
        {
            DisplayNotificationInChat(msg, style, true);
        }

        /// <summary>
        /// Displays notification in the main chat tab
        /// </summary>
        /// <param name="msg">Message to be printed in the chat tab</param>
        /// <param name="style">Style of the message to be printed, normal, object, etc.</param>
        /// <param name="highlightChatTab">Highligt (and flash in taskbar) chat tab if not selected</param>
        public void DisplayNotificationInChat(string msg, ChatBufferTextStyle style, bool highlightChatTab)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new MethodInvoker(() => DisplayNotificationInChat(msg, style, highlightChatTab)));
                return;
            }

            if (style != ChatBufferTextStyle.Invisible)
            {
                ChatBufferItem line = new ChatBufferItem(
                    DateTime.Now,
                    msg,
                    style
                );

                try
                {
                    mainChatManger.ProcessBufferItem(line, true);
                    if (highlightChatTab)
                    {
                        tabs["chat"].Highlight();
                    }
                }
                catch (Exception) { }
            }

            if (OnChatNotification != null)
            {
                try { OnChatNotification(this, new ChatNotificationEventArgs(msg, style)); }
                catch { }
            }
        }

        private void HandleIMFromObject(InstantMessageEventArgs e)
        {
            DisplayNotificationInChat(e.IM.FromAgentName + ": " + e.IM.Message);
        }

        private void HandleIM(InstantMessageEventArgs e)
        {
            if (TabExists(e.IM.IMSessionID.ToString()))
            {
                RadegastTab tab = tabs[e.IM.IMSessionID.ToString()];
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
                RadegastTab tab = tabs[e.IM.IMSessionID.ToString()];
                if (!tab.Selected) tab.Highlight();
                return;
            }

            ConferenceIMTabWindow imTab = AddConferenceIMTab(e);
            tabs[e.IM.IMSessionID.ToString()].Highlight();
        }

        private void HandleGroupIM(InstantMessageEventArgs e)
        {
            if (TabExists(e.IM.IMSessionID.ToString()))
            {
                RadegastTab tab = tabs[e.IM.IMSessionID.ToString()];
                if (!tab.Selected) tab.Highlight();
                return;
            }

            GroupIMTabWindow imTab = AddGroupIMTab(e);
            tabs[e.IM.IMSessionID.ToString()].Highlight();
        }

        private void InitializeMainTab()
        {
            LoginConsole loginConsole = new LoginConsole(instance);

            RadegastTab tab = AddTab("login", "Login", loginConsole);
            tab.AllowClose = false;
            tab.AllowDetach = false;
            tab.AllowMerge = false;
            tab.AllowHide = false;

            loginConsole.RegisterTab(tab);
        }

        private void InitializeChatTab()
        {
            chatConsole = new ChatConsole(instance);
            mainChatManger = chatConsole.ChatManager;

            RadegastTab tab = AddTab("chat", "Chat", chatConsole);
            tab.AllowClose = false;
            tab.AllowDetach = false;
            tab.AllowHide = false;
        }


        /// <summary>
        /// Create Tabs that only make sense when connected
        /// </summary>
        private void InitializeOnlineTabs()
        {
            RadegastTab tab = AddTab("friends", "Friends", new FriendsConsole(instance));
            tab.AllowClose = false;
            tab.AllowDetach = true;
            tab.Visible = false;

            tab = AddTab("groups", "Groups", new GroupsConsole(instance));
            tab.AllowClose = false;
            tab.AllowDetach = true;
            tab.Visible = false;

            tab = AddTab("inventory", "Inventory", new InventoryConsole(instance));
            tab.AllowClose = false;
            tab.AllowDetach = true;
            tab.Visible = false;

            tab = AddTab("search", "Search", new SearchConsole(instance));
            tab.AllowClose = false;
            tab.AllowDetach = true;
            tab.Visible = false;

            if (!TabExists("map"))
            {
                tab = AddTab("map", "Map", new MapConsole(instance));
                tab.AllowClose = false;
                tab.AllowDetach = true;
                tab.Visible = false;
            }

            tab = AddTab("voice", "Voice", new VoiceConsole(instance));
            tab.AllowClose = false;
            tab.AllowDetach = true;
            tab.Visible = false;
        }

        /// <summary>
        /// Close tabs that make no sense when not connected
        /// </summary>
        private void DisposeOnlineTabs()
        {
            lock (tabs)
            {
                ForceCloseTab("voice");

                // Mono crashes if we try to open map for the second time
                if (!instance.MonoRuntime)
                    ForceCloseTab("map");
                else if (TabExists("map"))
                    tabs["map"].Hide();

                ForceCloseTab("search");
                ForceCloseTab("inventory");
                ForceCloseTab("groups");
                ForceCloseTab("friends");
            }
        }

        private void ForceCloseTab(string name)
        {
            if (!TabExists(name)) return;

            RadegastTab tab = tabs[name];
            if (tab.Merged) SplitTab(tab);

            tab.AllowClose = true;
            tab.Close();
            tab = null;
        }


        public void RegisterContextAction(Type libomvType, String label, EventHandler handler)
        {
            instance.ContextActionManager.RegisterContextAction(libomvType, label, handler);
        }

        public void RegisterContextAction(ContextAction handler)
        {
            instance.ContextActionManager.RegisterContextAction(handler);
        }

        public void AddTab(RadegastTab tab)
        {
            ToolStripButton button = (ToolStripButton)tstTabs.Items.Add(tab.Label);
            button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            button.Image = null;
            button.AutoToolTip = false;
            button.Tag = tab.Name;
            button.Click += new EventHandler(TabButtonClick);
            tab.Button = button;
            tabs.Add(tab.Name, tab);

            if (OnTabAdded != null)
            {
                try { OnTabAdded(this, new TabEventArgs(tab)); }
                catch (Exception) { }
            }
        }

        public RadegastTab AddTab(string name, string label, Control control)
        {
            // WORKAROUND: one should never add tab that alrady exists
            // but under some weird conditions disconnect/connect
            // fire in the wrong order
            if (TabExists(name))
            {
                Logger.Log("Force closing tab '" + name + "'", Helpers.LogLevel.Warning, client);
                ForceCloseTab(name);
            }

            control.Visible = false;
            control.Dock = DockStyle.Fill;
            toolStripContainer1.ContentPanel.Controls.Add(control);

            ToolStripButton button = (ToolStripButton)tstTabs.Items.Add(label);
            button.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            button.Image = null;
            button.AutoToolTip = false;
            button.Tag = name.ToLower();
            button.Click += new EventHandler(TabButtonClick);

            RadegastTab tab = new RadegastTab(instance, button, control, name.ToLower(), label);
            tab.TabAttached += new EventHandler(tab_TabAttached);
            tab.TabDetached += new EventHandler(tab_TabDetached);
            tab.TabSelected += new EventHandler(tab_TabSelected);
            tab.TabClosed += new EventHandler(tab_TabClosed);
            tab.TabHidden += new EventHandler(tab_TabHidden);
            tabs.Add(name.ToLower(), tab);

            if (OnTabAdded != null)
            {
                try { OnTabAdded(this, new TabEventArgs(tab)); }
                catch (Exception) { }
            }

            return tab;
        }

        private void tab_TabAttached(object sender, EventArgs e)
        {
            RadegastTab tab = (RadegastTab)sender;
            tab.Select();
        }

        private void tab_TabDetached(object sender, EventArgs e)
        {
            RadegastTab tab = (RadegastTab)sender;
            frmDetachedTab form = (frmDetachedTab)tab.Owner;

            form.ReattachStrip = tstTabs;
            form.ReattachContainer = toolStripContainer1.ContentPanel;
        }

        private void tab_TabSelected(object sender, EventArgs e)
        {
            RadegastTab tab = (RadegastTab)sender;

            if (selectedTab != null &&
                selectedTab != tab)
            { selectedTab.Deselect(); }

            selectedTab = tab;

            tbtnCloseTab.Enabled = tab.AllowClose || tab.AllowHide;
            owner.AcceptButton = tab.DefaultControlButton;

            if (OnTabSelected != null)
            {
                try { OnTabSelected(this, new TabEventArgs(selectedTab)); }
                catch (Exception) { }
            }
        }

        void tab_TabHidden(object sender, EventArgs e)
        {
            RadegastTab tab = (RadegastTab)sender;

            if (selectedTab != null && selectedTab == tab)
            {
                tab.Deselect();
                SelectDefaultTab();
            }
        }

        private void tab_TabClosed(object sender, EventArgs e)
        {
            RadegastTab tab = (RadegastTab)sender;

            if (selectedTab != null && selectedTab == tab && tab.Name != "chat")
            {
                tab.Deselect();
                SelectDefaultTab();
            }

            tabs.Remove(tab.Name);

            if (OnTabRemoved != null)
            {
                try { OnTabRemoved(this, new TabEventArgs(tab)); }
                catch (Exception) { }
            }

            tab = null;
        }

        private void TabButtonClick(object sender, EventArgs e)
        {
            ToolStripButton button = (ToolStripButton)sender;

            RadegastTab tab = tabs[button.Tag.ToString()];
            tab.Select();
        }

        public void RemoveTabEntry(RadegastTab tab)
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

        public RadegastTab GetTab(string name)
        {
            return tabs[name.ToLower()];
        }

        public List<RadegastTab> GetOtherTabs()
        {
            List<RadegastTab> otherTabs = new List<RadegastTab>();

            foreach (ToolStripItem item in tstTabs.Items)
            {
                if (item.Tag == null) continue;
                if ((ToolStripItem)item == selectedTab.Button) continue;

                RadegastTab tab = tabs[item.Tag.ToString()];
                if (!tab.AllowMerge) continue;
                if (tab.Merged) continue;

                otherTabs.Add(tab);
            }

            return otherTabs;
        }

        /// <summary>
        /// Activates the next tab
        /// </summary>
        public void SelectNextTab()
        {
            List<ToolStripItem> buttons = new List<ToolStripItem>();

            foreach (ToolStripItem item in tstTabs.Items)
            {
                if (item.Tag == null || !item.Visible) continue;

                buttons.Add(item);
            }

            int current = 0;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] == selectedTab.Button)
                {
                    current = i;
                    break;
                }
            }

            current++;

            if (current == buttons.Count)
                current = 0;

            SelectTab(tabs[buttons[current].Tag.ToString()].Name);
        }

        /// <summary>
        /// Activates the previous tab
        /// </summary>
        public void SelectPreviousTab()
        {
            List<ToolStripItem> buttons = new List<ToolStripItem>();

            foreach (ToolStripItem item in tstTabs.Items)
            {
                if (item.Tag == null || !item.Visible) continue;

                buttons.Add(item);
            }

            int current = 0;

            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] == selectedTab.Button)
                {
                    current = i;
                    break;
                }
            }

            current--;

            if (current == -1)
                current = buttons.Count - 1;

            SelectTab(tabs[buttons[current].Tag.ToString()].Name);
        }


        public IMTabWindow AddIMTab(UUID target, UUID session, string targetName)
        {
            IMTabWindow imTab = new IMTabWindow(instance, target, session, targetName);

            RadegastTab tab = AddTab(session.ToString(), "IM: " + targetName, imTab);
            imTab.SelectIMInput();
            tab.Highlight();

            return imTab;
        }

        public ConferenceIMTabWindow AddConferenceIMTab(UUID session, string name)
        {
            ConferenceIMTabWindow imTab = new ConferenceIMTabWindow(instance, session, name);

            RadegastTab tab = AddTab(session.ToString(), name, imTab);
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

            RadegastTab tab = AddTab(session.ToString(), name, imTab);
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

            RadegastTab tab = AddTab("OT: " + avatar.Name, "OT: " + avatar.Name, otTab);
            otTab.GetTextures();
            return otTab;
        }

        public MasterTab AddMSTab(Avatar avatar)
        {
            MasterTab msTab = new MasterTab(instance, avatar);

            RadegastTab tab = AddTab("MS: " + avatar.Name, "MS: " + avatar.Name, msTab);
            return msTab;
        }

        public AttachmentTab AddATTab(Avatar avatar)
        {
            AttachmentTab atTab = new AttachmentTab(instance, avatar);

            RadegastTab tab = AddTab("AT: " + avatar.Name, "AT: " + avatar.Name, atTab);
            return atTab;
        }

        public AnimTab AddAnimTab(Avatar avatar)
        {
            AnimTab animTab = new AnimTab(instance, avatar);

            RadegastTab tab = AddTab("Anim: " + avatar.Name, "Anim: " + avatar.Name, animTab);
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

                List<RadegastTab> otherTabs = GetOtherTabs();

                tmnuMergeWith.Enabled = (otherTabs.Count > 0);
                if (!tmnuMergeWith.Enabled) return;

                foreach (RadegastTab tab in otherTabs)
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
            RadegastTab tab = tabs[item.Tag.ToString()];

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

        public void SplitTab(RadegastTab tab)
        {
            RadegastTab otherTab = tab.Split();
            if (otherTab == null) return;

            toolStripContainer1.ContentPanel.Controls.Add(tab.Control);
            toolStripContainer1.ContentPanel.Controls.Add(otherTab.Control);

            tabs.Remove(otherTab.Name);
            AddTab(otherTab);
        }

        private void tmnuDetachTab_Click(object sender, EventArgs e)
        {
            if (!selectedTab.AllowDetach) return;
            RadegastTab tab = selectedTab;
            SelectDefaultTab();
            tab.Detach(instance);
        }

        private void tbtnCloseTab_Click(object sender, EventArgs e)
        {
            RadegastTab tab = selectedTab;
            if (tab.AllowClose)
                tab.Close();
            else if (tab.AllowHide)
                tab.Hide();
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

                ctxBtnClose.Enabled = selectedTab.AllowClose || selectedTab.AllowHide;
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

                    List<RadegastTab> otherTabs = GetOtherTabs();

                    ctxBtnMerge.Enabled = (otherTabs.Count > 0);
                    if (!ctxBtnMerge.Enabled) return;

                    foreach (RadegastTab tab in otherTabs)
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

    /// <summary>
    /// Arguments for tab events
    /// </summary>
    public class TabEventArgs : EventArgs
    {
        /// <summary>
        /// Tab that was manipulated in the event
        /// </summary>
        public RadegastTab Tab;

        public TabEventArgs()
            : base()
        {
        }

        public TabEventArgs(RadegastTab tab)
            : base()
        {
            Tab = tab;
        }
    }

    /// <summary>
    /// Argument for chat notification events
    /// </summary>
    public class ChatNotificationEventArgs : EventArgs
    {
        public string Message;
        public ChatBufferTextStyle Style;

        public ChatNotificationEventArgs(string message, ChatBufferTextStyle style)
        {
            Message = message;
            Style = style;
        }
    }

    /// <summary>
    /// Element of list of nearby avatars
    /// </summary>
    public class NearbyAvatar
    {
        public UUID ID { get; set; }
        public string Name { get; set; }
    }

}
