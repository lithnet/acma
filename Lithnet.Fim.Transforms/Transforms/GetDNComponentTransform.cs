namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;
    using Microsoft.MetadirectoryServices.DetachedObjectModel;

    /// <summary>
    /// Gets a single component from a DN
    /// </summary>
    [DataContract(Name = "get-dn-component", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Get DN component")]
    public class GetDNComponentTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the GetDNComponentTransform class
        /// </summary>
        public GetDNComponentTransform()
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
        /// Gets or sets the index of the DN component to return
        /// </summary>
        [DataMember(Name = "component-index")]
        public int ComponentIndex { get; set; }

        /// <summary>
        /// Gets or sets the format of the RDN that should be return
        /// </summary>
        [DataMember(Name = "format")]
        public RdnFormat RdnFormat { get; set; }

        /// <summary>
        /// Gets or sets the relative direction of the component index required
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
            return this.ExtractDNComponent(TypeConverter.ConvertData<string>(inputValue));
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
                case "ComponentIndex":
                    if (this.ComponentIndex <= 0)
                    {
                        this.AddError("ComponentIndex", "Value must be greater than 0");
                    }
                    else
                    {
                        this.RemoveError("ComponentIndex");
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Gets the specified DN component from the input string
        /// </summary>
        /// <param name="value">The DN to extract the component from</param>
        /// <returns>The extracted DN component</returns>
        private object ExtractDNComponent(string value)
        {
            string[] split = DetachedUtils.SplitDN(value);

            if (this.ComponentIndex > split.Length)
            {
                throw new ArgumentException(string.Format("The specified DN component index '{0}' does not exist in DN '{1}'", this.ComponentIndex, value));
            }

            if (this.Direction == Direction.Left)
            {
                return this.ExtractDNComponentFromLeft(split);
            }
            else
            {
                return this.ExtractDNComponentFromRight(split);
            }
        }

        /// <summary>
        /// Extracts the specified DN component from the left hand side of the string
        /// </summary>
        /// <param name="dnComponents">The components that make up the DN</param>
        /// <returns>The extracted component</returns>
        private string ExtractDNComponentFromLeft(string[] dnComponents)
        {
            string newValue = string.Empty;

            string rdn = dnComponents[this.ComponentIndex - 1];

            if (this.RdnFormat == RdnFormat.ValueOnly)
            {
                int indexOfEquals = rdn.IndexOf("=", StringComparison.Ordinal);
                newValue = rdn.Remove(0, indexOfEquals + 1);
            }
            else
            {
                newValue = rdn;
            }

            return newValue.Trim();
        }

        /// <summary>
        /// Extracts the specified DN component from the right hand side of the string
        /// </summary>
        /// <param name="dnComponents">The components that make up the DN</param>
        /// <returns>The extracted component</returns>
        private string ExtractDNComponentFromRight(string[] dnComponents)
        {
            string newValue = string.Empty;

            string rdn = dnComponents[dnComponents.Length - this.ComponentIndex];

            if (this.RdnFormat == RdnFormat.ValueOnly)
            {
                int indexOfEquals = rdn.IndexOf("=", StringComparison.Ordinal);
                newValue = rdn.Remove(0, indexOfEquals + 1);
            }
            else
            {
                newValue = rdn;
            }

            return newValue.Trim();
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void Initialize()
        {
            this.ComponentIndex = 1;
            this.Direction = Direction.Left;
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
