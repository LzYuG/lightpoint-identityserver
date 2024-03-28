using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Shared.Attributes
{
    /// <summary>
    /// 机密属性
    /// 存入数据库之前对该字段进行加密
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property)]
    public class ConfidentialAttribute : Attribute
    {
        public ConfidentialAttribute()
        {
            
        }

        public ConfidentialAttribute(ConfidentialType type)
        {
            Type = type;
        }

        public ConfidentialType Type { get; set; } = ConfidentialType.单向加密;

        public enum ConfidentialType
        {
            双向加密,
            单向加密,
        }
    }
}
