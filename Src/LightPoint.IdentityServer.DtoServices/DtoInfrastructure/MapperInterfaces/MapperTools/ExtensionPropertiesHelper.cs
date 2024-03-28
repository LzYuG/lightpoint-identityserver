using LightPoint.IdentityServer.DtoModels.DM00.Common;
using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools
{
    public static class ExtensionPropertiesHelper
    {
        public static List<ExtensionPropertyDM> MapperToExtensionProperties<T>(T? data)
            where T : class
        {
            if(data == null)
            {
                return new List<ExtensionPropertyDM>();
            }
            // 扩展属性
            var extensionPropertyProps = data.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x=>x.AttributeType == typeof(ExtensionPropertyAttribute)));

            List<ExtensionPropertyDM> result = new List<ExtensionPropertyDM>();
            foreach (var extensionPropertyProp in extensionPropertyProps)
            {
                result.Add(new ExtensionPropertyDM()
                {
                    Key = extensionPropertyProp.Name,
                    Value = extensionPropertyProp.GetValue(data) as string
                });
            }

            return result;
        }

        public static T? ExtensionPropertiesToData<T>(IEnumerable<ExtensionPropertyDM>? extensionProperties, T? data)
           where T : class
        {
            if(extensionProperties == null)
            {
                return data;
            }
            if (data == null)
            {
                return null;
            }
            // 扩展属性
            var extensionPropertyProps = data.GetType().GetProperties().Where(x => x.CustomAttributes.Any(x => x.AttributeType == typeof(ExtensionPropertyAttribute)));

            List<ExtensionPropertyDM> result = new List<ExtensionPropertyDM>();
            foreach (var extensionPropertyProp in extensionPropertyProps)
            {
                var extensionProperty = extensionProperties.FirstOrDefault(x => x.Key == extensionPropertyProp.Name);
                if(extensionProperty != null)
                {
                    if(extensionPropertyProp.PropertyType == typeof(string))
                    {
                        extensionPropertyProp.SetValue(data, extensionProperty.Value);
                    }
                    // 这里要从string转为目标类型
                    MethodInfo parseMethod = extensionPropertyProp.PropertyType.GetMethod("Parse", new Type[] { typeof(string) })!;
                    if(parseMethod != null)
                    {
                        extensionPropertyProp.SetValue(data, parseMethod.Invoke(extensionProperty.Value, new object[0]));
                    }
                }
                result.Add(new ExtensionPropertyDM()
                {
                    Key = extensionPropertyProp.Name,
                    Value = extensionPropertyProp.GetValue(data) as string
                });
            }
            return data;
        }
    }
}
