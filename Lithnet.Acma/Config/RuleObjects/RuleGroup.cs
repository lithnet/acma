// -----------------------------------------------------------------------
// <copyright file="RuleGroup.cs" company="Ryan Newington">
// Copyright (c) 2013 Ryan Newington
// </copyright>
// -----------------------------------------------------------------------

namespace Lithnet.Acma
{
    using System;
    using System.Collections.ObjectModel;
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
    using System.Collections.Specialized;
    using System.Collections;

    /// <summary>
    /// Represents a group of rules
    /// </summary>
    [DataContract(Name = "rule-group", Namespace = "http://lithnet.local/Lithnet.Acma/v1/")]
    public class RuleGroup : RuleObject
    {
        private bool blockEventRules;

        public delegate void RuleGroupFailedEventHandler(RuleGroup sender, string failureReason);

        public static event RuleGroupFailedEventHandler RuleGroupFailedEvent;

        [DataMember(Name = "rules")]
        public ObservableCollection<RuleObject> Items { get; set; }

        public bool BlockEventRules
        {
            get
            {
                return this.blockEventRules;
            }
            set
            {
                this.blockEventRules = value;

                if (this.Items != null)
                {
                    foreach (var item in this.Items.OfType<RuleGroup>())
                    {
                        item.blockEventRules = value;
                    }
                }
            }
        }

        public RuleGroup()
        {
            this.Initialize();
        }

        public override IEnumerable<SchemaAttributeUsage> GetAttributeUsage(string parentPath, AcmaSchemaAttribute attribute)
        {
            string path = parentPath + string.Format ("\\Rule group ({0})", this.Operator.ToString().ToLower());

            foreach (RuleObject item in this.Items)
            {
                foreach (SchemaAttributeUsage usage in item.GetAttributeUsage(path, attribute))
                {
                    yield return usage;
                }
            }
        }

        private void RuleGroup_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ProcessRemovedItems(e.OldItems);
            ProcessNewItems(e.NewItems);
        }

        private void ProcessNewItems(IList items)
        {
            if (items != null)
            {
                foreach (RuleObject item in items)
                {
                    item.ObjectClassScopeProvider = this;

                    if (item is RuleGroup)
                    {
                        ((RuleGroup)item).BlockEventRules = this.BlockEventRules;
                    }
                }
            }
        }

        private void ProcessRemovedItems(IList items)
        {
            if (items != null)
            {
                foreach (RuleObject item in items)
                {
                    if (item.ObjectClassScopeProvider == this)
                    {
                        item.ObjectClassScopeProvider = null;
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this rule is event-based
        /// </summary>
        public override bool IsEventBased
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the logical operator used to apply to this rule group
        /// </summary>
        [DataMember(Name = "operator")]
        public GroupOperator Operator { get; set; }

        /// <summary>
        /// Gets all child objects that inherit from IEvaluableRuleObject
        /// </summary>
        /// <returns>An enumeration of IEvaluableRuleObjects</returns>
        public IEnumerable<RuleObject> GetChildRuleObjects()
        {
            foreach (RuleObject rule in this.Items)
            {
                yield return rule;

                if (rule is RuleGroup)
                {
                    foreach (RuleObject child in ((RuleGroup)rule).GetChildRuleObjects())
                    {
                        yield return child;
                    }
                }
            }
        }

        /// <summary>
        /// Evaluates the rules within the rule group
        /// </summary>
        /// <param name="sourceObject">The MAObject to evaluate</param>
        /// <param name="triggeringObject">The MAObject that is triggering the current evaluation event</param>
        /// <returns>A value indicating whether the rule group's conditions were met</returns>
        public override bool Evaluate(MAObjectHologram sourceObject)
        {
            if (this.HasErrors)
            {
                throw new InvalidOperationException(string.Format("The rule group has the following errors that must be fixed:\n{0}", this.ErrorList.Select(t => string.Format("{0}: {1}", t.Key, t.Value)).ToNewLineSeparatedString()));
            }

            bool hasSuccess = false;

            foreach (RuleObject rule in this.Items.OrderBy(t => t.IsEventBased))
            {
                bool result = rule.Evaluate(sourceObject);

                switch (this.Operator)
                {
                    case GroupOperator.None:
                        if (result)
                        {
                            this.RaiseRuleGroupFailure("Group operator was set to 'none', but an evaluation succeeded");
                            return false;
                        }

                        break;

                    case GroupOperator.All:
                        if (!result)
                        {
                            this.RaiseRuleGroupFailure("Group operator was set to 'all', but an evaluation failed");
                            return false;
                        }

                        break;

                    case GroupOperator.Any:
                        if (result)
                        {
                            return true;
                        }

                        break;

                    case GroupOperator.One:
                        if (result)
                        {
                            if (hasSuccess)
                            {
                                this.RaiseRuleGroupFailure("Group operator was set to 'one', but a second evaluation succeeded");
                                return false;
                            }
                        }

                        break;
                }

                if (result)
                {
                    hasSuccess = true;
                }
            }

            if (hasSuccess && ((this.Operator == GroupOperator.All) || (this.Operator == GroupOperator.One)))
            {
                return true;
            }
            else if (!hasSuccess && this.Operator == GroupOperator.None)
            {
                return true;
            }
            else
            {
                this.RaiseRuleGroupFailure("No evaluations succeeded");
                return false;
            }
        }

        protected void RaiseRuleGroupFailure(string message, params object[] args)
        {
            if (RuleGroup.RuleGroupFailedEvent != null)
            {
                RuleGroup.RuleGroupFailedEvent(this, string.Format(message, args));
            }
        }

        private void Initialize()
        {
            this.Operator = GroupOperator.Any;
            this.Items = new ObservableCollection<RuleObject>();
            this.SetupChangeMonitor();
        }

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.ProcessNewItems(this.Items);
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            switch (propertyName)
            {
                case "Items":
                    this.SetupChangeMonitor();
                    break;

                case "ObjectClassScopeProvider":
                    this.ProcessNewItems(this.Items);
                    break;

                default:
                    break;
            }
        }

        private void SetupChangeMonitor()
        {
            this.Items.CollectionChanged += RuleGroup_CollectionChanged;
        }
    }
}
