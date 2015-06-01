using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lithnet.Fim.UniversalMARE.Editor
{
    /// <summary>
    /// Expose the dictionary entries of a CommandMap as properties
    /// </summary>
    internal class CommandMapDescriptionProvider : TypeDescriptionProvider
    {
        /// <summary>
        /// Standard constructor
        /// </summary>
        public CommandMapDescriptionProvider()
            : this(TypeDescriptor.GetProvider(typeof(CommandMap)))
        {
        }

        /// <summary>
        /// Construct the provider based on a parent provider
        /// </summary>
        /// <param name="parent"></param>
        public CommandMapDescriptionProvider(TypeDescriptionProvider parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Get the type descriptor for a given object instance
        /// </summary>
        /// <param name="objectType">The type of object for which a type descriptor is requested</param>
        /// <param name="instance">The instance of the object</param>
        /// <returns>A custom type descriptor</returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new CommandMapDescriptor(base.GetTypeDescriptor(objectType, instance), instance as CommandMap);
        }
    }

}
