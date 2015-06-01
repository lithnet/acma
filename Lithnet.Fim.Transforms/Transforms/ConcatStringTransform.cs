namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Concatenates several string together using a specified delimiter
    /// </summary>
    [DataContract(Name = "concat-string", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Concatenate string")]
    [HandlesOwnMultivaluedInput]
    public class ConcatStringTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the ConcatStringTransform class
        /// </summary>
        public ConcatStringTransform()
        {
        }

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
        /// Gets or sets the delimiter to use between values
        /// </summary>
        [DataMember(Name = "delimiter")]
        public string Delimiter { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return TypeConverter.ConvertData<string>(inputValue);
        }

        /// <summary>
        /// Executes the transformation against the specified values
        /// </summary>
        /// <param name="inputValues">The incoming values to transform</param>
        /// <returns>The transformed values</returns>
        protected override IList<object> TransformMultipleValues(IList<object> inputValues)
        {
            List<object> list = new List<object>();
            StringBuilder builder = new StringBuilder();

            foreach (object value in inputValues)
            {
                if (value == null)
                {
                    continue;
                }

                builder.AppendFormat("{0}{1}", TypeConverter.ConvertData<string>(value), this.Delimiter);
            }

            string newString = builder.ToString();

            if (newString.EndsWith(this.Delimiter))
            {
                list.Add(newString.Substring(0, newString.Length - this.Delimiter.Length));
            }
            else
            {
                list.Add(newString);
            }

            return list;
        }
    }
}