using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Lithnet.Acma.DataModel;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Acma
{
    [DataContract(Name = "operation-event", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Description("Operation event")]
    public class AcmaOperationEvent : AcmaEvent
    {
        public AcmaOperationEvent()
        {
            this.Initialize();
        }

        public override AcmaEventType EventType
        {
            get
            {
                return AcmaEventType.OperationEvent;
            }
        }

        [DataMember(Name = "operation-types")]
        public AcmaEventOperationType OperationTypes { get; set; }
        
        /// <summary>
        /// Gets or sets a database query that is used to find event recipients
        /// </summary>
        [DataMember(Name = "recipient-queries")]
        public ObservableCollection<DBQueryObject> RecipientQueries { get; set; }

        private void ValidateRecipientQueriesChanges()
        {
            if (this.RecipientQueries == null || this.RecipientQueries.Count == 0)
            {
                this.AddError("RecipientQueries", "At least one recipient query must be present");
            }
            else
            {
                this.RemoveError("RecipientQueries");
            }
        }

        public IEnumerable<MAObjectHologram> GetQueryRecipients()
        {
            List<MAObjectHologram> recipients = new List<MAObjectHologram>();

            foreach (DBQueryObject query in this.RecipientQueries)
            {
                DBQueryGroup group;

                if (query is DBQueryGroup)
                {
                    group = query as DBQueryGroup;
                }
                else if (query is DBQueryByValue)
                {
                    group = new DBQueryGroup();
                    group.Operator = GroupOperator.Any;
                    group.AddChildQueryObjects(query);
                }
                else
                {
                    throw new NotSupportedException();
                }

                IEnumerable<MAObjectHologram> queryRecipients;

                queryRecipients = MAObjectHologram.GetMAObjectsFromDBQuery(group);

                if (queryRecipients != null)
                {
                    recipients.AddRange(queryRecipients);
                }
            }

            return recipients;
        }

        /// <summary>
        /// Initializes the object
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            this.RecipientQueries = new ObservableCollection<DBQueryObject>();
            this.RecipientQueries.CollectionChanged += RecipientQueries_CollectionChanged;
            this.OperationTypes = AcmaEventOperationType.FullImport;
        }

        private void RecipientQueries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ValidateRecipientQueriesChanges();
        }
    }
}
