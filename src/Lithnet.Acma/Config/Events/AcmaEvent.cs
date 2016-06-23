// -----------------------------------------------------------------------
// <copyright file="AcmaEvent.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Lithnet.Common.ObjectModel;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an event that can be sent to a hologram
    /// </summary>
    [DataContract(Name = "event", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [KnownType(typeof(AcmaInternalExitEvent))]
    [KnownType(typeof(AcmaExternalExitEvent))]
    [KnownType(typeof(AcmaOperationEvent))]
    public abstract class AcmaEvent : UINotifyPropertyChanges, IExtensibleDataObject, IObjectClassScopeProvider, IIdentifiedObject
    {
        public delegate void KeyChangingEventHandler(object sender, string newKey);

        public event KeyChangingEventHandler KeyChanging;

        /// <summary>
        /// The name of the unique ID cache group
        /// </summary>
        public const string CacheGroupName = "event";

        /// <summary>
        /// The ID of the object
        /// </summary>
        private string id;

        /// <summary>
        /// Indicates if the object is in the process of deserializing
        /// </summary>
        private bool deserializing;

        /// <summary>
        /// Initializes a new instance of the AcmaEvent class
        /// </summary>
        public AcmaEvent()
        {
            this.Initialize();
        }

        public bool AllowDuplicateIDs { get; set; }

        /// <summary>
        /// Gets or sets the ID of the event
        /// </summary>
        [DataMember(Name = "name")]
        public string ID
        {
            get
            {
                return this.id;
            }

            set
            {
                try
                {
                    if (this.AllowDuplicateIDs)
                    {
                        this.id = value;
                    }
                    else
                    {
                        string newID = UniqueIDCache.SetID(this, value, AcmaEvent.CacheGroupName, this.deserializing);

                        if (this.id != newID)
                        {
                            if (this.KeyChanging != null)
                            {
                                this.KeyChanging(this, newID);
                            }
                        }

                        this.id = newID;

                        if (this.id == null && UniqueIDCache.HasObject(AcmaEvent.CacheGroupName, this))
                        {
                            UniqueIDCache.RemoveItem(this, AcmaEvent.CacheGroupName);
                        }
                        else if (!UniqueIDCache.HasObject(AcmaEvent.CacheGroupName, this))
                        {
                            UniqueIDCache.AddItem(this, AcmaEvent.CacheGroupName);
                        }

                        this.RemoveError("Id");
                    }
                }
                catch (DuplicateIdentifierException)
                {
                    this.AddError("Id", "The specified ID is already in use");
                }
            }
        }

        /// <summary>
        /// Gets or sets the description of the object
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "disabled")]
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public abstract AcmaEventType EventType { get; }

        /// <summary>
        /// Gets or sets the serialization extension data object
        /// </summary>
        /// 
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Gets the object class that this rule applies to
        /// </summary>
        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                if (this.ObjectClassScopeProvider != null)
                {
                    return this.ObjectClassScopeProvider.ObjectClass;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the object class schema provider
        /// </summary>
        public IObjectClassScopeProvider ObjectClassScopeProvider { get; set; }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.ID;
        }

        /// <summary>
        /// Validates a change to a property
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "Id":
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.AddError("Id", "An event name must be specified");
                    }
                    else
                    {
                        this.RemoveError("Id");
                    }

                    break;
                default:
                    break;
            }
        }

        internal virtual void OnEventRaised(RaisedEvent raisedEvent, MAObjectHologram sender)
        {
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        protected virtual void Initialize()
        {
            
            this.ValidatePropertyChange("Id");
        }

        /// <summary>
        /// Occurs just prior to the object being deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.deserializing = true;
            this.Initialize();
        }

        /// <summary>
        /// Occurs after the object has been deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.deserializing = false;
        }
    }
}
