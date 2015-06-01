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
    public class FormatStringTransformTest
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
            FormatStringTransform transformToSeralize = new FormatStringTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.Format = "{0}\n{1}";
            UniqueIDCache.ClearIdCache();

            FormatStringTransform deserializedTransform = (FormatStringTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.Format, deserializedTransform.Format);
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void FormatStringTransformTestSingleValue()
        {
            FormatStringTransform transform = new FormatStringTransform();
            transform.Format = "Value is: {0}";

            this.ExecuteTest(transform, "Test", "Value is: Test");
        }

        [TestMethod()]
        public void FormatStringTransformTestMultivalue()
        {
            FormatStringTransform transform = new FormatStringTransform();
            transform.Format = "Values are: {0}, {1}, {2}";


            this.ExecuteTest(transform, new List<object>() { "test1", "test2", "test3" }, "Values are: test1, test2, test3");
        }

        [TestMethod()]
        public void FormatStringTransformTestMultivalueWithNull()
        {
            FormatStringTransform transform = new FormatStringTransform();
            transform.Format = "Values are: {0}, {1}, {2}";


            this.ExecuteTest(transform, new List<object>() { "test1", null, "test3" }, "Values are: test1, , test3");
        }

        private void ExecuteTest(FormatStringTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTest(FormatStringTransform transform, IList<object> sourceValues, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValues).FirstOrDefault() as string;
            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
