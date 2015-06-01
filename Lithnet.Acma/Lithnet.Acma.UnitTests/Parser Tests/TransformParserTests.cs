using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.Acma;
using Lithnet.Fim.Transforms;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Lithnet.Common.ObjectModel;
using System.Linq;
using Lithnet.Fim.Core;

namespace Lithnet.Acma.UnitTests
{
    [TestClass]
    public class TransformParserTests
    {
        public TransformParserTests()
        {
            UnitTestControl.Initialize();
        }

        private void Reset()
        {
            UniqueIDCache.ClearIdCache();
            ActiveConfig.XmlConfig.Transforms.Clear();
        }

        [TestMethod]
        public void TestSingleTransform()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test" });
            string transformString = "test";

            TransformParser t = new TransformParser(transformString);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test" });
        }

        [TestMethod]
        public void TestSingleTransformWithDigit()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string transformString = "test1";

            TransformParser t = new TransformParser(transformString);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1" });
        }

        [TestMethod]
        public void TestTwoTransforms()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            string transformString = "test1>>test2";

            TransformParser t = new TransformParser(transformString);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1" , "test2"});
        }

        [TestMethod]
        public void TestTwoTransformsWithInvalidName()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test" });
            string transformString = "test1";

            TransformParser t = new TransformParser(transformString);

            foreach (TokenError e in t.Errors)
            {
                Debug.WriteLine("Error: " + e);
            }

            if (!t.HasErrors)
            {
                Assert.Fail("The tokenizer did not return an error");
            }
        }

        [TestMethod]
        public void TestTwoTransformsWithInvalidOperator1()
        {
            this.Reset();

            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            string transformString = "test1>test2";

            TransformParser t = new TransformParser(transformString);

            foreach (TokenError e in t.Errors)
            {
                Debug.WriteLine("Error: " + e);
            }

            if (!t.HasErrors)
            {
                Assert.Fail("The tokenizer did not return an error");
            }
        }

        [TestMethod]
        public void TestThreeTransformsWithInvalidOperator1()
        {
            this.Reset();

            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test3" });
            string transformString = "test1>>test2>test3";

            TransformParser t = new TransformParser(transformString);

            foreach (TokenError e in t.Errors)
            {
                Debug.WriteLine("Error: " + e);
            }

            if (!t.HasErrors)
            {
                Assert.Fail("The tokenizer did not return an error");
            }
        }

        [TestMethod]
        public void TestThreeTransformsWithMissingTransform()
        {
            this.Reset();

            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test3" });
            string transformString = "test1>>test2>>test4";

            TransformParser t = new TransformParser(transformString);

            foreach (TokenError e in t.Errors)
            {
                Debug.WriteLine("Error: " + e);
            }

            if (!t.HasErrors)
            {
                Assert.Fail("The tokenizer did not return an error");
            }
        }


        [TestMethod]
        public void TestTwoTransformsWithInvalidOperator2()
        {
            this.Reset();

            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            string transformString = "test1!>test2";

            TransformParser t = new TransformParser(transformString);

            foreach (TokenError e in t.Errors)
            {
                Debug.WriteLine("Error: " + e);
            }

            if (!t.HasErrors)
            {
                Assert.Fail("The tokenizer did not return an error");
            }
        }

        [TestMethod]
        public void TestTwoTransformsWithInvalidOperator3()
        {
            this.Reset();

            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            string transformString = "test1 test2";

            TransformParser t = new TransformParser(transformString);

            foreach (TokenError e in t.Errors)
            {
                Debug.WriteLine("Error: " + e);
            }

            if (!t.HasErrors)
            {
                Assert.Fail("The tokenizer did not return an error");
            }
        }
    }
}
