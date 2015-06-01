using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MetadirectoryServices;
using System.Collections;

namespace Lithnet.Fim.Core
{
    public class ByteArrayComparer : IComparer<byte[]>, IComparer
    {
        public int Compare(byte[] x, byte[] y)
        {
            return Utils.BinaryCompare(x, y);
        }

        public int Compare(object x, object y)
        {
            if (x is byte[] && y is byte[])
            {
                return this.Compare((byte[])x, (byte[])y);
            }
            else
            {
                throw new ArgumentException("The objects being compared must be byte arrays");
            }
        }
    }
}
