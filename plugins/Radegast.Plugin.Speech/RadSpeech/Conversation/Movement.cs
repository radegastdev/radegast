namespace RadegastSpeech.Conversation
{
    class Movement : Mode
    {
        internal Movement(PluginControl pc) : base(pc)
        {
        }

        internal override bool Hear(string text)
        {
            return false;
        }
    }
}
