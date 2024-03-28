using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.DM01.SystemResource
{
    public class SystemTenantDQM : QueryDto<Guid>
    {
        /// <summary>
        /// 有效期至
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 是否被禁用
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// 服务密码，独立的租户申请一些被限制的接口时使用
        /// </summary>
        [Confidential(Type = ConfidentialAttribute.ConfidentialType.单向加密)]
        public string? ServerSecret { get; set; }
        /// <summary>
        /// 是否根租户
        /// </summary>
        public bool IsRoot { get; set; }
    }
}
