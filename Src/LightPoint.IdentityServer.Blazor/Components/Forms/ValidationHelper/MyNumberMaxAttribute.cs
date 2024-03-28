using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Forms.ValidationHelper
{
    internal class MyNumberMaxAttribute : ValidationAttribute
    {
        internal decimal Max { get; }

        internal MyNumberMaxAttribute(decimal max)
        {
            Max = max;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Max);
        }

        public override bool IsValid(object? value)
        {
            var type = value!.GetType();
            var typeMinValue = GetMinValueString(type);

            var attribute = new RangeAttribute(type, typeMinValue!, Max.ToString());

            return attribute.IsValid(value);
        }

        private static string? GetMinValueString(Type type)
        {
            if (type == typeof(byte)) return byte.MinValue.ToString();
            if (type == typeof(short)) return short.MinValue.ToString();
            if (type == typeof(int)) return int.MinValue.ToString();
            if (type == typeof(long)) return long.MinValue.ToString();
            if (type == typeof(float)) return float.MinValue.ToString();
            if (type == typeof(double)) return double.MinValue.ToString();
            if (type == typeof(sbyte)) return sbyte.MinValue.ToString();
            if (type == typeof(ushort)) return ushort.MinValue.ToString();
            if (type == typeof(uint)) return uint.MinValue.ToString();
            if (type == typeof(ulong)) return ulong.MinValue.ToString();
            if (type == typeof(decimal)) return decimal.MinValue.ToString();

            return null;
        }
    }
}
