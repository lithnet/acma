using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;

namespace Lithnet.Fim.UniversalMARE.UnitTests
{
    public class TestAttrib : Attrib
    {
        private AttributeType dataType;

        private string name;

        private bool isMultivalued;

        private ValueCollection values;

        public TestAttrib(string name, AttributeType dataType, bool isMultivalued)
        {
            this.name = name;
            this.dataType = dataType;
            this.isMultivalued = isMultivalued;
            this.values = new TestValueCollection(dataType);

            if (!this.isMultivalued)
            {
                this.values.Add(new TestValue(this.dataType, null));
            }
        }

        public override AttributeType DataType
        {
            get
            {
                return this.dataType;
            }
        }

        public override string Name
        {
            get
            {
                return this.name;
            }
        }

        public override bool IsMultivalued
        {
            get
            {
                return this.isMultivalued;
            }
        }

        public override long IntegerValue
        {
            get
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                return this.values[0].ToInteger();
            }
            set
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                ((TestValue)this.values[0]).Value = value;
            }
        }

        public override byte[] BinaryValue
        {
            get
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                return this.values[0].ToBinary();
            }
            set
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                ((TestValue)this.values[0]).Value = value;
            }
        }

        public override bool BooleanValue
        {
            get
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                return this.values[0].ToBoolean();
            }
            set
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                ((TestValue)this.values[0]).Value = value;
            }
        }

        public override ReferenceValue ReferenceValue
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override string StringValue
        {
            get
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                return this.values[0].ToString();
            }
            set
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                ((TestValue)this.values[0]).Value = value;
            }
        }

        public void SetValue(object value)
        {
            if (this.isMultivalued)
            {
                throw new InvalidOperationException();
            }

            if (this.values.Count == 0)
            {
                this.values.Add(new TestValue(this.dataType, value));
            }
            else
            {
                ((TestValue)this.values[0]).Value = value;
            }
        }

        public override string Value
        {
            get
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                return this.values[0].ToString();
            }
            set
            {
                if (this.isMultivalued)
                {
                    throw new InvalidOperationException();
                }

                ((TestValue)this.values[0]).Value = value;
            }
        }

        public override ValueCollection Values
        {
            get
            {
                return this.values;
            }
            set
            {
                this.values = value;
            }
        }

        public void SetValues(IEnumerable<object> values)
        {
            this.Values.Clear();

            foreach (object val in values)
            {
                TestValue value = new TestValue(this.dataType, val);
                this.values.Add(value);
            }
        }

        public override void Delete()
        {
            this.values.Clear();
        }

        public override bool IsPresent
        {
            get
            {
                if (this.values.Count == 0)
                {
                    return false;
                }
                else if (this.values.Count == 1)
                {
                    return ((TestValue)this.values[0]).Value != null;

                }
                else
                {
                    return true;
                }
            }
        }

        public override ManagementAgent LastContributingMA
        {
            get { throw new NotImplementedException(); }
        }

        public override DateTime LastContributionTime
        {
            get { throw new NotImplementedException(); }
        }
    }
}
