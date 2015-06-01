using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Core;

namespace Lithnet.Fim.UniversalMARE.UnitTests
{
    public class TestValue : Value
    {
        private AttributeType dataType;

        private object value;

        public TestValue(AttributeType dataType, object value)
        {
            this.dataType = dataType;
            this.value = value;
        }

        public override AttributeType DataType
        {
            get
            {
                return dataType;
            }
        }

        public override byte[] ToBinary()
        {
            return TypeConverter.ConvertData<byte[]>(this.value);
        }

        public override bool ToBoolean()
        {
            return TypeConverter.ConvertData<bool>(this.value);
        }

        public override long ToInteger()
        {
            return TypeConverter.ConvertData<long>(this.value);
        }

        public override string ToString()
        {
            return TypeConverter.ConvertData<string>(this.value);
        }

        public object Value
        {
            get
            {
                return this.value;
            }
            set
            {
                this.value = value;
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj ||
                this.value == obj)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
    }
}
