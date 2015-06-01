// -----------------------------------------------------------------------
// <copyright file="EventRule.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Fim.Core;
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// Defines a rule that can be used to determine the presence of an attribute
    /// </summary>
    [DataContract(Name = "event-rule", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [System.ComponentModel.Description("Event rule")]
    public class EventRule : Rule
    {
        public EventRule()
        {
            this.Initialize();
        }

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
        /// Gets or sets the name of the attribute 
        /// </summary>
        [DataMember(Name = "name")]
        public string EventName { get; set; }

        /// <summary>
        /// Gets or sets the attribute name that this rule applies to
        /// </summary>
        [DataMember(Name = "source")]
        public AcmaSchemaAttribute EventSource { get; set; }

        /// <summary>
        /// Evaluates the conditions of the rule
        /// </summary>
        /// <param name="sourceObject">The MAObject to evaluate</param>
        /// <param name="triggeringObject">The MAObject that is triggering the current evaluation event</param>
        /// <returns>A value indicating whether the rule's conditions were met</returns>
        public override bool Evaluate(MAObjectHologram sourceObject)
        {
            if (this.HasErrors)
            {
                throw new InvalidOperationException(string.Format("The EventRule '{0}' has the following errors that must be fixed:\n{1}", this.EventName, this.ErrorList.Select(t => string.Format("{0}: {1}", t.Key, t.Value)).ToNewLineSeparatedString()));
            }

            if (sourceObject.IncomingEvents == null)
            {
                this.RaiseRuleFailure(string.Format("The object had no incoming events"));
                return false;
            }

            if (this.EventSource == null)
            {
                if (sourceObject.IncomingEvents.Any(t => t.EventID.Equals(this.EventName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("The event '{0}' was not present on the object", this.EventName);
                    return false;
                }
            }
            else
            {
                AttributeValues sources = sourceObject.GetAttributeValues(this.EventSource);
                if (sourceObject.IncomingEvents.Any(t => t.EventID.Equals(this.EventName, StringComparison.CurrentCultureIgnoreCase) && sources.HasValue(t.Source.Id)))
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("The event '{0}' from source {{{1}}} was not present on the object", this.EventName, this.EventSource.Name);
                    return false;
                }
            }
        }

        public override IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            if (this.EventSource == attribute)
            {
                yield return new SchemaAttributeUsage("Event rule", parentPath, string.Format("Event source = {{{0}}}", this.EventSource.Name));
            }
        }

        private void Initialize()
        {
            this.EventName = null;
            this.RaisePropertyChanged("EventName");
        }

        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
            this.RaisePropertyChanged("EventName");
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.ValidateAttributeChange();
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "ObjectClassScopeProvider":
                    this.ValidateAttributeChange();
                    break;

                case "EventSource":
                    this.ValidateAttributeChange();
                    break;

                case "EventName":
                    this.ValidateEventNameChange();
                    break;

                default:
                    break;
            }
        }

        private void ValidateAttributeChange()
        {
            if (this.ObjectClass != null)
            {
                if (this.EventSource == null)
                {
                    this.RemoveError("EventSource");
                }
                else
                {
                    if (!this.ObjectClass.Attributes.Contains(this.EventSource))
                    {
                        this.AddError("EventSource", string.Format("The specified attribute '{0}' does not exist in the object class {1}", this.EventSource.Name, this.ObjectClass.Name));
                    }
                    else if (this.EventSource.Type != ExtendedAttributeType.Reference)
                    {
                        this.AddError("EventSource", string.Format("The event source attribute must be of a reference data type"));
                    }
                    else
                    {
                        this.RemoveError("EventSource");
                    }
                }
            }
        }

        private void ValidateEventNameChange()
        {
            if (string.IsNullOrWhiteSpace(this.EventName))
            {
                this.AddError("EventName", "An event name must be provided");
            }
            else
            {
                this.RemoveError("EventName");
            }
        }
    }
}
