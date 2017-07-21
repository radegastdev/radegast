// 
// Radegast Metaverse Client
// Copyright (c) 2009-2014, Radegast Development Team
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
//     * Redistributions of source code must retain the above copyright notice,
//       this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright
//       notice, this list of conditions and the following disclaimer in the
//       documentation and/or other materials provided with the distribution.
//     * Neither the name of the application "Radegast", nor the names of its
//       contributors may be used to endorse or promote products derived from
//       this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
// $Id$
//

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