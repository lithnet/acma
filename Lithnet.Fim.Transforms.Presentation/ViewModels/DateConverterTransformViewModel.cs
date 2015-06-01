using System;
using System.Collections.Generic;
using System.Linq;

namespace Lithnet.Fim.Transforms.Presentation
{
    public class DateConverterTransformViewModel : TransformViewModel
    {
        private DateConverterTransform model;

        public DateConverterTransformViewModel(DateConverterTransform model)
            : base(model)
        {
            this.model = model;
        }

        public DateType InputDateType
        {
            get
            {
                return this.model.InputDateType;
            }
            set
            {
                this.model.InputDateType = value;
            }
        }

        public string InputFormat
        {
            get
            {
                return this.model.InputFormat;
            }
            set
            {
                this.model.InputFormat = value;
            }
        }
        
        public TimeZoneInfo InputTimeZone
        {
            get
            {
                return this.model.InputTimeZone;
            }
            set
            {
                this.model.InputTimeZone = value;
            }
        }

        public DateOperator CalculationOperator
        {
            get
            {
                return this.model.CalculationOperator;
            }
            set
            {
                this.model.CalculationOperator = value;
            }
        }

        public TimeSpanType CalculationTimeSpanType
        {
            get
            {
                return this.model.CalculationTimeSpanType;
            }
            set
            {
                this.model.CalculationTimeSpanType = value;
            }
        }

        public int CalculationValue
        {
            get
            {
                return this.model.CalculationValue;
            }
            set
            {
                this.model.CalculationValue = value;
            }
        }

        public DateType OutputDateType
        {
            get
            {
                return this.model.OutputDateType;
            }
            set
            {
                this.model.OutputDateType = value;

                if (this.OutputDateType == DateType.FileTime)
                {
                    this.OutputTimeZone = TimeZoneInfo.Utc;
                }
            }
        }

        public string OutputFormat
        {
            get
            {
                return this.model.OutputFormat;
            }
            set
            {
                this.model.OutputFormat = value;
            }
        }

        public TimeZoneInfo OutputTimeZone
        {
            get
            {
                return this.model.OutputTimeZone;
            }
            set
            {
                this.model.OutputTimeZone = value;
            }
        }

        public bool OutputFormatIsEnabled
        {
            get
            {
                return this.OutputDateType == DateType.String;
            }
        }

        public bool InputFormatIsEnabled
        {
            get
            {
                return this.InputDateType == DateType.String;
            }
        }

        public bool OutputTimeZoneIsEnabled
        {
            get
            {
                return this.OutputDateType != DateType.FileTime;
            }
        }

        public bool InputTimeZoneIsEnabled
        {
            get
            {
                return this.InputDateType != DateType.FileTime;
            }
        }


        public bool CalculationIsEnabled
        {
            get
            {
                return this.CalculationOperator != DateOperator.None;
            }
        }


        public override string TransformDescription
        {
            get
            {
                return strings.DateConverterTransformDescription;
            }
        }

        public IEnumerable<TimeZoneInfo> SystemTimeZones
        {
            get
            {
                return TimeZoneInfo.GetSystemTimeZones();
            }
        }

        protected override void ValidatePropertyChange(string propertyName)
        {
            base.ValidatePropertyChange(propertyName);

            if (propertyName == "InputDateType")
            {
                if (this.InputDateType == DateType.FileTime)
                {
                    this.InputTimeZone = TimeZoneInfo.Utc;
                }
            }
            else if (propertyName == "OutputDateType")
            {
                if (this.OutputDateType == DateType.FileTime)
                {
                    this.OutputTimeZone = TimeZoneInfo.Utc;
                }
            }
        }
    }
}
