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
    public class ObjectChangeRuleTest
    {
        public ObjectChangeRuleTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            ObjectChangeRule toSeralize = new ObjectChangeRule();
            toSeralize.TriggerEvents = TriggerEvents.Delete | TriggerEvents.Add;

            ObjectChangeRule deserialized = (ObjectChangeRule)UnitTestControl.XmlSerializeRoundTrip<ObjectChangeRule>(toSeralize);

            Assert.AreEqual(toSeralize.TriggerEvents, deserialized.TriggerEvents);
        }

        /// <summary>
        ///A test for Evaluate when the object modification type is set to 'add'
        ///</summary>
        [TestMethod()]
        public void EvaluateOnObjectAdd()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person", ObjectModificationType.Add);
               
                // Positive Tests
                ObjectChangeRule target = new ObjectChangeRule();
                target.TriggerEvents = TriggerEvents.Add;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update | TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target.TriggerEvents = TriggerEvents.Update | TriggerEvents.Delete;
                Assert.IsFalse(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Delete;
                Assert.IsFalse(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Update;
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnObjectUpdate()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person", ObjectModificationType.Update);

                // Positive Tests
                ObjectChangeRule target = new ObjectChangeRule();
                target.TriggerEvents = TriggerEvents.Update;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update | TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Update | TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = new ObjectChangeRule();
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete;
                Assert.IsFalse(target.Evaluate(maObject));

                target = new ObjectChangeRule();
                target.TriggerEvents = TriggerEvents.Delete;
                Assert.IsFalse(target.Evaluate(maObject));

                target = new ObjectChangeRule();
                target.TriggerEvents = TriggerEvents.Add;
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnObjectReplace()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person", ObjectModificationType.Replace);

                // Positive Tests
                ObjectChangeRule target = new ObjectChangeRule();
                target.TriggerEvents = TriggerEvents.Update;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update | TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Update | TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete;
                Assert.IsFalse(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Delete;
                Assert.IsFalse(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add;
                Assert.IsFalse(target.Evaluate(maObject));

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateOnObjectDelete()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person", ObjectModificationType.Delete);
                
                // Positive Tests
                ObjectChangeRule target = new ObjectChangeRule();
                target.TriggerEvents = TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update | TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Update | TriggerEvents.Delete;
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target.TriggerEvents = TriggerEvents.Add | TriggerEvents.Update;
                Assert.IsFalse(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Update;
                Assert.IsFalse(target.Evaluate(maObject));

                target.TriggerEvents = TriggerEvents.Add;
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
