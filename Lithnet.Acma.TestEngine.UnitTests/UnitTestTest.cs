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
    public class UnitTestTest
    {
        public UnitTestTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod]
        public void TestSerialization()
        {
            UnitTest toSerlialize = new UnitTest();

            toSerlialize.Description = "unit test 1";
            toSerlialize.ExpectedConstructors.Add("ABC123");
            toSerlialize.ExpectedConstructors.Add("123DGS");
            toSerlialize.ID = "ID12340000";

            UnitTestStepObjectCreation step1 = new UnitTestStepObjectCreation();
            AcmaCSEntryChange csentry = new AcmaCSEntryChange();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = "person";
            csentry.DN = "testDN";
            step1.Name = "mystep1";
            step1.CSEntryChange = csentry;
            toSerlialize.Steps.Add(step1);

            UnitTestStepObjectEvaluation step2 = new UnitTestStepObjectEvaluation();
            step2.Name = "mystep2";
            step2.ObjectCreationStep = step1;
            step2.SuccessCriteria = new RuleGroup();
            step2.SuccessCriteria.Items.Add(new ObjectChangeRule() { Description = "test", TriggerEvents = TriggerEvents.Add });
            toSerlialize.Steps.Add(step2);

            UniqueIDCache.ClearIdCache();
            UnitTest deserialized = UnitTestControl.XmlSerializeRoundTrip<UnitTest>(toSerlialize);

            Assert.AreEqual(toSerlialize.Description, deserialized.Description);
            Assert.AreEqual(toSerlialize.ExpectedConstructors[0], deserialized.ExpectedConstructors[0]);
            Assert.AreEqual(toSerlialize.ExpectedConstructors[1], deserialized.ExpectedConstructors[1]);
            Assert.AreEqual(toSerlialize.ID, deserialized.ID);
            Assert.AreEqual(((UnitTestStepObjectCreation)toSerlialize.Steps[0]).Name, ((UnitTestStepObjectCreation)deserialized.Steps[0]).Name);
            Assert.AreEqual(((UnitTestStepObjectCreation)toSerlialize.Steps[0]).CSEntryChange.DN, ((UnitTestStepObjectCreation)deserialized.Steps[0]).CSEntryChange.DN);
            Assert.AreEqual(((UnitTestStepObjectCreation)toSerlialize.Steps[0]).CSEntryChange.ObjectType, ((UnitTestStepObjectCreation)deserialized.Steps[0]).CSEntryChange.ObjectType);
            Assert.AreEqual(((UnitTestStepObjectCreation)toSerlialize.Steps[0]).CSEntryChange.ObjectModificationType, ((UnitTestStepObjectCreation)deserialized.Steps[0]).CSEntryChange.ObjectModificationType);

            Assert.AreEqual(((UnitTestStepObjectEvaluation)toSerlialize.Steps[1]).Name, ((UnitTestStepObjectEvaluation)deserialized.Steps[1]).Name);
            Assert.AreEqual(((UnitTestStepObjectEvaluation)toSerlialize.Steps[1]).ObjectCreationStepName, ((UnitTestStepObjectEvaluation)deserialized.Steps[1]).ObjectCreationStepName);
            Assert.AreEqual(((UnitTestStepObjectEvaluation)toSerlialize.Steps[1]).SuccessCriteria.Items.Count, ((UnitTestStepObjectEvaluation)deserialized.Steps[1]).SuccessCriteria.Items.Count);
        }
    }
}
