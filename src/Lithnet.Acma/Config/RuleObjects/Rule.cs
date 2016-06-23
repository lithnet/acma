// -----------------------------------------------------------------------
// <copyright file="RuleBase.cs" company="Lithnet">
// Copyright (c) 2013
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using Microsoft.MetadirectoryServices;
    using Lithnet.Logging;
    using Lithnet.MetadirectoryServices;
    using System.Runtime.Serialization;
    using System.Xml.Schema;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// An abstract base class used for the implementation of constructor rules
    /// </summary>
    [KnownType(typeof(AttributeChangeRule))]
    [KnownType(typeof(AttributePresenceRule))]
    [KnownType(typeof(EventRule))]
    [KnownType(typeof(ValueComparisonRule))]
    [KnownType(typeof(ObjectChangeRule))]
    [KnownType(typeof(AdvancedComparisonRule))]
    [DataContract(Name = "rule", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public abstract class Rule : RuleObject
    {
        public delegate void RuleFailedEventHandler(Rule sender, string failureReason);

        public static event RuleFailedEventHandler RuleFailedEvent;
        
        /// <summary>
        /// Initializes a new instance of the RuleBase class
        /// </summary>
        protected Rule()
        {
        }
       
        protected void RaiseRuleFailure(string message, params object[] args)
        {
            if (Rule.RuleFailedEvent != null)
            {
                Rule.RuleFailedEvent(this, string.Format(message, args));
            }
        }
    }
}
