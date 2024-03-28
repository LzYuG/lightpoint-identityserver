using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Forms.ValidationHelper
{
    internal class MyArrayLengthAttribute : ValidationAttribute
    {
        internal int Length { get; }

        internal MyArrayLengthAttribute(int length)
        {
            Length = length;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Length);
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            if (!(value is Array valueAsArray))
            {
                return false;
            }

            return valueAsArray.Length == Length;
        }
    }
}
