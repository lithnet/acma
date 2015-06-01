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
    public class XmlLookupTransformTest
    {
        public XmlLookupTransformTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            XmlLookupTransform transformToSeralize = new XmlLookupTransform();
            transformToSeralize.DefaultValue = "A";
            transformToSeralize.FileName = "B";
            transformToSeralize.XPathQuery = "Query";
            transformToSeralize.OnMissingMatch = OnMissingMatch.UseNull;
            transformToSeralize.ID = "test001";
            transformToSeralize.UserDefinedReturnType = ExtendedAttributeType.Boolean;
            UniqueIDCache.ClearIdCache();

            XmlLookupTransform deserializedTransform = (XmlLookupTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.DefaultValue, deserializedTransform.DefaultValue);
            Assert.AreEqual(transformToSeralize.FileName, deserializedTransform.FileName);
            Assert.AreEqual(transformToSeralize.XPathQuery, deserializedTransform.XPathQuery);
            Assert.AreEqual(transformToSeralize.OnMissingMatch, deserializedTransform.OnMissingMatch);
            Assert.AreEqual(transformToSeralize.UserDefinedReturnType, deserializedTransform.UserDefinedReturnType);
        }

        [TestMethod()]
        public void XmlLookupTransformStringElementTest()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            this.ExecuteTestString(transform, "1234", "MyName");
        }

        [TestMethod()]
        public void XmlLookupTransformStringUseNull()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseNull;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTestString(transform, "novalue", null);
        }

        [TestMethod()]
        public void XmlLookupTransformBinaryElementTest()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseNull;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;

            this.ExecuteTestBinary(transform, new byte[] { 0, 1, 1, 33 }, new byte[] { 0, 1, 1, 34 });
        }

        [TestMethod()]
        public void XmlLookupTransformBinaryUseOriginal()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;

            this.ExecuteTestBinary(transform, new byte[] { 99, 99, 99, 99 }, new byte[] { 99, 99, 99, 99 });
        }

        [TestMethod()]
        public void XmlLookupTransformInteger()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(transform, 123456789L, 5L);
        }

        [TestMethod()]
        public void XmlLookupTransformIntegerUseOriginal()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.Integer;

            this.ExecuteTestInteger(transform, 55L, 55L);
        }

        [TestMethod()]
        public void XmlLookupTransformBinaryUseDefault()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseDefault;
            transform.DefaultValue = "AAEBIQ==";
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;

            this.ExecuteTestBinary(transform, new byte[] { 99, 99, 99, 99 }, new byte[] { 0, 1, 1, 33 });
        }

        [TestMethod()]
        public void XmlLookupTransformBinaryUseNull()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseNull;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;

            this.ExecuteTestBinary(transform, new byte[] { 99, 99, 99, 99 }, null);
        }

        [TestMethod()]
        public void XmlLookupTransformStringUseOriginal()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTestString(transform, "novalue", "novalue");
        }

        [TestMethod()]
        public void XmlLookupTransformStringUseDefault()
        {
            XmlLookupTransform transform = new XmlLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.xml";
            transform.XPathQuery = @"OUMappings/OUMapping[@sapOUNumber='{attributeValue}']/@MDSDisplayName";
            transform.OnMissingMatch = OnMissingMatch.UseDefault;
            transform.DefaultValue = "defaultValue";
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTestString(transform, "novalue", "defaultValue");
        }

        private void ExecuteTestBinary(XmlLookupTransform transform, byte[] sourceValue, byte[] expectedValue)
        {
            byte[] outValue = transform.TransformValue(sourceValue).FirstOrDefault() as byte[];

            CollectionAssert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestString(XmlLookupTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestInteger(XmlLookupTransform transform, long sourceValue, long expectedValue)
        {
            long outValue = (long)transform.TransformValue(sourceValue).FirstOrDefault();

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
