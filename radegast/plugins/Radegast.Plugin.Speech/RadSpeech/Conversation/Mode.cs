using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using OpenMetaverse;
using Radegast;

namespace RadegastSpeech.Conversation
{
    internal abstract class Mode
    {
        protected PluginControl control;
        protected Vector3 SessionPosition;
        internal string Title;
        protected string Announcement;
        protected List<string> altNames;
        protected bool isMuted = false;
        protected GridClient Client { get { return control.instance.Client; }}
        protected Talk.Control Talker { get { return control.talker; } }
        protected Conversation.Control Converse { get { return control.converse; } }
        protected Radegast.TabsConsole TabConsole { get { return control.instance.TabConsole; } }
        protected Listen.Control Listener { get { return control.listener; } }
        private static Regex BadNameChars;
        private static TalkingContextMenu menu;

        internal Mode(PluginControl pc)
        {
            // One copy of the context menu manager is shared by all conversations.
            if (menu==null)
            {
                menu = new TalkingContextMenu(pc);
            }

            control = pc;

            // Default position right rear
            SessionPosition = new Vector3(-2.0f, -2.0f, 0.0f);
            Title = "Un-named conversation";

            if (BadNameChars == null)
                BadNameChars = new Regex(@"[0-9]+", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Text from speech input.
        /// </summary>
        /// <param name="message"></param>
        internal virtual bool Hear(string message)
        {
            if (menu != null)
                return menu.Hear(message);

            return false;
        }

        internal virtual void Start()
        {
            menu.Start();
        }

        internal virtual void Stop()
        {
            altNames = null;
            menu.Stop();

//            TabConsole.KeyDown -=
//                new System.Windows.Forms.KeyEventHandler(OnKeyDown);
        }
        /// <summary>
        /// Key handler for navigating lists.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case System.Windows.Forms.Keys.Down:
                    Hear("next");
                    break;
                case System.Windows.Forms.Keys.Enter:
                case System.Windows.Forms.Keys.Space:
                    Hear("ok");
                    break;
                default:
                    return;
            }
            e.Handled = true;

        }

        /// <summary>
        /// Convert full avatar names into more humanistic form.
        /// </summary>
        /// <param name="who"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        protected string FriendlyName(string who, UUID id)
        {
            // Say full name the first time we hear this person,
            // just the first name thereafter.  Non-persons always
            // say the full name.
            string saythis;
            if (Talker.voices.KnowThisPerson(id))
                saythis = who.Split(' ')[0];
            else
                saythis = who;

            // Remove digits from names.
            saythis = BadNameChars.Replace(saythis, "");
            return saythis;
        }

        protected void FinishInterruption()
        {
            Converse.FinishInterruption( this );
        }

        protected bool amCurrent()
        {
            return (Converse.amCurrent(this));
        }

        protected string NiceName(string name)
        {
            return Regex.Replace(name, @"\((\d+,? *)+\)", "");
        }
    }
}
