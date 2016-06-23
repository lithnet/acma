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
    public class ClassConstructorGroupTests
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
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

            ClassConstructor toSerialize = new ClassConstructor();
            toSerialize.ObjectClass = ActiveConfig.DB.GetObjectClass("person");
            toSerialize.ExitEvents.Add(new AcmaInternalExitEvent() { ID = "testName" });
            toSerialize.Constructors.Add(groupToSerialize);
            toSerialize.Constructors.Add(constructorToSerialize2);

            toSerialize.ResurrectionParameters = new DBQueryGroup();
            toSerialize.ResurrectionParameters.Operator = GroupOperator.Any;
            toSerialize.ResurrectionParameters.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("sn"), ValueOperator.Equals, new ValueDeclaration("test")));
            toSerialize.ResurrectionParameters.DBQueries.Add(new DBQueryByValue(ActiveConfig.DB.GetAttribute("sn"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("sn")));
            
            UniqueIDCache.ClearIdCache();
            ClassConstructor deserialized = UnitTestControl.XmlSerializeRoundTrip<ClassConstructor>(toSerialize);

            Assert.AreEqual(groupToSerialize.ExecutionRule, ((AttributeConstructorGroup)deserialized.Constructors[0]).ExecutionRule);
            Assert.AreEqual(groupToSerialize.ID, ((AttributeConstructorGroup)deserialized.Constructors[0]).ID);
            Assert.AreEqual(groupToSerialize.Description, ((AttributeConstructorGroup)deserialized.Constructors[0]).Description);
            Assert.AreEqual(groupToSerialize.RuleGroup.Operator, ((AttributeConstructorGroup)deserialized.Constructors[0]).RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)groupToSerialize.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)((AttributeConstructorGroup)deserialized.Constructors[0]).RuleGroup.Items[0]).TriggerEvents);

            Assert.AreEqual(constructorToSerialize2.Attribute, ((AttributeConstructor)deserialized.Constructors[1]).Attribute);
            Assert.AreEqual(constructorToSerialize2.ID, ((AttributeConstructor)deserialized.Constructors[1]).ID);
            Assert.AreEqual(constructorToSerialize2.Description, ((AttributeConstructor)deserialized.Constructors[1]).Description);
            Assert.AreEqual(constructorToSerialize2.RuleGroup.Operator, ((AttributeConstructor)deserialized.Constructors[1]).RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)constructorToSerialize2.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)((AttributeConstructor)deserialized.Constructors[1]).RuleGroup.Items[0]).TriggerEvents);

        }
    }
}
