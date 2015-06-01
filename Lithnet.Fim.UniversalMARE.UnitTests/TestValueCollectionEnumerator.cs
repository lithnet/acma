using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.ComponentModel;
using System.Collections;

namespace Lithnet.Fim.UniversalMARE.UnitTests
{
    public class TestValueCollectionEnumerator : ValueCollectionEnumerator
    {
        private TestValueCollection collection;
        private IEnumerator Enumerator;

        public TestValueCollectionEnumerator(TestValueCollection collection)
        {
            this.collection = collection;
            this.Enumerator = collection.values.GetEnumerator();
        }

        public override Value Current
        {
            get
            {
                return (Value)this.Enumerator.Current;
            }
        }

        public override bool MoveNext()
        {
            return this.Enumerator.MoveNext();
        }

        public override void Reset()
        {
            this.Enumerator.Reset();
        }
    }
}
