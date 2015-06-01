using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Xml;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using System.Collections.Generic;
using System.Security.Principal;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.UnitTests
{
    [TestClass()]
    public class SidStringBiDirectionalTransformTest
    {
        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            SidStringBiDirectionalTransform transformToSeralize = new SidStringBiDirectionalTransform();
            transformToSeralize.ID = "test001";
            UniqueIDCache.ClearIdCache();

            SidStringBiDirectionalTransform deserializedTransform = (SidStringBiDirectionalTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void SidStringBiDirectionalTransformTestStringInput()
        {
            SidStringBiDirectionalTransform transform = new SidStringBiDirectionalTransform();
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;

            byte[] sidbytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidbytes, 0);

            this.ExecuteTest(transform, sid.Value, sidbytes);
        }

        [TestMethod()]
        public void SidStringBiDirectionalTransformTestBase64StringInput()
        {
            SidStringBiDirectionalTransform transform = new SidStringBiDirectionalTransform();
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;

            byte[] sidbytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidbytes, 0);
            string sidBase64 = Convert.ToBase64String(sidbytes);

            this.ExecuteTest(transform, sidBase64, sid.Value);
        }

        [TestMethod()]
        public void SidStringBiDirectionalTransformTestBinaryInput()
        {
            SidStringBiDirectionalTransform transform = new SidStringBiDirectionalTransform();
            SecurityIdentifier sid = WindowsIdentity.GetCurrent().User;

            byte[] sidbytes = new byte[sid.BinaryLength];
            sid.GetBinaryForm(sidbytes, 0);
             
            this.ExecuteTest(transform, sidbytes, sid.Value);
        }

        private void ExecuteTest(SidStringBiDirectionalTransform transform, string sourceValue, byte[] expectedValue)
        {
            byte[] outValue = transform.TransformValue(sourceValue).FirstOrDefault() as byte[];

            CollectionAssert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTest(SidStringBiDirectionalTransform transform, byte[] sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTest(SidStringBiDirectionalTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

    }
}
