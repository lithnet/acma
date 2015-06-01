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
    public class RegexReplaceTransformTest
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
            RegexReplaceTransform transformToSeralize = new RegexReplaceTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.FindPattern = "zz";
            transformToSeralize.ReplacePattern = "aa";
            UniqueIDCache.ClearIdCache();

            RegexReplaceTransform deserializedTransform = (RegexReplaceTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.FindPattern, deserializedTransform.FindPattern);
            Assert.AreEqual(transformToSeralize.ReplacePattern, deserializedTransform.ReplacePattern);
        }

        [TestMethod()]
        public void RegexReplaceTransformNonAlphaRemove()
        {
            RegexReplaceTransform transform = new RegexReplaceTransform();
            transform.FindPattern = @"[^a-zA-Z\-]";
            transform.ReplacePattern = string.Empty;

            this.ExecuteTestSubString(transform, "Ry^*an", "Ryan");
        }

        private void ExecuteTestSubString(RegexReplaceTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
