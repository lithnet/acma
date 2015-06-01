using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lithnet.Fim.UniversalMARE.Editor
{
    /// <summary>
    /// A map that exposes commands in a WPF binding friendly manner
    /// </summary>
    [TypeDescriptionProvider(typeof(CommandMapDescriptionProvider))]
    public class CommandMap
    {
        /// <summary>
        /// Store the commands
        /// </summary>
        private Dictionary<string, ICommand> commands;

        public CommandMap()
        {
            this.commands = new Dictionary<string, ICommand>();
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="executeMethod">The method to execute</param>
        public void AddCommand(string commandName, Action<object> executeMethod)
        {
            this.Commands[commandName] = new DelegateCommand(executeMethod);
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public void AddCommand(string commandName, Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            this.Commands[commandName] = new DelegateCommand(executeMethod, canExecuteMethod);
        }

        /// <summary>
        /// Remove a command from the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        public void RemoveCommand(string commandName)
        {
            this.Commands.Remove(commandName);
        }

        /// <summary>
        /// Expose the dictionary of commands
        /// </summary>
        public Dictionary<string, ICommand> Commands
        {
            get
            {
                if (null == this.commands)
                    this.commands = new Dictionary<string, ICommand>();

                return this.commands;
            }
        }
    }
}
