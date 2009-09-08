using System;
using System.Collections.Generic;

namespace Radegast.Commands
{
    public interface ICommandInterpreter
    {
        bool IsValidCommand(string cmdline);
        void ExecuteCommand(ConsoleWriteLine WriteLine, string cmdline);
        void Help(string helpArgs, ConsoleWriteLine WriteLine);
        void Dispose();
        void StartInterpreter(RadegastInstance inst);
        void StopInterpreter(RadegastInstance inst);
    }
}
