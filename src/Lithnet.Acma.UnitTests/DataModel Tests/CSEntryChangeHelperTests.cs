
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
    [TestClass]
    public class CSEntryChangeHelperTests
    {
        public CSEntryChangeHelperTests()
        {
            UnitTestControl.Initialize();
        }
       
        [TestMethod()]
        public void TestCSEntryChangeAttributeReplaceOnObjectAdd()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Add;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Add, expectedValues);

            // With existing attribute add
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, startValues));
            csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Add, expectedValues);

            // With existing attribute update
            // Invalid

            // With existing attribute replace
            // Invalid

            // With existing attribute delete
            // Invalid
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeReplaceOnObjectReplace()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Replace;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Add, expectedValues);

            // With existing attribute add
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, startValues));
            csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Add, expectedValues);

            // With existing attribute update
            // Invalid

            // With existing attribute replace
            // Invalid

            // With existing attribute delete
            // Invalid
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeReplaceOnObjectDelete()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Delete;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            try
            {
                csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
                Assert.Fail("The expected exception was not thrown");
            }
            catch (DeletedObjectModificationException)
            {
            }
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeReplaceOnObjectUpdate()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Update;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Replace, expectedValues);

            // With existing attribute add
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, startValues));
            csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Replace, expectedValues);

            // With existing attribute update
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(testAttribute.Name, startValueChanges));
            csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Replace, expectedValues);

            // With existing attribute replace
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(testAttribute.Name, startValues));
            csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Replace, expectedValues);

            // With existing attribute delete
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(testAttribute.Name));
            csentry.AttributeChanges.ReplaceAttribute(objectModificationType, testAttribute, changedValues);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Replace, expectedValues);
        }


        [TestMethod()]
        public void TestCSEntryChangeAttributeDeleteOnObjectAdd()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Add;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Unconfigured, null);

            // With existing attribute add
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, startValues));
            csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Unconfigured, null);

            // With existing attribute update
            // Invalid

            // With existing attribute replace
            // Invalid

            // With existing attribute delete
            // Invalid
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeDeleteOnObjectReplace()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Replace;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Unconfigured, null);

            // With existing attribute add
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, startValues));
            csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Unconfigured, null);

            // With existing attribute update
            // Invalid

            // With existing attribute replace
            // Invalid

            // With existing attribute delete
            // Invalid
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeDeleteOnObjectDelete()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Delete;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            try
            {
                csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
                Assert.Fail("The expected exception was not thrown");
            }
            catch (DeletedObjectModificationException)
            {
            }
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeDeleteOnObjectUpdate()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Update;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Delete, null);

            // With existing attribute add
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, startValues));
            csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Unconfigured, null);

            // With existing attribute update
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(testAttribute.Name, startValueChanges));
            csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Delete, null);

            // With existing attribute replace
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(testAttribute.Name, startValues));
            csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Delete, null);

            // With existing attribute delete
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(testAttribute.Name));
            csentry.AttributeChanges.DeleteAttribute(objectModificationType, testAttribute);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Delete, null);
        }


        [TestMethod()]
        public void TestCSEntryChangeAttributeUpdateOnObjectAdd()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Add;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<ValueChange> changedValueChanges = new List<ValueChange>(changedValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changedValueChanges);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Add, expectedValues);

            // With existing attribute add
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, startValues));
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changedValueChanges);
            expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com", "test.test@test.com", "test1.test1@test.com" };
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Add, expectedValues);

            // With existing attribute update
            // Invalid

            // With existing attribute replace
            // Invalid

            // With existing attribute delete
            // Invalid
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeUpdateOnObjectReplace()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Replace;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<ValueChange> changedValueChanges = new List<ValueChange>(changedValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changedValueChanges);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Add, expectedValues);

            // With existing attribute add
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, startValues));
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changedValueChanges);
            expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com", "test.test@test.com", "test1.test1@test.com" };
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Add, expectedValues);

            // With existing attribute update
            // Invalid

            // With existing attribute replace
            // Invalid

            // With existing attribute delete
            // Invalid
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeUpdateOnObjectDelete()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Delete;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            try
            {
                csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, startValueChanges);
                Assert.Fail("The expected exception was not thrown");
            }
            catch (DeletedObjectModificationException)
            {
            }
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeUpdateOnObjectUpdate()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Update;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            IList<object> startValues = new List<object>() { "test.test@test.com", "test1.test1@test.com" };
            IList<ValueChange> startValueChanges = new List<ValueChange>(startValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> changedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            IList<ValueChange> changedValueChanges = new List<ValueChange>(changedValues.Select(t => ValueChange.CreateValueAdd(t)));
            IList<object> expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            CSEntryChange csentry;

            // No pending AttributeChanges
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changedValueChanges);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Update, expectedValues);

            // With existing attribute add
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute.Name, startValues));
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changedValueChanges);
            expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com", "test.test@test.com", "test1.test1@test.com" };
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Add, expectedValues);

            // With existing attribute update
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(testAttribute.Name, startValueChanges));
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changedValueChanges);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Update, expectedValues);

            // With existing attribute replace
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeReplace(testAttribute.Name, startValues));
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changedValueChanges);
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Replace, expectedValues);

            // With existing attribute delete
            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeDelete(testAttribute.Name));
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changedValueChanges);
            expectedValues = new List<object>() { "test2.test2@test.com", "test3.test3@test.com" };
            TestAttributeChangeResults(csentry, testAttribute, AttributeModificationType.Replace, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeUpdateValueChangeRemoveDuplicates()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Update;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            CSEntryChange csentry;

            List<ValueChange> startValues = new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") };
            List<ValueChange> changes = new List<ValueChange>() { ValueChange.CreateValueAdd("test2.test2@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") };
            List<ValueChange> expectedValues = new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com"), ValueChange.CreateValueAdd("test2.test2@test.com") };

            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(testAttribute.Name, startValues));
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changes);
            TestAttributeChangeResults(csentry, testAttribute, expectedValues);
        }

        [TestMethod()]
        public void TestCSEntryChangeAttributeUpdateValueChangeRemoveAddDelete()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Update;
            AcmaSchemaAttribute testAttribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            CSEntryChange csentry;

            List<ValueChange> startValues = new List<ValueChange>() { ValueChange.CreateValueAdd("test.test@test.com"), ValueChange.CreateValueAdd("test1.test1@test.com") };
            List<ValueChange> changes = new List<ValueChange>() { ValueChange.CreateValueDelete("test.test@test.com") };
            List<ValueChange> expectedValues = new List<ValueChange>() { ValueChange.CreateValueAdd("test1.test1@test.com") };

            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeUpdate(testAttribute.Name, startValues));
            csentry.AttributeChanges.UpdateAttribute(objectModificationType, testAttribute, changes);
            TestAttributeChangeResults(csentry, testAttribute, expectedValues);
        }


        [TestMethod()]
        public void TestCSEntryChangeSplitCSEntry()
        {
            ObjectModificationType objectModificationType = ObjectModificationType.Add;
            AcmaSchemaAttribute testAttribute1 = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaAttribute testAttribute2 = ActiveConfig.DB.GetAttribute("accountName");
            CSEntryChange csentry;
            Guid reference = Guid.NewGuid();

            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute1.Name, reference));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute2.Name, "testuser"));

            List<CSEntryChange> changes = CSEntryChangeExtensions.SplitReferenceUpdatesFromCSEntryChangeAdd(csentry).ToList();

            if (changes.Count != 2)
            {
                Assert.Fail("The operation did not return the correct number of CSEntryChangeObjects");
            }

            CSEntryChange addChange = changes[0];
            
            if(addChange.ObjectModificationType != ObjectModificationType.Add)
            {
                Assert.Fail("The operation did not correct set the object modification type");
            }

            if (addChange.AttributeChanges.Count != 1)
            {
                Assert.Fail("The operation did not return the correct number of attribute changes");
            }

            if (addChange.AttributeChanges[0].Name != testAttribute2.Name)
            {
                Assert.Fail("The operation did not correctly split the reference attributes from the CSEntryChange");
            }

            if (addChange.AttributeChanges[0].ValueChanges[0].Value.ToString() != "testuser")
            {
                Assert.Fail("The operation did not correctly update the attribute change with the correct value");
            }

            CSEntryChange updateChange = changes[1];

            if (updateChange.ObjectModificationType != ObjectModificationType.Update)
            {
                Assert.Fail("The operation did not correct set the object modification type");
            }

            if (updateChange.AttributeChanges.Count != 1)
            {
                Assert.Fail("The operation did not return the correct number of attribute changes");
            }

            if (updateChange.AttributeChanges[0].Name != testAttribute1.Name)
            {
                Assert.Fail("The operation did not correctly split the reference attributes from the CSEntryChange");
            }

            if (updateChange.AttributeChanges[0].ValueChanges[0].Value.ToString() != reference.ToString())
            {
                Assert.Fail("The operation did not correctly update the attribute change with the correct value");
            }

        }

        [TestMethod()]
        public void TestCSEntryChangeSplitCSEntries()
        {
            List<CSEntryChange> incomingchanges = new List<CSEntryChange>();
            ObjectModificationType objectModificationType = ObjectModificationType.Add;
            AcmaSchemaAttribute testAttribute1 = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaAttribute testAttribute2 = ActiveConfig.DB.GetAttribute("accountName");
            CSEntryChange csentry;
            Guid reference = Guid.NewGuid();

            csentry = CreateNewCSEntry(objectModificationType);
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute1.Name, reference));
            csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute2.Name, "testuser"));

            incomingchanges.Add(csentry);

            CSEntryChange csentry2;
            csentry2 = CreateNewCSEntry(objectModificationType);
            csentry2.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(testAttribute2.Name, "testuser"));

            incomingchanges.Add(csentry2);

            List<CSEntryChange> changes = CSEntryChangeExtensions.SplitReferenceUpdatesFromCSEntryChanges(incomingchanges).ToList();

            if (changes.Count != 3)
            {
                Assert.Fail("The operation returned the incorrect number of changes");
            }
        }

        private static CSEntryChange CreateNewCSEntry(ObjectModificationType type)
        {
            CSEntryChange csentry = CSEntryChange.Create();
            csentry.DN = Guid.NewGuid().ToString();
            csentry.ObjectModificationType = type;
            csentry.ObjectType = "person";
            return csentry;
        }

        private static void TestAttributeChangeResults(CSEntryChange csentry, AcmaSchemaAttribute testAttribute, AttributeModificationType expectedModificationType, IList<object> expectedValues)
        {
            AttributeChange attributeChange = null;

            if (csentry.AttributeChanges.Contains(testAttribute.Name))
            {
                attributeChange = csentry.AttributeChanges[testAttribute.Name];
            }

            if (attributeChange == null && expectedModificationType == AttributeModificationType.Unconfigured)
            {
                return;
            }
            else if (attributeChange == null && expectedModificationType != AttributeModificationType.Unconfigured)
            {
                Assert.Fail("An AttributeChange was found where none were expected");
            }
            else if (attributeChange == null)
            {
                Assert.Fail("An AttributeChange was expected but not found");
            }

            if (attributeChange.ModificationType != expectedModificationType)
            {
                Assert.Fail("The AttributeChange was not of the expected type");
            }

            if (expectedModificationType != AttributeModificationType.Delete)
            {
                List<AttributeValue> comparerValues1 = attributeChange.ValueChanges.Select(t => new AttributeValue(testAttribute, t.Value)).ToList();
                List<AttributeValue> comparerValues2 = expectedValues.Select(t => new AttributeValue(testAttribute, t)).ToList();

                CollectionAssert.AreEquivalent(comparerValues1, comparerValues2);
                
            }
        }

        private static void TestAttributeChangeResults(CSEntryChange csentry, AcmaSchemaAttribute testAttribute, IList<ValueChange> expectedValueChanges)
        {
            AttributeChange attributeChange = null;

            if (csentry.AttributeChanges.Contains(testAttribute.Name))
            {
                attributeChange = csentry.AttributeChanges[testAttribute.Name];
            }

            if (attributeChange == null)
            {
                Assert.Fail("An AttributeChange was expected but not found");
            }

            if (attributeChange.ModificationType != AttributeModificationType.Update)
            {
                Assert.Fail("The AttributeChange was not of the expected type");
            }

            if (attributeChange.ValueChanges.Count != expectedValueChanges.Count)
            {
                Assert.Fail("The expected values were not found");
            }

            for (int i = 0; i < expectedValueChanges.Count; i++)
            {
                if (attributeChange.ValueChanges[i].ModificationType != expectedValueChanges[i].ModificationType)
                {
                    Assert.Fail("The expected values were not found");
                }

                AttributeValue value1 = new AttributeValue(testAttribute, attributeChange.ValueChanges[i].Value);
                AttributeValue value2 = new AttributeValue(testAttribute, expectedValueChanges[i].Value);

                if (value1 != value2)
                {
                    Assert.Fail("The expected values were not found");
                }
            }
        }
    }
}
