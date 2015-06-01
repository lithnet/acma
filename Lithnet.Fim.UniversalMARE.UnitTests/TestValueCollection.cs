using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using Lithnet.Fim.Core;

namespace Lithnet.Fim.UniversalMARE.UnitTests
{
    public class TestValueCollection : ValueCollection
    {
        internal List<Value> values = new List<Value>();

        private AttributeType dataType;

        public TestValueCollection(AttributeType dataType)
        {
            this.dataType = dataType;
        }
        
        public override void Add(long val)
        {
            this.values.Add(new TestValue(this.dataType, val));
        }

        public override void Add(string val)
        {
            this.values.Add(new TestValue(this.dataType, val));
        }

        public override void Add(byte[] val)
        {
            this.values.Add(new TestValue(this.dataType, val));
        }

        public override void Add(Value val)
        {
            this.values.Add(val);
        }

        public override void Add(ValueCollection val)
        {
            foreach (Value item in val)
            {
                this.values.Add(item);
            }
        }

        public override void Clear()
        {
            this.values.Clear();
        }

        public override bool Contains(byte[] val)
        {
            IEnumerable<byte[]> byteArray = this.values.Select(t => t.ToBinary());
            return this.values.Any(t => byteArray.Any(u => u.ContainsSameElements(val)));
        }

        public override bool Contains(long val)
        {
            return this.values.Any(t => t.ToInteger() == val);
        }

        public override bool Contains(string val)
        {
            return this.values.Any(t => t.ToString() == val);
        }

        public override bool Contains(Value val)
        {
            return this.values.Any(t => t == val);
        }

        public override int Count
        {
            get {
                return this.values.Count;
            }
        }

        public override void Remove(byte[] val)
        {
            Value existingValue = this.values.FirstOrDefault(t => t.ToBinary().ContainsSameElements(val));
            this.values.Remove(existingValue);
        }

        public override void Remove(long val)
        {
            Value existingValue = this.values.FirstOrDefault(t => t.ToInteger() == val);
            this.values.Remove(existingValue);
        }

        public override void Remove(string val)
        {
            Value existingValue = this.values.FirstOrDefault(t => t.ToString() == val);
            this.values.Remove(existingValue);
        }

        public override void Remove(Value val)
        {
            this.values.Remove(val);
        }

        public override void Set(bool val)
        {
            throw new NotImplementedException();
        }

        public override long[] ToIntegerArray()
        {
            return this.values.Select(t => t.ToInteger()).ToArray();
        }

        public override string[] ToStringArray()
        {
            return this.values.Select(t => t.ToString()).ToArray();
        }

        public override Value this[int index]
        {
            get
            {
                return this.values[index];
            }
        }

        public override ValueCollectionEnumerator GetEnumerator()
        {
            return new TestValueCollectionEnumerator(this);
        }
    }
}
