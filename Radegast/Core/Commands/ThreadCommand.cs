// 
// Radegast Metaverse Client
// Copyright (c) 2009, Radegast Development Team
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
using System;
using System.Collections.Generic;
using System.Threading;

namespace Radegast.Commands
{
    public class ThreadCommand : IRadegastCommand
    {
        readonly List<Thread> _commandThreads = new List<Thread>();
        private RadegastInstance instance;
        public string Name
        {
            get { return "thread"; }
        }

        public string Description
        {
            get { return "Runs a command in a thread"; }
        }

        public string Usage
        {
            get { return "thread <long running command>"; }
        }

        public void StartCommand(RadegastInstance inst)
        {
            instance = inst;
        }

        public void Dispose()
        {
            foreach (var cmd in _commandThreads)
            {
                try
                {
                    cmd.Abort();
                }
                catch (Exception)
                {
                }
            }
            instance = null;
        }

        public void Execute(string threadCmd, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            string args = String.Join(" ", cmdArgs);
            if (args == string.Empty)
            {
                lock (_commandThreads)
                {
                    if (_commandThreads.Count == 0)
                    {
                        WriteLine("No threaded commands.");
                    }
                    else
                    {
                        WriteLine("Threaded command list");
                        foreach (Thread thr in _commandThreads)
                        {
                            WriteLine(" * {0}", thr.Name);
                        }
                        int found = _commandThreads.Count;
                        WriteLine("Listed {0} threads{1}.", found, found == 1 ? "" : "s");
                    }
                }
                return;
            }
            Thread thread = new Thread(() =>
                                           {
                                               try
                                               {
                                                   WriteLine("Started command: {0}", args);
                                                   instance.CommandsManager.ExecuteCommand(WriteLine, args);
                                               }
                                               finally
                                               {
                                                   WriteLine("Done with {0}", args);
                                                   _commandThreads.Remove(Thread.CurrentThread);
                                               }
                                           }) { Name = "ThreadCommand for " + args };
            lock (_commandThreads) _commandThreads.Add(thread);
            thread.Start();
        }
    }
}