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
    [TestClass()]
    public class AttributeConstructorGroupTests
    {
        public AttributeConstructorGroupTests()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            AttributeValueDeleteConstructor constructorToSerialize1 = new AttributeValueDeleteConstructor();
            constructorToSerialize1.Attribute = ActiveConfig.DB.GetAttribute("sn");
            constructorToSerialize1.ID = "abc123";
            constructorToSerialize1.Description = "some description";
            constructorToSerialize1.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            constructorToSerialize1.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Delete });

            AttributeValueDeleteConstructor constructorToSerialize2 = new AttributeValueDeleteConstructor();
            constructorToSerialize2.Attribute = ActiveConfig.DB.GetAttribute("sn");
            constructorToSerialize2.ID = "abc1234";
            constructorToSerialize2.Description = "some description";
            constructorToSerialize2.RuleGroup = new RuleGroup() { Operator = GroupOperator.All };
            constructorToSerialize2.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Add });

            AttributeConstructorGroup groupToSerialize = new AttributeConstructorGroup();
            groupToSerialize.Description = "my description";
            groupToSerialize.ID = "myID";
            groupToSerialize.ExecutionRule = GroupExecutionRule.ExitAfterFirstSuccess;
            groupToSerialize.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            groupToSerialize.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Delete });
            groupToSerialize.Constructors.Add(constructorToSerialize1);
            groupToSerialize.Constructors.Add(constructorToSerialize2);

            UniqueIDCache.ClearIdCache();

            AttributeConstructorGroup deserializedGroup = UnitTestControl.XmlSerializeRoundTrip<AttributeConstructorGroup>(groupToSerialize);

            Assert.AreEqual(groupToSerialize.ExecutionRule, deserializedGroup.ExecutionRule);
            Assert.AreEqual(groupToSerialize.ID, deserializedGroup.ID);
            Assert.AreEqual(groupToSerialize.Description, deserializedGroup.Description);
            Assert.AreEqual(groupToSerialize.RuleGroup.Operator, deserializedGroup.RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)groupToSerialize.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)deserializedGroup.RuleGroup.Items[0]).TriggerEvents);

            Assert.AreEqual(constructorToSerialize1.Attribute, ((AttributeConstructor)deserializedGroup.Constructors[0]).Attribute);
            Assert.AreEqual(constructorToSerialize1.ID, ((AttributeConstructor)deserializedGroup.Constructors[0]).ID);
            Assert.AreEqual(constructorToSerialize1.Description, ((AttributeConstructor)deserializedGroup.Constructors[0]).Description);
            Assert.AreEqual(constructorToSerialize1.RuleGroup.Operator, ((AttributeConstructor)deserializedGroup.Constructors[0]).RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)constructorToSerialize1.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)((AttributeConstructor)deserializedGroup.Constructors[0]).RuleGroup.Items[0]).TriggerEvents);

            Assert.AreEqual(constructorToSerialize2.Attribute, ((AttributeConstructor)deserializedGroup.Constructors[1]).Attribute);
            Assert.AreEqual(constructorToSerialize2.ID, ((AttributeConstructor)deserializedGroup.Constructors[1]).ID);
            Assert.AreEqual(constructorToSerialize2.Description, ((AttributeConstructor)deserializedGroup.Constructors[1]).Description);
            Assert.AreEqual(constructorToSerialize2.RuleGroup.Operator, ((AttributeConstructor)deserializedGroup.Constructors[1]).RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)constructorToSerialize2.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)((AttributeConstructor)deserializedGroup.Constructors[1]).RuleGroup.Items[0]).TriggerEvents);
        
        }
    }
}
