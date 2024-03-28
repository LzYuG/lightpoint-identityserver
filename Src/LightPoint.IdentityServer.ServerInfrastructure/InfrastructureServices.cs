using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.IdentityResource;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.IdentityResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.DtoModels.DM04.LogAuditingResources;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces;
using LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore.Contexts;
using LightPoint.IdentityServer.ServerInfrastructure.Authorization;
using LightPoint.IdentityServer.ServerInfrastructure.Cache;
using LightPoint.IdentityServer.ServerInfrastructure.Configs;
using LightPoint.IdentityServer.ServerInfrastructure.Email;
using LightPoint.IdentityServer.ServerInfrastructure.FileStorage;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.ServerInfrastructure.ServiceInjectConfig;
using LightPoint.IdentityServer.ServerInfrastructure.SMS;
using LightPoint.IdentityServer.ServerInfrastructure.Store;
using LightPoint.IdentityServer.ServerInfrastructure.ValidationCode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LightPoint.IdentityServer.ServerInfrastructure
{
    public static class InfrastructureServices
    {

        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddScoped<TenantInfo>();
            services.AddScoped<TenantInfoAccessor>();
            services.AddScoped<TenantMiddleware>();
            services.AddScoped<GlobalSystemConfigService>();
            // 获取实现的服务工厂
            var serviceImpFactory = services.BuildServiceProvider().GetRequiredService<IServiceImpFactory>();

            services.AddScoped(typeof(IEmailService), serviceImpFactory.GetEmailServiceImplementType());
            services.AddScoped(typeof(IShortMessageServerService), serviceImpFactory.GetShortMessageServerServiceImplementType());
            services.AddScoped(typeof(IConfidentialService), serviceImpFactory.GetConfidentialServiceImplementType());
            services.AddScoped(typeof(IValidationCodeCreater), serviceImpFactory.GetValidationCodeCreaterImplementType());
            services.AddScoped(typeof(ILightPointCache), serviceImpFactory.GetCacheImplementType());
            services.AddScoped(typeof(IApplicationMenuStore), serviceImpFactory.GetApplicationMenuStoreImplementType());
            services.AddScoped(typeof(IFileRepository), serviceImpFactory.GetFileRepositoryImplementType());

            serviceImpFactory.AddDataContext(services, configuration, environment);
            serviceImpFactory.AddHangfire(services, configuration, environment);
            serviceImpFactory.AddIdentity(services);
        }

        public static void AddGlobalResourcesServices(this IServiceCollection services)
        {
            services.AddSystemResourceServices();
            services.AddApplicationResourceServices();
            services.AddIdentitySevrerResourceServices();
            services.AddLogAuditingResourcesServices();
        }

        public static void AddSystemResourceServices(this IServiceCollection services)
        {
            var serviceImpFactory = services.BuildServiceProvider().GetRequiredService<IServiceImpFactory>();
            _AddService<Guid, SystemTenant, SystemTenantDQM, SystemTenantDCM>(services, serviceImpFactory);
            _AddService<Guid, SystemCommonFile, SystemCommonFileDM, SystemCommonFileDM>(services, serviceImpFactory);
            _AddService<Guid, ServerCommonConfig, ServerCommonConfigDQM, ServerCommonConfigDCM>(services, serviceImpFactory);
            _AddService<Guid, ServerEmailConfig, ServerEmailConfigDQM, ServerEmailConfigDCM>(services, serviceImpFactory);
            _AddService<Guid, SystemAccountConfig, SystemAccountConfigDQM, SystemAccountConfigDCM>(services, serviceImpFactory);
            _AddService<Guid, ServerShortMessageServiceConfig, ServerShortMessageServiceConfigDQM, ServerShortMessageServiceConfigDCM>(services, serviceImpFactory);
        }

        public static void AddApplicationResourceServices(this IServiceCollection services)
        {
            var serviceImpFactory = services.BuildServiceProvider().GetRequiredService<IServiceImpFactory>();
            _AddService<Guid, ApplicationUser, ApplicationUserDQM, ApplicationUserDCM>(services, serviceImpFactory);
            _AddService<Guid, ApplicationRole, ApplicationRoleDQM, ApplicationRoleDCM>(services, serviceImpFactory);
            _AddService<int, ApplicationUserClaim, ApplicationUserClaimDM, ApplicationUserClaimDM>(services, serviceImpFactory);
            _AddService<Guid, ApplicationUserWithRole, ApplicationUserWithRoleDM, ApplicationUserWithRoleDM>(services, serviceImpFactory);
        }

        public static void AddIdentitySevrerResourceServices(this IServiceCollection services)
        {
            var serviceImpFactory = services.BuildServiceProvider().GetRequiredService<IServiceImpFactory>();
            _AddService<Guid, IdentityServerApiResource, IdentityServerApiResourceDQM, IdentityServerApiResourceDCM>(services, serviceImpFactory);
            _AddService<Guid, IdentityServerApiScope, IdentityServerApiScopeDQM, IdentityServerApiScopeDCM>(services, serviceImpFactory);
            _AddService<Guid, IdentityServerClient, IdentityServerClientDQM, IdentityServerClientDCM>(services, serviceImpFactory);
            _AddService<Guid, IdentityServerIdentityResource, IdentityServerIdentityResourceDQM, IdentityServerIdentityResourceDCM>(services, serviceImpFactory);
            _AddService<Guid, IdentityServerDeviceFlowCode, IdentityServerDeviceFlowCodeDM, IdentityServerDeviceFlowCodeDM>(services, serviceImpFactory);
            _AddService<Guid, IdentityServerPersistedGrant, IdentityServerPersistedGrantDM, IdentityServerPersistedGrantDM>(services, serviceImpFactory);
        }

        public static void AddLogAuditingResourcesServices(this IServiceCollection services)
        {
            var serviceImpFactory = services.BuildServiceProvider().GetRequiredService<IServiceImpFactory>();
            _AddService<Guid, ApplicationUserLoginedLog, ApplicationUserLoginedLogDM, ApplicationUserLoginedLogDM>(services, serviceImpFactory);
            _AddService<Guid, MessageSendedLog, MessageSendedLogDM, MessageSendedLogDM>(services, serviceImpFactory);
            _AddService<Guid, ServerRunningLog, ServerRunningLogDM, ServerRunningLogDM>(services, serviceImpFactory);
        }
        

        /// <summary>
        /// 辅助实体模型的仓储注入
        /// </summary>
        /// <typeparam name="Tid"></typeparam>
        /// <typeparam name="TDomain"></typeparam>
        /// <param name="services"></param>
        private static void _AddRepositorty<Tid, TDomain>(this IServiceCollection services, IServiceImpFactory serviceImpFactory)
            where TDomain : class, IDomainModelBase<Tid>, new()
        {

            var defaultQueryRepositoryType = serviceImpFactory.GetQueryRepositoryImplementType<Tid, TDomain>();
            services.AddScoped(typeof(IQueryRepository<Tid, TDomain>), defaultQueryRepositoryType);

            var defaultCommandRepositoryType = serviceImpFactory.GetCommandRepositoryImplementType<Tid, TDomain>();
            services.AddScoped(typeof(ICommandRepository<Tid, TDomain>), defaultCommandRepositoryType);
        }


        /// <summary>
        /// 辅助实体与视图模型的仓储和服务注入
        /// </summary>
        /// <typeparam name="Tid"></typeparam>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TQuery"></typeparam>
        /// <typeparam name="TCommand"></typeparam>
        /// <param name="services"></param>
        private static void _AddService<Tid, TDomain, TQuery, TCommand>(this IServiceCollection services, IServiceImpFactory serviceImpFactory)
            where TDomain : class, IDomainModelBase<Tid>, new()
            where TQuery : class, IQueryDtoBase<Tid>, new()
            where TCommand : class, ICommandDtoBase<Tid>, new()
        {
            // 仓储
            services._AddRepositorty<Tid, TDomain>(serviceImpFactory);

            // 映射器
            var modelMapperImp = typeof(ServiceFactory).Assembly.GetTypes().FirstOrDefault(x => x.GetInterfaces().Any(x => x == typeof(IModelMapper<Tid, TDomain, TQuery, TCommand>)));
            if (modelMapperImp != null)
            {
                services.AddScoped(typeof(IModelMapper<Tid, TDomain, TQuery, TCommand>), modelMapperImp);
            }
            else
            {
                services.AddScoped(typeof(IModelMapper<Tid, TDomain, TQuery, TCommand>), typeof(ModelMapper<Tid, TDomain, TQuery, TCommand>));
            }

            // 模型服务
            var defaultAppServiceImpType = serviceImpFactory.GetDtoServiceImplementType<Tid, TDomain, TQuery, TCommand>();
            var defaultAppServiceInterfaceType = serviceImpFactory.GetDtoServiceInterfaceType<Tid, TDomain, TQuery, TCommand>();

            var commandService = typeof(ServiceFactory).Assembly.GetTypes().FirstOrDefault(x => x.BaseType == defaultAppServiceImpType);
            var iCommandService = typeof(ServiceFactory).Assembly.GetTypes().FirstOrDefault(x => x.IsInterface && x.GetInterfaces().Any(x => x == defaultAppServiceInterfaceType));
            if (commandService != null)
            {
                services.AddScoped(iCommandService!, commandService);
                services.AddScoped(defaultAppServiceInterfaceType!, commandService);
            }
            else
            {
                services.AddScoped(defaultAppServiceInterfaceType, defaultAppServiceImpType);
            }
        }
    }
}
