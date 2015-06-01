// -----------------------------------------------------------------------
// <copyright file="Transform.cs" company="Lithnet">
// Copyright(c) 2014 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Fim.Core;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;
    using Lithnet.Common.ObjectModel;
    using System.Xml.Schema;
    using Lithnet.Logging;
    using System.Reflection;

    /// <summary>
    /// Represents an abstract class used as the base for all Transform objects
    /// </summary>
    [KnownType("GetDerivedTypes")]
    [DataContract(Name = "transform", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    public abstract class Transform : UINotifyPropertyChanges, IDisposable, IExtensibleDataObject, IIdentifiedObject
    {
        public const string CacheGroupName = "transform";

        public delegate void KeyChangingEventHandler(object sender, string newKey);

        public event KeyChangingEventHandler KeyChanging;

        private string id;

        private bool deserializing;

        private bool disposed;

        public bool SupportsMultivaluedInput { get; private set; }

        public bool HandlesOwnMultivaluedProcessing { get; private set; }

        public bool ImplementsLoopbackProcessing { get; private set; }

        public ExtensionDataObject ExtensionData { get; set; }

        public abstract IEnumerable<ExtendedAttributeType> PossibleReturnTypes { get; }

        public abstract IEnumerable<ExtendedAttributeType> AllowedInputTypes { get; }

        /// <summary>
        /// Gets the ID of the transform
        /// </summary>
        [DataMember(Name = "id")]
        public string ID
        {
            get
            {
                return this.id;
            }
            set
            {
                try
                {
                    string newID = UniqueIDCache.SetID(this, value, Transform.CacheGroupName, this.deserializing);

                    if (this.id != newID)
                    {
                        if (this.KeyChanging != null)
                        {
                            this.KeyChanging(this, newID);
                        }
                    }

                    this.id = newID;

                    if (this.id == null && UniqueIDCache.HasObject(Transform.CacheGroupName, this))
                    {
                        UniqueIDCache.RemoveItem(this, Transform.CacheGroupName);
                    }
                    else if (!UniqueIDCache.HasObject(Transform.CacheGroupName, this))
                    {
                        UniqueIDCache.AddItem(this, Transform.CacheGroupName);
                    }

                    this.RemoveError("Id");
                }
                catch (DuplicateIdentifierException)
                {
                    this.AddError("Id", "The specified ID is already in use");
                }
            }
        }

        public Transform()
        {
            this.Initialize();
        }

        ~Transform()
        {
            this.Dispose(false);
        }

        public static IList<object> ExecuteTransformChainWithLoopback(IEnumerable<Transform> transforms, IList<object> inputValues, object targetValue)
        {
            IList<object> transformInput = inputValues.ToList();

            int count = 0;

            foreach (Transform transform in transforms)
            {
                count++;

                if (transform.ImplementsLoopbackProcessing)
                {
                    if (count != transforms.Count())
                    {
                        throw new InvalidOperationException("A loopback transform must be executed last in the chain");
                    }

                    transformInput = new List<object>() { transform.TransformMultiValuesWithLoopback(transformInput, targetValue) };
                }
                else
                {
                    transformInput = transform.TransformValue(transformInput);
                }
            }

            return transformInput;
        }

        public static IList<object> ExecuteTransformChain(IEnumerable<Transform> transforms, IList<object> inputValues)
        {
            IList<object> transformInput = inputValues.ToList();

            foreach (Transform transform in transforms)
            {
                if (transform.ImplementsLoopbackProcessing)
                {
                    throw new InvalidOperationException(string.Format("The transform {0}:{1} requires loopback processing, but was executed from a call to a method that does not support it", transform.GetType().Name, transform.ID));
                }

                transformInput = transform.TransformValue(transformInput);
            }

            return transformInput;
        }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputType">The type of data supplied as the input value</param>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        public IList<object> TransformValue(object inputValue)
        {
            if (inputValue != null)
            {
                ExtendedAttributeType inputType = TypeConverter.GetDataTypeExtended(inputValue);
                this.ValidateInputType(inputType);
            }

            object returnValue = this.TransformSingleValue(inputValue);
            IList<object> returnValues = returnValue as IList<object>;

            if (returnValues == null)
            {
                if (returnValue == null)
                {
                    returnValues = new List<object>();
                }
                else
                {
                    returnValues = new List<object>() { returnValue };
                }
            }

            Logger.WriteLine("Transform {0}: {1} -> {2}", LogLevel.Debug, this.ID, inputValue.ToSmartStringOrNull(), returnValues.Where(t => t != null).Select(u => u.ToSmartStringOrNull()).ToCommaSeparatedString());

            return returnValues;
        }

        /// <summary>
        /// Transforms the specified objects
        /// </summary>
        /// <param name="inputType">The type of data in the enumeration</param>
        /// <param name="inputValues">The values to transform</param>
        /// <returns>A list of transformed objects</returns>
        public IList<object> TransformValue(IList<object> inputValues)
        {
            if (inputValues == null)
            {
                throw new ArgumentNullException("inputValues");
            }
            else if (inputValues.Count == 0)
            {
                return inputValues.ToList();
            }
            else if (inputValues.Count == 1)
            {
                return this.TransformValue(inputValues.First());
            }

            if (!this.SupportsMultivaluedInput)
            {
                throw new InvalidOperationException(string.Format("Transform {0} of type {1} does not support multivalued attributes", this.ID, this.GetType().Name));
            }

            IList<object> returnValues = new List<object>();

            if (this.HandlesOwnMultivaluedProcessing)
            {
                foreach (object value in inputValues)
                {
                    if (value != null)
                    {
                        this.ValidateInputType(TypeConverter.GetDataTypeExtended(value));
                    }
                }

                returnValues = this.TransformMultipleValues(inputValues);

                return returnValues;
            }
            else
            {
                foreach (object value in inputValues)
                {
                    IList<object> newValue = this.TransformValue(value);
                    returnValues.AddRange(newValue);
                }
            }

            Logger.WriteLine("Transform {0}: {1} -> {2}",
                    LogLevel.Debug,
                    this.ID,
                    inputValues.Select(t => t.ToSmartStringOrNull()).ToCommaSeparatedString(),
                    returnValues.Where(t => t != null).Select(u => u.ToSmartStringOrNull()).ToCommaSeparatedString());

            return returnValues;
        }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputType">The type of data supplied as the input value</param>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        public object TransformValuesWithLoopback(IList<object> inputValues, object targetValue)
        {
            if (inputValues.Any(t => t == null))
            {
                throw new ArgumentNullException("One or more values passed to the transform were null");
            }

            ExtendedAttributeType inputType = TypeConverter.GetDataTypeExtended(inputValues.First());
            this.ValidateInputType(inputType);
            object returnValue = this.TransformMultiValuesWithLoopback(inputValues, targetValue);
            Logger.WriteLine("Transform {0}: {1} -> {2}", LogLevel.Debug, this.ID, inputValues.Select(t => t.ToSmartStringOrNull()).ToCommaSeparatedString(), returnValue.ToSmartStringOrNull());
            return returnValue;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected abstract object TransformSingleValue(object inputValue);

        protected virtual IList<object> TransformMultipleValues(IList<object> inputValues)
        {
            throw new NotSupportedException("The transform reported that it supported processing multivalued input, but it does not implement the required method");
        }

        protected virtual object TransformMultiValuesWithLoopback(IList<object> inputValues, object targetValue)
        {
            throw new NotSupportedException("The transform reported that it supported processing loopback processing, but it does not implement the required method");
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            switch (propertyName)
            {
                case "Id":
                    if (string.IsNullOrEmpty(this.ID))
                    {
                        this.AddError("Id", "An ID must be specified");
                    }
                    else
                    {
                        if (Regex.IsMatch(this.ID, @"[^a-zA-Z0-9]+"))
                        {
                            this.AddError("Id", "The ID must contain only letters, numbers, hyphen, and underscores");
                        }
                        else
                        {
                            this.RemoveError("Id");
                        }
                    }

                    break;
            }
        }

        protected void ValidateInputType(ExtendedAttributeType inputType)
        {
            if (!this.AllowedInputTypes.Any(t => t == inputType))
            {
                throw new InvalidCastException(string.Format(
                    "The input type {0} for the transform {1} ({2}) was not one of the following valid input types: {3}",
                    inputType.ToString(),
                    this.ID,
                    this.GetType().Name,
                    this.AllowedInputTypes.Select(t => t.ToSmartStringOrNull()).ToCommaSeparatedString()));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    UniqueIDCache.RemoveItem(this, "transform");
                }
            }

            disposed = true;
        }

        private void Initialize()
        {
            this.ImplementsLoopbackProcessing = this.GetType().GetCustomAttributes(true).Any(t => t.GetType() == typeof(LoopbackTransformAttribute));
            this.SupportsMultivaluedInput = !((this.GetType().GetCustomAttributes(true).Any(t => t.GetType() == typeof(MultivaluedInputNotSupportedAttribute))));
            this.HandlesOwnMultivaluedProcessing = this.GetType().GetCustomAttributes(true).Any(t => t.GetType() == typeof(HandlesOwnMultivaluedInputAttribute));

            this.ValidatePropertyChange("Id");
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

        private static Type[] GetDerivedTypes()
        {
            return typeof(Transform).GetDerivedTypes(Assembly.GetExecutingAssembly()).ToArray();
        }
    }
}
