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
    public class JoinTests
    {
        private IMASynchronization rulesExtension;
        private MAExtensionObject re;

        public JoinTests()
        {
            Lithnet.Logging.Logger.LogLevel = Logging.LogLevel.Debug;
            UINotifyPropertyChanges.BeginIgnoreAllChanges();
            MAExtensionObject re = new MAExtensionObject();
            this.re = re;
            this.rulesExtension = re;
        }

        [TestMethod]
        public void TestSimpleJoinTransformString()
        {
            GetDNComponentTransform tx = new GetDNComponentTransform();
            tx.ID = "GetDN6";
            tx.ComponentIndex = 1;
            tx.RdnFormat = RdnFormat.ValueOnly;

            this.re.config.Transforms.Add(tx);

            this.ExecuteJoinTest(
                AttributeType.String,
                new List<Transform>() { tx },
                new List<object>() { "Test User" },
                new List<object>() { "CN=Test User, OU=Test" }
                );
        }

        [TestMethod]
        public void TestSimpleJoinDoubleTransformString()
        {
            GetDNComponentTransform tx1 = new GetDNComponentTransform();
            tx1.ID = "GetDN7";
            tx1.ComponentIndex = 1;
            tx1.RdnFormat = RdnFormat.ValueOnly;
            this.re.config.Transforms.Add(tx1);

            StringCaseTransform tx2 = new StringCaseTransform();
            tx2.ID = "ToLower634";
            tx2.StringCase = StringCaseType.Lower;

            this.re.config.Transforms.Add(tx2);

            this.ExecuteJoinTest(
                AttributeType.String,
                new List<Transform>() { tx1, tx2 },
                new List<object>() { "test user" },
                new List<object>() { "CN=Test User, OU=Test" }
                );
        }

        [TestMethod]
        public void TestSimpleJoinTransformInteger()
        {
            BitmaskTransform tx = new BitmaskTransform();
            tx.ID = "EnableAccount8";
            tx.Flag = 2;
            tx.Operation = BitwiseOperation.Or;

            this.re.config.Transforms.Add(tx);

            ExecuteJoinTest(
                AttributeType.Integer,
                new List<Transform>() { tx },
                new List<object>() { 514L },
                new List<object>() { 512L }
                );
        }

        [TestMethod]
        public void TestMultiAttributeJoinTransformInteger()
        {
            MultivalueToSingleValueTransform tx = new MultivalueToSingleValueTransform();
            tx.ID = "GetLargest9";
            tx.SelectorOperator = Core.ValueOperator.Largest;
            tx.CompareAs = ExtendedAttributeType.Integer;

            this.re.config.Transforms.Add(tx);

            this.ExecuteJoinTest(
                AttributeType.Integer,
                new List<Transform>() { tx },
                new List<object>() { 99L },
                new List<object>() { 1L, 99L }
                );
        }

        public void ExecuteJoinTest(
            AttributeType sourceDataType,
            List<Transform> transforms,
            List<object> expectedValues,
            params List<object>[] sourceAttributes
            )
        {
           
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
                "{0}>>{1}",
                attributeNames.ToSeparatedString("+"),
                transformNames.ToSeparatedString(">>"));

            ValueCollection collection = new TestValueCollection(sourceDataType);
            this.rulesExtension.MapAttributesForJoin(flowRuleName, csentry, ref collection);

            switch (sourceDataType)
            {
                case AttributeType.Binary:
                    CollectionAssert.AreEqual(collection.ToByteArrays(), expectedValues, new ByteArrayComparer());
                    break;

                case AttributeType.Integer:
                    CollectionAssert.AreEqual(collection.ToIntegerArray(), expectedValues);
                    break;

                case AttributeType.String:
                    CollectionAssert.AreEqual(collection.ToStringArray(), expectedValues);
                    break;

                case AttributeType.Boolean:
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
