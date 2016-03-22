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
    ///This is a test class for MAObjectHologramTest and is intended
    ///to contain all MAObjectHologramTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CSEntryExportTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        public void ExportCSEntryChangeAdd()
        {
            Guid id = Guid.NewGuid();
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.DN = id.ToString();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = "person";
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mail", "test.test@test.com"));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mailAlternateAddresses", new List<object> { "test1.test1@test.com", "test2.test2@test.com" }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("unixUid", 44L));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("expiryDates", new List<object>() { 55L, 66L, 77L }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("directReports", new List<object>() { new Guid("{8FC92471-7835-4804-8BBB-0A5ED7078074}"), new Guid("{0EF7CC21-729E-4ED9-A3AF-8203796334C6}") }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("supervisor", new Guid("{2807ED76-E262-4EB4-ABD9-9629F3830F12}")));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("connectedToSap", true));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("dateTimeMV", new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("dateTimeSV", DateTime.Parse("2012-01-01")));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSids", new List<object>() { new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 2, 4, 6, 8, 0 } }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSid", new byte[] { 0, 1, 2, 3, 4 }));
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            try
            {
                bool refretry;
                CSEntryExport.PutExportEntry(csentry, out refretry);

                MAObjectHologram sourceObject = ActiveConfig.DB.GetMAObject(id, objectClass);

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail")).ValueString != "test.test@test.com")
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV")).ValueDateTime != DateTime.Parse("2012-01-01"))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("unixUid")).ValueLong != 44L)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap")).ValueBoolean != true)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("objectSid")).ValueByte.SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("supervisor")).ValueGuid != new Guid("{2807ED76-E262-4EB4-ABD9-9629F3830F12}"))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses")).ContainsAllElements(new List<object> { "test1.test1@test.com", "test2.test2@test.com" }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("expiryDates")).ContainsAllElements(new List<object>() { 55L, 66L, 77L }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("dateTimeMV")).ContainsAllElements(new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).ContainsAllElements(new List<object>() { new Guid("{8FC92471-7835-4804-8BBB-0A5ED7078074}"), new Guid("{0EF7CC21-729E-4ED9-A3AF-8203796334C6}") }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("objectSids")).ContainsAllElements(new List<object>() { new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 2, 4, 6, 8, 0 } }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id);
            }
        }

        [TestMethod()]
        public void ExportCSEntryChangeAddResurrect()
        {
            Guid originalId = Guid.NewGuid();
            Guid replacementId = Guid.NewGuid();

            if (!ActiveConfig.XmlConfig.ClassConstructors.Contains("person"))
            {
                ActiveConfig.XmlConfig.ClassConstructors.Add(new ClassConstructor() { ObjectClass = ActiveConfig.DB.GetObjectClass("person") });
            }

            ActiveConfig.XmlConfig.ClassConstructors["person"].ResurrectionParameters = new DBQueryGroup();
            ActiveConfig.XmlConfig.ClassConstructors["person"].ResurrectionParameters.Operator = GroupOperator.Any;
            DBQueryByValue query = new DBQueryByValue(ActiveConfig.DB.GetAttribute("sapPersonId"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("sapPersonId"));
            ActiveConfig.XmlConfig.ClassConstructors["person"].ResurrectionParameters.DBQueries.Add(query);


            CSEntryChange csentry = CSEntryChange.Create();
            csentry.DN = originalId.ToString();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = "person";
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mail", "test.test@test.com"));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mailAlternateAddresses", new List<object> { "test1.test1@test.com", "test2.test2@test.com" }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("unixUid", 44L));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("expiryDates", new List<object>() { 55L, 66L, 77L }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("directReports", new List<object>() { new Guid("{8FC92471-7835-4804-8BBB-0A5ED7078074}"), new Guid("{0EF7CC21-729E-4ED9-A3AF-8203796334C6}") }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("supervisor", new Guid("{2807ED76-E262-4EB4-ABD9-9629F3830F12}")));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("connectedToSap", true));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSids", new List<object>() { new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 2, 4, 6, 8, 0 } }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSid", new byte[] { 0, 1, 2, 3, 4 }));

            try
            {
                bool refretry;
                CSEntryExport.PutExportEntry(csentry, out refretry);
                AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

                MAObjectHologram originalObject = ActiveConfig.DB.GetMAObject(originalId, objectClass);
                originalObject.SetObjectModificationType(ObjectModificationType.Update, false);
                originalObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("sapPersonId"), 7777L);
                originalObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "jesus");
                originalObject.DeletedTimestamp = DateTime.UtcNow.Ticks;
                originalObject.CommitCSEntryChange();

                csentry = CSEntryChange.Create();
                csentry.DN = replacementId.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Add;
                csentry.ObjectType = "person";
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mail", "test.test@test.com"));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("sapPersonId", 7777L));

                CSEntryExport.PutExportEntry(csentry, out refretry);

                MAObjectHologram newObject = ActiveConfig.DB.GetMAObjectOrDefault(replacementId);

                if (newObject == null)
                {
                    Assert.Fail("The object with the new ID was not found");
                }

                if (newObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("accountName")) != "jesus")
                {
                    Assert.Fail("The object was not resurrected");
                }

                if (newObject.DeletedTimestamp != 0)
                {
                    Assert.Fail("The object was not undeleted");
                }

                originalObject = ActiveConfig.DB.GetMAObjectOrDefault(originalId);

                if (originalObject != null)
                {
                    Assert.Fail("The original object still exists");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(originalId);
                ActiveConfig.DB.DeleteMAObjectPermanent(replacementId);
            }
        }

        [TestMethod()]
        public void ExportCSEntryChangeDelete()
        {
            Guid id = Guid.NewGuid();
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.DN = id.ToString();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = "person";
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mail", "test.test@test.com"));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mailAlternateAddresses", new List<object> { "test1.test1@test.com", "test2.test2@test.com" }));

            try
            {
                bool refretry;
                CSEntryExport.PutExportEntry(csentry, out refretry);

                csentry = CSEntryChange.Create();
                csentry.DN = id.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Delete;
                csentry.ObjectType = "person";
                AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

                CSEntryExport.PutExportEntry(csentry, out refretry);

                MAObjectHologram sourceObject = ActiveConfig.DB.GetMAObject(id, objectClass);

                if (sourceObject.DeletedTimestamp == 0)
                {
                    Assert.Fail("The object was not deleted");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id);
            }
        }

        [TestMethod()]
        public void ExportCSEntryChangeUpdate()
        {
            Guid id = Guid.NewGuid();
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.DN = id.ToString();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = "person";
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mail", "test.test@test.com"));

            try
            {
                bool refretry;
                CSEntryExport.PutExportEntry(csentry, out refretry);

                csentry = CSEntryChange.Create();
                csentry.DN = id.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.ObjectType = "person";
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mail", "test1.test1@test.com"));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mailAlternateAddresses", new List<object> { "test1.test1@test.com", "test2.test2@test.com" }));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("unixUid", 44L));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("expiryDates", new List<object>() { 55L, 66L, 77L }));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("directReports", new List<object>() { new Guid("{8FC92471-7835-4804-8BBB-0A5ED7078074}"), new Guid("{0EF7CC21-729E-4ED9-A3AF-8203796334C6}") }));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("supervisor", new Guid("{2807ED76-E262-4EB4-ABD9-9629F3830F12}")));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("connectedToSap", true));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSids", new List<object>() { new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 2, 4, 6, 8, 0 } }));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSid", new byte[] { 0, 1, 2, 3, 4 }));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("dateTimeMV", new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") }));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("dateTimeSV", DateTime.Parse("2012-01-01")));

                CSEntryExport.PutExportEntry(csentry, out refretry);
                AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

                MAObjectHologram sourceObject = ActiveConfig.DB.GetMAObject(id, objectClass);

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail")).ValueString != "test1.test1@test.com")
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("unixUid")).ValueLong != 44L)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap")).ValueBoolean != true)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("objectSid")).ValueByte.SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("supervisor")).ValueGuid != new Guid("{2807ED76-E262-4EB4-ABD9-9629F3830F12}"))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses")).ContainsAllElements(new List<object> { "test1.test1@test.com", "test2.test2@test.com" }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("expiryDates")).ContainsAllElements(new List<object>() { 55L, 66L, 77L }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).ContainsAllElements(new List<object>() { new Guid("{8FC92471-7835-4804-8BBB-0A5ED7078074}"), new Guid("{0EF7CC21-729E-4ED9-A3AF-8203796334C6}") }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("objectSids")).ContainsAllElements(new List<object>() { new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 2, 4, 6, 8, 0 } }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("dateTimeMV")).ContainsAllElements(new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") }))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV")).ValueDateTime != DateTime.Parse("2012-01-01"))
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id);
            }
        }


        [TestMethod()]
        public void ExportCSEntryChangeReplace()
        {
            Guid id = Guid.NewGuid();
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.DN = id.ToString();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = "person";
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mail", "test.test@test.com"));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mailAlternateAddresses", new List<object> { "test1.test1@test.com", "test2.test2@test.com" }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("unixUid", 44L));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("expiryDates", new List<object>() { 55L, 66L, 77L }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("directReports", new List<object>() { new Guid("{8FC92471-7835-4804-8BBB-0A5ED7078074}"), new Guid("{0EF7CC21-729E-4ED9-A3AF-8203796334C6}") }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("supervisor", new Guid("{2807ED76-E262-4EB4-ABD9-9629F3830F12}")));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("connectedToSap", true));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSids", new List<object>() { new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 2, 4, 6, 8, 0 } }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSid", new byte[] { 0, 1, 2, 3, 4 }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("dateTimeMV", new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01") }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("dateTimeSV", DateTime.Parse("2012-01-01")));

            try
            {
                bool refretry;
                CSEntryExport.PutExportEntry(csentry, out refretry);
                AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

                MAObjectHologram sourceObject = ActiveConfig.DB.GetMAObject(id, objectClass);

                csentry = CSEntryChange.Create();
                csentry.DN = id.ToString();
                csentry.ObjectModificationType = ObjectModificationType.Replace;
                csentry.ObjectType = "person";
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mail", "test1.test1@test.com"));

                CSEntryExport.PutExportEntry(csentry, out refretry);

                sourceObject = ActiveConfig.DB.GetMAObject(id, objectClass);
                
                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("mail")).ValueString != "test1.test1@test.com")
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("unixUid")).IsNull)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap")) != false)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("objectSid")).IsNull)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("supervisor")).IsNull)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses")).IsEmptyOrNull)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("expiryDates")).IsEmptyOrNull)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("directReports")).IsEmptyOrNull)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("objectSids")).IsEmptyOrNull)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetMVAttributeValues(ActiveConfig.DB.GetAttribute("dateTimeMV")).IsEmptyOrNull)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }

                if (!sourceObject.GetSVAttributeValue(ActiveConfig.DB.GetAttribute("dateTimeSV")).IsNull)
                {
                    Assert.Fail("One or more attribute changes were not committed");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(id);
            }
        }
    }
}
