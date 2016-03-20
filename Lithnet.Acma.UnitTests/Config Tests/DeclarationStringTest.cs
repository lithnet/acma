
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using System.Text.RegularExpressions;
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
    public class DeclarationStringTest
    {
        public DeclarationStringTest()
        {
            UnitTestControl.Initialize();
        }

        // Fixed value -> SV attribute
        [TestMethod()]
        public void ExpandSimpleFixedSVLong()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("unixUid");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "5";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                object value = target.Expand(sourceObject).First();

                value = TypeConverter.ConvertData<long>(value);
                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is long))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((long)value != 5L)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleFixedSVBooleanTrueString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "true";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                object value = target.Expand(sourceObject).First();
                value = TypeConverter.ConvertData<bool>(value);


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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleFixedSVBooleanFalseString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "false";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                object value = target.Expand(sourceObject).First();
                value = TypeConverter.ConvertData<bool>(value);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is bool))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((bool)value != false)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleFixedSVBinary()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "AAECAwQ=";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                object value = target.Expand(sourceObject).First();
                value = TypeConverter.ConvertData<byte[]>(value);

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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleFixedSVString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "test.test@test.com";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        // SV Attribute -> SV Attribute

        [TestMethod()]
        public void ExpandSimpleAttributeSVLong()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("unixUid");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{unixGid}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVDateTime()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{dateTimeSV}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV"), DateTime.Parse("2010-01-01"));

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is DateTime))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((DateTime)value != DateTime.Parse("2010-01-01"))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVBoolean()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");

            string declarationString = "{connectedToCallista}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVBinary()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{objectSid}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mail}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVReference()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{supervisor}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        // SV Variable 

        [TestMethod()]
        public void ExpandSimpleVariableSVDate()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "%date%";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is DateTime))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (((DateTime)value).Date != DateTime.Now.Date)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleVariableSVUtcDate()
        {
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "%utcdate%";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is DateTime))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (((DateTime)value).Date != DateTime.UtcNow.Date)
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        // Transformed SV Attribute -> SV Attribute (transforms only supported on long and string values)

        [TestMethod()]
        public void ExpandSimpleAttributeSVLongWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("sapExpiryDate");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{sapExpiryDate>>Add30Days}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeSVStringWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mail>>ToUpperCase}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        // MV Attribute -> MV Attribute

        [TestMethod()]
        public void ExpandSimpleAttributeMVLong()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{expiryDates}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeMVDateTime()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("dateTimeMV");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{dateTimeMV}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeMV"), new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") });

                object value = target.Expand(sourceObject);

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is IList<object>))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (!((IList<object>)value).SequenceEqual(new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") }))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeMVBoolean()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{connectedToCallista}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeMVString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mailAlternateAddresses}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeMVReference()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{directReports}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        // Transformed MV Attribute -> MV Attribute (transforms only supported on long and string values)

        [TestMethod()]
        public void ExpandSimpleAttributeMVLongWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{expiryDates>>Add30Days}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandSimpleAttributeMVStringWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{mailAlternateAddresses>>ToUpperCase}";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
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
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        // Complex String Constructions

        [TestMethod()]
        public void ExpandComplexSVString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{firstName}.{sn}@test.com";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), "test1");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "test1.test2@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandComplexSVStringLong()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{sapExpiryDate}.{sn}@test.com";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sapExpiryDate"), 44L);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "44.test2@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandComplexSVStringBinary()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{objectSid}.{sn}@test.com";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("objectSid"), new byte[] { 0, 1, 2, 3, 4 });
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "AAECAwQ=.test2@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandComplexSVStringBool()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{connectedToSap}.{sn}@test.com";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"), true);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "True.test2@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandComplexSVStringWithTransforms()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{firstName>>GetFirstCharacter}.{sn}@test.com";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), "test1");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != "t.test2@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void ExpandComplexSVStringWithVariable()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{firstName}.{sn}.%date%@test.com";
            ValueDeclaration target = new ValueDeclaration(declarationString);

            Guid newId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), "test1");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");

                object value = target.Expand(sourceObject).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if (Regex.IsMatch((string)value, @"test1\.test2\.\n?@test\.com"))
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
            }
        }

        private bool IsAttributeUnique(MAObjectHologram maObject, IEnumerable<string> uniqueAllocationAttributes, string valueToTest)
        {
            foreach (AcmaSchemaAttribute attribute in uniqueAllocationAttributes.Select(t => ActiveConfig.DB.GetAttribute(t)))
            {
                if (MAObjectHologram.DoesAttributeValueExist(attribute, valueToTest, maObject.ObjectID))
                {
                    return false;
                }
            }

            return true;
        }

        [TestMethod()]
        public void ExpandComplexSVStringWithMandatoryUniqueAllocationVariable()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{firstName}.{sn}%n%@test.com";
            UniqueValueDeclaration target = new UniqueValueDeclaration(declarationString);
            UniqueValueConstructor constructor = new UniqueValueConstructor();
            constructor.ValueDeclaration = target;
            constructor.UniqueAllocationAttributes = new List<AcmaSchemaAttribute>() { attribute };
            Guid newId = Guid.NewGuid();
            Guid existingId = Guid.NewGuid();
            try
            {
                MAObjectHologram existingObject = MAObjectHologram.CreateMAObject(existingId, "person");
                existingObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), newId.ToString() + "test1");
                existingObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");
                existingObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), newId.ToString() + "test1.test21@test.com");
                existingObject.CommitCSEntryChange();

                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), newId.ToString() + "test1");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");
                sourceObject.CommitCSEntryChange();

                object value = target.Expand(sourceObject, (string t, string u) => constructor.IsAttributeUnique(sourceObject, t, u)).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != newId.ToString() + "test1.test22@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
                MAObjectHologram.DeleteMAObjectPermanent(existingId);
            }
        }

        [TestMethod()]
        public void ExpandComplexSVStringWithOptionalUniqueAllocationVariableRequired()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{firstName}.{sn}%o%@test.com";
            UniqueValueDeclaration target = new UniqueValueDeclaration(declarationString);
            UniqueValueConstructor constructor = new UniqueValueConstructor();
            constructor.ValueDeclaration = target;
            constructor.UniqueAllocationAttributes = new List<AcmaSchemaAttribute>() { attribute };
            Guid newId = Guid.NewGuid();
            Guid existingId = Guid.NewGuid();
            try
            {
                MAObjectHologram existingObject = MAObjectHologram.CreateMAObject(existingId, "person");
                existingObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), newId.ToString() + "test1");
                existingObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");
                existingObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), newId.ToString() + "test1.test2@test.com");
                existingObject.CommitCSEntryChange();

                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), newId.ToString() + "test1");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");
                sourceObject.CommitCSEntryChange();

                object value = target.Expand(sourceObject, (string t, string u) => constructor.IsAttributeUnique(sourceObject, t, u)).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != newId.ToString() + "test1.test21@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
                MAObjectHologram.DeleteMAObjectPermanent(existingId);
            }
        }

        [TestMethod()]
        public void ExpandComplexSVStringWithOptionalUniqueAllocationVariableNotRequired()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            IList<string> uniqueAllocationAttributes = new List<string>() { "mail" };
            string declarationString = "{firstName}.{sn}%o%@test.com";
            UniqueValueDeclaration target = new UniqueValueDeclaration(declarationString);
            UniqueValueConstructor constructor = new UniqueValueConstructor();
            constructor.ValueDeclaration = target;
            constructor.UniqueAllocationAttributes = new List<AcmaSchemaAttribute>() { attribute };
            
            Guid newId = Guid.NewGuid();
            Guid existingId = Guid.NewGuid();
            try
            {
                MAObjectHologram sourceObject = MAObjectHologram.CreateMAObject(newId, "person");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("firstName"), newId.ToString() + "test1");
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sn"), "test2");
                sourceObject.CommitCSEntryChange();

                object value = target.Expand(sourceObject, (string t, string u) => constructor.IsAttributeUnique(sourceObject, t, u)).First();

                if (value == null)
                {
                    Assert.Fail("The declaration string did not return a value");
                }
                else if (!(value is string))
                {
                    Assert.Fail("The declaration string returned the wrong data type");
                }
                else if ((string)value != newId.ToString() + "test1.test2@test.com")
                {
                    Assert.Fail("The declaration string did not return the expected value");
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newId);
                MAObjectHologram.DeleteMAObjectPermanent(existingId);
            }
        }

        // Expected Exceptions on User Input Error

        [TestMethod()]
        public void ExceptionOnMVAttributeInComplexString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{expiryDates}@test.com";
            Guid newGuid = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = MAObjectHologram.CreateMAObject(newGuid, "person");
                maObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object>() { 5L, 6L });

                ValueDeclaration target = new ValueDeclaration(declarationString);
                target.Expand(maObject);
                Assert.Fail("The declaration string did not throw an exception");
            }
            catch (DeclarationExpansionException)
            {
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newGuid);
            }
        }

        [TestMethod()]
        public void ExceptionOnMultipleUniqueAllocationVariables()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            IList<string> uniqueAllocationAttributes = new List<string>() { "mail" };
            string declarationString = "{firstName}.{sn}%n%%o%@mail.com";

            try
            {
                UniqueValueDeclaration target = new UniqueValueDeclaration(declarationString);
                target.ParseDeclaration();

                if (!target.ErrorList.ContainsKey("Declaration"))
                {
                    Assert.Fail("The declaration string did not record and error");
                }
            }
            catch (InvalidDeclarationStringException)
            {
            }
        }

        [TestMethod()]
        public void ExceptionOnUniqueAllocationVariableWithoutUniqueAllocationAttributes()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{firstName}.{sn}%n%@mail.com";
            Guid newGuid = Guid.NewGuid();

            try
            {
                MAObjectHologram maObject = MAObjectHologram.CreateMAObject(newGuid, "person");

                ValueDeclaration target = new ValueDeclaration(declarationString);
                target.Expand(maObject);

                Assert.Fail("The declaration string did not throw an exception");
            }
            catch (DeclarationExpansionException ex)
            {
                if (!(ex.InnerException is InvalidDeclarationStringException))
                {
                    throw;
                }
            }
            finally
            {
                MAObjectHologram.DeleteMAObjectPermanent(newGuid);
            }
        }

        [TestMethod()]
        public void ExceptionOnSimpleUniqueAllocator()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "%n%";

            try
            {
                ValueDeclaration target = new ValueDeclaration(declarationString);
                target.Expand();
                Assert.Fail("The declaration string did not throw an exception");
            }
            catch (DeclarationExpansionException ex)
            {
                if (!(ex.InnerException is InvalidDeclarationStringException))
                {
                    throw;
                }
            }
        }

        [TestMethod()]
        public void ExceptionOnReferenceAttributeInComplexString()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{supervisor}@test.com";

            try
            {
                ValueDeclaration target = new ValueDeclaration(declarationString);
                target.Expand();
                //target.Expand();
                Assert.Fail("The declaration string did not throw an exception");
            }
            catch (DeclarationExpansionException ex)
            {
                if (!(ex.InnerException is InvalidDeclarationStringException))
                {
                    throw;
                }
            }
        }

        [TestMethod()]
        public void ExceptionOnIncompleteString1()
        {
            string declarationString = "{supervisor}{";

            try
            {
                ValueDeclaration target = new ValueDeclaration(declarationString);
                target.Expand();
                Assert.Fail("The declaration string did not throw an exception");
            }
            catch (DeclarationExpansionException ex)
            {
                if (!(ex.InnerException is InvalidDeclarationStringException))
                {
                    throw;
                }
            }
        }

        [TestMethod()]
        public void ExceptionOnIncompleteString2()
        {
            string declarationString = "{";

            try
            {
                ValueDeclaration target = new ValueDeclaration(declarationString);
                target.Expand();
                Assert.Fail("The declaration string did not throw an exception");
            }
            catch (DeclarationExpansionException ex)
            {
                if (!(ex.InnerException is InvalidDeclarationStringException))
                {
                    throw;
                }
            }
        }

        [TestMethod()]
        public void ExceptionOnIncompleteString3()
        {
            string declarationString = "%";

            try
            {
                ValueDeclaration target = new ValueDeclaration(declarationString);
                target.Expand();
                Assert.Fail("The declaration string did not throw an exception");
            }
            catch (DeclarationExpansionException ex)
            {
                if (!(ex.InnerException is InvalidDeclarationStringException))
                {
                    throw;
                }
            }
        }

        [TestMethod()]
        public void ExceptionOnMissingTransform()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaObjectClass schemaObject = ActiveConfig.DB.GetObjectClass("person");
            string declarationString = "{firstName}.{sn}@test.com";
            string transforms = "NonExistentTransform";

            try
            {
                ValueDeclaration target = new ValueDeclaration(declarationString, transforms);
                target.Expand();
                Assert.Fail("The declaration string did not throw an exception");
            }
            catch (DeclarationExpansionException)
            {
            }
        }
    }
}
