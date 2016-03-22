using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.Acma;
using Lithnet.Acma.DataModel;
using System.Collections.Generic;

namespace Lithnet.Acma.UnitTests.Constructor_Tests.AttributeConstructor_Tests
{
    [TestClass]
    public class UniqueValueConstructorTest
    {
        public UniqueValueConstructorTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod]
        public void PerformBulkUniqueAllocationTest()
        {
            UniqueValueConstructor constructor = new UniqueValueConstructor();
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("accountName");
            constructor.UniqueAllocationAttributes = new System.Collections.Generic.List<AcmaSchemaAttribute>() { attribute };
            constructor.ValueDeclaration = new UniqueValueDeclaration("{sn}%n%");
            constructor.Attribute = attribute;

            List<Guid> preCreatedIDs = new List<Guid>();
            List<Guid> testIDs = new List<Guid>();

            int prestage = 100;
            int testcount = 200;
            try
            {

                for (int i = 1; i <= prestage; i++)
                {
                    Guid newId = Guid.NewGuid();
                    MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                    preCreatedIDs.Add(newId);

                    sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "yzhu");
                    sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), string.Format("yzhu{0}", i));
                    sourceObject.CommitCSEntryChange();
                }

                for (int i = 1; i <= testcount; i++)
                {
                    Guid newId = Guid.NewGuid();
                    MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                    testIDs.Add(newId);

                    sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "yzhu");
                    constructor.Execute(sourceObject);
                    sourceObject.CommitCSEntryChange();

                    AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("accountName"));

                    Assert.AreEqual(string.Format("yzhu{0}", i + prestage), value.ValueString);
                }
            }
            finally
            {
                foreach (Guid id in preCreatedIDs)
                {
                    ActiveConfig.DB.DeleteMAObjectPermanent(id);
                }

                foreach (Guid id in testIDs)
                {
                    ActiveConfig.DB.DeleteMAObjectPermanent(id);
                }
            }

        }
    }
}
