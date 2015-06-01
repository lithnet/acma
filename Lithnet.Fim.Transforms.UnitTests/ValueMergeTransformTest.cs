using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Linq;
using System.Collections.Generic;

namespace Lithnet.Fim.Transforms.UnitTests
{
    [TestClass()]
    public class ValueMergeTransformTest
    {
        public ValueMergeTransformTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            ValueMergeTransform transformToSeralize = new ValueMergeTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.UserDefinedReturnType = ExtendedAttributeType.Integer;
            UniqueIDCache.ClearIdCache();

            ValueMergeTransform deserializedTransform = (ValueMergeTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.UserDefinedReturnType, deserializedTransform.UserDefinedReturnType);
        }

        [TestMethod()]
        public void TestSimpleTransform1()
        {
            ValueMergeTransform transform = new ValueMergeTransform();
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTest(transform, new List<object>() { "Test" , 3L, "Test2" }, new List<object>() { "Test", "3", "Test2" });
        }

        [TestMethod()]
        public void TestSimpleTransform2()
        {
            ValueMergeTransform transform = new ValueMergeTransform();
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTest(transform, new List<object>() { "Test", "Test" }, new List<object>() { "Test" });
        }

        [TestMethod()]
        public void TestSimpleTransform3()
        {
            ValueMergeTransform transform = new ValueMergeTransform();
            transform.UserDefinedReturnType = ExtendedAttributeType.Integer;

            this.ExecuteTest(transform, new List<object>() { 1L, 2L, "3", "1" }, new List<object>() { 1L, 2L, 3L });
        }

        [TestMethod()]
        public void TestSimpleTransform4()
        {
            ValueMergeTransform transform = new ValueMergeTransform();
            transform.UserDefinedReturnType = ExtendedAttributeType.Integer;

            this.ExecuteTest(transform, new List<object>() { "1" }, new List<object>() { 1L });
        }

        private void ExecuteTest(ValueMergeTransform transform, IList<object> sourceValues, IList<object> expectedValues)
        {
            IList<object> outValues = transform.TransformValue(sourceValues);

            CollectionAssert.AreEqual(expectedValues.ToArray(), outValues.ToArray());
        }
    }
}
