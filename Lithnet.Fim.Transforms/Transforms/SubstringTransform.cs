namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Returns the specified number of characters from the specified position within a string
    /// </summary>
    [DataContract(Name = "substring", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Substring")]
    public class SubstringTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the SubstringTransform class
        /// </summary>
        public SubstringTransform()
        {
            this.Initialize();
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
        /// Gets or sets the position within the string to obtain the start of the string from
        /// </summary>
        [DataMember(Name = "start-index")]
        public int StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the number of characters to obtain from the specified position within the string
        /// </summary>
        [DataMember(Name = "length")]
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the type of padding to apply when the number of characters in the string is less than the length specified
        /// </summary>
        [DataMember(Name = "padding-type")]
        public PadType PaddingType { get; set; }

        /// <summary>
        /// Gets or sets the character to use when the number of characters in the string is less than the length specified
        /// </summary>
        [DataMember(Name = "pad-character")]
        public string PadCharacter { get; set; }

        /// <summary>
        /// Gets or sets the relative direction of the substring operation
        /// </summary>
        [DataMember(Name = "direction")]
        public Direction Direction { get; set; }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            return this.GetSubstring(TypeConverter.ConvertData<string>(inputValue));
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
                case "Length":
                    if (this.Length <= 0)
                    {
                        this.AddError("Length", "Value must be greater than 0");
                    }
                    else
                    {
                        this.RemoveError("Length");
                    }

                    break;

                case "StartIndex":
                    if (this.StartIndex < 0)
                    {
                        this.AddError("StartIndex", "Value must be greater than or equal to 0");
                    }
                    else
                    {
                        this.RemoveError("StartIndex");
                    }

                    break;

                case "PadCharacter":
                case "PaddingType":
                    if (this.PadCharacter != null && this.PadCharacter.Length > 1)
                    {
                        this.AddError("PadCharacter", "Only a single character is allowed");
                    }
                    else
                    {
                        this.RemoveError("PadCharacter");
                    }

                    if (this.PaddingType == PadType.SpecifiedValue)
                    {
                        if (this.PadCharacter == null || this.PadCharacter.Length == 0)
                        {
                            this.AddError("PadCharacter", "A padding character must be supplied");
                        }
                        else
                        {
                            this.RemoveError("PadCharacter");
                        }
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the component of the string as defined by the parameters of the transform
        /// </summary>
        /// <param name="value">The incoming value to extract the sub string from</param>
        /// <returns>The extracted string</returns>
        private object GetSubstring(string value)
        {
            if (this.Direction == Direction.Left)
            {
                return this.GetSubstringFromLeft(value);
            }
            else
            {
                return this.GetSubstringFromRight(value);
            }
        }

        /// <summary>
        /// Gets the component of the string starting from the left of the string
        /// </summary>
        /// <param name="value">The incoming value to extract the sub string from</param>
        /// <returns>The extracted string</returns>
        private string GetSubstringFromLeft(string value)
        {
            string newString;

            if (this.Length > (value.Length - this.StartIndex))
            {
                newString = value.Substring(this.StartIndex, value.Length - this.StartIndex);

                if (this.PaddingType == PadType.LastCharacter || this.PaddingType == PadType.FirstCharacter)
                {
                    if (string.IsNullOrWhiteSpace(newString))
                    {
                        throw new ArgumentNullException(string.Format("Transform {0} failed. Character padding could not be applied as the string passed to the transform was empty", this.ID));
                    }
                }

                int charactersToAdd = this.Length - newString.Length;
                char? charToPad = null;

                switch (this.PaddingType)
                {
                    case PadType.FirstCharacter:
                        charToPad = newString[0];
                        break;

                    case PadType.LastCharacter:
                        charToPad = newString[newString.Length - 1];
                        break;

                    case PadType.SpecifiedValue:
                        charToPad = this.PadCharacter.FirstOrDefault();
                        break;

                    case PadType.None:
                    default:
                        charToPad = null;
                        break;
                }

                if (charToPad != null)
                {
                    for (int x = 0; x < charactersToAdd; x++)
                    {
                        newString += charToPad;
                    }
                }
            }
            else
            {
                newString = value.Substring(this.StartIndex, this.Length);
            }

            return newString;
        }

        /// <summary>
        /// Gets the component of the string starting from the right of the string
        /// </summary>
        /// <param name="value">The incoming value to extract the sub string from</param>
        /// <returns>The extracted string</returns>
        private string GetSubstringFromRight(string value)
        {
            string newString;
            int startIndex = value.Length - this.Length - this.StartIndex;

            if (startIndex < 0)
            {
                newString = value;
                int charactersToAdd = -startIndex;
                startIndex = 0;

                char? charToPad = null;

                switch (this.PaddingType)
                {
                    case PadType.FirstCharacter:
                        charToPad = newString[0];
                        break;

                    case PadType.LastCharacter:
                        charToPad = newString[newString.Length - 1];
                        break;

                    case PadType.SpecifiedValue:
                        charToPad = this.PadCharacter.FirstOrDefault();
                        break;

                    case PadType.None:
                    default:
                        charToPad = null;
                        break;
                }

                if (charToPad != null)
                {
                    for (int x = 0; x < charactersToAdd; x++)
                    {
                        newString = charToPad + newString;
                    }
                }
            }
            else
            {
                newString = value.Substring(startIndex, this.Length);
            }

            return newString;
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void Initialize()
        {
            this.StartIndex = 0;
            this.Direction = Direction.Left;
            this.Length = 1;
            this.PaddingType = PadType.None;
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
