using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.Fim.UniversalMARE;
using Lithnet.Fim.Transforms;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Core;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.UniversalMARE.UnitTests
{
    [TestClass]
    public class ExportTests
    {
        private IMASynchronization rulesExtension;
        private MAExtensionObject re;

        public ExportTests()
        {
            Lithnet.Logging.Logger.LogLevel = Logging.LogLevel.Debug;
            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            MAExtensionObject re = new MAExtensionObject();
            this.re = re;
            this.rulesExtension = re;
        }

        [TestMethod]
        public void TestSimpleExportTransformString()
        {
            GetDNComponentTransform tx = new GetDNComponentTransform();
            tx.ID = "GetDN786768";
            tx.ComponentIndex = 1;
            tx.RdnFormat = RdnFormat.ValueOnly;

            this.re.config.Transforms.Add(tx);

            this.ExecuteExportTest(
                AttributeType.String,
                AttributeType.String,
                new List<Transform>() { tx },
                new List<object>() { "Test User" },
                new List<object>() { "CN=Test User, OU=Test" }
                );
        }

        [TestMethod]
        public void TestLoopbackExportTransform()
        {
            MVBooleanToBitmaskTransform tx = new MVBooleanToBitmaskTransform();
            tx.ID = "BBT2";
            tx.DefaultValue = 768;
            tx.Flags.Add(new FlagValue() { Name = "Test", Value = 2 });

            this.re.config.Transforms.Add(tx);

            this.ExecuteExportLoopbackTest(
                AttributeType.Boolean,
                AttributeType.Integer,
                new List<Transform>() { tx },
                514L,
                512,
                true
                );
        }

        [TestMethod]
        public void TestLoopbackExportTransformWithNullTargetValue()
        {
            MVBooleanToBitmaskTransform tx = new MVBooleanToBitmaskTransform();
            tx.ID = "BBT1";
            tx.DefaultValue = 768;
            tx.Flags.Add(new FlagValue() { Name = "Test", Value = 2 });

            this.re.config.Transforms.Add(tx);

            this.ExecuteExportLoopbackTest(
                AttributeType.Boolean,
                AttributeType.Integer,
                new List<Transform>() { tx },
                770L,
                null,
                true
                );
        }

        [TestMethod]
        public void TestSimpleExportDoubleTransformString()
        {
            GetDNComponentTransform tx1 = new GetDNComponentTransform();
            tx1.ID = "GetDN34534534";
            tx1.ComponentIndex = 1;
            tx1.RdnFormat = RdnFormat.ValueOnly;
            this.re.config.Transforms.Add(tx1);

            StringCaseTransform tx2 = new StringCaseTransform();
            tx2.ID = "ToLower";
            tx2.StringCase = StringCaseType.Lower;

            this.re.config.Transforms.Add(tx2);

            this.ExecuteExportTest(
                AttributeType.String,
                AttributeType.String,
                new List<Transform>() { tx1, tx2 },
                new List<object>() { "test user" },
                new List<object>() { "CN=Test User, OU=Test" }
                );
        }

        [TestMethod]
        public void TestSimpleExportTransformInteger()
        {
            BitmaskTransform tx = new BitmaskTransform();
            tx.ID = "EnableAccount";
            tx.Flag = 2;
            tx.Operation = BitwiseOperation.Or;

            this.re.config.Transforms.Add(tx);

            ExecuteExportTest(
                AttributeType.Integer,
                AttributeType.Integer,
                new List<Transform>() { tx },
                new List<object>() { 514L },
                new List<object>() { 512L }
                );
        }

        [TestMethod]
        public void TestMultiAttributeExportTransformInteger()
        {
            MultivalueToSingleValueTransform tx = new MultivalueToSingleValueTransform();
            tx.ID = "GetLargest";
            tx.SelectorOperator = Core.ValueOperator.Largest;
            tx.CompareAs = ExtendedAttributeType.Integer;

            this.re.config.Transforms.Add(tx);

            this.ExecuteExportTest(
                AttributeType.Integer,
                AttributeType.Integer,
                new List<Transform>() { tx },
                new List<object>() { 99L },
                new List<object>() { 1L, 99L }
                );
        }

        [TestMethod]
        public void TestMultiValuedMultiAttributeExportTransformInteger()
        {
            MultivalueToSingleValueTransform tx = new MultivalueToSingleValueTransform();
            tx.ID = "GetLargestMV";
            tx.SelectorOperator = Core.ValueOperator.Largest;
            tx.CompareAs = ExtendedAttributeType.Integer;

            this.re.config.Transforms.Add(tx);

            this.ExecuteExportTest(
                AttributeType.Integer,
                AttributeType.Integer,
                new List<Transform>() { tx },
                new List<object>() { 105L },
                new List<object>() { 1L, 99L },
                new List<object>() { 105L }
                );
        }

        [TestMethod]
        public void TestBooleanOperationWithLookupTransform1()
        {
            BooleanOperationTransform tx1 = new BooleanOperationTransform();
            tx1.ID = Guid.NewGuid().ToString();
            tx1.Operator = BitwiseOperation.Or;

            SimpleLookupTransform tx2 = new SimpleLookupTransform();
            tx2.ID = Guid.NewGuid().ToString();
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "ADMIN", NewValue = "true" });
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "ABUSE", NewValue = "true" });
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "true", NewValue = "true" });
            tx2.DefaultValue = "false";

            this.re.config.Transforms.Add(tx1);
            this.re.config.Transforms.Add(tx2);

            this.ExecuteExportTest(
                AttributeType.String,
                AttributeType.Boolean,
                new List<Transform>() { tx1, tx2 },
                new List<object>() { true },
                new List<object>() { "ABUSE" },
                new List<object>() { true }
                );
        }

        [TestMethod]
        public void TestBooleanOperationWithLookupTransform2()
        {
            BooleanOperationTransform tx1 = new BooleanOperationTransform();
            tx1.ID = Guid.NewGuid().ToString();
            tx1.Operator = BitwiseOperation.Or;

            SimpleLookupTransform tx2 = new SimpleLookupTransform();
            tx2.ID = Guid.NewGuid().ToString();
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "ADMIN", NewValue = "true" });
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "ABUSE", NewValue = "true" });
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "true", NewValue = "true" });
            tx2.DefaultValue = "false";
            tx2.OnMissingMatch = OnMissingMatch.UseDefault;

            this.re.config.Transforms.Add(tx1);
            this.re.config.Transforms.Add(tx2);

            this.ExecuteExportTest(
                AttributeType.String,
                AttributeType.Boolean,
                new List<Transform>() { tx1, tx2 },
                new List<object>() { false },
                new List<object>() { false }
                );
        }

        [TestMethod]
        public void TestBooleanOperationWithLookupTransform3()
        {
            BooleanOperationTransform tx1 = new BooleanOperationTransform();
            tx1.ID = Guid.NewGuid().ToString();
            tx1.Operator = BitwiseOperation.Or;

            SimpleLookupTransform tx2 = new SimpleLookupTransform();
            tx2.ID = Guid.NewGuid().ToString();
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "ADMIN", NewValue = "true" });
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "ABUSE", NewValue = "true" });
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "true", NewValue = "true" });
            tx2.DefaultValue = "false";
            tx2.OnMissingMatch = OnMissingMatch.UseDefault;

            this.re.config.Transforms.Add(tx1);
            this.re.config.Transforms.Add(tx2);

            this.ExecuteExportTest(
                AttributeType.String,
                AttributeType.Boolean,
                new List<Transform>() { tx1, tx2 },
                new List<object>() { true },
                new List<object>() { true }
                );
        }

        [TestMethod]
        public void TestBooleanOperationWithLookupTransform4()
        {
            BooleanOperationTransform tx1 = new BooleanOperationTransform();
            tx1.ID = Guid.NewGuid().ToString();
            tx1.Operator = BitwiseOperation.Or;

            SimpleLookupTransform tx2 = new SimpleLookupTransform();
            tx2.ID = Guid.NewGuid().ToString();
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "ADMIN", NewValue = "true" });
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "ABUSE", NewValue = "true" });
            tx2.LookupItems.Add(new LookupItem() { CurrentValue = "true", NewValue = "true" });
            tx2.DefaultValue = "false";
            tx2.OnMissingMatch = OnMissingMatch.UseDefault;

            this.re.config.Transforms.Add(tx1);
            this.re.config.Transforms.Add(tx2);

            this.ExecuteExportTest(
                AttributeType.String,
                AttributeType.Boolean,
                new List<Transform>() { tx1, tx2 },
                new List<object>() { true },
                new List<object>() { "ABUSE" }
                );
        }

        public void ExecuteExportTest(
            AttributeType sourceDataType,
            AttributeType targetDataType,
            List<Transform> transforms,
            List<object> expectedValues,
            params List<object>[] sourceAttributes
            )
        {
            string csattributeName = "csattribute";

            TestCSEntry csentry = new TestCSEntry();
            csentry.Add(this.CreateAttribute(csattributeName, targetDataType, false, null));

            TestMVEntry mventry = new TestMVEntry();

            int count = 0;
            List<string> attributeNames = new List<string>();

            foreach (List<object> attribute in sourceAttributes)
            {
                count++;
                string attributeName = "mvattribute" + count;
                attributeNames.Add(attributeName);

                if (attribute.Count > 1)
                {
                    mventry.Add(this.CreateAttribute(attributeName, sourceDataType, attribute));
                }
                else if (attribute.Count == 1)
                {
                    mventry.Add(this.CreateAttribute(attributeName, sourceDataType, false, attribute.First()));
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
                "{0}<<{1}<<{2}",
                csattributeName,
                transformNames.ToSeparatedString("<<"),
                attributeNames.ToSeparatedString("+"));

            this.rulesExtension.MapAttributesForExport(flowRuleName, mventry, csentry);

            Attrib returnedAttribute = csentry["csattribute"];

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

        public void ExecuteExportLoopbackTest(
          AttributeType sourceDataType,
          AttributeType targetDataType,
          List<Transform> transforms,
          object expectedValue,
            object targetValue,
            object sourceValue
          )
        {
            string csattributeName = "csattribute";

            TestCSEntry csentry = new TestCSEntry();
            csentry.Add(this.CreateAttribute(csattributeName, targetDataType, false, targetValue));

            TestMVEntry mventry = new TestMVEntry();

            string attributeName = "mvattribute1";
            mventry.Add(this.CreateAttribute(attributeName, sourceDataType, false, sourceValue));

            List<string> transformNames = new List<string>();
            foreach (Transform transform in transforms)
            {
                transformNames.Add(transform.ID);
            }

            string flowRuleName = string.Format(
                "{0}<<{1}<<{2}",
                csattributeName,
                transformNames.ToSeparatedString("<<"),
                attributeName);

            this.rulesExtension.MapAttributesForExport(flowRuleName, mventry, csentry);

            Attrib returnedAttribute = csentry["csattribute"];

            switch (targetDataType)
            {
                case AttributeType.Binary:
                    CollectionAssert.AreEqual(returnedAttribute.BinaryValue, (byte[])expectedValue, new ByteArrayComparer());
                    break;

                case AttributeType.Boolean:
                    Assert.AreEqual(returnedAttribute.BooleanValue, (bool)expectedValue);
                    break;

                case AttributeType.Integer:
                    Assert.AreEqual(returnedAttribute.IntegerValue, (long)expectedValue);
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
