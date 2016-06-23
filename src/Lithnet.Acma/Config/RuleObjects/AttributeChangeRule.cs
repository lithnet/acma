// -----------------------------------------------------------------------
// <copyright file="AttributeChangeRule.cs" company="Lithnet">
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
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// A rule that can detect a state attributeChange in an attribute
    /// </summary>
    [DataContract(Name = "attribute-change-rule", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [System.ComponentModel.Description("Attribute change rule")]
    public class AttributeChangeRule : Rule
    {
        public AttributeChangeRule()
        {
        }

        [DataMember(Name = "attribute")]
        public AcmaSchemaAttribute Attribute { get; set; }

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
        /// Gets or sets the trigger events that this rule applies to 
        /// </summary>
        [DataMember(Name = "triggers")]
        public TriggerEvents TriggerEvents { get; set; }

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
                throw new InvalidOperationException(string.Format("The AttributeChangeRule on attribute '{0}' has the following errors that must be fixed:\n{1}", this.Attribute.Name, this.ErrorList.Select(t => string.Format("{0}: {1}", t.Key, t.Value)).ToNewLineSeparatedString()));
            }

            return this.EvaluateRule(sourceObject);
        }

        public override IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            if (this.Attribute == attribute)
            {
                yield return new SchemaAttributeUsage(this, "Attribute change rule", null, parentPath, string.Format("{{{0}}} {1}", this.Attribute, this.TriggerEvents.ToSmartString()));
            }
        }

        /// <summary>
        /// Evaluates the rule against the specified list of attribute changes
        /// </summary>
        /// <param name="attributeChanges">The list of attribute changes made to the object</param>
        /// <returns>A value indicating whether the conditions of the rule were met</returns>
        private bool EvaulateRuleOnAttributeChanges(KeyedCollection<string, AttributeChange> attributeChanges)
        {
            if (attributeChanges.Contains(this.Attribute.Name))
            {
                if (this.TriggerEvents == Acma.TriggerEvents.None)
                {
                    this.RaiseRuleFailure("An attribute change was present for attribute: {{{0}}}", this.Attribute.Name);
                    return false;
                }
                else if (this.EvaluateRuleOnAttributeChange(attributeChanges[this.Attribute.Name]))
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("The change type on {{{0}}} was not one of the specified trigger events: {1}", this.Attribute.Name, this.TriggerEvents.ToSmartString());
                    return false;
                }
            }
            else
            {
                if (this.TriggerEvents == Acma.TriggerEvents.None)
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("{{{0}}} was not in the list of attribute changes for the object", this.Attribute.Name);
                    return false;
                }
            }
        }

        /// <summary>
        /// Evaluates the rule against the specified attribute attributeChange
        /// </summary>
        /// <param name="attributeChange">The attribute attributeChange to evaluate</param>
        /// <returns>A value indicating whether the conditions of the rule were met</returns>
        private bool EvaluateRuleOnAttributeChange(AttributeChange attributeChange)
        {
            if (this.Attribute.Name != attributeChange.Name)
            {
                throw new InvalidOperationException("The attribute change did not match the expected attribute in the rule");
            }

            if (attributeChange.ModificationType == AttributeModificationType.Add && this.TriggerEvents.HasFlag(TriggerEvents.Add))
            {
                return true;
            }
            else if (attributeChange.ModificationType == AttributeModificationType.Update && this.TriggerEvents.HasFlag(TriggerEvents.Update))
            {
                return true;
            }
            else if (attributeChange.ModificationType == AttributeModificationType.Replace && this.TriggerEvents.HasFlag(TriggerEvents.Update))
            {
                return true;
            }
            else if (attributeChange.ModificationType == AttributeModificationType.Delete && this.TriggerEvents.HasFlag(TriggerEvents.Delete))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Evaluates the conditions of the rule
        /// </summary>
        /// <param name="sourceObject">The MAObject to evaluate</param>
        /// <param name="triggeringObject">The MAObject that is triggering the current evaluation event</param>
        /// <returns>A value indicating whether the conditions of the rule were met</returns>
        private bool EvaluateRule(MAObjectHologram sourceObject)
        {
            return this.EvaluateRuleOnCSEntry(sourceObject);
        }

        /// <summary>
        /// Evaluates the rule against the specified MAObject object
        /// </summary>
        /// <param name="sourceObject">The MAObject to evaluate</param>
        /// <returns>A value indicating whether the conditions of the rule were met</returns>
        private bool EvaluateRuleOnCSEntry(MAObjectHologram sourceObject)
        {
            if (this.EvaulateRuleOnAttributeChanges(sourceObject.InternalAttributeChanges))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Evaluates the conditions of the rule against the update of a referenced object
        /// </summary>
        /// <param name="triggeringObject">The MAObject that is triggering the current evaluation event</param>
        /// <returns>A value indicating whether the conditions of the rule were met</returns>
        private bool EvaulateRuleOnReferencedObjectUpdate(MAObjectHologram triggeringObject)
        {
            foreach (AttributeChange attributeChange in triggeringObject.InternalAttributeChanges.Where(t => this.Attribute.Name == t.Name))
            {
                if (this.EvaluateRuleOnAttributeChange(attributeChange))
                {
                    return true;
                }
            }

            return false;
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
                case "Attribute":
                case "ReferencedObjectAttribute":
                    this.ValidateAttributeChange();
                    break;

                default:
                    break;
            }
        }

        private void ValidateAttributeChange()
        {
            if (this.ObjectClass != null)
            {
                if (this.Attribute == null)
                {
                    this.AddError("Attribute", "An attribute must be specified");
                }
                else
                {
                    if (!this.ObjectClass.Attributes.Contains(this.Attribute))
                    {
                        this.AddError("Attribute", string.Format("The specified attribute '{0}' does not exist in the object class {1}", this.Attribute.Name, this.ObjectClass.Name));
                    }
                    else
                    {
                        this.RemoveError("Attribute");
                    }
                }
            }
        }
    }
}
