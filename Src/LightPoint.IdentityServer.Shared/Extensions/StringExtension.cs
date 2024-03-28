using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightPoint.IdentityServer.Shared.Extensions
{
    public static class StringExtension
    {
        public static string FirstToUpper(this string str)
        {
            var str1 = str.Substring(0, 1).ToUpper();
            var str2 = str.Substring(1, str.Length - 1);
            str = str1 + str2;
            return str;
        }
    }
}
