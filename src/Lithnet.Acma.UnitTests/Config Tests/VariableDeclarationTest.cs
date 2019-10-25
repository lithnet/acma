using Lithnet.Acma;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;
using System.Linq;
using System.Collections.Generic;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for VariableDeclarationTest and is intended
    ///to contain all VariableDeclarationTest Unit Tests
    ///</summary>
    [TestClass()]
    public class VariableDeclarationTest
    {
        public VariableDeclarationTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void ExpandVariableTestUtcDate()
        {
            string declaration = "%utcdate%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            DateTime expectedDt = DateTime.UtcNow;
            DateTime actualDt = (DateTime)target.Expand().First();

            Assert.IsTrue((actualDt.ToShortDateString() == expectedDt.ToShortDateString()) && (actualDt.ToShortTimeString() == expectedDt.ToShortTimeString()));
        }

        [TestMethod()]
        public void ExpandVariableTestDate()
        {
            string declaration = "%date%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            DateTime expectedDt = DateTime.Now;
            DateTime actualDt = (DateTime)target.Expand().First();

            Assert.IsTrue((actualDt.ToShortDateString() == expectedDt.ToShortDateString()) && (actualDt.ToShortTimeString() == expectedDt.ToShortTimeString()));
        }

        [TestMethod()]
        public void ExpandVariableTestNumber()
        {
            string declaration = "%n%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            int expected = 1;
            int actual = (int)target.Expand().First();
            Assert.AreEqual(expected, actual);

            expected = 2;
            actual = (int)target.Expand().First();
            Assert.AreEqual(expected, actual);
        }
      
        [TestMethod()]
        public void ExpandVariableTestOptionalNumber()
        {
            string declaration = "%o%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            string expected = null;
            string actual = target.Expand().First().ToSmartStringOrNull();
            Assert.AreEqual(expected, actual);

            expected = "1";
            actual = target.Expand().First().ToSmartStringOrNull();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ExpandVariableTestRandomString()
        {
            string declaration = "%randstring:10%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            string actual = (string)target.Expand().First();
            
            if (actual.Length != 10)
            {
                Assert.Fail("The string was not of the expected length");
            }
        }

        [TestMethod()]
        public void ExpandVariableTestRandomAlphaLCaseString()
        {
            string declaration = "%randstringalphalcase:10%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            string actual = (string)target.Expand().First();

            if (actual.Length != 10)
            {
                Assert.Fail("The string was not of the expected length");
            }

            if (!actual.All(char.IsLower))
            {
                Assert.Fail("The string contained non-lowercase letters");
            }
        }

        [TestMethod()]
        public void ExpandVariableTestRandomNumberFixedLength4()
        {
            string declaration = "%randnum:4%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            long actual = (long)target.Expand().First();

            if (actual == 0)
            {
                Assert.Fail("The value was not created");
            }
        }

        [TestMethod()]
        public void ExpandVariableTestRandomNumberFixedLength8()
        {
            string declaration = "%randnum:8%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            long actual = (long)target.Expand().First();

            if (actual == 0)
            {
                Assert.Fail("The value was not created");
            }
        }

        [TestMethod()]
        public void ExpandVariableTestRandomNumber()
        {
            string declaration = "%randnum%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            long actual = (long)target.Expand().First();

            if (actual == 0)
            {
                Assert.Fail("The value was not created");
            }
        }

        [TestMethod()]
        public void ExpandVariableTestRandomNumberTestClash()
        {
            this.ExpandVariableTestClash<long>("%randnum%");
        }

        [TestMethod()]
        public void ExpandVariableTestRandomStringTestClash()
        {
            this.ExpandVariableTestClash<string>("%randstringalphalcase:10%");
        }

        [TestMethod()]
        public void ExpandVariableTestRandomStringTestClash1()
        {
            this.ExpandVariableTestClash<string>("%randstring:15%");
        }

        [TestMethod()]
        public void ExpandVariableTestRandomStringTestClash2()
        {
            this.ExpandVariableTestClash<string>("%randstring:10%");
        }

        public void ExpandVariableTestClash<T>(string declaration)
        {
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            HashSet<T> values = new HashSet<T>();

            int clashCount = 0;
            int runs = 1000000;

            for (int i = 0; i < runs; i++)
            {
                T actual = (T)target.Expand().First();

                if (!values.Add(actual))
                {
                    clashCount++;
                }
            }

            if (clashCount > 0)
            {
                Assert.Fail($"Duplicate values created in {clashCount} out of {runs} runs");
            }
        }
    }
}
