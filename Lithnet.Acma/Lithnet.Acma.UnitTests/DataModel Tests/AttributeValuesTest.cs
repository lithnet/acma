
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Lithnet.Acma.DataModel;


namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for AttributeValuesTest and is intended
    ///to contain all AttributeValuesTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AttributeValuesTest
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
        public void SequenceEqualsStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            List<object> values = new List<object> { "test.test@test.com", "test1.test1@test.com", "test3.test3@test.com" };
            List<object> matchingValues = new List<object> { "test.test@test.com", "test1.test1@test.com", "test3.test3@test.com" };
            List<object> nonMatchingValues = new List<object> { "test.test@test.com", "test2.test2@test.com", "test3.test3@test.com" };

            SequenceEqualsTest(attribute, values, matchingValues, nonMatchingValues);

        }

        [TestMethod()]
        public void SequenceEqualsLongTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            List<object> values = new List<object> { 11L, 22L, 33L };
            List<object> matchingValues = new List<object> { 11L, 22L, 33L };
            List<object> nonMatchingValues = new List<object> { 11L, 22L, 99L };

            SequenceEqualsTest(attribute, values, matchingValues, nonMatchingValues);

        }

        [TestMethod()]
        public void SequenceEqualsBinaryTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSids");
            List<object> values = new List<object> { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 2, 3, 4, 5, 6 } };
            List<object> matchingValues = new List<object> { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 2, 3, 4, 5, 6 } };
            List<object> nonMatchingValues = new List<object> { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 9, 3, 4, 5, 9 } };

            SequenceEqualsTest(attribute, values, matchingValues, nonMatchingValues);
        }

        [TestMethod()]
        public void SequenceEqualsGuidTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("directReports");
            List<object> values = new List<object> { new Guid("36ab20a6-51f8-4a26-886d-bab4153579ee"), new Guid("c3593547-f37e-4cbf-b0f8-2c9c40107070"), new Guid("a6cf8e39-1b18-4695-8bc1-68b31b040d83") };
            List<object> matchingValues = new List<object> { new Guid("36ab20a6-51f8-4a26-886d-bab4153579ee"), new Guid("c3593547-f37e-4cbf-b0f8-2c9c40107070"), new Guid("a6cf8e39-1b18-4695-8bc1-68b31b040d83") };
            List<object> nonMatchingValues = new List<object> { new Guid("36ab20a6-51f8-4a26-886d-bab4153579ee"), new Guid("a6cf8e39-1b18-4695-8bc1-68b31b040d83") };

            SequenceEqualsTest(attribute, values, matchingValues, nonMatchingValues);
        }

        [TestMethod()]
        public void HasValueStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            List<object> testData = new List<object>()  { "test.test@test.com", "test1.test1@test.com", "test3.test3@test.com" };
            object matchValue = "test1.test1@test.com";
            object noMatchValue ="test9.test9@test.com";

            HasValueTest(attribute, testData, matchValue, noMatchValue);
        }

        [TestMethod()]
        public void HasValueLongTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            List<object> testData = new List<object>()  { 11L, 22L, 33L };
            object matchValue = 22L;
            object noMatchValue = 99L;

            HasValueTest(attribute, testData, matchValue, noMatchValue);
        }

        [TestMethod()]
        public void HasValueBinaryTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSids");
            List<object> testData = new List<object>() { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 2, 3, 4, 5, 6 } };
            object matchValue =  new byte[] { 1, 2, 3, 4, 5 };
            object noMatchValue = new byte[] { 9, 8, 7, 6, 5 };

            HasValueTest(attribute, testData, matchValue, noMatchValue);
        }

        [TestMethod()]
        public void HasValueGuidTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("directReports");
            List<object> testData = new List<object>() { new Guid("36ab20a6-51f8-4a26-886d-bab4153579ee"), new Guid("c3593547-f37e-4cbf-b0f8-2c9c40107070"), new Guid("a6cf8e39-1b18-4695-8bc1-68b31b040d83") };
            object matchValue = new Guid("c3593547-f37e-4cbf-b0f8-2c9c40107070");
            object noMatchValue = new Guid("ccccc547-f37e-4cbf-b0f8-2c9c40100000");

            HasValueTest(attribute, testData, matchValue, noMatchValue);
        }

        [TestMethod()]
        public void ContainsAllElementsStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("mailAlternateAddresses");
            List<object> values = new List<object> { "test.test@test.com", "test1.test1@test.com", "test3.test3@test.com" };
            List<object> matchingValues = new List<object> { "test.test@test.com", "test1.test1@test.com", "test3.test3@test.com" };
            List<object> nonMatchingValues = new List<object> { "test.test@test.com", "test2.test2@test.com", "test3.test3@test.com" };

            ContainsAllElementsTest(attribute, values, matchingValues, nonMatchingValues);

        }

        [TestMethod()]
        public void ContainsAllElementsLongTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("expiryDates");
            List<object> values = new List<object> { 11L, 22L, 33L };
            List<object> matchingValues = new List<object> { 11L, 22L, 33L };
            List<object> nonMatchingValues = new List<object> { 11L, 22L, 99L };

            ContainsAllElementsTest(attribute, values, matchingValues, nonMatchingValues);

        }

        [TestMethod()]
        public void ContainsAllElementsBinaryTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSids");
            List<object> values = new List<object> { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 2, 3, 4, 5, 6 } };
            List<object> matchingValues = new List<object> { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 2, 3, 4, 5, 6 } };
            List<object> nonMatchingValues = new List<object> { new byte[] { 0, 1, 2, 3, 4 }, new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 9, 3, 4, 5, 9 } };

            ContainsAllElementsTest(attribute, values, matchingValues, nonMatchingValues);
        }

        [TestMethod()]
        public void ContainsAllElementsGuidTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("directReports");
            List<object> values = new List<object> { new Guid("36ab20a6-51f8-4a26-886d-bab4153579ee"), new Guid("c3593547-f37e-4cbf-b0f8-2c9c40107070"), new Guid("a6cf8e39-1b18-4695-8bc1-68b31b040d83") };
            List<object> matchingValues = new List<object> { new Guid("36ab20a6-51f8-4a26-886d-bab4153579ee"), new Guid("c3593547-f37e-4cbf-b0f8-2c9c40107070"), new Guid("a6cf8e39-1b18-4695-8bc1-68b31b040d83") };
            List<object> nonMatchingValues = new List<object> { new Guid("36ab20a6-51f8-4a26-886d-bab4153579ee"), new Guid("a6cf8e39-1b18-4695-8bc1-68b31b040d83") };

            ContainsAllElementsTest(attribute, values, matchingValues, nonMatchingValues);
        }

        private static void HasValueTest(AcmaSchemaAttribute attribute, IList<object> values, object matchValue, object noMatchValue)
        {
            AttributeValues target = new InternalAttributeValues(attribute, values);

            Assert.AreEqual(true, target.HasValue(matchValue));
            Assert.AreEqual(false, target.HasValue(noMatchValue));
        }

        private static void ContainsAllElementsTest(AcmaSchemaAttribute attribute, IList<object> values, IList<object> matchingValues, IList<object> nonMatchingValues)
        {
            AttributeValues target = new InternalAttributeValues(attribute, values);

            Assert.AreEqual(true, target.ContainsAllElements(matchingValues));
            Assert.AreEqual(false, target.ContainsAllElements(nonMatchingValues));
        }

        private static void SequenceEqualsTest(AcmaSchemaAttribute attribute, IList<object> values, IList<object> matchingValues, IList<object> nonMatchingValues)
        {
            AttributeValues target = new InternalAttributeValues(attribute, values);

            Assert.AreEqual(true, target.SequenceEquals(matchingValues));
            Assert.AreEqual(false, target.SequenceEquals(nonMatchingValues));
        }
    }
}
