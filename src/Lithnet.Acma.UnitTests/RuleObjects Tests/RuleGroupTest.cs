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
    public class RuleGroupTest
    {
        public RuleGroupTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            RuleGroup toSeralize = new RuleGroup();
            toSeralize.Operator = GroupOperator.Any;
            ObjectChangeRule test1 = new ObjectChangeRule();
            test1.TriggerEvents = TriggerEvents.Add;
            toSeralize.Items.Add(test1);

            ObjectChangeRule test2 = new ObjectChangeRule();
            test2.TriggerEvents = TriggerEvents.Delete;
            toSeralize.Items.Add(test2);

            RuleGroup deserialized = (RuleGroup)UnitTestControl.XmlSerializeRoundTrip<RuleGroup>(toSeralize);

            Assert.AreEqual(toSeralize.Items.Count, deserialized.Items.Count);
            Assert.AreEqual(toSeralize.Operator, deserialized.Operator);
            Assert.AreEqual(((ObjectChangeRule)deserialized.Items[0]).TriggerEvents, test1.TriggerEvents);
            Assert.AreEqual(((ObjectChangeRule)deserialized.Items[1]).TriggerEvents, test2.TriggerEvents);
        }


        [TestMethod()]
        public void TestSerializationComplex()
        {
            RuleGroup group1 = new RuleGroup();
            group1.Operator = GroupOperator.Any;

            ObjectChangeRule test1 = new ObjectChangeRule();
            test1.TriggerEvents = TriggerEvents.Add;
            group1.Items.Add(test1);

            ObjectChangeRule test2 = new ObjectChangeRule();
            test2.TriggerEvents = TriggerEvents.Delete;
            group1.Items.Add(test2);

            RuleGroup group2 = new RuleGroup();
            group2.Operator = GroupOperator.One;

            ObjectChangeRule test3 = new ObjectChangeRule();
            test3.TriggerEvents = TriggerEvents.Add;
            group2.Items.Add(test3);

            ObjectChangeRule test4 = new ObjectChangeRule();
            test4.TriggerEvents = TriggerEvents.Delete;
            group2.Items.Add(test4);

            group1.Items.Add(group2);

            RuleGroup deserializedGroup1 = (RuleGroup)UnitTestControl.XmlSerializeRoundTrip<RuleGroup>(group1);

            Assert.AreEqual(group1.Items.Count, deserializedGroup1.Items.Count);
            Assert.AreEqual(group1.Operator, deserializedGroup1.Operator);
            Assert.AreEqual(((ObjectChangeRule)deserializedGroup1.Items[0]).TriggerEvents, test1.TriggerEvents);
            Assert.AreEqual(((ObjectChangeRule)deserializedGroup1.Items[1]).TriggerEvents, test2.TriggerEvents);
            Assert.AreEqual(((RuleGroup)deserializedGroup1.Items[2]).Items.Count, group2.Items.Count);

            RuleGroup deserializedGroup2 = (RuleGroup)deserializedGroup1.Items[2];
            Assert.AreEqual(group2.Operator, deserializedGroup2.Operator);

            Assert.AreEqual(((ObjectChangeRule)(deserializedGroup2.Items[0])).TriggerEvents, test3.TriggerEvents);
            Assert.AreEqual(((ObjectChangeRule)(deserializedGroup2.Items[1])).TriggerEvents, test4.TriggerEvents);
        }


        [TestMethod()]
        public void EvaluateGroupAllTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                // Positive Tests
                RuleGroup target = GetAllGroup();

                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                ValueComparisonRule rule4 = new ValueComparisonRule();
                rule4.Attribute = ActiveConfig.DB.GetAttribute("mail");
                rule4.ValueOperator = ValueOperator.Equals;
                rule4.ExpectedValue = new ValueDeclaration("test.test1@test.com");
                target.Items.Add(rule4);

                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateGroupAllTestNested()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                // Positive Tests
                RuleGroup target = GetAllGroup();
                RuleGroup group2 = GetAllGroup();
                target.Items.Add(group2);
                
                Assert.IsTrue(target.Evaluate(maObject));
                
                // Negative Tests
                ValueComparisonRule rule4 = new ValueComparisonRule();
                rule4.Attribute = ActiveConfig.DB.GetAttribute("mail");
                rule4.ValueOperator = ValueOperator.Equals;
                rule4.ExpectedValue = new ValueDeclaration("test.test1@test.com");
                target.Items.Add(rule4);

                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateGroupAnyTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                // Positive Tests
                RuleGroup target = GetAnyGroup();
                Assert.IsTrue(target.Evaluate(maObject));
                
                target = GetAnyGroupNoMatch();
                target.Items.Add(GetAnyGroup());
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = GetAnyGroupNoMatch();
                Assert.IsFalse(target.Evaluate(maObject));

                target = GetAnyGroupNoMatch();
                target.Items.Add(GetAnyGroupNoMatch());
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateGroupOneTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                // Positive Tests
                RuleGroup target = GetOneGroup();
                Assert.IsTrue(target.Evaluate(maObject));

                target = GetOneGroupNoMatch();
                target.Items.Add(GetOneGroup());
                Assert.IsTrue(target.Evaluate(maObject));

                // Negative Tests
                target = GetOneGroupNoMatch();
                Assert.IsFalse(target.Evaluate(maObject));

                target = GetOneGroupTwoMatches();
                Assert.IsFalse(target.Evaluate(maObject));

                target = GetOneGroup();
                target.Items.Add(GetOneGroup());
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void EvaluateGroupNoneTest()
        {
            Guid newId = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                // Positive Tests
                RuleGroup target = GetNoneGroup();
                Assert.IsTrue(target.Evaluate(maObject));

                target = GetNoneGroup();
                target.Items.Add(GetAnyGroupNoMatch());
                Assert.IsTrue(target.Evaluate(maObject));
                
                // Negative Tests
                target = GetNoneGroupNoMatch();
                Assert.IsFalse(target.Evaluate(maObject));
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }


        private static RuleGroup GetAllGroup()
        {
            ObjectClassScopeProviderForTest provider = new ObjectClassScopeProviderForTest("person");

            RuleGroup target = new RuleGroup();
            target.ObjectClassScopeProvider = provider;
            target.Operator = GroupOperator.All;
            ValueComparisonRule rule1 = new ValueComparisonRule();
            rule1.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule1.ValueOperator = ValueOperator.Equals;
            rule1.ExpectedValue = new ValueDeclaration("test.test@test.com");
            target.Items.Add(rule1);

            ValueComparisonRule rule2 = new ValueComparisonRule();
            rule2.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule2.ValueOperator = ValueOperator.Equals;
            rule2.ExpectedValue = new ValueDeclaration("test.test@test.com");
            target.Items.Add(rule2);

            ValueComparisonRule rule3 = new ValueComparisonRule();
            rule3.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule3.ValueOperator = ValueOperator.Equals;
            rule3.ExpectedValue = new ValueDeclaration("test.test@test.com");
            target.Items.Add(rule3);

            return target;
        }

        private static RuleGroup GetAnyGroup()
        {
            ObjectClassScopeProviderForTest provider = new ObjectClassScopeProviderForTest("person");

            RuleGroup target = new RuleGroup();
            target.ObjectClassScopeProvider = provider;
            target.Operator = GroupOperator.Any;
            ValueComparisonRule rule1 = new ValueComparisonRule();
            rule1.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule1.ValueOperator = ValueOperator.Equals;
            rule1.ExpectedValue = new ValueDeclaration("test.test@test.com");
            target.Items.Add(rule1);

            ValueComparisonRule rule2 = new ValueComparisonRule();
            rule2.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule2.ValueOperator = ValueOperator.Equals;
            rule2.ExpectedValue = new ValueDeclaration("test1.test1@test.com");
            target.Items.Add(rule2);

            ValueComparisonRule rule3 = new ValueComparisonRule();
            rule3.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule3.ValueOperator = ValueOperator.Equals;
            rule3.ExpectedValue = new ValueDeclaration("test2.test2@test.com");
            target.Items.Add(rule3);

            return target;
        }

        private static RuleGroup GetAnyGroupNoMatch()
        {
            ObjectClassScopeProviderForTest provider = new ObjectClassScopeProviderForTest("person");

            RuleGroup target = new RuleGroup();
            target.ObjectClassScopeProvider = provider;
            target.Operator = GroupOperator.Any;
            ValueComparisonRule rule1 = new ValueComparisonRule();
            rule1.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule1.ValueOperator = ValueOperator.Equals;
            rule1.ExpectedValue = new ValueDeclaration("test3.test3@test.com");
            target.Items.Add(rule1);

            ValueComparisonRule rule2 = new ValueComparisonRule();
            rule2.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule2.ValueOperator = ValueOperator.Equals;
            rule2.ExpectedValue = new ValueDeclaration("test1.test1@test.com");
            target.Items.Add(rule2);

            ValueComparisonRule rule3 = new ValueComparisonRule();
            rule3.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule3.ValueOperator = ValueOperator.Equals;
            rule3.ExpectedValue = new ValueDeclaration("test2.test2@test.com");
            target.Items.Add(rule3);

            return target;
        }

        private static RuleGroup GetOneGroupNoMatch()
        {
            ObjectClassScopeProviderForTest provider = new ObjectClassScopeProviderForTest("person");

            RuleGroup target = new RuleGroup();
            target.ObjectClassScopeProvider = provider;
            target.Operator = GroupOperator.One;
            ValueComparisonRule rule1 = new ValueComparisonRule();
            rule1.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule1.ValueOperator = ValueOperator.Equals;
            rule1.ExpectedValue = new ValueDeclaration("test3.test3@test.com");
            target.Items.Add(rule1);

            ValueComparisonRule rule2 = new ValueComparisonRule();
            rule2.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule2.ValueOperator = ValueOperator.Equals;
            rule2.ExpectedValue = new ValueDeclaration("test1.test1@test.com");
            target.Items.Add(rule2);

            ValueComparisonRule rule3 = new ValueComparisonRule();
            rule3.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule3.ValueOperator = ValueOperator.Equals;
            rule3.ExpectedValue = new ValueDeclaration("test2.test2@test.com");
            target.Items.Add(rule3);

            return target;
        }

        private static RuleGroup GetNoneGroup()
        {
            ObjectClassScopeProviderForTest provider = new ObjectClassScopeProviderForTest("person");

            RuleGroup target = new RuleGroup();
            target.ObjectClassScopeProvider = provider;
            target.Operator = GroupOperator.None;
            ValueComparisonRule rule1 = new ValueComparisonRule();
            rule1.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule1.ValueOperator = ValueOperator.Equals;
            rule1.ExpectedValue = new ValueDeclaration("test3.test3@test.com");
            target.Items.Add(rule1);

            ValueComparisonRule rule2 = new ValueComparisonRule();
            rule2.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule2.ValueOperator = ValueOperator.Equals;
            rule2.ExpectedValue = new ValueDeclaration("test1.test1@test.com");
            target.Items.Add(rule2);

            ValueComparisonRule rule3 = new ValueComparisonRule();
            rule3.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule3.ValueOperator = ValueOperator.Equals;
            rule3.ExpectedValue = new ValueDeclaration("test2.test2@test.com");
            target.Items.Add(rule3);

            return target;
        }

        private static RuleGroup GetNoneGroupNoMatch()
        {
            ObjectClassScopeProviderForTest provider = new ObjectClassScopeProviderForTest("person");

            RuleGroup target = new RuleGroup();
            target.ObjectClassScopeProvider = provider;
            target.Operator = GroupOperator.None;
            ValueComparisonRule rule1 = new ValueComparisonRule();
            rule1.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule1.ValueOperator = ValueOperator.Equals;
            rule1.ExpectedValue = new ValueDeclaration("test3.test3@test.com");
            target.Items.Add(rule1);

            ValueComparisonRule rule2 = new ValueComparisonRule();
            rule2.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule2.ValueOperator = ValueOperator.Equals;
            rule2.ExpectedValue = new ValueDeclaration("test.test@test.com");
            target.Items.Add(rule2);

            ValueComparisonRule rule3 = new ValueComparisonRule();
            rule3.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule3.ValueOperator = ValueOperator.Equals;
            rule3.ExpectedValue = new ValueDeclaration("test2.test2@test.com");
            target.Items.Add(rule3);

            return target;
        }

        private static RuleGroup GetOneGroup()
        {
            ObjectClassScopeProviderForTest provider = new ObjectClassScopeProviderForTest("person");

            RuleGroup target = new RuleGroup();
            target.ObjectClassScopeProvider = provider;
            target.Operator = GroupOperator.One;
            ValueComparisonRule rule1 = new ValueComparisonRule();
            rule1.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule1.ValueOperator = ValueOperator.Equals;
            rule1.ExpectedValue = new ValueDeclaration("test3.test3@test.com");
            target.Items.Add(rule1);

            ValueComparisonRule rule2 = new ValueComparisonRule();
            rule2.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule2.ValueOperator = ValueOperator.Equals;
            rule2.ExpectedValue = new ValueDeclaration("test.test@test.com");
            target.Items.Add(rule2);

            ValueComparisonRule rule3 = new ValueComparisonRule();
            rule3.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule3.ValueOperator = ValueOperator.Equals;
            rule3.ExpectedValue = new ValueDeclaration("test2.test2@test.com");
            target.Items.Add(rule3);

            return target;
        }

        private static RuleGroup GetOneGroupTwoMatches()
        {
            ObjectClassScopeProviderForTest provider = new ObjectClassScopeProviderForTest("person");

            RuleGroup target = new RuleGroup();
            target.ObjectClassScopeProvider = provider;
            target.Operator = GroupOperator.One;
            ValueComparisonRule rule1 = new ValueComparisonRule();
            rule1.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule1.ValueOperator = ValueOperator.Equals;
            rule1.ExpectedValue = new ValueDeclaration("test.test@test.com");
            target.Items.Add(rule1);

            ValueComparisonRule rule2 = new ValueComparisonRule();
            rule2.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule2.ValueOperator = ValueOperator.Equals;
            rule2.ExpectedValue = new ValueDeclaration("test.test@test.com");
            target.Items.Add(rule2);

            ValueComparisonRule rule3 = new ValueComparisonRule();
            rule3.Attribute = ActiveConfig.DB.GetAttribute("mail");
            rule3.ValueOperator = ValueOperator.Equals;
            rule3.ExpectedValue = new ValueDeclaration("test2.test2@test.com");
            target.Items.Add(rule3);

            return target;
        }

    }
}
