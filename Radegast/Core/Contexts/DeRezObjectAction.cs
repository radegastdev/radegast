using System;
using OpenMetaverse;

namespace Radegast
{
    public class DeRezObjectAction : ContextAction
    {
        public DeRezObjectAction(RadegastInstance inst)
            : base(inst)
        {
            Label = "DeRez..";
            ContextType = typeof(Primitive);
        }

        public override void OnInvoke(object sender, EventArgs e, object target)
        {
            Primitive thePrim = ToPrimitive(target) ?? ToPrimitive(sender);
            if (thePrim == null)
            {
                DebugLog("Not found prim: " + sender + " " +target);
                return;
            }
            DebugLog("Found prim: " + thePrim);
            Client.Inventory.RequestDeRezToInventory(
                thePrim.LocalID, DeRezDestination.AgentInventoryTake,
                Client.Inventory.FindFolderForType(AssetType.TrashFolder),
                UUID.Random());
        }
    }
}