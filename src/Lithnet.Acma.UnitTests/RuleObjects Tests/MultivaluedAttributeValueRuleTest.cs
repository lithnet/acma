using Lithnet.Acma;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;

using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for MultivaluedAttributeValueRuleTest and is intended
    ///to contain all MultivaluedAttributeValueRuleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MultivaluedAttributeValueRuleTest
    {
        public MultivaluedAttributeValueRuleTest()
        {
            UnitTestControl.Initialize();
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            ValueComparisonRule toSeralize = new ValueComparisonRule();
            toSeralize.Attribute = ActiveConfig.DB.GetAttribute("firstName");
            toSeralize.ExpectedValue = new ValueDeclaration("test");
            toSeralize.GroupOperator = GroupOperator.Any;
            toSeralize.ValueOperator = ValueOperator.Equals;
            toSeralize.View = HologramView.Proposed;
            
            ValueComparisonRule deserialized = (ValueComparisonRule)UnitTestControl.XmlSerializeRoundTrip<ValueComparisonRule>(toSeralize);

            Assert.AreEqual(toSeralize.Attribute, deserialized.Attribute);
            Assert.AreEqual(toSeralize.ExpectedValue.TransformsString, deserialized.ExpectedValue.TransformsString);
            Assert.AreEqual(toSeralize.ExpectedValue.Declaration, deserialized.ExpectedValue.Declaration);
            Assert.AreEqual(toSeralize.GroupOperator, deserialized.GroupOperator);
            Assert.AreEqual(toSeralize.ValueOperator, deserialized.ValueOperator);
            Assert.AreEqual(toSeralize.View, deserialized.View);
        }

        [TestMethod()]
        public void EvaluateMVAVRAnyMatchTest()
        {
            bool actual;
            bool expected = true;

            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.Any;
                target.ExpectedValue = new ValueDeclaration("test.test@test.com");

                
                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test4.test4@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateMVAVRAllMatchTest()
        {
            bool actual;
            bool expected = true;

            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();

                target.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.All;
                target.ExpectedValue = new ValueDeclaration("test.test@test.com");

                
                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test.test@test.com", "test.test@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test4.test4@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateMVAVROneMatchTest()
        {
            bool actual;
            bool expected = true;
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.One;
                target.ExpectedValue = new ValueDeclaration("test.test@test.com");

                
                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test.test@test.com", "test3.test3@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateMVAVRNoMatchTest()
        {
            bool actual;
            bool expected = true;

            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.None;
                target.ExpectedValue = new ValueDeclaration("test.test@test.com");

                
                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test4.test4@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test2.test2@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test.test@test.com", "test3.test3@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);

                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com" });
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
