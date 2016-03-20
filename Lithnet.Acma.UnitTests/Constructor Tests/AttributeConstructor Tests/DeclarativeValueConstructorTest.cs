using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for DeclarativeValueConstructorTest and is intended
    ///to contain all DeclarativeValueConstructorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DeclarativeValueConstructorTest
    {
        public DeclarativeValueConstructorTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            DeclarativeValueConstructor toSeralize = new DeclarativeValueConstructor();
            toSeralize.Attribute = ActiveConfig.DB.GetAttribute("sn");
            toSeralize.ID = "abc123";
            toSeralize.Description = "some description";
            toSeralize.ValueDeclarations = new List<ValueDeclaration>() { new ValueDeclaration("test") };
            toSeralize.ModificationType = AcmaAttributeModificationType.Replace;
            toSeralize.PresenceConditions = new RuleGroup() { Operator = GroupOperator.One };
            toSeralize.PresenceConditions.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Update });
            toSeralize.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            toSeralize.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Delete });
            Lithnet.Common.ObjectModel.UniqueIDCache.ClearIdCache();
            DeclarativeValueConstructor deserialized = UnitTestControl.XmlSerializeRoundTrip<DeclarativeValueConstructor>(toSeralize);

            Assert.AreEqual(toSeralize.Attribute, deserialized.Attribute);
            Assert.AreEqual(toSeralize.ID, deserialized.ID);
            Assert.AreEqual(toSeralize.Description, deserialized.Description);
            Assert.AreEqual(toSeralize.ModificationType, deserialized.ModificationType);
            Assert.AreEqual(toSeralize.RuleGroup.Operator, deserialized.RuleGroup.Operator);
            Assert.AreEqual(toSeralize.ValueDeclarations.Count, deserialized.ValueDeclarations.Count);

            Assert.AreEqual(toSeralize.ValueDeclarations[0].Declaration, deserialized.ValueDeclarations[0].Declaration);
            Assert.AreEqual(toSeralize.ValueDeclarations[0].TransformsString, deserialized.ValueDeclarations[0].TransformsString);

            Assert.AreEqual(((ObjectChangeRule)toSeralize.PresenceConditions.Items[0]).TriggerEvents, ((ObjectChangeRule)deserialized.PresenceConditions.Items[0]).TriggerEvents);

            Assert.AreEqual(((ObjectChangeRule)toSeralize.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)deserialized.RuleGroup.Items[0]).TriggerEvents);
        }

        [TestMethod()]
        public void ValueConstructorGuidAllocationTest()
        {
            DeclarativeValueConstructor attributeConstructor = new DeclarativeValueConstructor();
            attributeConstructor.ValueDeclarations.Add(new ValueDeclaration("%newguid%"));
            attributeConstructor.ModificationType = AcmaAttributeModificationType.Replace;
            attributeConstructor.Attribute = ActiveConfig.DB.GetAttribute("personID");

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                attributeConstructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("personID"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any value");
                }
                if (value.Value is Guid)
                {
                    if (value.ValueGuid == Guid.Empty)
                    {
                        Assert.Fail("The constructor did not generate the expected value");
                    }
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceSVFixedIntegerTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("unixUid");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("5"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueLong != 5L)
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceSVFixedDateTimeTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("2010-01-01"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueDateTime != DateTime.Parse("2010-01-01"))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceSVFixedBooleanTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("true"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                //sourceObject.UpdateAttributeValue(constructor.Attribute, new List<ValueChange>() { ValueChange.CreateValueAdd("false") });
                //sourceObject.CommitCSEntryChange();
                ////                ////sourceObject.UpdateAttributeValue(constructor.Attribute, new List<ValueChange>() { ValueChange.CreateValueAdd("true") });
                ////sourceObject.CommitCSEntryChange();

                //
                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueBoolean != true)
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceSVFixedBinaryTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("objectSid");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("AAECAwQ="));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (!value.ValueByte.SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceSVFixedStringTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mail");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueString != "test.test@test.com")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCConditionalSVFixedStringAddTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mail");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));
            constructor.PresenceConditions.Items.Add(new ValueComparisonRule() { Attribute = ActiveConfig.DB.GetAttribute("mailAddressFormat"), ValueOperator = ValueOperator.Equals, ExpectedValue = new ValueDeclaration("1") });
            constructor.PresenceConditions.Operator = GroupOperator.All;

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAddressFormat"), 1);
                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueString != "test.test@test.com")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCConditionalSVFixedStringAddNothingTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mail");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));
            constructor.PresenceConditions.Items.Add(new ValueComparisonRule() { Attribute = ActiveConfig.DB.GetAttribute("mailAddressFormat"), ValueOperator = ValueOperator.Equals, ExpectedValue = new ValueDeclaration("1") });
            constructor.PresenceConditions.Operator = GroupOperator.All;
            constructor.ModificationType = AcmaAttributeModificationType.Conditional;

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAddressFormat"), 0);
                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));

                if (!value.IsNull)
                {
                    Assert.Fail("The constructor unexpectedly generated a value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCConditionalSVFixedStringRevokeTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mail");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));
            constructor.PresenceConditions.Items.Add(new ValueComparisonRule() { Attribute = ActiveConfig.DB.GetAttribute("mailAddressFormat"), ValueOperator = ValueOperator.Equals, ExpectedValue = new ValueDeclaration("1"), GroupOperator = GroupOperator.Any });
            constructor.PresenceConditions.Operator = GroupOperator.All;
            constructor.ModificationType = AcmaAttributeModificationType.Conditional;

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAddressFormat"), 0);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));

                if (!value.IsNull)
                {
                    Assert.Fail("The constructor unexpectedly generated a value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCConditionalMVFixedStringRemoveSingleValueTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));
            constructor.PresenceConditions.Items.Add(new ValueComparisonRule() { Attribute = ActiveConfig.DB.GetAttribute("mailAddressFormat"), ValueOperator = ValueOperator.Equals, ExpectedValue = new ValueDeclaration("1") });
            constructor.PresenceConditions.Operator = GroupOperator.All;
            constructor.ModificationType = AcmaAttributeModificationType.Conditional;

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAddressFormat"), 0);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test1.test1@test.com" });
                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));
                if (values.IsEmptyOrNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (!values.ContainsAllElements(new List<object>() { "test1.test1@test.com" }))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCConditionalMVFixedStringAddTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));
            constructor.PresenceConditions.Items.Add(new ValueComparisonRule() { Attribute = ActiveConfig.DB.GetAttribute("mailAddressFormat"), ValueOperator = ValueOperator.Equals, ExpectedValue = new ValueDeclaration("1") });

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAddressFormat"), 1);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test1.test1@test.com" });
                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));

                if (values.IsEmptyOrNull)
                {

                    Assert.Fail("The constructor did not generate any values");
                }
                else if (values.ContainsSameElements(new List<object>() { "test.test@test.com", "test1.test1@test.com" }))
                {
                    Assert.Fail("The constructor did not generate the expected values");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceSVComplexStringTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mail");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("{firstName}.{sn}@test.com"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), "test1");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueString != "test1.test2@test.com")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceSVSimpleAttributeStringTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mail");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("{firstName}"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), "test1");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueString != "test1")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceMVSimpleLongTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("expiryDates2");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("{expiryDates}"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object>() { 1L, 2L, 3L, 4L });

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("expiryDates2"));

                if (values.Values.Count == 0)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object>() { 1L, 2L, 3L, 4L }))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceMVSimpleDateTimeTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("dateTimeMV2");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("{dateTimeMV}"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeMV"), new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") });

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("dateTimeMV2"));

                if (values.Values.Count == 0)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") }))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCReplaceMVSimpleStringTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("sapPreviousPersonIds");
            constructor.ModificationType = AcmaAttributeModificationType.Replace;
            constructor.ValueDeclarations.Add(new ValueDeclaration("{mailAlternateAddresses}"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test1.test1@test.com" });

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("sapPreviousPersonIds"));

                if (values.Values.Count == 0)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object>() { "test.test@test.com", "test1.test1@test.com" }))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteSVFixedStringTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mail");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));
                if (!value.IsNull)
                {
                    Assert.Fail("The constructor did not delete the value");
                }

                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test2.test2@test.com");
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);
                value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor deleted the wrong value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteSVFixedLongTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("sapExpiryDate");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("5"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sapExpiryDate"), 5L);
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("sapExpiryDate"));

                if (!value.IsNull)
                {
                    Assert.Fail("The constructor did not delete the value");
                }

                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sapExpiryDate"), 4L);
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);
                value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("sapExpiryDate"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor deleted the wrong value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteSVFixedDateTimeTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("2010-01-01"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV"), DateTime.Parse("2010-01-01"));
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV"));

                if (!value.IsNull)
                {
                    Assert.Fail("The constructor did not delete the value");
                }

                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV"), DateTime.Parse("2011-01-01"));
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);
                value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor deleted the wrong value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteSVFixedBooleanTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("true"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"), true);
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"));

                if (!value.IsNull)
                {
                    Assert.Fail("The constructor did not delete the value");
                }

                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"), false);
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);
                value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor deleted the wrong value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteSVFixedBinaryTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("objectSid");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("AAECAwQ="));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"), new byte[] { 0, 1, 2, 3, 4 });
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"));

                if (!value.IsNull)
                {
                    Assert.Fail("The constructor did not delete the value");
                }

                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"), new byte[] { 1, 2, 3, 4, 5 });
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);
                value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor deleted the wrong value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteMVFixedStringSingleTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" });
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));
                if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object> { "test1.test1@test.com", "test2.test2@test.com" }))
                {
                    Assert.Fail("The constructor did not delete the value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteMVFixedStringMultipleTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));
            constructor.ValueDeclarations.Add(new ValueDeclaration("test1.test1@test.com"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" });
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));
                if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object> { "test2.test2@test.com" }))
                {
                    Assert.Fail("The constructor did not delete the value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void DVCDeleteMVFixedDateTimeSingleTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("dateTimeMV");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("2010-01-01"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeMV"), new List<object> { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") });
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("dateTimeMV"));
                if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object> { DateTime.Parse("2011-01-01") }))
                {
                    Assert.Fail("The constructor did not delete the value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteMVFixedLongSingleTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("10"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 5L, 10L, 15L });
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("expiryDates"));
                if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object> { 5L, 15L }))
                {
                    Assert.Fail("The constructor did not delete the value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteMVFixedLongMultipleTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("10"));
            constructor.ValueDeclarations.Add(new ValueDeclaration("5"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 5L, 10L, 15L });
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("expiryDates"));
                if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object> { 15L }))
                {
                    Assert.Fail("The constructor did not delete the value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCDeleteMVFixedBinaryTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("objectSids");
            constructor.ModificationType = AcmaAttributeModificationType.Delete;
            constructor.ValueDeclarations.Add(new ValueDeclaration("AAECAwQ="));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("objectSids"), new List<object> { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 2, 3, 4, 5, 6 } });
                sourceObject.CommitCSEntryChange();

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("objectSids"));
                if (!values.ContainsAllElements(new List<object> { new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 2, 3, 4, 5, 6 } }))
                {
                    Assert.Fail("The constructor did not delete the value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void DVCAddSVFixedIntegerTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("unixUid");
            constructor.ModificationType = AcmaAttributeModificationType.Add;
            constructor.ValueDeclarations.Add(new ValueDeclaration("5"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueLong != 5L)
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCAddSVFixedDateTimeTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
            constructor.ModificationType = AcmaAttributeModificationType.Add;
            constructor.ValueDeclarations.Add(new ValueDeclaration("2010-01-01"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueDateTime != DateTime.Parse("2010-01-01"))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCAddSVFixedBooleanTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            constructor.ModificationType = AcmaAttributeModificationType.Add;
            constructor.ValueDeclarations.Add(new ValueDeclaration("true"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueBoolean != true)
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCAddSVFixedBinaryTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("objectSid");
            constructor.ModificationType = AcmaAttributeModificationType.Add;
            constructor.ValueDeclarations.Add(new ValueDeclaration("AAECAwQ="));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (!value.ValueByte.SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCAddSVFixedStringTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mail");
            constructor.ModificationType = AcmaAttributeModificationType.Add;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test.test@test.com"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                constructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (value.ValueString != "test.test@test.com")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void DVCAddMVFixedIntegerTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            constructor.ModificationType = AcmaAttributeModificationType.Add;
            constructor.ValueDeclarations.Add(new ValueDeclaration("10"));
            constructor.ValueDeclarations.Add(new ValueDeclaration("15"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object>() { 1L, 2L, 3L, 4L });

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("expiryDates"));

                if (values.Values.Count == 0)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object>() { 1L, 2L, 3L, 4L, 10L, 15L }))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCAddMVFixedBinaryTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("objectSids");
            constructor.ModificationType = AcmaAttributeModificationType.Add;
            constructor.ValueDeclarations.Add(new ValueDeclaration("AAECAwQ="));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("objectSids"), new List<object>() { new byte[] { 1, 2, 3, 4, 5 } });

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("objectSids"));

                if (values.Values.Count == 0)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                if (!(values.Values.SequenceEqual(new List<object> { new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 0, 1, 2, 3, 4 } })
                    || values.Values.SequenceEqual(new List<object> { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 } })))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DVCAddMVFixedStringTest()
        {
            DeclarativeValueConstructor constructor = new DeclarativeValueConstructor();
            constructor.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            constructor.ModificationType = AcmaAttributeModificationType.Add;
            constructor.ValueDeclarations.Add(new ValueDeclaration("test1.test1@test.com"));
            constructor.ValueDeclarations.Add(new ValueDeclaration("test2.test2@test.com"));

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com" });

                constructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));

                if (values.Values.Count == 0)
                {
                    Assert.Fail("The constructor did not generate any values");
                }
                else if (!values.Values.OrderBy(t => t.Value).SequenceEqual(new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" }))
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
