// -----------------------------------------------------------------------
// <copyright file="TransformKeyedCollection.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.ObjectModel;
    using Lithnet.Fim.Core;
    using System.Xml.Schema;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.IO;
    using System.Linq;
    using Lithnet.Common.ObjectModel;

    /// <summary>
    /// A keyed collection of Transform objects
    /// </summary>

    [CollectionDataContract(Name = "transforms", ItemName = "transform", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    public class TransformKeyedCollection : ObservableKeyedCollection<string, Transform>, IExtensibleDataObject
    {
        /// <summary>
        /// Initializes a new instance of the TransformKeyedCollection class
        /// </summary>
        public TransformKeyedCollection()
            : base()
        {
        }

        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Extracts the key from the specified element
        /// </summary>
        /// <param name="item">The element from which to extract the key</param>
        /// <returns>The key for the specified element</returns>
        protected override string GetKeyForItem(Transform item)
        {
            return item.ID;
        }

        protected override void InsertItem(int index, Transform item)
        {
            base.InsertItem(index, item);
            item.KeyChanging += item_KeyChanging;
        }

        private void item_KeyChanging(object sender, string e)
        {
            Transform item = (Transform)sender;
            this.ChangeItemKey(item, e);
        }

        protected override void RemoveItem(int index)
        {
            Transform item = this[index];

            if (item != null)
            {
                item.KeyChanging -= item_KeyChanging;
            }

            base.RemoveItem(index);
        }
    }
}
