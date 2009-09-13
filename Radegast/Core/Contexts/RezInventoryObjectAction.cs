using System;
using OpenMetaverse;

namespace Radegast
{
    public class RezInventoryObjectAction : ContextAction
    {
        public RezInventoryObjectAction(RadegastInstance inst)
            : base(inst)
        {
            Label = "Rez..";
            ContextType = typeof(InventoryObject);
        }
        public override void OnInvoke(object sender, EventArgs e, object target)
        {
            Client.Inventory.RequestRezFromInventory(
                Client.Network.CurrentSim,
                Client.Self.SimRotation, Client.Self.SimPosition,
                (InventoryItem) (target ?? sender));
        }
    }
}