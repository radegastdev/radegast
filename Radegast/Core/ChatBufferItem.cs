using System;
using System.Collections.Generic;
using System.Text;

namespace Radegast
{
    public class ChatBufferItem
    {
        private DateTime timestamp;
        private string text;
        private ChatBufferTextStyle style;

        public ChatBufferItem()
        {

        }

        public ChatBufferItem(DateTime timestamp, string text, ChatBufferTextStyle style)
        {
            this.timestamp = timestamp;
            this.text = text;
            this.style = style;
        }

        public DateTime Timestamp
        {
            get { return timestamp; }
            set { timestamp = value; }
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public ChatBufferTextStyle Style
        {
            get { return style; }
            set { style = value; }
        }
    }

    public enum ChatBufferTextStyle
    {
        Normal,
        StatusBlue,
        StatusDarkBlue,
        LindenChat,
        ObjectChat,
        StartupTitle,
        Error,
        Alert
    }
}
