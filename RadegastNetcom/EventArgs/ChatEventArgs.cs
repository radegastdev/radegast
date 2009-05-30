using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace RadegastNc
{
    public class ChatEventArgs : EventArgs
    {
        private string message;
        private ChatAudibleLevel audible;
        private ChatType type;
        private ChatSourceType sourceType;
        private string fromName;
        private UUID id;
        private UUID ownerid;
        private Vector3 position;

        public ChatEventArgs(string message, ChatAudibleLevel audible, ChatType type, ChatSourceType sourceType, string fromName, UUID id, UUID ownerid, Vector3 position)
        {
            this.message = message;
            this.audible = audible;
            this.type = type;
            this.sourceType = sourceType;
            this.fromName = fromName;
            this.id = id;
            this.ownerid = ownerid;
            this.position = position;
        }

        public string Message
        {
            get { return message; }
        }

        public ChatAudibleLevel Audible
        {
            get { return audible; }
        }

        public ChatType Type
        {
            get { return type; }
        }

        public ChatSourceType SourceType
        {
            get { return sourceType; }
        }

        public string FromName
        {
            get { return fromName; }
        }

        public UUID ID
        {
            get { return id; }
        }

        public UUID OwnerID
        {
            get { return ownerid; }
        }

        public Vector3 Position
        {
            get { return position; }
        }
    }
}