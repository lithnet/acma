using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Xml;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Linq;

namespace Lithnet.Fim.Transforms.UnitTests
{
    /// <summary>
    ///This is a test class for BooleanOperationTransform and is intended to contain all BooleanOperationTransform Unit Tests
    ///</summary>
    [TestClass()]
    public class BooleanOperationTransformTests
    {
        public BooleanOperationTransformTests()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            BooleanOperationTransform transformToSeralize = new BooleanOperationTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.Operator = BitwiseOperation.And;
            UniqueIDCache.ClearIdCache();

            BooleanOperationTransform deserializedTransform = (BooleanOperationTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.Operator, deserializedTransform.Operator);
        }

        // AND tests

        [TestMethod()]
        public void BooleanOperation2BooleanAnd()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.And;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, true }, true);
        }

        [TestMethod()]
        public void BooleanOperation3BooleanAnd()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.And;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, true, true }, true);
        }

        [TestMethod()]
        public void BooleanOperation3BooleanAndNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.And;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, true, false }, false);
        }

        [TestMethod()]
        public void BooleanOperation2BooleanAndNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.And;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, false }, false);
        }

        // OR Tests

        [TestMethod()]
        public void BooleanOperation2BooleanOr()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Or;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, true }, true);
        }

        [TestMethod()]
        public void BooleanOperation3BooleanOr()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Or;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, true, false }, true);
        }

        [TestMethod()]
        public void BooleanOperation3BooleanOrNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Or;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { false, false, false }, false);
        }

        [TestMethod()]
        public void BooleanOperation2BooleanOrNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Or;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { false, false }, false);
        }

        // Xor

        [TestMethod()]
        public void BooleanOperation2BooleanXor()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Xor;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, false }, true);
        }

        [TestMethod()]
        public void BooleanOperation3BooleanXor()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Xor;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, false, false }, true);
        }

        [TestMethod()]
        public void BooleanOperation3BooleanXorNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Xor;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, true, false }, false);
        }

        [TestMethod()]
        public void BooleanOperation2BooleanXorNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Xor;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, true }, false);
        }

        [TestMethod()]
        public void BooleanOperation2BooleanXorNegative2()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Xor;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { false, false }, false);
        }

        // NAND tests

        [TestMethod()]
        public void BooleanOperation2BooleanNand()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Nand;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { false, false}, true);
        }

        [TestMethod()]
        public void BooleanOperation3BooleanNand()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Nand;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { false, false, false}, true);
        }

        [TestMethod()]
        public void BooleanOperation3BooleanNandNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Nand;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { false, true, false }, false);
        }

        [TestMethod()]
        public void BooleanOperation2BooleanNandNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.Nand;

            this.ExecuteBooleanTest(transform, AttributeType.Boolean, new List<object>() { true, true }, false);
        }

        // String based input tests

        [TestMethod()]
        public void BooleanOperation2StringAnd()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.And;

            this.ExecuteBooleanTest(transform, AttributeType.String, new List<object>() { "true", "true" }, true);
        }

        [TestMethod()]
        public void BooleanOperation3StringAnd()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.And;

            this.ExecuteBooleanTest(transform, AttributeType.String, new List<object>() { "true", "true", "true" }, true);
        }

        [TestMethod()]
        public void BooleanOperation3StringAndNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.And;

            this.ExecuteBooleanTest(transform, AttributeType.String, new List<object>() { "true", "true", "false" }, false);
        }

        [TestMethod()]
        public void BooleanOperation2StringAndNegative()
        {
            BooleanOperationTransform transform = new BooleanOperationTransform();
            transform.ID = Guid.NewGuid().ToString();
            transform.Operator = BitwiseOperation.And;

            this.ExecuteBooleanTest(transform, AttributeType.String, new List<object>() { "true", "false" }, false);
        }


        private void ExecuteBooleanTest(BooleanOperationTransform transform, AttributeType type, IList<object> inputValues, bool expectedValue)
        {
            bool result = (bool)transform.TransformValue(inputValues).First();
            Assert.AreEqual(expectedValue, result);
        }
    }
}
