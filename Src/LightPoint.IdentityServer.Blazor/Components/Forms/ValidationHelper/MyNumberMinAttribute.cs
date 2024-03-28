using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Forms.ValidationHelper
{
    internal class MyNumberMinAttribute : ValidationAttribute
    {
        internal decimal Min { get; }

        internal MyNumberMinAttribute(decimal min)
        {
            Min = min;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Min);
        }

        public override bool IsValid(object? value)
        {
            var type = value!.GetType();
            var typeMaxValue = GetMaxValueString(type);

            var attribute = new RangeAttribute(type, Min.ToString(), typeMaxValue!);

            return attribute.IsValid(value);
        }

        private static string? GetMaxValueString(Type type)
        {
            if (type == typeof(byte)) return byte.MaxValue.ToString();
            if (type == typeof(short)) return short.MaxValue.ToString();
            if (type == typeof(int)) return int.MaxValue.ToString();
            if (type == typeof(long)) return long.MaxValue.ToString();
            if (type == typeof(float)) return float.MaxValue.ToString();
            if (type == typeof(double)) return double.MaxValue.ToString();
            if (type == typeof(sbyte)) return sbyte.MaxValue.ToString();
            if (type == typeof(ushort)) return ushort.MaxValue.ToString();
            if (type == typeof(uint)) return uint.MaxValue.ToString();
            if (type == typeof(ulong)) return ulong.MaxValue.ToString();
            if (type == typeof(decimal)) return decimal.MaxValue.ToString();

            return null;
        }
    }
}
