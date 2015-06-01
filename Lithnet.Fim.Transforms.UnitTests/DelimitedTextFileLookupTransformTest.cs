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
    [TestClass()]
    public class DelimitedTextFileLookupTransformTest
    {
        public DelimitedTextFileLookupTransformTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            DelimitedTextFileLookupTransform transformToSeralize = new DelimitedTextFileLookupTransform();
            transformToSeralize.DefaultValue = "A";
            transformToSeralize.FileName = "B";
            transformToSeralize.FindColumn = 1;
            transformToSeralize.HasHeaderRow = true;
            transformToSeralize.OnMissingMatch = OnMissingMatch.UseNull;
            transformToSeralize.ReplaceColumn = 5;
            transformToSeralize.ID = "test001";
            transformToSeralize.CustomDelimiterRegex = "1234";
            transformToSeralize.DelimiterType = DelimiterType.Custom;
            transformToSeralize.CustomEscapeSequence = "\\";
            transformToSeralize.UserDefinedReturnType = ExtendedAttributeType.Integer;
            UniqueIDCache.ClearIdCache();
            
            DelimitedTextFileLookupTransform deserializedTransform = (DelimitedTextFileLookupTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.DefaultValue, deserializedTransform.DefaultValue);
            Assert.AreEqual(transformToSeralize.FileName, deserializedTransform.FileName);
            Assert.AreEqual(transformToSeralize.FindColumn, deserializedTransform.FindColumn);
            Assert.AreEqual(transformToSeralize.HasHeaderRow, deserializedTransform.HasHeaderRow);
            Assert.AreEqual(transformToSeralize.OnMissingMatch, deserializedTransform.OnMissingMatch);
            Assert.AreEqual(transformToSeralize.ReplaceColumn, deserializedTransform.ReplaceColumn);
            Assert.AreEqual(transformToSeralize.CustomDelimiterRegex, deserializedTransform.CustomDelimiterRegex);
            Assert.AreEqual(transformToSeralize.DelimiterType, deserializedTransform.DelimiterType);
            Assert.AreEqual(transformToSeralize.CustomEscapeSequence, deserializedTransform.CustomEscapeSequence);
            Assert.AreEqual(transformToSeralize.UserDefinedReturnType, deserializedTransform.UserDefinedReturnType);
        }

        [TestMethod()]
        public void DtfTransformCustomValueTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.esv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.Custom;
            transform.CustomDelimiterRegex = @"(?<!\\)=";
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            this.ExecuteTestString(transform, "1234", "MyName");
        }

        [TestMethod()]
        public void DtfTransformCustomValueTestWithEscapeCharacters()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.esv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.Custom;
            transform.CustomDelimiterRegex = @"(?<!\\)=";
            transform.CustomEscapeSequence = @"\";
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;
            this.ExecuteTestBinary(transform, new byte[] { 0, 1, 1, 33 }, new byte[] { 0, 1, 1, 34 });
        }


        [TestMethod()]
        public void DtfTransformTsvValueTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.tsv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.TabSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            this.ExecuteTestString(transform, "1234", "MyName");
        }

        [TestMethod()]
        public void DtfTransformTsvQuotedValuesTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.tsv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.TabSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTestString(transform, "123456", "My new name");
        }

        [TestMethod()]
        public void DtfTransformCsvValueTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            this.ExecuteTestString(transform, "1234", "MyName");
        }

        [TestMethod()]
        public void DtfTransformCsvCommentValueTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappingsWithComments.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.CommentCharacter = "#";
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            this.ExecuteTestString(transform, "1234", "MyName");
        }

        [TestMethod()]
        public void DtfTransformCsvQuotedValuesTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTestString(transform, "123456", "My new name");
        }

        [TestMethod()]
        public void DtfTransformCsvQuotedValuesAndCommasTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            this.ExecuteTestString(transform, "1234567", "My new name, with commas");
        }

        [TestMethod()]
        public void DtfTransformCsvQuotedValuesAndCommasAndQuotedValuesTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            this.ExecuteTestString(transform, "12345678", "My new name, with commas, and \"quotes\"");
        }

        [TestMethod()]
        public void DtfTransformCsvMissingValueNullTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseNull;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;

            this.ExecuteTestString(transform, "novalue", null);
        }

        [TestMethod()]
        public void DtfTransformCsvBinaryTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;
            this.ExecuteTestBinary(transform, new byte[] { 0, 1, 1, 33 }, new byte[] { 0, 1, 1, 34 });
        }

        [TestMethod()]
        public void DtfTransformCsvIntegerTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.Integer;
            this.ExecuteTestInteger(transform, 123456789L, 5L);
        }

        [TestMethod()]
        public void DtfTransformCsvBinaryOriginalValueTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;
            this.ExecuteTestBinary(transform, new byte[] { 99, 99, 99, 99 }, new byte[] { 99, 99, 99, 99 });
        }

        [TestMethod()]
        public void DtfTransformCsvBinaryDefaultValueTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseDefault;
            transform.DefaultValue = "AAEBIQ==";
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;
            this.ExecuteTestBinary(transform, new byte[] { 99, 99, 99, 99 }, new byte[] { 0, 1, 1, 33 });
        }

        [TestMethod()]
        public void DtfTransformCsvBinaryNullTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseNull;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.Binary;
            this.ExecuteTestBinary(transform, new byte[] { 99, 99, 99, 99 }, null);
        }

        [TestMethod()]
        public void DtfTransformCsvMissingValueOriginalTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseOriginal;
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            this.ExecuteTestString(transform, "novalue", "novalue");
        }

        [TestMethod()]
        public void DtfTransformCsvMissingValueDefaultTest()
        {
            DelimitedTextFileLookupTransform transform = new DelimitedTextFileLookupTransform();
            transform.FileName = @"..\..\TestData\OUMappings.csv";
            transform.HasHeaderRow = false;
            transform.FindColumn = 0;
            transform.ReplaceColumn = 1;
            transform.OnMissingMatch = OnMissingMatch.UseDefault;
            transform.DefaultValue = "defaultValue";
            transform.DelimiterType = DelimiterType.CommaSeparated;
            transform.UserDefinedReturnType = ExtendedAttributeType.String;
            this.ExecuteTestString(transform, "novalue", "defaultValue");
        }

        private void ExecuteTestBinary(Transform target, byte[] sourceValue, byte[] expectedValue)
        {
            byte[] outValue = target.TransformValue(sourceValue).FirstOrDefault() as byte[];
            CollectionAssert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestString(Transform target, string sourceValue, string expectedValue)
        {
            string outValue = target.TransformValue(sourceValue).FirstOrDefault() as string;
            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestInteger(Transform target, long sourceValue, long expectedValue)
        {
            long outValue = (long)target.TransformValue(sourceValue).FirstOrDefault();
            Assert.AreEqual(expectedValue, outValue);
        }
    }
}
