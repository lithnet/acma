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
using System.Collections.ObjectModel;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Acma.UnitTests
{
    [TestClass()]
    public class XmlConfigFileTests
    {
        public XmlConfigFileTests()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            AttributeValueDeleteConstructor constructorToSerialize1 = new AttributeValueDeleteConstructor();
            constructorToSerialize1.Attribute = ActiveConfig.DB.GetAttribute("sn");
            constructorToSerialize1.ID = "abc123";
            constructorToSerialize1.Description = "some description";
            constructorToSerialize1.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            constructorToSerialize1.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Delete });

            AttributeValueDeleteConstructor constructorToSerialize2 = new AttributeValueDeleteConstructor();
            constructorToSerialize2.Attribute = ActiveConfig.DB.GetAttribute("sn");
            constructorToSerialize2.ID = "abc123";
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

            ClassConstructor classConstructorToSerialize = new ClassConstructor();
            classConstructorToSerialize.ObjectClass = ActiveConfig.DB.GetObjectClass("person");
            classConstructorToSerialize.ExitEvents.Add(new AcmaInternalExitEvent() { ID = "testName" });
            classConstructorToSerialize.Constructors.Add(groupToSerialize);
            classConstructorToSerialize.Constructors.Add(constructorToSerialize2);

            classConstructorToSerialize.ResurrectionParameters = new DBQueryGroup();
            classConstructorToSerialize.ResurrectionParameters.Operator = GroupOperator.Any;
            classConstructorToSerialize.ResurrectionParameters.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("sn"), ValueOperator.Equals, new ValueDeclaration("test")));
            classConstructorToSerialize.ResurrectionParameters.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("sn"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("sn")));

            XmlConfigFile toSerialize = new XmlConfigFile();

            toSerialize.ClassConstructors.Add(classConstructorToSerialize);
            toSerialize.Transforms = new TransformKeyedCollection();
            toSerialize.Transforms.Add(new TrimStringTransform() { TrimType = TrimType.Both, ID = "trimboth" });
            UniqueIDCache.ClearIdCache();

            XmlConfigFile deserialized = UnitTestControl.XmlSerializeRoundTrip<XmlConfigFile>(toSerialize);

            Assert.AreEqual(toSerialize.ClassConstructors.Count, deserialized.ClassConstructors.Count);
            Assert.AreEqual(toSerialize.Transforms.Count, deserialized.Transforms.Count);
        }
    }
}
