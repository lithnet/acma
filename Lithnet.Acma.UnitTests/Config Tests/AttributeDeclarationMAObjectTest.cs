
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

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for DeclarativeValueConstructorTest and is intended
    ///to contain all DeclarativeValueConstructorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AttributeDelcarationMAObjectTest
    {
        public AttributeDelcarationMAObjectTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVLong()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("unixGid");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{unixGid}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixGid"), 99L);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is long))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((long)value != 99L)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVBoolean()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{connectedToCallista}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("connectedToCallista"), true);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is bool))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((bool)value != true)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVBinary()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{objectSid}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"), new byte[] { 0, 1, 2, 3, 4 });

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is byte[]))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((byte[])value).SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mail}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "test.test@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVStringWithOptionalText()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "[{mail}.au]";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "test.test@test.com.au")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVStringWithOptionalText2()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "[{mail}.au]";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                //sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                IList<object> value = target.Expand(sourceObject);

                if (value != null && value.Count > 0)
                {
                    Assert.Fail("The declaration string unexpectedly returned a value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void ExpandSimpleAttributeSVReference()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{supervisor}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), new Guid("8c3cbf4e-5216-4a04-9140-b0b11020fa4c"));

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is Guid))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((Guid)value != new Guid("8c3cbf4e-5216-4a04-9140-b0b11020fa4c"))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        // Transformed SV Attribute -> SV Attribute (transforms only supported on long and string values)

        [TestMethod()]
        public void ExpandSimpleAttributeSVLongWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("sapExpiryDate");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{sapExpiryDate>>Add30Days}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sapExpiryDate"), 1L);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is long))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((long)value != 25920000000001L)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVStringWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mail>>ToUpperCase}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "TEST.TEST@TEST.COM")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        // SV Attribute by Reference

        [TestMethod()]
        public void ExpandReferencedAttributeSVLong()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{supervisor->unixGid}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            Guid parentId = Guid.NewGuid();
            try
            {
                MAObjectHologram parentObject = UnitTestControl.DataContext.CreateMAObject(parentId, "person");
                parentObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixGid"), 99L);
                parentObject.CommitCSEntryChange();

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), parentId);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is long))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((long)value != 99L)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parentId);
            }
        }

        [TestMethod()]
        public void ExpandReferencedAttributeSVBoolean()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{supervisor->connectedToSap}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            Guid parentId = Guid.NewGuid();
            try
            {
                MAObjectHologram parentObject = UnitTestControl.DataContext.CreateMAObject(parentId, "person");
                parentObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"), true);
                parentObject.CommitCSEntryChange();

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), parentId);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is bool))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((bool)value != true)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parentId);
            }
        }

        [TestMethod()]
        public void ExpandReferencedAttributeSVBinary()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{supervisor->objectSid}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            Guid parentId = Guid.NewGuid();
            try
            {
                MAObjectHologram parentObject = UnitTestControl.DataContext.CreateMAObject(parentId, "person");
                parentObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"), new byte[] { 0, 1, 2, 3, 4 });
                parentObject.CommitCSEntryChange();

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), parentId);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is byte[]))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((byte[])value).SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parentId);
            }
        }

        [TestMethod()]
        public void ExpandReferencedAttributeSVString()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{supervisor->mail}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            Guid parentId = Guid.NewGuid();
            try
            {
                MAObjectHologram parentObject = UnitTestControl.DataContext.CreateMAObject(parentId, "person");
                parentObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                parentObject.CommitCSEntryChange();

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), parentId);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "test.test@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parentId);
            }
        }

        [TestMethod()]
        public void ExpandReferencedAttributeSVReference()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{supervisor->supervisor}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            Guid parentId = Guid.NewGuid();
            try
            {
                MAObjectHologram parentObject = UnitTestControl.DataContext.CreateMAObject(parentId, "person");
                parentObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), new Guid("6d3ff395-c94d-43df-90ae-c9e03579b7d4"));
                parentObject.CommitCSEntryChange();

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), parentId);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is Guid))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((Guid)value != new Guid("6d3ff395-c94d-43df-90ae-c9e03579b7d4"))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parentId);
            }
        }

        // SV attribute by SV reference chain

        [TestMethod()]
        public void ExpandSVReferenceChainAttributeSVString()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{supervisor->supervisor->supervisor->mail}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            Guid parent1Id = Guid.NewGuid();
            Guid parent2Id = Guid.NewGuid();
            Guid parent3Id = Guid.NewGuid();

            try
            {
                MAObjectHologram parentObject1 = UnitTestControl.DataContext.CreateMAObject(parent1Id, "person");
                parentObject1.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), parent2Id);
                parentObject1.CommitCSEntryChange();

                MAObjectHologram parentObject2 = UnitTestControl.DataContext.CreateMAObject(parent2Id, "person");
                parentObject2.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), parent3Id);
                parentObject2.CommitCSEntryChange();

                MAObjectHologram parentObject3 = UnitTestControl.DataContext.CreateMAObject(parent3Id, "person");
                parentObject3.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                parentObject3.CommitCSEntryChange();

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), parent1Id);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "test.test@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parent1Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parent2Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parent3Id);
            }
        }

        // MV attribute by MV reference 

        [TestMethod()]
        public void ExpandMVReferenceAttributeMVString()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{directReports->mail}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            Guid child1Id = Guid.NewGuid();
            Guid child2Id = Guid.NewGuid();
            Guid child3Id = Guid.NewGuid();

            try
            {
                MAObjectHologram childObject1 = UnitTestControl.DataContext.CreateMAObject(child1Id, "person");
                childObject1.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                childObject1.CommitCSEntryChange();

                MAObjectHologram childObject2 = UnitTestControl.DataContext.CreateMAObject(child2Id, "person");
                childObject2.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test2.test2@test.com");
                childObject2.CommitCSEntryChange();

                MAObjectHologram childObject3 = UnitTestControl.DataContext.CreateMAObject(child3Id, "person");
                childObject3.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test3.test3@test.com");
                childObject3.CommitCSEntryChange();

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object>() { child1Id, child2Id, child3Id });

                object value = target.Expand(sourceObject);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is IList<object>))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((IList<object>)value).SequenceEqual(new List<object>() { "test.test@test.com", "test2.test2@test.com", "test3.test3@test.com" }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child1Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child2Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child3Id);
            }
        }

        [TestMethod()]
        public void ExpandMVReferenceChainAttributeMVString()
        {
            /*
             * Reference Map
             * 
             * parentObject/directReports -> 
             *                 childObject1/directReports ->
             *                                              childObject3/mail
             *                                              childObject4/mail
             *                 childObject1/directReports ->
             *                                              childObject5/mail
             *                                              childObject6/mail
             */

            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{directReports->directReports->mail}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid parentId = Guid.NewGuid();
            Guid child1Id = Guid.NewGuid();
            Guid child2Id = Guid.NewGuid();
            Guid child3Id = Guid.NewGuid();
            Guid child4Id = Guid.NewGuid();
            Guid child5Id = Guid.NewGuid();
            Guid child6Id = Guid.NewGuid();
            try
            {
                MAObjectHologram childObject3 = UnitTestControl.DataContext.CreateMAObject(child3Id, "person");
                childObject3.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "1test.test@test.com");
                childObject3.CommitCSEntryChange();

                MAObjectHologram childObject4 = UnitTestControl.DataContext.CreateMAObject(child4Id, "person");
                childObject4.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "2test2.test2@test.com");
                childObject4.CommitCSEntryChange();

                MAObjectHologram childObject5 = UnitTestControl.DataContext.CreateMAObject(child5Id, "person");
                childObject5.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "3test3.test3@test.com");
                childObject5.CommitCSEntryChange();

                MAObjectHologram childObject6 = UnitTestControl.DataContext.CreateMAObject(child6Id, "person");
                childObject6.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "4test4.test4@test.com");
                childObject6.CommitCSEntryChange();

                MAObjectHologram childObject1 = UnitTestControl.DataContext.CreateMAObject(child1Id, "person");
                childObject1.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object>() { child3Id, child4Id });
                childObject1.CommitCSEntryChange();

                MAObjectHologram childObject2 = UnitTestControl.DataContext.CreateMAObject(child2Id, "person");
                childObject2.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object>() { child6Id, child5Id });
                childObject2.CommitCSEntryChange();

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(parentId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object>() { child1Id, child2Id });

                object value = target.Expand(sourceObject);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is IList<object>))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((IList<object>)value).OrderBy(t => t).SequenceEqual(new List<object>() { "1test.test@test.com", "2test2.test2@test.com", "3test3.test3@test.com", "4test4.test4@test.com" }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parentId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child1Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child2Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child3Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child4Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child5Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child6Id);
            }
        }

        // MV Attribute -> MV Attribute

        [TestMethod()]
        public void ExpandSimpleAttributeMVLong()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{expiryDates}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object>() { 1L, 2L, 3L, 4L });

                object value = target.Expand(sourceObject);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is IList<object>))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((IList<object>)value).SequenceEqual(new List<object>() { 1L, 2L, 3L, 4L }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeMVBoolean()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{connectedToCallista}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("connectedToCallista"), true);

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is bool))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((bool)value != true)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeMVString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mailAlternateAddresses}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" });

                object value = target.Expand(sourceObject);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is IList<object>))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((IList<object>)value).SequenceEqual(new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeMVReference()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{directReports}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object>() { new Guid("fb116a90-35e3-47e9-92b7-679c4821e648"), new Guid("352285df-3fdc-468e-9412-70d62a62cefe"), new Guid("50ac8dbb-15fe-485c-8f02-87a77ab68a7b") });

                object value = target.Expand(sourceObject);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is IList<object>))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((IList<object>)value).SequenceEqual(new List<object>() { new Guid("fb116a90-35e3-47e9-92b7-679c4821e648"), new Guid("352285df-3fdc-468e-9412-70d62a62cefe"), new Guid("50ac8dbb-15fe-485c-8f02-87a77ab68a7b") }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        // Transformed MV Attribute -> MV Attribute (transforms only supported on long and string values)

        [TestMethod()]
        public void ExpandSimpleAttributeMVLongWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{expiryDates>>Add30Days}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object>() { 1L, 2L, 3L, 4L });

                object value = target.Expand(sourceObject);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is IList<object>))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((IList<object>)value).SequenceEqual(new List<object>() { 25920000000001L, 25920000000002L, 25920000000003L, 25920000000004L }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeMVStringWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mailAlternateAddresses>>ToUpperCase}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" });

                object value = target.Expand(sourceObject);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is IList<object>))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((IList<object>)value).SequenceEqual(new List<object>() { "TEST.TEST@TEST.COM", "TEST1.TEST1@TEST.COM", "TEST2.TEST2@TEST.COM" }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandMVReferenceChainAttributeMVStringWithTransforms()
        {
            /*
             * Reference Map
             * 
             * parentObject/directReports -> 
             *                 childObject1/directReports ->
             *                                              childObject3/mail
             *                                              childObject4/mail
             *                 childObject1/directReports ->
             *                                              childObject5/mail
             *                                              childObject6/mail
             */

            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{directReports->directReports->mail>>ToUpperCase}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid parentId = Guid.NewGuid();
            Guid child1Id = Guid.NewGuid();
            Guid child2Id = Guid.NewGuid();
            Guid child3Id = Guid.NewGuid();
            Guid child4Id = Guid.NewGuid();
            Guid child5Id = Guid.NewGuid();
            Guid child6Id = Guid.NewGuid();
            try
            {
                MAObjectHologram childObject3 = UnitTestControl.DataContext.CreateMAObject(child3Id, "person");
                childObject3.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "1test.test@test.com");
                childObject3.CommitCSEntryChange();

                MAObjectHologram childObject4 = UnitTestControl.DataContext.CreateMAObject(child4Id, "person");
                childObject4.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "2test2.test2@test.com");
                childObject4.CommitCSEntryChange();

                MAObjectHologram childObject5 = UnitTestControl.DataContext.CreateMAObject(child5Id, "person");
                childObject5.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "3test3.test3@test.com");
                childObject5.CommitCSEntryChange();

                MAObjectHologram childObject6 = UnitTestControl.DataContext.CreateMAObject(child6Id, "person");
                childObject6.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "4test4.test4@test.com");
                childObject6.CommitCSEntryChange();

                MAObjectHologram childObject1 = UnitTestControl.DataContext.CreateMAObject(child1Id, "person");
                childObject1.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object>() { child3Id, child4Id });
                childObject1.CommitCSEntryChange();

                MAObjectHologram childObject2 = UnitTestControl.DataContext.CreateMAObject(child2Id, "person");
                childObject2.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object>() { child6Id, child5Id });
                childObject2.CommitCSEntryChange();

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(parentId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("directReports"), new List<object>() { child1Id, child2Id });

                object value = target.Expand(sourceObject);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is IList<object>))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((IList<object>)value).OrderBy(t => t).SequenceEqual(new List<object>() { "1TEST.TEST@TEST.COM", "2TEST2.TEST2@TEST.COM", "3TEST3.TEST3@TEST.COM", "4TEST4.TEST4@TEST.COM" }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(parentId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child1Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child2Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child3Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child4Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child5Id);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(child6Id);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVStringCurrentView()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{#mail}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test2.test2@test.com");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "test.test@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVStringProposedView()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mail}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test2.test2@test.com");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "test2.test2@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(newId);
            }
        }
    }
}
