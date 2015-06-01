using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Lithnet.Acma.DataModel;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for GuidAllocationConstructorTest and is intended
    ///to contain all GuidAllocationConstructorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SequentialIntegerAllocationConstructorTest
    {
        public SequentialIntegerAllocationConstructorTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            SequentialIntegerAllocationConstructor toSeralize = new SequentialIntegerAllocationConstructor();
            toSeralize.Attribute = ActiveConfig.DB.GetAttribute("sn");
            toSeralize.ID = "abc12355";
            toSeralize.Description = "some description";
            toSeralize.Sequence = ActiveConfig.DB.GetSequence("unixUid");
            toSeralize.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            toSeralize.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Delete });
            UniqueIDCache.ClearIdCache();

            SequentialIntegerAllocationConstructor deserialized = UnitTestControl.XmlSerializeRoundTrip<SequentialIntegerAllocationConstructor>(toSeralize);

            Assert.AreEqual(toSeralize.Attribute, deserialized.Attribute);
            Assert.AreEqual(toSeralize.ID, deserialized.ID);
            Assert.AreEqual(toSeralize.Description, deserialized.Description);
            Assert.AreEqual(toSeralize.Sequence, deserialized.Sequence);
            Assert.AreEqual(toSeralize.RuleGroup.Operator, deserialized.RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)toSeralize.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)deserialized.RuleGroup.Items[0]).TriggerEvents);
        }

        [TestMethod()]
        public void ValueConstructorSequentialIntegerAllocationTest()
        {
            SequentialIntegerAllocationConstructor attributeConstructor = new SequentialIntegerAllocationConstructor();
            attributeConstructor.Attribute = ActiveConfig.DB.GetAttribute("unixUid");
            attributeConstructor.Sequence = ActiveConfig.DB.GetSequence("unixUid");

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");

                long nextInt = ActiveConfig.DB.GetNextSequenceValue("unixUid") + 1;

                attributeConstructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any value");
                }
                if (value.Value is long)
                {
                    if (value.ValueLong != nextInt)
                    {
                        Assert.Fail("The constructor did not generate the expected value");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
