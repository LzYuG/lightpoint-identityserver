using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Forms.ValidationHelper
{
    internal class MyOneOfAttribute : ValidationAttribute
    {
        internal object[] Values { get; set; }

        internal MyOneOfAttribute(object[] values)
        {
            Values = values;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name, JsonSerializer.Serialize(Values));
        }

        public override bool IsValid(object? value)
        {
            if (value is Array)
            {
                return false;
            }

            foreach (var v in Values)
            {
                if (v.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
