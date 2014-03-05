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
using System;
using System.Collections.Generic;
#if (COGBOT_LIBOMV || USE_STHREADS)
using ThreadPoolUtil;
using Thread = ThreadPoolUtil.Thread;
using ThreadPool = ThreadPoolUtil.ThreadPool;
using Monitor = ThreadPoolUtil.Monitor;
#endif
using System.Threading;

namespace Radegast.Commands
{
    public class ThreadCommand : IRadegastCommand
    {
        private List<Thread> _listed = null;
        List<Thread> _commandThreads = new List<Thread>();
        Queue<KeyValuePair<string, ThreadStart>> _commandQueue { get { return instance.CommandsManager.CommandQueue; } }
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
            instance.CommandsManager.CommandsByName.Add("kill", this);
            instance.CommandsManager.CommandsByName.Add("threads", this);
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
            if (threadCmd == "kill")
            {
                if (_listed == null)
                {
                    WriteLine("Must load the threadlist Using: thread");
                    return;
                }
                lock (_listed)
                {
                    if (_listed.Count == 0)
                    {
                        WriteLine("No threaded commands to kill.");
                    }
                    else
                    {
                        WriteLine("Killing all threaded commands");
                        int found = 0;
                        foreach (var cmd in _listed)
                        {
                            try
                            {
                                if (cmd.IsAlive)
                                {
                                    found++;
                                    cmd.Abort();
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        WriteLine("Killed {0} thread{1}.", found, found == 1 ? "" : "s");
                        _listed = null;

                    }
                }
                lock (_commandQueue)
                {
                    int commandQueueCount = _commandQueue.Count;
                    if (commandQueueCount == 0)
                    {
                        WriteLine("No queued commands.");
                    }
                    else
                    {
                        _commandQueue.Clear();
                        WriteLine("Clear queued commands: " + commandQueueCount);
                    }
                }
                return;
            }

            //cleared each time becasue the list will change 
            _listed = null;
            if (args == string.Empty)
            {
                lock (_commandThreads) _listed = new List<Thread>(_commandThreads);
                if (_listed.Count == 0)
                {
                    WriteLine("No threaded commands.");
                }
                else
                {
                    int found = _listed.Count;
                    WriteLine("Listing {0} thread{1}.", found, found == 1 ? "" : "s");
                    for (int i = 0; i < _listed.Count; i++)
                    {
                        WriteLine(" * {0}: {1}", i, _listed[i].Name);
                    }
                }
                lock (_commandQueue)
                {
                    if (_commandQueue.Count == 0)
                    {
                        WriteLine("No queued commands.");
                    }
                    else
                    {
                        WriteLine("Queued commands: {0}", _commandQueue.Count);
                        foreach (var c in _commandQueue)
                        {
                            WriteLine(" Q: {0}", c.Key);
                        }
                    }
                }
                return;
            }
            string name = string.Format("ThreadCommand {0}: {1}", DateTime.Now, args);
            Thread thread = new Thread(() =>
                                           {
                                               try
                                               {
                                                   WriteLine("Started command: {0}", args);
                                                   instance.CommandsManager.ExecuteCommandForeground(WriteLine, args);
                                               }
                                               catch (Exception e)
                                               {
                                                   WriteLine("Exception: " + name + "\n" + e);
                                               }
                                               finally
                                               {
                                                   WriteLine("Done with {0}", args);
                                                   _commandThreads.Remove(Thread.CurrentThread);
                                               }
                                           }) {Name = name};
            lock (_commandThreads) _commandThreads.Add(thread);
            thread.Start();
        }
    }

    public class PauseCommand : RadegastCommand
    {
        public PauseCommand(RadegastInstance inst)
            : base(inst)
        {
            Name = "pause";
            Description = "pauses command script execution";
            Usage = "pause [seconds]";
        }
        public override void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            float time;
            if (cmdArgs.Length == 0 || !float.TryParse(cmdArgs[0], out time))
                WriteLine(Usage);
            else
            {
                WriteLine("pausing for " + time);
                Thread.Sleep((int)(time * 1000));
                WriteLine("paused for " + time);
            }
        }
    }
}