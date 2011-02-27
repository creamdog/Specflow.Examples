using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace service.tests.Extensions
{
    public static class ByteExtensions
    {
        public static string ToBase64String(this byte[] bytes)
        {
            var arrayLength = (long)((4.0d / 3.0d) * bytes.Length);

            if (arrayLength % 4 != 0)
                arrayLength += 4 - arrayLength % 4;

            var base64CharArray = new char[arrayLength];

            Convert.ToBase64CharArray(bytes, 0, bytes.Length, base64CharArray, 0, Base64FormattingOptions.None);
            return new String(base64CharArray);
        }
    }
}
