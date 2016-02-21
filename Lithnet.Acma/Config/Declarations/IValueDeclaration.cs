// -----------------------------------------------------------------------
// <copyright file="IValueDeclaration.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Collections.Generic;
    using Microsoft.MetadirectoryServices;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Defines the common interface used by ValueDeclarations
    /// </summary>
    public interface IValueDeclaration
    {
        /// <summary>
        /// Expands the declaration
        /// </summary>
        /// <param name="hologram">The object to obtain the expansion values for</param>
        /// <returns>The expanded values</returns>
        IList<object> ExpandDeclaration(MAObjectHologram hologram);

        /// <summary>
        /// Expands the declaration
        /// </summary>
        /// <param name="csentry">The object to obtain the expansion values for</param>
        /// <returns>The expanded values</returns>
        IList<object> ExpandDeclaration(CSEntryChange csentry);

        /// <summary>
        /// Expands the declaration
        /// </summary>
        /// <returns>The expanded values</returns>
        IList<object> ExpandDeclaration();

        SchemaAttributeUsage GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute);
    }
}
