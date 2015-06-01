namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Toggles flags in an integer containing a bitmask based on a series of incoming boolean attributes
    /// </summary>
    [LoopbackTransform]
    [DataContract(Name = "mv-boolean-to-bitmask", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Boolean values to bitmask (loopback)")]
    public class MVBooleanToBitmaskTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the MVBooleanToBitmaskTransform class
        /// </summary>
        public MVBooleanToBitmaskTransform()
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
                yield return ExtendedAttributeType.Integer;
            }
        }

        /// <summary>
        /// Defines the input data types that this transform allows
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                yield return ExtendedAttributeType.Boolean;
            }
        }

        /// <summary>
        /// Gets or sets the list of flag values to use in this transform
        /// </summary>
        [DataMember(Name = "flag-values")]
        public List<FlagValue> Flags { get; set; }

        /// <summary>
        /// Gets or sets the default value to use to bitmask is the target value is blank
        /// </summary>
        [DataMember(Name = "default-value")]
        public long DefaultValue { get; set; }

        /// <summary>
        /// This method is unsupported in this transform type
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>This method always throws an exception</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            throw new NotSupportedException("This transform does not support simple transforms");
        }

        /// <summary>
        /// Executes the transformation of the target value based on the input values
        /// </summary>
        /// <param name="inputValues">The incoming values to apply to the target value</param>
        /// <param name="targetValue">The target value to apply the transform to</param>
        /// <returns>The transformed value</returns>
        protected override object TransformMultiValuesWithLoopback(IList<object> inputValues, object targetValue)
        {
            if (!TransformGlobal.HostProcessSupportsLoopbackTransforms)
            {
                throw new ConfigurationException("The hosting process does not support loopback transforms");
            }

            if (targetValue == null || (targetValue is string && string.IsNullOrWhiteSpace((string)targetValue)))
            {
                targetValue = this.DefaultValue;
            }

            if (this.Flags.Count == 0)
            {
                throw new InvalidOperationException("No flag values were defined in the transform " + this.ID);
            }

            if (this.Flags.Count != inputValues.Count)
            {
                throw new ArgumentException(
                   string.Format(
                        "The transform {0} specifies {1} flags, but {2} values were received in the input stream",
                        this.ID, 
                        this.Flags.Count, 
                        inputValues.Count));
            }

            long returnValue = TypeConverter.ConvertData<long>(targetValue);
            int count = 0;

            foreach (bool flagSwitch in inputValues.Select(t => t == null ? false : TypeConverter.ConvertData<bool>(t)))
            {
                BitwiseOperation operation;

                if (flagSwitch)
                {
                    operation = BitwiseOperation.Or;
                }
                else
                {
                    operation = BitwiseOperation.Nand;
                }

                returnValue = this.ProcessFlagValue(this.Flags[count].Value, returnValue, operation);
                count++;
            }

            return returnValue;
        }

        /// <summary>
        /// Applies the flag value using the specified bitwise operator
        /// </summary>
        /// <param name="flag">The value to set or unset</param>
        /// <param name="value">The value to bitmask</param>
        /// <param name="operation">The type of bitwise operation to perform</param>
        /// <returns>The transformed value</returns>
        private long ProcessFlagValue(long flag, long value, BitwiseOperation operation)
        {
            long newValue;

            switch (operation)
            {
                case BitwiseOperation.And:
                    newValue = value & flag;
                    break;

                case BitwiseOperation.Or:
                    newValue = value | flag;
                    break;

                case BitwiseOperation.Nand:
                    newValue = value & ~flag;
                    break;

                case BitwiseOperation.Xor:
                    newValue = value ^ flag;
                    break;

                default:
                    throw new ArgumentException("operator");
            }

            return newValue;
        }

        /// <summary>
        /// Initializes the class
        /// </summary>
        private void Initialize()
        {
            this.Flags = new List<FlagValue>();
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