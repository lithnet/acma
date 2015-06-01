
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using System.Text.RegularExpressions;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for MADataContextTest and is intended
    ///to contain all MADataContextTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MADataContextTest
    {
        public MADataContextTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void GetMAObjectsByStringSVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object matchValue = "myaccount";
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("accountName");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("accountName");

            GetMAObjectsBySVtoSVMatch(sourceId, objectClass, matchValue, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByBinarySVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object matchValue = new byte[] { 0, 1, 2, 3, 4 };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("objectSid");
            GetMAObjectsBySVtoSVMatch(sourceId, objectClass, matchValue, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByLongSVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object matchValue = 5555L;
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("sapExpiryDate");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("sapExpiryDate");

            GetMAObjectsBySVtoSVMatch(sourceId, objectClass, matchValue, incomingAttribute1, existingAttribute1);
        }


        [TestMethod()]
        public void GetMAObjectsByDateTimeSVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object matchValue = DateTime.Parse("2010-01-01");
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("dateTimeSV");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("dateTimeSV");

            GetMAObjectsBySVtoSVMatch(sourceId, objectClass, matchValue, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByReferenceSVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object matchValue = Guid.NewGuid();
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("supervisor");

            GetMAObjectsBySVtoSVMatch(sourceId, objectClass, matchValue, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByReferenceSVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object incomingValue = Guid.NewGuid();
            List<object> existingValues = new List<object>() { incomingValue, Guid.NewGuid(), Guid.NewGuid() };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("directReports");

            GetMAObjectsBySVtoMVMatch(sourceId, objectClass, incomingValue, existingValues, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByLongSVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object incomingValue = 5555L;
            List<object> existingValues = new List<object>() { incomingValue, 66L, 34534534L };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("sapExpiryDate");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("expiryDates");

            GetMAObjectsBySVtoMVMatch(sourceId, objectClass, incomingValue, existingValues, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByDateTimeSVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object incomingValue = DateTime.Parse("2010-01-01");
            List<object> existingValues = new List<object>() { incomingValue, DateTime.Parse("2011-01-01"), DateTime.Parse("2012-01-01") };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("dateTimeSV");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("dateTimeMV");

            GetMAObjectsBySVtoMVMatch(sourceId, objectClass, incomingValue, existingValues, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByBinarySVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object incomingValue = new byte[] { 0, 1, 2, 3, 4 };
            List<object> existingValues = new List<object>() { new byte[] { 1, 2, 3, 4 }, new byte[] { 2, 3, 4, 5 }, incomingValue };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("objectSids");

            GetMAObjectsBySVtoMVMatch(sourceId, objectClass, incomingValue, existingValues, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByStringSVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object incomingValue = "test.test@test.com";
            List<object> existingValues = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", incomingValue };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            GetMAObjectsBySVtoMVMatch(sourceId, objectClass, incomingValue, existingValues, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByStringMVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues = new List<object>() { "test2.test2@test.com", "test.test@test.com", "test4.test4@test.com" };
            List<object> existingValues = new List<object>() { "test1.test1@test.com", "test3.test3@test.com", "test.test@test.com" };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            GetMAObjectsByMVtoMVMatch(sourceId, objectClass, incomingValues, existingValues, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByBinaryMVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues = new List<object>() { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4 }, new byte[] { 2, 3, 4, 5 } };
            List<object> existingValues = new List<object>() { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 4, 3, 2, 1 }, new byte[] { 5, 4, 3, 2 } };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("objectSids");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("objectSids");

            GetMAObjectsByMVtoMVMatch(sourceId, objectClass, incomingValues, existingValues, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByLongMVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues = new List<object>() { 1111L, 2222L, 3333L };
            List<object> existingValues = new List<object>() { 4444L, 5555L, 1111L };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("expiryDates");

            GetMAObjectsByMVtoMVMatch(sourceId, objectClass, incomingValues, existingValues, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByDateTimeMVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues = new List<object>() { DateTime.Parse("2010-01-01"), DateTime.Parse("2011-01-01"), DateTime.Parse("2012-01-01") };
            List<object> existingValues = new List<object>() { DateTime.Parse("2013-01-01"), DateTime.Parse("2014-01-01"), DateTime.Parse("2010-01-01") };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("dateTimeMV");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("dateTimeMV");

            GetMAObjectsByMVtoMVMatch(sourceId, objectClass, incomingValues, existingValues, incomingAttribute1, existingAttribute1);
        }


        [TestMethod()]
        public void GetMAObjectsByReferenceMVToMVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            Guid guid = Guid.NewGuid();
            List<object> incomingValues = new List<object>() { Guid.NewGuid(), guid, Guid.NewGuid() };
            List<object> existingValues = new List<object>() { Guid.NewGuid(), Guid.NewGuid(), guid };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("directReports");

            GetMAObjectsByMVtoMVMatch(sourceId, objectClass, incomingValues, existingValues, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByReferenceMVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object existingValue = Guid.NewGuid();
            List<object> incomingValues = new List<object>() { existingValue, Guid.NewGuid(), Guid.NewGuid() };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("supervisor");

            GetMAObjectsByMVtoSVMatch(sourceId, objectClass, incomingValues, existingValue, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByLongMVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object existingValue = 5555L;
            List<object> incomingValues = new List<object>() { existingValue, 66L, 34534534L };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("sapExpiryDate");

            GetMAObjectsByMVtoSVMatch(sourceId, objectClass, incomingValues, existingValue, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByDateTimeMVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object existingValue = DateTime.Parse("2010-01-01");
            List<object> incomingValues = new List<object>() { existingValue, DateTime.Parse("2011-01-01"), DateTime.Parse("2012-01-01") };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("dateTimeMV");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("dateTimeSV");

            GetMAObjectsByMVtoSVMatch(sourceId, objectClass, incomingValues, existingValue, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByBinaryMVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object existingValue = new byte[] { 0, 1, 2, 3, 4 };
            List<object> incomingValues = new List<object>() { new byte[] { 1, 2, 3, 4 }, new byte[] { 2, 3, 4, 5 }, existingValue };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("objectSids");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("objectSid");

            GetMAObjectsByMVtoSVMatch(sourceId, objectClass, incomingValues, existingValue, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByStringMVToSVAttributeMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object existingValue = "test.test@test.com";
            List<object> incomingValues = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", existingValue };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mail");

            GetMAObjectsByMVtoSVMatch(sourceId, objectClass, incomingValues, existingValue, incomingAttribute1, existingAttribute1);
        }

        [TestMethod()]
        public void GetMAObjectsByComplexSearchMultipleMatchedAttributes()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test.test@test.com", "test5.test5@test.com" };
            List<object> existingValues1 = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 2, 4, 6, 8 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), Guid.NewGuid(), guid };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            GetMAObjectsByComplexSearch(sourceId, objectClass, incomingValues1, existingValues1, incomingValues2, existingValues2, existingValues3, incomingValues3, incomingAttribute1, existingAttribute1, incomingAttribute2, existingAttribute2, incomingAttribute3, existingAttribute3);
        }

        [TestMethod()]
        public void GetMAObjectsByComplexSearchSingleSVtoMVMatchedAttribute()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test.test@test.com" };
            List<object> existingValues1 = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 7, 7, 7, 7 }, new byte[] { 2, 4, 6, 8 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mail");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            GetMAObjectsByComplexSearch(sourceId, objectClass, incomingValues1, existingValues1, incomingValues2, existingValues2, existingValues3, incomingValues3, incomingAttribute1, existingAttribute1, incomingAttribute2, existingAttribute2, incomingAttribute3, existingAttribute3);
        }

        [TestMethod()]
        public void GetMAObjectsByComplexSearchSingleMVtoMVMatchedAttribute()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test.test@test.com", "test3.test3@test.com" };
            List<object> existingValues1 = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 7, 7, 7, 7 }, new byte[] { 2, 4, 6, 8 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            GetMAObjectsByComplexSearch(sourceId, objectClass, incomingValues1, existingValues1, incomingValues2, existingValues2, existingValues3, incomingValues3, incomingAttribute1, existingAttribute1, incomingAttribute2, existingAttribute2, incomingAttribute3, existingAttribute3);
        }


        [TestMethod()]
        public void GetMAObjectsByComplexNestedAndSearchSingleMVtoMVMatchedAttribute()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test.test@test.com", "test3.test3@test.com" };
            List<object> existingValues1 = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 7, 7, 7, 7 }, new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), guid, Guid.NewGuid() };

            List<object> existingValues4 = new List<object>() { 44L, 55L };
            List<object> incomingValues4 = new List<object>() { 44L, 77L, 99L };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            AcmaSchemaAttribute incomingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaAttribute existingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");

            DBQueryByValue match1 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            DBQueryByValue match2 = new DBQueryByValue(existingAttribute2, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute2.Name)));
            DBQueryByValue match3 = new DBQueryByValue(existingAttribute3, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute3.Name)));
            DBQueryByValue match4 = new DBQueryByValue(existingAttribute4, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute4.Name)));

            DBQueryGroup group1 = new DBQueryGroup();
            group1.Operator = GroupOperator.All;
            group1.AddChildQueryObjects(match1, match4);

            DBQueryGroup group2 = new DBQueryGroup();
            group2.Operator = GroupOperator.All;
            group2.AddChildQueryObjects(match2, group1);

            DBQueryGroup group3 = new DBQueryGroup();
            group3.Operator = GroupOperator.All;
            group3.AddChildQueryObjects(match3, group2);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValues1);
                sourceObject.SetAttributeValue(existingAttribute2, existingValues2);
                sourceObject.SetAttributeValue(existingAttribute3, existingValues3);
                sourceObject.SetAttributeValue(existingAttribute4, existingValues4);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues1));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute2.Name, incomingValues2));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute3.Name, incomingValues3));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute4.Name, incomingValues4));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group3, csentry).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The match was not found");
                }
                else if (results.Count > 1)
                {
                    Assert.Fail("Multiple matches were found");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void GetMAObjectsByComplexNestedAndSearchSingleMVtoMVMatchedAttributeNoResults()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test.test@test.com", "test3.test3@test.com" };
            List<object> existingValues1 = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 7, 7, 7, 7 }, new byte[] { 5, 1, 5, 3, 5 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), guid, Guid.NewGuid() };

            List<object> existingValues4 = new List<object>() { 44L, 55L };
            List<object> incomingValues4 = new List<object>() { 44L, 77L, 99L };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            AcmaSchemaAttribute incomingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaAttribute existingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");

            DBQueryByValue match1 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            DBQueryByValue match2 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute2.Name)));
            DBQueryByValue match3 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute3.Name)));
            DBQueryByValue match4 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute4.Name)));

            DBQueryGroup group1 = new DBQueryGroup();
            group1.Operator = GroupOperator.All;
            group1.AddChildQueryObjects(match1, match4);

            DBQueryGroup group2 = new DBQueryGroup();
            group2.Operator = GroupOperator.All;
            group2.AddChildQueryObjects(match2, group1);

            DBQueryGroup group3 = new DBQueryGroup();
            group3.Operator = GroupOperator.All;
            group3.AddChildQueryObjects(match3, group2);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValues1);
                sourceObject.SetAttributeValue(existingAttribute2, existingValues2);
                sourceObject.SetAttributeValue(existingAttribute3, existingValues3);
                sourceObject.SetAttributeValue(existingAttribute4, existingValues4);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues1));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute2.Name, incomingValues2));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute3.Name, incomingValues3));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute4.Name, incomingValues4));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group3, csentry).ToList();

                if (results.Count > 0)
                {
                    Assert.Fail("Matches were found");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }


        [TestMethod()]
        public void GetMAObjectsByComplexAndSearchSingleMVtoMVMatchedAttribute2()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test.test@test.com", "test3.test3@test.com" };
            List<object> existingValues1 = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 7, 7, 7, 7 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 7, 7, 7, 7 }, new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), guid, Guid.NewGuid() };

            List<object> existingValues4 = new List<object>() { 44L, 55L };
            List<object> incomingValues4 = new List<object>() { 44L, 77L, 99L };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            AcmaSchemaAttribute incomingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaAttribute existingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");

            DBQueryByValue match1 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            DBQueryByValue match2 = new DBQueryByValue(existingAttribute2, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute2.Name)));
            DBQueryByValue match3 = new DBQueryByValue(existingAttribute3, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute3.Name)));
            DBQueryByValue match4 = new DBQueryByValue(existingAttribute4, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute4.Name)));


            DBQueryGroup group1 = new DBQueryGroup();
            group1.Operator = GroupOperator.All;
            group1.AddChildQueryObjects(match1, match2, match4);

            DBQueryGroup group3 = new DBQueryGroup();
            group3.Operator = GroupOperator.All;
            group3.AddChildQueryObjects(match3, group1);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValues1);
                sourceObject.SetAttributeValue(existingAttribute2, existingValues2);
                sourceObject.SetAttributeValue(existingAttribute3, existingValues3);
                sourceObject.SetAttributeValue(existingAttribute4, existingValues4);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues1));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute2.Name, incomingValues2));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute3.Name, incomingValues3));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute4.Name, incomingValues4));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group3, csentry).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The match was not found");
                }
                else if (results.Count > 1)
                {
                    Assert.Fail("Multiple matches were found");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void GetMAObjectsByComplexOrSearchSingleMVtoMVMatchedAttribute3()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test.test@test.com", "test3.test3@test.com" };
            List<object> existingValues1 = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 7, 7, 7, 7 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 7, 7, 7, 7 }, new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            List<object> existingValues4 = new List<object>() { 44L, 55L };
            List<object> incomingValues4 = new List<object>() { 44L, 77L, 99L };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            AcmaSchemaAttribute incomingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaAttribute existingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");

            DBQueryByValue match1 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            DBQueryByValue match2 = new DBQueryByValue(existingAttribute2, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute2.Name)));
            DBQueryByValue match3 = new DBQueryByValue(existingAttribute3, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute3.Name)));
            DBQueryByValue match4 = new DBQueryByValue(existingAttribute4, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute4.Name)));


            DBQueryGroup group1 = new DBQueryGroup();
            group1.Operator = GroupOperator.All;
            group1.AddChildQueryObjects(match1, match2, match4);

            DBQueryGroup group3 = new DBQueryGroup();
            group3.Operator = GroupOperator.Any;
            group3.AddChildQueryObjects(match3, group1);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValues1);
                sourceObject.SetAttributeValue(existingAttribute2, existingValues2);
                sourceObject.SetAttributeValue(existingAttribute3, existingValues3);
                sourceObject.SetAttributeValue(existingAttribute4, existingValues4);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues1));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute2.Name, incomingValues2));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute3.Name, incomingValues3));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute4.Name, incomingValues4));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group3, csentry).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The match was not found");
                }
                else if (results.Count > 1)
                {
                    Assert.Fail("Multiple matches were found");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void GetMAObjectsByComplexOrSearchSingleMVtoMVMatchedAttribute4()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test5.test5@test.com", "test3.test3@test.com" };
            List<object> existingValues1 = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 0, 7, 7, 7, 7 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 7, 7, 7, 7 }, new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), guid, Guid.NewGuid() };

            List<object> existingValues4 = new List<object>() { 43L, 55L };
            List<object> incomingValues4 = new List<object>() { 44L, 77L, 99L };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            AcmaSchemaAttribute incomingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");
            AcmaSchemaAttribute existingAttribute4 = ActiveConfig.DB.GetAttribute("expiryDates");

            DBQueryByValue match1 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            DBQueryByValue match2 = new DBQueryByValue(existingAttribute2, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute2.Name)));
            DBQueryByValue match3 = new DBQueryByValue(existingAttribute3, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute3.Name)));
            DBQueryByValue match4 = new DBQueryByValue(existingAttribute4, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute4.Name)));

            DBQueryGroup group1 = new DBQueryGroup();
            group1.Operator = GroupOperator.Any;
            group1.AddChildQueryObjects(match1, match2, match4);

            DBQueryGroup group2 = new DBQueryGroup();
            group2.Operator = GroupOperator.All;
            group2.AddChildQueryObjects(match3, group1);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValues1);
                sourceObject.SetAttributeValue(existingAttribute2, existingValues2);
                sourceObject.SetAttributeValue(existingAttribute3, existingValues3);
                sourceObject.SetAttributeValue(existingAttribute4, existingValues4);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues1));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute2.Name, incomingValues2));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute3.Name, incomingValues3));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute4.Name, incomingValues4));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group2, csentry).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The match was not found");
                }
                else if (results.Count > 1)
                {
                    Assert.Fail("Multiple matches were found");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void GetMAObjectsByComplexSearchSingleMVtoSVMatchedAttribute()
        {
            UnitTestControl.DeleteAllMAObjects();

            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test.test@test.com", "test3.test3@test.com", "test2.test2@test.com", "test4.test4@test.com" };
            List<object> existingValues1 = new List<object>() { "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 7, 7, 7, 7 }, new byte[] { 2, 4, 6, 8 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mail");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            GetMAObjectsByComplexSearch(sourceId, objectClass, incomingValues1, existingValues1, incomingValues2, existingValues2, existingValues3, incomingValues3, incomingAttribute1, existingAttribute1, incomingAttribute2, existingAttribute2, incomingAttribute3, existingAttribute3);
        }

        [TestMethod()]
        public void GetMAObjectsByComplexSearchNoMatch()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            List<object> incomingValues1 = new List<object>() { "test9.test9@test.com", "test3.test3@test.com", "test2.test2@test.com", "test4.test4@test.com" };
            List<object> existingValues1 = new List<object>() { "test.test@test.com" };

            List<object> incomingValues2 = new List<object>() { new byte[] { 0, 1, 2, 3, 4 } };
            List<object> existingValues2 = new List<object>() { new byte[] { 0, 7, 7, 7, 7 }, new byte[] { 2, 4, 6, 8 }, new byte[] { 1, 3, 5, 7 } };

            Guid guid = Guid.NewGuid();
            List<object> existingValues3 = new List<object>() { guid };
            List<object> incomingValues3 = new List<object>() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mail");

            AcmaSchemaAttribute incomingAttribute2 = ActiveConfig.DB.GetAttribute("objectSid");
            AcmaSchemaAttribute existingAttribute2 = ActiveConfig.DB.GetAttribute("objectSids");

            AcmaSchemaAttribute incomingAttribute3 = ActiveConfig.DB.GetAttribute("directReports");
            AcmaSchemaAttribute existingAttribute3 = ActiveConfig.DB.GetAttribute("supervisor");

            DBQueryByValue match1 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            DBQueryByValue match2 = new DBQueryByValue(existingAttribute2, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute2.Name)));
            DBQueryByValue match3 = new DBQueryByValue(existingAttribute3, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute3.Name)));

            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.Any;
            group.DBQueries.Add(match1);
            group.DBQueries.Add(match2);
            group.DBQueries.Add(match3);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValues1);
                sourceObject.SetAttributeValue(existingAttribute2, existingValues2);
                sourceObject.SetAttributeValue(existingAttribute3, existingValues3);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues1));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute2.Name, incomingValues2));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute3.Name, incomingValues3));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, csentry).ToList();

                if (results.Count != 0)
                {
                    Assert.Fail("Matches were found");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void GetResurrectionObject()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            object existingValue = "test.test@test.com";
            List<object> incomingValues = new List<object>() { "test2.test2@test.com", "test4.test4@test.com", existingValue };
            AcmaSchemaAttribute incomingAttribute1 = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaAttribute existingAttribute1 = ActiveConfig.DB.GetAttribute("mail");

            DBQueryByValue match = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));

            DBQueryGroup matchGroup = new DBQueryGroup();
            matchGroup.DBQueries.Add(match);
            matchGroup.Operator = GroupOperator.All;

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValue);
                sourceObject.CommitCSEntryChange();
                sourceObject.SetObjectModificationType(ObjectModificationType.Delete, false);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectType = objectClass;
                csentry.ObjectModificationType = ObjectModificationType.Add;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues));

                MAObjectHologram result = UnitTestControl.DataContext.GetResurrectionObject(matchGroup, csentry);

                if (result == null)
                {
                    Assert.Fail("The match was not found");
                }
                else
                {
                    if (result.Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void DeltaAddTest()
        {
            Guid sourceId = Guid.NewGuid();
            string objectClass = "person";
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The delta table contained no records");
                }
                if (results.Count > 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }

                    if (results.First().DeltaChangeType != "add")
                    {
                        Assert.Fail("The wrong modification type was recorded on the object");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void DeltaDeleteTest()
        {
            Guid sourceId = Guid.NewGuid();
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                UnitTestControl.DataContext.ClearDeltas(watermark);


                sourceObject = UnitTestControl.DataContext.GetMAObject(sourceId, objectClass);
                sourceObject.SetObjectModificationType(ObjectModificationType.Delete, false);
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The delta table contained no records");
                }
                if (results.Count > 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }

                    if (results.First().DeltaChangeType != "delete")
                    {
                        Assert.Fail("The wrong modification type was recorded on the object");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void DeltaModifySVTest()
        {
            Guid sourceId = Guid.NewGuid();
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mail");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                UnitTestControl.DataContext.ClearDeltas(watermark);

                sourceObject = UnitTestControl.DataContext.GetMAObject(sourceId, objectClass);
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(attribute, "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The delta table contained no records");
                }
                if (results.Count > 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }

                    if (results.First().DeltaChangeType != "modify")
                    {
                        Assert.Fail("The wrong modification type was recorded on the object");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void DeltaModifyMVTest()
        {
            Guid sourceId = Guid.NewGuid();

            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                UnitTestControl.DataContext.ClearDeltas(watermark);

                sourceObject = UnitTestControl.DataContext.GetMAObject(sourceId, objectClass);
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(attribute, "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The delta table contained no records");
                }
                if (results.Count > 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }

                    if (results.First().DeltaChangeType != "modify")
                    {
                        Assert.Fail("The wrong modification type was recorded on the object");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void DeltaDeleteMVTest()
        {
            Guid sourceId = Guid.NewGuid();

            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                UnitTestControl.DataContext.ClearDeltas(watermark);

                sourceObject = UnitTestControl.DataContext.GetMAObject(sourceId, objectClass);
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(attribute, null);
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The delta table contained no records");
                }
                if (results.Count > 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }

                    if (results.First().DeltaChangeType != "modify")
                    {
                        Assert.Fail("The wrong modification type was recorded on the object");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void DeltaAddMVTest()
        {
            Guid sourceId = Guid.NewGuid();

            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The delta table contained no records");
                }
                if (results.Count > 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }

                    if (results.First().DeltaChangeType != "add")
                    {
                        Assert.Fail("The wrong modification type was recorded on the object");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }


        [TestMethod()]
        public void DeltaAddDeleteTest()
        {
            Guid sourceId = Guid.NewGuid();
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                sourceObject = UnitTestControl.DataContext.GetMAObject(sourceId, objectClass);
                sourceObject.SetObjectModificationType(ObjectModificationType.Delete, false);
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count >= 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void DeltaAddModifyTest()
        {
            Guid sourceId = Guid.NewGuid();
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                sourceObject = UnitTestControl.DataContext.GetMAObject(sourceId, objectClass);
                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(attribute, "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The delta table contained no records");
                }
                if (results.Count > 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }

                    if (results.First().DeltaChangeType != "add")
                    {
                        Assert.Fail("The wrong modification type was recorded on the object");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void DeltaModifyDeleteTest()
        {
            Guid sourceId = Guid.NewGuid();
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                UnitTestControl.DataContext.ClearDeltas(watermark);

                sourceObject.SetObjectModificationType(ObjectModificationType.Update, false);
                sourceObject.SetAttributeValue(attribute, "test1.test1@test.com");
                sourceObject.CommitCSEntryChange();

                sourceObject = UnitTestControl.DataContext.GetMAObject(sourceId, objectClass);
                sourceObject.SetObjectModificationType(ObjectModificationType.Delete, false);
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The delta table contained no records");
                }
                if (results.Count > 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }

                    if (results.First().DeltaChangeType != "delete")
                    {
                        Assert.Fail("The wrong modification type was recorded on the object");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        [TestMethod()]
        public void DeltaDeleteAddTest()
        {
            Guid sourceId = Guid.NewGuid();
            AcmaSchemaObjectClass objectClass = ActiveConfig.DB.GetObjectClass("person");

            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");

            try
            {
                byte[] watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();

                UnitTestControl.DataContext.ClearDeltas(watermark);

                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(attribute, "test.test@test.com");
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                UnitTestControl.DataContext.ClearDeltas(watermark);

                sourceObject.SetObjectModificationType(ObjectModificationType.Delete, false);
                sourceObject.CommitCSEntryChange();

                sourceObject = UnitTestControl.DataContext.GetMAObject(sourceId, objectClass);
                sourceObject.SetObjectModificationType(ObjectModificationType.Add, false);
                sourceObject.DeletedTimestamp = 0;
                sourceObject.CommitCSEntryChange();

                watermark = UnitTestControl.DataContext.GetHighWatermarkMAObjectsDelta();
                List<MAObjectHologram> results = UnitTestControl.DataContext.GetDeltaMAObjects(watermark).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The delta table contained no records");
                }
                if (results.Count > 1)
                {
                    Assert.Fail("The delta table contained more than the expected number of records");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }

                    if (results.First().DeltaChangeType != "modify")
                    {
                        Assert.Fail("The wrong modification type was recorded on the object");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        private static void GetMAObjectsByComplexSearch(Guid sourceId, string objectClass, List<object> incomingValues1, List<object> existingValues1, List<object> incomingValues2, List<object> existingValues2, List<object> existingValues3, List<object> incomingValues3, AcmaSchemaAttribute incomingAttribute1, AcmaSchemaAttribute existingAttribute1, AcmaSchemaAttribute incomingAttribute2, AcmaSchemaAttribute existingAttribute2, AcmaSchemaAttribute incomingAttribute3, AcmaSchemaAttribute existingAttribute3)
        {
            UnitTestControl.DeleteAllMAObjects();

            DBQueryByValue match1 = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            DBQueryByValue match2 = new DBQueryByValue(existingAttribute2, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute2.Name)));
            DBQueryByValue match3 = new DBQueryByValue(existingAttribute3, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute3.Name)));

            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.Any;
            group.AddChildQueryObjects(match1, match2, match3);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValues1);
                sourceObject.SetAttributeValue(existingAttribute2, existingValues2);
                sourceObject.SetAttributeValue(existingAttribute3, existingValues3);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues1));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute2.Name, incomingValues2));
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute3.Name, incomingValues3));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, csentry).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The match was not found");
                }
                else if (results.Count > 1)
                {
                    Assert.Fail("Multiple matches were found");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        private static void GetMAObjectsByMVtoSVMatch(Guid sourceId, string objectClass, IList<object> incomingValues, object existingValue, AcmaSchemaAttribute incomingAttribute1, AcmaSchemaAttribute existingAttribute1)
        {
            UnitTestControl.DeleteAllMAObjects();

            DBQueryByValue match = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.Any;
            group.AddChildQueryObjects(match);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValue);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, csentry).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The match was not found");
                }
                else if (results.Count > 1)
                {
                    Assert.Fail("Multiple matches were found");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        private static void GetMAObjectsByMVtoMVMatch(Guid sourceId, string objectClass, IList<object> incomingValues, IList<object> existingValues, AcmaSchemaAttribute incomingAttribute1, AcmaSchemaAttribute existingAttribute1)
        {
            UnitTestControl.DeleteAllMAObjects();

            DBQueryByValue match = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            List<DBQueryByValue> matches = new List<DBQueryByValue>() { match };
            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.Any;
            group.AddChildQueryObjects(match);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValues);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValues));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, csentry).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The match was not found");
                }
                else if (results.Count > 1)
                {
                    Assert.Fail("Multiple matches were found");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        private static void GetMAObjectsBySVtoMVMatch(Guid sourceId, string objectClass, object incomingValue, IList<object> existingValues, AcmaSchemaAttribute incomingAttribute1, AcmaSchemaAttribute existingAttribute1)
        {
            UnitTestControl.DeleteAllMAObjects();

            DBQueryByValue match = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            List<DBQueryByValue> matches = new List<DBQueryByValue>() { match };
            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.Any;
            group.AddChildQueryObjects(match);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, existingValues);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, incomingValue));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, csentry).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The match was not found");
                }
                else if (results.Count > 1)
                {
                    Assert.Fail("Multiple matches were found");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }

        private static void GetMAObjectsBySVtoSVMatch(Guid sourceId, string objectClass, object matchValue, AcmaSchemaAttribute incomingAttribute1, AcmaSchemaAttribute existingAttribute1)
        {
            UnitTestControl.DeleteAllMAObjects();

            DBQueryByValue match = new DBQueryByValue(existingAttribute1, ValueOperator.Equals, new ValueDeclaration(string.Format("{{{0}}}", incomingAttribute1.Name)));
            List<DBQueryByValue> matches = new List<DBQueryByValue>() { match };
            DBQueryGroup group = new DBQueryGroup();
            group.Operator = GroupOperator.Any;
            group.AddChildQueryObjects(match);

            try
            {
                MAObjectHologram sourceObject = UnitTestControl.DataContext.CreateMAObject(sourceId, objectClass);
                                sourceObject.SetAttributeValue(existingAttribute1, matchValue);
                sourceObject.CommitCSEntryChange();

                CSEntryChange csentry = CSEntryChange.Create();
                csentry.DN = Guid.NewGuid().ToString();
                csentry.ObjectModificationType = ObjectModificationType.Update;
                csentry.AttributeChanges.Add(AttributeChange.CreateAttributeAdd(incomingAttribute1.Name, matchValue));

                List<MAObjectHologram> results = UnitTestControl.DataContext.GetMAObjectsFromDBQuery(group, csentry).ToList();

                if (results.Count == 0)
                {
                    Assert.Fail("The match was not found");
                }
                else if (results.Count > 1)
                {
                    Assert.Fail("Multiple matches were found");
                }
                else
                {
                    if (results.First().Id != sourceId)
                    {
                        Assert.Fail("The wrong object was returned by the search");
                    }
                }
            }
            finally
            {
                UnitTestControl.DataContext.DeleteMAObjectPermanent(sourceId);
            }
        }
    }
}