using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Fim.Transforms.UnitTests
{
    [TestClass()]
    public class TransformKeyedCollectionTest
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

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            TransformKeyedCollection objectToSerialize = new TransformKeyedCollection();
            StringCaseTransform transform1 = new StringCaseTransform();
            transform1.ID = "tx0001";
            transform1.StringCase = StringCaseType.Title;

            GetDNComponentTransform transform2 = new GetDNComponentTransform();
            transform2.ID = "tx0002";
            transform2.RdnFormat = RdnFormat.ValueOnly;
            transform2.ComponentIndex = 1;

            objectToSerialize.Add(transform1);
            objectToSerialize.Add(transform2);
            UniqueIDCache.ClearIdCache();

            TransformKeyedCollection deserializedObject = (TransformKeyedCollection)UnitTestControl.XmlSerializeRoundTrip<TransformKeyedCollection>(objectToSerialize);

            Assert.AreEqual(objectToSerialize.Count, deserializedObject.Count);
            Assert.AreEqual(objectToSerialize[0].ID, transform1.ID);
            Assert.AreEqual(objectToSerialize[1].ID, transform2.ID);
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }
    }
}
