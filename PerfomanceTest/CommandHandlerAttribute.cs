using System;

namespace PerformanceTest
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    class CommandHandlerAttribute : Attribute
    {
        public CommandHandlerAttribute(string commandName) 
        {
            CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
        }
        public CommandHandlerAttribute(string commandName, string commandDescription)
        {
            CommandName = commandName ?? throw new ArgumentNullException(nameof(commandName));
            CommandDescription = commandDescription ?? throw new ArgumentNullException(nameof(commandDescription));
        }

        public string CommandName { get; }
        public string CommandDescription { get; }
    }
}
