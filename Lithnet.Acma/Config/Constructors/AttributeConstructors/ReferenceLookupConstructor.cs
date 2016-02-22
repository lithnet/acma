// -----------------------------------------------------------------------
// <copyright file="ReferenceLookupConstructor.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using Lithnet.MetadirectoryServices;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// A constructor used to keep a referential link between two objects
    /// </summary>
    [DataContract(Name = "reference-lookup-constructor", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Description("Reference lookup")]
    public class ReferenceLookupConstructor : AttributeConstructor
    {
        /// <summary>
        /// Initializes a new instance of the ReferenceLookupConstructor class
        /// </summary>
        public ReferenceLookupConstructor()
            : base()
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets the database query used to lookup references
        /// </summary>
        [DataMember(Name = "query-group")]
        public DBQueryGroup QueryGroup { get; set; }

        /// <summary>
        /// Gets or sets the action to take when multiple results are found
        /// </summary>
        [DataMember(Name = "multiple-match-action")]
        public MultipleResultAction MultipleResultAction { get; set; }

        /// <summary>
        /// Constructs a target attribute value based on the rules in the constructor
        /// </summary>
        /// <param name="hologram">The object to construct the value for</param>
        internal override void Execute(MAObjectHologram hologram)
        {
            List<MAObjectHologram> matchedObjects = hologram.MADataContext.GetMAObjectsFromDBQuery(this.QueryGroup, hologram).ToList();

            if (matchedObjects.Count == 0)
            {
                hologram.DeleteAttribute(this.Attribute);
            }
            else
            {
                switch (this.MultipleResultAction)
                {
                    case MultipleResultAction.UseAll:
                        if (!this.Attribute.IsMultivalued && matchedObjects.Count > 1)
                        {
                            throw new MultipleMatchException(string.Format("The reference lookup constructor for attribute {0} returned more than one result", this.Attribute));
                        }

                        hologram.SetAttributeValue(this.Attribute, matchedObjects.Select(t => (object)t.ObjectID).ToList());
                        break;

                    case MultipleResultAction.UseFirst:
                        hologram.SetAttributeValue(this.Attribute, matchedObjects.First().ObjectID);
                        break;

                    case MultipleResultAction.UseNone:
                        if (matchedObjects.Count > 1)
                        {
                            hologram.SetAttributeValue(this.Attribute, null);
                        }
                        else
                        {
                            hologram.SetAttributeValue(this.Attribute, matchedObjects.First().ObjectID);
                        }

                        break;

                    case MultipleResultAction.Error:
                        throw new MultipleMatchException(string.Format("The reference lookup constructor for attribute {0} returned more than one result", this.Attribute));

                    default:
                        throw new UnknownOrUnsupportedDataTypeException();
                }
            }

            this.RaiseCompletedEvent();
        }

        protected override IEnumerable<SchemaAttributeUsage> GetAttributeUsageInternal(string parentPath, AcmaSchemaAttribute attribute)
        {
            foreach (SchemaAttributeUsage usage in this.QueryGroup.GetAttributeUsage(parentPath, attribute))
            {
                yield return usage;
            }
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        private void Initialize()
        {
            this.MultipleResultAction = MultipleResultAction.UseFirst;
            this.QueryGroup = new DBQueryGroup();
            this.QueryGroup.Operator = GroupOperator.Any;
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