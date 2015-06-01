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
    public class GetDNComponentTransformTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

            GetDNComponentTransform transformToSeralize = new GetDNComponentTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.RdnFormat = RdnFormat.Rdn;
            transformToSeralize.ComponentIndex = 5;
            transformToSeralize.Direction = Direction.Right;

            UniqueIDCache.ClearIdCache();

            GetDNComponentTransform deserializedTransform = (GetDNComponentTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.RdnFormat, deserializedTransform.RdnFormat);
            Assert.AreEqual(transformToSeralize.ComponentIndex, deserializedTransform.ComponentIndex);
            Assert.AreEqual(transformToSeralize.Direction, deserializedTransform.Direction);
        }

        [TestMethod()]
        public void GetDNComponentTransformValue1()
        {
            GetDNComponentTransform transform = new GetDNComponentTransform();
            transform.ComponentIndex = 1;
            transform.RdnFormat = RdnFormat.ValueOnly;
            transform.Direction = Direction.Left;

            this.ExecuteTestGetDN(transform, "cn=Test user, ou=Organisation", "Test user");
        }

        [TestMethod()]
        public void GetDNComponentTransformValue2()
        {
            GetDNComponentTransform transform = new GetDNComponentTransform();
            transform.ComponentIndex = 2;
            transform.RdnFormat = RdnFormat.ValueOnly;
            transform.Direction = Direction.Left;

            this.ExecuteTestGetDN(transform, "cn=Test user, ou=Organisation", "Organisation");
        }
      
        [TestMethod()]
        public void GetDNComponentTransformValue1Right()
        {
            GetDNComponentTransform transform = new GetDNComponentTransform();
            transform.ComponentIndex = 1;
            transform.RdnFormat = RdnFormat.ValueOnly;
            transform.Direction = Direction.Right;

            this.ExecuteTestGetDN(transform, "cn=Test user, ou=Organisation", "Organisation");
        }

        [TestMethod()]
        public void GetDNComponentTransformValue2Right()
        {
            GetDNComponentTransform transform = new GetDNComponentTransform();
            transform.ComponentIndex = 2;
            transform.RdnFormat = RdnFormat.ValueOnly;
            transform.Direction = Direction.Right;

            this.ExecuteTestGetDN(transform, "cn=Test user, ou=Organisation", "Test user");
        }

        [TestMethod()]
        public void GetDNComponentTransformRDN1()
        {
            GetDNComponentTransform transform = new GetDNComponentTransform();
            transform.ComponentIndex = 1;
            transform.RdnFormat = RdnFormat.Rdn;
            transform.Direction = Direction.Left;
            this.ExecuteTestGetDN(transform, "cn=Test user, ou=Organisation", "cn=Test user");
        }

        [TestMethod()]
        public void GetDNComponentTransformRDN2()
        {
            GetDNComponentTransform transform = new GetDNComponentTransform();
            transform.ComponentIndex = 2;
            transform.RdnFormat = RdnFormat.Rdn;
            transform.Direction = Direction.Left;

            this.ExecuteTestGetDN(transform, "cn=Test user, ou=Organisation", "ou=Organisation");
        }

        private void ExecuteTestGetDN(GetDNComponentTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
