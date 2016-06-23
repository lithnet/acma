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

namespace Lithnet.Acma
{
    [DataContract(Name = "exit-event-internal", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    [Description("Exit event")]
    public class AcmaInternalExitEvent : AcmaEvent
    {
        public AcmaInternalExitEvent()
        {
            this.Initialize();
        }

        public override AcmaEventType EventType
        {
            get
            {
                return AcmaEventType.InternalExitEvent;
            }
        }

        /// <summary>
        /// Gets or sets the rule group set that determines if this event should be raised
        /// </summary>
        [DataMember(Name = "rule-group")]
        public RuleGroup RuleGroup
        {
            get
            {
                return this.ruleGroup;
            }

            set
            {
                if (this.ruleGroup != null)
                {
                    this.ruleGroup.ObjectClassScopeProvider = null;
                }

                if (value != null)
                {
                    value.ObjectClassScopeProvider = this;
                }

                this.ruleGroup = value;
            }
        }

        /// <summary>
        /// The rule group used for determining if the event should be raised
        /// </summary>
        private RuleGroup ruleGroup;

        /// <summary>
        /// Gets or sets a database query that is used to find event recipients
        /// </summary>
        [DataMember(Name = "recipient-queries")]
        public ObservableCollection<DBQueryObject> RecipientQueries { get; set; }

        /// <summary>
        /// Gets or sets a list of attribute containing event recipients
        /// </summary>
        [DataMember(Name = "recipients")]
        public ObservableCollection<AcmaSchemaAttribute> Recipients { get; set; }

        public IEnumerable<MAObjectHologram> GetEventRecipients(MAObjectHologram hologram = null)
        {
            List<MAObjectHologram> recipients = new List<MAObjectHologram>();

            recipients.AddRange(this.GetEventRecipientsFromAttributes(hologram));
            recipients.AddRange(this.GetEventRecipientsFromQuery(hologram));

            return recipients.Distinct();
        }

        /// <summary>
        /// Gets the recipients of the specified exit event
        /// </summary>
        /// <param name="exitEventRecipients">A list of reference attributes to obtain recipients from</param>
        /// <returns>A list of MAObjectHologram to send the specified exit event to</returns>
        private IList<MAObjectHologram> GetEventRecipientsFromAttributes(MAObjectHologram hologram)
        {
            List<MAObjectHologram> recipients = new List<MAObjectHologram>();

            foreach (AcmaSchemaAttribute recipientAttribute in this.Recipients)
            {
                AttributeValues values = hologram.GetAttributeValues(recipientAttribute);

                if (!values.IsEmptyOrNull)
                {
                    foreach (AttributeValue value in values)
                    {
                        MAObjectHologram recipient = ActiveConfig.DB.GetMAObjectOrDefault(value.ValueGuid);
                        if (recipient == null)
                        {
                            hologram.MarkMissingReference(value.ValueGuid, recipientAttribute);
                        }
                        else
                        {
                            recipients.Add(recipient);
                        }
                    }
                }
            }

            return recipients;
        }

        private IList<MAObjectHologram> GetEventRecipientsFromQuery(MAObjectHologram hologram)
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

                queryRecipients = ActiveConfig.DB.GetMAObjectsFromDBQuery(group, hologram);

                if (queryRecipients != null)
                {
                    recipients.AddRange(queryRecipients);
                }
            }

            return recipients;
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.RuleGroup = new RuleGroup();
            this.RuleGroup.Items.CollectionChanged += this.RuleGroup_CollectionChanged;

            this.Recipients = new ObservableCollection<AcmaSchemaAttribute>();
            this.Recipients.CollectionChanged += this.Recipients_CollectionChanged;

            this.RecipientQueries = new ObservableCollection<DBQueryObject>();
            this.RecipientQueries.CollectionChanged += this.RecipientQueries_CollectionChanged;
            this.ValidatePropertyChange("RuleGroup");
        }
       
        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "RuleGroup":
                    if (this.RuleGroup == null || this.RuleGroup.Items.Count == 0)
                    {
                        this.AddError("RuleGroup", "A rule group must be present with one or more rules");
                    }
                    else
                    {
                        this.RemoveError("RuleGroup");
                    }

                    break;
            }
        }

        private void ValidateRecipientQueriesChanges()
        {
            if (this.EventType == AcmaEventType.OperationEvent)
            {
                if ((this.RecipientQueries == null || this.RecipientQueries.Count == 0))
                {
                    this.AddError("RecipientQueries", "At least one recipient query must be present");
                }
                else
                {
                    this.RemoveError("RecipientQueries");
                }
            }
            else
            {
                if ((this.RecipientQueries == null || this.RecipientQueries.Count == 0) && (this.Recipients == null || this.Recipients.Count == 0))
                {
                    this.AddError("RecipientQueries", "At least one recipient attribute or query must be present");
                    this.AddError("Recipients", "At least one recipient attribute or query must be present");
                }
                else
                {
                    this.RemoveError("RecipientQueries");
                    this.RemoveError("Recipients");
                }
            }
        }

        /// <summary>
        /// Occurs when an item in the recipient query group changes
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event arguments</param>
        private void RecipientQueries_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ValidatePropertyChange("RecipientQueries");
        }

        /// <summary>
        /// Occurs when an item in the recipients group changes
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event arguments</param>
        private void Recipients_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ValidatePropertyChange("Recipients");
        }

        /// <summary>
        /// Occurs when an item in the rule group changes
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The event arguments</param>
        private void RuleGroup_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.ValidatePropertyChange("RuleGroup");
        }
    }
}
