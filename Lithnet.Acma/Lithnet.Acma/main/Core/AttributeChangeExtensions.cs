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
    using System.Collections.ObjectModel;

    /// <summary>
    /// Contains methods to manage CSEntryChanges
    /// </summary>
    public static class AttributeChangeExtensions
    {
        /// <summary>
        /// Processes an attribute delete request on the specified CSEntryChange
        /// </summary>
        /// <param name="csentry">The CSEntryChange to process</param>
        /// <param name="attribute">The attribute associated with this request</param>
        public static void DeleteAttribute(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute)
        {
            if (attributeChanges.Contains(attribute.Name))
            {
                ConvertAttributeChangeToDelete(attributeChanges, objectModificationType, attribute, attributeChanges[attribute.Name]);
            }
            else
            {
                CreateAttributeChangeDelete(attributeChanges, objectModificationType, attribute);
            }
        }

        public static void AddOrReplaceAttributeChange(this KeyedCollection<string, AttributeChange> attributeChanges, AttributeChange newAttributeChange)
        {
            if (!attributeChanges.Contains(newAttributeChange.Name))
            {
                attributeChanges.Add(newAttributeChange);
            }
            else
            {
                attributeChanges.Remove(newAttributeChange.Name);
                attributeChanges.Add(newAttributeChange);
            }
        }

        /// <summary>
        /// Processes an attribute replace request on the specified CSEntryChange
        /// </summary>
        /// <param name="csentry">The CSEntryChange to process</param>
        /// <param name="attribute">The attribute associated with this request</param>
        /// <param name="value">The new value to assign to the attribute</param>
        public static void ReplaceAttribute(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, object value)
        {
            ReplaceAttribute(attributeChanges, objectModificationType, attribute, new List<object>() { value });
        }

        /// <summary>
        /// Processes an attribute replace request on the specified CSEntryChange
        /// </summary>
        /// <param name="csentry">The CSEntryChange to process</param>
        /// <param name="attribute">The attribute associated with this request</param>
        /// <param name="values">The new values to assign to the attribute</param>
        public static void ReplaceAttribute(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, IList<object> values)
        {
            if (values == null || values.Count == 0)
            {
                DeleteAttribute(attributeChanges, objectModificationType, attribute);
                return;
            }

            if (!attribute.IsMultivalued && values.Count > 1)
            {
                throw new TooManyValuesException();
            }

            TypeConverter.ThrowOnAnyInvalidDataType(values);

            if (attributeChanges.Contains(attribute.Name))
            {
                ConvertAttributeChangeToReplace(attributeChanges, objectModificationType, attribute, values, attributeChanges[attribute.Name]);
            }
            else
            {
                CreateAttributeChangeReplace(attributeChanges, objectModificationType, attribute, values);
            }
        }

        /// <summary>
        /// Processes an attribute update request on the specified CSEntryChange
        /// </summary>
        /// <param name="csentry">The CSEntryChange to process</param>
        /// <param name="attribute">The attribute associated with this request</param>
        /// <param name="valueChanges">The value changes to apply to the attribute</param>
        public static void UpdateAttribute(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, IList<ValueChange> valueChanges)
        {
            if (valueChanges == null)
            {
                throw new ArgumentNullException("valueChanges");
            }

            if (!attribute.IsMultivalued && valueChanges.Count(t => t.ModificationType == ValueModificationType.Add) > 1)
            {
                throw new TooManyValuesException();
            }

            valueChanges = RemoveDuplicateValueChanges(attribute, valueChanges);

            if (attributeChanges.Contains(attribute.Name))
            {
                AttributeChange existingChange = attributeChanges[attribute.Name];
                switch (existingChange.ModificationType)
                {
                    case AttributeModificationType.Add:
                        ConvertAttributeChangeUpdateFromAdd(attributeChanges, objectModificationType, attribute, valueChanges, existingChange);
                        break;

                    case AttributeModificationType.Delete:
                        ConvertAttributeChangeUpdateFromDelete(attributeChanges, objectModificationType, attribute, valueChanges, existingChange);
                        break;

                    case AttributeModificationType.Replace:
                        ConvertAttributeChangeUpdateFromReplace(attributeChanges, objectModificationType, attribute, valueChanges, existingChange);
                        break;

                    case AttributeModificationType.Update:
                        ConvertAttributeChangeUpdateFromUpdate(attributeChanges, objectModificationType, attribute, valueChanges, existingChange);
                        break;

                    case AttributeModificationType.Unconfigured:
                    default:
                        throw new UnknownOrUnsupportedModificationTypeException(existingChange.ModificationType);
                }
            }
            else
            {
                CreateAttributeChangeUpdate(attributeChanges, objectModificationType, attribute, valueChanges);
            }

            RemoveAttributeChangeIfEmpty(attributeChanges, attribute.Name);
        }

        private static void RemoveAttributeChangeIfEmpty(this KeyedCollection<string, AttributeChange> attributeChanges, string attributeName)
        {
            if (attributeChanges.Contains(attributeName))
            {
                AttributeChange change = attributeChanges[attributeName];

                if (change.ValueChanges.Count == 0 && change.ModificationType != AttributeModificationType.Delete)
                {
                    attributeChanges.Remove(attributeName);
                }
            }
        }

        /// <summary>
        /// Creates a new AttributeChange of type 'update'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply the AttributeChange to</param>
        /// <param name="attribute">The attribute to create the AttributeChange for</param>
        /// <param name="valueChanges">The value changes to apply</param>
        private static void CreateAttributeChangeUpdate(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, IList<ValueChange> valueChanges)
        {
            switch (objectModificationType)
            {
                case ObjectModificationType.Add:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, valueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList<object>()));
                    break;

                case ObjectModificationType.Delete:
                    throw new DeletedObjectModificationException();

                case ObjectModificationType.Replace:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, valueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList<object>()));
                    break;

                case ObjectModificationType.Update:
                    attributeChanges.Add(AttributeChange.CreateAttributeUpdate(attribute.Name, valueChanges));
                    break;

                case ObjectModificationType.Unconfigured:
                case ObjectModificationType.None:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(objectModificationType);
            }
        }

        /// <summary>
        /// Converts an AttributeChange of type 'add' to a new AttributeChange of type 'update'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply the AttributeChange to</param>
        /// <param name="attribute">The attribute to create the AttributeChange for</param>
        /// <param name="valueChanges">The value changes to apply</param>
        /// <param name="existingChange">The existing attribute change that was found on the CSEntryChange</param>
        private static void ConvertAttributeChangeUpdateFromAdd(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, IList<ValueChange> valueChanges, AttributeChange existingChange)
        {
            IList<ValueChange> mergedList = MergeValueChangeLists(attribute, existingChange.ValueChanges, valueChanges);

            //if (mergedList.Count == 0)
            //{
            //    return;
            //}

            //if (mergedList.ContainsSameElements(attributeChanges[attribute.Name].ValueChanges) && attributeChanges[attribute.Name].ModificationType == AttributeModificationType.Update)
            //{
            //    return;
            //}

            attributeChanges.Remove(existingChange);

            switch (objectModificationType)
            {
                case ObjectModificationType.Add:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, mergedList.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList()));
                    break;

                case ObjectModificationType.Delete:
                    throw new DeletedObjectModificationException();

                case ObjectModificationType.Replace:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, mergedList.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList()));
                    break;

                case ObjectModificationType.Update:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, mergedList.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList()));
                    break;

                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(objectModificationType);
            }
        }

        /// <summary>
        /// Converts an AttributeChange of type 'delete' to a new AttributeChange of type 'update'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply the AttributeChange to</param>
        /// <param name="attribute">The attribute to create the AttributeChange for</param>
        /// <param name="valueChanges">The value changes to apply</param>
        /// <param name="existingChange">The existing attribute change that was found on the CSEntryChange</param>
        private static void ConvertAttributeChangeUpdateFromDelete(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, IList<ValueChange> valueChanges, AttributeChange existingChange)
        {
            attributeChanges.Remove(existingChange);
            IList<object> valueAdds = valueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();

            switch (objectModificationType)
            {
                case ObjectModificationType.Add:
                    throw new InvalidOperationException("The attribute change type is not valid for the object modification type");

                case ObjectModificationType.Delete:
                    throw new DeletedObjectModificationException();

                case ObjectModificationType.Replace:
                    throw new InvalidOperationException("The attribute change type is not valid for the object modification type");

                case ObjectModificationType.Update:
                    attributeChanges.Add(AttributeChange.CreateAttributeReplace(attribute.Name, valueAdds));
                    break;

                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(objectModificationType);
            }
        }

        /// <summary>
        /// Converts an AttributeChange of type 'replace' to a new AttributeChange of type 'update'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply the AttributeChange to</param>
        /// <param name="attribute">The attribute to create the AttributeChange for</param>
        /// <param name="valueChanges">The value changes to apply</param>
        /// <param name="existingChange">The existing attribute change that was found on the CSEntryChange</param>
        private static void ConvertAttributeChangeUpdateFromReplace(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, IList<ValueChange> valueChanges, AttributeChange existingChange)
        {
            attributeChanges.Remove(existingChange);
            IList<ValueChange> mergedList = MergeValueChangeLists(attribute, existingChange.ValueChanges, valueChanges);
            IList<object> valueAdds = mergedList.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList();

            switch (objectModificationType)
            {
                case ObjectModificationType.Add:
                    throw new InvalidOperationException("The attribute change type is not valid for the object modification type");

                case ObjectModificationType.Delete:
                    throw new DeletedObjectModificationException();

                case ObjectModificationType.Replace:
                    throw new InvalidOperationException("The attribute change type is not valid for the object modification type");

                case ObjectModificationType.Update:
                    attributeChanges.Add(AttributeChange.CreateAttributeReplace(attribute.Name, valueAdds));
                    break;

                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(objectModificationType);
            }
        }

        /// <summary>
        /// Converts an AttributeChange of type 'update' to a new AttributeChange of type 'update'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply the AttributeChange to</param>
        /// <param name="attribute">The attribute to create the AttributeChange for</param>
        /// <param name="valueChanges">The value changes to apply</param>
        /// <param name="existingChange">The existing attribute change that was found on the CSEntryChange</param>
        private static void ConvertAttributeChangeUpdateFromUpdate(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, IList<ValueChange> valueChanges, AttributeChange existingChange)
        {
            IList<ValueChange> mergedList = MergeValueChangeLists(attribute, existingChange.ValueChanges, valueChanges);
            attributeChanges.Remove(existingChange);

            switch (objectModificationType)
            {
                case ObjectModificationType.Add:
                    throw new InvalidOperationException("The attribute change type is not valid for the object modification type");

                case ObjectModificationType.Delete:
                    throw new DeletedObjectModificationException();

                case ObjectModificationType.Replace:
                    throw new InvalidOperationException("The attribute change type is not valid for the object modification type");

                case ObjectModificationType.Update:
                    attributeChanges.Add(AttributeChange.CreateAttributeUpdate(attribute.Name, mergedList));
                    break;

                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(objectModificationType);
            }
        }

        /// <summary>
        /// Creates a new AttributeChange of type 'replace'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply the AttributeChange to</param>
        /// <param name="attribute">The attribute to create the AttributeChange for</param>
        /// <param name="values">The values to assign</param>
        private static void CreateAttributeChangeReplace(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, IList<object> values)
        {
            TypeConverter.ThrowOnAnyInvalidDataType(values);

            switch (objectModificationType)
            {
                case ObjectModificationType.Add:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, values));
                    break;

                case ObjectModificationType.Delete:
                    throw new DeletedObjectModificationException();

                case ObjectModificationType.Replace:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, values));
                    break;

                case ObjectModificationType.Update:
                    attributeChanges.Add(AttributeChange.CreateAttributeReplace(attribute.Name, values));
                    break;

                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(objectModificationType);
            }
        }

        /// <summary>
        /// Converts an existing AttributeChange to a new AttributeChange of type 'replace'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply the AttributeChange to</param>
        /// <param name="attribute">The attribute to create the AttributeChange for</param>
        /// <param name="values">The values to assign</param>
        /// <param name="existingChange">The existing attribute change that was found on the CSEntryChange</param>
        private static void ConvertAttributeChangeToReplace(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, IList<object> values, AttributeChange existingChange)
        {
            TypeConverter.ThrowOnAnyInvalidDataType(values);

            attributeChanges.Remove(existingChange);

            switch (objectModificationType)
            {
                case ObjectModificationType.Add:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, values));
                    break;

                case ObjectModificationType.Delete:
                    throw new DeletedObjectModificationException();

                case ObjectModificationType.Replace:
                    attributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, values));
                    break;

                case ObjectModificationType.Update:
                    attributeChanges.Add(AttributeChange.CreateAttributeReplace(attribute.Name, values));
                    break;

                case ObjectModificationType.Unconfigured:
                case ObjectModificationType.None:
                default:
                    break;
            }
        }

        /// <summary>
        /// Converts an existing AttributeChange to a new AttributeChange of type 'delete'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply the AttributeChange to</param>
        /// <param name="attribute">The attribute to create the AttributeChange for</param>
        /// <param name="existingChange">The existing attribute change that was found on the CSEntryChange</param>
        private static void ConvertAttributeChangeToDelete(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute, AttributeChange existingChange)
        {
            switch (objectModificationType)
            {
                case ObjectModificationType.Add:
                    attributeChanges.Remove(existingChange);
                    break;

                case ObjectModificationType.Delete:
                    throw new DeletedObjectModificationException();

                case ObjectModificationType.Replace:
                    attributeChanges.Remove(existingChange);
                    break;

                case ObjectModificationType.Update:
                    switch (existingChange.ModificationType)
                    {
                        case AttributeModificationType.Add:
                            attributeChanges.Remove(existingChange);
                            break;

                        case AttributeModificationType.Delete:
                            break;

                        case AttributeModificationType.Replace:
                            attributeChanges.Remove(existingChange);
                            Logger.WriteLine("Removed " + existingChange.Name);
                            attributeChanges.Add(AttributeChange.CreateAttributeDelete(attribute.Name));
                            Logger.WriteLine("Added " + attribute.Name);
                            break;

                        case AttributeModificationType.Update:
                            attributeChanges.Remove(existingChange);
                            attributeChanges.Add(AttributeChange.CreateAttributeDelete(attribute.Name));
                            break;

                        case AttributeModificationType.Unconfigured:
                        default:
                            break;
                    }

                    break;

                case ObjectModificationType.Unconfigured:
                case ObjectModificationType.None:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(objectModificationType);
            }
        }

        /// <summary>
        /// Creates a new AttributeChange of type 'delete'
        /// </summary>
        /// <param name="csentry">The CSEntryChange to apply the AttributeChange to</param>
        /// <param name="attribute">The attribute to create the AttributeChange for</param>
        private static void CreateAttributeChangeDelete(this KeyedCollection<string, AttributeChange> attributeChanges, ObjectModificationType objectModificationType, AcmaSchemaAttribute attribute)
        {
            switch (objectModificationType)
            {
                case ObjectModificationType.Add:
                    // This is a new object, so there is nothing to delete
                    break;

                case ObjectModificationType.Delete:
                    throw new DeletedObjectModificationException();

                case ObjectModificationType.Replace:
                    // This object is being replaced, so the absence of an attribute add implies that any existing attribute values shall be deleted
                    break;

                case ObjectModificationType.Update:
                    attributeChanges.Add(AttributeChange.CreateAttributeDelete(attribute.Name));
                    break;

                case ObjectModificationType.None:
                case ObjectModificationType.Unconfigured:
                default:
                    throw new UnknownOrUnsupportedModificationTypeException(objectModificationType);
            }
        }

        /// <summary>
        /// Merges two lists of ValueChange objects, canceling out any matching Add/Delete pairs and duplicate values
        /// </summary>
        /// <param name="attribute">The attribute that the ValueChanges represent</param>
        /// <param name="sourceList">The original list of ValueChanges</param>
        /// <param name="newValues">The new list of ValueChanges</param>
        /// <returns>A new list containing the merged values from both lists</returns>
        public static IList<ValueChange> MergeValueChangeLists(AcmaSchemaAttribute attribute, IList<ValueChange> sourceList, IList<ValueChange> newValues)
        {
            IList<ValueChange> newList = new List<ValueChange>(sourceList);

            if (newValues.Count == 0)
            {
                return newList;
            }

            ValueChangeComparer valueComparer = new ValueChangeComparer(attribute, false);

            IList<ValueChange> nonDuplicatedNewList = RemoveDuplicateValueChanges(attribute, newValues);

            foreach (var newChange in nonDuplicatedNewList)
            {
                ValueChange changeInSource = sourceList.FirstOrDefault(t => valueComparer.Equals(newChange, t));

                if (changeInSource == null)
                {
                    newList.Add(newChange);
                }
                else
                {
                    if (changeInSource.ModificationType == ValueModificationType.Add)
                    {
                        if (newChange.ModificationType == ValueModificationType.Delete)
                        {
                            Logger.WriteLine("Canceled attribute add due to subsequent delete: {0} - {1}", LogLevel.Debug, attribute.Name, changeInSource.Value.ToSmartStringOrNull());
                            newList.Remove(changeInSource);
                        }
                        else if (newChange.ModificationType == ValueModificationType.Add)
                        {
                            Logger.WriteLine("Duplicate add detected: {0} - {1}", LogLevel.Debug, attribute.Name, changeInSource.Value.ToSmartStringOrNull());
                            continue;
                        }
                    }
                    else if (changeInSource.ModificationType == ValueModificationType.Delete)
                    {
                        if (newChange.ModificationType == ValueModificationType.Add)
                        {
                            Logger.WriteLine("Canceled attribute delete due to subsequent add: {0} - {1}", LogLevel.Debug, attribute.Name, changeInSource.Value.ToSmartStringOrNull());
                            newList.Remove(changeInSource);
                            newList.Add(newChange);
                        }
                        else if (newChange.ModificationType == ValueModificationType.Delete)
                        {
                            Logger.WriteLine("Duplicate delete detected: {0} - {1}", LogLevel.Debug, attribute.Name, changeInSource.Value.ToSmartStringOrNull());
                            continue;
                        }
                    }
                }
            }

            return newList;
        }

        public static IList<ValueChange> RemoveDuplicateValueChanges(AcmaSchemaAttribute attribute, IList<ValueChange> changes)
        {
            ValueChangeComparer changeComparer = new ValueChangeComparer(attribute, true);
            return changes.Distinct(changeComparer).ToList();
        }
    }
}
