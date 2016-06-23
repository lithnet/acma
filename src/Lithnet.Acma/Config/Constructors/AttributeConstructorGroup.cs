// -----------------------------------------------------------------------
// <copyright file="AttributeConstructorGroup.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Logging;
    using System.Collections.Generic;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// A group containing AttributeConstructor and AttributeConstructorGroup objects
    /// </summary>
    [DataContract(Name = "attribute-constructor-group", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class AttributeConstructorGroup : ExecutableConstructorObject, IObjectClassScopeProvider
    {
        /// <summary>
        /// The child constructors in this group
        /// </summary>
        private Constructors constructors;

        /// <summary>
        /// Initializes a new instance of the AttributeConstructorGroup class
        /// </summary>
        public AttributeConstructorGroup()
        {
            this.Initialize();
        }

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

        /// <summary>
        /// Gets the name of this object
        /// </summary>
        public override string Name
        {
            get
            {
                return this.ID;
            }
        }

        /// <summary>
        /// Gets or sets the execution rule to apply to this group
        /// </summary>
        [DataMember(Name = "execution-rule")]
        public GroupExecutionRule ExecutionRule { get; set; }

        /// <summary>
        /// Executes this object if the rule conditions associated with this object are met
        /// </summary>
        /// <param name="hologram">The object to evaluate</param>
        /// <returns>Returns a value indicating whether the constructor group executed</returns>
        public override bool EvaluateAndExecute(MAObjectHologram hologram, IList<string> constructorOverrides)
        {
            if (this.HasErrors)
            {
                throw new InvalidOperationException(string.Format("The constructor group '{0}' has the following errors that must be fixed:\n{1}", this.ID, this.ErrorList.Select(t => string.Format("{0}: {1}", t.Key, t.Value)).ToNewLineSeparatedString()));
            }

            if (this.Disabled)
            {
                Logger.WriteLine("Skipping {0} (disabled)", LogLevel.Debug, this.ID);
                return false;
            }

            if (this.RuleGroup != null)
            {
                if (this.RuleGroup.Evaluate(hologram))
                {
                    this.ExecuteConstructors(hologram, constructorOverrides);
                    return true;
                }
                else
                {
                    if (constructorOverrides != null && constructorOverrides.Contains(this.ID))
                    {
                        Logger.WriteLine("Execution rules overridden for {0}", LogLevel.Debug, this.ID);
                        this.ExecuteConstructors(hologram, constructorOverrides);
                        return true;
                    }
                    else
                    {
                        Logger.WriteLine("Skipping {0} (conditions not met)", LogLevel.Debug, this.ID);
                        return false;
                    }
                }
            }
            else
            {
                return this.ExecuteConstructors(hologram, constructorOverrides);
            }
        }

        public override IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            foreach(SchemaAttributeUsage usage in base.GetAttributeUsage(parentPath, attribute))
            {
                yield return usage;
            }

            string path = string.Format("{0}\\{1}", parentPath, this.ID);

            foreach (SchemaAttributeUsage usage in this.Constructors.GetAttributeUsage(path, attribute))
            {
                yield return usage;
            }
        }

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

            if (propertyName == "Constructors")
            {
                if (this.Constructors != null)
                {
                    this.constructors.ObjectClassScopeProvider = this;
                }
            }
        }

        /// <summary>
        /// Calls the Execute function of all the child objects
        /// </summary>
        /// <param name="hologram">The object being processed</param>
        /// <returns>Returns a value indicating whether the constructor group executed</returns>
        private bool ExecuteConstructors(MAObjectHologram hologram, IList<string> constructorOverrides)
        {
            Logger.WriteLine("Executing: " + this.ID);

            bool executed = false;

            try
            {
                Logger.IncreaseIndent();

                foreach (ExecutableConstructorObject constructor in this.Constructors)
                {
                    executed = constructor.EvaluateAndExecute(hologram, constructorOverrides);

                    if (executed)
                    {
                        if (this.ExecutionRule == GroupExecutionRule.ExitAfterFirstSuccess)
                        {
                            Logger.WriteLine("Exiting group {0} (first match found)", LogLevel.Debug, this.ID);
                            return executed;
                        }
                    }
                }
            }
            finally
            {
                Logger.DecreaseIndent();
            }

            return executed;
        }
        
        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
            this.Constructors = new Constructors();
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
    }
}