// -----------------------------------------------------------------------
// <copyright file="AcmaEvents.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System.Runtime.Serialization;
    using Lithnet.Acma.DataModel;
    using Lithnet.Common.ObjectModel;

    /// <summary>
    /// A keyed collection of AcmaEvents
    /// </summary>
    [CollectionDataContract(Name = "events", ItemName = "event", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class AcmaEvents : ObservableKeyedCollection<string, AcmaEvent>, IExtensibleDataObject, IObjectClassScopeProvider
    {
        /// <summary>
        /// Initializes a new instance of the AcmaEvents class
        /// </summary>
        public AcmaEvents()
            : base()
        {
        }

        /// <summary>
        /// Gets or sets the serialization extension data object
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        /// <summary>
        /// Gets or sets the object class scope provider
        /// </summary>
        [PropertyChanged.AlsoNotifyFor("ObjectClass")]
        public IObjectClassScopeProvider ObjectClassScopeProvider { get; set; }

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
        /// Extracts the key from the specified element
        /// </summary>
        /// <param name="item">The element from which to extract the key</param>
        /// <returns>The key for the specified element</returns>
        protected override string GetKeyForItem(AcmaEvent item)
        {
            return item.ID;
        }

        /// <summary>
        /// Inserts a new item into the collection at the specified index
        /// </summary>
        /// <param name="index">The index to insert the item at</param>
        /// <param name="item">The item to insert</param>
        protected override void InsertItem(int index, AcmaEvent item)
        {
            item.ObjectClassScopeProvider = this;

            base.InsertItem(index, item);
            item.KeyChanging += item_KeyChanging;

        }

        /// <summary>
        /// Removes an item at the specified index
        /// </summary>
        /// <param name="index">The index at which to remove the item</param>
        protected override void RemoveItem(int index)
        {
            AcmaEvent item = this[index];

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
            AcmaEvent item = (AcmaEvent)sender;
            this.ChangeItemKey(item, e);
        }

        /// <summary>
        /// Sets an item at the specified index
        /// </summary>
        /// <param name="index">The index to set the item at</param>
        /// <param name="item">The item to insert</param>
        protected override void SetItem(int index, AcmaEvent item)
        {
            if (item.ObjectClassScopeProvider != this)
            {
                item.ObjectClassScopeProvider = this;
            }

            base.SetItem(index, item);
        }
    }
}
