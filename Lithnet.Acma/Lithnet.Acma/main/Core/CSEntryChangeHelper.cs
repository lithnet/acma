// -----------------------------------------------------------------------
// <copyright file="CSEntryChangeHelper.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
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
    using Lithnet.Acma.DataModel;
    using Microsoft.MetadirectoryServices.DetachedObjectModel;

    /// <summary>
    /// Contains methods to manage CSEntryChanges
    /// </summary>
    public static class CSEntryChangeExtensions
    {
        /// <summary>
        /// Creates a CSEntryChange of the type 'add' for the supplied MAObjectHologram
        /// </summary>
        /// <param name="maObject">The MAObjectHologram to create the CSEntryChange from</param>
        /// <returns>A new CSEntryChange object representing the current state of the specified MAObjectHologram </returns>
        public static AcmaCSEntryChange CreateCSEntryChangeFromMAObjectHologram(this MAObjectHologram maObject)
        {
            return CSEntryChangeExtensions.CreateCSEntryChangeFromMAObjectHologram(maObject, ObjectModificationType.Add);
        }

        /// <summary>
        /// Creates a CSEntryChange of the specified modification type for the supplied MAObjectHologram
        /// </summary>
        /// <param name="maObject">The MAObjectHologram to create the CSEntryChange from</param>
        /// <param name="objectModificationType">The object modification type to apply</param>
        /// <returns>A new CSEntryChange object representing the current state of the specified MAObjectHologram </returns>
        public static AcmaCSEntryChange CreateCSEntryChangeFromMAObjectHologram(this MAObjectHologram maObject, ObjectModificationType objectModificationType)
        {
            AcmaCSEntryChange csentry = new AcmaCSEntryChange();

            csentry.ObjectModificationType = objectModificationType;
            csentry.DN = maObject.Id.ToString();
            csentry.ObjectType = maObject.ObjectClass.Name;

            if (objectModificationType != ObjectModificationType.Delete)
            {
                AttributeModificationType attributeModificationType = objectModificationType == ObjectModificationType.Update ? AttributeModificationType.Replace : AttributeModificationType.Add;

                foreach (AcmaSchemaAttribute attribute in maObject.ObjectClass.Attributes.Where(t => t.Name != "objectId" && t.Name != "objectClass"))
                {
                    AttributeValues values = maObject.GetAttributeValues(attribute);

                    if (values.IsEmptyOrNull)
                    {
                        continue;
                    }

                    if (attributeModificationType == AttributeModificationType.Add)
                    {
                        AttributeChange change = AttributeChange.CreateAttributeAdd(attribute.Name, values.ToObjectList());
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, values.ToObjectList()));
                    }
                    else
                    {
                        csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(attribute.Name, values.ToObjectList()));
                    }
                }
            }

            return csentry;
        }

        /// <summary>
        /// Converts a CSEntryChange from an 'add' modification type to 'update', converting the contained AttributeChanges from 'add' to 'replace'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to modify</param>
        /// <returns>A copy of the original CSEntryChange with the modification type set to 'update'</returns>
        public static CSEntryChange ConvertCSEntryChangeAddToUpdate(this CSEntryChange csentry)
        {
            CSEntryChange newcsentry = CSEntryChange.Create();
            newcsentry.ObjectModificationType = ObjectModificationType.Update;
            newcsentry.ObjectType = csentry.ObjectType;
            newcsentry.DN = csentry.DN;

            foreach (AttributeChange attributeChange in csentry.AttributeChanges)
            {
                AttributeChange newAttributeChange = AttributeChange.CreateAttributeReplace(attributeChange.Name, attributeChange.ValueChanges.Select(t => t.Value).ToList<object>());
                newcsentry.AttributeChanges.Add(newAttributeChange);
            }

            Logger.WriteLine("Converted CSEntryChangeAdd to CSEntryChangeUpdate", LogLevel.Debug);
            return newcsentry;
        }

        /// <summary>
        /// Converts a CSEntryChange from an 'replace' modification type to 'update', converting the contained AttributeChanges from 'add' to 'replace', and adding in the appropriate 'delete' AttributeChanges
        /// </summary>
        /// <param name="csentry">The CSEntryChange to modify</param>
        /// <returns>A copy of the original CSEntryChange with the modification type set to 'update'</returns>
        public static CSEntryChange ConvertCSEntryChangeReplaceToUpdate(this CSEntryChange csentry, MAObjectHologram maObject)
        {
            CSEntryChange newcsentry = CSEntryChange.Create();
            newcsentry.ObjectModificationType = ObjectModificationType.Update;
            newcsentry.ObjectType = csentry.ObjectType;
            newcsentry.DN = csentry.DN;
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass(csentry.ObjectType);

            foreach (AttributeChange attributeChange in csentry.AttributeChanges)
            {
                AttributeChange newAttributeChange = AttributeChange.CreateAttributeReplace(attributeChange.Name, attributeChange.ValueChanges.Select(t => t.Value).ToList<object>());
                newcsentry.AttributeChanges.Add(newAttributeChange);
            }

            foreach (AcmaSchemaAttribute attribute in objectClass.Attributes.Where(t => !t.IsBuiltIn && !t.IsInheritedInClass(objectClass.Name)))
            {
                if (!csentry.AttributeChanges.Any(t => t.Name == attribute.Name))
                {
                    if (maObject.HasAttribute(attribute))
                    {
                        newcsentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(attribute.Name));
                    }
                }
            }

            Logger.WriteLine("Converted CSEntryChangeReplace to CSEntryChangeUpdate", LogLevel.Debug);
            return newcsentry;
        }

        public static IEnumerable<CSEntryChange> SplitReferenceUpdatesFromCSEntryChanges(this IEnumerable<CSEntryChange> csentries)
        {
            List<CSEntryChange> newList = new List<CSEntryChange>();
            foreach (CSEntryChange csentry in csentries)
            {
                if (csentry.ObjectModificationType == ObjectModificationType.Add)
                {
                    newList.AddRange(SplitReferenceUpdatesFromCSEntryChangeAdd(csentry));
                }
                else
                {
                    newList.Add(csentry);
                }
            }

            return newList;
        }

        public static IEnumerable<CSEntryChange> SplitReferenceUpdatesFromCSEntryChangeAdd(this CSEntryChange csentry)
        {
            if (csentry.ObjectModificationType != ObjectModificationType.Add)
            {
                throw new UnknownOrUnsupportedModificationTypeException(csentry.ObjectModificationType);
            }

            List<CSEntryChange> updatedCSEntryChanges = new List<CSEntryChange>();
            updatedCSEntryChanges.Add(csentry);

            CSEntryChange newChange = null;

            foreach (AttributeChange attributeChange in csentry.AttributeChanges.ToList())
            {
                AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(attributeChange.Name);

                if (attribute.Type == ExtendedAttributeType.Reference)
                {
                    csentry.AttributeChanges.Remove(attributeChange);
                    if (newChange == null)
                    {
                        newChange = CSEntryChange.Create();
                        newChange.ObjectModificationType = ObjectModificationType.Update;
                        newChange.DN = csentry.DN;
                        newChange.ObjectType = csentry.ObjectType;
                    }

                    newChange.AttributeChanges.Add(attributeChange);
                }
                else
                {
                    continue;
                }
            }

            if (newChange != null)
            {
                updatedCSEntryChanges.Add(newChange);
            }

            return updatedCSEntryChanges;
        }
    }
}
