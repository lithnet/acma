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
    public class StringEscapeTransformTest
    {
        public StringEscapeTransformTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            StringEscapeTransform transformToSeralize = new StringEscapeTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.EscapeType = StringEscapeType.LdapDN;
            UniqueIDCache.ClearIdCache();

            StringEscapeTransform deserializedTransform = (StringEscapeTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.EscapeType, deserializedTransform.EscapeType);
        }
        
        [TestMethod()]
        public void TestEscapeXmlAmp()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.Xml;

            this.ExecuteTest(transform, "Things & stuff", "Things &amp; stuff");
        }

        [TestMethod()]
        public void TestEscapeXmlQuot()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.Xml;

            this.ExecuteTest(transform, "Things \"stuff\"", "Things &quot;stuff&quot;");
        }

        [TestMethod()]
        public void TestEscapeXmlLt()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.Xml;

            this.ExecuteTest(transform, "Things < stuff", "Things &lt; stuff");
        }

        [TestMethod()]
        public void TestEscapeXmlGt()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.Xml;

            this.ExecuteTest(transform, "Things > stuff", "Things &gt; stuff");
        }

        [TestMethod()]
        public void TestEscapeXmlApos()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.Xml;

            this.ExecuteTest(transform, "Things ' stuff", "Things &apos; stuff");
        }

        [TestMethod()]
        public void TestEscapeDNSpace()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, "Department of, IT", "\"Department of, IT\"");
        }

        [TestMethod()]
        public void TestEscapeDNQuote()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, "\"IT\"", "\\\"IT\\\"");
        }

        [TestMethod()]
        public void TestEscapeDNHash()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, "#IT#", "\\#IT\\#");
        }

        [TestMethod()]
        public void TestEscapeDNPlus()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, "+IT+", "\\+IT\\+");
        }

        [TestMethod()]
        public void TestEscapeDNSemiColon()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, ";IT;", "\\;IT\\;");
        }

        [TestMethod()]
        public void TestEscapeDNLtGt()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, "<IT>", "\\<IT\\>");
        }

        [TestMethod()]
        public void TestEscapeDNEquals()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, "=IT=", "\\=IT\\=");
        }

        [TestMethod()]
        public void TestEscapeDNBackSlash()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, "\\IT\\", "\\\\IT\\\\");
        }

        [TestMethod()]
        public void TestEscapeDNLeadingSpace()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, " IT", "\\ IT");
        }

        [TestMethod()]
        public void TestEscapeDNTrailingSpace()
        {
            StringEscapeTransform transform = new StringEscapeTransform();
            transform.EscapeType = StringEscapeType.LdapDN;

            this.ExecuteTest(transform, "IT ", "IT\\ ");
        }

        private void ExecuteTest(StringEscapeTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
