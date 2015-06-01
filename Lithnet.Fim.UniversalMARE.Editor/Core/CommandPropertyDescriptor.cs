using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lithnet.Fim.UniversalMARE.Editor
{
    /// <summary>
    /// A property descriptor which exposes an ICommand instance
    /// </summary>
    internal class CommandPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// Store the command which will be executed
        /// </summary>
        private ICommand command;

        /// <summary>
        /// Construct the descriptor
        /// </summary>
        /// <param name="command"></param>
        public CommandPropertyDescriptor(KeyValuePair<string, ICommand> command)
            : base(command.Key, null)
        {
            this.command = command.Value;
        }

        /// <summary>
        /// Always read only in this case
        /// </summary>
        public override bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Nope, it's read only
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Not needed
        /// </summary>
        public override Type ComponentType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get the ICommand from the parent command map
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override object GetValue(object component)
        {
            CommandMap map = component as CommandMap;

            if (null == map)
                throw new ArgumentException("component is not a CommandMap instance", "component");

            return map.Commands[this.Name];
        }

        /// <summary>
        /// Get the type of the property
        /// </summary>
        public override Type PropertyType
        {
            get { return typeof(ICommand); }
        }

        /// <summary>
        /// Not needed
        /// </summary>
        /// <param name="component"></param>
        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not needed
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(object component, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not needed
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
