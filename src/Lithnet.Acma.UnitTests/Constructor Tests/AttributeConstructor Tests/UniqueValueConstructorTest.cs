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

        [TestMethod()]
        public void UniqueValueConstructorOptionalNumberTest()
        {
            UniqueValueConstructor attributeConstructor = new UniqueValueConstructor();
            attributeConstructor.ValueDeclaration = new UniqueValueDeclaration("mytestvalue%o%");
            attributeConstructor.Attribute = ActiveConfig.DB.GetAttribute("personID");
            attributeConstructor.UniqueAllocationAttributes.Add(attributeConstructor.Attribute);

            Guid id1 = Guid.NewGuid();
            Guid id2 = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject1 = ActiveConfig.DB.CreateMAObject(id1, "person");
                MAObjectHologram sourceObject2 = ActiveConfig.DB.CreateMAObject(id2, "person");

                attributeConstructor.Execute(sourceObject1);

                AttributeValue value = sourceObject1.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("personID"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any value");
                }

                if (value.Value.ToString() != "mytestvalue")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }

                attributeConstructor.Execute(sourceObject2);
                value = sourceObject2.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("personID"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any value");
                }

                if (value.Value.ToString() != "mytestvalue1")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id1);
                ActiveConfig.DB.DeleteMAObjectPermanent(id2);
            }
        }

        [TestMethod()]
        public void UniqueValueConstructorRequiredNumberTest()
        {
            UniqueValueConstructor attributeConstructor = new UniqueValueConstructor();
            attributeConstructor.ValueDeclaration = new UniqueValueDeclaration("mytestvalue%n%");
            attributeConstructor.Attribute = ActiveConfig.DB.GetAttribute("personID");
            attributeConstructor.UniqueAllocationAttributes.Add(attributeConstructor.Attribute);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                attributeConstructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("personID"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any value");
                }

                if (value.Value.ToString() != "mytestvalue1")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void UniqueValueConstructorSelfUniqueTest()
        {
            UniqueValueConstructor attributeConstructor = new UniqueValueConstructor();
            attributeConstructor.ValueDeclaration = new UniqueValueDeclaration("mytestvalue");
            attributeConstructor.Attribute = ActiveConfig.DB.GetAttribute("personID");
            attributeConstructor.UniqueAllocationAttributes.Add(attributeConstructor.Attribute);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");

                attributeConstructor.Execute(sourceObject);

                AttributeValue value = sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("personID"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any value");
                }

                if (value.Value.ToString() != "mytestvalue")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void UniqueValueConstructorSelfUniqueDuplicateTest()
        {
            UniqueValueConstructor attributeConstructor = new UniqueValueConstructor();
            attributeConstructor.ValueDeclaration = new UniqueValueDeclaration("mytestvalue");
            attributeConstructor.Attribute = ActiveConfig.DB.GetAttribute("personID");
            attributeConstructor.UniqueAllocationAttributes.Add(attributeConstructor.Attribute);

            Guid id1 = Guid.NewGuid();
            Guid id2 = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject1 = ActiveConfig.DB.CreateMAObject(id1, "person");
                MAObjectHologram sourceObject2 = ActiveConfig.DB.CreateMAObject(id2, "person");

                attributeConstructor.Execute(sourceObject1);

                AttributeValue value = sourceObject1.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("personID"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any value");
                }

                if (value.Value.ToString() != "mytestvalue")
                {
                    Assert.Fail("The constructor did not generate the expected value");
                }

                sourceObject1.CommitCSEntryChange();

                try
                {
                    attributeConstructor.Execute(sourceObject2);
                    Assert.Fail("The constructor did not fail as expected");
                }
                catch (MaximumAllocationAttemptsExceededException)
                {
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id1);
                ActiveConfig.DB.DeleteMAObjectPermanent(id2);
            }
        }

        [TestMethod()]
        public void UniqueValueConstructorSelfUniqueRandomStringTest()
        {
            UniqueValueConstructor attributeConstructor = new UniqueValueConstructor();
            attributeConstructor.ValueDeclaration = new UniqueValueDeclaration("%randstring:10%");
            attributeConstructor.Attribute = ActiveConfig.DB.GetAttribute("personID");
            attributeConstructor.UniqueAllocationAttributes.Add(attributeConstructor.Attribute);

            Guid id1 = Guid.NewGuid();
            Guid id2 = Guid.NewGuid();

            try
            {
                MAObjectHologram sourceObject1 = ActiveConfig.DB.CreateMAObject(id1, "person");
                MAObjectHologram sourceObject2 = ActiveConfig.DB.CreateMAObject(id2, "person");

                attributeConstructor.Execute(sourceObject1);

                AttributeValue value = sourceObject1.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("personID"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any value");
                }

                sourceObject1.CommitCSEntryChange();

                attributeConstructor.Execute(sourceObject2);

                value = sourceObject2.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("personID"));
                if (value.IsNull)
                {
                    Assert.Fail("The constructor did not generate any value");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id1);
                ActiveConfig.DB.DeleteMAObjectPermanent(id2);
            }
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
