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
    [TestClass()]
    public class AdvancedComparisonRuleTest
    {
        public AdvancedComparisonRuleTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            AdvancedComparisonRule toSeralize = new AdvancedComparisonRule();
            toSeralize.CompareAs = ExtendedAttributeType.DateTime;
            toSeralize.SourceValue = new ValueDeclaration("test1");
            toSeralize.TargetValue = new ValueDeclaration("test2");
            toSeralize.ValueOperator = ValueOperator.NotContains;
            toSeralize.GroupOperator = GroupOperator.None;

            AdvancedComparisonRule deserialized = (AdvancedComparisonRule)UnitTestControl.XmlSerializeRoundTrip<AdvancedComparisonRule>(toSeralize);

            Assert.AreEqual(toSeralize.CompareAs, deserialized.CompareAs);
            Assert.AreEqual(toSeralize.SourceValue.Declaration, deserialized.SourceValue.Declaration);
            Assert.AreEqual(toSeralize.TargetValue.Declaration, deserialized.TargetValue.Declaration);
            Assert.AreEqual(toSeralize.ValueOperator, deserialized.ValueOperator);
            Assert.AreEqual(toSeralize.GroupOperator, deserialized.GroupOperator);
        }

        [TestMethod()]
        public void EvaluateSVBinaryEqualsTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                AdvancedComparisonRule target = new AdvancedComparisonRule();

                target.CompareAs = ExtendedAttributeType.Binary;
                target.SourceValue = new ValueDeclaration("AAECAwQ=");
                target.TargetValue = new ValueDeclaration("AAECAwQ=");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.Any;

                if (!target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison failed");
                }

                target.TargetValue = new ValueDeclaration("AEECAwQ=");

                if (target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison did not fail");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateSVStringEqualsTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                AdvancedComparisonRule target = new AdvancedComparisonRule();

                target.CompareAs = ExtendedAttributeType.String;
                target.SourceValue = new ValueDeclaration("test1");
                target.TargetValue = new ValueDeclaration("test1");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.Any;

                if (!target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison failed");
                }

                target.TargetValue = new ValueDeclaration("test3");

                if (target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison did not fail");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateSVBooleanEqualsTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                AdvancedComparisonRule target = new AdvancedComparisonRule();

                target.CompareAs = ExtendedAttributeType.Boolean;
                target.SourceValue = new ValueDeclaration("true");
                target.TargetValue = new ValueDeclaration("true");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.Any;

                if (!target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison failed");
                }

                target.TargetValue = new ValueDeclaration("false");

                if (target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison did not fail");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateSVDateTimeEqualsTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                AdvancedComparisonRule target = new AdvancedComparisonRule();

                target.CompareAs = ExtendedAttributeType.DateTime;
                target.SourceValue = new ValueDeclaration("2000-01-01");
                target.TargetValue = new ValueDeclaration("2000-01-01");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.Any;

                if (!target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison failed");
                }

                target.TargetValue = new ValueDeclaration("2001-01-01");

                if (target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison did not fail");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateSVDateTimeGreaterThanTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                AdvancedComparisonRule target = new AdvancedComparisonRule();

                target.CompareAs = ExtendedAttributeType.DateTime;
                target.SourceValue = new ValueDeclaration("2001-01-01");
                target.TargetValue = new ValueDeclaration("2000-01-01");
                target.ValueOperator = ValueOperator.GreaterThan;
                target.GroupOperator = GroupOperator.Any;

                if (!target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison failed");
                }

                target.TargetValue = new ValueDeclaration("2002-01-01");

                if (target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison did not fail");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateMVStringAnyEqualsTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test1", "test2" });
                maObject.CommitCSEntryChange();

                AdvancedComparisonRule target = new AdvancedComparisonRule();

                target.CompareAs = ExtendedAttributeType.String;
                target.SourceValue = new ValueDeclaration("{mailAlternateAddresses}");
                target.TargetValue = new ValueDeclaration("test1");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.Any;

                if (!target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison failed");
                }

                target.TargetValue = new ValueDeclaration("test3");

                if (target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison did not fail");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateMVStringAllEqualsTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test1", "test2" });
                maObject.CommitCSEntryChange();

                AdvancedComparisonRule target = new AdvancedComparisonRule();

                target.CompareAs = ExtendedAttributeType.String;
                target.SourceValue = new ValueDeclaration("{mailAlternateAddresses}");
                target.TargetValue = new ValueDeclaration("{mailAlternateAddresses}");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.All;

                if (!target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison failed");
                }

                target.TargetValue = new ValueDeclaration("test1");

                if (target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison did not fail");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateMVStringNoneEqualsTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test1", "test2" });
                maObject.CommitCSEntryChange();

                AdvancedComparisonRule target = new AdvancedComparisonRule();

                target.CompareAs = ExtendedAttributeType.String;
                target.SourceValue = new ValueDeclaration("{mailAlternateAddresses}");
                target.TargetValue = new ValueDeclaration("test3");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.None;

                if (!target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison failed");
                }

                target.TargetValue = new ValueDeclaration("test1");

                if (target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison did not fail");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateMVStringOneEqualsTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test1", "test2" });
                maObject.CommitCSEntryChange();

                AdvancedComparisonRule target = new AdvancedComparisonRule();

                target.CompareAs = ExtendedAttributeType.String;
                target.SourceValue = new ValueDeclaration("{mailAlternateAddresses}");
                target.TargetValue = new ValueDeclaration("test1");
                target.ValueOperator = ValueOperator.Equals;
                target.GroupOperator = GroupOperator.One;

                if (!target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison failed");
                }

                target.TargetValue = new ValueDeclaration("{mailAlternateAddresses}");

                if (target.Evaluate(maObject))
                {
                    Assert.Fail("The value comparison did not fail");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

    }
}
