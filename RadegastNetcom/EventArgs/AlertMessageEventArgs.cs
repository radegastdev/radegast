using System;
using System.Collections.Generic;
using System.Text;

namespace RadegastNc
{
    public class AlertMessageEventArgs : EventArgs
    {
        private string message;

        public AlertMessageEventArgs(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get { return message; }
        }
    }
}
