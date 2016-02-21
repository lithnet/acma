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

    /// <summary>
    /// Defines a rule that can be used to determine the presence of an attribute
    /// </summary>    
    [DataContract(Name = "advanced-comparison-rule", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [System.ComponentModel.Description("Advanced value comparison rule")]
    public class AdvancedComparisonRule : Rule
    {
        private bool deserializing;

        public AdvancedComparisonRule()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the expected value of the attribute
        /// </summary>
        [DataMember(Name = "source-value")]
        public ValueDeclaration SourceValue { get; set; }

        /// <summary>
        /// Gets the operator to apply to the attribute value
        /// </summary>
        [DataMember(Name = "operator")]
        public ValueOperator ValueOperator { get; set; }

        /// <summary>
        /// Gets the expected value of the attribute
        /// </summary>
        [DataMember(Name = "target-value")]
        public ValueDeclaration TargetValue { get; set; }

        [DataMember(Name = "compare-as")]
        public ExtendedAttributeType CompareAs { get; set; }

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
            if (this.SourceValue.GetAttributeUsage(parentPath, attribute) != null)
            {
                yield return new SchemaAttributeUsage(this, "Advanced value comparison rule",
                    null,
                   parentPath,
                   string.Format("{0} {1} {2}", this.SourceValue.Declaration, this.ValueOperator, this.TargetValue.Declaration));
            }

            if (this.TargetValue.GetAttributeUsage(parentPath, attribute) != null)
            {
                yield return new SchemaAttributeUsage(this, "Value comparison rule",
                   null,
                    parentPath,
                   string.Format("{0} {1} {2}", this.SourceValue.Declaration, this.ValueOperator, this.TargetValue.Declaration));
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
                throw new InvalidOperationException(string.Format("The advanced value comparison rule has the following errors that must be fixed:\n{0}", this.ErrorList.Select(t => string.Format("{0}: {1}", t.Key, t.Value)).ToNewLineSeparatedString()));
            }

            return this.EvaluateNew(sourceObject);
        }

        private bool EvaluateNew(MAObjectHologram sourceObject)
        {
            IList<object> sourceValues = this.SourceValue.Expand(sourceObject);
            IList<object> targetValues = this.TargetValue.Expand(sourceObject);

            if (sourceValues.Count == 0)
            {
                if (this.CompareAs == ExtendedAttributeType.Boolean)
                {
                    sourceValues.Add(false);
                }
                else
                {
                    if (this.ValueOperator == ValueOperator.NotEquals)
                    {
                        return true;
                    }
                    else
                    {
                        this.RaiseRuleFailure("Source declaration had no values");
                        return false;
                    }
                }
            }

            if (this.ValueOperator == ValueOperator.IsPresent)
            {
                if (sourceValues.Any(t => t != null))
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("No values were present");
                    return false;
                }
            }

            if (this.ValueOperator == ValueOperator.NotPresent)
            {
                if (sourceValues.Count == 0 || sourceValues.All(t => t == null))
                {
                    return true;
                }
                else
                {
                    this.RaiseRuleFailure("At least one value was present");
                    return false;
                }
            }

            bool hasSuccess = false;

            foreach (object value in sourceValues)
            {

                bool result = this.EvaluateAttributeValue(value, targetValues);

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
                if (this.CompareAs == ExtendedAttributeType.Boolean)
                {
                    actualValue = false;
                }
                else
                {
                    this.RaiseRuleFailure(string.Format("The source value was null"));
                    return false;
                }
            }

            bool result = false;

            foreach (object expectedValue in expectedValues)
            {
                switch (this.CompareAs)
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
                this.RaiseRuleFailure("Comparison failed\nComparison Operator: {0}\nExpected Values: {1}\nActual Value: {2}", this.ValueOperator.ToString(), expectedValues.Select(t => t.ToSmartStringOrNull()).ToCommaSeparatedString(), actualValue.ToSmartString());
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
        }

        /// <summary>
        /// Gets the conditions required to apply this rule to a multivalued attribute
        /// </summary>
        [DataMember(Name = "multivalued-condition")]
        public GroupOperator GroupOperator { get; set; }


        private void Initialize()
        {
            this.SourceValue = new ValueDeclaration();
            this.TargetValue = new ValueDeclaration();
            this.GroupOperator = Acma.GroupOperator.Any;
            this.ValueOperator = ValueOperator.Equals;
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
        }
    }
}
