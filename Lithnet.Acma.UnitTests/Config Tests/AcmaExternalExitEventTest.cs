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
using System.IO;

namespace Lithnet.Acma.UnitTests
{
    [TestClass()]
    public class AcmaExternalExitEventTest
    {
        public AcmaExternalExitEventTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            AcmaExternalExitEventCmd toSerialize = new AcmaExternalExitEventCmd();
            toSerialize.ID = "test external event";
            toSerialize.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
            toSerialize.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Delete });
            toSerialize.CommandLine = "cmd.exe";
            toSerialize.Arguments = new ValueDeclaration("/c dir >> testfile.txt");
            Lithnet.Common.ObjectModel.UniqueIDCache.ClearIdCache();

            AcmaExternalExitEventCmd deserialized = UnitTestControl.XmlSerializeRoundTrip<AcmaExternalExitEventCmd>(toSerialize);

            Assert.AreEqual(toSerialize.ID, deserialized.ID);
            Assert.AreEqual(toSerialize.RuleGroup.Operator, deserialized.RuleGroup.Operator);
            Assert.AreEqual(((ObjectChangeRule)toSerialize.RuleGroup.Items[0]).TriggerEvents, ((ObjectChangeRule)deserialized.RuleGroup.Items[0]).TriggerEvents);
            Assert.AreEqual(toSerialize.CommandLine, deserialized.CommandLine);
            Assert.AreEqual(toSerialize.Arguments.Declaration, deserialized.Arguments.Declaration);
        }

        [TestMethod]
        public void TextExitEvent()
        {
            Guid newId = Guid.NewGuid();

            string tempFileName = Path.Combine(Path.GetTempPath(), newId.ToString());

            try
            {
                AcmaExternalExitEventCmd e = new AcmaExternalExitEventCmd();
                e.ID = "test external event";
                e.RuleGroup = new RuleGroup() { Operator = GroupOperator.Any };
                e.RuleGroup.Items.Add(new ObjectChangeRule() { TriggerEvents = TriggerEvents.Add });
                e.CommandLine = "cmd.exe";
                e.Arguments = new ValueDeclaration("/c dir >> " + tempFileName.Replace(@"\",@"\\"));

                if (!ActiveConfig.XmlConfig.ClassConstructors.Contains("person"))
                {
                    ClassConstructor constructor = new ClassConstructor();
                    constructor.ObjectClass = ActiveConfig.DB.GetObjectClass("person");
                    ActiveConfig.XmlConfig.ClassConstructors.Add(constructor);
                }

                ActiveConfig.XmlConfig.ClassConstructors["person"].ExitEvents.Add(e);

                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                if (!System.IO.File.Exists(tempFileName))
                {
                    Assert.Fail("The external event did not created the expected file");
                }
            }
            finally
            {
                if (System.IO.File.Exists(tempFileName))
                {
                    System.IO.File.Delete(tempFileName);
                }
            }
        }
    }
}
