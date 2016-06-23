// -----------------------------------------------------------------------
// <copyright file="ExecutableConstructorObject.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Collections.Specialized;
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Lithnet.Common.ObjectModel;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an abstract base class for executable constructor objects
    /// </summary>
    [DataContract(Name = "constructor-object", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [KnownType(typeof(AttributeConstructor))]
    [KnownType(typeof(AttributeConstructorGroup))]
    public abstract class ExecutableConstructorObject : UINotifyPropertyChanges, IObjectClassScopeProvider, IExtensibleDataObject, IIdentifiedObject
    {
        public delegate void KeyChangingEventHandler(object sender, string newKey);

        public event KeyChangingEventHandler KeyChanging;

        /// <summary>
        /// The name of the cache group used to stored the IDs of objects of this type
        /// </summary>
        public const string CacheGroupName = "constructors";

        /// <summary>
        /// The ID of this object
        /// </summary>
        private string id;

        /// <summary>
        /// The execution rules for this object
        /// </summary>
        private RuleGroup ruleGroup;

        /// <summary>
        /// Indicates if the object is in the process of deserializing
        /// </summary>
        private bool deserializing;

        /// <summary>
        /// Initializes a new instance of the ExecutableConstructorObject class
        /// </summary>
        public ExecutableConstructorObject()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the name of the object
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets the ID of this object
        /// </summary>
        [DataMember(Name = "id")]
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
                    string newID = UniqueIDCache.SetID(this, value, ExecutableConstructorObject.CacheGroupName, this.deserializing);

                    if (this.id != newID)
                    {
                        if (this.KeyChanging != null)
                        {
                            this.KeyChanging(this, newID);
                        }
                    }

                    this.id = newID;

                    if (this.id == null && UniqueIDCache.HasObject(ExecutableConstructorObject.CacheGroupName, this))
                    {
                        UniqueIDCache.RemoveItem(this, ExecutableConstructorObject.CacheGroupName);
                    }
                    else if (!UniqueIDCache.HasObject(ExecutableConstructorObject.CacheGroupName, this))
                    {
                        UniqueIDCache.AddItem(this, ExecutableConstructorObject.CacheGroupName);
                    }

                    this.RemoveError("Id");
                }
                catch (DuplicateIdentifierException)
                {
                    this.AddError("Id", "The specified ID is already in use");
                }
            }
        }

        /// <summary>
        /// Gets or sets the description for this object
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the rule group set that applies to this constructor
        /// </summary>
        [DataMember(Name = "rule-group")]
        public RuleGroup RuleGroup
        {
            get
            {
                return this.ruleGroup;
            }

            set
            {
                if (this.ruleGroup != null)
                {
                    this.ruleGroup.ObjectClassScopeProvider = null;
                }

                if (value != null)
                {
                    value.ObjectClassScopeProvider = this;
                }

                this.ruleGroup = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this object is disabled
        /// </summary>
        [DataMember(Name = "disabled")]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the serialization extension data object
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Gets or sets the object class scope provider
        /// </summary>
        [PropertyChanged.AlsoNotifyFor("ObjectClass")]
        public IObjectClassScopeProvider ObjectClassScopeProvider { get; set; }

        public virtual IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            string path = string.Format("{0}\\{1}", parentPath, this.ID);

            if (this.RuleGroup != null)
            {
                foreach (SchemaAttributeUsage usage in this.RuleGroup.GetAttributeUsage(path, attribute))
                {
                    yield return usage;
                }
            }
        }

        /// <summary>
        /// Gets the object class that this rule applies to
        /// </summary>
        public virtual AcmaSchemaObjectClass ObjectClass
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
        /// Executes the object if the execution conditions are evaluated and met
        /// </summary>
        /// <param name="hologram">The object being processed</param>
        /// <returns>A value indicating whether the constructor object met the requirements for execution</returns>
        public abstract bool EvaluateAndExecute(MAObjectHologram hologram, IList<string> constructorOverrides);

        /// <summary>
        /// Validates a change to a property
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            if (this.deserializing)
            {
                return;
            }

            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "Id":
                    if (string.IsNullOrWhiteSpace(this.ID))
                    {
                        this.AddError("Id", "An ID must be specified");
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
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
            this.ValidatePropertyChange("Id");
        }

        /// <summary>
        /// Occurs when an item in the collection of rule objects changes
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event parameters</param>
        private void Rules_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (RuleObject item in e.NewItems)
                    {
                        item.ObjectClassScopeProvider = this;
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (RuleObject item in e.OldItems)
                    {
                        item.ObjectClassScopeProvider = null;
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (RuleObject item in e.OldItems)
                    {
                        item.ObjectClassScopeProvider = null;
                    }

                    foreach (RuleObject item in e.NewItems)
                    {
                        item.ObjectClassScopeProvider = this;
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    foreach (RuleObject item in e.NewItems)
                    {
                        item.ObjectClassScopeProvider = this;
                    }

                    break;

                default:
                    break;
            }
        }
    }
}
