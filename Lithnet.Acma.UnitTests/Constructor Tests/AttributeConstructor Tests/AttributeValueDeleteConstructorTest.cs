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
    ///This is a test class for AttributeValueDeleteConstructorTest and is intended
    ///to contain all AttributeValueDeleteConstructorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AttributeValueDeleteConstructorTest
    {
        public AttributeValueDeleteConstructorTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            AttributeValueDeleteConstructor toSeralize = new AttributeValueDeleteConstructor();
            toSeralize.Attribute = ActiveConfig.DB.GetAttribute("sn");
            toSeralize.ID = "abc1235645645";
            toSeralize.Description = "some description";
            toSeralize.RuleGroup = new RuleGroup() { Operator = GroupOperator.All };
            toSeralize.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Add });
            Lithnet.Common.ObjectModel.UniqueIDCache.ClearIdCache();
            AttributeValueDeleteConstructor deserialized = UnitTestControl.XmlSerializeRoundTrip<AttributeValueDeleteConstructor>(toSeralize);

            Assert.AreEqual(toSeralize.Attribute, deserialized.Attribute);
            Assert.AreEqual(toSeralize.ID, deserialized.ID);
            Assert.AreEqual(toSeralize.Description, deserialized.Description);
            Assert.AreEqual(toSeralize.RuleGroup.Operator, deserialized.RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)toSeralize.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)deserialized.RuleGroup.Items[0]).TriggerEvents);
        }


        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void AttributeDeleteConstructorSVTest()
        {
            AttributeValueDeleteConstructor attributeConstructor = new AttributeValueDeleteConstructor();
            attributeConstructor.Attribute = ActiveConfig.DB.GetAttribute("mail");
            attributeConstructor.RuleGroup = new RuleGroup();
            attributeConstructor.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Add });

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();
                sourceObject.DiscardPendingChanges();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                attributeConstructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail"));

                if (!value.IsNull)
                {
                    Assert.Fail("The constructor did not delete the attribute value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void AttributeDeleteConstructorMVTest()
        {
            AttributeValueDeleteConstructor attributeConstructor = new AttributeValueDeleteConstructor();
            attributeConstructor.Attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            attributeConstructor.RuleGroup = new RuleGroup();
            attributeConstructor.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Add });

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" });
                sourceObject.CommitCSEntryChange();
                sourceObject.DiscardPendingChanges();
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                attributeConstructor.Execute(sourceObject);

                AttributeValues values = sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));

                if (values.Values.Count != 0)
                {
                    Assert.Fail("The constructor did not delete the attribute value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
