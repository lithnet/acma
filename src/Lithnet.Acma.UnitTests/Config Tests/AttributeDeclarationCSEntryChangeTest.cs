
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
    public class AttributeDelcarationCSEntryChangeTest
    {
        public AttributeDelcarationCSEntryChangeTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVLong()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("unixGid");
            string declarationString = "{unixGid}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, 99L));

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

        [TestMethod()]
        public void ExpandSimpleAttributeSVBoolean()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToCallista");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{connectedToCallista}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, true));

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

        [TestMethod()]
        public void ExpandSimpleAttributeSVBinary()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{objectSid}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, new byte[] { 0, 1, 2, 3, 4 }));

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

        [TestMethod()]
        public void ExpandSimpleAttributeSVString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mail}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, "test.test@test.com"));

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

        [TestMethod()]
        public void ExpandSimpleAttributeSVStringWithOptionalText()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "[{mail}.au]";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, "test.test@test.com"));

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

        [TestMethod()]
        public void ExpandSimpleAttributeSVStringWithOptionalText2()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "[{mail}.au]";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;

            IList<object> value = target.Expand(sourceObject);

            if (value != null && value.Count > 0)
            {
                Assert.Fail("The declaration string unexpectedly returned a value");
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


            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, new Guid("8c3cbf4e-5216-4a04-9140-b0b11020fa4c")));

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


        [TestMethod()]
        public void ExpandSimpleAttributeSVLongWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("sapExpiryDate");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{sapExpiryDate>>Add30Days}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, 1L));

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

        [TestMethod()]
        public void ExpandSimpleAttributeSVStringWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mail>>ToUpperCase}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, "test.test@test.com"));

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

        // SV Attribute by Reference

        [TestMethod()]
        public void ThrowOnReferencedAttribute()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("supervisor");
            string declarationString = "{supervisor->unixGid}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, Guid.NewGuid()));

            try
            {
                object value = target.Expand(sourceObject).First();

                Assert.Fail("The expansion did not trigger an exception");
            }
            catch (NotSupportedException)
            { }
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

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, new List<object>() { 1L, 2L, 3L, 4L }));

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

        [TestMethod()]
        public void ExpandSimpleAttributeMVBoolean()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToCallista");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{connectedToCallista}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, true));

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

        [TestMethod()]
        public void ExpandSimpleAttributeMVString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mailAlternateAddresses}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" }));

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

        [TestMethod()]
        public void ExpandSimpleAttributeMVReference()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{directReports}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, new List<object>() { new Guid("fb116a90-35e3-47e9-92b7-679c4821e648"), new Guid("352285df-3fdc-468e-9412-70d62a62cefe"), new Guid("50ac8dbb-15fe-485c-8f02-87a77ab68a7b") }));

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

        // Transformed MV Attribute -> MV Attribute (transforms only supported on long and string values)

        [TestMethod()]
        public void ExpandSimpleAttributeMVLongWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{expiryDates>>Add30Days}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, new List<object>() { 1L, 2L, 3L, 4L }));

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

        [TestMethod()]
        public void ExpandSimpleAttributeMVStringWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mailAlternateAddresses>>ToUpperCase}";
            AttributeDeclarationParser p = new AttributeDeclarationParser(declarationString);
            AttributeDeclaration target = p.GetAttributeDeclaration();

            CSEntryChange sourceObject = CSEntryChange.Create();
            sourceObject.ObjectModificationType = ObjectModificationType.Add;
            sourceObject.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(attribute.Name, new List<object>() { "test.test@TEST.COM", "test1.test1@TEST.COM", "test2.test2@TEST.COM" }));

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
    }
}
