using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant
{
    /// <summary>
    /// 提供给身份认证服务，实现Store时的过滤条件使用（非Blazor场景使用）
    /// 通过TenantMiddle中间件注入
    /// </summary>
    public class TenantInfo
    {
        public string? TenantIdentifier { get; set; }

        public string? Name { get; set; }
    }
}
