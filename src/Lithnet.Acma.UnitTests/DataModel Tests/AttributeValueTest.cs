
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for AttributeValueTest and is intended
    ///to contain all AttributeValueTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AttributeValueTest
    {
        public AttributeValueTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueObjectTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("accountName");
            AttributeValue value = new AttributeValue(attribute, "test");
            AttributeValue matchingValue = value;
            AttributeValue nonMatchingValue = new AttributeValue(attribute, "test2");

            EqualsAttributeValueTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("accountName");
            AttributeValue value = new AttributeValue(attribute, "test");
            AttributeValue matchingValue = new AttributeValue(attribute, "test");
            AttributeValue nonMatchingValue = new AttributeValue(attribute, "test2");

            EqualsAttributeValueTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueLongTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("unixUid");
            AttributeValue value = new AttributeValue(attribute, 44L);
            AttributeValue matchingValue = new AttributeValue(attribute, 44L);
            AttributeValue nonMatchingValue = new AttributeValue(attribute, 55L);

            EqualsAttributeValueTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueDateTimeTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
            AttributeValue value = new AttributeValue(attribute, DateTime.Parse("2010-01-01"));
            AttributeValue matchingValue = new AttributeValue(attribute, DateTime.Parse("2010-01-01"));
            AttributeValue nonMatchingValue = new AttributeValue(attribute, DateTime.Parse("2011-01-01"));

            EqualsAttributeValueTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueBooleanTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            AttributeValue value = new AttributeValue(attribute, true);
            AttributeValue matchingValue = new AttributeValue(attribute, true);
            AttributeValue nonMatchingValue = new AttributeValue(attribute, false);

            EqualsAttributeValueTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueBinaryTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSid");
            AttributeValue value = new AttributeValue(attribute, new byte[] {0,1,2,3,4});
            AttributeValue matchingValue = new AttributeValue(attribute, new byte[] { 0,1,2,3,4});
            AttributeValue nonMatchingValue = new AttributeValue(attribute, new byte[] { 1, 2, 3, 4, 5 });

            EqualsAttributeValueTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueGuidTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("supervisor");
            AttributeValue value = new AttributeValue(attribute, new Guid("{8C55BB1B-7CC9-4E36-8FDA-0FFF5D8150AF}"));
            AttributeValue matchingValue = new AttributeValue(attribute, new Guid("{8C55BB1B-7CC9-4E36-8FDA-0FFF5D8150AF}"));
            AttributeValue nonMatchingValue = new AttributeValue(attribute, Guid.NewGuid());

            EqualsAttributeValueTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueNullTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("supervisor");
            AttributeValue value = new AttributeValue(attribute, new Guid("{8C55BB1B-7CC9-4E36-8FDA-0FFF5D8150AF}"));
            AttributeValue matchingValue = new AttributeValue(attribute, new Guid("{8C55BB1B-7CC9-4E36-8FDA-0FFF5D8150AF}"));
            AttributeValue nonMatchingValue = new AttributeValue(attribute, Guid.NewGuid());

            EqualsAttributeValueTest(attribute, value, matchingValue, null);
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueTypeMismatchTest()
        {
            AcmaSchemaAttribute attribute1 = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaAttribute attribute2 = ActiveConfig.DB.GetAttribute("objectSid");
            AttributeValue value = new AttributeValue(attribute1, new Guid("{8C55BB1B-7CC9-4E36-8FDA-0FFF5D8150AF}"));
            AttributeValue matchingValue = new AttributeValue(attribute1, new Guid("{8C55BB1B-7CC9-4E36-8FDA-0FFF5D8150AF}"));
            AttributeValue nonMatchingValue = new AttributeValue(attribute2, new byte[] { 0, 1, 2, 3, 4 });

            EqualsAttributeValueTest(attribute1, value, matchingValue, null);
        }

        [TestMethod()]
        public void EqualsObjectAsAttributeValueMVMismatchTest()
        {
            AcmaSchemaAttribute attribute1 = ActiveConfig.DB.GetAttribute("supervisor");
            AcmaSchemaAttribute attribute2 = ActiveConfig.DB.GetAttribute("objectSids");
            AttributeValue value = new AttributeValue(attribute1, new Guid("{8C55BB1B-7CC9-4E36-8FDA-0FFF5D8150AF}"));
            AttributeValue matchingValue = new AttributeValue(attribute1, new Guid("{8C55BB1B-7CC9-4E36-8FDA-0FFF5D8150AF}"));
            AttributeValue nonMatchingValue = new AttributeValue(attribute2, new byte[] { 0, 1, 2, 3, 4 });

            EqualsAttributeValueTest(attribute1, value, matchingValue, null);
        }

        [TestMethod()]
        public void EqualsObjectAsStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("accountName");
            object value = "test";
            object matchingValue = "test";
            object nonMatchingValue = "test2";

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsLongTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("unixUid");
            object value = 234232423L;
            object matchingValue = 234232423L;
            object nonMatchingValue = 233L;

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsDateTimeTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
            object value = DateTime.Parse("2010-01-01");
            object matchingValue = DateTime.Parse("2010-01-01");
            object nonMatchingValue = DateTime.Parse("2011-01-01");

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsLongStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("unixUid");
            object value = 234232423L;
            object matchingValue = "234232423";
            object nonMatchingValue = "233";

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsBooleanTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            object value = true;
            object matchingValue = true;
            object nonMatchingValue =  false;

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsBooleanStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            object value = "true";
            object matchingValue = "true";
            object nonMatchingValue = "false";

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsBooleanStringTest2()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            object value = true;
            object matchingValue = "true";
            object nonMatchingValue = "false";

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsGuidTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("supervisor");
            object value = new Guid("9b74bb66-d344-4f12-b74a-8ea94a2a8c0f");
            object matchingValue = new Guid("9b74bb66-d344-4f12-b74a-8ea94a2a8c0f");
            object nonMatchingValue = new Guid("9999bb66-d344-4f12-b74a-8ea94a2aFFFF");

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsGuidStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("supervisor");
            object value = "9b74bb66-d344-4f12-b74a-8ea94a2a8c0f";
            object matchingValue = "9b74bb66-d344-4f12-b74a-8ea94a2a8c0f";
            object nonMatchingValue =  "9999bb66-d344-4f12-b74a-8ea94a2aFFFF";

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsBinaryTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSid");
            object value = new byte[] { 0, 1, 2, 3, 4 };
            object matchingValue = new byte[] { 0, 1, 2, 3, 4 };
            object nonMatchingValue = new byte[] { 0, 1 };

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void EqualsObjectAsBinaryStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSid");
            object value = new byte[] { 0, 1, 2, 3, 4 };
            object matchingValue = "AAECAwQ=";
            object nonMatchingValue = "AAACAwQ=";

            EqualsObjectTest(attribute, value, matchingValue, nonMatchingValue);
        }

        [TestMethod()]
        public void IsNullTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("accountName");
            AttributeValue target = new AttributeValue(attribute, null);
            Assert.AreEqual(true, target.IsNull);

            target = new AttributeValue(attribute, "notnull");
            Assert.AreEqual(false, target.IsNull);
        }

        [TestMethod()]
        public void ValueTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("accountName");
            AttributeValue target = new AttributeValue(attribute, "myname");
            Assert.AreEqual(true, (string)target.Value == "myname");
            Assert.AreEqual(false, (string)target.Value == "notmyname");
        }

        [TestMethod()]
        public void ValueBooleanTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("connectedToSap");
            AttributeValue target = new AttributeValue(attribute, true);
            Assert.AreEqual(true, target.ValueBoolean == true);
            Assert.AreEqual(false, target.ValueBoolean == false);
        }

        [TestMethod()]
        public void ValueByteTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("objectSid");
            AttributeValue target = new AttributeValue(attribute, new byte[] { 0, 1, 2, 3, 4 });
            Assert.AreEqual(true, target.ValueByte.SequenceEqual(new byte[] { 0, 1, 2, 3, 4 }));
            Assert.AreEqual(false, target.ValueByte.SequenceEqual(new byte[] { 0, 0, 2, 3, 4 }));
        }

        [TestMethod()]
        public void ValueGuidTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("supervisor");
            AttributeValue target = new AttributeValue(attribute,  new Guid("1da92f76-e46f-42b0-a84d-a6a72be95ca6"));
            Assert.AreEqual(true, target.ValueGuid ==  new Guid("1da92f76-e46f-42b0-a84d-a6a72be95ca6"));
            Assert.AreEqual(false, target.ValueGuid == new Guid("11111f76-e46f-42b0-a84d-a6a72be95555"));
        }

        [TestMethod()]
        public void ValueLongTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("unixUid");
            AttributeValue target = new AttributeValue(attribute,  1234L);
            Assert.AreEqual(true, target.ValueLong == 1234L);
            Assert.AreEqual(false, target.ValueLong == 4321L);
        }

        [TestMethod()]
        public void ValueDateTimeTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("dateTimeSV");
            AttributeValue target = new AttributeValue(attribute, DateTime.Parse("2010-01-01"));
            Assert.AreEqual(true, target.ValueDateTime == DateTime.Parse("2010-01-01"));
            Assert.AreEqual(false, target.ValueDateTime == DateTime.Parse("2011-01-01"));
        }

        [TestMethod()]
        public void ValueStringTest()
        {
            AcmaSchemaAttribute attribute = ActiveConfig.DB.GetAttribute("accountName");
            AttributeValue target = new AttributeValue(attribute, "test");
            Assert.AreEqual(true, target.ValueString == "test");
            Assert.AreEqual(false, target.ValueString == "nottest");
        }

        private static void EqualsAttributeValueTest(AcmaSchemaAttribute attribute, AttributeValue target, AttributeValue matchingValue, AttributeValue nonMatchingValue)
        {
            Assert.AreEqual(true, target.Equals(matchingValue));
            Assert.AreEqual(false, target.Equals(nonMatchingValue));
        }

        private static void EqualsObjectTest(AcmaSchemaAttribute attribute, object value, object matchingValue, object nonMatchingValue)
        {
            AttributeValue target = new AttributeValue(attribute, value);
            Assert.AreEqual(true, target.Equals(matchingValue));
            Assert.AreEqual(false, target.Equals(nonMatchingValue));
        }
    }
}
