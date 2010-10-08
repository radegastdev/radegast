using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

namespace RadegastSpeech.Conversation
{
    class Max : Mode
    {
        internal Max(PluginControl pc) : base(pc)
        {
        }

        internal override void Start()
        {
//TODO            Talker.Say("Max", "arf arf");
        }
        internal override bool Hear(string message)
        {
            if (message == "thank you max")
            {
                FinishInterruption();
                return true;
            }
            Client.Self.Chat(message, 2, ChatType.Normal);
            return true;
        }


    }
}
