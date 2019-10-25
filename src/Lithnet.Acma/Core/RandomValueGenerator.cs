using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Lithnet.Acma
{
    public static class RandomValueGenerator
    {
        private static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static readonly char[] AlphaNumericCharacterSet = {
            'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M',
            'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'  };

        public static readonly char[] LowercaseAlphaCharacterSet = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
            'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
              };

        public static string GenerateRandomString(int length)
        {
            return GenerateRandomString(length, AlphaNumericCharacterSet);
        }

        public static string GenerateRandomString(int length, params char[] allowedChars)
        {
            char[] identifier = new char[length];
            byte[] randomData = GetRandomBytes(length);

            for (int idx = 0; idx < identifier.Length; idx++)
            {
                int pos = randomData[idx] % allowedChars.Length;
                identifier[idx] = allowedChars[pos];
            }

            return new string(identifier);
        }

        public static long GenerateRandomNumber(int length)
        {
            ulong minValue = ulong.Parse("1".PadRight(length, '0'));
            ulong maxValue = ulong.Parse("1".PadRight(length + 1, '0'));

            return (long)GetValue(minValue, maxValue);
        }

        public static long GenerateRandomNumber()
        {
            return GenerateRandomNumber(8);
        }

        private static ulong GetValue(ulong minValue, ulong maxValue)
        {
            if (minValue >= maxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            }

            if (maxValue > long.MaxValue)
            {
                throw new ArgumentOutOfRangeException(nameof(maxValue));
            }

            ulong diff = (ulong)maxValue - minValue;

            ulong upperBound = ulong.MaxValue / diff * diff;

            ulong value;
            do
            {
                value = GetRandomUInt64();
            } while (value >= upperBound);
            return (ulong)(minValue + (value % diff));
        }

        private static ulong GetRandomUInt64()
        {
            byte[] randomBytes = GetRandomBytes(sizeof(ulong));
            return BitConverter.ToUInt64(randomBytes, 0);
        }

        private static byte[] GetRandomBytes(int size)
        {
            byte[] buffer = new byte[size];
            rng.GetBytes(buffer);
            return buffer;
        }
    }
}
