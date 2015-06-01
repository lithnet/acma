// -----------------------------------------------------------------------
// <copyright file="FlowRuleAliasKeyedCollection.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.UniversalMARE
{
    using System;
    using System.Collections;
    using System.Collections.ObjectModel;
    using Lithnet.Fim.Core;
    using System.Xml.Schema;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.IO;
    using System.Linq;
    using Lithnet.Common.ObjectModel;

    /// <summary>
    /// A keyed collection of FlowRuleAlias objects
    /// </summary>

    [CollectionDataContract(Name = "flow-rule-aliases", ItemName = "flow-rule-alias", Namespace = "http://lithnet.local/Lithnet.IdM.UniversalMARE/v1")]
    public class FlowRuleAliasKeyedCollection : ObservableKeyedCollection<string, FlowRuleAlias>, IExtensibleDataObject
    {
        /// <summary>
        /// Initializes a new instance of the FlowRuleAliasKeyedCollection class
        /// </summary>
        public FlowRuleAliasKeyedCollection()
            : base()
        {
        }

        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Extracts the key from the specified element
        /// </summary>
        /// <param name="item">The element from which to extract the key</param>
        /// <returns>The key for the specified element</returns>
        protected override string GetKeyForItem(FlowRuleAlias item)
        {
            return item.Alias;
        }
    }
}
