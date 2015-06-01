using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.ComponentModel;

namespace Lithnet.Common.ObjectModel
{
    public class ObservableAggregateCollection : ObservableCollection<object>
    {

        public void AddCollection(INotifyCollectionChanged collection)
        {
            collection.CollectionChanged += collection_CollectionChanged;

            foreach( object item in collection as ICollection)
            {
                this.Add(item);
            }
        }

        void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach(object item in e.NewItems)
                    {
                        this.Add(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Move:
                    //TODO: Implement item move
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (object item in e.OldItems)
                    {
                        this.Remove(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (object item in e.OldItems)
                    {
                        this.Remove(item);
                    }

                    foreach (object item in e.NewItems)
                    {
                        this.Add(item);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this.Clear();

                    foreach (object item in e.NewItems)
                    {
                        this.Add(item);
                    }

                    break;

                default:
                    break;
            }
        }

        
    }
}
