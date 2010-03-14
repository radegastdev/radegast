using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RadegastSpeech.Environment
{
    class Control : AreaControl
    {
        internal People people;
        internal Control(PluginControl pc)
            : base(pc)
        {
            people = new People(control);
        }

        internal override void Shutdown()
        {
            people.Shutdown();
        }

        internal override void Start()
        {
            people.Start();
        }
    }
}
