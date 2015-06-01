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
    [TestClass()]
    public class ADGroupTypeToStringTransformTest
    {
        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            ADGroupTypeToStringTransform transformToSeralize = new ADGroupTypeToStringTransform();
            transformToSeralize.ID = "test001";
            UniqueIDCache.ClearIdCache();

            ADGroupTypeToStringTransform deserializedTransform = (ADGroupTypeToStringTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSecurityGlobal()
        {
            ADGroupTypeToStringTransform transform = new ADGroupTypeToStringTransform();
            this.ExecuteTest(transform, -2147483646, "Security");
        }

        [TestMethod()]
        public void TestSecurityDomainLocal()
        {
            ADGroupTypeToStringTransform transform = new ADGroupTypeToStringTransform();
            this.ExecuteTest(transform, -2147483644, "Security");
        }

        [TestMethod()]
        public void TestSecurityUniversal()
        {
            ADGroupTypeToStringTransform transform = new ADGroupTypeToStringTransform();
            this.ExecuteTest(transform, -2147483640, "Security");
        }

        [TestMethod()]
        public void TestDistributionGlobal()
        {
            ADGroupTypeToStringTransform transform = new ADGroupTypeToStringTransform();
            this.ExecuteTest(transform, 2, "Distribution");
        }

        [TestMethod()]
        public void TestDistributionDomainLocal()
        {
            ADGroupTypeToStringTransform transform = new ADGroupTypeToStringTransform();
            this.ExecuteTest(transform, 4, "Distribution");
        }

        [TestMethod()]
        public void TestDistributionUniversal()
        {
            ADGroupTypeToStringTransform transform = new ADGroupTypeToStringTransform();
            this.ExecuteTest(transform, 8, "Distribution");
        }

        private void ExecuteTest(ADGroupTypeToStringTransform transform, long sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
