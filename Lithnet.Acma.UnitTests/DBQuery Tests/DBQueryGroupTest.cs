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
    ///This is a test class for DBQueryByAttributeTest and is intended
    ///to contain all DBQueryByAttributeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DBQueryGroupTest
    {
        public DBQueryGroupTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            DBQueryGroup toSeralize = new DBQueryGroup();
            toSeralize.Operator = GroupOperator.None;

            DBQueryByValue test1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("sn"),  ValueOperator.Equals, ActiveConfig.DB.GetAttribute("firstName"));
            test1.Description = "description";
            toSeralize.DBQueries.Add(test1);

            DBQueryByValue test2 = new DBQueryByValue();
            test2.ValueDeclarations = new List<ValueDeclaration>() { new ValueDeclaration("test1"), new ValueDeclaration("test2") };
            test2.Description = "description";
            test2.Operator = ValueOperator.Equals;
            test2.SearchAttribute = ActiveConfig.DB.GetAttribute("sn");
            toSeralize.DBQueries.Add(test2);

            DBQueryGroup deserialized = (DBQueryGroup)UnitTestControl.XmlSerializeRoundTrip<DBQueryGroup>(toSeralize);

            Assert.AreEqual(toSeralize.Operator, deserialized.Operator);
            Assert.AreEqual(toSeralize.DBQueries.Count, deserialized.DBQueries.Count);

            DBQueryByValue deserialized1 = deserialized.DBQueries[0] as DBQueryByValue;

            Assert.AreEqual(test1.Description, deserialized1.Description);
            Assert.AreEqual(test1.Operator, deserialized1.Operator);
            Assert.AreEqual(test1.SearchAttribute, deserialized1.SearchAttribute);
            Assert.AreEqual(test1.ValueDeclarations[0].Declaration, deserialized1.ValueDeclarations[0].Declaration);

            DBQueryByValue deserialized2 = deserialized.DBQueries[1] as DBQueryByValue;

            Assert.AreEqual(test2.Description, deserialized2.Description);
            Assert.AreEqual(test2.Operator, deserialized2.Operator);
            Assert.AreEqual(test2.SearchAttribute, deserialized2.SearchAttribute);
            CollectionAssert.AreEqual(test2.ValueDeclarations.ToArray(), deserialized2.ValueDeclarations.ToArray());

        }

        [TestMethod()]
        public void TestSerializationComplex()
        {
            DBQueryGroup group1 = new DBQueryGroup();
            group1.Operator = GroupOperator.Any;

            DBQueryByValue test1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("sn"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("firstName") );
            test1.Description = "description1";
            group1.DBQueries.Add(test1);

            DBQueryByValue test2 = new DBQueryByValue();
            test2.ValueDeclarations = new List<ValueDeclaration> { new ValueDeclaration("test21"), new ValueDeclaration("test22") };
            test2.Description = "description2";
            test2.Operator = ValueOperator.Equals;
            test2.SearchAttribute = ActiveConfig.DB.GetAttribute("sn");
            group1.DBQueries.Add(test2);

            DBQueryGroup group2 = new DBQueryGroup();
            group2.Operator = GroupOperator.One;

            DBQueryByValue test3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("sn"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("firstName") );
            test3.Description = "description3";
            group2.DBQueries.Add(test3);

            DBQueryByValue test4 = new DBQueryByValue();
            test4.ValueDeclarations = new List<ValueDeclaration> { new ValueDeclaration("test11"), new ValueDeclaration("test12") };
            test4.Description = "description4";
            test4.Operator = ValueOperator.Equals;
            test4.SearchAttribute = ActiveConfig.DB.GetAttribute("sn");
            group2.DBQueries.Add(test4);

            group1.DBQueries.Add(group2);

            DBQueryGroup deserializedGroup1 = (DBQueryGroup)UnitTestControl.XmlSerializeRoundTrip<DBQueryGroup>(group1);

            Assert.AreEqual(group1.DBQueries.Count, deserializedGroup1.DBQueries.Count);
            Assert.AreEqual(group1.Operator, deserializedGroup1.Operator);

            DBQueryByValue deserialized1 = group1.DBQueries[0] as DBQueryByValue;

            Assert.AreEqual(test1.Description, deserialized1.Description);
            Assert.AreEqual(test1.Operator, deserialized1.Operator);
            Assert.AreEqual(test1.SearchAttribute, deserialized1.SearchAttribute);
            Assert.AreEqual(test1.ValueDeclarations[0].Declaration, deserialized1.ValueDeclarations[0].Declaration);

            DBQueryByValue deserialized2 = group1.DBQueries[1] as DBQueryByValue;

            Assert.AreEqual(test2.Description, deserialized2.Description);
            Assert.AreEqual(test2.Operator, deserialized2.Operator);
            Assert.AreEqual(test2.SearchAttribute, deserialized2.SearchAttribute);
            CollectionAssert.AreEqual(test2.ValueDeclarations.ToArray(), deserialized2.ValueDeclarations.ToArray());

            DBQueryGroup deserializedGroup2 = (DBQueryGroup)deserializedGroup1.DBQueries[2];
            Assert.AreEqual(group2.Operator, deserializedGroup2.Operator);

            DBQueryByValue deserialized3 = group2.DBQueries[0] as DBQueryByValue;

            Assert.AreEqual(test3.Description, deserialized3.Description);
            Assert.AreEqual(test3.Operator, deserialized3.Operator);
            Assert.AreEqual(test3.SearchAttribute, deserialized3.SearchAttribute);
            Assert.AreEqual(test3.ValueDeclarations[0].Declaration, deserialized3.ValueDeclarations[0].Declaration);

            DBQueryByValue deserialized4 = group2.DBQueries[1] as DBQueryByValue;

            Assert.AreEqual(test4.Description, deserialized4.Description);
            Assert.AreEqual(test4.Operator, deserialized4.Operator);
            Assert.AreEqual(test4.SearchAttribute, deserialized4.SearchAttribute);
            CollectionAssert.AreEqual(test4.ValueDeclarations.ToArray(), deserialized4.ValueDeclarations.ToArray());

        }


        [TestMethod()]
        public void DBQueryGroupOr()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.Any);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group1);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }


        [TestMethod()]
        public void DBQueryGroupOrNestedAnd()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue match3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("accountName"), ValueOperator.Equals, new ValueDeclaration("testuser"));
            DBQueryByValue match4 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.All);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup group2 = new DBQueryGroup(GroupOperator.Any);
            group2.AddChildQueryObjects(match3, match4, group1);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group2);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 99L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryGroupOrNestedOr()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue match3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("accountName"), ValueOperator.Equals, new ValueDeclaration("testuser"));
            DBQueryByValue match4 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.Any);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup group2 = new DBQueryGroup(GroupOperator.Any);
            group2.AddChildQueryObjects(match3, match4, group1);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group2);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 99L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryGroupOrNestedNot()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue match3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("accountName"), ValueOperator.Equals, new ValueDeclaration("testuser"));
            DBQueryByValue match4 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.None);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup group2 = new DBQueryGroup(GroupOperator.Any);
            group2.AddChildQueryObjects(match3, match4, group1);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group2);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 99L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }


        [TestMethod()]
        public void DBQueryGroupAnd()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.All);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group1);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryGroupAndNestedAnd()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue match3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("accountName"), ValueOperator.Equals, new ValueDeclaration("testuser"));
            DBQueryByValue match4 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.All);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup group2 = new DBQueryGroup(GroupOperator.All);
            group2.AddChildQueryObjects(match3, match4, group1);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group2);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 99L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 99L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryGroupAndNestedOr()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue match3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("accountName"), ValueOperator.Equals, new ValueDeclaration("testuser"));
            DBQueryByValue match4 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.Any);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup group2 = new DBQueryGroup(GroupOperator.All);
            group2.AddChildQueryObjects(match3, match4, group1);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group2);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 99L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 99L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryGroupAndNestedNot()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue match3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("accountName"), ValueOperator.Equals, new ValueDeclaration("testuser"));
            DBQueryByValue match4 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.None);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup group2 = new DBQueryGroup(GroupOperator.All);
            group2.AddChildQueryObjects(match3, match4, group1);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group2);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 99L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 99L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }


        [TestMethod()]
        public void DBQueryGroupNot()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.None);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group1);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryGroupNotNestedAnd()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue match3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("accountName"), ValueOperator.Equals, new ValueDeclaration("testuser"));
            DBQueryByValue match4 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.All);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup group2 = new DBQueryGroup(GroupOperator.None);
            group2.AddChildQueryObjects(match3, match4, group1);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group2);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryGroupNotNestedOr()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue match3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("accountName"), ValueOperator.Equals, new ValueDeclaration("testuser"));
            DBQueryByValue match4 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.Any);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup group2 = new DBQueryGroup(GroupOperator.None);
            group2.AddChildQueryObjects(match3, match4, group1);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group2);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryGroupNotNestedNot()
        {
            Guid searchObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, new ValueDeclaration("test.test@test.com"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("unixUid"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue match3 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("accountName"), ValueOperator.Equals, new ValueDeclaration("testuser"));
            DBQueryByValue match4 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("expiryDates"), ValueOperator.Equals, new ValueDeclaration("99"));
            DBQueryByValue objectMatch = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group1 = new DBQueryGroup(GroupOperator.None);
            group1.AddChildQueryObjects(match1, match2);

            DBQueryGroup group2 = new DBQueryGroup(GroupOperator.None);
            group2.AddChildQueryObjects(match3, match4, group1);

            DBQueryGroup queryGroup = new DBQueryGroup(GroupOperator.All);
            queryGroup.AddChildQueryObjects(objectMatch, group2);

            try
            {
                MAObjectHologram searchObject = ActiveConfig.DB.CreateMAObject(searchObjectId, "person");
                                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser1");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                List<MAObjectHologram> results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().ObjectID != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 99L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }

                searchObject.SetObjectModificationType(ObjectModificationType.Update, false);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("unixUid"), 88L);
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("accountName"), "testuser");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("expiryDates"), 88L);
                searchObject.CommitCSEntryChange();

                results = ActiveConfig.DB.GetMAObjectsFromDBQuery(queryGroup).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                ActiveConfig.DB.DeleteMAObjectPermanent(searchObjectId);
            }
        }
    }
}
