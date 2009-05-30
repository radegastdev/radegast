using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace RadegastNc
{
    public class TeleportStatusEventArgs : EventArgs
    {
        private string message;
        private TeleportStatus status;
        private TeleportFlags flags;

        public TeleportStatusEventArgs(string message, TeleportStatus status, TeleportFlags flags)
        {
            this.message = message;
            this.status = status;
            this.flags = flags;
        }

        public string Message
        {
            get { return message; }
        }

        public TeleportStatus Status
        {
            get { return status; }
        }

        public TeleportFlags Flags
        {
            get { return flags; }
        }
    }
}
