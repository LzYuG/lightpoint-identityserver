using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Forms.ValidationHelper
{
    internal class MyNumberAttribute : ValidationAttribute
    {
        internal decimal Number { get; }

        internal MyNumberAttribute(decimal number)
        {
            Number = number;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, Number);
        }

        public override bool IsValid(object? value)
        {
            if (value == null)
            {
                return true;
            }

            if (!(value is decimal valueAsDecimal))
            {
                return false;
            }

            return Number == valueAsDecimal;
        }
    }
}
