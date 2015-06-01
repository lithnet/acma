//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Xml;
//using Lithnet.Fim.Core;
//using Lithnet.Fim.Transforms;
//using Microsoft.MetadirectoryServices;
//using Lithnet.Common.ObjectModel;

//namespace Lithnet.Fim.Transforms.UnitTests
//{
//    /// <summary>
//    ///This is a test class for TrimStringTransformTest and is intended to contain all TrimStringTransformTest Unit Tests
//    ///</summary>
//    [TestClass()]
//    public class BooleanToBitmaskTransformTest
//    {
//        [ClassInitialize()]
//        public static void InitializeTest(TestContext testContext)
//        {
//            UnitTestControl.Initialize();
//        }

//        [TestMethod()]
//        public void TestSerialization()
//        {
//            UniqueIDCache.ClearIdCache();
//            BooleanToBitmaskTransform transformToSeralize = new BooleanToBitmaskTransform();
//            transformToSeralize.ID = "test001";
//            transformToSeralize.Flag = 2;
//            transformToSeralize.DefaultValue = 512;
//            UniqueIDCache.ClearIdCache();

//            BooleanToBitmaskTransform deserializedTransform = (BooleanToBitmaskTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

//            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
//            Assert.AreEqual(transformToSeralize.Flag, deserializedTransform.Flag);
//            Assert.AreEqual(transformToSeralize.DefaultValue, deserializedTransform.DefaultValue);
//        }
       
//        [TestMethod()]
//        public void BitmaskLoopbackInputTest()
//        {
//            BooleanToBitmaskTransform transform = new BooleanToBitmaskTransform();
//            transform.Flag = 2;

//            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 512, true, 514);
//            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 514, false, 512);
//            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 512, false, 512);
//            this.ExecuteTestBitwiseTransformLoopbackInput(transform, 514, true, 514);
//        }

//        [TestMethod()]
//        public void BitmaskLoopbackInputTestWithNullPrimaryInput()
//        {
//            BooleanToBitmaskTransform transform = new BooleanToBitmaskTransform();
//            transform.Flag = 2;
//            transform.DefaultValue = 512;

//            this.ExecuteTestBitwiseTransformLoopbackInput(transform, null, true, 514);
//            this.ExecuteTestBitwiseTransformLoopbackInput(transform, null, false, 512);
//            this.ExecuteTestBitwiseTransformLoopbackInput(transform, null, false, 512);
//            this.ExecuteTestBitwiseTransformLoopbackInput(transform, null, true, 514);
//        }

//        private void ExecuteTestBitwiseTransformLoopbackInput(BooleanToBitmaskTransform transform, object targetValue, bool inputValue, long expectedValue)
//        {
//            List<object> inputValues = new List<object>() { inputValue };
//            long outValue = (long)transform.Execute(inputValues, ExtendedAttributeType.Integer, targetValue);
//            Assert.AreEqual(expectedValue, outValue);
//        }
//    }
//}
