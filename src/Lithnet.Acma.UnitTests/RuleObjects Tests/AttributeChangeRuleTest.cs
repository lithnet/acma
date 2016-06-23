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
    ///This is a test class for AttributeChangeRuleTest and is intended
    ///to contain all AttributeChangeRuleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AttributeChangeRuleTest
    {
        public AttributeChangeRuleTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            ObjectClassScopeProviderForTest provider = new ObjectClassScopeProviderForTest("person");
            AttributeChangeRule toSeralize = new AttributeChangeRule();
            toSeralize.ObjectClassScopeProvider = provider;
            toSeralize.Attribute = ActiveConfig.DB.GetAttribute("sn");
            toSeralize.TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete;

            AttributeChangeRule deserialized = (AttributeChangeRule)UnitTestControl.XmlSerializeRoundTrip<AttributeChangeRule>(toSeralize);

            deserialized.ObjectClassScopeProvider = provider;
            Assert.AreEqual(toSeralize.Attribute, deserialized.Attribute);
            Assert.AreEqual(toSeralize.TriggerEvents, deserialized.TriggerEvents);
            Assert.IsTrue(deserialized.ErrorCount == 0);
        }

        /// <summary>
        ///A test for Evaluate when the attribute modification type is set to 'add'
        ///</summary>
        [TestMethod()]
        public void EvaluateOnAttributeAdd()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                maObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = maObject.ObjectID.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.ObjectType = maObject.ObjectClass.Name;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("accountName", "mytestvalue"));
                maObject.AttachCSEntryChange(csentry);

                bool expected = true;

                // Positive Tests
                AttributeChangeRule target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add;
                bool actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                bool notExpected = true;
                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Update | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Update;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnNoAttributeChange()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                maObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = maObject.ObjectID.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.ObjectType = maObject.ObjectClass.Name;

                // Positive Tests
                AttributeChangeRule target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.None;
                bool actual = target.Evaluate(maObject);
                Assert.IsTrue(actual);


                // Negative Tests
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("accountName", "mytestvalue"));
                maObject.AttachCSEntryChange(csentry);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.None;
                actual = target.Evaluate(maObject);
                Assert.IsFalse(actual);
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnAttributeUpdate()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                maObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = maObject.ObjectID.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.ObjectType = maObject.ObjectClass.Name;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate("accountName", "mytestvalue"));
                maObject.AttachCSEntryChange(csentry);

                bool expected = true;

                AttributeChangeRule target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Update;
                bool actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Update | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                bool notExpected = true;
                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnAttributeReplace()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                maObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = maObject.ObjectID.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.ObjectType = maObject.ObjectClass.Name;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace("accountName", "mytestvalue"));
                maObject.AttachCSEntryChange(csentry);

                bool expected = true;

                // Positive Tests
                AttributeChangeRule target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Update;
                bool actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Update | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                bool notExpected = true;
                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);

            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnAttributeDelete()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                maObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = maObject.ObjectID.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.ObjectType = maObject.ObjectClass.Name;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete("accountName"));
                maObject.AttachCSEntryChange(csentry);

                bool expected = true;

                // Positive Tests
                AttributeChangeRule target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Delete;
                bool actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Update | TriggerEvents.Delete;
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                bool notExpected = true;
                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Update;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);

                target = new AttributeChangeRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("accountName");
                target.TriggerEvents = TriggerEvents.Add;
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(notExpected, actual);
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
