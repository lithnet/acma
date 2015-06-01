using Lithnet.Acma;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;

using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    [TestClass()]
    public class ValueComparisonRuleTest
    {
        public ValueComparisonRuleTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            ValueComparisonRule toSeralize = new ValueComparisonRule();
            toSeralize.Attribute = ActiveConfig.DB.GetAttribute("firstName");
            toSeralize.ExpectedValue = new ValueDeclaration("test");
            toSeralize.ValueOperator = ValueOperator.Equals;
            toSeralize.View = HologramView.Proposed;

            ValueComparisonRule deserialized = (ValueComparisonRule)UnitTestControl.XmlSerializeRoundTrip<ValueComparisonRule>(toSeralize);

            Assert.AreEqual(toSeralize.Attribute, deserialized.Attribute);
            Assert.AreEqual(toSeralize.ExpectedValue.Declaration, deserialized.ExpectedValue.Declaration);
            Assert.AreEqual(toSeralize.ExpectedValue.TransformsString, deserialized.ExpectedValue.TransformsString);
            Assert.AreEqual(toSeralize.ValueOperator, deserialized.ValueOperator);
            Assert.AreEqual(toSeralize.View, deserialized.View);
        }

        
        [TestMethod()]
        public void EvaluateSVAVRBinaryEqualsTest()
        {
            bool expected = true;
            bool actual;
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("objectSid");
                target.ValueOperator = ValueOperator.Equals;
                target.ExpectedValue = new ValueDeclaration("AAECAwQ=");

                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"), new byte[] { 0, 1, 2, 3, 4 });
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"), new byte[] { 0, 0, 0, 3, 4 });
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateSVAVRStringEqualsProposedTest()
        {
            bool expected = true;
            bool actual;
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("mail");
                target.ValueOperator = ValueOperator.Equals;
                target.ExpectedValue = new ValueDeclaration("test.test@test.com");
                target.View = HologramView.Proposed;

                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "not.test.test@test.com");
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void EvaluateSVAVRStringEqualsCurrentTest()
        {
            bool actual;
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("mail");
                target.ValueOperator = ValueOperator.Equals;
                target.ExpectedValue = new ValueDeclaration("test.test@test.com");
                target.View = HologramView.Current;

                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                actual = target.Evaluate(maObject);
                Assert.AreEqual(false, actual);

                maObject.CommitCSEntryChange();
                // Negative Tests
                actual = target.Evaluate(maObject);
                Assert.AreEqual(true, actual);

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateSVAVRBooleanEqualsTest()
        {
            bool expected = true;
            bool actual;

            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
                target.ValueOperator = ValueOperator.Equals;
                target.ExpectedValue = new ValueDeclaration("true");

                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"), true);
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"), false);
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateSVAVRLongEqualsTest()
        {
            bool expected = true;
            bool actual;

            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("sapExpiryDate");
                target.ValueOperator = ValueOperator.Equals;
                target.ExpectedValue = new ValueDeclaration("1234567890");

                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sapExpiryDate"), 1234567890L);
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sapExpiryDate"), 9876543210L);
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateSVAVRDateTimeEqualsTest()
        {
            bool expected = true;
            bool actual;

            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");

                ValueComparisonRule target = new ValueComparisonRule();
                target.Attribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
                target.ValueOperator = ValueOperator.Equals;
                target.ExpectedValue = new ValueDeclaration("2010-01-01");

                // Positive Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV"), DateTime.Parse("2010-01-01"));
                actual = target.Evaluate(maObject);
                Assert.AreEqual(expected, actual);

                // Negative Tests
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV"), DateTime.Parse("2011-01-01"));
                actual = target.Evaluate(maObject);
                Assert.AreNotEqual(expected, actual);
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
