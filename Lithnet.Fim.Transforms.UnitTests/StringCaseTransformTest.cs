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
    ///This is a test class for StringCaseTransformTest and is intended to contain all StringCaseTransformTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StringCaseTransformTest
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
            StringCaseTransform transformToSeralize = new StringCaseTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.StringCase = StringCaseType.Title;
            UniqueIDCache.ClearIdCache();

            StringCaseTransform deserializedTransform = (StringCaseTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.StringCase, deserializedTransform.StringCase);
        }

        [ClassInitialize()]
        public static void InitializeTest(TestContext testContext)
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void StringCaseLowerTest()
        {
            StringCaseTransform transform = new StringCaseTransform();
            transform.StringCase = StringCaseType.Lower;

            this.ExecuteTest(transform, "This Is a TeSt", "this is a test");
        }

        [TestMethod()]
        public void StringCaseUpperTest()
        {
            StringCaseTransform transform = new StringCaseTransform();
            transform.StringCase = StringCaseType.Upper;

            this.ExecuteTest(transform, "This Is a TeSt", "THIS IS A TEST");
        }

        [TestMethod()]
        public void StringCaseTitleTest()
        {
            StringCaseTransform transform = new StringCaseTransform();
            transform.StringCase = StringCaseType.Title;

            this.ExecuteTest(transform, "This Is a TeSt", "This Is A Test");
        }

        private void ExecuteTest(StringCaseTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
