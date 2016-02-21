using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.Acma;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Collections;
using System.Collections.Generic;
using Lithnet.Acma.UnitTests;

namespace Lithnet.Acma.TestEngine.UnitTests
{
    [TestClass]
    public class UnitTestFileTest
    {
        public UnitTestFileTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod]
        public void TestSerialization()
        {
            UnitTest unitTest1 = new UnitTest();

            unitTest1.Description = "unit test 1";
            unitTest1.ExpectedConstructors.Add("ABC123");
            unitTest1.ExpectedConstructors.Add("123DGS");
            unitTest1.ID = "ID1234";

            UnitTestStepObjectCreation ut1step1 = new UnitTestStepObjectCreation();
            AcmaCSEntryChange csentry1 = new AcmaCSEntryChange();
            csentry1.ObjectModificationType = ObjectModificationType.Add;
            csentry1.ObjectType = "person";
            csentry1.DN = "testDN";
            ut1step1.Name = "mystep1";
            ut1step1.CSEntryChange = csentry1;
            unitTest1.Steps.Add(ut1step1);


            UnitTestStepObjectEvaluation ut1step2 = new UnitTestStepObjectEvaluation();
            ut1step2.Name = "mystep2";
            ut1step2.ObjectCreationStep = ut1step1;
            ut1step2.SuccessCriteria = new RuleGroup();
            ut1step2.SuccessCriteria.Items.Add(new ObjectChangeRule() { Description = "test", TriggerEvents = TriggerEvents.Add });
            unitTest1.Steps.Add(ut1step2);

            UnitTest unitTest2 = new UnitTest();

            unitTest2.Description = "unit test 1";
            unitTest2.ExpectedConstructors.Add("ABC123");
            unitTest2.ExpectedConstructors.Add("123DGS");
            unitTest2.ID = "ID1234";

            UnitTestStepObjectModification ut2step1 = new UnitTestStepObjectModification();
            AcmaCSEntryChange csentry2 = new AcmaCSEntryChange();
            csentry2.ObjectModificationType = ObjectModificationType.Update;
            csentry2.ObjectType = "person";
            csentry2.DN = "testDN";
            ut2step1.Name = "mystep1";
            ut2step1.CSEntryChange = csentry2;
            unitTest2.Steps.Add(ut2step1);

            UnitTestStepObjectEvaluation ut2step2 = new UnitTestStepObjectEvaluation();
            ut2step2.Name = "mystep2";
            ut2step2.ObjectCreationStep = ut1step1;
            ut2step2.SuccessCriteria = new RuleGroup();
            ut2step2.SuccessCriteria.Items.Add(new ObjectChangeRule() { Description = "test", TriggerEvents = TriggerEvents.Add });
            unitTest2.Steps.Add(ut2step2);

            UnitTestFile toSerialize = new UnitTestFile();
            toSerialize.ConfigVersion = "1";
            toSerialize.Description = "test description";
            toSerialize.UnitTestObjects.Add(unitTest1);
            toSerialize.UnitTestObjects.Add(unitTest2);

            UniqueIDCache.ClearIdCache();
            UnitTestFile deserialized = UnitTestControl.XmlSerializeRoundTrip<UnitTestFile>(toSerialize);

            Assert.AreEqual(toSerialize.Description, deserialized.Description);
            Assert.AreEqual(toSerialize.ConfigVersion, deserialized.ConfigVersion);
            Assert.AreEqual(toSerialize.UnitTestObjects.Count, deserialized.UnitTestObjects.Count);
            
            UnitTest deserializedUnitTest1 = deserialized.UnitTestObjects[0] as UnitTest;
            Assert.AreEqual(unitTest1.Description, deserializedUnitTest1.Description);
            Assert.AreEqual(unitTest1.ExpectedConstructors[0], deserializedUnitTest1.ExpectedConstructors[0]);
            Assert.AreEqual(unitTest1.ExpectedConstructors[1], deserializedUnitTest1.ExpectedConstructors[1]);
            Assert.AreEqual(unitTest1.ID, deserializedUnitTest1.ID);
            Assert.AreEqual(((UnitTestStepObjectCreation)unitTest1.Steps[0]).Name, ((UnitTestStepObjectCreation)deserializedUnitTest1.Steps[0]).Name);
            Assert.AreEqual(((UnitTestStepObjectCreation)unitTest1.Steps[0]).CSEntryChange.DN, ((UnitTestStepObjectCreation)deserializedUnitTest1.Steps[0]).CSEntryChange.DN);
            Assert.AreEqual(((UnitTestStepObjectCreation)unitTest1.Steps[0]).CSEntryChange.ObjectType, ((UnitTestStepObjectCreation)deserializedUnitTest1.Steps[0]).CSEntryChange.ObjectType);
            Assert.AreEqual(((UnitTestStepObjectCreation)unitTest1.Steps[0]).CSEntryChange.ObjectModificationType, ((UnitTestStepObjectCreation)deserializedUnitTest1.Steps[0]).CSEntryChange.ObjectModificationType);

            Assert.AreEqual(((UnitTestStepObjectEvaluation)unitTest1.Steps[1]).Name, ((UnitTestStepObjectEvaluation)deserializedUnitTest1.Steps[1]).Name);
            Assert.AreEqual(((UnitTestStepObjectEvaluation)unitTest1.Steps[1]).ObjectCreationStepName, ((UnitTestStepObjectEvaluation)deserializedUnitTest1.Steps[1]).ObjectCreationStepName);
            Assert.AreEqual(((UnitTestStepObjectEvaluation)unitTest1.Steps[1]).SuccessCriteria.Items.Count, ((UnitTestStepObjectEvaluation)deserializedUnitTest1.Steps[1]).SuccessCriteria.Items.Count);
        }
    }
}
