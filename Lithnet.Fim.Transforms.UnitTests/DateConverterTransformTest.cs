using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using System.Collections.Generic;
using Lithnet.Fim.Core;
using Lithnet.Fim.Transforms;
using Microsoft.MetadirectoryServices;
using Lithnet.Common.ObjectModel;
using System.Linq;

namespace Lithnet.Fim.Transforms.UnitTests
{
    /// <summary>
    ///This is a test class for DateCalculationTransformTest and is intended
    ///to contain all DateCalculationTransformTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DateConverterTransformTest
    {
        public DateConverterTransformTest()
        {
            UnitTestControl.Initialize();
        }

        [TestMethod()]
        public void TestSerialization()
        {
            UniqueIDCache.ClearIdCache();
            DateConverterTransform transformToSeralize = new DateConverterTransform();
            transformToSeralize.ID = "test001";
            transformToSeralize.InputDateType = DateType.FimServiceString;
            transformToSeralize.InputFormat = "abc";
            transformToSeralize.InputTimeZone = TimeZoneInfo.Utc;
            transformToSeralize.CalculationOperator = DateOperator.Add;
            transformToSeralize.CalculationTimeSpanType = TimeSpanType.Hours;
            transformToSeralize.CalculationValue = 6;
            transformToSeralize.OutputDateType = DateType.String;
            transformToSeralize.OutputFormat = "def";
            transformToSeralize.OutputTimeZone = TimeZoneInfo.Local;
            UniqueIDCache.ClearIdCache();

            DateConverterTransform deserializedTransform = (DateConverterTransform)UnitTestControl.XmlSerializeRoundTrip<Transform>(transformToSeralize);

            Assert.AreEqual(transformToSeralize.ID, deserializedTransform.ID);
            Assert.AreEqual(transformToSeralize.InputDateType, deserializedTransform.InputDateType);
            Assert.AreEqual(transformToSeralize.InputFormat, deserializedTransform.InputFormat);
            Assert.AreEqual(transformToSeralize.InputTimeZone, deserializedTransform.InputTimeZone);
            Assert.AreEqual(transformToSeralize.CalculationOperator, deserializedTransform.CalculationOperator);
            Assert.AreEqual(transformToSeralize.CalculationTimeSpanType, deserializedTransform.CalculationTimeSpanType);
            Assert.AreEqual(transformToSeralize.CalculationValue, deserializedTransform.CalculationValue);
            Assert.AreEqual(transformToSeralize.OutputFormat, deserializedTransform.OutputFormat);
            Assert.AreEqual(transformToSeralize.OutputDateType, deserializedTransform.OutputDateType);
            Assert.AreEqual(transformToSeralize.OutputTimeZone, deserializedTransform.OutputTimeZone);
        }

        [TestMethod()]
        public void DateConverterTransformAdd30Days()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.Ticks;
            transform.CalculationTimeSpanType = TimeSpanType.Days;
            transform.CalculationOperator = DateOperator.Add;
            transform.CalculationValue = 30;

            this.ExecuteTestDateConverterCalculation(transform, transform.CalculationTimeSpanType, transform.CalculationValue);
        }


        [TestMethod()]
        public void DateCalculationTransformMinus30Days()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.Ticks;
            transform.CalculationTimeSpanType = TimeSpanType.Days;
            transform.CalculationOperator = DateOperator.Add;
            transform.CalculationValue = -30;

            this.ExecuteTestDateConverterCalculation(transform, transform.CalculationTimeSpanType, transform.CalculationValue);
        }

        [TestMethod()]
        public void DateCalculationTransformAdd7Days()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.Ticks;
            transform.CalculationTimeSpanType = TimeSpanType.Days;
            transform.CalculationOperator = DateOperator.Add;
            transform.CalculationValue = 7;

            this.ExecuteTestDateConverterCalculation(transform, transform.CalculationTimeSpanType, transform.CalculationValue);
        }

        [TestMethod()]
        public void DateCalculationTransformAdd5Hours()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.Ticks;
            transform.CalculationTimeSpanType = TimeSpanType.Hours;
            transform.CalculationOperator = DateOperator.Add;
            transform.CalculationValue = 5;

            this.ExecuteTestDateConverterCalculation(transform, transform.CalculationTimeSpanType, transform.CalculationValue);
        }

        [TestMethod()]
        public void DateCalculationTransformMinus24Hours()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.Ticks;
            transform.CalculationTimeSpanType = TimeSpanType.Hours;
            transform.CalculationOperator = DateOperator.Add;
            transform.CalculationValue = -24;

            this.ExecuteTestDateConverterCalculation(transform, transform.CalculationTimeSpanType, transform.CalculationValue);
        }

        [TestMethod()]
        public void DateCalculationTransformAdd24HoursMV()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.Ticks;
            transform.CalculationTimeSpanType = TimeSpanType.Hours;
            transform.CalculationOperator = DateOperator.Add;
            transform.CalculationValue = 24;

            this.ExecuteTestDateConverterCalculationMV(transform, transform.CalculationTimeSpanType, transform.CalculationValue);
        }

        [TestMethod()]
        public void DateFormatTransformDateTimeToShortDate()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.DateTime;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.String;
            transform.OutputFormat = "d";

            this.ExecuteTestFormatDate(transform, new DateTime(2000, 1, 1, 0, 5, 0), "1/01/2000");
        }

        [TestMethod()]
        public void DateFormatTransformDateTimeToShortDatePlus5Days()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.DateTime;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.String;
            transform.CalculationOperator = DateOperator.Add;
            transform.CalculationTimeSpanType = TimeSpanType.Days;
            transform.CalculationValue = 5;
            transform.OutputFormat = "d";

            this.ExecuteTestFormatDate(transform, new DateTime(2000, 1, 1, 0, 5, 0), "6/01/2000");
        }

        [TestMethod()]
        public void DateFormatTransformShortDateToDateTime()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.String;
            transform.InputFormat = "d";
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.DateTime;

            this.ExecuteTestStringToDateTime(transform, "1/01/2000", new DateTime(2000, 1, 1, 0, 0, 0));
        }

        [TestMethod()]
        public void DateFormatTransformShortDate()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.String;
            transform.OutputFormat = "d";

            this.ExecuteTestFormatDate(transform, new DateTime(2000, 1, 1, 0, 5, 0).Ticks, "1/01/2000");
        }

        [TestMethod()]
        public void ExecuteTestFormatDateAsLongDate()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.String;
            transform.OutputFormat = "D";

            this.ExecuteTestFormatDate(transform, new DateTime(2000, 1, 1, 1, 0, 0).Ticks, "Saturday, 1 January 2000");
        }

        [TestMethod()]
        public void ExecuteTestFormatDateAsFullDateToUniversal()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Utc;
            transform.OutputDateType = DateType.String;
            transform.OutputFormat = "F";

            this.ExecuteTestFormatDate(
                transform,
                new DateTime(2000, 1, 1, 1, 0, 0).Ticks,
                new DateTime(2000, 1, 1, 1, 0, 0).ToUniversalTime().ToString("F"));
        }

        [TestMethod()]
        public void ExecuteTestFormatDateAsFullDateToLocal()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Utc;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.String;
            transform.OutputFormat = "F";

            this.ExecuteTestFormatDate(
                transform,
                new DateTime(2000, 1, 1, 1, 0, 0).Ticks,
                new DateTime(2000, 1, 1, 1, 0, 0).ToLocalTime().ToString("F"));
        }

        [TestMethod()]
        public void ExecuteTestFormatDateAsFullDate()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.String;
            transform.OutputFormat = "F";

            this.ExecuteTestFormatDate(
                transform,
                new DateTime(2000, 1, 1, 1, 0, 0).Ticks,
                new DateTime(2000, 1, 1, 1, 0, 0).ToString("F"));
        }

        [TestMethod()]
        public void DateFormatTransformShortTime()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.Ticks;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.String;
            transform.OutputFormat = "t";

            this.ExecuteTestFormatDate(transform, new DateTime(2000, 1, 1, 1, 0, 0).Ticks, "1:00 AM");
        }

        [TestMethod()]
        public void DateFormatTransformFileTime()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Utc;
            transform.InputDateType = DateType.FileTime;
            transform.OutputTimeZone = TimeZoneInfo.Utc;
            transform.OutputDateType = DateType.DateTime;

            this.ExecuteTestTicksToDateTime(transform, new DateTime(0).Ticks, new DateTime(1601, 1, 1, 0, 0, 0));
        }

        [TestMethod()]
        public void StringToDateFormatAdjustToUniversal()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.String;
            transform.OutputTimeZone = TimeZoneInfo.Utc;
            transform.OutputDateType = DateType.Ticks;
            transform.InputFormat = "dd/MM/yyyy hh:mm:ss tt";

            this.ExecuteTestStringToTicks(transform, "01/01/2000 01:00:00 AM", new DateTime(2000, 1, 1, 1, 0, 0).ToUniversalTime().Ticks);
        }

        [TestMethod()]
        public void StringToDateFormatAssumeAndAdjustUniversal()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Utc;
            transform.InputDateType = DateType.String;
            transform.OutputTimeZone = TimeZoneInfo.Utc;
            transform.OutputDateType = DateType.Ticks;
            transform.InputFormat = "dd/MM/yyyy hh:mm:ss tt";

            this.ExecuteTestStringToTicks(transform, "01/01/2000 01:00:00 AM", new DateTime(2000, 1, 1, 1, 0, 0, DateTimeKind.Utc).Ticks);
        }

        [TestMethod()]
        public void StringToDateFormatAssumeLocal()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.String;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.Ticks;
            transform.InputFormat = "dd/MM/yyyy hh:mm:ss tt";

            this.ExecuteTestStringToTicks(transform, "01/01/2000 01:00:00 AM", new DateTime(2000, 1, 1, 1, 0, 0).Ticks);
        }

        [TestMethod()]
        public void FimServiceStringToDate()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Local;
            transform.InputDateType = DateType.FimServiceString;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.Ticks;

            this.ExecuteTestStringToTicks(transform, "2014-01-01T01:00:00.000", new DateTime(2014, 1, 1, 1, 0, 0).Ticks);
        }

        [TestMethod()]
        public void FimServiceStringUtcToDateLocal()
        {
            DateConverterTransform transform = new DateConverterTransform();
            transform.InputTimeZone = TimeZoneInfo.Utc;
            transform.InputDateType = DateType.FimServiceString;
            transform.OutputTimeZone = TimeZoneInfo.Local;
            transform.OutputDateType = DateType.Ticks;

            this.ExecuteTestStringToTicks(transform, "2014-01-02T10:00:00.000", new DateTime(2014, 1, 2, 10, 0, 0, DateTimeKind.Utc).ToLocalTime().Ticks);
        }

        private void ExecuteTestStringToTicks(DateConverterTransform transform, string sourceValue, long expectedValue)
        {
            long outValue = (long)transform.TransformValue(sourceValue).FirstOrDefault();

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestStringToDateTime(DateConverterTransform transform, string sourceValue, DateTime expectedValue)
        {
            DateTime outValue = (DateTime)transform.TransformValue(sourceValue).FirstOrDefault();

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestTicksToDateTime(DateConverterTransform transform, long sourceValue, DateTime expectedValue)
        {
            DateTime outValue = (DateTime)transform.TransformValue(sourceValue).FirstOrDefault();

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestFormatDate(DateConverterTransform transform, DateTime sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestFormatDate(DateConverterTransform transform, long sourceValue, string expectedValue)
        {
            string outValue = transform.TransformValue(sourceValue).FirstOrDefault() as string;

            Assert.AreEqual(expectedValue, outValue);
        }

        private void ExecuteTestDateConverterCalculation(DateConverterTransform transform, TimeSpanType type, int valueToAdd)
        {
            DateTime startDate = new DateTime(2000, 1, 1);
            long outValue = (long)transform.TransformValue(startDate.Ticks).First();
            long endTicks = outValue;
            DateTime endDate = new DateTime(endTicks);

            switch (type)
            {
                case TimeSpanType.Months:
                    Assert.AreEqual(startDate.AddMonths(valueToAdd), endDate);

                    break;
                case TimeSpanType.Weeks:
                    Assert.AreEqual(startDate.AddDays(valueToAdd * 7), endDate);

                    break;
                case TimeSpanType.Days:
                    Assert.AreEqual(startDate.AddDays(valueToAdd), endDate);
                    break;

                case TimeSpanType.Hours:
                    Assert.AreEqual(startDate.AddHours(valueToAdd), endDate);

                    break;
                case TimeSpanType.Minutes:
                    Assert.AreEqual(startDate.AddMinutes(valueToAdd), endDate);

                    break;
                default:
                    Assert.Inconclusive("Verify the correctness of this test method.");
                    break;
            }
        }

        private void ExecuteTestDateConverterCalculationMV(DateConverterTransform transform, TimeSpanType type, int valueToAdd)
        {
            DateTime startDate1 = new DateTime(2000, 1, 1);
            DateTime startDate2 = new DateTime(2010, 1, 1);
            DateTime startDate3 = new DateTime(2020, 1, 1);

            IList<object> startDates = new List<object>() { startDate1.Ticks, startDate2.Ticks, startDate3.Ticks };
            IList<object> outValues = transform.TransformValue(startDates);

            switch (type)
            {
                case TimeSpanType.Months:
                    Assert.AreEqual(startDate1.AddMonths(valueToAdd).Ticks, (long)outValues[0]);
                    Assert.AreEqual(startDate2.AddMonths(valueToAdd).Ticks, (long)outValues[1]);
                    Assert.AreEqual(startDate3.AddMonths(valueToAdd).Ticks, (long)outValues[2]);
                    break;

                case TimeSpanType.Weeks:
                    Assert.AreEqual(startDate1.AddDays(valueToAdd * 7).Ticks, (long)outValues[0]);
                    Assert.AreEqual(startDate2.AddDays(valueToAdd * 7).Ticks, (long)outValues[1]);
                    Assert.AreEqual(startDate3.AddDays(valueToAdd * 7).Ticks, (long)outValues[2]);
                    break;

                case TimeSpanType.Days:
                    Assert.AreEqual(startDate1.AddDays(valueToAdd).Ticks, (long)outValues[0]);
                    Assert.AreEqual(startDate2.AddDays(valueToAdd).Ticks, (long)outValues[1]);
                    Assert.AreEqual(startDate3.AddDays(valueToAdd).Ticks, (long)outValues[2]);
                    break;

                case TimeSpanType.Hours:
                    Assert.AreEqual(startDate1.AddHours(valueToAdd).Ticks, (long)outValues[0]);
                    Assert.AreEqual(startDate2.AddHours(valueToAdd).Ticks, (long)outValues[1]);
                    Assert.AreEqual(startDate3.AddHours(valueToAdd).Ticks, (long)outValues[2]);

                    break;
                case TimeSpanType.Minutes:
                    Assert.AreEqual(startDate1.AddMinutes(valueToAdd).Ticks, (long)outValues[0]);
                    Assert.AreEqual(startDate2.AddMinutes(valueToAdd).Ticks, (long)outValues[1]);
                    Assert.AreEqual(startDate3.AddMinutes(valueToAdd).Ticks, (long)outValues[2]);

                    break;
                default:
                    Assert.Inconclusive("Verify the correctness of this test method.");
                    break;
            }
        }
    }
}
