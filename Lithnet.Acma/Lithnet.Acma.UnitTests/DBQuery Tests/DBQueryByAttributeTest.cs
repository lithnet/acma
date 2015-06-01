using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Lithnet.Acma;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using Microsoft.MetadirectoryServices;
using Lithnet.Logging;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for DBQueryByAttributeTest and is intended
    ///to contain all DBQueryByAttributeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DBQueryByAttributeTest
    {

        public DBQueryByAttributeTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            DBQueryByValue toSeralize = new DBQueryByValue(ActiveConfig.DB.GetAttribute("sn"),  ValueOperator.Equals,ActiveConfig.DB.GetAttribute("firstName") );
            toSeralize.Description = "attribute";

            DBQueryByValue deserialized = (DBQueryByValue)UnitTestControl.XmlSerializeRoundTrip<DBQueryByValue>(toSeralize);

            Assert.AreEqual(toSeralize.Description, deserialized.Description);
            Assert.AreEqual(toSeralize.Operator, deserialized.Operator);
            Assert.AreEqual(toSeralize.SearchAttribute, deserialized.SearchAttribute);
            Assert.AreEqual(toSeralize.ValueDeclarations[0].Declaration, deserialized.ValueDeclarations[0].Declaration);
        }

        [TestMethod()]
        public void DBQueryByAttributeSVToSVEquals()
        {
            Guid searchObjectId = Guid.NewGuid();
            Guid valueSourceObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("mail"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, new ValueDeclaration(searchObjectId.ToString()));
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram searchObject = UnitTestControl.DataContext.CreateMAObject(searchObjectId, "person");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.CommitCSEntryChange();

                MAObjectHologram valueSourceObject = UnitTestControl.DataContext.CreateMAObject(valueSourceObjectId, "person");
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                valueSourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().Id != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                valueSourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                valueSourceObject.CommitCSEntryChange();

                results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(searchObjectId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(valueSourceObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryByAttributeSVToSVNotEquals()
        {
            Guid searchObjectId = Guid.NewGuid();
            Guid valueSourceObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.NotEquals, ActiveConfig.DB.GetAttribute("mail"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, searchObjectId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram searchObject = UnitTestControl.DataContext.CreateMAObject(searchObjectId, "person");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                searchObject.CommitCSEntryChange();

                MAObjectHologram valueSourceObject = UnitTestControl.DataContext.CreateMAObject(valueSourceObjectId, "person");
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test1.test1@test.com");
                valueSourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().Id != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                valueSourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                valueSourceObject.CommitCSEntryChange();

                results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(searchObjectId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(valueSourceObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryByAttributeMVToSVEquals()
        {
            Guid searchObjectId = Guid.NewGuid();
            Guid valueSourceObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("mail"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, searchObjectId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram searchObject = UnitTestControl.DataContext.CreateMAObject(searchObjectId, "person");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" });
                searchObject.CommitCSEntryChange();

                MAObjectHologram valueSourceObject = UnitTestControl.DataContext.CreateMAObject(valueSourceObjectId, "person");
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");
                valueSourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().Id != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                valueSourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test3.test3@test.com");

                valueSourceObject.CommitCSEntryChange();

                results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(searchObjectId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(valueSourceObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryByAttributeMVToSVNotEquals()
        {
            Guid searchObjectId = Guid.NewGuid();
            Guid valueSourceObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.NotEquals, ActiveConfig.DB.GetAttribute("mail"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, searchObjectId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram searchObject = UnitTestControl.DataContext.CreateMAObject(searchObjectId, "person");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test1.test1@test.com", "test2.test2@test.com" });
                searchObject.CommitCSEntryChange();

                MAObjectHologram valueSourceObject = UnitTestControl.DataContext.CreateMAObject(valueSourceObjectId, "person");
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test4.test4@test.com");
                valueSourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().Id != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                valueSourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mail"), "test.test@test.com");

                valueSourceObject.CommitCSEntryChange();

                results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(searchObjectId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(valueSourceObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryByAttributeMVToMVEquals()
        {
            Guid searchObjectId = Guid.NewGuid();
            Guid valueSourceObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, searchObjectId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram searchObject = UnitTestControl.DataContext.CreateMAObject(searchObjectId, "person");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                searchObject.CommitCSEntryChange();

                MAObjectHologram valueSourceObject = UnitTestControl.DataContext.CreateMAObject(valueSourceObjectId, "person");
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test4.test4@test.com", "test.test@test.com" });
                valueSourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().Id != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                valueSourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test4.test4@test.com", "test5.test5@test.com" });

                valueSourceObject.CommitCSEntryChange();

                results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(searchObjectId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(valueSourceObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryByAttributeMVToMVNotEquals()
        {
            Guid searchObjectId = Guid.NewGuid();
            Guid valueSourceObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.NotEquals, ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));
            DBQueryByValue match2 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("objectId"), ValueOperator.Equals, searchObjectId);
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1, match2);

            try
            {
                MAObjectHologram searchObject = UnitTestControl.DataContext.CreateMAObject(searchObjectId, "person");
                searchObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test5.test5@test.com", "test6.test6@test.com", "test7.test7@test.com" });
                searchObject.CommitCSEntryChange();

                MAObjectHologram valueSourceObject = UnitTestControl.DataContext.CreateMAObject(valueSourceObjectId, "person");
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test.test@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                valueSourceObject.CommitCSEntryChange();

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count() != 1)
                {
                    Assert.Fail("The incorrect number of results were returned");
                }

                if (results.First().Id != searchObjectId)
                {
                    Assert.Fail("The incorrect object was returned");
                }

                valueSourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                valueSourceObject.SetAttributeValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), new List<object>() { "test5.test5@test.com", "test2.test2@test.com", "test3.test3@test.com" });
                valueSourceObject.CommitCSEntryChange();

                results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, valueSourceObject).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("The test returned an unexpected object");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(searchObjectId);
                UnitTestControl.DataContext.DeleteMAObjectPermanent(valueSourceObjectId);
            }
        }


        [TestMethod()]
        public void DBQueryByAttributeExceptionOnSVNull()
        {
            Guid valueSourceObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mail"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("mail"));
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1);

            try
            {
                MAObjectHologram valueSourceObject = UnitTestControl.DataContext.CreateMAObject(valueSourceObjectId, "person");
                valueSourceObject.CommitCSEntryChange();

                try
                {
                    DBQueryBuilder queryBuilder = new DBQueryBuilder(group, 0, valueSourceObject);
                    Assert.Fail("The expected exception was not thrown");
                }
                catch (QueryValueNullException)
                {
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(valueSourceObjectId);
            }
        }

        [TestMethod()]
        public void DBQueryByAttributeExceptionOnMVNull()
        {
            Guid valueSourceObjectId = Guid.NewGuid();

            DBQueryByValue match1 = new DBQueryByValue(ActiveConfig.DB.GetAttribute("mailAlternateAddresses"), ValueOperator.Equals, ActiveConfig.DB.GetAttribute("mailAlternateAddresses"));
            DBQueryGroup group = new DBQueryGroup(GroupOperator.All);
            group.AddChildQueryObjects(match1);

            try
            {
                MAObjectHologram valueSourceObject = UnitTestControl.DataContext.CreateMAObject(valueSourceObjectId, "person");
                valueSourceObject.CommitCSEntryChange();

                try
                {
                    DBQueryBuilder queryBuilder = new DBQueryBuilder(group, 0, valueSourceObject);
                    Assert.Fail("The expected exception was not thrown");
                }
                catch (QueryValueNullException)
                {
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(valueSourceObjectId);
            }
        }

    }
}
