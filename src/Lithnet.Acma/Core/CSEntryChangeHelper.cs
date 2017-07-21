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
    using Lithnet.MetadirectoryServices;
    using Lithnet.Acma.DataModel;
    using Microsoft.MetadirectoryServices.DetachedObjectModel;

    /// <summary>
    /// Contains methods to manage CSEntryChanges
    /// </summary>
    public static class CSEntryChangeExtensions
    {
        public static CSEntryChange ToCSEntryChange(this MAObjectHologram hologram)
        {
            return hologram.ToCSEntryChange(hologram.ObjectClass.Attributes);
        }

        public static CSEntryChange ToCSEntryChange(this MAObjectHologram hologram, SchemaType type)
        {
            IEnumerable<AcmaSchemaAttribute> requiredAttributes = type.Attributes.Where(t => !t.Name.Equals("objectId", StringComparison.CurrentCultureIgnoreCase)).Select(u => ActiveConfig.DB.GetAttribute(u.Name));

            return hologram.ToCSEntryChange(requiredAttributes);
        }

        public static CSEntryChange ToCSEntryChange(this MAObjectHologram hologram, IEnumerable<AcmaSchemaAttribute> requiredAttributes)
        {
            string objectClassName = hologram.ObjectClass == null ? hologram.DeltaObjectClassName : hologram.ObjectClass.Name;

            if (objectClassName == null)
            {
                throw new ArgumentNullException("objectClassName", string.Format("The object class for the object {0} was not present", hologram.ObjectID));
            }

            if (hologram.DeltaChangeType != "delete")
            {
                hologram.PreLoadAVPs();
            }

            return GetCSEntry(hologram, requiredAttributes);
        }

        /// <summary>
        /// Creates a CSEntryChange for the specified MAObjectHologram
        /// </summary>
        /// <param name="maObject">The MAObjectHologram to construct the CSEntry for</param>
        /// <returns>The newly created CSEntryChange</returns>
        private static CSEntryChange GetCSEntry(MAObjectHologram hologram, IEnumerable<AcmaSchemaAttribute> requiredAttributes)
        {
            CSEntryChange csentry = CSEntryChange.Create();

            switch (hologram.DeltaChangeType)
            {
                case null:
                case "add":
                    csentry.ObjectModificationType = ObjectModificationType.Add;
                    csentry.ObjectType = hologram.ObjectClass.Name;
                    csentry.DN = hologram.ObjectID.ToString();
                    csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectId", hologram.ObjectID.ToString()));
                    GetObject(hologram, csentry, requiredAttributes.Where(t => t.Name != "objectId"));
                    break;

                case "delete":
                    csentry.ObjectModificationType = ObjectModificationType.Delete;
                    csentry.DN = hologram.ObjectID.ToString();
                    csentry.ObjectType = hologram.ObjectClass == null ? hologram.DeltaObjectClassName : hologram.ObjectClass.Name;
                    csentry.AnchorAttributes.Add(AnchorAttribute.Create("objectId", hologram.ObjectID.ToString()));
                    break;

                case "modify":
                case "attrmodify":
                    csentry.ObjectModificationType = ObjectModificationType.Replace;
                    csentry.ObjectType = hologram.ObjectClass.Name;
                    csentry.DN = hologram.ObjectID.ToString();
                    csentry.AnchorAttributes.Add(AnchorAttribute.Create("objectId", hologram.ObjectID.ToString()));
                    GetObject(hologram, csentry, requiredAttributes);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("The change type {0} is unknown", hologram.DeltaChangeType));
            }

            return csentry;
        }


        /// <summary>
        /// Contributes to a CSEntryChange for the specified MA_Delta_Object by populating newly added object and its attributes
        /// </summary>
        /// <param name="maObject">The MAObject to construct the CSEntry for</param>
        /// <param name="csentry">The CSEntryChange object to contribute to</param>
        private static void GetObject(MAObjectHologram maObject, CSEntryChange csentry, IEnumerable<AcmaSchemaAttribute> requiredAttributes)
        {
            try
            {
                foreach (AcmaSchemaAttribute maAttribute in requiredAttributes)
                {
                    List<object> values = new List<object>();
                    AttributeValues dbvalues = maObject.GetAttributeValues(maAttribute);
                    values.AddRange(dbvalues.Where(t => !t.IsNull).Select(t => t.Value));

                    if (values.Count > 0)
                    {
                        if (maAttribute.Type == ExtendedAttributeType.Reference)
                        {
                            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(maAttribute.Name, values.Select(t => t.ToString()).ToList<object>()));
                        }
                        else if (maAttribute.Type == ExtendedAttributeType.DateTime)
                        {
                            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(maAttribute.Name, values.Select(t => ((DateTime)t).ToResourceManagementServiceDateFormat()).ToList<object>()));
                        }
                        else
                        {
                            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(maAttribute.Name, values));
                        }
                    }
                }
            }
            catch (SafetyRuleViolationException ex)
            {
                csentry.ErrorCodeImport = MAImportError.ImportErrorCustomContinueRun;
                csentry.ErrorName = ex.Message;
                csentry.ErrorDetail = ex.Message + "\n" + ex.StackTrace;
            }
        }


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
            csentry.DN = maObject.ObjectID.ToString();
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

            if (csentry.ErrorCodeImport == MAImportError.Success)
            {
                MAStatistics.AddImportOperation();
            }
            else
            {
                MAStatistics.AddImportError();
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
