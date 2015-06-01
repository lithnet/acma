using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.Fim.UniversalMARE;
using Lithnet.Fim.Transforms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Core;
using Lithnet.Common.Presentation;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.UniversalMARE.UnitTests
{
    [TestClass]
    public class ImportTests
    {
        private IMASynchronization rulesExtension;
        private MAExtensionObject re;

        public ImportTests()
        {
            Lithnet.Logging.Logger.LogLevel = Logging.LogLevel.Debug;
            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            MAExtensionObject re = new MAExtensionObject();
            this.re = re;
            this.rulesExtension = re;
        }

        [TestMethod]
        public void TestSimpleImportTransformString()
        {
            GetDNComponentTransform tx = new GetDNComponentTransform();
            tx.ID = "GetDN1";
            tx.ComponentIndex = 1;
            tx.RdnFormat = RdnFormat.ValueOnly;

            this.re.config.Transforms.Add(tx);

            this.ExecuteImportTest(
                AttributeType.String,
                AttributeType.String,
                new List<Transform>() { tx },
                new List<object>() { "Test User" },
                new List<object>() { "CN=Test User, OU=Test" }
                );
        }

        [TestMethod]
        public void TestLoopbackImportTransform()
        {
            MVBooleanToBitmaskTransform tx = new MVBooleanToBitmaskTransform();
            tx.ID = "BBT345341";
            tx.DefaultValue = 768;
            tx.Flags.Add(new FlagValue() { Name = "Test", Value = 2 });

            this.re.config.Transforms.Add(tx);

            this.ExecuteImportLoopbackTest(
                AttributeType.Boolean,
                AttributeType.Integer,
                new List<Transform>() { tx },
                514L,
                512,
                true
                );
        }

        [TestMethod]
        public void TestLoopbackImportTransformWithNullTargetValue()
        {
            MVBooleanToBitmaskTransform tx = new MVBooleanToBitmaskTransform();
            tx.ID = "BBT";
            tx.DefaultValue = 768;
            tx.Flags.Add(new FlagValue() { Name = "Test", Value = 2 });

            this.re.config.Transforms.Add(tx);

            this.ExecuteImportLoopbackTest(
                AttributeType.Boolean,
                AttributeType.Integer,
                new List<Transform>() { tx },
                770L,
                null,
                true
                );
        }

        [TestMethod]
        public void TestSimpleImportDoubleTransformString()
        {
            GetDNComponentTransform tx1 = new GetDNComponentTransform();
            tx1.ID = "GetDN2";
            tx1.ComponentIndex = 1;
            tx1.RdnFormat = RdnFormat.ValueOnly;
            this.re.config.Transforms.Add(tx1);

            StringCaseTransform tx2 = new StringCaseTransform();
            tx2.ID = "ToLower3";
            tx2.StringCase = StringCaseType.Lower;

            this.re.config.Transforms.Add(tx2);

            this.ExecuteImportTest(
                AttributeType.String,
                AttributeType.String,
                new List<Transform>() { tx1, tx2 },
                new List<object>() { "test user" },
                new List<object>() { "CN=Test User, OU=Test" }
                );
        }

        [TestMethod]
        public void TestSimpleImportTransformInteger()
        {
            BitmaskTransform tx = new BitmaskTransform();
            tx.ID = "EnableAccount3";
            tx.Flag = 2;
            tx.Operation = BitwiseOperation.Or;

            this.re.config.Transforms.Add(tx);

            ExecuteImportTest(
                AttributeType.Integer,
                AttributeType.Integer,
                new List<Transform>() { tx },
                new List<object>() { 514L },
                new List<object>() { 512L }
                );
        }

        [TestMethod]
        public void TestMultiAttributeImportTransformInteger()
        {
            MultivalueToSingleValueTransform tx = new MultivalueToSingleValueTransform();
            tx.ID = "GetLargest4";
            tx.SelectorOperator = Core.ValueOperator.Largest;
            tx.CompareAs = ExtendedAttributeType.Integer;

            this.re.config.Transforms.Add(tx);

            this.ExecuteImportTest(
                AttributeType.Integer,
                AttributeType.Integer,
                new List<Transform>() { tx },
                new List<object>() { 99L },
                new List<object>() { 1L, 99L }
                );
        }

        [TestMethod]
        public void TestMultiValuedMultiAttributeImportTransformInteger()
        {
            MultivalueToSingleValueTransform tx = new MultivalueToSingleValueTransform();
            tx.ID = "GetLargestMV5";
            tx.SelectorOperator = Core.ValueOperator.Largest;
            tx.CompareAs = ExtendedAttributeType.Integer;

            this.re.config.Transforms.Add(tx);

            this.ExecuteImportTest(
                AttributeType.Integer,
                AttributeType.Integer,
                new List<Transform>() { tx },
                new List<object>() { 105L },
                new List<object>() { 1L, 99L },
                new List<object>() { 105L }
                );
        }

        public void ExecuteImportTest(
            AttributeType sourceDataType,
            AttributeType targetDataType,
            List<Transform> transforms,
            List<object> expectedValues,
            params List<object>[] sourceAttributes
            )
        {
            string mvattributeName = "mvattribute";

            TestMVEntry mventry = new TestMVEntry();
            mventry.Add(this.CreateAttribute(mvattributeName, targetDataType, false, null));

            TestCSEntry csentry = new TestCSEntry();

            int count = 0;
            List<string> attributeNames = new List<string>();

            foreach (List<object> attribute in sourceAttributes)
            {
                count++;
                string attributeName = "csattribute" + count;
                attributeNames.Add(attributeName);

                if (attribute.Count > 1)
                {
                    csentry.Add(this.CreateAttribute(attributeName, sourceDataType, attribute));
                }
                else if (attribute.Count == 1)
                {
                    csentry.Add(this.CreateAttribute(attributeName, sourceDataType, false, attribute.First()));
                }
                else
                {
                    throw new NullReferenceException("A value must be provided");
                }
            }

            List<string> transformNames = new List<string>();
            foreach (Transform transform in transforms)
            {
                transformNames.Add(transform.ID);
            }

            string flowRuleName = string.Format(
                "{0}>>{1}>>{2}",
                attributeNames.ToSeparatedString("+"),
                transformNames.ToSeparatedString(">>"),
                mvattributeName);

            this.rulesExtension.MapAttributesForImport(flowRuleName, csentry, mventry);

            Attrib returnedAttribute = mventry["mvattribute"];

            switch (targetDataType)
            {
                case AttributeType.Binary:
                    CollectionAssert.AreEqual(returnedAttribute.Values.ToByteArrays(), expectedValues, new ByteArrayComparer());
                    break;

                case AttributeType.Boolean:
                    Assert.AreEqual(returnedAttribute.BooleanValue, (bool)expectedValues.First());
                    break;

                case AttributeType.Integer:
                    CollectionAssert.AreEqual(returnedAttribute.Values.ToIntegerArray(), expectedValues);
                    break;

                case AttributeType.String:
                    CollectionAssert.AreEqual(returnedAttribute.Values.ToStringArray(), expectedValues);
                    break;

                case AttributeType.Reference:
                case AttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }

        public void ExecuteImportLoopbackTest(
          AttributeType sourceDataType,
          AttributeType targetDataType,
          List<Transform> transforms,
          object expectedValue,
            object targetValue,
            object sourceValue
          )
        {
            string mvattributeName = "mvattribute";

            TestMVEntry mventry = new TestMVEntry();
            mventry.Add(this.CreateAttribute(mvattributeName, targetDataType, false, targetValue));

            TestCSEntry csentry = new TestCSEntry();

            string attributeName = "csattribute1";
            csentry.Add(this.CreateAttribute(attributeName, sourceDataType, false, sourceValue));

            List<string> transformNames = new List<string>();
            foreach (Transform transform in transforms)
            {
                transformNames.Add(transform.ID);
            }

            string flowRuleName = string.Format(
                "{0}>>{1}>>{2}",
                attributeName,
                transformNames.ToSeparatedString(">>"),
                mvattributeName);

            this.rulesExtension.MapAttributesForImport(flowRuleName, csentry, mventry);

            Attrib returnedAttribute = mventry["mvattribute"];

            switch (targetDataType)
            {
                case AttributeType.Binary:
                    CollectionAssert.AreEqual(returnedAttribute.BinaryValue, (byte[])expectedValue, new ByteArrayComparer());
                    break;

                case AttributeType.Boolean:
                    Assert.AreEqual(returnedAttribute.BooleanValue, (bool)expectedValue);
                    break;

                case AttributeType.Integer:
                    Assert.AreEqual(returnedAttribute.IntegerValue , (long)expectedValue);
                    break;

                case AttributeType.String:
                    Assert.AreEqual(returnedAttribute.StringValue, (string)expectedValue);
                    break;

                case AttributeType.Reference:
                case AttributeType.Undefined:
                default:
                    throw new UnknownOrUnsupportedDataTypeException();
            }
        }


        private TestAttrib CreateAttribute(string name, AttributeType type, bool isMultivalued, object value)
        {
            TestAttrib attribute = new TestAttrib(name, type, false);

            if (value != null)
            {
                attribute.SetValue(value);
            }

            return attribute;
        }

        private TestAttrib CreateAttribute(string name, AttributeType type, IEnumerable<object> values)
        {
            TestAttrib attribute = new TestAttrib(name, type, true);

            if (values != null)
            {
                attribute.SetValues(values);
            }

            return attribute;
        }
    }
}
