// -----------------------------------------------------------------------
// <copyright file="AttributePresenceRule.cs" company="Lithnet">
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
    [DataContract(Name = "attribute-presence-rule", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [System.ComponentModel.Description("Attribute presence rule")]
    public class AttributePresenceRule : Rule
    {
        public AttributePresenceRule()
        {
            this.Initialize();
        }
      
        [DataMember(Name = "referenced-object")]
        public AcmaSchemaAttribute ReferencedObjectAttribute { get; set; }

        [DataMember(Name = "attribute")]
        public AcmaSchemaAttribute Attribute { get; set; }

        [DataMember(Name = "is-referenced")]
        public bool IsReferenced { get; set; }

        private void attribute_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged("Attribute");
        }

        /// <summary>
        /// Gets the operator to apply to the attribute value
        /// </summary>
        [DataMember(Name="operator")]
        public PresenceOperator Operator { get; set; }

        [DataMember(Name = "view")]
        public HologramView View { get; set; }

        /// <summary>
        /// Gets a value indicating whether this rule is event-based
        /// </summary>
        public override bool IsEventBased
        {
            get
            {
                return false;
            }
        }

        public override IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            if (this.Attribute == attribute)
            {
                yield return new SchemaAttributeUsage("Attribute presence rule", parentPath, string.Format("{{{0}}} {1}", this.Attribute, this.Operator));
            }
        }

        private bool EvaluateReferencedAttribute(MAObjectHologram sourceObject)
        {
            if (this.ReferencedObjectAttribute == null)
            {
                throw new InvalidOperationException("The value comparison rule does not have a reference attribute specified");
            }

            if (this.ReferencedObjectAttribute.IsMultivalued)
            {
                throw new InvalidOperationException("Cannot perform a presence comparison on a multivalued reference source attribute");
            }

            AttributeValues references = sourceObject.GetAttributeValues(this.ReferencedObjectAttribute);

            if (references.IsEmptyOrNull)
            {
                this.RaiseRuleFailure(string.Format("The reference attribute {{{0}}} was null", this.ReferencedObjectAttribute.Name));
                return false;
            }

            Guid refGuid = references.First().ValueGuid;

            MAObjectHologram referenceSource = sourceObject.MADataContext.GetMAObjectOrDefault(refGuid);

            if (referenceSource == null)
            {
                this.RaiseRuleFailure(string.Format("The reference attribute {{{0}}} referred to object {1} which did not exist in the database", this.ReferencedObjectAttribute.Name, refGuid));
                return false;
            }
            else
            {
                return this.EvaluateAttributeValue(referenceSource);
            }
        }

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
                throw new InvalidOperationException(string.Format("The AttributePresenceRule on attribute '{0}' has the following errors that must be fixed:\n{1}", this.Attribute.Name, this.ErrorList.Select(t => string.Format("{0}: {1}", t.Key, t.Value)).ToNewLineSeparatedString()));
            }

            if (this.IsReferenced)
            {
                return this.EvaluateReferencedAttribute(sourceObject);
            }
            else
            {
                return EvaluateAttributeValue(sourceObject);
            }
        }

        private bool EvaluateAttributeValue(MAObjectHologram sourceObject)
        {
            if (sourceObject.HasAttribute(this.Attribute, this.View))
            {
                if (this.Operator == PresenceOperator.IsPresent)
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("{{{0}}} was present", this.Attribute.Name);
                    return false;
                }
            }
            else
            {
                if (this.Operator == PresenceOperator.NotPresent)
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("{{{0}}} was not present", this.Attribute.Name);
                    return false;
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.ValidateAttributeChange();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        private void Initialize()
        {
            this.RaisePropertyChanged("Attribute");
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "ObjectClassScopeProvider":
                    this.ValidateAttributeChange();
                    break;

                case "Attribute":
                case "IsReferenced":
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
                    if (this.IsReferenced)
                    {
                        if (this.ReferencedObjectAttribute == null)
                        {
                            this.AddError("ReferencedObjectAttribute", "A referenced object attribute must be specified");
                        }
                        else
                        {
                            if (!this.ObjectClass.Attributes.Contains(this.ReferencedObjectAttribute))
                            {
                                this.AddError("ReferencedObjectAttribute", string.Format("The specified attribute '{0}' does not exist in the object class {1}", this.ReferencedObjectAttribute.Name, this.ObjectClass.Name));
                            }
                            else
                            {
                                this.RemoveError("ReferencedObjectAttribute");
                            }

                            if (!ActiveConfig.DB.Attributes.Contains(this.Attribute))
                            {
                                this.AddError("Attribute", string.Format("The specified attribute '{0}' does not exist", this.Attribute.Name, this.ObjectClass.Name));
                            }
                            else
                            {
                                this.RemoveError("Attribute");
                            }
                        }
                    }
                    else
                    {
                        this.RemoveError("ReferencedObjectAttribute");

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
}
