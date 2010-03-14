using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenMetaverse;

namespace RadegastSpeech.Sound
{
    abstract class Control : AreaControl
    {
        internal Control(PluginControl pc)
            : base(pc)
        {
        }

        /// <summary>
        /// Play a sound once at a specific location.
        /// </summary>
        /// <param name="?"></param>
        internal abstract void Play(
            string filename,
            int sps,
            OpenMetaverse.Vector3 pos,
            bool deleteAfter,
            bool spatialized);

        internal abstract void Stop();
        internal override void Start()
        {
 
        }
        internal override void Shutdown()
        {
            Stop();
        }

    }
}
