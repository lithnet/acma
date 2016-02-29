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
    using Lithnet.MetadirectoryServices;
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Lithnet.Acma.ServiceModel;

    /// <summary>
    /// Defines a rule that can be used to determine the presence of an attribute
    /// </summary>    
    [DataContract(Name = "value-comparison-rule", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [System.ComponentModel.Description("Value comparison rule")]
    public class ValueComparisonRule : Rule
    {
        private bool deserializing;

        public ValueComparisonRule()
        {
            this.Initialize();
        }

        [DataMember(Name = "referenced-object")]
        public AcmaSchemaAttribute ReferencedObjectAttribute { get; set; }

        [DataMember(Name = "attribute")]
        public AcmaSchemaAttribute Attribute { get; set; }

        [DataMember(Name = "is-referenced")]
        public bool IsReferenced { get; set; }

        /// <summary>
        /// Gets the operator to apply to the attribute value
        /// </summary>
        [DataMember(Name = "operator")]
        public ValueOperator ValueOperator { get; set; }

        /// <summary>
        /// Gets the expected value of the attribute
        /// </summary>
        [DataMember(Name = "value-declaration")]
        public ValueDeclaration ExpectedValue { get; set; }

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
                yield return new SchemaAttributeUsage(this, "Value comparison rule",
                    null,
                    parentPath,
                    string.Format("{0} {1} {2}", this.Attribute.Name, this.ValueOperator, this.ExpectedValue.Declaration));
            }

            if (this.ReferencedObjectAttribute == attribute)
            {
                yield return new SchemaAttributeUsage(this, "Value comparison rule",
                    null,
                    parentPath,
                    string.Format("{0}->{1} {2} {3}", this.Attribute.Name, this.ReferencedObjectAttribute.Name, this.ValueOperator, this.ExpectedValue.Declaration));
            }

            if (this.ExpectedValue.GetAttributeUsage(parentPath, attribute) != null)
            {
                yield return new SchemaAttributeUsage(this, "Value comparison rule",
                    null,
                   parentPath,
                   string.Format("{0} {1} {2}", this.Attribute.Name, this.ValueOperator, this.ExpectedValue.Declaration));
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
                throw new InvalidOperationException(string.Format("The ValueComparisonRule on attribute '{0}' has the following errors that must be fixed:\n{1}", this.Attribute.Name, this.ErrorList.Select(t => string.Format("{0}: {1}", t.Key, t.Value)).ToNewLineSeparatedString()));
            }

            return this.EvaluateNew(sourceObject);
        }

        private bool EvaluateNew(MAObjectHologram sourceObject)
        {
            IEnumerable<MAObjectHologram> sourceObjects = this.GetSourceObjects(sourceObject);

            IList<object> actualValues = this.GetAttributeValues(sourceObjects);

            IList<object> expectedValues = this.ExpectedValue.Expand(sourceObject);

            if (actualValues.Count == 0)
            {
                if (this.Attribute.Type == ExtendedAttributeType.Boolean)
                {
                    actualValues.Add(false);
                }
                else
                {
                    if (this.ValueOperator == ValueOperator.NotEquals)
                    {
                        return true;
                    }
                    else
                    {
                        if ((this.Attribute.IsMultivalued || (this.ReferencedObjectAttribute != null && this.ReferencedObjectAttribute.IsMultivalued)) && this.GroupOperator == Acma.GroupOperator.None)
                        {
                            return true;
                        }
                        else
                        {
                            this.RaiseRuleFailure("{{{0}}} had no values", this.Attribute.Name);
                            return false;
                        }
                    }
                }
            }

            bool hasSuccess = false;

            foreach (object value in actualValues)
            {

                bool result = this.EvaluateAttributeValue(value, expectedValues);

                switch (this.GroupOperator)
                {
                    case GroupOperator.None:
                        if (result)
                        {
                            this.RaiseRuleFailure("Group operator was set to 'none', but an evaluation succeeded");
                            return false;
                        }

                        break;

                    case GroupOperator.All:
                        if (!result)
                        {
                            this.RaiseRuleFailure("Group operator was set to 'all', but an evaluation failed");
                            return false;
                        }

                        break;

                    case GroupOperator.Any:
                        if (result)
                        {
                            return true;
                        }

                        break;

                    case GroupOperator.One:
                        if (result)
                        {
                            if (hasSuccess)
                            {
                                this.RaiseRuleFailure("Group operator was set to 'one', but a second evaluation succeeded");
                                return false;
                            }
                        }

                        break;
                }

                if (result)
                {
                    hasSuccess = true;
                }
            }

            if (hasSuccess && ((this.GroupOperator == GroupOperator.All) || (this.GroupOperator == GroupOperator.One)))
            {
                return true;
            }
            else if (!hasSuccess && this.GroupOperator == GroupOperator.None)
            {
                return true;
            }
            else
            {
                this.RaiseRuleFailure("No evaluations succeeded");
                return false;
            }
        }

        /// <summary>
        /// Evaluates a specific value against the rule
        /// </summary>
        /// <param name="actualValue">The value to evaluate</param>
        /// <returns>A value indicating whether the rule conditions were met</returns>
        protected bool EvaluateAttributeValue(object actualValue, IList<object> expectedValues)
        {
            if (actualValue == null)
            {
                if (this.Attribute.Type == ExtendedAttributeType.Boolean)
                {
                    actualValue = false;
                }
                else
                {
                    this.RaiseRuleFailure("The value of {{{0}}} was null", this.Attribute.Name);
                    return false;
                }
            }

            bool result = false;

            foreach (object expectedValue in expectedValues)
            {
                switch (this.Attribute.Type)
                {
                    case ExtendedAttributeType.Binary:
                        result = ComparisonEngine.CompareBinary(TypeConverter.ConvertData<byte[]>(actualValue), TypeConverter.ConvertData<byte[]>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.Boolean:
                        result = ComparisonEngine.CompareBoolean(TypeConverter.ConvertData<bool>(actualValue), TypeConverter.ConvertData<bool>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.Integer:
                        result = ComparisonEngine.CompareLong(TypeConverter.ConvertData<long>(actualValue), TypeConverter.ConvertData<long>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.DateTime:
                        result = ComparisonEngine.CompareDateTime(TypeConverter.ConvertData<DateTime>(actualValue), TypeConverter.ConvertData<DateTime>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.String:
                        result = ComparisonEngine.CompareString(TypeConverter.ConvertData<string>(actualValue), TypeConverter.ConvertData<string>(expectedValue), this.ValueOperator);
                        break;

                    case ExtendedAttributeType.Reference:
                        result = ComparisonEngine.CompareString(TypeConverter.ConvertData<string>(actualValue), TypeConverter.ConvertData<string>(expectedValue), this.ValueOperator);
                        break;

                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }

                if (result)
                {
                    break;
                }
            }

            if (!result)
            {
                this.RaiseRuleFailure("Comparison failed\n{{{0}}}\nComparison Operator: {1}\nExpected Values: {2}\nActual Value: {3}", this.Attribute.Name, this.ValueOperator.ToString(), expectedValues.Select(t => t.ToSmartStringOrNull()).ToCommaSeparatedString(), actualValue.ToSmartString());
            }

            return result;
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            if (this.deserializing)
            {
                return;
            }

            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "ObjectClassScopeProvider":
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

        /// <summary>
        /// Gets the conditions required to apply this rule to a multivalued attribute
        /// </summary>
        [DataMember(Name = "multivalued-condition")]
        public GroupOperator GroupOperator { get; set; }

        /// <summary>
        /// Expands the attributes referenced in the constructedValue
        /// </summary>
        /// <param name="maObject">The object to construct the value for</param>
        /// <returns>The constructedValue with any attributes expanded out to their values</returns>
        private IList<object> GetSourceValues(MAObjectHologram maObject)
        {
            IEnumerable<MAObjectHologram> attributeSourceObjects = this.GetSourceObjects(maObject);

            if (attributeSourceObjects == null)
            {
                throw new ReferencedObjectNotPresentException();
            }

            List<object> values = new List<object>();

            foreach (MAObjectHologram sourceObject in attributeSourceObjects)
            {
                IList<object> sourceValues = this.GetAttributeValues(sourceObject);

                if (sourceValues.Count == 0)
                {
                    continue;
                }
                else
                {
                    values.AddRange(sourceValues);
                }
            }

            return values;
        }

        private IList<object> GetAttributeValues(IEnumerable<MAObjectHologram> sourceObjects)
        {
            List<object> values = new List<object>();

            foreach (MAObjectHologram sourceObject in sourceObjects)
            {
                values.AddRange(this.GetAttributeValues(sourceObject));
            }

            return values;
        }

        private IList<object> GetAttributeValues(MAObjectHologram sourceObject)
        {
            List<object> values = new List<object>();

            AttributeValues attributeValues = sourceObject.GetAttributeValues(this.Attribute, this.View);

            if (!attributeValues.IsEmptyOrNull)
            {
                foreach (AttributeValue attributeValue in attributeValues)
                {
                    values.Add(attributeValue.Value);
                }
            }

            if (values.Count == 0 && this.Attribute.Type == ExtendedAttributeType.Boolean)
            {
                values.Add(false);
            }

            return values;
        }

        /// <summary>
        /// Gets the object containing the attribute value to use
        /// </summary>
        /// <param name="sourceObject">The object that is under construction</param>
        /// <returns>If the declaration contains a reference to another object, then that object is returned, otherwise the original object is returned</returns>
        private IEnumerable<MAObjectHologram> GetSourceObjects(MAObjectHologram sourceObject)
        {
            if (this.IsReferenced)
            {
                if (this.ReferencedObjectAttribute == null)
                {
                    throw new InvalidOperationException("The value comparison rule does not have a reference attribute specified");
                }

                IEnumerable<MAObjectHologram> sources = new List<MAObjectHologram>();

                try
                {
                    sources = sourceObject.GetReferencedObjects(this.ReferencedObjectAttribute);

                    if (!sources.Any())
                    {
                        this.RaiseRuleFailure(string.Format("The reference attribute {0} was null", this.ReferencedObjectAttribute.Name));
                    }
                }
                catch (ReferencedObjectNotPresentException)
                {
                    this.RaiseRuleFailure(string.Format("The reference attribute {0} referred to an object which did not exist in the database", this.ReferencedObjectAttribute.Name));
                }

                return sources;
            }
            else
            {
                return new List<MAObjectHologram>() { sourceObject };
            }
        }

        private void Initialize()
        {
            this.Attribute = null;
            this.ExpectedValue = new ValueDeclaration();
            this.GroupOperator = Acma.GroupOperator.Any;
            this.ValueOperator = ValueOperator.Equals;
            this.View = HologramView.Proposed;
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.deserializing = true;
            this.Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.deserializing = false;
            this.ValidateAttributeChange();
        }
    }
}
