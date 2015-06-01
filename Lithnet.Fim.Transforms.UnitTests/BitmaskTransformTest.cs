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
    [TestClass()]
    public class BitmaskTransformTest
    {
        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            BitmaskTransform transformToSeralize = new BitmaskTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.Flag = 2;
            transformToSeralize.Operation = BitwiseOperation.And;
            UniqueIDCache.ClearIdCache();

            BitmaskTransform deserializedTransform = (BitmaskTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.Flag, deserializedTransform.Flag);
            Assert.AreEqual(transformToSeralize.Operation, deserializedTransform.Operation);
        }

        [TestMethod()]
        public void BitmaskPrimaryInputTestAnd()
        {
            BitmaskTransform transform = new BitmaskTransform();
            transform.Flag = 512;
            transform.Operation = BitwiseOperation.And;

            this.ExecuteTestBitwiseTransformPrimaryInput(transform, 512, 512);
        }

        [TestMethod()]
        public void BitmaskPrimaryInputTestOr()
        {
            BitmaskTransform transform = new BitmaskTransform();
            transform.Flag = 2;
            transform.Operation = BitwiseOperation.Or;

            this.ExecuteTestBitwiseTransformPrimaryInput(transform, 512, 514);
        }

        [TestMethod()]
        public void BitmaskPrimaryInputTestNand()
        {
            BitmaskTransform transform = new BitmaskTransform();
            transform.Flag = 2;
            transform.Operation = BitwiseOperation.Nand;

            this.ExecuteTestBitwiseTransformPrimaryInput(transform, 514, 512);
            this.ExecuteTestBitwiseTransformPrimaryInput(transform, 512, 512);
        }

        [TestMethod()]
        public void BitmaskPrimaryInputTestXor()
        {
            BitmaskTransform transform = new BitmaskTransform();
            transform.Flag = 2;
            transform.Operation = BitwiseOperation.Xor;

            this.ExecuteTestBitwiseTransformPrimaryInput(transform, 514, 512);
            this.ExecuteTestBitwiseTransformPrimaryInput(transform, 512, 514);
        }
        
        private void ExecuteTestBitwiseTransformPrimaryInput(BitmaskTransform transform, long sourceValue, long expectedValue)
        {
            long outValue = (long)transform.TransformValue(sourceValue).FirstOrDefault();
            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
