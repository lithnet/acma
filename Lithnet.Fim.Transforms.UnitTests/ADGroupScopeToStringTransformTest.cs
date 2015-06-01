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
    public class ADGroupScopeToStringTransformTest
    {
        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            ADGroupScopeToStringTransform transformToSeralize = new ADGroupScopeToStringTransform();
            transformToSeralize.ID = "test001";
            UniqueIDCache.ClearIdCache();

            ADGroupScopeToStringTransform deserializedTransform = (ADGroupScopeToStringTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

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
            ADGroupScopeToStringTransform transform = new ADGroupScopeToStringTransform();
            this.ExecuteTest(transform, -2147483646, "Global");
        }

        [TestMethod()]
        public void TestSecurityDomainLocal()
        {
            ADGroupScopeToStringTransform transform = new ADGroupScopeToStringTransform();
            this.ExecuteTest(transform, -2147483644, "DomainLocal");
        }

        [TestMethod()]
        public void TestSecurityUniversal()
        {
            ADGroupScopeToStringTransform transform = new ADGroupScopeToStringTransform();
            this.ExecuteTest(transform, -2147483640, "Universal");
        }

        [TestMethod()]
        public void TestDistributionGlobal()
        {
            ADGroupScopeToStringTransform transform = new ADGroupScopeToStringTransform();
            this.ExecuteTest(transform, 2, "Global");
        }

        [TestMethod()]
        public void TestDistributionDomainLocal()
        {
            ADGroupScopeToStringTransform transform = new ADGroupScopeToStringTransform();
            this.ExecuteTest(transform, 4, "DomainLocal");
        }

        [TestMethod()]
        public void TestDistributionUniversal()
        {
            ADGroupScopeToStringTransform transform = new ADGroupScopeToStringTransform();
            this.ExecuteTest(transform, 8, "Universal");
        }

        private void ExecuteTest(ADGroupScopeToStringTransform transform, long sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
