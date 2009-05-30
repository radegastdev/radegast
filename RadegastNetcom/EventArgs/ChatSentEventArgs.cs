using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace RadegastNc
{
    public class ChatSentEventArgs : EventArgs
    {
        private string message;
        private ChatType type;
        private int channel;

        public ChatSentEventArgs(string message, ChatType type, int channel)
        {
            this.message = message;
            this.type = type;
            this.channel = channel;
        }

        public string Message
        {
            get { return message; }
        }

        public ChatType Type
        {
            get { return type; }
        }

        public int Channel
        {
            get { return channel; }
        }
    }
}