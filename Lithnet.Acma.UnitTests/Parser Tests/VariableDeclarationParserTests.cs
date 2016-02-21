using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lithnet.Acma;
using Lithnet.Transforms;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;
using Lithnet.Common.ObjectModel;
using System.Linq;
using Lithnet.MetadirectoryServices;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    [TestClass]
    public class VariableDeclarationParserTests
    {
        public VariableDeclarationParserTests()
        {
            UnitTestControl.Initialize();
        }

        private void Reset()
        {
            UniqueIDCache.ClearIdCache();
            ActiveConfig.XmlConfig.Transforms.Clear();
        }

        [TestMethod]
        public void TestSingleVariable()
        {
            this.Reset();
            string declaration = "%newguid%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.VariableName, "newguid");
            Assert.AreEqual(t.VariableParameter, null);
        }


        [TestMethod]
        public void TestVariableWithParameters()
        {
            this.Reset();
            string declaration = "%randstring:30%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.VariableName, "randstring");
            Assert.AreEqual(t.VariableParameter, "30");
        }

        [TestMethod]
        public void TestVariableWithSingleTransform()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "%newguid>>test1%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.VariableName, "newguid");
            Assert.AreEqual(t.VariableParameter, null);
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", });
        }

        [TestMethod]
        public void TestVariableWithParameterAndSingleTransform()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "%randstring:30>>test1%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.VariableName, "randstring");
            Assert.AreEqual(t.VariableParameter, "30");
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", });
        }

        [TestMethod]
        public void TestVariableWithTwoTransforms()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            string declaration = "%newguid>>test1>>test2%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.VariableName, "newguid");
            Assert.AreEqual(t.VariableParameter, null);
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", "test2" });
        }

        [TestMethod]
        public void TestVariableWithParameterAndTwoTransforms()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            string declaration = "%randstring:30>>test1>>test2%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.VariableName, "randstring");
            Assert.AreEqual(t.VariableParameter, "30");
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", "test2" });
        }

        [TestMethod]
        public void TestMissingAttribute()
        {
            this.Reset();

            string declaration = "%novariable%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestVariableWithoutSupportForParameters()
        {
            this.Reset();

            string declaration = "%newguid:9%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestVariableWithUnsupportedParameterValue()
        {
            this.Reset();

            string declaration = "%randstring:X%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestMissingOpenSymbol()
        {
            this.Reset();

            string declaration = "newguid%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestMissingClosingSymbol()
        {
            this.Reset();

            string declaration = "%newguid";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestMissingVariableName()
        {
            this.Reset();

            string declaration = "%%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestOnlyStartingElement()
        {
            this.Reset();

            string declaration = "%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestOnlyStartingElementAndWhiteSpace()
        {
            this.Reset();

            string declaration = "% ";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestSymbolAtClosingElement()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "%newguid>>test1!%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestOnlyWhiteSpace()
        {
            this.Reset();

            string declaration = " ";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestBadTransformOperator()
        {
            this.Reset();

            string declaration = "%newguid>!test1%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestMissingTransform()
        {
            this.Reset();

            string declaration = "%newguid>>noTransform%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
        public void TestInvalidSymbol()
        {
            this.Reset();

            string declaration = "%newguid>>?test1%";

            VariableDeclarationParser t = new VariableDeclarationParser(declaration);

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
