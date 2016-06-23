// -----------------------------------------------------------------------
// <copyright file="ClassConstructor.cs" company="Ryan Newington">
// Copyright (c) Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Lithnet.Common.ObjectModel;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Logging;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an object containing constructors required for building an object of the specified class
    /// </summary>
    [DataContract(Name = "class-constructor", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class ClassConstructor : UINotifyPropertyChanges, IObjectClassScopeProvider, IExtensibleDataObject
    {
        public delegate void KeyChangingEventHandler(object sender, string newKey);

        public event KeyChangingEventHandler KeyChanging;

        /// <summary>
        /// The child constructor objects within this group
        /// </summary>
        private Constructors constructors;

        /// <summary>
        /// The events that can be raised by this class
        /// </summary>
        private AcmaEvents exitEvents;

        private AcmaSchemaObjectClass objectClass;

        /// <summary>
        /// Initializes a new instance of the ClassConstructor class
        /// </summary>
        public ClassConstructor()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the query used to find objects that can be resurrected
        /// </summary>
        [DataMember(Name = "resurrection-parameters")]
        public DBQueryGroup ResurrectionParameters { get; set; }

        /// <summary>
        /// Gets or sets the object class that this object applies to
        /// </summary>
        [DataMember(Name = "object-class")]
        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                return this.objectClass;
            }
            set
            {
                if (this.objectClass != null)
                {
                    this.objectClass.PropertyChanged -= objectClass_PropertyChanged;

                }

                this.objectClass = value;

                if (this.objectClass != null)
                {
                    this.objectClass.PropertyChanged += objectClass_PropertyChanged;
                }
            }
        }

        private void objectClass_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (this.KeyChanging != null)
            {
                this.KeyChanging(this, this.objectClass.Name);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this object is disabled
        /// </summary>
        [DataMember(Name = "disabled")]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets the list of child objects contained in this group
        /// </summary>
        [DataMember(Name = "constructors")]
        public Constructors Constructors
        {
            get
            {
                return this.constructors;
            }

            private set
            {
                if (this.constructors != null)
                {
                    this.constructors.ObjectClassScopeProvider = null;
                }

                if (value != null)
                {
                    value.ObjectClassScopeProvider = this;
                }

                this.constructors = value;
            }
        }

        public virtual IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            string path = string.Format("{0}\\{1}", parentPath, this.ObjectClass.Name);

            foreach (SchemaAttributeUsage usage in this.ResurrectionParameters.GetAttributeUsage(path + "\\Resurrection parameters", attribute))
            {
                yield return usage;
            }

            foreach (SchemaAttributeUsage usage in this.Constructors.GetAttributeUsage(path, attribute))
            {
                yield return usage;
            }
        }

        /// <summary>
        /// Gets a list of events to evaluate and send after the construction of an object
        /// </summary>
        [DataMember(Name = "exit-events")]
        public AcmaEvents ExitEvents
        {
            get
            {
                return this.exitEvents;
            }

            private set
            {
                if (this.exitEvents != null)
                {
                    this.exitEvents.ObjectClassScopeProvider = null;
                }

                if (value != null)
                {
                    value.ObjectClassScopeProvider = this;
                }

                this.exitEvents = value;
            }
        }

        /// <summary>
        /// Gets or sets the object class scope provider for this object
        /// </summary>
        [PropertyChanged.AlsoNotifyFor("ObjectClass")]
        public IObjectClassScopeProvider ObjectClassScopeProvider { get; set; }

        /// <summary>
        /// Gets or sets the serialization extension data object
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Gets the name of the object
        /// </summary>
        public string Id
        {
            get
            {
                if (this.ObjectClass == null)
                {
                    return null;
                }
                else
                {
                    return this.ObjectClass.Name;
                }
            }
        }

        /// <summary>
        /// Gets the name of this object
        /// </summary>
        public string Name
        {
            get
            {
                return this.ObjectClass.Name;
            }
        }

        /// <summary>
        /// Returns a string that represents the current object
        /// </summary>
        /// <returns>A string that represents the current object</returns>
        public override string ToString()
        {
            return this.Id;
        }

        /// <summary>
        /// Calls the EvaluateAndExecute function of all the child objects
        /// </summary>
        /// <param name="hologram">The object being processed</param>
        /// <returns>Returns a value indicating whether the object executed</returns>
        public bool EvaluateAndExecute(MAObjectHologram hologram, IList<string> constructorOverrides)
        {
            bool executed = false;

            if (this.HasErrors)
            {
                throw new InvalidOperationException(string.Format("The constructor for object class '{0}' has the following errors that must be fixed:\n{1}", this.ObjectClass.Name, this.ErrorList.Select(t => string.Format("{0}: {1}", t.Key, t.Value)).ToNewLineSeparatedString()));
            }

            if (this.Disabled)
            {
                Logger.WriteLine("Class constructor is disabled", LogLevel.Debug, this.Name);
                return false;
            }

            if (this.ObjectClass != hologram.ObjectClass)
            {
                throw new InvalidOperationException(string.Format("The class constructor for {0} was executed against object of class {1}", this.ObjectClass.Name, hologram.ObjectClass.Name));
            }

            foreach (ExecutableConstructorObject constructor in this.Constructors)
            {
                if (constructor.EvaluateAndExecute(hologram, constructorOverrides))
                {
                    executed = true;
                }
            }

            return executed;
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
            this.ObjectClassScopeProvider = this;
            this.Constructors = new Constructors();
            this.ResurrectionParameters = new DBQueryGroup();
            this.ExitEvents = new AcmaEvents();
        }

        /// <summary>
        /// Occurs just prior to the object being deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        /// <summary>
        /// Occurs after the object has been deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (this.ObjectClass == null)
            {
                this.AddError("ObjectClass", "An object class must be provided");
            }
            else
            {
                this.RemoveError("ObjectClass");
            }
        }
    }
}