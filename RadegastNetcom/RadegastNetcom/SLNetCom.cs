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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Timers;
using OpenMetaverse;
using OpenMetaverse.Packets;

namespace RadegastNc
{
	/// <summary>
	/// RadegastNetcom is a class built on top of libsecondlife that provides a way to
    /// raise events on the proper thread (for GUI apps especially).
	/// </summary>
	public partial class RadegastNetcom
	{
		private GridClient client;
        private LoginOptions loginOptions;

        private bool loggingIn = false;
        private bool loggedIn = false;        
        private bool teleporting = false;

        private const string MainGridLogin = @"https://login.agni.lindenlab.com/cgi-bin/login.cgi";
        private const string BetaGridLogin = @"https://login.aditi.lindenlab.com/cgi-bin/login.cgi";

        // NetcomSync is used for raising certain events on the
        // GUI/main thread. Useful if you're modifying GUI controls
        // in the client app when responding to those events.
        private ISynchronizeInvoke netcomSync;

        public RadegastNetcom(GridClient client)
        {
            this.client = client;
            loginOptions = new LoginOptions();
            
            AddClientEvents();
            AddPacketCallbacks();
        }

        private void AddClientEvents()
        {
            client.Self.OnChat += new AgentManager.ChatCallback(Self_OnChat);
            client.Self.OnInstantMessage += new AgentManager.InstantMessageCallback(Self_OnInstantMessage);
            client.Self.OnBalanceUpdated += new AgentManager.BalanceCallback(Avatar_OnBalanceUpdated);
            client.Self.OnTeleport += new AgentManager.TeleportCallback(Self_OnTeleport);
            client.Network.OnConnected += new NetworkManager.ConnectedCallback(Network_OnConnected);
            client.Network.OnDisconnected += new NetworkManager.DisconnectedCallback(Network_OnDisconnected);
            client.Network.OnLogin += new NetworkManager.LoginCallback(Network_OnLogin);
            client.Network.OnLogoutReply += new NetworkManager.LogoutCallback(Network_OnLogoutReply);
            client.Network.OnCurrentSimChanged += new NetworkManager.CurrentSimChangedCallback(Network_OnCurrentSimChanged);
        }

        void Network_OnCurrentSimChanged(Simulator PreviousSimulator)
        {
            // client.Appearance.SetPreviousAppearance(true);
        }

        private void Self_OnInstantMessage(InstantMessage im, Simulator simulator)
        {
            InstantMessageEventArgs ea = new InstantMessageEventArgs(im, simulator);

            if (netcomSync != null)
                netcomSync.BeginInvoke(new OnInstantMessageRaise(OnInstantMessageReceived), new object[] { ea });
            else
                OnInstantMessageReceived(ea);
        }

        private void Network_OnLogin(LoginStatus login, string message)
        {
            if (login == LoginStatus.Success)
                loggedIn = true;

            ClientLoginEventArgs ea = new ClientLoginEventArgs(login, message);

            if (netcomSync != null)
                netcomSync.BeginInvoke(new OnClientLoginRaise(OnClientLoginStatus), new object[] { ea });
            else
                OnClientLoginStatus(ea);
        }

        private void Network_OnLogoutReply(List<UUID> inventoryItems)
        {
            loggedIn = false;

            if (netcomSync != null)
                netcomSync.BeginInvoke(new OnClientLogoutRaise(OnClientLoggedOut), new object[] { EventArgs.Empty });
            else
                OnClientLoggedOut(EventArgs.Empty);
        }

        private void Self_OnTeleport(string message, TeleportStatus status, TeleportFlags flags)
        {
            if (status == TeleportStatus.Finished || status == TeleportStatus.Failed)
                teleporting = false;

            TeleportStatusEventArgs ea = new TeleportStatusEventArgs(message, status, flags);

            if (netcomSync != null)
                netcomSync.BeginInvoke(new OnTeleportStatusRaise(OnTeleportStatusChanged), new object[] { ea });
            else
                OnTeleportStatusChanged(ea);
        }

        private void Network_OnConnected(object sender)
        {
            client.Self.RequestBalance();
        }

        private void Self_OnChat(string message, ChatAudibleLevel audible, ChatType type, ChatSourceType sourceType, string fromName, UUID id, UUID ownerid, Vector3 position)
        {
            ChatEventArgs ea = new ChatEventArgs(message, audible, type, sourceType, fromName, id, ownerid, position);

            if (netcomSync != null)
                netcomSync.BeginInvoke(new OnChatRaise(OnChatReceived), new object[] { ea });
            else
                OnChatReceived(ea);
        }

        private void AddPacketCallbacks()
        {
            client.Network.RegisterCallback(PacketType.AlertMessage, new NetworkManager.PacketCallback(AlertMessageHandler));
        }

        private void Network_OnDisconnected(NetworkManager.DisconnectType type, string message)
        {
            if (!loggedIn) return;
            loggedIn = false;

            ClientDisconnectEventArgs ea = new ClientDisconnectEventArgs(type, message);

            if (netcomSync != null)
                netcomSync.BeginInvoke(new OnClientDisconnectRaise(OnClientDisconnected), new object[] { ea });
            else
                OnClientDisconnected(ea);
        }

        private void Avatar_OnBalanceUpdated(int balance)
        {
            MoneyBalanceEventArgs ea = new MoneyBalanceEventArgs(balance);

            if (netcomSync != null)
                netcomSync.BeginInvoke(new OnMoneyBalanceRaise(OnMoneyBalanceUpdated), new object[] { ea });
            else
                OnMoneyBalanceUpdated(ea);
        }

        private void AlertMessageHandler(Packet packet, Simulator simulator)
        {
            if (packet.Type != PacketType.AlertMessage) return;
            AlertMessagePacket alertPacket = (AlertMessagePacket)packet;

            AlertMessageEventArgs ea = new AlertMessageEventArgs(
                Utils.BytesToString(alertPacket.AlertData.Message));

            if (netcomSync != null)
                netcomSync.BeginInvoke(new OnAlertMessageRaise(OnAlertMessageReceived), new object[] { ea });
            else
                OnAlertMessageReceived(ea);
        }
		
		public void Login()
		{
            loggingIn = true;

            OverrideEventArgs ea = new OverrideEventArgs();
            OnClientLoggingIn(ea);

            if (ea.Cancel)
            {
                loggingIn = false;
                return;
            }

            if (string.IsNullOrEmpty(loginOptions.FirstName) ||
                string.IsNullOrEmpty(loginOptions.LastName) ||
                string.IsNullOrEmpty(loginOptions.Password))
            {
                OnClientLoginStatus(
                    new ClientLoginEventArgs(LoginStatus.Failed, "One or more fields are blank."));
            }

            string startLocation = string.Empty;

            switch (loginOptions.StartLocation)
            {
                case StartLocationType.Home: startLocation = "home"; break;
                case StartLocationType.Last: startLocation = "last"; break;
                
                case StartLocationType.Custom:
                    startLocation = loginOptions.StartLocationCustom.Trim();

                    StartLocationParser parser = new StartLocationParser(startLocation);
                    startLocation = NetworkManager.StartLocation(parser.Sim, parser.X, parser.Y, parser.Z);

                    break;
            }

            string password;

            if (loginOptions.IsPasswordMD5)
                password = loginOptions.Password;
            else
                password = Utils.MD5(loginOptions.Password);

            LoginParams loginParams = client.Network.DefaultLoginParams(
                loginOptions.FirstName, loginOptions.LastName, password,
                loginOptions.UserAgent, loginOptions.Author);

            loginParams.Start = startLocation;

            switch (loginOptions.Grid)
            {
                case LoginGrid.MainGrid: loginParams.URI = MainGridLogin; break;
                case LoginGrid.BetaGrid: loginParams.URI = BetaGridLogin; break;
                case LoginGrid.Custom: loginParams.URI = loginOptions.GridCustomLoginUri; break;
            }
            
            client.Network.BeginLogin(loginParams);
		}
		
		public void Logout()
		{
            if (!loggedIn)
            {
                OnClientLoggedOut(EventArgs.Empty);
                return;
            }

            OverrideEventArgs ea = new OverrideEventArgs();
            OnClientLoggingOut(ea);
            if (ea.Cancel) return;

            client.Network.Logout();
		}
		
		public void ChatOut(string chat, ChatType type, int channel)
		{
            if (!loggedIn) return;

            client.Self.Chat(chat, channel, type);
            OnChatSent(new ChatSentEventArgs(chat, type, channel));
		}

        public void SendInstantMessage(string message, UUID target, UUID session)
        {
            if (!loggedIn) return;
            
            //client.Self.InstantMessage(target, message, session);

            client.Self.InstantMessage(
                loginOptions.FullName, target, message, session, InstantMessageDialog.MessageFromAgent,
                InstantMessageOnline.Online, client.Self.SimPosition, client.Network.CurrentSim.ID, null);
            
            OnInstantMessageSent(new InstantMessageSentEventArgs(message, target, session, DateTime.Now));
        }

        public void SendIMStartTyping(UUID target, UUID session)
        {
            if (!loggedIn) return;

            client.Self.InstantMessage(
                loginOptions.FullName, target, "typing", session, InstantMessageDialog.StartTyping,
                InstantMessageOnline.Online, client.Self.SimPosition, client.Network.CurrentSim.ID, null);
        }

        public void SendIMStopTyping(UUID target, UUID session)
        {
            if (!loggedIn) return;

            client.Self.InstantMessage(
                loginOptions.FullName, target, "typing", session, InstantMessageDialog.StopTyping,
                InstantMessageOnline.Online, client.Self.SimPosition, client.Network.CurrentSim.ID, null);
        }

        public void Teleport(string sim, Vector3 coordinates)
        {
            if (!loggedIn) return;
            if (teleporting) return;

            TeleportingEventArgs ea = new TeleportingEventArgs(sim, coordinates);
            OnTeleporting(ea);
            if (ea.Cancel) return;

            teleporting = true;
            client.Self.Teleport(sim, coordinates);
        }

        public bool IsLoggingIn
        {
            get { return loggingIn; }
        }

        public bool IsLoggedIn
        {
            get { return loggedIn; }
        }

        public bool IsTeleporting
        {
            get { return teleporting; }
        }

        public LoginOptions LoginOptions
        {
            get { return loginOptions; }
            set { loginOptions = value; }
        }

        public ISynchronizeInvoke NetcomSync
        {
            get { return netcomSync; }
            set { netcomSync = value; }
        }
	}
}
