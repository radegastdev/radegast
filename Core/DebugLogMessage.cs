using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace Radegast
{
    public class DebugLogMessage
    {
        private string message;
        private Helpers.LogLevel level;

        public DebugLogMessage(string message, Helpers.LogLevel level)
        {
            this.message = message;
            this.level = level;
        }

        public string Message
        {
            get { return message; }
        }

        public Helpers.LogLevel Level
        {
            get { return level; }
        }
    }
}
