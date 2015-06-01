using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using System.Collections.Generic;
using System.Linq;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.UnitTests
{
    [TestClass()]
    public class MultivalueToSingleValueTransformTest
    {
        private TestContext testContextInstance;
       
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
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            MultivalueToSingleValueTransform transformToSeralize = new MultivalueToSingleValueTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.SelectorOperator = ValueOperator.Equals;
            transformToSeralize.SelectorValue = "test";
            transformToSeralize.CompareAs = ExtendedAttributeType.DateTime;
            UniqueIDCache.ClearIdCache();
            
            MultivalueToSingleValueTransform deserializedTransform = (MultivalueToSingleValueTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.SelectorOperator, deserializedTransform.SelectorOperator);
            Assert.AreEqual(transformToSeralize.SelectorValue, deserializedTransform.SelectorValue);
            Assert.AreEqual(transformToSeralize.CompareAs, deserializedTransform.CompareAs);
        }

        [TestMethod()]
        public void MVToSVEqualsTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.Equals;
            transform.SelectorValue = "Ryan";
            transform.CompareAs = ExtendedAttributeType.String;

            this.ExecuteTestString(
                transform,
                new List<object>() { "Bob", "Ryan" },
                "Ryan");
        }

        [TestMethod()]
        public void MVToSVNotEqualsTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.NotEquals;
            transform.SelectorValue = "Ryan";
            transform.CompareAs = ExtendedAttributeType.String;

            this.ExecuteTestString(
                transform,
                new List<object>() { "Bob", "Ryan" },
                "Bob");
        }

        [TestMethod()]
        public void MVToSVContainsTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.Contains;
            transform.SelectorValue = "ya";
            transform.CompareAs = ExtendedAttributeType.String;

            this.ExecuteTestString(
                transform,
                new List<object>() { "Bob", "Ryan" },
                "Ryan");
        }

        [TestMethod()]
        public void MVToSVNotContainsTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.NotContains;
            transform.SelectorValue = "ya";
            transform.CompareAs = ExtendedAttributeType.String;

            this.ExecuteTestString(
                transform,
                new List<object>() { "Bob", "Ryan" },
                "Bob");
        }

        [TestMethod()]
        public void MVToSVStartsWithTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.StartsWith;
            transform.SelectorValue = "Ry";
            transform.CompareAs = ExtendedAttributeType.String;

            this.ExecuteTestString(
                transform,
                new List<object>() { "Bob", "Ryan" },
                "Ryan");
        }

        [TestMethod()]
        public void MVToSVEndsWithTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.EndsWith;
            transform.SelectorValue = "an";
            transform.CompareAs = ExtendedAttributeType.String;

            this.ExecuteTestString(
                transform,
                new List<object>() { "Bob", "Ryan" },
                "Ryan");
        }

        [TestMethod()]
        public void MVToSVFirstTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.First;
            transform.CompareAs = ExtendedAttributeType.String;

            this.ExecuteTestString(
                transform,
                new List<object>() { "Bob", "Ryan" },
                "Bob");
        }

        [TestMethod()]
        public void MVToSVLastTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.Last;
            transform.CompareAs = ExtendedAttributeType.String;

            this.ExecuteTestString(
                transform,
                new List<object>() { "Bob", "Ryan" },
                "Ryan");
        }

        [TestMethod()]
        public void MVToSVGreaterThanTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.GreaterThan;
            transform.SelectorValue = "5";
            transform.CompareAs = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(
                transform,
                new List<object>() { 4L, 9L },
                9L);
        }

        [TestMethod()]
        public void MVToSVGreaterThanOrEqTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.GreaterThanOrEq;
            transform.SelectorValue = "9";
            transform.CompareAs = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(
                transform,
                new List<object>() { 4L, 9L },
                9L);
        }

        [TestMethod()]
        public void MVToSVLessThanTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.LessThan;
            transform.SelectorValue = "5";
            transform.CompareAs = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(
                transform,
                new List<object>() { 4L, 9L },
                4L);
        }

        [TestMethod()]
        public void MVToSVLessThanOrEqTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.LessThanOrEq;
            transform.SelectorValue = "4";
            transform.CompareAs = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(
                transform,
                new List<object>() { 4L, 9L },
                4L);
        }

        [TestMethod()]
        public void MVToSVLargestTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.Largest;
            transform.CompareAs = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(
                transform,
                new List<object>() { 4L, 9L },
               9L);
        }

        [TestMethod()]
        public void MVToSVSmallestTest()
        {
            MultivalueToSingleValueTransform transform = new MultivalueToSingleValueTransform();
            transform.SelectorOperator = ValueOperator.Smallest;
            transform.CompareAs = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(
                transform,
                new List<object>() { 4L, 9L },
               4L);
        }

        private void ExecuteTestString(MultivalueToSingleValueTransform transform, IList<object> sourceValues, string expectedValue)
        {
            IEnumerable<object> outValues = transform.TransformValue(sourceValues);

            if (outValues.Count() > 1)
            {
                Assert.Fail("Multiple objects were returned");
            }

            Assert.AreEqual(expectedValue, (string)outValues.First());
        }

        private void ExecuteTestInteger(MultivalueToSingleValueTransform transform, IList<object> sourceValues, long expectedValue)
        {
            IEnumerable<object> outValues = transform.TransformValue(sourceValues);

            if (outValues.Count() > 1)
            {
                Assert.Fail("Multiple objects were returned");
            }

            Assert.AreEqual(expectedValue, (long)outValues.First());
        }

        private void ExecuteTestBinary(MultivalueToSingleValueTransform transform, IList<object> sourceValues, byte[] expectedValue)
        {
            IEnumerable<object> outValues = transform.TransformValue(sourceValues);

            if (outValues.Count() > 1)
            {
                Assert.Fail("Multiple objects were returned");
            }

            CollectionAssert.AreEqual(expectedValue, (byte[])outValues.First());
        }
    }
}
