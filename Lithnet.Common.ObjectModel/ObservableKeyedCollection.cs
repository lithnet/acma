namespace Lithnet.Common.ObjectModel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    public abstract class ObservableKeyedCollection<TKey, TItem> : KeyedCollection<TKey, TItem>, INotifyCollectionChanged
    {
        private bool deferNotifyCollectionChanged = false;

        public ObservableKeyedCollection()
            : base()
        {
        }

        protected override void SetItem(int index, TItem item)
        {
            base.SetItem(index, item);
            this.RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, index));
        }

        protected override void InsertItem(int index, TItem item)
        {
            base.InsertItem(index, item);
            this.RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            this.RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void RemoveItem(int index)
        {
            TItem item = this[index];
            base.RemoveItem(index);
            this.RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
        }

        public void AddRange(IEnumerable<TItem> items)
        {
            this.deferNotifyCollectionChanged = true;

            foreach (var item in items)
            {
                this.Add(item);
            }
            
            this.deferNotifyCollectionChanged = false;
            this.RaiseCollectionChangedEvent(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected virtual void RaiseCollectionChangedEvent(NotifyCollectionChangedEventArgs e)
        {
            if (this.deferNotifyCollectionChanged)
            {
                return;
            }

            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
    }
}
