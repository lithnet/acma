namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Text;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Removes diacritics from a string
    /// </summary>
    [DataContract(Name = "remove-diacritics", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Remove diacritics")]
    public class RemoveDiacriticsTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the RemoveDiacriticsTransform class
        /// </summary>
        public RemoveDiacriticsTransform()
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
                yield return ExtendedAttributeType.String;
            }
        }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.RemoveDiacritics(TypeConverter.ConvertData<string>(inputValue));
        }

        /// <summary>
        /// Removes the diacritics from characters within a string
        /// </summary>
        /// <param name="value">The string containing the diacritics</param>
        /// <returns>The original string with diacritics removed</returns>
        private object RemoveDiacritics(string value)
        {
            string normalizedString = value.Normalize(NormalizationForm.FormD);
            StringBuilder stringBuilder = new StringBuilder();

            foreach (char c in normalizedString)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}