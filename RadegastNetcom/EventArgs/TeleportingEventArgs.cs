using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace RadegastNc
{
    public class TeleportingEventArgs : OverrideEventArgs
    {
        private string _sim;
        private Vector3 _coordinates;

        public TeleportingEventArgs(string sim, Vector3 coordinates) : base()
        {
            _sim = sim;
            _coordinates = coordinates;
        }

        public string SimName
        {
            get { return _sim; }
        }

        public Vector3 Coordinates
        {
            get { return _coordinates; }
        }
    }
}