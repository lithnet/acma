namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Escapes a string using the specified escaping rules
    /// </summary>
    [DataContract(Name = "string-escape", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Escape string")]
    public class StringEscapeTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the StringCaseTransform class
        /// </summary>
        public StringEscapeTransform()
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
                yield return ExtendedAttributeType.Boolean;
                yield return ExtendedAttributeType.Integer;
            }
        }

        /// <summary>
        /// Gets or sets the type of escaping to apply to the string
        /// </summary>
        [DataMember(Name = "escape-type")]
        public StringEscapeType EscapeType { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.EscapeValue(TypeConverter.ConvertData<string>(inputValue));
        }

        /// <summary>
        /// Escapes the specified string
        /// </summary>
        /// <param name="value">The incoming value to transform</param>
        /// <returns>A copy of the original value with its case modified</returns>
        private object EscapeValue(string value)
        {
            switch (this.EscapeType)
            {
                case StringEscapeType.Xml:
                    return this.EscapeXml(value);

                case StringEscapeType.LdapDN:
                    return this.EscapeLdapDN(value);

                default:
                    throw new ArgumentException();
            }
        }

        private string EscapeXml(string value)
        {
            return System.Security.SecurityElement.Escape(value);
        }

        private string EscapeLdapDN(string s)
        {
            var replacements = new Dictionary<string, string>()
                { {@"\", @"\\"},
                  {@"#", @"\#"},
                  {@"+", @"\+"},
                  {@";", @"\;"},
                  {@"=", @"\="},
                  {"\"", "\\\""},
                  {@"<", @"\<"},
                  {@">", @"\>"}};

            string ret = s;

            //escape the chars that need to be escaped
            foreach (var pair in replacements)
            {
                ret = ret.Replace(pair.Key, pair.Value);
            }

            var whiteSpaceEscapeChars = @"\";
            //escape leading white space
            int whiteSpaceCount = 0;
            while (whiteSpaceCount < ret.Length && Char.IsWhiteSpace(ret[whiteSpaceCount]))
            {
                ret = String.Format("{0}{1}{2}", ret.Substring(0, whiteSpaceCount), whiteSpaceEscapeChars,
                    ret.Substring(whiteSpaceCount));
                whiteSpaceCount += 1 + whiteSpaceEscapeChars.Length;
            }

            //escape trailing whitespace
            if (whiteSpaceCount < ret.Length)
            {
                whiteSpaceCount = ret.Length - 1;
                while (whiteSpaceCount >= 0 && Char.IsWhiteSpace(ret[whiteSpaceCount]))
                {
                    ret = String.Format("{0}{1}{2}", ret.Substring(0, whiteSpaceCount), whiteSpaceEscapeChars,
                        ret.Substring(whiteSpaceCount));
                    whiteSpaceCount--;
                }
            }

            if (ret.Contains(","))
            {
                ret = "\"" + ret + "\"";
            }

            return ret;
        }
    }
}