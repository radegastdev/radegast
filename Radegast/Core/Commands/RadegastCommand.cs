namespace Radegast.Commands
{
    public class RadegastCommand : IRadegastCommand
    {
        private readonly CommandExecuteDelegate _execute;

        /// <summary>
        /// for subclasses (they should override Execute)
        /// </summary>
        /// <param name="name"></param>
        public RadegastCommand(string name)
            : this(name, "Description of " + name + " is unknown.", name + " <stuff>")
        {
        }
        /// <summary>
        /// for subclasses (they should override Execute)
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="usage"></param>
        public RadegastCommand(string name, string desc, string usage)
        {
            Name = name;
            Description = desc;
            Usage = usage;
            _execute = null;
        }
        /// <summary>
        /// For simple creation of new commands
        /// </summary>
        /// <param name="name"></param>
        /// <param name="desc"></param>
        /// <param name="usage"></param>
        /// <param name="exec"></param>
        public RadegastCommand(string name, string desc, string usage, CommandExecuteDelegate exec)
            : this(name, desc, usage)
        {
            _execute = exec;
        }

        virtual public string Name { get; private set; }

        virtual public string Description { get; private set; }

        virtual public string Usage { get; private set; }

        virtual public void StartCommand(RadegastInstance inst) { }

        virtual public void StopCommand(RadegastInstance inst) { }

        virtual public void Execute(string name, string[] cmdArgs, ConsoleWriteLine WriteLine)
        {
            if (_execute == null) WriteLine("Someone did not implement {0}!", name);
            else _execute(name, cmdArgs, WriteLine);
        }
    }
}