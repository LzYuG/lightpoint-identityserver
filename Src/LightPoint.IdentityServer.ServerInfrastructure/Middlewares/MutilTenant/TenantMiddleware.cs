using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS01.SystemResources.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant
{
    /// <summary>
    /// 任何不携带租户的请求，视为根租户的请求
    /// 如果多租户的数据库不唯一
    /// 则至少应有一个默认的根数据库通过常量数据连接字符串配置，提供租户信息读取
    /// </summary>
    public class TenantMiddleware : IMiddleware
    {
        private readonly TenantInfo _tenantInfo;
        private readonly ISystemTenantService _systemTenantService;

        public TenantMiddleware(TenantInfo tenantInfo, ISystemTenantService systemTenantService)
        {
            _tenantInfo = tenantInfo;
            _systemTenantService = systemTenantService;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            string? value = "";

            // 本地localhost映射下无法支持多租户，如果想支持多租户请求，需要借助域名的转发，正常情况下从二级域名或者三级域名的标识识别租户
            // （不存在则取根租户）
            // 如 tenant1.test.com => tenant1
            // 如 tenant1.sso.test.com => tenant1
            var host = context.Request.Host.Value;
            var hostParts = host.Split('.');
            // 二级或三级域名
            if (hostParts.Length > 2)
            {
                value = hostParts[0];
            }

            if (!string.IsNullOrWhiteSpace(value) )
            {
                // 一般情况下，根租户是不判断过期的
                var tenant = await _systemTenantService.GetApiBoAsync(x => x.TenantIdentifier == value && (x.IsRoot || (!x.IsRoot && x.ExpireTime > DateTime.Now)));
                // 域名对应了租户
                if(tenant != null)
                {
                    _tenantInfo.TenantIdentifier = value;
                    _tenantInfo.Name = tenant!.Name;
                }
                else
                {
                    // 否则还是取根租户
                    var rootTenant = await _systemTenantService.GetApiBoAsync(x => x.IsRoot);
                    if (rootTenant != null)
                    {
                        _tenantInfo.TenantIdentifier = rootTenant.TenantIdentifier;
                        _tenantInfo.Name = rootTenant.Name;
                    }
                }
            }
            else
            {
                var rootTenant = await _systemTenantService.GetApiBoAsync(x => x.IsRoot);
                if (rootTenant != null)
                {
                    _tenantInfo.TenantIdentifier = rootTenant.TenantIdentifier;
                    _tenantInfo.Name = rootTenant.Name;
                }
            }

            await next(context);
        }
    }
}
