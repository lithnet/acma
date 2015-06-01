using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Linq;

namespace Lithnet.Fim.Transforms.UnitTests
{
    /// <summary>
    ///This is a test class for RegexReplaceTransformTest and is intended
    ///to contain all RegexReplaceTransformTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FormatNumberTransformTest
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
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            FormatNumberTransform transformToSeralize = new FormatNumberTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.Format = "d";
            UniqueIDCache.ClearIdCache();

            FormatNumberTransform deserializedTransform = (FormatNumberTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.Format, deserializedTransform.Format);
        }

        [TestMethod()]
        public void FormatNumberTransformAsN()
        {
            FormatNumberTransform transform = new FormatNumberTransform();
            transform.Format = "n";

            this.ExecuteTestFormatNumber(transform, 12345, "12,345.00");
        }

        [TestMethod()]
        public void ExecuteTestFormatNumberAsHex()
        {
            FormatNumberTransform transform = new FormatNumberTransform();
            transform.Format = "X8";
            this.ExecuteTestFormatNumber(transform, 255, "000000FF");
        }

        [TestMethod()]
        public void ExecuteTestFormatNumberAsPercent()
        {
            FormatNumberTransform transform = new FormatNumberTransform();

            transform.Format = "P";
            this.ExecuteTestFormatNumber(transform, 1, "100.00 %");
        }

        private void ExecuteTestFormatNumber(FormatNumberTransform transform, long sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
