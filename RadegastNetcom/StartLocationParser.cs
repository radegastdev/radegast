using System;
using System.Collections.Generic;
using System.Text;

namespace RadegastNc
{
    public class StartLocationParser
    {
        private string location;

        public StartLocationParser(string location)
        {
            if (location == null) throw new Exception("Location cannot be null.");

            this.location = location;
        }

        private string GetSim(string location)
        {
            if (!location.Contains("/")) return location;

            string[] locSplit = location.Split('/');
            return locSplit[0];
        }

        private int GetX(string location)
        {
            if (!location.Contains("/")) return 128;

            string[] locSplit = location.Split('/');

            int returnResult;
            bool stringToInt = int.TryParse(locSplit[1], out returnResult);

            if (stringToInt)
                return returnResult;
            else
                return 128;
        }

        private int GetY(string location)
        {
            if (!location.Contains("/")) return 128;

            string[] locSplit = location.Split('/');

            int returnResult;
            bool stringToInt = int.TryParse(locSplit[2], out returnResult);

            if (stringToInt)
                return returnResult;
            else
                return 128;
        }

        private int GetZ(string location)
        {
            if (!location.Contains("/")) return 0;

            string[] locSplit = location.Split('/');

            int returnResult;
            bool stringToInt = int.TryParse(locSplit[3], out returnResult);

            if (stringToInt)
                return returnResult;
            else
                return 0;
        }

        public string Sim
        {
            get { return GetSim(location); }
        }

        public int X
        {
            get { return GetX(location); }
        }

        public int Y
        {
            get { return GetY(location); }
        }

        public int Z
        {
            get { return GetZ(location); }
        }
    }
}
