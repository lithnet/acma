namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Looks up a value in a delimited text file and returns another value from the same row in the file
    /// </summary>
    [DataContract(Name = "delimited-text-file-lookup", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Delimited text file lookup")]
    public class DelimitedTextFileLookupTransform : Transform
    {
        /// <summary>
        /// A static regular expression used to parse CSV files
        /// </summary>
        private const string CsvDelimiter = "\\s*,\\s*(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

        /// <summary>
        /// A static regular expression used to parse TSV files
        /// </summary>
        private const string TsvDelimiter = "\\t(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)";

        /// <summary>
        /// An internal in-memory cache of the lines of the file
        /// </summary>
        private List<string[]> cachedLines;

        /// <summary>
        /// A flag that indicates if the file has been read and cached already
        /// </summary>
        private bool hasCachedFile;

        /// <summary>
        /// Initializes a new instance of the DelimitedTextFileLookupTransform class
        /// </summary>
        public DelimitedTextFileLookupTransform()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the data type that this transform should return the result as 
        /// </summary>
        [DataMember(Name = "return-type")]
        public ExtendedAttributeType UserDefinedReturnType { get; set; }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.Binary;
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Boolean;
            }
        }

        /// <summary>
        /// Defines the input data types that this transform allows
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                yield return ExtendedAttributeType.Binary;
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.String;
                yield return ExtendedAttributeType.Boolean;
            }
        }

        /// <summary>
        /// Gets or sets the path to the delimited test file
        /// </summary>
        [DataMember(Name = "filename")]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the text file has a header row
        /// </summary>
        [DataMember(Name = "has-header-row")]
        public bool HasHeaderRow { get; set; }

        /// <summary>
        /// Gets or sets the column number to use to match the incoming value
        /// </summary>
        [DataMember(Name = "find-column")]
        public int FindColumn { get; set; }

        /// <summary>
        /// Gets or sets the column number to use to find the replacement value
        /// </summary>
        [DataMember(Name = "replace-column")]
        public int ReplaceColumn { get; set; }

        /// <summary>
        /// Gets or sets the action to take when no match was found
        /// </summary>
        [DataMember(Name = "on-missing-match")]
        public OnMissingMatch OnMissingMatch { get; set; }

        /// <summary>
        /// Gets or sets the default value to use if the OnMissingMatch value is set to UseDefault
        /// </summary>
        [DataMember(Name = "default-value")]
        public string DefaultValue { get; set; }

        /// <summary>
        /// Gets or sets the type of delimiter used within the file
        /// </summary>
        [DataMember(Name = "delimiter-type")]
        public DelimiterType DelimiterType { get; set; }

        /// <summary>
        /// Gets or sets regular expression to use to break up a custom delimited file
        /// </summary>
        [DataMember(Name = "custom-regex")]
        public string CustomDelimiterRegex { get; set; }

        /// <summary>
        /// Gets or sets a value that is used to start a comment line in the file
        /// </summary>
        [DataMember(Name = "comment-char")]
        public string CommentCharacter { get; set; }

        /// <summary>
        /// Gets or sets the custom escape sequence used in the file
        /// </summary>
        [DataMember(Name = "custom-escape-sequence")]
        public string CustomEscapeSequence { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            object returnValue = this.GetReplacementValueOrDefault(TypeConverter.ConvertData<string>(inputValue));
            return TypeConverter.ConvertData(returnValue, this.UserDefinedReturnType);
        }

        /// <summary>
        /// Performs validation when a property changes
        /// </summary>
        /// <param name="propertyName">The name of the property that changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "FindColumn":
                case "ReplaceColumn":
                    if (this.FindColumn == this.ReplaceColumn)
                    {
                        this.AddError("ReplaceColumn", "The find and replace column numbers must be different");
                    }
                    else if (this.FindColumn < 0)
                    {
                        this.AddError("FindColumn", "The value cannot be a negative number");
                    }
                    else if (this.ReplaceColumn < 0)
                    {
                        this.AddError("ReplaceColumn", "The value cannot be a negative number");
                    }
                    else
                    {
                        this.RemoveError("FindColumn");
                        this.RemoveError("ReplaceColumn");
                    }

                    break;

                case "FileName":
                    if (string.IsNullOrWhiteSpace(this.FileName))
                    {
                        this.AddError("FileName", "A file name must be specified");
                    }
                    else
                    {
                        this.RemoveError("FileName");
                    }

                    break;

                case "OnMissingMatch":
                case "DefaultValue":
                    if (this.OnMissingMatch == Transforms.OnMissingMatch.UseDefault)
                    {
                        if (string.IsNullOrWhiteSpace(this.DefaultValue))
                        {
                            this.AddError("DefaultValue", "A value must be specified");
                        }
                        else
                        {
                            this.RemoveError("DefaultValue");
                        }
                    }
                    else
                    {
                        this.RemoveError("DefaultValue");
                    }

                    break;

                case "DelimiterType":
                case "CustomDelimiterRegex":
                    if (this.DelimiterType == Transforms.DelimiterType.Custom)
                    {
                        if (string.IsNullOrWhiteSpace(this.CustomDelimiterRegex))
                        {
                            this.AddError("CustomDelimiterRegex", "A delimiter regular expression must be provided");
                        }
                        else
                        {
                            try
                            {
                                Regex test = new Regex(this.CustomDelimiterRegex);
                                this.RemoveError("CustomDelimiterRegex");
                            }
                            catch
                            {
                                this.AddError("CustomDelimiterRegex", "The delimiter regular expression was not valid");
                            }
                        }
                    }
                    else
                    {
                        this.RemoveError("CustomDelimiterRegex");
                        this.RemoveError("DelimiterType");
                        this.RemoveError("CustomEscapeSequence");
                    }

                    break;
            }
        }

        /// <summary>
        /// Loads the file into memory
        /// </summary>
        private void CacheFile()
        {
            string delimiter;

            this.cachedLines = new List<string[]>();

            if (this.DelimiterType == Transforms.DelimiterType.CommaSeparated)
            {
                delimiter = DelimitedTextFileLookupTransform.CsvDelimiter;
            }
            else if (this.DelimiterType == Transforms.DelimiterType.TabSeparated)
            {
                delimiter = DelimitedTextFileLookupTransform.TsvDelimiter;
            }
            else
            {
                delimiter = this.CustomDelimiterRegex;
            }

            TextReader reader = System.IO.File.OpenText(this.FileName);

            string line = reader.ReadLine();

            bool hasReadHeaderRow = false;

            while (line != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    if (string.IsNullOrWhiteSpace(this.CommentCharacter) || !line.StartsWith(this.CommentCharacter))
                    {
                        if (!this.HasHeaderRow || (this.HasHeaderRow && hasReadHeaderRow))
                        {
                            string[] columns = Regex.Split(line, delimiter);

                            if (columns.Length < this.FindColumn || columns.Length < this.ReplaceColumn)
                            {
                                throw new ArgumentException("The text file does not contain the required number of columns");
                            }

                            this.cachedLines.Add(columns);
                        }
                        else if (this.HasHeaderRow)
                        {
                            hasReadHeaderRow = true;
                        }
                    }
                }

                line = reader.ReadLine();
            }

            this.hasCachedFile = true;
        }

        /// <summary>
        /// Searches for the input value within the text file data
        /// </summary>
        /// <param name="value">The incoming value to find</param>
        /// <returns>The replacement value that was found in the file, or another value as per the OnMissingMatch parameter</returns>
        private object GetReplacementValueOrDefault(string value)
        {
            if (!this.hasCachedFile)
            {
                this.CacheFile();
            }

            foreach (string[] row in this.cachedLines)
            {
                string sourceValue = this.UnescapeValue(row[this.FindColumn]);

                if (sourceValue.Equals(value, StringComparison.CurrentCultureIgnoreCase))
                {
                    string replaceText = this.UnescapeValue(row[this.ReplaceColumn]);
                    if (string.IsNullOrEmpty(replaceText))
                    {
                        return null;
                    }
                    else
                    {
                        return replaceText;
                    }
                }
            }

            return this.GetValueFromMissingMatch(value);
        }

        /// <summary>
        /// Unescapes the delimiter values 
        /// </summary>
        /// <param name="value">The value to unescape</param>
        /// <returns>The unescaped value</returns>
        private string UnescapeValue(string value)
        {
            if (this.DelimiterType != Transforms.DelimiterType.Custom)
            {
                if (value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal))
                {
                    value = value.Substring(1, value.Length - 1);
                    value = value.Substring(0, value.Length - 1);
                }

                value = value.Replace("\"\"", "\"");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(this.CustomEscapeSequence))
                {
                    value = Regex.Replace(value, Regex.Escape(this.CustomEscapeSequence) + "(.)", "$1");
                }
            }

            return value;
        }

        /// <summary>
        /// Gets the appropriate value to return when no match was found
        /// </summary>
        /// <param name="value">The original value</param>
        /// <returns>The value to use</returns>
        private object GetValueFromMissingMatch(object value)
        {
            switch (this.OnMissingMatch)
            {
                case OnMissingMatch.UseNull:
                    return null;

                case OnMissingMatch.UseOriginal:
                    return value;

                case OnMissingMatch.UseDefault:
                    return this.DefaultValue;

                default:
                    throw new ArgumentException("The OnMissingMatch value is unknown or unsupported");
            }
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void Initialize()
        {
            this.FindColumn = 0;
            this.ReplaceColumn = 1;
            this.DelimiterType = Transforms.DelimiterType.CommaSeparated;
            this.HasHeaderRow = true;
            this.UserDefinedReturnType = ExtendedAttributeType.String;
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
