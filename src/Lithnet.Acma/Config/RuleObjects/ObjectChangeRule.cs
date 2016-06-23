// -----------------------------------------------------------------------
// <copyright file="ObjectChangeRule.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Lithnet.Logging;
    using Microsoft.MetadirectoryServices;
    using Lithnet.MetadirectoryServices;
    using System.Runtime.Serialization;
    using System.Xml.Schema;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// A rule used to detect a state attributeChange on an object
    /// </summary>
    [DataContract(Name = "object-change-rule", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [System.ComponentModel.Description("Object change rule")]
    public class ObjectChangeRule : Rule
    {
        public ObjectChangeRule()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the trigger events that this rule applies to 
        /// </summary>
        [DataMember(Name="triggers")]
        public TriggerEvents TriggerEvents { get; set; }

        /// <summary>
        /// Gets a value indicating whether this rule is event-based
        /// </summary>
        public override bool IsEventBased
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Evaluates the conditions of the rule
        /// </summary>
        /// <param name="hologram">The MAObject to evaluate</param>
        /// <param name="triggeringObject">The MAObject that is triggering the current evaluation event</param>
        /// <returns>A value indicating whether the rule's conditions were met</returns>
        public override bool Evaluate(MAObjectHologram hologram)
        {
            if (this.TriggerEvents.HasFlag(hologram.AcmaModificationType))
            {
                return true;
            }

            this.RaiseRuleFailure("The change type on the object was not one of the specified trigger events: {0}", this.TriggerEvents.ToSmartString());
            return false;
        }

        public override IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            yield break;
        }

        private void Initialize()
        {
            this.TriggerEvents = Acma.TriggerEvents.Add;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
            this.RaisePropertyChanged("TriggerEvents");
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "TriggerEvents":
                    if (this.TriggerEvents == 0)
                    {
                        this.AddError("TriggerEvents", "At least one trigger event must be specified");
                    }
                    else
                    {
                        this.RemoveError("TriggerEvents");
                    }

                    break;

                default:
                    break;
            }
        }
    }
}