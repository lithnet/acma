// -----------------------------------------------------------------------
// <copyright file="SequentialIntegerAllocationConstructor.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Microsoft.MetadirectoryServices;

    /// <summary>
    /// A constructor used to allocate a unique, sequential integer to an attribute value
    /// </summary>
    [DataContract(Name = "sequential-integer-allocation-constructor", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Description("Sequence")]
    public class SequentialIntegerAllocationConstructor : AttributeConstructor
    {
        /// <summary>
        /// Initializes a new instance of the SequentialIntegerAllocationConstructor class
        /// </summary>
        public SequentialIntegerAllocationConstructor()
            : base()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the sequence object used by this constructor
        /// </summary>
        [DataMember(Name = "sequence")]
        public AcmaSequence Sequence { get; set; }

        /// <summary>
        /// Constructs a target attribute value based on the rules in the constructor
        /// </summary>
        /// <param name="hologram">The object to construct the value for</param>
        internal override void Execute(MAObjectHologram hologram)
        {
            long newValue = this.Sequence.GetNextValue();
            List<ValueChange> valueChanges = new List<ValueChange>() { ValueChange.CreateValueAdd(newValue) };

            this.ApplyValueChanges(hologram, valueChanges, AcmaAttributeModificationType.Replace);
            this.RaiseCompletedEvent();
        }

        protected override IEnumerable<SchemaAttributeUsage> GetAttributeUsageInternal(string parentPath, AcmaSchemaAttribute attribute)
        {
            yield break;
        }

        /// <summary>
        /// Validates a change to a property
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed</param>
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "Sequence")
            {
                if (this.Sequence == null)
                {
                    this.AddError("Sequence", "A sequence must be specified");
                }
                else
                {
                    this.RemoveError("Sequence");
                }
            }
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
            this.RaisePropertyChanged("Sequence");
        }

        /// <summary>
        /// Occurs just prior to the object being deserialized
        /// </summary>
        /// <param name="context">The serialization context</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
