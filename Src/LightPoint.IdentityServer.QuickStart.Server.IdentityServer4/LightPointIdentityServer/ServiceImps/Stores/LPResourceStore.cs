using AntDesign.Core.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using Microsoft.Extensions.Logging;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.Stores
{
    public class LPResourceStore : IResourceStore
    {
        private readonly IIdentityServerApiResourceService _identityServerApiResourceService;
        private readonly IIdentityServerApiScopeService _identityServerApiScopeService;
        private readonly IIdentityServerIdentityResourceService _identityServerIdentityResourceService;
        private readonly TenantInfoAccessor _tenantInfoAccessor;

        public LPResourceStore(IIdentityServerApiResourceService identityServerApiResourceService,
            IIdentityServerApiScopeService identityServerApiScopeService,
            IIdentityServerIdentityResourceService identityServerIdentityResourceService,
            TenantInfoAccessor tenantInfoAccessor)
        {
            _identityServerApiResourceService = identityServerApiResourceService;
            _identityServerApiScopeService = identityServerApiScopeService;
            _identityServerIdentityResourceService = identityServerIdentityResourceService;
            _tenantInfoAccessor = tenantInfoAccessor;
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            return (await _identityServerApiResourceService.GetApiBoCollectionAsync(x => apiResourceNames.Any(y => y == x.Name) && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier, x => x.SortCode!, true)).Select(
                x => new ApiResource()
                {
                    Name = x.Name,
                    Description = x.Description,
                    ApiSecrets = x.Secrets?.Select(x => Mapper<IdentityServerApiResourceSecretDM, Secret>.MapToNewObj(x)).ToList(),
                    DisplayName = x.DisplayName,
                    Enabled = x.Enabled,
                    Scopes = x.Scopes,
                    UserClaims = x.UserClaims,
                    ShowInDiscoveryDocument = x.ShowInDiscoveryDocument
                });
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            return (await _identityServerApiResourceService.GetApiBoCollectionAsync(x => x.Scopes.Any(y => scopeNames.Contains(y.Scope)) && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier, x => x.SortCode!, true)).Select(
                x => new ApiResource()
                {
                    Name = x.Name,
                    Description = x.Description,
                    ApiSecrets = x.Secrets?.Select(x => Mapper<IdentityServerApiResourceSecretDM, Secret>.MapToNewObj(x)).ToList(),
                    DisplayName = x.DisplayName,
                    Enabled = x.Enabled,
                    Scopes = x.Scopes,
                    UserClaims = x.UserClaims,
                    ShowInDiscoveryDocument = x.ShowInDiscoveryDocument
                });
        }

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            return (await _identityServerApiScopeService.GetApiBoCollectionAsync(x => scopeNames.Contains(x.Name) && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier, x => x.SortCode!, true)).Select(
                x => new ApiScope()
                {
                    Name = x.Name,
                    Description = x.Description,
                    DisplayName = x.DisplayName,
                    Emphasize = x.Emphasize,
                    Enabled = x.Enabled,
                    Required = x.Required,
                    ShowInDiscoveryDocument = x.ShowInDiscoveryDocument,
                    UserClaims = x.UserClaims
                });
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            return (await _identityServerIdentityResourceService.GetApiBoCollectionAsync(x => scopes.Contains(x.Name) && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier, x => x.SortCode!, true)).Select(x =>
            new IdentityResource()
            {
                Name = x.Name,
                Description = x.Description,
                DisplayName = x.DisplayName,
                Emphasize = x.Emphasize,
                Enabled = x.Enabled,
                Required = x.Required,
                ShowInDiscoveryDocument = x.ShowInDiscoveryDocument,
                UserClaims = x.UserClaims
            });
        }

        public async Task<Resources> GetAllResourcesAsync()
        {

            var identity = (await _identityServerIdentityResourceService.GetApiBoCollectionAsync(x => x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier, x => x.SortCode!, true)).Select(x =>
                new IdentityResource()
                {
                    Name = x.Name,
                    Description = x.Description,
                    DisplayName = x.DisplayName,
                    Emphasize = x.Emphasize,
                    Enabled = x.Enabled,
                    Required = x.Required,
                    ShowInDiscoveryDocument = x.ShowInDiscoveryDocument,
                    UserClaims = x.UserClaims
                });

            var apis = (await _identityServerApiResourceService.GetApiBoCollectionAsync(x => x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier, x => x.SortCode!, true)).Select(
                x => new ApiResource()
                {
                    Name = x.Name,
                    Description = x.Description,
                    ApiSecrets = x.Secrets?.Select(x => Mapper<IdentityServerApiResourceSecretDM, Secret>.MapToNewObj(x)).ToList(),
                    DisplayName = x.DisplayName,
                    Enabled = x.Enabled,
                    Scopes = x.Scopes,
                    UserClaims = x.UserClaims,
                    ShowInDiscoveryDocument = x.ShowInDiscoveryDocument
                });

            var scopes = (await _identityServerApiScopeService.GetApiBoCollectionAsync(x => x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier, x => x.SortCode!, true)).Select(
                x => new ApiScope()
                {
                    Name = x.Name,
                    Description = x.Description,
                    DisplayName = x.DisplayName,
                    Emphasize = x.Emphasize,
                    Enabled = x.Enabled,
                    Required = x.Required,
                    ShowInDiscoveryDocument = x.ShowInDiscoveryDocument,
                    UserClaims = x.UserClaims
                });

            var result = new Resources(
                identity,
                apis,
                scopes
            );

            return result;
        }
    }
}
