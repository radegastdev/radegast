using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace RadegastNc
{
    public class ClientDisconnectEventArgs : EventArgs
    {
        private NetworkManager.DisconnectType type;
        private string message;

        public ClientDisconnectEventArgs(NetworkManager.DisconnectType type, string message)
        {
            this.type = type;
            this.message = message;
        }

        public NetworkManager.DisconnectType Type
        {
            get { return type; }
        }

        public string Message
        {
            get { return message; }
        }
    }
}
