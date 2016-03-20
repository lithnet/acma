using Lithnet.Acma;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Lithnet.Common.ObjectModel;
using System.Xml;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for EventRuleTest and is intended
    ///to contain all EventRuleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EventRuleTest
    {
        public EventRuleTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            EventRule toSeralize = new EventRule();
            toSeralize.EventName = "attribute";
            toSeralize.EventSource = ActiveConfig.DB.GetAttribute("supervisor");
            UniqueIDCache.ClearIdCache();

            EventRule deserialized = (EventRule)UnitTestControl.XmlSerializeRoundTrip<EventRule>(toSeralize);

            Assert.AreEqual(toSeralize.EventName, deserialized.EventName);
            Assert.AreEqual(toSeralize.EventSource, deserialized.EventSource);
        }


        [TestMethod()]
        public void EvaluateOnSimpleEvent()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = MAObjectHologram.CreateMAObject(newId, "person");

                AcmaEvent maevent = GetAccountNameChangedEvent();

                RaisedEvent exitEvent = new RaisedEvent(maevent, maObject);
                maObject.IncomingEvents = new List<RaisedEvent>() { exitEvent };

                // Positive Tests
                EventRule target = new EventRule();
                target.EventName = "accountNameChanged";
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests

                maevent = GetSupervisorChangedEvent();
                exitEvent = new RaisedEvent(maevent, maObject);
                maObject.IncomingEvents = new List<RaisedEvent>() { exitEvent };

                target.EventName = "accountNameChanged";
                Assert.IsFalse(target.Evaluate(maObject));

                target.EventName = "accountNameChanged";
                target.EventSource = ActiveConfig.DB.GetAttribute("supervisor");
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        private static AcmaEvent GetAccountNameChangedEvent()
        {
            UniqueIDCache.ClearIdCache();
            AcmaInternalExitEvent maevent = new AcmaInternalExitEvent();
            maevent.ID = "AccountNameChanged";
            maevent.Recipients.Add(ActiveConfig.DB.GetAttribute("supervisor"));
            maevent.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            maevent.RuleGroup.Items.Add(new AttributeChangeRule() { Attribute= ActiveConfig.DB.GetAttribute("accountName"), TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete | TriggerEvents.Update });
            return maevent;
        }

        private static AcmaEvent GetSupervisorChangedEvent()
        {
            UniqueIDCache.ClearIdCache();
            AcmaInternalExitEvent maevent = new AcmaInternalExitEvent();
            maevent.ID = "SupervisorChanged";
            maevent.Recipients.Add(ActiveConfig.DB.GetAttribute("supervisor"));
            maevent.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            maevent.RuleGroup.Items.Add(new AttributeChangeRule() { Attribute = ActiveConfig.DB.GetAttribute("supervisor"), TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete | TriggerEvents.Update });
            return maevent;
        }

        [TestMethod()]
        public void EvaluateOnSourceEvent()
        {
            Guid supervisorId = Guid.NewGuid();
            Guid targetId = Guid.NewGuid();

            try
            {
                MAObjectHologram supervisorObject = MAObjectHologram.CreateMAObject(supervisorId, "person");
                MAObjectHologram targetObject = MAObjectHologram.CreateMAObject(targetId, "person");

                AcmaSchemaAttribute supervisorAttribute = ActiveConfig.DB.GetAttribute("supervisor");
                targetObject.SetAttributeValue(supervisorAttribute, supervisorObject.ObjectID);
                targetObject.CommitCSEntryChange();

                AcmaEvent maevent = GetAccountNameChangedEvent();
                RaisedEvent exitEvent = new RaisedEvent(maevent, supervisorObject);
                targetObject.IncomingEvents = new List<RaisedEvent>() { exitEvent };

                // Positive Tests
                EventRule target = new EventRule();
                target.EventName = "accountNameChanged";
                target.EventSource = ActiveConfig.DB.GetAttribute("supervisor");
                Assert.IsTrue(target.Evaluate(targetObject));

                // Negative Tests
                target = new EventRule();
                target.EventName = "accountNameChanged";
                target.EventSource = ActiveConfig.DB.GetAttribute("directReports");
                Assert.IsFalse(target.Evaluate(targetObject));
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(targetId);
                MAObjectHologram.DeleteMAObjectPermanent(supervisorId);
            }
        }
    }
}
