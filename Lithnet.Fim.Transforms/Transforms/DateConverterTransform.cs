namespace Lithnet.Fim.Transforms
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Lithnet.Fim.Core;

    /// <summary>
    /// Converts and performs calculations on a date time value
    /// </summary>
    [DataContract(Name = "date-converter", Namespace = "http://lithnet.local/Lithnet.IdM.Transforms/v1/")]
    [System.ComponentModel.Description("Date conversion")]
    public class DateConverterTransform : Transform
    {
        /// <summary>
        /// Initializes a new instance of the DateConverterTransform class
        /// </summary>
        public DateConverterTransform()
        {
            this.Initialize();
        }

        /// <summary>
        /// Defines the data types that this transform may return
        /// </summary>
        public override IEnumerable<ExtendedAttributeType> PossibleReturnTypes
        {
            get
            {
                yield return ExtendedAttributeType.Integer;
                yield return ExtendedAttributeType.String;

                if (TransformGlobal.HostProcessSupportsNativeDateTime)
                {
                    yield return ExtendedAttributeType.DateTime;
                }
            }
        }

        /// <summary>
        /// Defines the input data types that this transform allows
        /// </summary>
        [PropertyChanged.DependsOn("InputDateType")]
        public override IEnumerable<ExtendedAttributeType> AllowedInputTypes
        {
            get
            {
                if (this.InputDateType == DateType.Ticks || this.InputDateType == DateType.FileTime)
                {
                    yield return ExtendedAttributeType.Integer;
                }
                else if (this.InputDateType == DateType.DateTime)
                {
                    if (TransformGlobal.HostProcessSupportsNativeDateTime)
                    {
                        yield return ExtendedAttributeType.DateTime;
                    }
                }
                else
                {
                    yield return ExtendedAttributeType.String;
                }
            }
        }

        /// <summary>
        /// Gets or sets the time zone of the incoming date as a native TimeZoneInfo object
        /// </summary>
        public TimeZoneInfo InputTimeZone { get; set; }

        /// <summary>
        /// Gets or sets the format string for the incoming date, for use when the InputDateType parameter is a custom string parameter
        /// </summary>
        [DataMember(Name = "input-format")]
        public string InputFormat { get; set; }

        /// <summary>
        /// Gets or sets the type of incoming date
        /// </summary>
        [DataMember(Name = "input-type")]
        public DateType InputDateType { get; set; }

        /// <summary>
        /// Gets or sets the operator to apply to this calculation
        /// </summary>
        [DataMember(Name = "calculation-operator")]
        public DateOperator CalculationOperator { get; set; }

        /// <summary>
        /// Gets or sets the type of time span value supplied
        /// </summary>
        [DataMember(Name = "calculation-time-span-type")]
        public TimeSpanType CalculationTimeSpanType { get; set; }

        /// <summary>
        /// Gets or sets the time span value to use in the calculation
        /// </summary>
        [DataMember(Name = "calculation-value")]
        public int CalculationValue { get; set; }

        /// <summary>
        /// Gets or sets the output time zone as a native TimeZoneInfo object
        /// </summary>
        public TimeZoneInfo OutputTimeZone { get; set; }

        /// <summary>
        /// Gets or sets the format string for the outgoing when the OutputDateType parameter is set to a custom date string
        /// </summary>
        [DataMember(Name = "output-format")]
        public string OutputFormat { get; set; }

        /// <summary>
        /// Gets or sets the outgoing date type
        /// </summary>
        [DataMember(Name = "output-type")]
        public DateType OutputDateType { get; set; }

        /// <summary>
        /// Gets or sets the time zone of the incoming date as a time zone ID string
        /// </summary>
        [DataMember(Name = "input-time-zone")]
        private string InputTimeZoneId
        {
            get
            {
                if (this.InputTimeZone == null)
                {
                    return null;
                }
                else
                {
                    return this.InputTimeZone.Id;
                }
            }

            set
            {
                if (value == null)
                {
                    this.InputTimeZone = null;
                }
                else
                {
                    this.InputTimeZone = TimeZoneInfo.FindSystemTimeZoneById(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the output time zone as a time zone ID string
        /// </summary>
        [DataMember(Name = "output-time-zone")]
        private string OutputTimeZoneId
        {
            get
            {
                if (this.OutputTimeZone == null)
                {
                    return null;
                }
                else
                {
                    return this.OutputTimeZone.Id;
                }
            }

            set
            {
                if (value == null)
                {
                    this.OutputTimeZone = null;
                }
                else
                {
                    this.OutputTimeZone = TimeZoneInfo.FindSystemTimeZoneById(value);
                }
            }
        }

        /// <summary>
        /// Executes the transformation against the specified value
        /// </summary>
        /// <param name="inputValue">The incoming value to transform</param>
        /// <returns>The transformed value</returns>
        protected override object TransformSingleValue(object inputValue)
        {
            if (this.InputDateType == DateType.FileTime && this.InputTimeZone.BaseUtcOffset.Ticks != 0)
            {
                throw new InvalidOperationException("The timezone for a FileTime type must be UTC");
            }

            if (this.OutputDateType == DateType.FileTime && this.OutputTimeZone.BaseUtcOffset.Ticks != 0)
            {
                throw new InvalidOperationException("The timezone for a FileTime type must be UTC");
            }

            return this.TransformDate(inputValue);
        }

        /// <summary>
        /// Transforms the date according to the parameters defined in the transform
        /// </summary>
        /// <param name="inputValue">The date value passed into the transform</param>
        /// <returns>The transformed date time value</returns>
        private object TransformDate(object inputValue)
        {
            DateTime date;

            switch (this.InputDateType)
            {
                case DateType.FileTime:
                    date = this.GetDateTimeFromFileTime(TypeConverter.ConvertData<long>(inputValue));
                    break;

                case DateType.Ticks:
                    date = this.GetDateTimeFromTicks(TypeConverter.ConvertData<long>(inputValue), this.InputTimeZone);
                    break;

                case DateType.FimServiceString:
                    date = this.GetDateTimeFromFimServiceString(TypeConverter.ConvertData<string>(inputValue), this.InputTimeZone);
                    break;

                case DateType.FimServiceStringTruncated:
                    date = this.GetDateTimeFromFimServiceStringTruncated(TypeConverter.ConvertData<string>(inputValue), this.InputTimeZone);
                    break;

                case DateType.String:
                    date = this.GetDateTimeFromString(TypeConverter.ConvertData<string>(inputValue), this.InputTimeZone);
                    break;

                case DateType.DateTime:
                    date = this.GetDateTimeFromDateTime(TypeConverter.ConvertData<DateTime>(inputValue), this.InputTimeZone);
                    break;

                default:
                    throw new InvalidOperationException("Unknown enum value");
            }

            return this.TransformDate(date);
        }

        /// <summary>
        /// Transforms the date according to the parameters defined in the transform
        /// </summary>
        /// <param name="date">The date value passed into the transform</param>
        /// <returns>The transformed date time value</returns>
        private object TransformDate(DateTime date)
        {
            // Apply the calculations
            date = this.ApplyDateCalcuation(date);

            // Adjust the outgoing time zone
            date = this.AdjustTimeZone(date, this.OutputTimeZone);

            // Return the date in the specified format
            return this.GetDateTimeOutput(date);
        }

        /// <summary>
        /// Performs the configured calculation on the date value
        /// </summary>
        /// <param name="date">The date value to be processed</param>
        /// <returns>The original date time value after any calculations have been applied</returns>
        private DateTime ApplyDateCalcuation(DateTime date)
        {
            if (this.CalculationValue == 0 || this.CalculationOperator == DateOperator.None)
            {
                return date;
            }

            int valueToModify = this.CalculationOperator == DateOperator.Subtract ? -this.CalculationValue : this.CalculationValue;

            switch (this.CalculationTimeSpanType)
            {
                case TimeSpanType.Months:
                    date = date.AddMonths(valueToModify);
                    break;

                case TimeSpanType.Weeks:
                    date = date.AddDays(valueToModify * 7);
                    break;

                case TimeSpanType.Days:
                    date = date.AddDays(valueToModify);
                    break;

                case TimeSpanType.Hours:
                    date = date.AddHours(valueToModify);
                    break;

                case TimeSpanType.Minutes:
                    date = date.AddMinutes(valueToModify);
                    break;

                default:
                    throw new NotSupportedException("The time span type is unknown");
            }

            return date;
        }

        /// <summary>
        /// Converts a ticks value to a native DateTime
        /// </summary>
        /// <param name="ticks">The ticks to convert</param>
        /// <param name="tz">The TimeZoneInfo object for the specified date</param>
        /// <returns>The native DateTime value represented by the ticks and time zone information</returns>
        private DateTime GetDateTimeFromTicks(long ticks, TimeZoneInfo tz)
        {
            DateTime parsedDateTime = new DateTime(ticks);
            return new DateTimeOffset(parsedDateTime, tz.GetUtcOffset(parsedDateTime)).UtcDateTime;
        }

        /// <summary>
        /// Converts a native DateTime to a DateTime with time zone information
        /// </summary>
        /// <param name="dateTime">The date time to modify</param>
        /// <param name="tz">The TimeZoneInfo object to apply to the date</param>
        /// <returns>A new DateTime containing the relevant time zone information</returns>
        private DateTime GetDateTimeFromDateTime(DateTime dateTime, TimeZoneInfo tz)
        {
            return new DateTimeOffset(dateTime, tz.GetUtcOffset(dateTime)).UtcDateTime;
        }

        /// <summary>
        /// Converts a FILETIME value to a native DateTime object
        /// </summary>
        /// <param name="dateTime">The FILETIME value (as UTC time)</param>
        /// <returns>A native DateTime in the UTC time zone</returns>
        private DateTime GetDateTimeFromFileTime(long dateTime)
        {
            return DateTime.FromFileTimeUtc(dateTime);
        }

        /// <summary>
        /// Converts a string of text representing a date and time into a native DateTime object
        /// </summary>
        /// <param name="value">The string representation of the date</param>
        /// <param name="tz">The time zone of the date</param>
        /// <returns>A native DateTime with time zone information incorporated</returns>
        private DateTime GetDateTimeFromString(string value, TimeZoneInfo tz)
        {
            DateTime parsedDateTime = DateTime.ParseExact(value, this.InputFormat, CultureInfo.CurrentCulture, DateTimeStyles.None);

            switch (parsedDateTime.Kind)
            {
                case DateTimeKind.Local:
                    return parsedDateTime.ToUniversalTime();

                case DateTimeKind.Unspecified:
                    return new DateTimeOffset(parsedDateTime, tz.GetUtcOffset(parsedDateTime)).UtcDateTime;

                case DateTimeKind.Utc:
                    return parsedDateTime;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Converts a FIM service (ISO 8601) date string to a native DateTime
        /// </summary>
        /// <param name="value">The FIM service date string</param>
        /// <param name="tz">The time zone information for the date</param>
        /// <returns>A native DateTime with time zone information included</returns>
        private DateTime GetDateTimeFromFimServiceString(string value, TimeZoneInfo tz)
        {
            DateTime parsedDateTime = DateTime.ParseExact(value, TypeConverter.FimServiceDateFormat, CultureInfo.CurrentCulture, DateTimeStyles.None);

            switch (parsedDateTime.Kind)
            {
                case DateTimeKind.Local:
                    return parsedDateTime.ToUniversalTime();

                case DateTimeKind.Unspecified:
                    return new DateTimeOffset(parsedDateTime, tz.GetUtcOffset(parsedDateTime)).UtcDateTime;

                case DateTimeKind.Utc:
                    return parsedDateTime;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Converts a FIM service (ISO 8601) date string (with milliseconds set to zero) to a native DateTime
        /// </summary>
        /// <param name="value">The FIM service date string</param>
        /// <param name="tz">The time zone information for the date</param>
        /// <returns>A native DateTime with time zone information included</returns>
        private DateTime GetDateTimeFromFimServiceStringTruncated(string value, TimeZoneInfo tz)
        {
            DateTime parsedDateTime = DateTime.ParseExact(value, TypeConverter.FimServiceDateFormatTruncated, CultureInfo.CurrentCulture, DateTimeStyles.None);

            switch (parsedDateTime.Kind)
            {
                case DateTimeKind.Local:
                    return parsedDateTime.ToUniversalTime();

                case DateTimeKind.Unspecified:
                    return new DateTimeOffset(parsedDateTime, tz.GetUtcOffset(parsedDateTime)).UtcDateTime;

                case DateTimeKind.Utc:
                    return parsedDateTime;

                default:
                    throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Converts a DateTime to the specified output format
        /// </summary>
        /// <param name="date">The DateTime object to convert</param>
        /// <returns>The value of the original date time in the format specified by the <see cref="OutputType"/>parameter</returns>
        private object GetDateTimeOutput(DateTime date)
        {
            switch (this.OutputDateType)
            {
                case DateType.Ticks:
                    return date.Ticks;

                case DateType.FimServiceString:
                    return date.ToString(TypeConverter.FimServiceDateFormat);

                case DateType.FimServiceStringTruncated:
                    return date.ToString(TypeConverter.FimServiceDateFormatTruncated);

                case DateType.String:
                    return date.ToString(this.OutputFormat);

                case DateType.DateTime:
                    return date;

                case DateType.FileTime:
                    return date.ToFileTimeUtc();

                default:
                    throw new InvalidOperationException("Unknown enum value");
            }
        }

        /// <summary>
        /// Converts a DateTime object to the specified time zone
        /// </summary>
        /// <param name="date">The date time to convert</param>
        /// <param name="tz">The time zone to convert to</param>
        /// <returns>A new DateTime object that has been shifted to the specified time zone</returns>
        private DateTime AdjustTimeZone(DateTime date, TimeZoneInfo tz)
        {
            return TimeZoneInfo.ConvertTime(date, tz);
        }

        /// <summary>
        /// Performs initialization functions for the class
        /// </summary>
        private void Initialize()
        {
            this.CalculationOperator = DateOperator.None;
            this.CalculationTimeSpanType = TimeSpanType.Days;
            this.CalculationValue = 0;
            this.InputDateType = DateType.Ticks;
            this.OutputDateType = DateType.Ticks;
            this.InputTimeZone = TimeZoneInfo.Local;
            this.OutputTimeZone = TimeZoneInfo.Local;
        }

        /// <summary>
        /// Performs pre-deserialization initialization
        /// </summary>
        /// <param name="context">The current serialization context</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}
