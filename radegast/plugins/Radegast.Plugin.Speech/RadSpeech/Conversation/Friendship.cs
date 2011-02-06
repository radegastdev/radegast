using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

namespace RadegastSpeech.Conversation
{
    internal class Friendship : Mode
    {
        private string who;

        internal Friendship(PluginControl pc, string name, UUID session)
            : base(pc)
        {
            who = name;
        }
        internal override void Start()
        {

        }

        internal override bool Hear(string message)
        {
            return false;
        }
    }
}
