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
    public class AttributeDeclarationParserTests
    {
        public AttributeDeclarationParserTests()
        {
            UnitTestControl.Initialize();
        }

        private void Reset()
        {
            UniqueIDCache.ClearIdCache();
            ActiveConfig.XmlConfig.Transforms.Clear();
        }

        [TestMethod]
        public void TestSingleAttribute()
        {
            this.Reset();
            string declaration = "{mail}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
        }

        [TestMethod]
        public void TestSingleAttributeWithOptionalTextLeft()
        {
            this.Reset();
            string declaration = "[My mail address {mail}]";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual("My mail address ", t.PreText);
            Assert.AreEqual("", t.PostText);
            Assert.AreEqual(HologramView.Proposed, t.View);
            Assert.AreEqual(ActiveConfig.DB.GetAttribute("mail"), t.Attribute);
        }

        [TestMethod]
        public void TestSingleAttributeWithOptionalTextRight()
        {
            this.Reset();
            string declaration = "[{mail} is my mail address]";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual("", t.PreText);
            Assert.AreEqual(" is my mail address", t.PostText);
            Assert.AreEqual(HologramView.Proposed, t.View);
            Assert.AreEqual(ActiveConfig.DB.GetAttribute("mail"), t.Attribute);
        }

        [TestMethod]
        public void TestSingleAttributeWithOptionalTextLeftRight()
        {
            this.Reset();
            string declaration = "[This address '{mail}' is my mail address]";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual("This address '", t.PreText);
            Assert.AreEqual("' is my mail address", t.PostText);
            Assert.AreEqual(HologramView.Proposed, t.View);
            Assert.AreEqual(ActiveConfig.DB.GetAttribute("mail"), t.Attribute);
        }

        [TestMethod]
        public void TestCurrentAttributeWithOptionalTextLeftRight()
        {
            this.Reset();
            string declaration = "[This address '{#mail}' is my mail address]";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual("This address '", t.PreText);
            Assert.AreEqual("' is my mail address", t.PostText);
            Assert.AreEqual(HologramView.Current, t.View);
            Assert.AreEqual(ActiveConfig.DB.GetAttribute("mail"), t.Attribute);
        }

        [TestMethod]
        public void TestSingleAttributeWithTransformAndOptionalTextLeftRight()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "[This address '{mail>>test1}' is my mail address]";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual("This address '", t.PreText);
            Assert.AreEqual("' is my mail address", t.PostText);
            Assert.AreEqual(HologramView.Proposed, t.View);
            Assert.AreEqual(ActiveConfig.DB.GetAttribute("mail"), t.Attribute);
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1" });
        }

        [TestMethod]
        public void TestSingleAttributeWithMultipleTransformAndOptionalTextLeftRight()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test3" });
            string declaration = "[This address '{mail>>test1>>test2>>test3}' is my mail address]";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual("This address '", t.PreText);
            Assert.AreEqual("' is my mail address", t.PostText);
            Assert.AreEqual(HologramView.Proposed, t.View);
            Assert.AreEqual(ActiveConfig.DB.GetAttribute("mail"), t.Attribute);
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", "test2", "test3" });
        }

        [TestMethod]
        public void TestSingleReferenceAttributeWithOptionalText()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "[This is the direct reports {directReports->mail} mail]";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            Assert.AreEqual("This is the direct reports ", t.PreText);
            Assert.AreEqual(" mail", t.PostText);
            CollectionAssert.AreEqual(t.EvaluationPath.ToList(), new List<AcmaSchemaAttribute>() { ActiveConfig.DB.GetAttribute("directReports") });
        }

        [TestMethod]
        public void TestSingleReferenceAttributeWithOptionalTextAndTransforms()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            string declaration = "[This is the direct reports {directReports->mail>>test1>>test2} mail]";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            Assert.AreEqual("This is the direct reports ", t.PreText);
            Assert.AreEqual(" mail", t.PostText);
            CollectionAssert.AreEqual(t.EvaluationPath.ToList(), new List<AcmaSchemaAttribute>() { ActiveConfig.DB.GetAttribute("directReports") });
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", "test2"});

        }

        [TestMethod]
        public void TestThreeReferenceAttributesWithThreeTransformsAndOptionalText()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test3" });
            string declaration = "[Test {directReports->supervisor->directReports->mail>>test1>>test2>>test3} test]";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual("Test ", t.PreText);
            Assert.AreEqual(" test", t.PostText);
            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            CollectionAssert.AreEqual(t.EvaluationPath.ToList(), new List<AcmaSchemaAttribute>()
             { 
                    ActiveConfig.DB.GetAttribute("directReports"),
                    ActiveConfig.DB.GetAttribute("supervisor") ,
                    ActiveConfig.DB.GetAttribute("directReports")

             });

            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", "test2", "test3" });

        }


        [TestMethod]
        public void TestSingleAttributeCurrentView()
        {
            this.Reset();
            string declaration = "{#mail}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Current);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
        }

        [TestMethod]
        public void TestSingleAttributeWithSingleTransform()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "{mail>>test1}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", });
        }

        [TestMethod]
        public void TestSingleReferenceAttribute()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "{directReports->mail}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            CollectionAssert.AreEqual(t.EvaluationPath.ToList(), new List<AcmaSchemaAttribute>() { ActiveConfig.DB.GetAttribute("directReports") });
        }
        
        [TestMethod]
        public void TestSingleReferenceAttributeWithSingleTransform()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "{directReports->mail>>test1}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            CollectionAssert.AreEqual(t.EvaluationPath.ToList(), new List<AcmaSchemaAttribute>() { ActiveConfig.DB.GetAttribute("directReports") });
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", });
        }

        [TestMethod]
        public void TestSingleReferenceAttributeWithTwoTransforms()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            string declaration = "{directReports->mail>>test1>>test2}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            CollectionAssert.AreEqual(t.EvaluationPath.ToList(), new List<AcmaSchemaAttribute>() { ActiveConfig.DB.GetAttribute("directReports") });
            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", "test2" });
        }

        [TestMethod]
        public void TestTwoReferenceAttributes()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "{directReports->supervisor->mail}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            CollectionAssert.AreEqual(t.EvaluationPath.ToList(), new List<AcmaSchemaAttribute>() 
                { 
                    ActiveConfig.DB.GetAttribute("directReports"),
                    ActiveConfig.DB.GetAttribute("supervisor") 
                });
        }

        [TestMethod]
        public void TestThreeReferenceAttributes()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            string declaration = "{directReports->supervisor->directReports->mail}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            CollectionAssert.AreEqual(t.EvaluationPath.ToList(), new List<AcmaSchemaAttribute>()
             { 
                    ActiveConfig.DB.GetAttribute("directReports"),
                    ActiveConfig.DB.GetAttribute("supervisor") ,
                    ActiveConfig.DB.GetAttribute("directReports")

             });
        }

        [TestMethod]
        public void TestThreeReferenceAttributesWithThreeTransforms()
        {
            this.Reset();
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test1" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test2" });
            ActiveConfig.XmlConfig.Transforms.Add(new TypeConverterTransform() { ID = "test3" });
            string declaration = "{directReports->supervisor->directReports->mail>>test1>>test2>>test3}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

            if (t.HasErrors)
            {
                foreach (TokenError e in t.Errors)
                {
                    Debug.WriteLine("Error: " + e);
                }

                Assert.Fail("The tokenizer returned an unexpected error");
            }

            Assert.AreEqual(t.View, HologramView.Proposed);
            Assert.AreEqual(t.Attribute, ActiveConfig.DB.GetAttribute("mail"));
            CollectionAssert.AreEqual(t.EvaluationPath.ToList(), new List<AcmaSchemaAttribute>()
             { 
                    ActiveConfig.DB.GetAttribute("directReports"),
                    ActiveConfig.DB.GetAttribute("supervisor") ,
                    ActiveConfig.DB.GetAttribute("directReports")

             });

            CollectionAssert.AreEqual(t.Transforms.Select(u => u.ID).ToList(), new List<string>() { "test1", "test2", "test3" });

        }

        [TestMethod]
        public void TestMissingAttribute()
        {
            this.Reset();

            string declaration = "{missingAttribute}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestMissingOpenBrace()
        {
            this.Reset();

            string declaration = "mail}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestMissingClosingBrace()
        {
            this.Reset();

            string declaration = "{mail";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestMissingAttributeName()
        {
            this.Reset();

            string declaration = "{}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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

            string declaration = "{";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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

            string declaration = "{ ";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestOnlyEndingElement()
        {
            this.Reset();

            string declaration = "}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestBadReferenceOperator()
        {
            this.Reset();

            string declaration = "{directReports-!mail}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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

            string declaration = "{directReports>!mail}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestNonReferenceAttribute()
        {
            this.Reset();

            string declaration = "{mail->sn}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestNonReferenceAttributeInChain()
        {
            this.Reset();

            string declaration = "{directReports->supervisor->mail->sn}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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

            string declaration = "{sn>>noTransform}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestMissingTransformInReferenceChain()
        {
            this.Reset();

            string declaration = "{directReports->sn>>noTransform}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestInvalidSymbolInReferenceChain()
        {
            this.Reset();

            string declaration = "{directReports->?sn}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestViewModifierInReferenceChain()
        {
            this.Reset();

            string declaration = "{directReports->#sn}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration);

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
        public void TestSingleAttributeNotInObjectClass()
        {
            this.Reset();
            string declaration = "{sn}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration, ActiveConfig.DB.GetObjectClass("group"));

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
        public void TestReferenceAttributeNotInObjectClass()
        {
            this.Reset();
            string declaration = "{shadowParent->sn}";

            AttributeDeclarationParser t = new AttributeDeclarationParser(declaration, ActiveConfig.DB.GetObjectClass("group"));

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
