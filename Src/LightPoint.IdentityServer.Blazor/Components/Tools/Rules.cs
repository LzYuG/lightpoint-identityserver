using AntDesign;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Components.Tools
{
    public static class Rules
    {
        public static List<FormValidationRule> Required = new List<FormValidationRule>()
        {
            new FormValidationRule(){ Required = true, Type = FormFieldType.String, Message = "{0}是必填的"},
        };

        public static List<FormValidationRule> ListRequired = new List<FormValidationRule>()
        {
            new FormValidationRule(){ Required = true, Type = FormFieldType.String, Message = "{0}是必填的", Transform = (obj) => string.Join(",", (List<string>)obj) }
        };
    }
}
