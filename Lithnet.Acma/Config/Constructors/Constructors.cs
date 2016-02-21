// -----------------------------------------------------------------------
// <copyright file="Constructors.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Lithnet.Common.ObjectModel;
    using System.Collections.Generic;

    /// <summary>
    /// A keyed collection of Constructor objects
    /// </summary>
    [CollectionDataContract(Name = "constructors", ItemName = "constructor", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class Constructors : ObservableKeyedCollection<string, ExecutableConstructorObject>, IObjectClassScopeProvider, IExtensibleDataObject
    {
        /// <summary>
        /// Initializes a new instance of the Constructors class
        /// </summary>
        public Constructors()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the serialization extension data object
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Gets the object class that this rule applies to
        /// </summary>
        public virtual AcmaSchemaObjectClass ObjectClass
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

        /// <summary>
        /// Gets or sets the object class scope provider
        /// </summary>
        [PropertyChanged.AlsoNotifyFor("ObjectClass")]
        public IObjectClassScopeProvider ObjectClassScopeProvider { get; set; }

        public virtual IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            foreach(ExecutableConstructorObject constructor in this)
            {
                foreach (SchemaAttributeUsage usage in constructor.GetAttributeUsage(parentPath, attribute))
                {
                    yield return usage;
                }
            }
        }

        /// <summary>
        /// Extracts the key from the specified element
        /// </summary>
        /// <param name="item">The element from which to extract the key</param>
        /// <returns>The key for the specified element</returns>
        protected override string GetKeyForItem(ExecutableConstructorObject item)
        {
            return item.ID;
        }

        /// <summary>
        /// Inserts an item into the collection at the specified index
        /// </summary>
        /// <param name="index">The index at which to insert the item</param>
        /// <param name="item">The item to insert</param>
        protected override void InsertItem(int index, ExecutableConstructorObject item)
        {
            item.ObjectClassScopeProvider = this;
            base.InsertItem(index, item);
            item.KeyChanging += item_KeyChanging;
        }

        /// <summary>
        /// Removes an item at a specific index from the collection
        /// </summary>
        /// <param name="index">The index of the item to remove</param>
        protected override void RemoveItem(int index)
        {
            ExecutableConstructorObject item = this[index];

            if (item != null)
            {
                item.KeyChanging -= item_KeyChanging;

                if (item.ObjectClassScopeProvider == this)
                {
                    item.ObjectClassScopeProvider = null;
                }
            }
            base.RemoveItem(index);
        }

        private void item_KeyChanging(object sender, string e)
        {
            ExecutableConstructorObject item = (ExecutableConstructorObject)sender;
            this.ChangeItemKey(item, e);
        }

        /// <summary>
        /// Sets at item at a specific location within the collection
        /// </summary>
        /// <param name="index">The index at which to set the item</param>
        /// <param name="item">The item to set</param>
        protected override void SetItem(int index, ExecutableConstructorObject item)
        {
            if (item.ObjectClassScopeProvider != this)
            {
                item.ObjectClassScopeProvider = this;
            }

            base.SetItem(index, item);
        }
    }
}
