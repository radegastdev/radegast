using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace RadegastNc
{
    public class ClientLoginEventArgs : EventArgs
    {
        private LoginStatus status;
        private string message;

        public ClientLoginEventArgs(LoginStatus status, string message)
        {
            this.status = status;
            this.message = message;
        }

        public LoginStatus Status
        {
            get { return status; }
        }

        public string Message
        {
            get { return message; }
        }
    }
}