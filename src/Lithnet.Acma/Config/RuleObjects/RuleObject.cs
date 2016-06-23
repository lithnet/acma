// -----------------------------------------------------------------------
// <copyright file="IEvaluableRuleObject.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.MetadirectoryServices;
    using System.Runtime.Serialization;
    using System.Xml.Schema;
    using Lithnet.Acma.DataModel;
    using Lithnet.Common.ObjectModel;

    /// <summary>
    /// Defines and interface for rule objects
    /// </summary>
    [KnownType(typeof(Rule))]
    [KnownType(typeof(RuleGroup))]
    [DataContract(Name = "rule-base", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public abstract class RuleObject : UINotifyPropertyChanges,  IObjectClassScopeProvider, IExtensibleDataObject
    {
        /// <summary>
        /// Gets a value indicating whether this rule is event-based
        /// </summary>
        public abstract bool IsEventBased { get; }

        [PropertyChanged.AlsoNotifyFor("ObjectClass")]
        public IObjectClassScopeProvider ObjectClassScopeProvider { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Gets the object class that this rule applies to
        /// </summary>
        public AcmaSchemaObjectClass ObjectClass
        {
            get
            {
                if (this.ObjectClassScopeProvider != null)
                {
                    return this.ObjectClassScopeProvider.ObjectClass;
                }
                else
                {
                    return null;
                }
            }
        }

        public abstract IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute);

        public virtual ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Evaluates the conditions of the rule
        /// </summary>
        /// <param name="hologram">The MAObject to evaluate</param>
        /// <param name="triggeringObject">The MAObject that is triggering the current evaluation event</param>
        /// <returns>A value indicating whether the rule's conditions were met</returns>
        public abstract bool Evaluate(MAObjectHologram hologram);


        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
        }
    }
}
