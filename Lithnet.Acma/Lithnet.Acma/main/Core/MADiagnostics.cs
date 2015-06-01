// -----------------------------------------------------------------------
// <copyright file="MADiagnostics.cs" company="Lithnet">
// Copyright (c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Contains debugging and diagnostic routines used in troubleshooting and reporting
    /// </summary>
    public static class MADiagnostics
    {
        /// <summary>
        /// Dumps all the information in a CSEntryChange object to the log
        /// </summary>
        /// <param name="csentry">The CSEntryChange to dump</param>
        public static void DumpCSEntryChange(CSEntryChange csentry)
        {
            Logger.WriteSeparatorLine('-');
            Logger.WriteLine("CSEntryChange: " + csentry.Identifier.ToString());
            Logger.WriteLine("DN: " + csentry.DN);
            Logger.WriteLine("Object class: " + csentry.ObjectType);
            Logger.WriteLine("Object modification type: " + csentry.ObjectModificationType.ToString());
            Logger.WriteLine("Import error code: " + csentry.ErrorCodeImport.ToString());
            Logger.WriteLine("Error name: " + csentry.ErrorName);
            Logger.WriteLine("Error detail: " + csentry.ErrorDetail);

            Logger.WriteLine("Anchor Attributes");
            foreach (var anchorAttribute in csentry.AnchorAttributes)
            {
                Logger.WriteLine("  Attribute: {0}, Modification type: {1}, Value: {2}", anchorAttribute.Name, anchorAttribute.DataType.ToString(), anchorAttribute.Value.ToSmartString());
            }

            Logger.WriteLine("Attribute Changes");
            foreach (var attributeChange in csentry.AttributeChanges)
            {
                Logger.WriteLine("  Attribute: {0}, Type: {1}, Multivalued: {2}, Modification Type: {3}", attributeChange.Name, attributeChange.DataType.ToString(), attributeChange.IsMultiValued.ToString(), attributeChange.ModificationType.ToString());
                foreach (var valueChange in attributeChange.ValueChanges)
                {
                    Logger.WriteLine("      {0}: {1}", valueChange.ModificationType.ToString(), valueChange.Value.ToSmartString());
                }
            }

            Logger.WriteSeparatorLine('-');
        }
    }
}
