using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Shared.BusinessEnums.BE02.ApplicationIdentityResources
{
    /// <summary>
    /// 多因子认证的类型
    /// </summary>
    public enum ApplicationUserMultiFactorAuthenticationType
    {
        时间型动态令牌,
        /// <summary>
        /// 仅使用账号密码登录时可用
        /// </summary>
        短信验证码,
        /// <summary>
        /// 仅使用账号密码登录时可用
        /// </summary>
        邮箱验证码,
        
    }
}
