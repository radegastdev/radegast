using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse.Assets;
using OpenMetaverse;
using Radegast;
namespace RadegastSpeech.Conversation
{
    class InvNotecard : Mode
    {
        private InventoryNotecard asset;
        private string text;
        private int StartPosition;
        private int StopPosition;
        private bool isMore;
        internal InvNotecard(PluginControl pc, InventoryNotecard a)
            : base(pc)
        {
            asset = a;
        }

        internal override void Start()
        {
            base.Start();
            Talker.SayMore("Reading " + asset.Name + ". Just a moment.");
            Client.Assets.RequestInventoryAsset(
                asset,
                true,
                Assets_OnNotecardReceived);
        }

        internal override void Stop()
        {
            base.Stop();
        }
        internal override bool Hear(string cmd)
        {
            switch (cmd)
            {
                case "next":
                case "more":
                    NextSection();
                    return true;
                case "repeat":
                    ReadSection();
                    return true;
                case "start over":
                    StopPosition = 0;
                    NextSection();
                    return true;
                case "stop":
                    control.instance.MainForm.KeyDown -=
                        new System.Windows.Forms.KeyEventHandler(MainForm_KeyPress);
                    FinishInterruption();
                    return true;
            }
            return false;
        }

        void Assets_OnNotecardReceived(AssetDownload transfer, Asset asset)
        {

            if (transfer.Success)
            {
                AssetNotecard n = (AssetNotecard)asset;
                n.Decode();
                AssetNotecard recievedNotecard = n;

                text = string.Empty;

                for (int i = 0; i < n.BodyText.Length; i++)
                {
                    char c = n.BodyText[i];

                    // Special marker for embedded things.
                    if ((int)c == 0xdbc0)
                    {
                        int index = (int)n.BodyText[++i] - 0xdc00;
                        InventoryItem e = n.EmbeddedItems[index];
                        text += " (embedded) ";
                    }
                    else
                    {
                        text += c;
                    }
                }

                // TODO put in controls to stop, back up etc
                StopPosition = 0;
                NextSection();
                control.instance.MainForm.KeyDown +=
                    new System.Windows.Forms.KeyEventHandler(MainForm_KeyPress);
            }
            else
            {
                Talker.Say("Failed to download the notecard.", Talk.BeepType.Bad);
            }
        }

        void MainForm_KeyPress(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Space ||
                e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                NextSection();
                e.Handled = true;
                return;
            }

            if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                PrevSection();
                e.Handled = true;
                return;
            }

            if (e.KeyCode == System.Windows.Forms.Keys.Right)
            {
                ReadSection();
                e.Handled = true;
                return;
            }
        }

        void ReadSection()
        {
            Talker.SayMore( text.Substring( StartPosition, StopPosition-StartPosition ));
            if (!isMore)
               Talker.SayMore("End of notecard.");
        }

        /// <summary>
        /// Delineate the previous section and read it.
        /// </summary>
        void PrevSection()
        {
            if (StartPosition == 0)
            {
                ReadSection();
                return;
            }

            StopPosition = StartPosition;

            //TODO back up more cleverly.
            StartPosition = StopPosition -100;
            if (StartPosition < 0)
                StartPosition = 0;

            ReadSection();
        }

        /// <summary>
        /// Delineate the next section and read it.
        /// </summary>
        void NextSection()
        {
            StartPosition = StopPosition;
            int section = text.IndexOfAny(
                new char[] { '.', '\n' },
                StartPosition);

            if (section >= 0)
            {
                isMore = true;
                StopPosition = section + 1;
            }
            else
            {
                isMore = false;
                StopPosition = text.Length - 1;
            }

            ReadSection();
        }
    }
}
