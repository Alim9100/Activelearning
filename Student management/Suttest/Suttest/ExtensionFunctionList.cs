using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class ExtensionFunctionList
    {
        public static int Converttoint(this string str)
        {
            return Convert.ToInt32(str);
        }
        public static byte[] ConvertTobytesUnicode(this string str)
        {
            return UnicodeEncoding.Unicode.GetBytes(str);
        }
        public static byte[] ConvertTobytesAscii(this string str)
        {
            return ASCIIEncoding.ASCII.GetBytes(str);
        }
        public static string ConvertToStringsUnicode(this byte[] buffer, int Count)
        {
            return UnicodeEncoding.Unicode.GetString(buffer, 0, Count);
        }
        public static string ConvertToStringAscii(this byte[] buffer, int Count)
        {
            return ASCIIEncoding.ASCII.GetString(buffer, 0, Count);
        }
    }
}
