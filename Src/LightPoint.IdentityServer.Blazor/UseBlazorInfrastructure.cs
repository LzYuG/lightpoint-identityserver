using LightPoint.IdentityServer.ServerInfrastructure.ServiceInjectConfig;
using LightPoint.IdentityServer.ServerInfrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using LightPoint.IdentityServer.ServerInfrastructure.EndpointInterfaces;
using LightPoint.IdentityServer.Blazor.Utils;
using LightPoint.IdentityServer.Blazor.Layout.Helpers;
using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Authorization;
using Microsoft.AspNetCore.Components.Authorization;

namespace LightPoint.IdentityServer.Blazor
{
    public static class UseBlazorInfrastructure
    {
        public static IServiceCollection AddLightPointIdentityService<TServiceImpFactory>(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
            where TServiceImpFactory : class, IServiceImpFactory
        {
            services.AddSingleton<RepositoryFactory>();
            services.AddSingleton<IServiceImpFactory, TServiceImpFactory>();
            services.AddSingleton<IRepositoryImpFactory, TServiceImpFactory>();
            services.AddInfrastructureServices(configuration, environment);

            services.AddRazorPages();
            //.AddRazorPagesOptions(o =>
            //{
            //    o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            //});
            services.AddServerSideBlazor();

            services.AddScoped<AuthenticationStateProvider, LightPointAuthenticationStateProvider>();
            services.AddCascadingAuthenticationState();

            // 这是直接注入的版本，如果要使用自实现的视图模型
            services.AddGlobalResourcesServices();
            // Blazor框架
            services.FrameworkDependentInject<LayoutInitor>();

            return services;
        }

        public static IServiceCollection WithAccountPageService<TIdentityServerAccountPageService>(this IServiceCollection service)
            where TIdentityServerAccountPageService : class, IIdentityServerAccountPageService
        {
            service.AddScoped<IIdentityServerAccountPageService, TIdentityServerAccountPageService>();
            return service;
        }

        public static IServiceCollection WithConsentPageService<TIdentityServerConsentPageService>(this IServiceCollection service)
            where TIdentityServerConsentPageService : class, IIdentityServerConsentPageService
        {
            service.AddScoped<IIdentityServerConsentPageService, TIdentityServerConsentPageService>();
            return service;
        }

        public static IServiceCollection WithDevicePageService<TIdentityServerDevicePageService>(this IServiceCollection service)
            where TIdentityServerDevicePageService : class, IIdentityServerDevicePageService
        {
            service.AddScoped<IIdentityServerDevicePageService, TIdentityServerDevicePageService>();
            return service;
        }

        public static IServiceCollection WithIdentityServerResourceSecretService<TIdentityServerResourceSecretService>(this IServiceCollection service)
            where TIdentityServerResourceSecretService : class, IIdentityServerResourceSecretService
        {
            service.AddScoped<IIdentityServerResourceSecretService, TIdentityServerResourceSecretService>();
            return service;
        }

        public static void AddIdentityServerEndpoint
            <TAuthorizeEndpoint,
             TDeviceAuthorizationEndpoint,
             TDiscoveryEndpoint,
             TEndSessionEndpoint,
             TIntrospectionEndpoint,
             TRevocationEndpoint,
             TTokenEndpoint,
             TUserInfoEndpoint>(this IServiceCollection service)
             where TAuthorizeEndpoint : class, IAuthorizeEndpoint
             where TDeviceAuthorizationEndpoint : class, IDeviceAuthorizationEndpoint
             where TDiscoveryEndpoint : class, IDiscoveryEndpoint
             where TEndSessionEndpoint : class, IEndSessionEndpoint
             where TIntrospectionEndpoint : class, IIntrospectionEndpoint
             where TRevocationEndpoint : class, IRevocationEndpoint
             where TTokenEndpoint : class, ITokenEndpoint
             where TUserInfoEndpoint : class, IUserInfoEndpoint
        {
            List<IdentityServerEndpointInfo> identityServerEndpointInfos = new List<IdentityServerEndpointInfo>
            {
                new IdentityServerEndpointInfo("/.well-known/openid-configuration", nameof(TDiscoveryEndpoint), typeof(TDiscoveryEndpoint)),
                new IdentityServerEndpointInfo("/connect/authorize", nameof(TAuthorizeEndpoint), typeof(TAuthorizeEndpoint)),
                new IdentityServerEndpointInfo("/connect/token", nameof(TTokenEndpoint), typeof(TTokenEndpoint)),
                new IdentityServerEndpointInfo("/connect/userinfo", nameof(TUserInfoEndpoint), typeof(TUserInfoEndpoint)),
                new IdentityServerEndpointInfo("/connect/deviceauthorization", nameof(TDeviceAuthorizationEndpoint), typeof(TDeviceAuthorizationEndpoint)),
                new IdentityServerEndpointInfo("/connect/introspect", nameof(TIntrospectionEndpoint), typeof(TIntrospectionEndpoint)),
                new IdentityServerEndpointInfo("/connect/revocation", nameof(TRevocationEndpoint), typeof(TRevocationEndpoint)),
                new IdentityServerEndpointInfo("/connect/endsession", nameof(TEndSessionEndpoint), typeof(TEndSessionEndpoint)),
            };

            service.AddSingleton(identityServerEndpointInfos);
            service.AddSingleton<IAuthorizeEndpoint, TAuthorizeEndpoint>();
            service.AddSingleton<IDeviceAuthorizationEndpoint, TDeviceAuthorizationEndpoint>();
            service.AddSingleton<IDiscoveryEndpoint, TDiscoveryEndpoint>();
            service.AddSingleton<IEndSessionEndpoint, TEndSessionEndpoint>();
            service.AddSingleton<IIntrospectionEndpoint, TIntrospectionEndpoint>();
            service.AddSingleton<IRevocationEndpoint, TRevocationEndpoint>();
            service.AddSingleton<ITokenEndpoint, TTokenEndpoint>();
            service.AddSingleton<IUserInfoEndpoint, TUserInfoEndpoint>();
        }
    }
}
