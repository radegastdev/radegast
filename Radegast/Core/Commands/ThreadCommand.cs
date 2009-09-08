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
            get { return "Runs a command in a thread";  }
        }

        public string Usage
        {
            get { return "thread <long running command>"; }
        }

        public void StartCommand(RadegastInstance inst)
        {
            instance = inst;
        }

        public void StopCommand(RadegastInstance inst)
        {
            foreach(var cmd in _commandThreads)
            {
                try
                {
                    cmd.Abort();
                }
                catch(Exception)
                {
                }
            }
            instance = null;
        }

        public void Execute(string threadCmd, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            string args = String.Join(" ", cmdArgs);
            if (args==string.Empty)
            {
                lock (_commandThreads)
                {
                    if (_commandThreads.Count==0)
                    {
                        WriteLine("No threaded commands.");                       
                    }
                    else
                    {
                        WriteLine("Threaded command list" );
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
                                           }) {Name = "ThreadCommand for " + args};
            lock (_commandThreads) _commandThreads.Add(thread);
            thread.Start();
        }
    }
}