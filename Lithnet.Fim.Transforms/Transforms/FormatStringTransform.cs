namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Substitutes values into a string
    /// </summary>
    [DataContract(Name = "format-string", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Format string")]
    [HandlesOwnMultivaluedInput]
    public class FormatStringTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the FormatStringTransform class
        /// </summary>
        public FormatStringTransform()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the transform's behavior when dealing with multiple input values
        /// </summary>
        [DataMember(Name = "multivalue-behaviour")]
        public MutivalueBehaviour UserDefinedMultivalueInputBehaviour { get; set; }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.String;
            }
        }

        /// <summary>
        /// Defines the input data types that this transform allows
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.Boolean;
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Binary;
            }
        }

        /// <summary>
        /// Gets or sets the base string to insert the values into
        /// </summary>
        [DataMember(Name = "format")]
        public string Format { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return string.Format(this.Format, inputValue.ToSmartStringOrEmptyString());
        }

        /// <summary>
        /// Executes the transformation against the specified values
        /// </summary>
        /// <param name="inputValues">The incoming values to transform</param>
        /// <returns>The transformed values</returns>
        protected override IList<object> TransformMultipleValues(IList<object> inputValues)
        {
            List<object> returnValues = new List<object>();

            if (this.UserDefinedMultivalueInputBehaviour == MutivalueBehaviour.Grouped)
            {
                returnValues.Add(string.Format(this.Format, inputValues.Select(t => t.ToSmartStringOrEmptyString()).ToArray<object>()));
            }
            else
            {
                foreach (object value in inputValues)
                {
                    returnValues.Add(this.TransformSingleValue(value));
                }
            }

            return returnValues;
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void Initialize()
        {
            this.UserDefinedMultivalueInputBehaviour = MutivalueBehaviour.Grouped;
        }

        /// <summary>
        /// Performs pre-deserialization initialization
        /// </summary>
        /// <param name="context">The current serialization context</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}