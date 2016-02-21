using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
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
    public class CSEntryImportTest
    {
        public CSEntryImportTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void ImportCSEntryChangeTest()
        {
            Dictionary<AcmaSchemaObjectClass, IEnumerable<AcmaSchemaAttribute>> types = new Dictionary<AcmaSchemaObjectClass, IEnumerable<AcmaSchemaAttribute>>();
            List<AcmaSchemaAttribute> attributes = new List<AcmaSchemaAttribute>();
            attributes.Add(ActiveConfig.DB.GetAttribute("mail"));
            attributes.Add(ActiveConfig.DB.GetAttribute("unixUid"));
            attributes.Add(ActiveConfig.DB.GetAttribute("supervisor"));
            attributes.Add(ActiveConfig.DB.GetAttribute("connectedToSap"));
            attributes.Add(ActiveConfig.DB.GetAttribute("objectSid"));
            attributes.Add(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));
            attributes.Add(ActiveConfig.DB.GetAttribute("expiryDates"));
            attributes.Add(ActiveConfig.DB.GetAttribute("directReports"));
            attributes.Add(ActiveConfig.DB.GetAttribute("objectSids"));
            attributes.Add(ActiveConfig.DB.GetAttribute("dateTimeSV"));
            attributes.Add(ActiveConfig.DB.GetAttribute("dateTimeMV"));

            types.Add(ActiveConfig.DB.GetObjectClass("person"), attributes);

            Guid id = Guid.NewGuid();
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.DN = id.ToString();
            csentry.ObjectModificationType = ObjectModificationType.Add;
            csentry.ObjectType = "person";
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mail", "fimtest+test.test@test.com"));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("mailAlternateAddresses", new List<object> { "fimtest+test1.test1@test.com", "fimtest+test2.test2@test.com" }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("unixUid", 44L));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("expiryDates", new List<object>() { 55L, 66L, 77L }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("directReports", new List<object>() { new Guid("{8FC92471-7835-4804-8BBB-0A5ED7078074}"), new Guid("{0EF7CC21-729E-4ED9-A3AF-8203796334C6}") }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("supervisor", new Guid("{2807ED76-E262-4EB4-ABD9-9629F3830F12}")));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("connectedToSap", true));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSids", new List<object>() { new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 2, 4, 6, 8, 0 } }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("objectSid", new byte[] { 0, 1, 2, 3, 4 }));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("dateTimeSV", "2010-01-01"));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd("dateTimeMV", "2011-01-01"));

            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");
            try
            {
                bool refretry;
                CSEntryExport.PutExportEntry(csentry, UnitTestControl.DataContext, out refretry);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.GetMAObject(id, objectClass);
                CSEntryChange generatedCSEntry = CSEntryImport.GetCSEntry(sourceObject, types);

                if (generatedCSEntry.ErrorCodeImport != MAImportError.Success)
                {
                    Assert.Fail("The CSEntryChange generator failed to generate the object");
                }

                AttributeValue value;

                value = new AttributeValue(ActiveConfig.DB.GetAttribute("mail"), generatedCSEntry.AttributeChanges["mail"].ValueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add).Value);
                if (value != "fimtest+test.test@test.com")
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                value = new AttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), generatedCSEntry.AttributeChanges["unixUid"].ValueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add).Value);
                if (value != 44L)
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                value = new AttributeValue(ActiveConfig.DB.GetAttribute("connectedToSap"), generatedCSEntry.AttributeChanges["connectedToSap"].ValueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add).Value);
                if (value != true)
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                value = new AttributeValue(ActiveConfig.DB.GetAttribute("objectSid"), generatedCSEntry.AttributeChanges["objectSid"].ValueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add).Value);
                if (value != new byte[] { 0, 1, 2, 3, 4 })
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                value = new AttributeValue(ActiveConfig.DB.GetAttribute("supervisor"), generatedCSEntry.AttributeChanges["supervisor"].ValueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add).Value);
                if (value != new Guid("{2807ED76-E262-4EB4-ABD9-9629F3830F12}"))
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                string dateValue = generatedCSEntry.AttributeChanges["dateTimeSV"].ValueChanges.FirstOrDefault(t => t.ModificationType == ValueModificationType.Add).Value as string;
                if (dateValue != "2010-01-01T00:00:00.000")
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                AttributeValues values = new InternalAttributeValues(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), generatedCSEntry.AttributeChanges["mailAlternateAddresses"].ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                if (!values.ContainsAllElements(new List<object> { "fimtest+test1.test1@test.com", "fimtest+test2.test2@test.com" }))
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                List<string> list = generatedCSEntry.AttributeChanges["dateTimeMV"].ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).Cast<string>().ToList();
                if (!list.ContainsSameElements(new List<object> { "2011-01-01T00:00:00.000" }))
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                values = new InternalAttributeValues(ActiveConfig.DB.GetAttribute("expiryDates"), generatedCSEntry.AttributeChanges["expiryDates"].ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                if (!values.ContainsAllElements(new List<object>() { 55L, 66L, 77L }))
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                values = new InternalAttributeValues(ActiveConfig.DB.GetAttribute("directReports"), generatedCSEntry.AttributeChanges["directReports"].ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                if (!values.ContainsAllElements(new List<object>() { new Guid("{8FC92471-7835-4804-8BBB-0A5ED7078074}"), new Guid("{0EF7CC21-729E-4ED9-A3AF-8203796334C6}") }))
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

                values = new InternalAttributeValues(ActiveConfig.DB.GetAttribute("objectSids"), generatedCSEntry.AttributeChanges["objectSids"].ValueChanges.Where(t => t.ModificationType == ValueModificationType.Add).Select(t => t.Value).ToList());
                if (!values.ContainsAllElements(new List<object>() { new byte[] { 0, 1, 2, 3, 4, 5 }, new byte[] { 2, 4, 6, 8, 0 } }))
                {
                    Assert.Fail("One or more attribute changes were not generated");
                }

            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(id);
            }
        }
    }
}
