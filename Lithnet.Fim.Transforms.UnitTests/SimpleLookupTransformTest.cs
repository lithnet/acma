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
    public class SimpleTransformTest
    {
        private TestContext testContextInstance;
        
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
            SimpleLookupTransform transformToSeralize = new SimpleLookupTransform();
            transformToSeralize.DefaultValue = "A";
            transformToSeralize.LookupItems.Add(new LookupItem() { CurrentValue = "test1", NewValue = "test2" });
            transformToSeralize.LookupItems.Add(new LookupItem() { CurrentValue = "test3", NewValue = "test4" });
            transformToSeralize.OnMissingMatch = OnMissingMatch.UseNull;
            transformToSeralize.UserDefinedReturnType = ExtendedAttributeType.Binary;

            transformToSeralize.ID = "test001";
            UniqueIDCache.ClearIdCache();

            SimpleLookupTransform deserializedTransform = (SimpleLookupTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.DefaultValue, deserializedTransform.DefaultValue);
            Assert.AreEqual(transformToSeralize.OnMissingMatch, deserializedTransform.OnMissingMatch);
            Assert.AreEqual(transformToSeralize.UserDefinedReturnType, deserializedTransform.UserDefinedReturnType);
            CollectionAssert.AreEqual(transformToSeralize.LookupItems, deserializedTransform.LookupItems, new LookupItemComparer());
        }

        [TestMethod()]
        public void SimpleLookupTransformStringElementTest()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "test1", NewValue = "test2" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "test3", NewValue = "test4" });
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTestString(transform, "test1", "test2");
        }
              
        [TestMethod()]
        public void SimpleLookupTransformStringUseNull()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "test1", NewValue = "test2" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "test3", NewValue = "test4" });
            transform.OnMissingMatch = OnMissingMatch.UseNull;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            
            this.ExecuteTestString(transform, "novalue", null);
        }

        [TestMethod()]
        public void SimpleLookupTransformBinaryElementTest()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "test1", NewValue = "test2" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "AAEBIQ==", NewValue = "AAEBIg==" });
            transform.OnMissingMatch = OnMissingMatch.UseNull;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;

            this.ExecuteTestBinary(transform, new byte[] { 0, 1, 1, 33 }, new byte[] { 0, 1, 1, 34 });
        }

        [TestMethod()]
        public void SimpleLookupTransformBinaryUseOriginal()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "test1", NewValue = "test2" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "AAEBIQ==", NewValue = "AAEBIg==" });
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;

            this.ExecuteTestBinary(transform, new byte[] { 99, 99, 99, 99 }, new byte[] { 99, 99, 99, 99 });
        }

        [TestMethod()]
        public void SimpleLookupTransformInteger()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "123456789", NewValue = "5" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "AAEBIQ==", NewValue = "AAEBIg==" });
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(transform, 123456789, 5);
        }

        [TestMethod()]
        public void SimpleLookupTransformIntegerUseOriginal()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "123456789", NewValue = "5" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "AAEBIQ==", NewValue = "AAEBIg==" });
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(transform, 55, 55);
        }

        [TestMethod()]
        public void SimpleLookupTransformBinaryUseDefault()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "123456789", NewValue = "5" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "AAEBIQ==", NewValue = "AAEBIg==" });
            transform.OnMissingMatch = OnMissingMatch.UseDefault;
            transform.DefaultValue = "AAEBIQ==";
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;

            this.ExecuteTestBinary(transform, new byte[] { 99, 99, 99, 99 }, new byte[] { 0, 1, 1, 33 });
        }

        [TestMethod()]
        public void SimpleLookupTransformBinaryUseNull()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "123456789", NewValue = "5" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "AAEBIQ==", NewValue = "AAEBIg==" });
            transform.OnMissingMatch = OnMissingMatch.UseNull;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;
            
            this.ExecuteTestBinary(transform, new byte[] { 99, 99, 99, 99 }, null);
        }

        [TestMethod()]
        public void SimpleLookupTransformStringUseOriginal()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "123456789", NewValue = "5" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "AAEBIQ==", NewValue = "AAEBIg==" });
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            
            this.ExecuteTestString(transform, "novalue", "novalue");
        }

        [TestMethod()]
        public void SimpleLookupTransformStringUseDefault()
        {
            SimpleLookupTransform transform = new SimpleLookupTransform();
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "123456789", NewValue = "5" });
            transform.LookupItems.Add(new LookupItem() { CurrentValue = "AAEBIQ==", NewValue = "AAEBIg==" });
            transform.OnMissingMatch = OnMissingMatch.UseDefault;
            transform.DefaultValue = "defaultValue";
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTestString(transform, "novalue", "defaultValue");
        }

        private void ExecuteTestBinary(SimpleLookupTransform transform, byte[] sourceValue, byte[] expectedValue)
        {
            byte[] outValue = transform.TransformValue(sourceValue).FirstOrDefault() as byte[];

            CollectionAssert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestString(SimpleLookupTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestInteger(SimpleLookupTransform transform, long sourceValue, long expectedValue)
        {
            long outValue = (long)transform.TransformValue(sourceValue).FirstOrDefault();

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
