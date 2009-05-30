using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace RadegastNc
{
    public class InstantMessageEventArgs : EventArgs
    {
        private InstantMessage im;
        private Simulator sim;

        public InstantMessageEventArgs(InstantMessage im, Simulator sim)
        {
            this.im = im;
            this.sim = sim;
        }

        public InstantMessage IM
        {
            get { return im; }
        }

        public Simulator Sim
        {
            get { return sim; }
        }
    }
}
