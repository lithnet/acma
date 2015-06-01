using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.Common.ObjectModel
{
    public class ValueWrapper<T>
    {
        private T internalValue;

        public ValueWrapper()
        {
            this.internalValue = default(T);
        }

        public ValueWrapper(T value)
        {
            this.internalValue = value;
        }

        public T Value
        {
            get
            {
                return this.internalValue;
            }

            set
            {
                this.internalValue = value;
            }
        }
    }
}
