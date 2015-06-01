using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using System.Collections.Generic;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.UnitTests
{
    /// <summary>
    ///This is a test class for StringCaseTransformTest and is intended to contain all StringCaseTransformTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ConcatStringTransformTest
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

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            ConcatStringTransform transformToSeralize = new ConcatStringTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.Delimiter = ",";
            UniqueIDCache.ClearIdCache();

            ConcatStringTransform deserializedTransform = (ConcatStringTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.Delimiter, deserializedTransform.Delimiter);
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void ConcatStringTransformTestSingleValue()
        {
            ConcatStringTransform transform = new ConcatStringTransform();
            transform.Delimiter = ",";

            this.ExecuteTest(transform, "Test", "Test");
        }

        [TestMethod()]
        public void ConcatStringTransformTestMultivalue()
        {
            ConcatStringTransform transform = new ConcatStringTransform();
            transform.Delimiter = ",";

            this.ExecuteTest(transform, new List<object>() { "test1", "test2", "test3" }, "test1,test2,test3");
        }

        [TestMethod()]
        public void ConcatStringTransformTestMultivalueInteger()
        {
            ConcatStringTransform transform = new ConcatStringTransform();
            transform.Delimiter = ",";

            this.ExecuteTest(transform, new List<object>() { 1L, 2L, 3L }, "1,2,3");
        }

        private void ExecuteTest(ConcatStringTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTest(ConcatStringTransform transform, IList<object> sourceValues, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValues).FirstOrDefault() as string;
            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
