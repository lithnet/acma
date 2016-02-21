
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using Lithnet.Acma;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MetadirectoryServices;
using System.IO;
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
    public class LdifAttributeValuePairTest
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
        public void TestAttributeValueWithOption()
        {
            string line = "dn;en-au: test";
            LdifAttributeValuePair kvp = new LdifAttributeValuePair(line);

            if (kvp.Name != "dn")
            {
                Assert.Fail("The key was not set correctly");
            }

            if (!kvp.Options.ContainsSameElements(new List<string>() {"en-au"}))
            {
                Assert.Fail("The options were not set correctly");
            }

            if (kvp.Value.ToString() != "test")
            {
                Assert.Fail("The value was not set correctly");
            }
        }

        [TestMethod()]
        public void TestAttributeValueWithOptions()
        {
            string line = "dn;en-au;mytest: test";
            LdifAttributeValuePair kvp = new LdifAttributeValuePair(line);

            if (kvp.Name != "dn")
            {
                Assert.Fail("The key was not set correctly");
            }

            if (!kvp.Options.ContainsSameElements(new List<string>() { "en-au" , "mytest" })) 
            {
                Assert.Fail("The options were not set correctly");
            }

            if (kvp.Value.ToString() != "test")
            {
                Assert.Fail("The value was not set correctly");
            }
        }

        [TestMethod()]
        public void TestAttributeValueWithOptionsWithSpaces()
        {
            string line = "dn; en-au; mytest : test";
            LdifAttributeValuePair kvp = new LdifAttributeValuePair(line);

            if (kvp.Name != "dn")
            {
                Assert.Fail("The key was not set correctly");
            }

            if (!kvp.Options.ContainsSameElements(new List<string>() { "en-au", "mytest" }))
            {
                Assert.Fail("The options were not set correctly");
            }

            if (kvp.Value.ToString() != "test")
            {
                Assert.Fail("The value was not set correctly");
            }
        }


        [TestMethod()]
        public void TestAttributeValueWithSingleLeadingSpace()
        {
            string line = "dn: test";
            LdifAttributeValuePair kvp = new LdifAttributeValuePair(line);

            if (kvp.Name != "dn")
            {
                Assert.Fail("The key was not set correctly");
            }

            if (kvp.Value.ToString() != "test")
            {
                Assert.Fail("The value was not set correctly");
            }
        }

        [TestMethod()]
        public void TestAttributeValueWithNoLeadingSpace()
        {
            string line = "dn:test";
            LdifAttributeValuePair kvp = new LdifAttributeValuePair(line);

            if (kvp.Name != "dn")
            {
                Assert.Fail("The key was not set correctly");
            }

            if (kvp.Value.ToString() != "test")
            {
                Assert.Fail("The value was not set correctly");
            }
        }


        [TestMethod()]
        public void TestAttributeValueWithDoubleLeadingSpace()
        {
            string line = "dn:  test";
            LdifAttributeValuePair kvp = new LdifAttributeValuePair(line);

            if (kvp.Name != "dn")
            {
                Assert.Fail("The key was not set correctly");
            }

            if (kvp.Value.ToString() != "test")
            {
                Assert.Fail("The value was not set correctly");
            }
        }

        [TestMethod()]
        public void TestAttributeValueWithNoValue()
        {
            string line = "dn:";
            LdifAttributeValuePair kvp = new LdifAttributeValuePair(line);

            if (kvp.Name != "dn")
            {
                Assert.Fail("The key was not set correctly");
            }

            if (kvp.Value != null)
            {
                Assert.Fail("The value was not set correctly");
            }
        }


        [TestMethod()]
        public void TestAttributeValueWithEmptyValue1()
        {
            string line = "dn: ";
            LdifAttributeValuePair kvp = new LdifAttributeValuePair(line);

            if (kvp.Name != "dn")
            {
                Assert.Fail("The key was not set correctly");
            }

            if (kvp.Value != null)
            {
                Assert.Fail("The value was not set correctly");
            }
        }

        [TestMethod()]
        public void TestAttributeValueWithEmptyValue2()
        {
            string line = "dn:  ";
            LdifAttributeValuePair kvp = new LdifAttributeValuePair(line);

            if (kvp.Name != "dn")
            {
                Assert.Fail("The key was not set correctly");
            }

            if (kvp.Value != null)
            {
                Assert.Fail("The value was not set correctly");
            }
        }


        [TestMethod()]
        public void TestReadFoldedLine()
        {
            string source = "dn: my\n  test string ";
            System.IO.TextReader reader = new StringReader(source);

            string line = LdifReader.GetNextDataRow(reader, true, true);

            if (line != "dn: my test string ")
            {
                Assert.Fail("The key was not set correctly");
            }
        }

        [TestMethod()]
        public void TestReadLeadingEmptyLine()
        {
            string source = "\ndn: my test string ";
            System.IO.TextReader reader = new StringReader(source);

            string line = LdifReader.GetNextDataRow(reader, true, true);

            if (line != null)
            {
                Assert.Fail("The key was not set correctly");
            }
        }

        [TestMethod()]
        public void TestReadNonFoldedLine()
        {
            string source = "dn: my\ntest string ";
            System.IO.TextReader reader = new StringReader(source);

            string line = LdifReader.GetNextDataRow(reader, true, true);

            if (line != "dn: my")
            {
                Assert.Fail("The key was not set correctly");
            }
        }

        [TestMethod()]
        public void TestReadMultipleFoldedLines1()
        {
            string source = "dn: my\n  test\n  string ";
            System.IO.TextReader reader = new StringReader(source);

            string line = LdifReader.GetNextDataRow(reader, true, true);

            if (line != "dn: my test string ")
            {
                Assert.Fail("The key was not set correctly");
            }
        }

        [TestMethod()]
        public void TestReadMultipleFoldedLines2()
        {
            string source = "dn: my\n  test\n  str\n ing ";
            System.IO.TextReader reader = new StringReader(source);

            string line = LdifReader.GetNextDataRow(reader, true, true);

            if (line != "dn: my test string ")
            {
                Assert.Fail("The key was not set correctly");
            }
        }

        [TestMethod()]
        public void TestReadMultipleFoldedLines3()
        {
            string source = "dn: my\n  test\n   str\n ing ";
            System.IO.TextReader reader = new StringReader(source);

            string line = LdifReader.GetNextDataRow(reader, true, true);

            if (line != "dn: my test  string ")
            {
                Assert.Fail("The key was not set correctly");
            }
        }


        [TestMethod()]
        public void TestReadRemaingValuePairs()
        {
            string source = "dn: my test string\nobjectClass: user\ncn:ryan\nsn:newington\n";
            System.IO.TextReader reader = new StringReader(source);

            IEnumerable<LdifAttributeValuePair> pairs = LdifReader.GetRemainingValuePairsInEntry(reader);

            if (!pairs.Select(t => t.Value).ContainsSameElements(new List<string>() { "my test string", "user", "ryan", "newington" }))
            {
                Assert.Fail("The elements were not returned correctly");
            }
        }


        [TestMethod()]
        public void TestReadRemaingValuePairsWithNoEndingCrLf()
        {
            string source = "dn: my test string\nobjectClass: user\ncn:ryan\nsn:newington";
            System.IO.TextReader reader = new StringReader(source);

            IEnumerable<LdifAttributeValuePair> pairs = LdifReader.GetRemainingValuePairsInEntry(reader);

            if (!pairs.Select(t => t.Value).ContainsSameElements(new List<string>() { "my test string", "user", "ryan", "newington" }))
            {
                Assert.Fail("The elements were not returned correctly");
            }
        }


        [TestMethod()]
        public void TestReadRemaingValuePairsWithSecondEntry()
        {
            string source = "dn: my test string\nobjectClass: user\ncn:ryan\nsn:newington\n\nmail:ryan.newington@acma.com";
            System.IO.TextReader reader = new StringReader(source);

            IEnumerable<LdifAttributeValuePair> pairs = LdifReader.GetRemainingValuePairsInEntry(reader);

            if (!pairs.Select(t => t.Value).ContainsSameElements(new List<string>() { "my test string", "user", "ryan", "newington" }))
            {
                Assert.Fail("The elements were not returned correctly");
            }
        }
    }
}