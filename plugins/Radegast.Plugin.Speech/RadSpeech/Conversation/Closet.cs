using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;
using OpenMetaverse.Assets;
using Radegast;
using System.Windows.Forms;

namespace RadegastSpeech.Conversation
{
    internal class Closet : Mode
    {
        private Inventory inv;
        private InventoryBase selected;
        private InventoryConsole invTab;
        private TreeView tree;
        private const string INVNAME = "inventory";

        internal Closet(PluginControl pc)
            : base(pc)
        {
            Title = "inventory";
            inv = control.instance.Client.Inventory.Store;
            selected = inv.RootNode.Data;

            control.listener.CreateGrammar(
                INVNAME,
                new string[] {
                    "read it",
                    "go there",
                    "close the closet",
                    "describe it" });
        }

        internal override void Start()
        {
            base.Start();
            if (!control.instance.TabConsole.Tabs.ContainsKey(INVNAME))
                return;

            control.listener.ActivateGrammar(INVNAME);
            
            invTab = (InventoryConsole)control.instance.TabConsole.Tabs[INVNAME].Control;
            tree = invTab.invTree;
 
            tree.AfterSelect += new TreeViewEventHandler(OnItemChange);
            tree.AfterExpand += new TreeViewEventHandler(tree_AfterExpand);
            tree.AfterCollapse += new TreeViewEventHandler(tree_AfterCollapse);

            Talker.SayMore("Inventory");
            SayWhere();
        }

        internal override void Stop()
        {
            base.Stop();
            Listener.DeactivateGrammar(INVNAME);

            if (tree == null) return;
            tree.AfterSelect -= new TreeViewEventHandler(OnItemChange);
            tree.AfterExpand -= new TreeViewEventHandler(tree_AfterExpand);
            tree.AfterCollapse -= new TreeViewEventHandler(tree_AfterCollapse);
        }

        void tree_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            Talker.SayMore(DescriptiveName( selected ) + " collapsed.");
        }

        void tree_AfterExpand(object sender, TreeViewEventArgs e)
        {
            Talker.SayMore(DescriptiveName(selected) + " expanded.");
        }

        void OnItemChange(object sender, TreeViewEventArgs e)
        {
            selected = (InventoryBase)e.Node.Tag;
            SayWhere();
        }

        internal override bool Hear(string cmd)
        {
            if (base.Hear(cmd)) return true;

            if (selected is InventoryNotecard && cmd == "read it")
            {
                control.converse.AddInterruption(
                    new InvNotecard(control, selected as InventoryNotecard));
                return true;
            }

            if (selected is InventoryLandmark && cmd == "go there")
            {
                control.converse.AddInterruption(
                    new InvLandmark( control, selected as InventoryLandmark));
                return true;
            }

            switch (cmd)
            {
                case "describe it":
                {
                    ListNode();
                    break;
                }
                case "close my inventory":
                case "close the closet":
                    Talker.SayMore("The closet is closed.");
                    control.listener.DeactivateGrammar(INVNAME);
                    control.converse.SelectConversation("chat");
                    return true;

                default:
                    return false;
            }
            return true;
        }

        void SayWhere()
        {
            Talker.SayMore("Looking at " + DescriptiveName( selected ));
        }

         void ListNode()
        {
            SayWhere();

            if (selected is InventoryItem)
            {
                if (!(selected is InventoryFolder))
                {
                    Talker.SayMore("Going farther is not yet implemented.");
                    return;
                }
            }
        }
 
         string DescriptiveName(InventoryBase item)
         {
             string name = NiceName(item.Name);

             if (item is InventoryFolder)
                 return name + " folder";

             if (item is InventoryNotecard)
                 return name + ", a notecard";

             if (item is InventoryWearable)
                 return name + ", a " + WearableType(item as InventoryWearable);

             if (item is InventoryLandmark)
                 return name + ", a landmark";

             // TODO other types

             return name;
         }

        string ItemType(InventoryItem item)
        {
            switch (item.AssetType)
            {
                case AssetType.Notecard: return "note card";
                case AssetType.Folder: return "folder";
                case AssetType.Clothing: return "piece of clothing";
                case AssetType.CallingCard: return "calling card";
                case AssetType.Landmark: return "landmark";
                case AssetType.Bodypart: return "body part";
                default: return "thing";
            }

        }

        /// <summary>
        /// Get a pronouncable form of each wearable type.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string WearableType(InventoryWearable item)
        {
            switch (item.WearableType)
            {
                case OpenMetaverse.WearableType.Shirt: return "shirt";
                case OpenMetaverse.WearableType.Pants: return "pants";
                case OpenMetaverse.WearableType.Skirt: return "skirt";
                case OpenMetaverse.WearableType.Shoes: return "shoes";
                case OpenMetaverse.WearableType.Jacket: return "jacket";
                case OpenMetaverse.WearableType.Socks: return "socks";
                case OpenMetaverse.WearableType.Undershirt: return "undershirt";
                case OpenMetaverse.WearableType.Underpants: return "underpants";
                case OpenMetaverse.WearableType.Skin: return "skin";
                case OpenMetaverse.WearableType.Eyes: return "eyes";
                case OpenMetaverse.WearableType.Gloves: return "gloves";
                case OpenMetaverse.WearableType.Hair: return "hair";
                case OpenMetaverse.WearableType.Shape: return "body shape";
                default: return "clothes";
            }
        }

        void Wearing()
        {

        }
    }
}
