using LightPoint.IdentityServer.Blazor.Layout.Helpers;
using LightPoint.IdentityServer.ServerInfrastructure.Store;
using LightPoint.IdentityServer.ServerInfrastructure.Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.QuickStart.Foundation.LightPointIdentityServer.ServiceImps
{
    public class ApplicationMenuStore : IApplicationMenuStore
    {
        public Task<List<ApplicationMenu>> GetMenus()
        {
            return Task.FromResult(
                new List<ApplicationMenu>()
            {
                new ApplicationMenu()
                 {
                     Id = Guid.NewGuid().ToString(), Name = "主页", Icon = "home", Path = "/"
                 },
                 new ApplicationMenu()
                 {
                     Id = Guid.NewGuid().ToString(), Name = "系统维护", Icon = "apartment", AllowRoles = "SystemSuperAdmin", OnlyRoot = true, OnlyRootTenant = true, Path = "/SystemResourcesBusiness", Childrens = new List<ApplicationMenu>()
                     {
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "系统租户", Path = "/SystemResourcesBusiness/SystemTenant"},
                     }
                 },
                 new ApplicationMenu()
                 {
                     Id = Guid.NewGuid().ToString(), Name = "租户日常维护", Icon = "control",AllowRoles = "SystemSuperAdmin", Path = "/TenantResourcesBusiness", Childrens = new List<ApplicationMenu>()
                     {
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "租户环境配置", Path = "/SystemResourcesBusiness/SystemGlobalConfig"},
                     }
                 },
                 new ApplicationMenu()
                 {
                     Id = Guid.NewGuid().ToString(), Name = "租户机密存储", Icon = "database", AllowRoles = "SystemSuperAdmin", OnlyRootTenant = true, Path = "/ResourcesOperationMaintenanceBusiness", Childrens = new List<ApplicationMenu>()
                     {
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "数据库", Path = "/TenantSecretRepositoryBusiness/TenantDataBase"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "缓存服务", Path = "/TenantSecretRepositoryBusiness/TenantCacheServer"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "文件存储服务", Path = "/TenantSecretRepositoryBusiness/TenantFileRepositoryServer"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "AMQP服务", Path = "/TenantSecretRepositoryBusiness/TenantAQMPServer"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "常规配置", Path = "/TenantSecretRepositoryBusiness/TenantCommonConfig"},
                     }
                 },
                 new ApplicationMenu()
                 {
                     Id = Guid.NewGuid().ToString(), Name = "身份资源", Icon = "user", AllowRoles = "SystemSuperAdmin", Path = "/ResourcesOperationMaintenanceBusiness", Childrens = new List<ApplicationMenu>()
                     {
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "用户", Path = "/ResourcesOperationMaintenanceBusiness/ApplicationUser"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "角色", Path = "/ResourcesOperationMaintenanceBusiness/ApplicationRole"},
                     }
                 },
                 new ApplicationMenu()
                 {
                     Id = Guid.NewGuid().ToString(), Name = "身份认证服务资源", Icon = "cloud-server", AllowRoles = "SystemSuperAdmin", Path = "/IdentitySeverResourcesBusiness", Childrens = new List<ApplicationMenu>()
                     {
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "Client", Path = "/IdentitySeverResourcesBusiness/IdentityServerClient"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "ApiResource", Path = "/IdentitySeverResourcesBusiness/IdentityServerApiResource"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "ApiScope", Path = "/IdentitySeverResourcesBusiness/IdentityServerApiScope"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "IdentityResource", Path = "/IdentitySeverResourcesBusiness/IdentityServerIdentityResource"},
                     }
                 },
                 new ApplicationMenu()
                 {
                     Id = Guid.NewGuid().ToString(), Name = "日志审计", Icon = "reconciliation", AllowRoles = "SystemSuperAdmin", Path = "/LogAuditingBusiness", Childrens = new List<ApplicationMenu>()
                     {
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "用户登录日志", Path = "/LogAuditingBusiness/ApplicationUserLoginedLog"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "信息发送日志", Path = "/LogAuditingBusiness/MessageSendedLog"},
                         new ApplicationMenu(){ Id = Guid.NewGuid().ToString(), Name = "系统运行日志", Path = "/LogAuditingBusiness/ServerRunningLog"},
                     }
                 } }
             );
        }
    }
}
