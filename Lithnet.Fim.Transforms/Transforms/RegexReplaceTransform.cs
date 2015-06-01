namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Uses a regular expression to find and replace a value
    /// </summary>
    [DataContract(Name = "regex-replace", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Regex find and replace")]
    public class RegexReplaceTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the RegexReplaceTransform class
        /// </summary>
        public RegexReplaceTransform()
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
        /// Gets or sets the regular expression pattern to find
        /// </summary>
        [DataMember(Name = "find-pattern")]
        public string FindPattern { get; set; }

        /// <summary>
        /// Gets or sets the regular expression replace pattern
        /// </summary>
        [DataMember(Name = "replace-pattern")]
        public string ReplacePattern { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.PerformRegexReplace(TypeConverter.ConvertData<string>(inputValue));
        }

        /// <summary>
        /// Validates a change to a property
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "FindPattern":
                    if (string.IsNullOrWhiteSpace(this.FindPattern))
                    {
                        this.AddError("FindPattern", "A value must be specified");
                    }
                    else
                    {
                        this.RemoveError("FindPattern");
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Performs the regular expression match and replacement
        /// </summary>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        private object PerformRegexReplace(string value)
        {
            string newValue = Regex.Replace(value, this.FindPattern ?? string.Empty, this.ReplacePattern ?? string.Empty);

            return string.IsNullOrEmpty(newValue) ? null : newValue;
        }
    }
}