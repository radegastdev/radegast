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
using OpenMetaverse;

namespace Radegast.Netcom
{
    public partial class RadegastNetcom
    {
        // For the NetcomSync stuff
        private delegate void OnClientLoginRaise(LoginProgressEventArgs e);
        private delegate void OnClientLogoutRaise(EventArgs e);
        private delegate void OnClientDisconnectRaise(DisconnectedEventArgs e);
        private delegate void OnChatRaise(ChatEventArgs e);
        private delegate void OnInstantMessageRaise(InstantMessageEventArgs e);
        private delegate void OnAlertMessageRaise(AlertMessageEventArgs e);
        private delegate void OnMoneyBalanceRaise(BalanceEventArgs e);
        private delegate void OnTeleportStatusRaise(TeleportEventArgs e);

        public event EventHandler<OverrideEventArgs> ClientLoggingIn;
        public event EventHandler<LoginProgressEventArgs> ClientLoginStatus;
        public event EventHandler<OverrideEventArgs> ClientLoggingOut;
        public event EventHandler ClientLoggedOut;
        public event EventHandler<DisconnectedEventArgs> ClientDisconnected;
        public event EventHandler<ChatEventArgs> ChatReceived;
        public event EventHandler<ChatSentEventArgs> ChatSent;
        public event EventHandler<InstantMessageEventArgs> InstantMessageReceived;
        public event EventHandler<InstantMessageSentEventArgs> InstantMessageSent;
        public event EventHandler<TeleportingEventArgs> Teleporting;
        public event EventHandler<TeleportEventArgs> TeleportStatusChanged;
        public event EventHandler<AlertMessageEventArgs> AlertMessageReceived;
        public event EventHandler<BalanceEventArgs> MoneyBalanceUpdated;

        protected virtual void OnClientLoggingIn(OverrideEventArgs e)
        {
            if (ClientLoggingIn != null) ClientLoggingIn(this, e);
        }

        protected virtual void OnClientLoginStatus(LoginProgressEventArgs e)
        {
            if (ClientLoginStatus != null) ClientLoginStatus(this, e);
        }

        protected virtual void OnClientLoggingOut(OverrideEventArgs e)
        {
            if (ClientLoggingOut != null) ClientLoggingOut(this, e);
        }

        protected virtual void OnClientLoggedOut(EventArgs e)
        {
            if (ClientLoggedOut != null) ClientLoggedOut(this, e);
        }

        protected virtual void OnClientDisconnected(DisconnectedEventArgs e)
        {
            if (ClientDisconnected != null) ClientDisconnected(this, e);
        }

        protected virtual void OnChatReceived(ChatEventArgs e)
        {
            if (ChatReceived != null) ChatReceived(this, e);
        }

        protected virtual void OnChatSent(ChatSentEventArgs e)
        {
            if (ChatSent != null) ChatSent(this, e);
        }

        protected virtual void OnInstantMessageReceived(InstantMessageEventArgs e)
        {
            if (InstantMessageReceived != null) InstantMessageReceived(this, e);
        }

        protected virtual void OnInstantMessageSent(InstantMessageSentEventArgs e)
        {
            if (InstantMessageSent != null) InstantMessageSent(this, e);
        }

        protected virtual void OnTeleporting(TeleportingEventArgs e)
        {
            if (Teleporting != null) Teleporting(this, e);
        }

        protected virtual void OnTeleportStatusChanged(TeleportEventArgs e)
        {
            if (TeleportStatusChanged != null) TeleportStatusChanged(this, e);
        }

        protected virtual void OnAlertMessageReceived(AlertMessageEventArgs e)
        {
            if (AlertMessageReceived != null) AlertMessageReceived(this, e);
        }

        protected virtual void OnMoneyBalanceUpdated(BalanceEventArgs e)
        {
            if (MoneyBalanceUpdated != null) MoneyBalanceUpdated(this, e);
        }
    }
}
