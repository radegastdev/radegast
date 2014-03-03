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
using System.ComponentModel;
using System.Windows.Forms;
using OpenMetaverse;
using Radegast;
using OpenMetaverse.Packets;

namespace Radegast.Netcom
{
    /// <summary>
    /// RadegastNetcom is a class built on top of libsecondlife that provides a way to
    /// raise events on the proper thread (for GUI apps especially).
    /// </summary>
    public partial class RadegastNetcom : IDisposable
    {
        private RadegastInstance instance;
        private GridClient client { get { return instance.Client; } }
        public LoginOptions loginOptions;

        private bool loggingIn = false;
        private bool loggedIn = false;
        private bool teleporting = false;
        private bool agreeToTos = false;
        public bool AgreeToTos { get { return agreeToTos; } set { agreeToTos = value; } }
        private Grid grid;
        public Grid Grid { get { return grid; } }

        // NetcomSync is used for raising certain events on the
        // GUI/main thread. Useful if you're modifying GUI controls
        // in the client app when responding to those events.
        private Control netcomSync;

        #region ClientConnected event
        /// <summary>The event subscribers, null of no subscribers</summary>
        private EventHandler<EventArgs> m_ClientConnected;

        ///<summary>Raises the ClientConnected Event</summary>
        /// <param name="e">A ClientConnectedEventArgs object containing
        /// the old and the new client</param>
        protected virtual void OnClientConnected(EventArgs e)
        {
            EventHandler<EventArgs> handler = m_ClientConnected;
            if (handler != null)
                handler(this, e);
        }

        /// <summary>Thread sync lock object</summary>
        private readonly object m_ClientConnectedLock = new object();

        /// <summary>Raise event delegate</summary>
        private delegate void ClientConnectedRaise(EventArgs e);

        /// <summary>Raised when the GridClient object in the main Radegast instance is changed</summary>
        public event EventHandler<EventArgs> ClientConnected
        {
            add { lock (m_ClientConnectedLock) { m_ClientConnected += value; } }
            remove { lock (m_ClientConnectedLock) { m_ClientConnected -= value; } }
        }
        #endregion ClientConnected event

        public RadegastNetcom(RadegastInstance instance)
        {
            this.instance = instance;
            loginOptions = new LoginOptions();

            instance.ClientChanged += new EventHandler<ClientChangedEventArgs>(instance_ClientChanged);
            RegisterClientEvents(client);
        }

        private void RegisterClientEvents(GridClient client)
        {
            client.Self.ChatFromSimulator += new EventHandler<ChatEventArgs>(Self_ChatFromSimulator);
            client.Self.IM += new EventHandler<InstantMessageEventArgs>(Self_IM);
            client.Self.MoneyBalance += new EventHandler<BalanceEventArgs>(Self_MoneyBalance);
            client.Self.TeleportProgress += new EventHandler<TeleportEventArgs>(Self_TeleportProgress);
            client.Self.AlertMessage += new EventHandler<AlertMessageEventArgs>(Self_AlertMessage);
            client.Network.Disconnected += new EventHandler<DisconnectedEventArgs>(Network_Disconnected);
            client.Network.LoginProgress += new EventHandler<LoginProgressEventArgs>(Network_LoginProgress);
            client.Network.LoggedOut += new EventHandler<LoggedOutEventArgs>(Network_LoggedOut);
        }

        private void UnregisterClientEvents(GridClient client)
        {
            client.Self.ChatFromSimulator -= new EventHandler<ChatEventArgs>(Self_ChatFromSimulator);
            client.Self.IM -= new EventHandler<InstantMessageEventArgs>(Self_IM);
            client.Self.MoneyBalance -= new EventHandler<BalanceEventArgs>(Self_MoneyBalance);
            client.Self.TeleportProgress -= new EventHandler<TeleportEventArgs>(Self_TeleportProgress);
            client.Self.AlertMessage -= new EventHandler<AlertMessageEventArgs>(Self_AlertMessage);
            client.Network.Disconnected -= new EventHandler<DisconnectedEventArgs>(Network_Disconnected);
            client.Network.LoginProgress -= new EventHandler<LoginProgressEventArgs>(Network_LoginProgress);
            client.Network.LoggedOut -= new EventHandler<LoggedOutEventArgs>(Network_LoggedOut);
        }

        void instance_ClientChanged(object sender, ClientChangedEventArgs e)
        {
            UnregisterClientEvents(e.OldClient);
            RegisterClientEvents(client);
        }

        private bool CanSyncInvoke
        {
            get
            {
                return netcomSync != null && !netcomSync.IsDisposed && netcomSync.IsHandleCreated && netcomSync.InvokeRequired;
            }
        }

        public void Dispose()
        {
            if (client != null)
            {
                UnregisterClientEvents(client);
            }
        }

        void Self_IM(object sender, InstantMessageEventArgs e)
        {
            if (CanSyncInvoke)
                netcomSync.BeginInvoke(new OnInstantMessageRaise(OnInstantMessageReceived), new object[] { e });
            else
                OnInstantMessageReceived(e);
        }

        void Network_LoginProgress(object sender, LoginProgressEventArgs e)
        {
            if (e.Status == LoginStatus.Success)
            {
                loggedIn = true;
                client.Self.RequestBalance();
                if (CanSyncInvoke)
                {
                    netcomSync.BeginInvoke(new ClientConnectedRaise(OnClientConnected), new object[] { EventArgs.Empty });
                }
                else
                {
                    OnClientConnected(EventArgs.Empty);
                }
            }

            if (e.Status == LoginStatus.Failed)
            {
                instance.MarkEndExecution();
            }

            LoginProgressEventArgs ea = new LoginProgressEventArgs(e.Status, e.Message, string.Empty);

            if (CanSyncInvoke)
                netcomSync.BeginInvoke(new OnClientLoginRaise(OnClientLoginStatus), new object[] { e });
            else
                OnClientLoginStatus(e);
        }

        void Network_LoggedOut(object sender, LoggedOutEventArgs e)
        {
            loggedIn = false;

            if (CanSyncInvoke)
                netcomSync.BeginInvoke(new OnClientLogoutRaise(OnClientLoggedOut), new object[] { EventArgs.Empty });
            else
                OnClientLoggedOut(EventArgs.Empty);
        }

        void Self_TeleportProgress(object sender, TeleportEventArgs e)
        {
            if (e.Status == TeleportStatus.Finished || e.Status == TeleportStatus.Failed)
                teleporting = false;

            if (CanSyncInvoke)
                netcomSync.BeginInvoke(new OnTeleportStatusRaise(OnTeleportStatusChanged), new object[] { e });
            else
                OnTeleportStatusChanged(e);
        }

        private void Self_ChatFromSimulator(object sender, ChatEventArgs e)
        {
            if (CanSyncInvoke)
                netcomSync.BeginInvoke(new OnChatRaise(OnChatReceived), new object[] { e });
            else
                OnChatReceived(e);
        }

        void Network_Disconnected(object sender, DisconnectedEventArgs e)
        {
            loggedIn = false;
            instance.MarkEndExecution();

            if (CanSyncInvoke)
                netcomSync.BeginInvoke(new OnClientDisconnectRaise(OnClientDisconnected), new object[] { e });
            else
                OnClientDisconnected(e);
        }

        void Self_MoneyBalance(object sender, BalanceEventArgs e)
        {
            if (CanSyncInvoke)
                netcomSync.BeginInvoke(new OnMoneyBalanceRaise(OnMoneyBalanceUpdated), new object[] { e });
            else
                OnMoneyBalanceUpdated(e);
        }

        void Self_AlertMessage(object sender, AlertMessageEventArgs e)
        {
            if (CanSyncInvoke)
                netcomSync.BeginInvoke(new OnAlertMessageRaise(OnAlertMessageReceived), new object[] { e });
            else
                OnAlertMessageReceived(e);
        }

        public void Login()
        {
            loggingIn = true;

            // Report crashes only once and not on relogs/reconnects
            LastExecStatus execStatus = instance.GetLastExecStatus();
            if (!instance.AnotherInstanceRunning() && execStatus != LastExecStatus.Normal && (!instance.ReportedCrash))
            {
                instance.ReportedCrash = true;
                loginOptions.LastExecEvent = execStatus;
                Logger.Log("Reporting crash of the last application run to the grid login service", Helpers.LogLevel.Warning);
            }
            else
            {
                loginOptions.LastExecEvent = LastExecStatus.Normal;
                Logger.Log("Reporting normal shutdown of the last application run to the grid login service", Helpers.LogLevel.Info);
            }
            instance.MarkStartExecution();

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
                    new LoginProgressEventArgs(LoginStatus.Failed, "One or more fields are blank.", string.Empty));
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

            if (LoginOptions.IsPasswordMD5(loginOptions.Password))
            {
                password = loginOptions.Password;
            }
            else
            {
                if (loginOptions.Password.Length > 16)
                {
                    password = Utils.MD5(loginOptions.Password.Substring(0, 16));
                }
                else
                {
                    password = Utils.MD5(loginOptions.Password);
                }
            }

            LoginParams loginParams = client.Network.DefaultLoginParams(
                loginOptions.FirstName, loginOptions.LastName, password,
                loginOptions.Channel, loginOptions.Version);

            grid = loginOptions.Grid;
            loginParams.Start = startLocation;
            loginParams.AgreeToTos = AgreeToTos;
            loginParams.URI = grid.LoginURI;
            loginParams.LastExecEvent = loginOptions.LastExecEvent;
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

        public Control NetcomSync
        {
            get { return netcomSync; }
            set { netcomSync = value; }
        }
    }
}
