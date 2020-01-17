/**
 * Radegast Metaverse Client
 * Copyright(c) 2009-2014, Radegast Development Team
 * Copyright(c) 2016-2020, Sjofn, LLC
 * All rights reserved.
 *  
 * Radegast is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Lesser General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program.If not, see<https://www.gnu.org/licenses/>.
 */

using System;

namespace Radegast.Netcom
{
    public class StartLocationParser
    {
        private string location;

        public StartLocationParser(string location)
        {
            this.location = location ?? throw new Exception("Location cannot be null.");
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

            if (locSplit.Length > 2)
            {
                int returnResult;
                bool stringToInt = int.TryParse(locSplit[2], out returnResult);

                if (stringToInt)
                    return returnResult;
            }

            return 128;
        }

        private int GetZ(string location)
        {
            if (!location.Contains("/")) return 0;

            string[] locSplit = location.Split('/');

            if (locSplit.Length > 3)
            {
                int returnResult;
                bool stringToInt = int.TryParse(locSplit[3], out returnResult);

                if (stringToInt)
                    return returnResult;
            }

            return 0;
        }

        public string Sim => GetSim(location);

        public int X => GetX(location);

        public int Y => GetY(location);

        public int Z => GetZ(location);
    }
}
