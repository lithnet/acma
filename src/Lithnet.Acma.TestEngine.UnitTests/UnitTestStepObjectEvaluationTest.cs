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
    public class UnitTestStepObjectEvaluationTest
    {
        public UnitTestStepObjectEvaluationTest()
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

            UnitTest deserialized = UnitTestControl.XmlSerializeRoundTrip<UnitTest>(toSerlialize);

            Assert.AreEqual(step2.Name, ((UnitTestStepObjectEvaluation)deserialized.Steps[1]).Name);
            Assert.AreEqual(step2.ObjectCreationStepName, ((UnitTestStepObjectEvaluation)deserialized.Steps[1]).ObjectCreationStepName);
            Assert.AreEqual(step2.SuccessCriteria.Items.Count, ((UnitTestStepObjectEvaluation)deserialized.Steps[1]).SuccessCriteria.Items.Count);
            Assert.AreEqual(((ObjectChangeRule)step2.SuccessCriteria.Items[0]).TriggerEvents, ((ObjectChangeRule)((UnitTestStepObjectEvaluation)deserialized.Steps[1]).SuccessCriteria.Items[0]).TriggerEvents);
        }
    }
}
