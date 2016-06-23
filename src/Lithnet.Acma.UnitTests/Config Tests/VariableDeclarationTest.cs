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
        }

        /// <summary>
        ///A test for ExpandVariable
        ///</summary>
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
            int actual;
            actual = (int)target.Expand().First();
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
            int expected = 1;
            int actual;
            actual = (int)target.Expand().First();
            Assert.AreEqual(expected, actual);

            expected = 2;
            actual = (int)target.Expand().First();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod()]
        public void ExpandVariableTestRandomString()
        {
            string declaration = "%randstring:10%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            string actual;
            actual = (string)target.Expand().First();
            
            if (actual.Length != 10)
            {
                Assert.Fail("The string was not of the expected length");
            }
        }

        [TestMethod()]
        public void ExpandVariableTestRandomNumberFixedLength4()
        {
            string declaration = "%randnum:4%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            long actual;
            actual = (long)target.Expand().First();

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
            long actual;
            actual = (long)target.Expand().First();

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
            long actual;
            actual = (long)target.Expand().First();

            if (actual == 0)
            {
                Assert.Fail("The value was not created");
            }
        }

        [TestMethod()]
        public void ExpandVariableTestRandomNumberTestClash()
        {
            string declaration = "%randnum%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            long actual;
            HashSet<long> values = new HashSet<long>();

            for (int i = 0; i < 1000000; i++)
            {
                actual = (long)target.Expand().First();

                if (!values.Add(actual))
                {
                    Assert.Fail("Duplicate value detected");
                }
            }
        }

        [TestMethod()]
        public void ExpandVariableTestRandomStringTestClash1()
        {
            string declaration = "%randstring:15%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            string actual;
            HashSet<string> values = new HashSet<string>();

            for (int i = 0; i < 1000000; i++)
            {
                actual = (string)target.Expand().First();

                if (!values.Add(actual))
                {
                    Assert.Fail("Duplicate value detected");
                }
            }
        }

        [TestMethod()]
        public void ExpandVariableTestRandomStringTestClash2()
        {
            string declaration = "%randstring:10%";
            VariableDeclarationParser parser = new VariableDeclarationParser(declaration);
            VariableDeclaration target = parser.GetVariableDeclaration();
            string actual;
            HashSet<string> values = new HashSet<string>();

            for (int i = 0; i < 1000000; i++)
            {
                actual = (string)target.Expand().First();

                if (!values.Add(actual))
                {
                    Assert.Fail("Duplicate value detected");
                }
            }
        }
    }
}
