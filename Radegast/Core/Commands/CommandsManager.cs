using System;
using System.Collections.Generic;
using OpenMetaverse;

namespace Radegast.Commands
{
    public class CommandsManager : ICommandInterpreter
    {
        const string CmdPrefix = "//";
        public readonly List<IRadegastCommand> CommandsLoaded = new List<IRadegastCommand>();
        public readonly Dictionary<string, IRadegastCommand> CommandsByName = new Dictionary<string, IRadegastCommand>();
        public readonly List<ICommandInterpreter> InterpretersLoaded = new List<ICommandInterpreter>();
        RadegastInstance instance;
        public CommandsManager(RadegastInstance inst)
        {
            instance = inst;
            AddCmd("help", "Shows help info", "help help",
                   (name, cmdargs, writeline) =>
                   {
                       string args = String.Join(" ", cmdargs);
                       Help(args, writeline);
                       lock (InterpretersLoaded) foreach (ICommandInterpreter manager in InterpretersLoaded)
                           {
                               manager.Help(args, writeline);
                           }
                   });
        }

        public IRadegastCommand AddCmd(string name, string desc, string usage, CommandExecuteDelegate executeDelegate)
        {
            IRadegastCommand cmd = new RadegastCommand(name, desc, usage, executeDelegate);
            LoadCommand(cmd);
            return cmd;
        }

        public void AddExtraManager(ICommandInterpreter manager)
        {
            lock (InterpretersLoaded)
            {
                InterpretersLoaded.Add(manager);
            }
        }

        public void Help(string args, ConsoleWriteLine WriteLine)
        {
            int found = 0;
            WriteLine("Result of Help : (0)", args);
            foreach (var cmd in instance.CommandsManager.GetCommands())
            {
                if (args != "" && !args.Contains(cmd.Name) && !cmd.Name.Contains(args)) continue;
                WriteLine(" {0}: {1} Usage: {2}{3}", cmd.Name, cmd.Description, CmdPrefix, cmd.Usage);
                found++;
            }
            if (found == 0)
                WriteLine("no help.");
            else
                WriteLine("Listed {0} command{1}.", found, found == 1 ? "" : "s");
        }

        public void LoadType(Type type)
        {
            if (typeof(IRadegastCommand).IsAssignableFrom(type))
            {
                foreach (var ci in type.GetConstructors())
                {
                    if (ci.GetParameters().Length > 0) continue;
                    try
                    {
                        IRadegastCommand plug = (IRadegastCommand)ci.Invoke(new object[0]);
                        LoadCommand(plug);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("ERROR in Radegast Command: " + type + " because " + ex.Message + " " + ex.StackTrace, Helpers.LogLevel.Debug);
                        throw ex;
                    }
                }
                return;
            }
            if (typeof(ICommandInterpreter).IsAssignableFrom(type))
            {
                if (GetType()==type) return;
                foreach (var ci in type.GetConstructors())
                {
                    if (ci.GetParameters().Length > 0) continue;
                    try
                    {
                        ICommandInterpreter plug = (ICommandInterpreter)ci.Invoke(new object[0]);
                        LoadInterpreter(plug);
                        break;
                    }
                    catch (Exception ex)
                    {
                        Logger.Log("ERROR in Radegast ICommandInterpreter: " + type + " because " + ex.Message + " " + ex.StackTrace, Helpers.LogLevel.Debug);
                        throw ex;
                    }
                }
                return;
            }

        }

        private void LoadInterpreter(ICommandInterpreter interpreter)
        {
            interpreter.StartInterpreter(instance);
            lock (InterpretersLoaded) InterpretersLoaded.Add(interpreter);
        }

        private void LoadCommand(IRadegastCommand command)
        {
            command.StartCommand(instance);
            CommandsByName[command.Name.ToLower()] = command;
            lock (CommandsLoaded) CommandsLoaded.Add(command);
        }

        private void WriteLine(string fmt, object[] args)
        {
            instance.TabConsole.DisplayNotificationInChat(String.Format(fmt, args));
        }

        public void ExecuteCommand(string cmd)
        {
            ExecuteCommand(WriteLine, cmd);
        }

        public void ExecuteCommand(ConsoleWriteLine WriteLine, string cmdline)
        {
            if (cmdline == null) return;
            cmdline = cmdline.Trim();
            lock (InterpretersLoaded)
                foreach (ICommandInterpreter manager in InterpretersLoaded)
                {
                    if (manager.IsValidCommand(cmdline))
                    {
                        manager.ExecuteCommand(WriteLine, cmdline);
                        return;
                    }
                }

            // our local impl
            while (cmdline.StartsWith(CmdPrefix))
            {
                cmdline = cmdline.Substring(CmdPrefix.Length);
                cmdline = cmdline.Trim();
            }
            if (cmdline == "") return;
            string[] parsd = ParseArguments(cmdline);
            string cmd = parsd[0];
            ExecuteCommand(WriteLine, cmd, SplitOff(parsd, 1));
        }

        private void ExecuteCommand(ConsoleWriteLine WriteLine, string cmd, string[] parms)
        {
            IRadegastCommand cmdimpl;
            if (CommandsByName.TryGetValue(cmd.ToLower(), out cmdimpl))
            {
                cmdimpl.Execute(cmd, parms, WriteLine);
                return;
            }
            WriteLine("Command no found {0}", cmd);
        }

        public void Dispose()
        {
            lock (CommandsLoaded)
            {
                CommandsLoaded.ForEach(plug =>
                {
                    try
                    {
                        plug.StopCommand(instance);
                    }
                    catch (Exception) { }
                });
                CommandsLoaded.Clear();
            }
            lock (InterpretersLoaded)
            {
                InterpretersLoaded.ForEach(plug =>
                {
                    try
                    {
                        plug.StopInterpreter(instance);
                        plug.Dispose();
                    }
                    catch (Exception) { }
                });
                InterpretersLoaded.Clear();
            }
            CommandsByName.Clear();
        }

        public void StartInterpreter(RadegastInstance inst)
        {
            instance = inst;
        }

        public void StopInterpreter(RadegastInstance inst)
        {
            Dispose();
        }


        public static string[] SplitOff(string[] args, int p)
        {
            string[] newstring = new string[args.Length - p];
            int ci = 0;
            while (p < args.Length)
            {
                newstring[ci] = args[p];
                p++;
                ci++;
            }
            return newstring;
        }

        public static string[] ParseArguments(string str)
        {
            List<string> list = new List<string>();
            string current = String.Empty;
            string trimmed = null;
            bool withinQuote = false;
            bool escaped = false;

            foreach (char c in str)
            {
                if (c == '"')
                {
                    if (escaped)
                    {
                        current += '"';
                        escaped = false;
                    }
                    else
                    {
                        current += '"';
                        withinQuote = !withinQuote;
                    }
                }
                else if (c == ' ' || c == '\t')
                {
                    if (escaped || withinQuote)
                    {
                        current += c;
                        escaped = false;
                    }
                    else
                    {
                        trimmed = current.Trim();
                        if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
                        {
                            trimmed = trimmed.Remove(0, 1);
                            trimmed = trimmed.Remove(trimmed.Length - 1);
                            trimmed = trimmed.Trim();
                        }
                        if (trimmed.Length > 0)
                            list.Add(trimmed);
                        current = String.Empty;
                    }
                }
                else if (c == '\\')
                {
                    if (escaped)
                    {
                        current += '\\';
                        escaped = false;
                    }
                    else
                    {
                        escaped = true;
                    }
                }
                else
                {
                    if (escaped)
                        throw new FormatException(c.ToString() + " is not an escapable character.");
                    current += c;
                }
            }

            trimmed = current.Trim();

            if (trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
            {
                trimmed = trimmed.Remove(0, 1);
                trimmed = trimmed.Remove(trimmed.Length - 1);
                trimmed = trimmed.Trim();
            }

            if (trimmed.Length > 0)
                list.Add(trimmed);

            return list.ToArray();
        }

        public bool IsValidCommand(string msg)
        {
            if (string.IsNullOrEmpty(msg)) return false;
            lock (InterpretersLoaded) foreach (ICommandInterpreter manager in InterpretersLoaded)
                    if (manager.IsValidCommand(msg)) return true;
            msg = msg.Trim();
            if (!msg.StartsWith(CmdPrefix)) return false;
            msg = msg.Substring(2);
            return CommandExists(ParseArguments(msg)[0]);
        }

        public bool CommandExists(string cmd)
        {
            lock (CommandsByName) return CommandsByName.ContainsKey(cmd);
        }

        public IEnumerable<IRadegastCommand> GetCommands()
        {
            lock (CommandsLoaded) return new List<IRadegastCommand>(CommandsLoaded);
        }
    }
}