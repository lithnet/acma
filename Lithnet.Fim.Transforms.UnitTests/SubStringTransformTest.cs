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
    ///This is a test class for FormatStringCaseTest and is intended
    ///to contain all FormatStringCaseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SubStringTransformTest
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
            SubstringTransform transformToSeralize = new SubstringTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.Length = 5;
            transformToSeralize.PadCharacter = "d";
            transformToSeralize.PaddingType = PadType.LastCharacter;
            transformToSeralize.StartIndex = 99;
            transformToSeralize.Direction = Direction.Right;
            UniqueIDCache.ClearIdCache();

            SubstringTransform deserializedTransform = (SubstringTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.Length, deserializedTransform.Length);
            Assert.AreEqual(transformToSeralize.PadCharacter, deserializedTransform.PadCharacter);
            Assert.AreEqual(transformToSeralize.PaddingType, deserializedTransform.PaddingType);
            Assert.AreEqual(transformToSeralize.StartIndex, deserializedTransform.StartIndex);
            Assert.AreEqual(transformToSeralize.Direction, deserializedTransform.Direction);
        }


        [TestMethod()]
        public void LeftStringTransformNoPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 3;
            transform.PaddingType = PadType.None;
            transform.Direction = Direction.Left;

            this.ExecuteTestSubString(transform, "newington", "new");
        }

        [TestMethod()]
        public void LeftStringTransformLastCharacterPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.PaddingType = PadType.LastCharacter;
            transform.Direction = Direction.Left;

            this.ExecuteTestSubString(transform, "ma", "maaa");
        }

        [TestMethod()]
        public void LeftStringTransformLastCharacterPaddingEmptyString()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.PaddingType = PadType.LastCharacter;
            transform.Direction = Direction.Left;

            try
            {
                this.ExecuteTestSubString(transform, "", "");
                Assert.Fail("The expected exception was not thrown");
            }
            catch (ArgumentNullException)
            {
            }
        }

        [TestMethod()]
        public void LeftStringTransformFirstCharacterPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.PaddingType = PadType.FirstCharacter;
            transform.Direction = Direction.Left;
            this.ExecuteTestSubString(transform, "ma", "mamm");
        }

        [TestMethod()]
        public void LeftStringTransformSpecifiedCharacterPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.PaddingType = PadType.SpecifiedValue;
            transform.PadCharacter = "x";
            transform.Direction = Direction.Left;

            this.ExecuteTestSubString(transform, "ma", "maxx");
        }


        [TestMethod()]
        public void RightStringTransformNoPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 3;
            transform.PaddingType = PadType.None;
            transform.Direction = Direction.Right;

            this.ExecuteTestSubString(transform, "newington", "ton");
        }

        [TestMethod()]
        public void RightStringTransformLastCharacterPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.PaddingType = PadType.LastCharacter;
            transform.Direction = Direction.Right;

            this.ExecuteTestSubString(transform, "ma", "aama");
        }

        [TestMethod()]
        public void RightStringTransformFirstCharacterPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.PaddingType = PadType.FirstCharacter;
            transform.Direction = Direction.Right;

            this.ExecuteTestSubString(transform, "ma", "mmma");
        }

        [TestMethod()]
        public void ExecuteTestRightStringSpecifiedCharacterPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.PaddingType = PadType.SpecifiedValue;
            transform.PadCharacter = "x";
            transform.Direction = Direction.Right;

            this.ExecuteTestSubString(transform, "ma", "xxma");
        }


        [TestMethod()]
        public void SubStringTransformNoPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 3;
            transform.StartIndex = 1;
            transform.PaddingType = PadType.None;
            transform.Direction = Direction.Left;

            this.ExecuteTestSubString(transform, "newington", "ewi");
        }

        [TestMethod()]
        public void SubStringTransformLastCharacterPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.StartIndex = 1;
            transform.PaddingType = PadType.LastCharacter;
            transform.Direction = Direction.Left;

            this.ExecuteTestSubString(transform, "mai", "aiii");
        }

        [TestMethod()]
        public void SubStringTransformFirstCharacterPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.StartIndex = 1;
            transform.PaddingType = PadType.FirstCharacter;
            transform.Direction = Direction.Left;

            this.ExecuteTestSubString(transform, "mai", "aiaa");
        }

        [TestMethod()]
        public void SubStringTransformSpecifiedCharacterPadding()
        {
            SubstringTransform transform = new SubstringTransform();
            transform.Length = 4;
            transform.StartIndex = 1;
            transform.PaddingType = PadType.SpecifiedValue;
            transform.PadCharacter = "x";
            transform.Direction = Direction.Left;

            this.ExecuteTestSubString(transform, "mai", "aixx");
        }

        private void ExecuteTestSubString(SubstringTransform transform, string sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }
    }
}

