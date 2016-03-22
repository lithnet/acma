using Lithnet.Acma;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using Microsoft.MetadirectoryServices;
using Lithnet.Logging;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;
using System.Collections.ObjectModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for AttributeValueQueryTest and is intended
    ///to contain all AttributeValueQueryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DBQueryByValueTest
    {
        public DBQueryByValueTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            DBQueryByValue toSeralize = new DBQueryByValue();
            toSeralize.Description = "attribute";
            toSeralize.Operator = ValueOperator.Equals;
            toSeralize.SearchAttribute = ActiveConfig.DB.GetAttribute("sn");
            toSeralize.ValueDeclarations = new List<ValueDeclaration>() { new ValueDeclaration("test value1"), new ValueDeclaration("test value2") };
            DBQueryByValue deserialized = (DBQueryByValue)UnitTestControl.XmlSerializeRoundTrip<DBQueryByValue>(toSeralize);

            Assert.AreEqual(toSeralize.Description, deserialized.Description);
            Assert.AreEqual(toSeralize.Operator, deserialized.Operator);
            Assert.AreEqual(toSeralize.SearchAttribute, deserialized.SearchAttribute);

            CollectionAssert.AreEqual(toSeralize.ValueDeclarations.ToArray(), deserialized.ValueDeclarations.ToArray());
        }

        [TestMethod()]
        public void DBQueryBySingleValueSVEquals()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, "test.test@test.com");
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueSVContains()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Contains, "test.test");
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueSVStartsWith()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.StartsWith, "test.test");
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueSVEndsWith()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.EndsWith, "@test.com");
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test1.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueSVNotEquals()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.NotEquals, "test1.test1@test.com");
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueSVGreaterThan()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.GreaterThan, 99L);
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 100L);
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 98L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueSVGreaterThanOrEq()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.GreaterThanOrEq, 99L);
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 100L);
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 98L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueSVLessThan()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.LessThan, 99L);
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 98L);
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 100L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueSVLessThanOrEq()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.LessThanOrEq, 99L);
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 98L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 100L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }
       
        [TestMethod()]
        public void DBQueryByValueSVIsPresent()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.IsPresent, new ValueDeclaration());
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), null);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByValueSVIsNotPresent()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.NotPresent, new ValueDeclaration());
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), null);
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueMVEquals()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Equals, "test.test@test.com");
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test.test@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueMVNotEquals()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.NotEquals, "test.test@test.com");
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test1.test1@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueMVGreaterThan()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.GreaterThan, 99L);
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 100L, 110L, 90L });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 80L, 85L, 90L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueMVGreaterThanOrEq()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.GreaterThanOrEq, 99L);
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 100L, 110L, 90L });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 99L, 98L, 90L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 80L, 85L, 90L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueMVLessThan()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.LessThan, 99L);
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 100L, 110L, 90L });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 110L, 120L, 100L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueMVLessThanOrEq()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.LessThanOrEq, 99L);
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 100L, 110L, 90L });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 99L, 100L, 110L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 110L, 120L, 100L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryBySingleValueMVContains()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Contains, "\\%@test.com");
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test.test@test.com", "test2.test2@test2.com", "test3.test3@test3.com" });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), "test1.test1@test1.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByValueMVIsPresent()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.IsPresent, new ValueDeclaration());
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test.test@test.com", "test2.test2@test2.com", "test3.test3@test3.com" });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.DeleteAttribute(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByValueMVNotPresent()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.NotPresent, new ValueDeclaration());
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                // sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test.test@test.com", "test2.test2@test2.com", "test3.test3@test3.com" });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test.test@test.com", "test2.test2@test2.com", "test3.test3@test3.com" });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }


        [TestMethod()]
        public void DBQueryByMultivalueSVEquals()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new List<object>() { "test.test@test.com", "test2.test2@test.com" });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueSVNotEquals()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.NotEquals, new List<object>() { "test1.test1@test.com", "test2.test2@test.com" });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueSVGreaterThan()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.GreaterThan, new List<object>() { 99L, 95L });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 100L);
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 90L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMutliValueSVGreaterThanOrEq()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.GreaterThanOrEq, new List<object>() { 99L, 100L });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 100L);
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 98L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueSVLessThan()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.LessThan, new List<object>() { 99L, 95L });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 98L);
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 100L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueSVLessThanOrEq()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.LessThanOrEq, new List<object>() { 99L, 95L });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 98L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 100L);
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMutlivalueSVContains()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Contains, new List<object>() { "\\%@test.com", "\\%@test99.com" });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test1.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueMVEquals()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Equals, new List<object>() { "test.test@test.com", "test3.test3@test.com", "test2.test2@test.com" });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test.test@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueMVNotEquals()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.NotEquals, new List<object>() { "test.test@test.com", "test3.test5@test.com", "test2.test6@test.com" });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test1.test1@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueMVGreaterThan()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.GreaterThan, new List<object>() { 99L, 98L });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 100L, 110L, 90L });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 80L, 85L, 90L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueMVGreaterThanOrEq()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.GreaterThanOrEq, new List<object>() { 99L, 98L });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 100L, 110L, 90L });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 99L, 98L, 90L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 80L, 85L, 90L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueMVLessThan()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.LessThan, new List<object>() { 99L, 98L });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 100L, 110L, 90L });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 110L, 120L, 100L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueMVLessThanOrEq()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.LessThanOrEq, new List<object>() { 99L, 98L });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 100L, 110L, 90L });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 99L, 100L, 110L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), new List<object> { 110L, 120L, 100L });
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryByMultivalueMVContains()
        {
            Guid newId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Contains, new List<object>() { "\\%@test.com", "\\%@test99.com" });
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, newId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram sourceObject = ActiveConfig.DB.CreateMAObject(newId, "person");
                                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object> { "test.test@test.com", "test2.test2@test2.com", "test3.test3@test3.com" });
                sourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != newId)
                {
                    Assert.Fail("The incorrect object was returned");
                }


                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), "test1.test1@test1.com");
                sourceObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(group).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(newId);
            }
        }

        [TestMethod()]
        public void DBQueryValueExceptionOnNull()
        {
            try
            {
                DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Contains, new ValueDeclaration());
                DBQueryGroup group = new DBQueryGroup(GroupOperator.Any);
                group.AddChildQueryObjects(match1);
                DBQueryBuilder builder = new DBQueryBuilder(group, 0);
                Assert.Fail("The expected exception wasn't thrown");
            }
            catch (QueryValueNullException)
            {
            }
        }

        [TestMethod()]
        public void DBQueryValueExceptionOnEmptyArray()
        {
            try
            {
                DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Contains, new List<object>());
                DBQueryGroup group = new DBQueryGroup(GroupOperator.Any);
                group.AddChildQueryObjects(match1);
                DBQueryBuilder builder = new DBQueryBuilder(group, 0);

                Assert.Fail("The expected exception wasn't thrown");
            }
            catch (QueryValueNullException)
            {
            }
        }

        [TestMethod()]
        public void DBQueryValueExceptionOnNullValueInArray()
        {
            try
            {
                DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Contains, new List<object>() { null });
                DBQueryGroup group = new DBQueryGroup(GroupOperator.Any);
                group.AddChildQueryObjects(match1);
                DBQueryBuilder builder = new DBQueryBuilder(group, 0);
                Assert.Fail("The expected exception wasn't thrown");
            }
            catch (QueryValueNullException)
            {
            }
        }
    }
}
