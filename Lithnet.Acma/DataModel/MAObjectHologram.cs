// -----------------------------------------------------------------------
// <copyright file="MAObjectHologram.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using Lithnet.Acma.DataModel;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Microsoft.MetadirectoryServices.DetachedObjectModel;
    using Lithnet.Common.ObjectModel;
    using System.Collections.Specialized;
    using System.Text;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an MAObject from the ACMA database, overlayed with a CSEntryChange containing changes to apply to the underlying database.
    /// The MAObjectHologram class allows constructors to access both previous and new values of each attribute, and supports committing the AttributeChanges 
    /// in a CSEntryChange to the database as an atomic operation. Changes to attributes are automatically added to the attached CSEntryChange, and only applied to the 
    /// underlying database when the Commit operation is called. Requests for attribute values will always return the proposed value if present, otherwise it will return the 
    /// current value in the database. Certain methods support requesting either the proposed or current version of an attribute.
    /// </summary>
    [Serializable]
    [KnownType(typeof(List<string>))]
    public class MAObjectHologram : MAObject, ISerializable
    {
        /// <summary>
        /// Gets or sets a value indicating if a referenced object was not found during processing of constructors for this object
        /// </summary>
        private bool referenceRetryRequired;

        private string objectDisplayText;

        private bool disposed = false;

        private TriggerEvents acmaModificationType = TriggerEvents.Unconfigured;

        /// <summary>
        /// The local hologram cache allows this object to speed up repeating calls to objects it references
        /// It should be valid only for the life of this object
        /// Don't try to use this as a static cache for all holograms, as this this not the source of those objects
        /// </summary>
        private Dictionary<Guid, MAObjectHologram> hologramCache;

        private ObjectModificationType CSEntryModificationType
        {
            get
            {
                switch (this.AcmaModificationType)
                {
                    case TriggerEvents.Unconfigured:
                        return ObjectModificationType.Unconfigured;

                    case TriggerEvents.Add:
                        return ObjectModificationType.Add;

                    case TriggerEvents.Update:
                        return ObjectModificationType.Update;

                    case TriggerEvents.Delete:
                        if (this.ObjectClass.AllowResurrection)
                        {
                            return ObjectModificationType.Update;
                        }
                        else
                        {
                            return ObjectModificationType.Delete;
                        }

                    case TriggerEvents.Undelete:
                        return ObjectModificationType.Update;

                    case TriggerEvents.None:
                        return ObjectModificationType.None;

                    default:
                        throw new UnknownOrUnsupportedModificationTypeException();
                }
            }
        }

        public TriggerEvents AcmaModificationType
        {
            get
            {
                return this.acmaModificationType;
            }
            set
            {
                if (this.acmaModificationType == value)
                {
                    return;
                }

                if (this.acmaModificationType != TriggerEvents.Unconfigured && this.acmaModificationType != TriggerEvents.None)
                {
                    throw new InvalidOperationException("Cannot set the object modification type as it has already been set");
                }

                this.acmaModificationType = value;
            }
        }

        internal AttributeChangeCollection InternalAttributeChanges { get; private set; }

        public ReadOnlyCollection<AttributeChange> AttributeChanges
        {
            get
            {
                return this.InternalAttributeChanges.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// Initializes a new instance of the MAObjectHologram class
        /// </summary>
        /// <param name="row">The data row for the MAObject</param>
        /// <param name="adapter">The data adapter for the object</param>
        /// <param name="dc">The data context in use on this thread</param>
        public MAObjectHologram(DataRow row, SqlDataAdapter adapter, MADataContext dc)
            : base(row, adapter, dc)
        {
            this.AcmaModificationType = TriggerEvents.Unconfigured;

            this.hologramCache = new Dictionary<Guid, MAObjectHologram>();
            this.InternalAttributeChanges = new AttributeChangeCollection();
            this.InternalAttributeChanges.CollectionChanged += AttributeChanges_CollectionChanged;
        }

        private void AttributeChanges_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Delete attribute changes on inheritance source delete

            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (AttributeChange attributeChange in e.NewItems)
                {
                    AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(attributeChange.Name, this.ObjectClass);

                    if (!(attributeChange is AcmaInheritedAttributeChange))
                    {
                        this.ThrowOnInheritedValueModification(attribute);
                    }

                    foreach (AcmaSchemaMapping mapping in this.ObjectClass.InheritedAttributes.Where(t =>
                        t.InheritanceSourceAttribute.Name == attributeChange.Name))
                    {
                        this.CreateInheritedAttributeChangeOnReferenceChange(attributeChange, mapping);
                    }
#if DEBUG
                    string valueChanges = string.Empty;

                    if (attributeChange.ModificationType == AttributeModificationType.Update)
                    {
                        valueChanges = attributeChange.ValueChanges.Select(t => string.Format("{0}:{1}", t.ModificationType.ToSmartString().ToLower(), t.Value.ToSmartString())).ToCommaSeparatedString();
                    }
                    else if (attributeChange.ModificationType == AttributeModificationType.Replace || attributeChange.ModificationType == AttributeModificationType.Add)
                    {
                        valueChanges = attributeChange.ValueChanges.Select(t => string.Format("{0}", t.Value.ToSmartString())).ToCommaSeparatedString();
                    }

                    if (valueChanges != string.Empty)
                    {
                        valueChanges = " -> " + valueChanges;
                    }

                    Logger.WriteLine(
                        "Added attribute {0} on {{{1}}}{2}",
                        LogLevel.Debug,
                        attributeChange.ModificationType.ToSmartString().ToLower(),
                        attributeChange.Name,
                        valueChanges);
#endif
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
#if DEBUG
                foreach (AttributeChange attributeChange in e.OldItems)
                {
                    if (attributeChange.ModificationType == AttributeModificationType.Delete)
                    {
                        Logger.WriteLine(
                       "Removed attribute {0} on {{{1}}}",
                       LogLevel.Debug,
                       attributeChange.ModificationType.ToSmartString().ToLower(),
                       attributeChange.Name,
                       attributeChange.ValueChanges.Select(t => string.Format("{0}:{1}", t.ModificationType.ToSmartString(), t.Value.ToSmartString())).ToCommaSeparatedString());

                    }
                    else
                    {

                    }
                    Logger.WriteLine(
                        "Removed attribute {0} on {{{1}}} -> {2}",
                        LogLevel.Debug,
                        attributeChange.ModificationType.ToSmartString().ToLower(),
                        attributeChange.Name,
                        attributeChange.ValueChanges.Select(t => string.Format("{0}:{1}", t.ModificationType.ToSmartString(), t.Value.ToSmartString())).ToCommaSeparatedString());
                }
#endif
            }
        }

        private void CreateInheritedAttributeChangeOnReferenceChange(AttributeChange attributeChange, AcmaSchemaMapping mapping)
        {
            AcmaInheritedAttributeChange newchange = new AcmaInheritedAttributeChange(
                mapping.Attribute,
                attributeChange.ModificationType == AttributeModificationType.Update ? AttributeModificationType.Replace : attributeChange.ModificationType,
                null);

            if (this.InternalAttributeChanges.Contains(newchange.Name))
            {
                this.InternalAttributeChanges.Remove(newchange.Name);
            }

            this.InternalAttributeChanges.Add(newchange);

            Logger.WriteLine(
                "Created InheritedAttributeChange for attribute {{{2}}} triggered by a change on reference attribute {{{0}->{1}}}",
                LogLevel.Debug,
                mapping.InheritanceSourceAttribute.Name,
                mapping.InheritedAttribute.Name,
                mapping.Attribute.Name);
        }

        /// <summary>
        /// Gets a value indicating whether a referenced object was not found during processing of constructors for this object
        /// </summary>
        internal bool ReferenceRetryRequired
        {
            get
            {
                return this.referenceRetryRequired;
            }
        }

        /// <summary>
        /// Gets the text to display when referring to this object in logs
        /// </summary>
        public string DisplayText
        {
            get
            {
                if (this.objectDisplayText == null)
                {
                    if (this.ObjectClass != null)
                    {
                        if (this.ObjectClass.HasAttribute("displayName"))
                        {
                            AcmaSchemaAttribute attribute = this.ObjectClass.GetAttribute("displayName");
                            string text = string.Empty;

                            AttributeValues values = this.GetAttributeValues(attribute);
                            if (!values.IsEmptyOrNull)
                            {
                                text = values.First().ValueString;
                            }

                            if (string.IsNullOrWhiteSpace(text))
                            {
                                this.objectDisplayText = this.ObjectID.ToString();
                            }
                            else
                            {
                                this.objectDisplayText = string.Format("{2}:{1}:{0}", this.ObjectID, text, this.ObjectClass.Name);
                            }
                        }
                        else
                        {
                            this.objectDisplayText = string.Format("{1}:{0}", this.ObjectID, this.ObjectClass.Name);
                        }
                    }

                    if (this.objectDisplayText == null)
                    {
                        this.objectDisplayText = this.ObjectID.ToString();
                    }
                }

                return this.objectDisplayText;
            }
        }

        public IEnumerable<string> ChangedAttributeNames
        {
            get
            {
                return this.InternalAttributeChanges.Select(t => t.Name);
            }
        }

        /// <summary>
        /// Gets a list of events that were raised by the triggering object
        /// </summary>
        internal IList<RaisedEvent> IncomingEvents { get; set; }

        public AttributeValues GetMVAttributeValues(AcmaSchemaAttribute attribute)
        {
            return this.GetMVAttributeValues(attribute, HologramView.Proposed);
        }

        public AttributeValues GetMVAttributeValues(AcmaSchemaAttribute attribute, HologramView view)
        {
            return this.GetAttributeValues(attribute, view);
        }

        public AttributeValue GetSVAttributeValue(AcmaSchemaAttribute attribute)
        {
            return this.GetSVAttributeValue(attribute, HologramView.Proposed);
        }

        public AttributeValue GetSVAttributeValue(AcmaSchemaAttribute attribute, HologramView view)
        {
            if (attribute.IsMultivalued)
            {
                throw new ArgumentException();
            }

            AttributeValues values = this.GetAttributeValues(attribute, view);

            return values.FirstOrDefault() ?? new AttributeValue(attribute);
        }

        /// <summary>
        /// Gets the value of a single valued attribute. If the attribute has a pending attributeChange, the new value is returned, otherwise the value from the database is returned.
        /// </summary>
        /// <param name="attribute">The single valued attribute to obtain the value for</param>
        /// <returns>The current value of the specified attribute</returns>
        public AttributeValues GetAttributeValues(AcmaSchemaAttribute attribute)
        {
            return this.GetAttributeValues(attribute, HologramView.Proposed);
        }

        /// <summary>
        /// Gets the value of a single valued attribute. If the attribute has a pending attributeChange, the new value is returned, otherwise the value from the database is returned.
        /// </summary>
        /// <param name="attribute">The single valued attribute to obtain the value for</param>
        /// <returns>The current value of the specified attribute</returns>
        public AttributeValues GetAttributeValues(AcmaSchemaAttribute attribute, HologramView view)
        {
            if (attribute.IsInheritedInClass(this.ObjectClass))
            {
                return this.GetAttributeValuesInherited(attribute);
            }

            this.ObjectClass.ThrowOnNoSuchAttribute(attribute);

            if (view == HologramView.Proposed)
            {
                return this.GetAttributeProposedValues(attribute);
            }
            else
            {
                return this.DBGetAttributeValues(attribute) ?? new InternalAttributeValues(attribute);
            }
        }

        internal void SetObjectModificationType(TriggerEvents modificationType)
        {
            this.AcmaModificationType = modificationType;
        }

        /// <summary>
        /// Creates a new CSEntryChange and attaches it to this object
        /// </summary>
        /// <param name="modificationType">The type of modification to make to the object</param>
        /// <returns>A newly created CSEntryChange</returns>
        internal void SetObjectModificationType(ObjectModificationType modificationType, bool isUndeleting)
        {
            if (isUndeleting)
            {
                this.AcmaModificationType = TriggerEvents.Undelete;
            }
            else
            {
                switch (modificationType)
                {
                    case ObjectModificationType.Add:
                        this.AcmaModificationType = TriggerEvents.Add;
                        break;

                    case ObjectModificationType.Delete:
                        this.AcmaModificationType = TriggerEvents.Delete;
                        break;

                    case ObjectModificationType.None:
                        this.AcmaModificationType = TriggerEvents.None;
                        break;

                    case ObjectModificationType.Replace:
                        this.AcmaModificationType = TriggerEvents.Update;
                        break;

                    case ObjectModificationType.Unconfigured:
                        this.AcmaModificationType = TriggerEvents.Unconfigured;
                        break;

                    case ObjectModificationType.Update:
                        this.AcmaModificationType = TriggerEvents.Update;
                        break;

                    default:
                        throw new UnknownOrUnsupportedModificationTypeException(modificationType);
                }
            }
        }

        /// <summary>
        /// Gets an enumeration of objects that are referenced by the specified attribute of this object
        /// </summary>
        /// <param name="attribute">The attribute containing the list of referenced objects</param>
        /// <returns>An enumeration of MAObjectHologram objects</returns>
        internal IEnumerable<MAObjectHologram> GetReferencedObjects(AcmaSchemaAttribute attribute)
        {
            return this.GetReferencedObjects(attribute, DataRowVersion.Proposed);
        }

        /// <summary>
        /// Gets an enumeration of objects that are referenced by the specified attribute of this object
        /// </summary>
        /// <param name="attribute">The attribute containing the list of referenced objects</param>
        /// <param name="version">The version of the data to obtain. When set to 'Proposed', then the uncommitted values from the attached CSEntryChange are returned. Otherwise the value stored in the database are returned</param>
        /// <returns>An enumeration of MAObjectHologram objects</returns>
        internal IEnumerable<MAObjectHologram> GetReferencedObjects(AcmaSchemaAttribute attribute, DataRowVersion version)
        {
            if (version == DataRowVersion.Proposed)
            {
                return this.GetReferencedObjectsProposed(attribute);
            }
            else
            {
                return this.GetReferencedObjectsCurrent(attribute);
            }
        }

        /// <summary>
        /// Deletes the values of an attribute
        /// </summary>
        /// <param name="attribute">The attribute containing the values to delete</param>
        internal void DeleteAttribute(AcmaSchemaAttribute attribute)
        {
            this.ThrowOnInheritedValueModification(attribute);
            this.ThrowOnModificationTypeNotSet();

            if (this.ShouldProcessAttributeChangeDeleteRequest(attribute))
            {
                this.InternalAttributeChanges.DeleteAttribute(this.CSEntryModificationType, attribute);
            }
        }

        /// <summary>
        /// Sets the value of an attribute to the specified value, replacing any existing values
        /// </summary>
        /// <param name="attribute">The attribute containing the values to set</param>
        /// <param name="value">The value to set</param>
        internal void SetAttributeValue(AcmaSchemaAttribute attribute, object value)
        {
            if (value == null)
            {
                this.DeleteAttribute(attribute);
                return;
            }

            TypeConverter.ThrowOnInvalidDataType(value);
            this.ThrowOnInheritedValueModification(attribute);
            this.ThrowOnModificationTypeNotSet();

            if (this.ShouldProcessAttributeChangeSetRequest(attribute, value))
            {
                this.InternalAttributeChanges.ReplaceAttribute(this.CSEntryModificationType, attribute, value);
            }
        }

        /// <summary>
        /// Sets the value of an attribute to the specified values, replacing any existing values
        /// </summary>
        /// <param name="attribute">The attribute containing the values to set</param>
        /// <param name="values">The new values to apply to the attribute</param>
        internal void SetAttributeValue(AcmaSchemaAttribute attribute, IList<object> values)
        {
            if (values == null || values.Count == 0)
            {
                this.DeleteAttribute(attribute);
                return;
            }

            if (values.Any(t => t == null))
            {
                throw new InvalidOperationException("One or more of the values in the list of value changes is null");
            }

            this.ThrowOnInheritedValueModification(attribute);
            this.ThrowOnModificationTypeNotSet();

            // TODO: need to validate or cast the values array into the correct data type.


            if (this.ShouldProcessAttributeChangeSetRequest(attribute, values))
            {
                this.InternalAttributeChanges.ReplaceAttribute(this.CSEntryModificationType, attribute, values);
            }
        }

        /// <summary>
        /// Updates the value of an attribute using the specified ValueChange objects
        /// </summary>
        /// <param name="attribute">The attribute containing the values to update</param>
        /// <param name="valueChanges">The list of ValueChange objects to apply</param>
        internal void UpdateAttributeValue(AcmaSchemaAttribute attribute, IList<ValueChange> valueChanges)
        {
            if (valueChanges.Any(t => t.Value == null))
            {
                throw new InvalidOperationException("One or more of the values in the list of value changes is null");
            }

            this.ThrowOnInheritedValueModification(attribute);
            this.ThrowOnModificationTypeNotSet();

            if (this.ShouldProcessAttributeChangeUpdateRequest(attribute, valueChanges))
            {
                this.InternalAttributeChanges.UpdateAttribute(this.CSEntryModificationType, attribute, valueChanges);
            }
        }

        /// <summary>
        /// Gets a value indicating if the specified attribute is present on the object
        /// </summary>
        /// <param name="attribute">The attribute to search for values for</param>
        /// <returns>A boolean value indicating if the attribute is present on the object</returns>
        public bool HasAttribute(AcmaSchemaAttribute attribute)
        {
            return this.HasAttribute(attribute, HologramView.Proposed);
        }

        /// <summary>
        /// Gets a value indicating if the specified attribute is present on the object
        /// </summary>
        /// <param name="attribute">The attribute to search for values for</param>
        /// <returns>A boolean value indicating if the attribute is present on the object</returns>
        public bool HasAttribute(AcmaSchemaAttribute attribute, HologramView view)
        {
            return !this.GetAttributeValues(attribute, view).IsEmptyOrNull;
        }

        /// <summary>
        /// Gets a value indicating if the specified attribute has the specified value
        /// </summary>
        /// <param name="attribute">The attribute to search for values for</param>
        /// <param name="value">The value to test</param>
        /// <returns>A boolean value indicating if the attribute and value is present on the object</returns>
        public bool HasAttributeValue(AcmaSchemaAttribute attribute, object value)
        {
            return this.HasAttributeValue(attribute, HologramView.Proposed, value);
        }

        /// <summary>
        /// Gets a value indicating if the specified attribute has the specified value
        /// </summary>
        /// <param name="attribute">The attribute to search for values for</param>
        /// <param name="value">The value to test</param>
        /// <returns>A boolean value indicating if the attribute and value is present on the object</returns>
        public bool HasAttributeValue(AcmaSchemaAttribute attribute, HologramView view, object value)
        {
            TypeConverter.ThrowOnInvalidDataType(value);

            return this.GetAttributeValues(attribute, view).HasValue(value);
        }

        public void ProcessEvents(IList<RaisedEvent> events)
        {
            this.SetObjectModificationType(ObjectModificationType.Update, false);
            this.CommitCSEntryChange(events);
        }

        /// <summary>
        /// Discards any attached CSEntryChange, and any pending changes to this object along with it
        /// </summary>
        internal void DiscardPendingChanges()
        {
            this.InternalAttributeChanges.Clear();
            this.acmaModificationType = TriggerEvents.Unconfigured;
            this.Rollback();
        }

        /// <summary>
        /// Commits the attribute changes in the attached CSEntryChange to the database
        /// </summary>
        internal void CommitCSEntryChange()
        {
            this.CommitCSEntryChange(null, null, null, null, 0);
        }

        internal void CommitCSEntryChange(IList<string> constructors)
        {
            this.CommitCSEntryChange(null, null, null, constructors);
        }

        internal void CommitCSEntryChange(IList<string> constructors, IList<RaisedEvent> incomingEvents)
        {
            this.CommitCSEntryChange(null, null, incomingEvents, constructors);
        }

        internal void CommitCSEntryChange(IList<AttributeChange> attributeChanges)
        {
            this.CommitCSEntryChange(null, attributeChanges);
        }

        internal void CommitCSEntryChange(IList<AttributeChange> attributeChanges, IList<RaisedEvent> incomingEvents)
        {
            this.CommitCSEntryChange(null, attributeChanges, incomingEvents);
        }

        internal void CommitCSEntryChange(CSEntryChange csentry, bool isUndeleting)
        {
            this.SetObjectModificationType(csentry.ObjectModificationType, isUndeleting);
            this.CommitCSEntryChange(null, csentry.AttributeChanges);
        }

        internal void CommitCSEntryChange(IList<RaisedEvent> incomingEvents)
        {
            this.CommitCSEntryChange(null, null, incomingEvents);
        }

        /// <summary>
        /// An internal method used for unit testing only
        /// </summary>
        /// <param name="csentry"></param>
        internal void AttachCSEntryChange(CSEntryChange csentry)
        {
            this.SetObjectModificationType(csentry.ObjectModificationType, false);
            this.AddAttributeChanges(csentry.AttributeChanges);
        }

        /// <summary>
        /// Commits the attribute changes in the attached CSEntryChange to the database
        /// </summary>
        /// <param name="triggeringObject">The foreign object that is triggering the commit operation</param>
        /// <param name="incomingEvents">The events raised by the triggering object that should be processed by this objective</param>
        /// <param name="depth">The current level of the commit depth</param>
        private void CommitCSEntryChange(MAObjectHologram triggeringObject = null, IList<AttributeChange> attributeChanges = null, IList<RaisedEvent> incomingEvents = null, IList<string> constructors = null, int depth = 0)
        {
            depth++;

            if (depth > ActiveConfig.CommitDepth)
            {
                throw new CircularUpdateException("The recursive commit depth has been exceeded");
            }

            try
            {
                Logger.IncreaseIndent();
                Logger.WriteSeparatorLine('>', true);

                if (triggeringObject == null)
                {
                    Logger.WriteLine(string.Format("Processing CSEntryChange for object {0} ", this.DisplayText));

                    Logger.WriteLine(
                        "Change details (this object): Modification type: {0}; Changed attributes: {1}",
                        this.AcmaModificationType.ToString(),
                        this.ChangedAttributeNames.ToCommaSeparatedString());
                }
                else
                {
                    Logger.WriteLine(string.Format(
                        "Processing CSEntryChange for object {0} triggered by {1}",
                       this.DisplayText,
                        triggeringObject.DisplayText));

                    Logger.WriteLine(
                        "Change details (this object): Modification type: {0}; Changed attributes: {1}",
                        this.AcmaModificationType.ToString(),
                        this.ChangedAttributeNames.ToCommaSeparatedString());

                    Logger.WriteLine(
                        "Change details (triggering object): Modification type: {0}; Changed attributes: {1}",
                        triggeringObject.AcmaModificationType.ToString(),
                        triggeringObject.ChangedAttributeNames.ToCommaSeparatedString());
                }

                if (attributeChanges != null && attributeChanges.Count > 0)
                {
                    this.AddAttributeChanges(attributeChanges);
                }

                if (incomingEvents != null)
                {
                    Logger.WriteLine(string.Format("Incoming events: {0}", incomingEvents.Select(t => t.EventID).ToCommaSeparatedString()));
                    this.IncomingEvents = incomingEvents;
                }

                // Executes attribute constructors for this object 
                this.ExecuteConstructors(constructors);

                // Gets any object that have changed due to a reference link update
                List<MAObjectHologram> updatedLinks = this.UpdateReferenceBackLinks();

                // Creates the events to raise in response to this object changing
                List<RaisedEvent> raisedEvents = this.CreateExitEvents();

                // Applies the CSEntryChange to the underlying database. Note, at this point, proposed attributes become current attributes.
                this.ApplyInternalAttributeChanges();

                if (this.AcmaModificationType.HasFlag(TriggerEvents.Delete))
                {
                    // Sets the deleted time stamp so exit events and other queries looking for deleted = 0 do not find this object
                    this.SetDeletedTimeStamp();
                }

                // Writes any pending changes back to the database
                this.Commit();

                // Process shadow objects 
                this.ProcessShadowObjectLinks(depth);

                // Calls CommitCSEntryChange on any objects that had a reference link update
                this.CommitReferenceBackLinkUpdatedObjects(updatedLinks, depth);

                // Sends notification to inheritors of attribute changes on this object
                this.CreateAndSendAttributeChangesToInheritors(depth);

                // Raises exit events and sends them to event subscribers
                this.SendExitEvents(raisedEvents, depth);

                if (this.AcmaModificationType.HasFlag(TriggerEvents.Delete))
                {
                    this.Delete(true);
                }

                // Clears the CSEntryChange and resets the object back to read-only
                this.DiscardPendingChanges();
            }
            finally
            {
                Logger.WriteLine("Commit CSEntryChange complete");
                Logger.WriteSeparatorLine('<', true);
                Logger.DecreaseIndent();
            }
        }

        private void ProcessShadowObjectLinks(int depth)
        {
            foreach (AcmaSchemaShadowObjectLink link in this.ObjectClass.ShadowChildLinks)
            {
                AttributeValue provisioningControlValue = this.GetSVAttributeValue(link.ProvisioningAttribute);

                if (this.InternalAttributeChanges.Contains(link.ProvisioningAttribute.Name))
                {
                    // A change on an provisioning control attribute 

                    AttributeChange change = this.InternalAttributeChanges[link.ProvisioningAttribute.Name];

                    if (change.ModificationType == AttributeModificationType.Delete)
                    {
                        this.DeprovisionShadowObject(link);
                    }
                    else
                    {
                        if (provisioningControlValue.ValueBoolean == true)
                        {
                            this.ProvisionShadowObject(link);
                        }
                        else
                        {
                            this.DeprovisionShadowObject(link);
                        }
                    }
                }
                else
                {
                    // There is no change on the provisioning control attribute

                    if (provisioningControlValue.ValueBoolean == true)
                    {
                        List<AttributeChange> changes = this.CreateSyntheticAttributeChangesForShadowChild(link, false);
                        AttributeValue referenceValue = this.GetSVAttributeValue(link.ReferenceAttribute);

                        MAObjectHologram shadowChild = this.GetMAObjectOrDefault(referenceValue.ValueGuid, link.ShadowObjectClass);

                        if (shadowChild == null)
                        {
                            throw new Microsoft.MetadirectoryServices.ObjectNotFoundException(
                                string.Format(
                                    "The shadow object {0} referenced by {1} with link {2} on object {3} was not found in the database",
                                    referenceValue.ValueGuid,
                                    link.ReferenceAttribute.Name,
                                    link.Name,
                                    this.ObjectID)
                                );
                        }

                        if (changes.Count > 0)
                        {
                            shadowChild.SetObjectModificationType(ObjectModificationType.Update, false);
                            shadowChild.CommitCSEntryChange(this, changes, null, null, depth);
                            MAStatistics.AddInheritedUpdate();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.DisplayText;
        }

        public string AttributeDataToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendFormat("Object ID: {0}\n", this.ObjectID);
            builder.AppendFormat("Object class: {0}\n", this.ObjectClass.Name);
            builder.AppendFormat("Deleted: {0}\n", this.DeletedTimestamp == 0 ? "No" : new DateTime(this.DeletedTimestamp).ToString());

            if (this.ObjectClass.IsShadowObject)
            {
                builder.AppendFormat("Shadow parent: {0}\n", this.ShadowParentID);
                builder.AppendFormat("Shadow link: {0}\n", this.ShadowLinkName);
            }

            builder.AppendFormat("Inherited update: {0}\n", this.InheritedUpdate ? "Yes" : "No");

            foreach (AcmaSchemaAttribute attribute in this.ObjectClass.Attributes.Where(t => !t.IsBuiltIn).OrderBy(t => t.Name))
            {
                AttributeValues values = this.GetAttributeValues(attribute);

                if (!values.IsEmptyOrNull)
                {
                    foreach (AttributeValue value in values)
                    {
                        builder.AppendFormat("{0}: {1}\n", attribute.Name, value.ToSmartStringOrNull());
                    }
                }
            }

            return builder.ToString();
        }

        /// <summary>
        /// Determines if the specified object is equal to this object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>True if the other object is an MAObjectHologram with the same ID, otherwise false</returns>
        public override bool Equals(object obj)
        {
            MAObjectHologram maObject = obj as MAObjectHologram;

            if (maObject != null)
            {
                return this.ObjectID == maObject.ObjectID;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Returns the hash code for this instance
        /// </summary>
        /// <returns>The hash code for this instance</returns>
        public override int GetHashCode()
        {
            return this.ObjectID.GetHashCode();
        }

        /// <summary>
        /// Provisions a shadow object with this object as the parent
        /// </summary>
        /// <param name="schemaObject">The type of shadow object to create</param>
        /// <param name="shadowLink">The reference mapping for this object</param>
        /// <returns>A new MAObjectHologram representing the shadow object</returns>
        internal MAObjectHologram ProvisionShadowObject(AcmaSchemaShadowObjectLink shadowLink, CSEntryChange csentry = null)
        {
            MAObjectHologram shadowObject = null;

            AttributeValue existingId = this.GetSVAttributeValue(shadowLink.ReferenceAttribute);

            if (!existingId.IsNull && csentry == null && shadowLink.ShadowObjectClass.AllowResurrection)
            {
                shadowObject = this.UndeleteShadowObject(shadowLink, existingId.ValueGuid);

                if (shadowObject != null)
                {
                    if (shadowObject.ObjectClass.Name != shadowLink.ShadowObjectClass.Name)
                    {
                        Logger.WriteLine("Deleted shadow object was found in the database, but the object class did not match the expected object class. Discarding the existing object");
                        shadowObject = null;
                    }
                }
            }

            if (shadowObject == null && csentry == null)
            {
                Guid newId = Guid.NewGuid();
                shadowObject = this.MADataContext.CreateMAObject(newId, shadowLink.ShadowObjectClass.Name, this, ObjectModificationType.Add);

                Logger.WriteLine(
                    "Created new shadow object '{0}' for parent '{1}' referenced by {{{2}}} and using {{{3}}} as the provisioning indicator",
                    shadowObject.DisplayText,
                    this.DisplayText,
                    shadowLink.ReferenceAttribute.Name,
                    shadowLink.ProvisioningAttribute.Name);
            }
            else if (shadowObject == null && csentry != null)
            {
                Guid newId = new Guid(csentry.DN);
                shadowObject = this.MADataContext.CreateMAObject(newId, shadowLink.ShadowObjectClass.Name, this, ObjectModificationType.Add);
                shadowObject.SetObjectModificationType(ObjectModificationType.Add, false);

                Logger.WriteLine(
                    "Created new shadow object '{0}' for parent '{1}' referenced by {{{2}}} and using {{{3}}} as the provisioning indicator",
                    shadowObject.DisplayText,
                    this.DisplayText,
                    shadowLink.ReferenceAttribute.Name,
                    shadowLink.ProvisioningAttribute.Name);

            }
            else
            {
                Logger.WriteLine(
                    "Undeleted existing shadow object '{0}' for parent '{1}' referenced by {{{2}}} and using {{{3}}} as the provisioning indicator",
                    shadowObject.DisplayText,
                    this.DisplayText,
                    shadowLink.ReferenceAttribute.Name,
                    shadowLink.ProvisioningAttribute.Name);
                shadowObject.SetObjectModificationType(ObjectModificationType.Update, true);
            }

            this.DBUpdateAttribute(shadowLink.ProvisioningAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd(true) });
            this.DBUpdateAttribute(shadowLink.ReferenceAttribute, new List<ValueChange>() { ValueChange.CreateValueAdd(shadowObject.ObjectID) });
            this.Commit();

            shadowObject.ShadowLink = shadowLink;
            shadowObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("shadowParent"), this.ObjectID);
            IList<AttributeChange> changes = this.CreateSyntheticAttributeChangesForShadowChild(shadowLink, true);

            if (csentry != null)
            {
                changes.AddRange(csentry.AttributeChanges);
            }

            shadowObject.CommitCSEntryChange(this, changes, null, null, 1);
            MAStatistics.AddShadowAdd();
            return shadowObject;
        }

        public IEnumerable<Guid> GetShadowObjects()
        {
            foreach (AcmaSchemaShadowObjectLink link in this.ObjectClass.ShadowChildLinks)
            {
                AttributeValue shadowChildId = this.GetSVAttributeValue(link.ReferenceAttribute);

                if (!shadowChildId.IsNull)
                {
                    yield return shadowChildId.ValueGuid;
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Dispose unmanaged managed resources.

                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~MAObjectHologram()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Deprovisions a shadow object
        /// </summary>
        /// <param name="shadowLink">The reference to the shadow object</param>
        internal void DeprovisionShadowObject(AcmaSchemaShadowObjectLink shadowLink)
        {
            AttributeValue shadowObjectId = this.GetSVAttributeValue(shadowLink.ReferenceAttribute);

            if (!shadowObjectId.IsNull)
            {
                this.DBDeleteAttribute(shadowLink.ProvisioningAttribute);

                MAObjectHologram shadowObject = this.GetMAObjectOrDefault(shadowObjectId.ValueGuid, shadowLink.ShadowObjectClass);

                if (shadowObject != null)
                {
                    shadowObject.Delete(false);
                    MAStatistics.AddShadowDelete();
                    Logger.WriteLine(
                        "Deleted shadow object '{0}' for parent '{1}' referenced by {{{2}}} and using {{{3}}} as the provisioning indicator",
                        shadowObject.DisplayText,
                        this.DisplayText,
                        shadowLink.ReferenceAttribute.Name,
                        shadowLink.ProvisioningAttribute.Name);
                }

                this.Commit();
            }
        }

        /// <summary>
        /// Determines if a 'set' attribute request against this object should be processed into the underlying CSEntryChange
        /// </summary>
        /// <param name="attribute">The attribute to modify</param>
        /// <param name="value">The value to apply</param>
        /// <returns>A value indicating if the modification to the CSEntryChange should occur</returns>
        private bool ShouldProcessAttributeChangeSetRequest(AcmaSchemaAttribute attribute, object value)
        {
            return this.ShouldProcessAttributeChangeSetRequest(attribute, new List<object>() { value });
        }

        /// <summary>
        /// Determines if a 'set' attribute request against this object should be processed into the underlying CSEntryChange
        /// </summary>
        /// <param name="attribute">The attribute to modify</param>
        /// <param name="values">The values to apply</param>
        /// <returns>A value indicating if the modification to the CSEntryChange should occur</returns>
        private bool ShouldProcessAttributeChangeSetRequest(AcmaSchemaAttribute attribute, IList<object> values)
        {
            if (values.Count == 0)
            {
                return false;
            }

            if (attribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                return true;
            }

            if (!this.InternalAttributeChanges.Contains(attribute.Name))
            {
                AttributeValues existingValues = this.DBGetAttributeValues(attribute);
                if (!existingValues.IsEmptyOrNull && existingValues.ContainsAllElements(values))
                {
                    //Logger.WriteLine("Discarding request to set attribute '{0}' because there were no changes to make to the underlying database", LogLevel.Debug, attribute.Name);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if a 'delete' attribute request against this object should be processed into the underlying CSEntryChange
        /// </summary>
        /// <param name="attribute">The attribute to modify</param>
        /// <returns>A value indicating if the modification to the CSEntryChange should occur</returns>
        private bool ShouldProcessAttributeChangeDeleteRequest(AcmaSchemaAttribute attribute)
        {
            if (attribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                return true;
            }

            if (!this.InternalAttributeChanges.Contains(attribute.Name))
            {

                AttributeValues existingValues = this.DBGetAttributeValues(attribute);
                if (existingValues == null || existingValues.IsEmptyOrNull)
                {
                    //  Logger.WriteLine("Discarding request to delete attribute '{0}' because there were no changes to make to the underlying database", LogLevel.Debug, attribute.Name);
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Determines if an 'update' attribute request against this object should be processed into the underlying CSEntryChange
        /// </summary>
        /// <param name="attribute">The attribute to modify</param>
        /// <param name="valueChanges">The value changes to apply</param>
        /// <returns>A value indicating if the modification to the CSEntryChange should occur</returns>
        private bool ShouldProcessAttributeChangeUpdateRequest(AcmaSchemaAttribute attribute, IList<ValueChange> valueChanges)
        {
            if (valueChanges.Count == 0)
            {
                return false;
            }

            if (attribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                return true;
            }

            if (!this.InternalAttributeChanges.Contains(attribute.Name))
            {
                AttributeValues existingValues = this.DBGetAttributeValues(attribute);

                if (valueChanges.Any(t => t.ModificationType == ValueModificationType.Delete && existingValues.HasValue(t.Value)))
                {
                    return true;
                }

                if (valueChanges.Any(t => t.ModificationType == ValueModificationType.Add && !existingValues.HasValue(t.Value)))
                {
                    return true;
                }

                // Logger.WriteLine("Discarding request to update attribute '{0}' because there were no changes to make to the underlying database", LogLevel.Debug, attribute.Name);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Updates the reference links that apply to this object
        /// </summary>
        /// <returns>A list of objects that were updated</returns>
        private List<MAObjectHologram> UpdateReferenceBackLinks()
        {
            List<MAObjectHologram> updatedLinks = new List<MAObjectHologram>();

            foreach (AcmaSchemaReferenceLink link in this.ObjectClass.ForwardLinks)
            {
                updatedLinks.AddRange(link.ProcessLinks(this));
            }

            return updatedLinks;
        }

        private List<RaisedEvent> CreateExitEvents()
        {
            List<RaisedEvent> raisedEvents = new List<RaisedEvent>();

            if (!ActiveConfig.XmlConfig.ClassConstructors.Contains(this.ObjectClass.Name))
            {
                return raisedEvents;
            }

            ClassConstructor classConstructor = ActiveConfig.XmlConfig.ClassConstructors[this.ObjectClass.Name];
            if (classConstructor.ExitEvents.Count == 0)
            {
                return raisedEvents;
            }

            return this.CreateExitEvents(classConstructor);
        }

        /// <summary>
        /// Creates any exit events that apply to the current change
        /// </summary>
        /// <param name="triggeringObject">The object that triggered this event</param>
        /// <param name="classConstructor">The class constructor for this object</param>
        /// <returns>A list of RaisedEvents</returns>
        private List<RaisedEvent> CreateExitEvents(ClassConstructor classConstructor)
        {
            List<RaisedEvent> raisedEvents = new List<RaisedEvent>();

            foreach (AcmaEvent maevent in classConstructor.ExitEvents)
            {
                if (maevent.EventType == AcmaEventType.OperationEvent)
                {
                    continue;
                }
                else if (maevent.EventType == AcmaEventType.InternalExitEvent)
                {
                    if (maevent.IsDisabled)
                    {
                        continue;
                    }

                    if (((AcmaInternalExitEvent)maevent).RuleGroup.Evaluate(this))
                    {
                        raisedEvents.Add(new RaisedEvent(maevent, this));
                    }
                }
                else if (maevent.EventType == AcmaEventType.ExternalExitEvent)
                {
                    if (((AcmaExternalExitEvent)maevent).RuleGroup.Evaluate(this))
                    {
                        raisedEvents.Add(new RaisedEvent(maevent, this));
                    }
                }
                else
                {
                    throw new ArgumentException("The event type is unknown");
                }
            }

            if (raisedEvents.Count > 0)
            {
                Logger.WriteLine("The following exit event conditions were met: {0}", LogLevel.Debug, raisedEvents.Select(t => t.EventID).ToCommaSeparatedString());
            }

            return raisedEvents;
        }

        /// <summary>
        /// Creates a distribution list of MAObjectHolograms and the events each should receive
        /// </summary>
        /// <param name="raisedEvents">The events that were raised by this object</param>
        /// <returns>A list of events and recipients</returns>
        private Dictionary<MAObjectHologram, List<RaisedEvent>> CreateInternalExitEventDistributionList(List<RaisedEvent> raisedEvents)
        {
            Dictionary<MAObjectHologram, List<RaisedEvent>> outboundInternalEvents = new Dictionary<MAObjectHologram, List<RaisedEvent>>();

            foreach (RaisedEvent exitEvent in raisedEvents)
            {
                AcmaInternalExitEvent internalEvent = exitEvent.Event as AcmaInternalExitEvent;

                if (internalEvent == null)
                {
                    continue;
                }

                IEnumerable<MAObjectHologram> recipients = internalEvent.GetEventRecipients(this.MADataContext, this);

                foreach (MAObjectHologram recipient in recipients)
                {
                    if (outboundInternalEvents.ContainsKey(recipient))
                    {
                        if (!outboundInternalEvents[recipient].Contains(exitEvent))
                        {
                            outboundInternalEvents[recipient].Add(exitEvent);
                        }
                    }
                    else
                    {
                        outboundInternalEvents.Add(recipient, new List<RaisedEvent>() { exitEvent });
                    }
                }
            }

            return outboundInternalEvents;
        }

        /// <summary>
        /// Determines the exit events to create and sends them
        /// </summary>
        /// <param name="triggeringObject">The object triggering this update</param>
        /// <param name="depth">The current commit depth</param>
        private void SendExitEvents(List<RaisedEvent> raisedEvents, int depth)
        {
            if (raisedEvents.Count > 0)
            {
                Dictionary<MAObjectHologram, List<RaisedEvent>> outboundInternalEvents = this.CreateInternalExitEventDistributionList(raisedEvents);

                if (outboundInternalEvents.Count > 0)
                {
                    this.SendInternalExitEvents(outboundInternalEvents, depth);
                }

                this.SendExternalExitExents(raisedEvents);
            }
        }

        /// <summary>
        /// Sends the exit events to the specified MAObjectHolograms
        /// </summary>
        /// <param name="outgoingEvents">The recipients and events to send to each</param>
        /// <param name="depth">The current commit depth</param>
        private void SendInternalExitEvents(Dictionary<MAObjectHologram, List<RaisedEvent>> outgoingEvents, int depth)
        {
            foreach (MAObjectHologram recipient in outgoingEvents.Keys)
            {
                Logger.WriteLine("Sending exit events '{0}' to {1}", outgoingEvents[recipient].Select(t => t.EventID).ToCommaSeparatedString(), recipient.DisplayText);

                recipient.SetObjectModificationType(ObjectModificationType.Update, false);
                recipient.CommitCSEntryChange(this, null, outgoingEvents[recipient], null, depth);
            }
        }

        private void SendExternalExitExents(IList<RaisedEvent> outgoingEvents)
        {
            foreach (RaisedEvent e in outgoingEvents.Where(t => (t.Event.EventType == AcmaEventType.ExternalExitEvent)))
            {
                try
                {
                    Logger.WriteLine("Executing external exit event {0}", e.EventID);
                    Logger.IncreaseIndent();

                    ((AcmaExternalExitEvent)e.Event).Execute(e);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("The external event {0} returned an error", e.EventID);
                    Logger.WriteException(ex);
                }
                finally
                {
                    Logger.DecreaseIndent();
                }
            }
        }

        /// <summary>
        /// Creates the synthetic attribute changes that inheritors need to know that a value on this object was modified
        /// </summary>
        /// <param name="depth">The current commit depth</param>
        private void CreateAndSendAttributeChangesToInheritors(int depth)
        {
            Dictionary<MAObjectHologram, List<AttributeChange>> inheritorAttributeChanges = this.CreateSyntheticAttributeChangesForInheritors();

            if (inheritorAttributeChanges.Count > 0)
            {
                Logger.WriteLine("Sending attribute changes to inheritors");

                foreach (MAObjectHologram maObject in inheritorAttributeChanges.Keys)
                {
                    MAStatistics.AddInheritedUpdate();
                    maObject.SetObjectModificationType(ObjectModificationType.Update, false);
                    maObject.CommitCSEntryChange(this, inheritorAttributeChanges[maObject], null, null, depth);
                }
            }
        }

        /// <summary>
        /// Commits changes to the objects made by a reference link update
        /// </summary>
        /// <param name="updatedObjects">The objects that were updated by this object</param>
        /// <param name="depth">The current commit depth</param>
        private void CommitReferenceBackLinkUpdatedObjects(IList<MAObjectHologram> updatedObjects, int depth)
        {
            if (updatedObjects.Count > 0)
            {
                Logger.WriteLine("Processing reference back-link updates");

                foreach (MAObjectHologram maObject in updatedObjects)
                {
                    if (maObject.ReferenceRetryRequired)
                    {
                        this.referenceRetryRequired = true;
                    }

                    MAStatistics.AddBacklinkUpdate();
                    //maObject.SetObjectModificationType(ObjectModificationType.Update);
                    maObject.CommitCSEntryChange(this, null, null, null, depth);
                }
            }
        }

        /// <summary>
        /// Adds the specified attribute changes to the underlying CSEntryChange
        /// </summary>
        /// <param name="attributeChanges">The list of attribute changes to add</param>
        private void AddAttributeChanges(IEnumerable<AttributeChange> attributeChanges)
        {
            if (attributeChanges != null)
            {
                foreach (AttributeChange attributeChange in attributeChanges)
                {
                    this.InternalAttributeChanges.AddOrReplaceAttributeChange(attributeChange);
                }
            }
        }

        /// <summary>
        /// Records that an object referenced in an attribute on this object was missing from the database
        /// </summary>
        /// <param name="missingObjectId">The ID of the missing object</param>
        internal void MarkMissingReference(Guid missingObjectId)
        {
            this.MarkMissingReference(missingObjectId, null);
        }

        /// <summary>
        /// Records that an object referenced in an attribute on this object was missing from the database
        /// </summary>
        /// <param name="missingObjectId">The ID of the missing object</param>
        /// <param name="attribute">The attribute that referenced the object</param>
        internal void MarkMissingReference(Guid missingObjectId, AcmaSchemaAttribute attribute)
        {
            if (missingObjectId == Guid.Empty)
            {
                return;
            }

            this.referenceRetryRequired = true;
            MAStatistics.AddReferenceRetryRequest();
            if (attribute != null)
            {
                Logger.WriteLine("Warning: Object {0} referenced by object {1} on attribute {2} was not found", missingObjectId.ToString(), this.DisplayText, attribute.Name);
            }
            else
            {
                Logger.WriteLine("Warning: Object {0} referenced by object {1} was not found", missingObjectId.ToString(), this.DisplayText);
            }
        }

        /// <summary>
        /// Undeletes a shadow object that once was attached to this object
        /// </summary>
        /// <param name="existingId">The ID of the existing object</param>
        /// <returns>An MAObjectHologram referencing the undeleted object</returns>
        private MAObjectHologram UndeleteShadowObject(AcmaSchemaShadowObjectLink link, Guid existingId)
        {
            if (existingId == Guid.Empty)
            {
                return null;
            }

            MAObjectHologram shadowObject = this.GetMAObjectOrDefault(existingId, link.ShadowObjectClass);

            if (shadowObject != null)
            {
                shadowObject.DeletedTimestamp = 0;
                shadowObject.Commit();
            }
            else
            {
                Logger.WriteLine("Existing shadow object '{0}' was not found in the database", LogLevel.Debug, existingId.ToString());
            }

            return shadowObject;
        }

        /// <summary>
        /// Clears the provisioning attribute of the shadow parent of this object
        /// </summary>
        private void DereferenceShadowParent()
        {
            if (this.ShadowParentID != Guid.Empty)
            {
                if (this.ShadowLink != null)
                {
                    this.ShadowParent.SetObjectModificationType(ObjectModificationType.Update, false);
                    this.ShadowParent.SetAttributeValue(this.ShadowLink.ProvisioningAttribute, false);

                    if (!this.ObjectClass.AllowResurrection)
                    {
                        this.ShadowParent.DeleteAttribute(this.ShadowLink.ReferenceAttribute);
                    }

                    this.ShadowParent.CommitCSEntryChange();
                    Logger.WriteLine(
                        "Dereferenced shadow object '{0}' for parent '{1}' referenced by {{{2}}} and using {{{3}}} as the provisioning indicator",
                        this.DisplayText,
                        this.ShadowParentID.ToString(),
                        this.ShadowLink.ReferenceAttribute.Name,
                        this.ShadowLink.ProvisioningAttribute.Name);
                }
            }
        }

        /// <summary>
        /// Deletes any shadow objects attached to this object
        /// </summary>
        private void DeleteShadowObjects()
        {
            if (!this.ObjectClass.IsShadowParent)
            {
                // This object class is not a shadow parent class, therefore there is no need to search for potential shadow children
                return;
            }

            List<MAObjectHologram> shadowObjects = this.MADataContext.GetMAObjectsFromDBQuery(ActiveConfig.DB.GetAttribute("shadowParent"), ValueOperator.Equals, this.ObjectID).ToList();

            if (shadowObjects.Count > 0)
            {
                Logger.WriteLine("Deleting all shadow objects for object '{0}'", this.DisplayText);

                try
                {
                    Logger.IncreaseIndent();

                    foreach (MAObjectHologram shadowObject in shadowObjects)
                    {
                        shadowObject.SetObjectModificationType(ObjectModificationType.Delete, false);
                        shadowObject.CommitCSEntryChange(this);
                    }
                }
                finally
                {
                    Logger.DecreaseIndent();
                }
            }
        }

        /// <summary>
        /// Deletes this object
        /// </summary>
        /// <param name="dereferenceShadowParent">A value indicating if this object's shadow parent should be dereferenced</param>
        internal void Delete(bool dereferenceShadowParent)
        {
            if (this.ObjectClass.IsShadowObject & dereferenceShadowParent)
            {
                this.DereferenceShadowParent();
            }

            this.DeleteShadowObjects();

            this.DereferenceObject();

            if (this.ObjectClass.AllowResurrection)
            {
                this.SetDeletedTimeStamp();
                this.Commit();
            }
            else
            {
                this.MADataContext.DeleteMAObjectPermanent(this.ObjectID);
            }

            Logger.WriteLine("Deleted {0} object: {1}", this.ObjectClass.Name, this.DisplayText);
        }

        private void SetDeletedTimeStamp()
        {
            if (this.DeletedTimestamp == 0)
            {
                Logger.WriteLine("Marked object for deletion");
                this.DeletedTimestamp = DateTime.UtcNow.Ticks;
            }
        }

        private void DereferenceObject(int depth = 0)
        {
            depth++;

            Logger.WriteLine("Performing discovery for objects referencing this object ({0})",
                            LogLevel.Debug, this.DisplayText);

            ValueChange valuechange = ValueChange.CreateValueDelete(this.ObjectID);
            List<ValueChange> valueChanges = new List<ValueChange>() { valuechange };
            Dictionary<MAObjectHologram, List<AttributeChange>> referrerChanges = new Dictionary<MAObjectHologram, List<AttributeChange>>();

            Dictionary<Guid, IList<string>> referencingObjects = this.MADataContext.GetReferencingObjects(this);

            foreach (KeyValuePair<Guid, IList<string>> kvp in referencingObjects)
            {
                MAObjectHologram referencingObject = this.MADataContext.GetMAObjectOrDefault(kvp.Key);

                if (referencingObject == null)
                {
                    this.MarkMissingReference(kvp.Key);
                    continue;
                }

                if (!referrerChanges.ContainsKey(referencingObject))
                {
                    referrerChanges.Add(referencingObject, new List<AttributeChange>());
                }

                foreach (string attribute in kvp.Value)
                {
                    if (this.ObjectClass.IsShadowObject &&
                        this.ObjectClass.AllowResurrection &&
                        this.ShadowParentID == kvp.Key &&
                        this.ShadowLink.ReferenceAttribute.Name == attribute)
                    {
                        // This object class allows objects to be undeleted, so the reference needs to be left on the 
                        // shadow parent
                        continue;
                    }

                    AttributeChange newchange = new AttributeChangeDetached(
                           attribute,
                           AttributeModificationType.Update,
                           valueChanges);

                    referrerChanges[referencingObject].Add(newchange);

                    Logger.WriteLine(
                            "Created synthetic AttributeChange for object '{0}' referring to object pending delete '{1}' via attribute {{{2}}}",
                            LogLevel.Debug,
                            referencingObject.DisplayText,
                            this.DisplayText,
                            attribute);
                }
            }

            foreach (MAObjectHologram hologram in referrerChanges.Keys)
            {
                if (referrerChanges[hologram].Count == 0)
                {
                    continue;
                }

                MAStatistics.AddInheritedUpdate();
                hologram.SetObjectModificationType(ObjectModificationType.Update, false);
                hologram.CommitCSEntryChange(this, referrerChanges[hologram], null, null, depth);
            }
        }

        /// <summary>
        /// Gets a list of synthetic attribute changes that need to apply to inheritors attached to this object
        /// </summary>
        /// <returns>A list of MAObjectHolograms and the synthetic attribute changes that need to be processed on each</returns>
        private Dictionary<MAObjectHologram, List<AttributeChange>> CreateSyntheticAttributeChangesForInheritors()
        {
            Dictionary<MAObjectHologram, List<AttributeChange>> list = new Dictionary<MAObjectHologram, List<AttributeChange>>();

            Dictionary<string, IEnumerable<MAObjectHologram>> queryResultCache = new Dictionary<string, IEnumerable<MAObjectHologram>>();

            foreach (AttributeChange attributeChange in this.InternalAttributeChanges)
            {
                foreach (AcmaSchemaMapping mapping in ActiveConfig.DB.Mappings.Where(t =>
                    t.InheritedAttribute != null &&
                    t.InheritanceSourceObjectClass == this.ObjectClass &&
                    t.InheritanceSourceAttribute.Name != "shadowParent" &&
                    t.InheritedAttribute.Name == attributeChange.Name))
                {
                    AttributeChange newchange = new AcmaInheritedAttributeChange(
                        mapping.Attribute,
                        attributeChange.ModificationType,
                        attributeChange.ValueChanges);

                    string key = mapping.InheritanceSourceAttribute.Name + mapping.ObjectClass.Name;

                    if (!queryResultCache.ContainsKey(key))
                    {
                        DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
                        queryGroup.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectClass"), ValueOperator.Equals, mapping.ObjectClass.Name));
                        queryGroup.DBQueries.Add(new DBQueryByValue(mapping.InheritanceSourceAttribute, ValueOperator.Equals, this.ObjectID));
                        queryGroup.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.NotEquals, this.ObjectID));
                        queryResultCache.Add(key, this.MADataContext.GetMAObjectsFromDBQuery(queryGroup).ToList());
                    }

                    foreach (MAObjectHologram maObject in queryResultCache[key])
                    {
                        if (maObject.ObjectID == this.ObjectID)
                        {
                            continue;
                        }

                        if (list.ContainsKey(maObject))
                        {
                            list[maObject].Add(newchange);
                        }
                        else
                        {
                            list.Add(maObject, new List<AttributeChange>() { newchange });
                        }

                        Logger.WriteLine(
                            "Created InheritedAttributeChange for attribute {{{3}}} inherited by '{0}' via {{{1}->{2}}}",
                            LogLevel.Debug,
                            maObject.DisplayText,
                            mapping.InheritanceSourceAttribute.Name,
                            mapping.InheritedAttribute.Name,
                            mapping.Attribute.Name);
                    }
                }
            }

            return list;
        }

        /// <summary>
        /// Gets a list of synthetic attribute changes that need to apply to inheritors attached to this object
        /// </summary>
        /// <returns>A list of MAObjectHolograms and the synthetic attribute changes that need to be processed on each</returns>
        private List<AttributeChange> CreateSyntheticAttributeChangesForShadowChild(AcmaSchemaShadowObjectLink link, bool createChangesForAllInheritedAttributes)
        {
            List<AttributeChange> list = new List<AttributeChange>();

            AttributeValue targetID = this.GetSVAttributeValue(link.ReferenceAttribute);
            AttributeValue provisioningAttribute = this.GetSVAttributeValue(link.ProvisioningAttribute);

            if (targetID.IsNull || provisioningAttribute.ValueBoolean == false)
            {
                return list;
            }


            foreach (AcmaSchemaMapping mapping in ActiveConfig.DB.Mappings.Where(t =>
                t.InheritedAttribute != null &&
                t.InheritanceSourceAttribute.Name == "shadowParent" &&
                t.ObjectClass == link.ShadowObjectClass &&
                t.InheritanceSourceObjectClass == this.ObjectClass &&
                (createChangesForAllInheritedAttributes || this.InternalAttributeChanges.Contains(t.InheritedAttribute.Name))
                ))
            {
                AttributeChange newchange = this.CreateInheritedAttributeChangeOrDefault(mapping);

                if (newchange != null)
                {
                    list.Add(newchange);

                    Logger.WriteLine(
                        "Created shadow InheritedAttributeChange for attribute {{{3}}} inherited by '{0}' via {{{1}->{2}}}",
                        LogLevel.Debug,
                        link.ReferenceAttribute.Name,
                        mapping.InheritanceSourceAttribute.Name,
                        mapping.InheritedAttribute.Name,
                        mapping.Attribute.Name);
                }
            }

            return list;
        }

        private AttributeChange CreateInheritedAttributeChangeOrDefault(AcmaSchemaMapping mapping)
        {
            AttributeChange newchange;

            AttributeValues values = this.GetAttributeValues(mapping.InheritedAttribute);

            if (values.IsEmptyOrNull)
            {
                return null;
            }
            else
            {
                List<ValueChange> valueChanges = new List<ValueChange>();

                foreach (AttributeValue value in values.Values)
                {
                    valueChanges.Add(ValueChange.CreateValueAdd(value.Value));
                }

                newchange = new AcmaInheritedAttributeChange(
                    mapping.Attribute,
                    AttributeModificationType.Add,
                    valueChanges);

            }

            return newchange;
        }

        /// <summary>
        /// Gets an enumeration of objects from the proposed value of the specified attribute
        /// </summary>
        /// <param name="attribute">The attribute containing the list of referenced objects</param>
        /// <returns>An enumeration of MAObjectHologram objects</returns>
        private IEnumerable<MAObjectHologram> GetReferencedObjectsProposed(AcmaSchemaAttribute attribute)
        {
            if (attribute.Type != ExtendedAttributeType.Reference)
            {
                throw new ArgumentException("The specified attribute is not of the reference type");
            }

            AttributeValues values = this.GetAttributeValues(attribute, HologramView.Proposed);

            foreach (AttributeValue value in values.Where(t => !t.IsNull))
            {
                MAObjectHologram referencedObject = this.GetMAObjectOrDefault(value.ValueGuid);
                if (referencedObject == null)
                {
                    this.MarkMissingReference(value.ValueGuid, attribute);
                }
                else
                {
                    yield return referencedObject;
                }
            }
        }

        /// <summary>
        /// Gets an enumeration of objects from the current database value of the specified attribute
        /// </summary>
        /// <param name="attribute">The attribute containing the list of referenced objects</param>
        /// <returns>An enumeration of MAObjectHologram objects</returns>
        private IEnumerable<MAObjectHologram> GetReferencedObjectsCurrent(AcmaSchemaAttribute attribute)
        {
            if (attribute.Type != ExtendedAttributeType.Reference)
            {
                throw new ArgumentException("The specified attribute is not of the reference type");
            }

            AttributeValues values = this.DBGetAttributeValues(attribute);

            foreach (AttributeValue value in values.Where(t => !t.IsNull))
            {
                MAObjectHologram referencedObject = this.GetMAObjectOrDefault(value.ValueGuid);
                if (referencedObject == null)
                {
                    this.MarkMissingReference(value.ValueGuid, attribute);
                }
                else
                {
                    yield return referencedObject;
                }
            }
        }

        /// <summary>
        /// Executes the applicable ClassConstructor group for this object class
        /// </summary>
        /// <param name="triggeringObject">The object triggering the construction event</param>
        private void ExecuteConstructors(IList<string> constructors)
        {
            if (!ActiveConfig.XmlConfig.ClassConstructors.Contains(this.ObjectClass.Name))
            {
                return;
            }

            ActiveConfig.XmlConfig.ClassConstructors[this.ObjectClass.Name].EvaluateAndExecute(this, constructors);
        }

        private Guid GetInheritanceSource(AcmaSchemaAttribute attribute)
        {
            AcmaSchemaMapping mapping = ActiveConfig.DB.GetMapping(attribute.Name, this.ObjectClass.Name);

            if (mapping == null)
            {
                throw new InvalidOperationException("The specified attribute was not an inherited attribute");
            }

            AttributeValue reference = this.GetSVAttributeValue(mapping.InheritanceSourceAttribute);

            return reference.ValueGuid;
        }

        private AttributeValues GetAttributeValuesInherited(AcmaSchemaAttribute attribute)
        {
            Guid inheritanceSource = this.GetInheritanceSource(attribute);
            AcmaSchemaMapping mapping = ActiveConfig.DB.GetMapping(attribute.Name, this.ObjectClass.Name);

            MAObjectHologram referencedObject = this.GetMAObjectOrDefault(inheritanceSource, mapping.InheritanceSourceObjectClass);

            if (referencedObject == null)
            {
                return new InternalAttributeValues(attribute);
            }

            return referencedObject.GetAttributeValues(mapping.InheritedAttribute);
        }

        /// <summary>
        /// Gets the value of a multi valued attribute. If the attribute has a pending attributeChange, the new values are returned, otherwise the values from the database are returned.
        /// </summary>
        /// <param name="attribute">The attribute to obtain the values for</param>
        /// <returns>An AttributeValues object containing the values of the attribute</returns>
        private AttributeValues GetAttributeProposedValues(AcmaSchemaAttribute attribute)
        {
            if (this.InternalAttributeChanges.Count == 0)
            {
                return this.DBGetAttributeValues(attribute) ?? new InternalAttributeValues(attribute);
            }

            if (!this.InternalAttributeChanges.Contains(attribute.Name))
            {
                return this.DBGetAttributeValues(attribute) ?? new InternalAttributeValues(attribute);
            }

            AttributeChange change = this.InternalAttributeChanges[attribute.Name];

            switch (change.ModificationType)
            {
                case AttributeModificationType.Add:
                case AttributeModificationType.Replace:
                    IEnumerable<ValueChange> valueChanges = change.ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add);
                    List<object> values = new List<object>();
                    foreach (ValueChange valueChange in valueChanges)
                    {
                        values.Add(valueChange.Value);
                    }

                    return new InternalAttributeValues(attribute, values);

                case AttributeModificationType.Delete:
                    return new InternalAttributeValues(attribute);

                case AttributeModificationType.Update:
                    List<object> newValues = new List<object>();
                    newValues.AddRange(change.ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(u => u.Value));

                    if (attribute.IsMultivalued)
                    {
                        foreach (AttributeValue value in this.DBGetAttributeValues(attribute).Values)
                        {
                            if (!change.ValueChanges.Any(t => value.ValueEquals(t.Value) && t.ModificationType == ValueModificationType.Delete))
                            {
                                newValues.Add(value.Value);
                            }
                        }
                    }

                    return new InternalAttributeValues(attribute, newValues.ToArray<object>());

                case AttributeModificationType.Unconfigured:
                default:
                    throw new ArgumentException("The change type is unknown or unsupported");
            }
        }

        /// <summary>
        /// Applies the attribute changes in the attached CSEntryChange to this object
        /// </summary>
        private void ApplyInternalAttributeChanges()
        {
            //if (this.CSEntryModificationType == Microsoft.MetadirectoryServices.ObjectModificationType.Replace)
            //{
            //    foreach (AcmaSchemaMapping mapping in this.ObjectClass.Mappings.Where(t => !t.Attribute.IsBuiltIn))
            //    {
            //        this.DBDeleteAttribute(mapping.Attribute);
            //    }
            //}

            if (this.InternalAttributeChanges.Count > 0)
            {
                Logger.WriteLine("Committing changes to database");
                foreach (AttributeChange attributeChange in this.InternalAttributeChanges)
                {
                    this.ApplyAttributeChange(attributeChange);
                }
            }
        }

        /// <summary>
        /// Commits the value changes in an AttributeChange object to the database
        /// </summary>
        /// <param name="attributeChange">The AttributeChange to commit</param>
        private void ApplyAttributeChange(AttributeChange attributeChange)
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute(attributeChange.Name, this.ObjectClass);
            if (attribute.Operation == AcmaAttributeOperation.AcmaInternalTemp)
            {
                return;
            }

            if (attribute.IsInheritedInClass(this.ObjectClass))
            {
                this.InheritedUpdate = true;
                return;
            }

            switch (attributeChange.ModificationType)
            {
                //case AttributeModificationType.Add:
                // this technically shouldn't be possible.
                //if (this.DBHasAttribute(attribute))
                //{
                //    this.DBDeleteAttribute(attribute);
                //}
                //if (attribute.IsInAVPTable)
                //{
                //    this.CreateValueDeletesForExistingValues(attribute, attributeChange);
                //}

                //this.DBUpdateAttribute(attribute, attributeChange.ValueChanges);
                //break;

                case AttributeModificationType.Delete:
                    this.DBDeleteAttribute(attribute);
                    break;

                case AttributeModificationType.Add:
                case AttributeModificationType.Replace:
                    if (attribute.IsInAVPTable)
                    {
                        this.CreateValueDeletesForExistingValues(attribute, attributeChange);
                    }

                    this.DBUpdateAttribute(attribute, attributeChange.ValueChanges);
                    break;

                case AttributeModificationType.Update:
                    this.DBUpdateAttribute(attribute, attributeChange.ValueChanges);
                    break;

                case AttributeModificationType.Unconfigured:
                default:
                    throw new ArgumentException("Unknown or unsupported change type");
            }
        }

        /// <summary>
        /// Throws an error when a CSEntryChange has not been attached to this
        /// </summary>
        private void ThrowOnModificationTypeNotSet()
        {
            if (this.AcmaModificationType == TriggerEvents.Unconfigured || this.AcmaModificationType == TriggerEvents.None)
            {
                throw new ModificationTypeNotSetException(string.Format("The object '{0}' is read only as no modification type has been set", this.DisplayText));
            }
        }

        private void CreateValueDeletesForExistingValues(AcmaSchemaAttribute attribute, AttributeChange change)
        {
            AttributeValues existingValues = this.DBGetAttributeValues(attribute);
            List<ValueChange> valueDeletes = new List<ValueChange>();

            foreach (AttributeValue value in existingValues)
            {
                if (!value.IsNull)
                {
                    valueDeletes.Add(ValueChange.CreateValueDelete(value.Value));
                }
            }

            IList<ValueChange> mergedChanges = AttributeChangeExtensions.MergeValueChangeLists(attribute, valueDeletes, change.ValueChanges);
            change.ValueChanges.Clear();

            foreach (ValueChange valueChange in mergedChanges)
            {
                change.ValueChanges.Add(valueChange);
            }
        }

        private MAObjectHologram GetMAObjectOrDefault(Guid id)
        {
            return this.GetMAObjectOrDefault(id, null);
        }

        private MAObjectHologram GetMAObjectOrDefault(Guid id, AcmaSchemaObjectClass objectClass)
        {
            if (!this.hologramCache.ContainsKey(id))
            {
                MAObjectHologram hologram;

                if (objectClass == null)
                {
                    hologram = this.MADataContext.GetMAObjectOrDefault(id);
                }
                else
                {
                    hologram = this.MADataContext.GetMAObjectOrDefault(id, objectClass);
                }

                if (hologram != null)
                {
                    this.hologramCache.Add(id, hologram);
                }
                else
                {
                    this.MarkMissingReference(id);
                    return null;
                }
            }

            return this.hologramCache[id];
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            foreach (KeyValuePair<string, IList<string>> kvp in this.GetSerializationValues())
            {
                if (kvp.Value.Count > 1)
                {
                    info.AddValue(kvp.Key, kvp.Value, typeof(List<object>));
                }
                else if (kvp.Value.Count == 1)
                {
                    info.AddValue(kvp.Key, kvp.Value.First(), typeof(object));
                }
                else
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Gets a list of attributes and values for the resource in a string format suitable for serialization
        /// </summary>
        /// <returns></returns>
        internal Dictionary<string, IList<string>> GetSerializationValues()
        {
            Dictionary<string, IList<string>> values = new Dictionary<string, IList<string>>();

            foreach (AcmaSchemaAttribute attribute in this.ObjectClass.Attributes)
            {
                AttributeValues value = this.GetAttributeValues(attribute);

                if (!value.IsEmptyOrNull)
                {
                    values.Add(attribute.Name, this.GetAttributeValues(attribute).GetSerializationValues());
                }
            }

            return values;
        }
    }
}
