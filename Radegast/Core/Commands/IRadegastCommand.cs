namespace Radegast
{
    public delegate void CommandExecuteDelegate(string name, string[] cmdArgs, ConsoleWriteLine WriteLine);
    public delegate void ConsoleWriteLine(string fmt, params object[] args);
    public interface IRadegastCommand
    {
        string Name { get; }
        string Description { get; }
        string Usage { get; }
        void StartCommand(RadegastInstance inst);
        void StopCommand(RadegastInstance inst);
        void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine);
    }
}