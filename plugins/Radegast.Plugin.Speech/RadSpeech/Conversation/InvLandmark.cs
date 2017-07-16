using OpenMetaverse;

namespace RadegastSpeech.Conversation
{
    class InvLandmark : Mode
    {
        private InventoryLandmark asset;
        internal InvLandmark(PluginControl pc, InventoryLandmark a)
            : base(pc)
        {
            asset = a;
        }

        internal override void Start()
        {
            base.Start();
            Talker.SayMore("Do you want to go to " + NiceName(asset.Name));
        }

        internal override bool Hear(string cmd)
        {
            // Commands "go" "describe"
            switch (cmd)
            {
                case "no":
                    FinishInterruption();
                    return true;
                case "describe":
                    Describe();
                    return true;
                case "yes":
                case "go":
                    Talker.SayMore("Here we go.");
                    Client.Self.Teleport(asset.AssetUUID);
                    // TODO Should force conversation to Chat here.
                    FinishInterruption();
                    return true;
                default:
                    return false;
            }
        }

        void Describe()
        {
            Talker.SayMore(asset.Description);
        }

     }
}
