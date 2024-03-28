using LightPoint.IdentityServer.DtoModels.DM00.Common;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using LightPoint.IdentityServer.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static LightPoint.IdentityServer.Shared.Attributes.ConfidentialAttribute;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools
{
    public static class ConfidentialPropertiesHelper
    {
        public static async Task<TDto> EncryptModelConfidentialPropsAsync<TDto>(this IConfidentialService confidentialService, TDto data)
            where TDto : class, new()
        {
            // 机密属性
            var confidentialProps = data.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(ConfidentialAttribute)));

            foreach(var confidentialPropertyProp in confidentialProps)
            {
                var value = confidentialPropertyProp.GetValue(data) as string;
                if(!string.IsNullOrWhiteSpace(value) && !value.StartsWith(ConfidentialConstants.EncryptedPreString))
                {
                    var confidentialedValue = "";
                    var confidentialType = _GetAttributeConfidentialType(confidentialPropertyProp);
                    if (confidentialType == ConfidentialType.双向加密)
                    {
                        confidentialedValue = ConfidentialConstants.EncryptedPreString + await confidentialService.BidirectionalEncrypt(value);
                    }
                    else
                    {
                        confidentialedValue = ConfidentialConstants.EncryptedPreString + await confidentialService.UnbidirectionalEncrypt(value);
                    }
                    confidentialPropertyProp.SetValue(data, confidentialedValue);
                }
            }
            return data;
        }

        public static async Task<TDto> DecryptModelConfidentialPropsAsync<TDto>(this IConfidentialService confidentialService, TDto data)
            where TDto : class, new()
        {
            // 机密属性
            var confidentialProps = data.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(ConfidentialAttribute)));

            foreach (var confidentialPropertyProp in confidentialProps)
            {
                var value = confidentialPropertyProp.GetValue(data) as string;
                var confidentialType = _GetAttributeConfidentialType(confidentialPropertyProp);
                if (!string.IsNullOrWhiteSpace(value) && value.StartsWith(ConfidentialConstants.EncryptedPreString) && confidentialType == ConfidentialType.双向加密)
                {
                    value = value.Replace(ConfidentialConstants.EncryptedPreString, "");
                    var confidentialedValue = await confidentialService.BidirectionalDecrypt(value);
                    confidentialPropertyProp.SetValue(data, confidentialedValue);
                }
            }
            return data;
        }


        private static ConfidentialType _GetAttributeConfidentialType(PropertyInfo confidentialPropertyProp)
        {
            var confidentialTypeValue = confidentialPropertyProp.CustomAttributes.First(x => x.AttributeType == typeof(ConfidentialAttribute)).NamedArguments.FirstOrDefault(x => x.MemberName == nameof(ConfidentialAttribute.Type)).TypedValue.Value;
            if (confidentialTypeValue == null)
            {
                confidentialTypeValue = confidentialPropertyProp.CustomAttributes.First(x => x.AttributeType == typeof(ConfidentialAttribute)).ConstructorArguments.FirstOrDefault(x => x.ArgumentType == typeof(ConfidentialAttribute.ConfidentialType)).Value;
            }
            var confidentialType = ConfidentialType.双向加密;
            if (confidentialTypeValue != null)
            {
                confidentialType = (ConfidentialType)confidentialTypeValue;
            }

            return confidentialType;
        }
    }
}
