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

using OpenMetaverse;

namespace Radegast.Commands
{
    public class RadegastCommand : IRadegastCommand
    {
        private readonly CommandExecuteDelegate _execute;

        /// <summary>
        /// Radegast instance received during start command
        /// </summary>
        protected RadegastInstance Instance { get; private set; }

        /// <summary>
        /// GridClinet associated with RadegastInstanc received during command startup
        /// </summary>
        protected GridClient Client => Instance.Client;

        /// <summary>
        /// For simple creation of new commands
        /// </summary>
        /// <param name="inst"></param>
        public RadegastCommand(RadegastInstance inst)
        {
            Instance = inst;
            _execute = null;
        }

        /// <summary>
        /// For simple creation of new commands
        /// </summary>
        /// <param name="inst"></param>
        /// <param name="exec"></param>
        public RadegastCommand(RadegastInstance inst, CommandExecuteDelegate exec)
        {
            Instance = inst;
            _execute = exec;
        }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string Usage { get; set; }

        public virtual void StartCommand(RadegastInstance inst)
        {
            Instance = inst;
        }

        // maybe we shoould make this class abstract to force people to implement
        public virtual void Dispose()
        {
            Instance = null;
        }

        public virtual void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            if (_execute == null) WriteLine("Someone did not implement {0}!", name);
            else _execute(name, cmdArgs, WriteLine);
        }

    }
}