using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lithnet.Fim.UniversalMARE.Editor
{
    /// <summary>
    /// This class is responsible for providing custom properties to WPF - in this instance
    /// allowing you to bind to commands by name
    /// </summary>
    internal class CommandMapDescriptor : CustomTypeDescriptor
    {
        private CommandMap map;

        /// <summary>
        /// Store the command map for later
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="map"></param>
        public CommandMapDescriptor(ICustomTypeDescriptor descriptor, CommandMap map)
            : base(descriptor)
        {
            this.map = map;
        }

        /// <summary>
        /// Get the properties for this command map
        /// </summary>
        /// <returns>A collection of synthesized property descriptors</returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            //TODO: See about caching these properties (need the _map to be observable so can respond to add/remove)
            PropertyDescriptor[] props = new PropertyDescriptor[this.map.Commands.Count];

            int pos = 0;

            foreach (KeyValuePair<string, ICommand> command in this.map.Commands)
                props[pos++] = new CommandPropertyDescriptor(command);

            return new PropertyDescriptorCollection(props);
        }
    }
}
