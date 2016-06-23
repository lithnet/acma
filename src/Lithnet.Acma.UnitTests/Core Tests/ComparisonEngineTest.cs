using Lithnet.Acma;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Lithnet.MetadirectoryServices;
using Lithnet.Transforms;
using Lithnet.Acma.DataModel;

namespace Lithnet.Acma.UnitTests
{
    /// <summary>
    ///This is a test class for ComparisonEngineTest and is intended
    ///to contain all ComparisonEngineTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ComparisonEngineTest
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

        /// <summary>
        ///A test for CompareBinary
        ///</summary>
        [TestMethod()]
        public void CompareBinaryEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.Equals;
            byte[] actualValue = new byte[] { 0, 1, 2, 3 };
            byte[] matchingValue = new byte[] { 0, 1, 2, 3 };
            byte[] nonMatchingValue = new byte[] { 0, 1, 2, 4 };

            Assert.AreEqual(true, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareBinary(actualValue, nonMatchingValue, valueOperator));

            actualValue = null;
            Assert.AreEqual(false, ComparisonEngine.CompareBinary(actualValue, nonMatchingValue, valueOperator));

            matchingValue = null;
            Assert.AreEqual(true, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareBinaryPresenceTest()
        {
            ValueOperator valueOperator = ValueOperator.IsPresent;
            byte[] actualValue = new byte[] { 0, 1, 2, 3 };
            byte[] matchingValue = new byte[] { 0, 1, 2, 3 };
            byte[] nonMatchingValue = new byte[] { 0, 1, 2, 4 };

            Assert.AreEqual(true, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));

            actualValue = null;
            Assert.AreEqual(false, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareBinaryNoPresenceTest()
        {
            ValueOperator valueOperator = ValueOperator.NotPresent;
            byte[] actualValue = null;
            byte[] matchingValue = new byte[] { 0, 1, 2, 3 };
            byte[] nonMatchingValue = new byte[] { 0, 1, 2, 4 };

            Assert.AreEqual(true, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));

            actualValue = new byte[] { 0, 1, 2, 3, 4 };
            Assert.AreEqual(false, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareBinaryFromStringEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.Equals;
            string actualValue = "AAECAwQ=";
            string matchingValue = "AAECAwQ=";
            string nonMatchingValue = "AAACAwQ=";

            Assert.AreEqual(true, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareBinary(actualValue, nonMatchingValue, valueOperator));
        }

        /// <summary>
        ///A test for CompareBinary
        ///</summary>
        [TestMethod()]
        public void CompareBinaryNotEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.NotEquals;
            byte[] actualValue = new byte[] { 0, 1, 2, 3 };
            byte[] matchingValue = new byte[] { 0, 1, 2, 4 };
            byte[] nonMatchingValue = new byte[] { 0, 1, 2, 3 };

            Assert.AreEqual(true, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareBinary(actualValue, nonMatchingValue, valueOperator));

            matchingValue = null;
            Assert.AreEqual(true, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));

            actualValue = null;
            Assert.AreEqual(false, ComparisonEngine.CompareBinary(actualValue, matchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareStringIsPresentTest()
        {
            ValueOperator valueOperator = ValueOperator.IsPresent;
            string actualValue = "abcd";
            string matchingValue = null;
            string nonMatchingValue = null;

            Assert.AreEqual(true, ComparisonEngine.CompareString(actualValue, matchingValue, valueOperator));

            actualValue = null;
            Assert.AreEqual(false, ComparisonEngine.CompareString(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareStringNotPresentTest()
        {
            ValueOperator valueOperator = ValueOperator.NotPresent;
            string actualValue = null;
            string matchingValue = null;
            string nonMatchingValue = null;

            Assert.AreEqual(true, ComparisonEngine.CompareString(actualValue, matchingValue, valueOperator));

            actualValue = "abds";
            Assert.AreEqual(false, ComparisonEngine.CompareString(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareStringEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.Equals;
            string actualValue = "abcd";
            string matchingValue = "abcd";
            string nonMatchingValue = "defg";

            Assert.AreEqual(true, ComparisonEngine.CompareString(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareString(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareStringNotEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.NotEquals;
            string actualValue = "abcd";
            string matchingValue = "defg";
            string nonMatchingValue = "abcd";

            Assert.AreEqual(true, ComparisonEngine.CompareString(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareString(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareStringContainsTest()
        {
            ValueOperator valueOperator = ValueOperator.Contains;
            string actualValue = "abcdefghijklmnop";
            string matchingValue = "cdef";
            string nonMatchingValue = "zzzz";

            Assert.AreEqual(true, ComparisonEngine.CompareString(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareString(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareStringNotContainsTest()
        {
            ValueOperator valueOperator = ValueOperator.NotContains;
            string actualValue = "abcdefghijklmnop";
            string matchingValue = "zzzz";
            string nonMatchingValue = "cdef";

            Assert.AreEqual(true, ComparisonEngine.CompareString(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareString(actualValue, nonMatchingValue, valueOperator));
        }
        
        [TestMethod()]
        public void CompareStringStartsWithTest()
        {
            ValueOperator valueOperator = ValueOperator.StartsWith;
            string actualValue = "abcdefghijklmnop";
            string matchingValue = "abcd";
            string nonMatchingValue = "zzz";

            Assert.AreEqual(true, ComparisonEngine.CompareString(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareString(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareStringEndsWithTest()
        {
            ValueOperator valueOperator = ValueOperator.EndsWith;
            string actualValue = "abcdefghijklmnop";
            string matchingValue = "mnop";
            string nonMatchingValue = "zzz";

            Assert.AreEqual(true, ComparisonEngine.CompareString(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareString(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareLongEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.Equals;
            long actualValue = 1234567890L;
            long matchingValue = 1234567890L;
            long nonMatchingValue = 9876543210L;

            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareLong(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareLongFromStringEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.Equals;
            string actualValue = "1234567890";
            string matchingValue = "1234567890";
            string nonMatchingValue = "9876543210";

            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareLong(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareLongNotEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.NotEquals;
            long actualValue = 1234567890L;
            long matchingValue = 9876543210L;
            long nonMatchingValue = 1234567890L ;

            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareLong(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareLongGreaterThanTest()
        {
            ValueOperator valueOperator = ValueOperator.GreaterThan;
            long actualValue = 20L;
            long matchingValue = 10L;
            long nonMatchingValue = 30L;

            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareLong(actualValue, nonMatchingValue, valueOperator));
        }
        
        [TestMethod()]
        public void CompareLongGreaterThanOrEqTest()
        {
            ValueOperator valueOperator = ValueOperator.GreaterThanOrEq;
            long actualValue = 20L;
            long matchingValue = 10L;
            long nonMatchingValue = 30L;

            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));
            
            matchingValue = 20L;
            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));

            Assert.AreEqual(false, ComparisonEngine.CompareLong(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareLongLessThanTest()
        {
            ValueOperator valueOperator = ValueOperator.LessThan;
            long actualValue = 10L;
            long matchingValue = 20L;
            long nonMatchingValue = 5L;

            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareLong(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareLongLessThanOrEqTest()
        {
            ValueOperator valueOperator = ValueOperator.LessThanOrEq;
            long actualValue = 10L;
            long matchingValue = 20L;
            long nonMatchingValue = 5L;

            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));

            matchingValue = 10L;
            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));

            Assert.AreEqual(false, ComparisonEngine.CompareLong(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareLongAndTest()
        {
            ValueOperator valueOperator = ValueOperator.And;
            long actualValue = 514L;
            long matchingValue = 2L;
            long nonMatchingValue = 5L;

            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));
            
            Assert.AreEqual(false, ComparisonEngine.CompareLong(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareLongOrTest()
        {
            ValueOperator valueOperator = ValueOperator.Or;
            long actualValue = 514L;
            long matchingValue = 2L;
            long nonMatchingValue = 5L;

            Assert.AreEqual(true, ComparisonEngine.CompareLong(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareLong(actualValue, nonMatchingValue, valueOperator));
        }



        [TestMethod()]
        public void CompareDateTimeEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.Equals;
            DateTime actualValue = DateTime.Parse("1/1/2000");
            DateTime matchingValue = DateTime.Parse("1/1/2000");
            DateTime nonMatchingValue = DateTime.Parse("1/1/2001");

            Assert.AreEqual(true, ComparisonEngine.CompareDateTime(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareDateTime(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareDateTimeNotEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.NotEquals;
            DateTime actualValue = DateTime.Parse("1/1/2000");
            DateTime matchingValue = DateTime.Parse("1/1/2001");
            DateTime nonMatchingValue = DateTime.Parse("1/1/2000");

            Assert.AreEqual(true, ComparisonEngine.CompareDateTime(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareDateTime(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareDateTimeGreaterThanTest()
        {
            ValueOperator valueOperator = ValueOperator.GreaterThan;
            DateTime actualValue = DateTime.Parse("1/1/2000");
            DateTime matchingValue = DateTime.Parse("1/1/1999");
            DateTime nonMatchingValue = DateTime.Parse("1/1/2001");

            Assert.AreEqual(true, ComparisonEngine.CompareDateTime(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareDateTime(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareDateTimeGreaterThanOrEqTest()
        {
            ValueOperator valueOperator = ValueOperator.GreaterThanOrEq;
            DateTime actualValue = DateTime.Parse("1/1/2000");
            DateTime matchingValue = DateTime.Parse("1/1/1999");
            DateTime nonMatchingValue = DateTime.Parse("1/1/2001");

            Assert.AreEqual(true, ComparisonEngine.CompareDateTime(actualValue, matchingValue, valueOperator));

            matchingValue = DateTime.Parse("1/1/2000");
            Assert.AreEqual(true, ComparisonEngine.CompareDateTime(actualValue, matchingValue, valueOperator));

            Assert.AreEqual(false, ComparisonEngine.CompareDateTime(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareDateTimeLessThanTest()
        {
            ValueOperator valueOperator = ValueOperator.LessThan;
            DateTime actualValue = DateTime.Parse("1/1/1999");
            DateTime matchingValue = DateTime.Parse("1/1/2000");
            DateTime nonMatchingValue = DateTime.Parse("1/1/1989");

            Assert.AreEqual(true, ComparisonEngine.CompareDateTime(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareDateTime(actualValue, nonMatchingValue, valueOperator));
        }

        [TestMethod()]
        public void CompareDateTimeLessThanOrEqTest()
        {
            ValueOperator valueOperator = ValueOperator.LessThanOrEq;
            DateTime actualValue = DateTime.Parse("1/1/1999");
            DateTime matchingValue = DateTime.Parse("1/1/2000");
            DateTime nonMatchingValue = DateTime.Parse("1/1/1989");

            Assert.AreEqual(true, ComparisonEngine.CompareDateTime(actualValue, matchingValue, valueOperator));

            matchingValue = DateTime.Parse("1/1/1999");
            Assert.AreEqual(true, ComparisonEngine.CompareDateTime(actualValue, matchingValue, valueOperator));

            Assert.AreEqual(false, ComparisonEngine.CompareDateTime(actualValue, nonMatchingValue, valueOperator));
        }



        /// <summary>
        ///A test for CompareBoolean
        ///</summary>
        [TestMethod()]
        public void CompareBooleanEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.Equals;
            bool actualValue = true;
            bool matchingValue = true;
            bool nonMatchingValue = false;

            Assert.AreEqual(true, ComparisonEngine.CompareBoolean(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareBoolean(actualValue, nonMatchingValue, valueOperator));
        }

        /// <summary>
        ///A test for CompareBoolean
        ///</summary>
        [TestMethod()]
        public void CompareBooleanFromStringEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.Equals;
            string actualValue = "true";
            string matchingValue = "true";
            string nonMatchingValue = "false";

            Assert.AreEqual(true, ComparisonEngine.CompareBoolean(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareBoolean(actualValue, nonMatchingValue, valueOperator));
        }


        /// <summary>
        ///A test for CompareBoolean
        ///</summary>
        [TestMethod()]
        public void CompareBooleanNotEqualsTest()
        {
            ValueOperator valueOperator = ValueOperator.NotEquals;
            bool actualValue = true;
            bool matchingValue = false;
            bool nonMatchingValue = true;

            Assert.AreEqual(true, ComparisonEngine.CompareBoolean(actualValue, matchingValue, valueOperator));
            Assert.AreEqual(false, ComparisonEngine.CompareBoolean(actualValue, nonMatchingValue, valueOperator));
        }
    }
}
