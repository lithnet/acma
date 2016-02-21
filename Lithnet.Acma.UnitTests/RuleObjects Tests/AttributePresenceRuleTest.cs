using Lithnet.Acma;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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
    ///This is a test class for ObjectChangeRuleTest and is intended
    ///to contain all ObjectChangeRuleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AttributePresenceRuleTest
    {
        public AttributePresenceRuleTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            AttributePresenceRule toSeralize = new AttributePresenceRule();
            toSeralize.Attribute = ActiveConfig.DB.GetAttribute("sn");
            toSeralize.Operator = PresenceOperator.IsPresent;
            toSeralize.View = HologramView.Proposed;
            AttributePresenceRule deserialized = (AttributePresenceRule)UnitTestControl.XmlSerializeRoundTrip<AttributePresenceRule>(toSeralize);

            Assert.AreEqual(toSeralize.Attribute, deserialized.Attribute);
            Assert.AreEqual(toSeralize.Operator, deserialized.Operator);
            Assert.AreEqual(toSeralize.View, deserialized.View);
        }


        [TestMethod()]
        public void EvaluateOnSVAttributePresentProposed()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "mytestvalue");

                // Positive Tests
                AttributePresenceRule target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.Operator = PresenceOperator.IsPresent;
                target.View = HologramView.Proposed;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.Operator = PresenceOperator.NotPresent;
                target.View = HologramView.Proposed;
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnSVAttributePresentCurrent()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "mytestvalue");
                maObject.CommitCSEntryChange();

                // Positive Tests
                AttributePresenceRule target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.Operator = PresenceOperator.IsPresent;
                target.View = HologramView.Current;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.Operator = PresenceOperator.NotPresent;
                target.View = HologramView.Current;
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnSVAttributeNotPresentProposed()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), null);

                // Positive Tests
                AttributePresenceRule target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.Operator = PresenceOperator.NotPresent;
                target.View = HologramView.Proposed;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.Operator = PresenceOperator.IsPresent;
                target.View = HologramView.Proposed;
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnSVAttributeNotPresentCurrent()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), null);
                maObject.CommitCSEntryChange();

                // Positive Tests
                AttributePresenceRule target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.Operator = PresenceOperator.NotPresent;
                target.View = HologramView.Current;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.Operator = PresenceOperator.IsPresent;
                target.View = HologramView.Current;
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnMVAttributeNotPresentProposed()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), null);

                // Positive Tests
                AttributePresenceRule target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("directReports");
                target.Operator = PresenceOperator.NotPresent;
                target.View = HologramView.Proposed;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("directReports");
                target.Operator = PresenceOperator.IsPresent;
                target.View = HologramView.Proposed;
                Assert.IsFalse(target.Evaluate(maObject));

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnMVAttributePresentProposed()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() });

                // Positive Tests
                AttributePresenceRule target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("directReports");
                target.Operator = PresenceOperator.IsPresent;
                target.View = HologramView.Proposed;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("directReports");
                target.Operator = PresenceOperator.NotPresent;
                target.View = HologramView.Proposed;
                Assert.IsFalse(target.Evaluate(maObject));

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnMVAttributeNotPresentCurrent()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), null);
                maObject.CommitCSEntryChange();

                // Positive Tests
                AttributePresenceRule target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("directReports");
                target.Operator = PresenceOperator.NotPresent;
                target.View = HologramView.Current;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("directReports");
                target.Operator = PresenceOperator.IsPresent;
                target.View = HologramView.Current;
                Assert.IsFalse(target.Evaluate(maObject));

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnMVAttributePresentCurrent()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object> { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() });
                maObject.CommitCSEntryChange();

                // Positive Tests
                AttributePresenceRule target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("directReports");
                target.Operator = PresenceOperator.IsPresent;
                target.View = HologramView.Current;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = new AttributePresenceRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("directReports");
                target.Operator = PresenceOperator.NotPresent;
                target.View = HologramView.Current;
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
