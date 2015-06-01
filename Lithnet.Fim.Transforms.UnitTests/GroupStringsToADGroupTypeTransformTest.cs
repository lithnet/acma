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
    public class GroupStringsToADGroupTypeTransformTest
    {
        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            GroupStringsToADGroupTypeTransform transformToSeralize = new GroupStringsToADGroupTypeTransform();
            transformToSeralize.ID = "test001";
            UniqueIDCache.ClearIdCache();

            GroupStringsToADGroupTypeTransform deserializedTransform = (GroupStringsToADGroupTypeTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSingleValueSecurity()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, "Security", -2147483646);
        }

        [TestMethod()]
        public void TestSingleValueDistribution()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, "Distribution", 2);
        }

        [TestMethod()]
        public void TestSingleValueGlobal()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, "Global", 2);
        }

        [TestMethod()]
        public void TestSingleValueDomainLocal()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, "DomainLocal", 4);
        }

        [TestMethod()]
        public void TestSingleValueUniversal()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, "Universal", 8);
        }

        [TestMethod()]
        public void TestDistributionGlobal()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, new List<object>() { "Distribution", "Global"}, 2);
        }

        [TestMethod()]
        public void TestDistributionDomainLocal()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, new List<object>() { "Distribution", "DomainLocal" }, 4);
        }

        [TestMethod()]
        public void TestDistributionUniversal()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, new List<object>() { "Distribution", "Universal" }, 8);
        }


        [TestMethod()]
        public void TestSecurityGlobal()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, new List<object>() { "Security", "Global" }, -2147483646);
        }

        [TestMethod()]
        public void TestSecurityDomainLocal()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, new List<object>() { "Security", "DomainLocal" }, -2147483644);
        }

        [TestMethod()]
        public void TestSecurityUniversal()
        {
            GroupStringsToADGroupTypeTransform transform = new GroupStringsToADGroupTypeTransform();
            this.ExecuteTest(transform, new List<object>() { "Security", "Universal" }, -2147483640);
        }

        private void ExecuteTest(GroupStringsToADGroupTypeTransform transform, string sourceValue, long expectedValue)
        {
            long outValue = (long)(transform.TransformValue(sourceValue)).First();

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTest(GroupStringsToADGroupTypeTransform transform, IList<object> sourceValues, long expectedValue)
        {
            long outValue = (long)(transform.TransformValue(sourceValues).FirstOrDefault());
            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
