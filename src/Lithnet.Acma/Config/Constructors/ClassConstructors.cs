// -----------------------------------------------------------------------
// <copyright file="ClassConstructors.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Runtime.Serialization;
    using Lithnet.Common.ObjectModel;
    using System.Collections.Generic;
    using Lithnet.Acma.DataModel;

    /// <summary>
    /// A keyed collection of ClassConstructor objects
    /// </summary>
    [CollectionDataContract(Name = "class-constructors", ItemName = "class-constructor", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class ClassConstructors : ObservableKeyedCollection<string, ClassConstructor>, IExtensibleDataObject
    {
        /// <summary>
        /// Initializes a new instance of the ClassConstructors class
        /// </summary>
        public ClassConstructors()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the serialization extension data object
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        public virtual IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            string path = string.Format("{0}\\{1}", parentPath, "Class constructors");
            
            foreach (ClassConstructor constructor in this)
            {
                foreach(SchemaAttributeUsage usage in constructor.GetAttributeUsage(path, attribute))
                {
                    yield return usage;
                }
            }
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index
        /// </summary>
        /// <param name="index">The index at which to insert the item</param>
        /// <param name="item">The item to insert</param>
        protected override void InsertItem(int index, ClassConstructor item)
        {
            base.InsertItem(index, item);
            item.KeyChanging += item_KeyChanging;
        }

        /// <summary>
        /// Removes an item at a specific index from the collection
        /// </summary>
        /// <param name="index">The index of the item to remove</param>
        protected override void RemoveItem(int index)
        {
            ClassConstructor item = this[index];

            if (item != null)
            {
                item.KeyChanging -= item_KeyChanging;
            }

            base.RemoveItem(index);
        }

        private void item_KeyChanging(object sender, string e)
        {
            ClassConstructor item = (ClassConstructor)sender;
            this.ChangeItemKey(item, e);
        }

        /// <summary>
        /// Extracts the key from the specified element
        /// </summary>
        /// <param name="item">The element from which to extract the key</param>
        /// <returns>The key for the specified element</returns>
        protected override string GetKeyForItem(ClassConstructor item)
        {
            return item.ObjectClass.Name;
        }
    }
}
