using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant
{
    /// <summary>
    /// Blazor视图使用的租户信息访问器
    /// </summary>
    public class TenantInfoAccessor
    {
        private readonly TenantInfo _tenantInfo;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public TenantInfoAccessor(TenantInfo tenantInfo, IHttpContextAccessor httpContextAccessor)
        {
            _tenantInfo = tenantInfo;
            _httpContextAccessor = httpContextAccessor;
        }

        public TenantInfo TenantInfo { get => !string.IsNullOrWhiteSpace(_tenantInfo.TenantIdentifier) ? _tenantInfo : _httpContextAccessor.HttpContext!.RequestServices.GetRequiredService<TenantInfo>(); }

        public string? TenantIdentifier { get => TenantInfo.TenantIdentifier; }


        public string? CurrentRemoteIP { get =>
                string.IsNullOrWhiteSpace(_httpContextAccessor.HttpContext!.Request.Headers["X-Forwarded-For"].FirstOrDefault()) 
                    ? _httpContextAccessor.HttpContext.Connection.RemoteIpAddress?.ToString() 
                    : _httpContextAccessor.HttpContext!.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        }
    }
}
