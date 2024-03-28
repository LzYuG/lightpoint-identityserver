using IdentityServer4.Models;
using IdentityServer4.Stores;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.Stores
{
    public class LPClientStore : IClientStore
    {
        private readonly IIdentityServerClientService _identityServerClientService;
        private readonly TenantInfoAccessor _tenantInfoAccessor;

        public LPClientStore(IIdentityServerClientService identityServerClientService,
            TenantInfoAccessor tenantInfoAccessor)
        {
            _identityServerClientService = identityServerClientService;
            _tenantInfoAccessor = tenantInfoAccessor;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var dqm = await _identityServerClientService.GetApiBoAsync(x => x.ClientId == clientId && x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier, true);
            if(dqm == null)
            {
                return null!;
            }
            var res = new Client()
            {
                ClientId = dqm.ClientId,
                AbsoluteRefreshTokenLifetime = dqm.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = dqm.AccessTokenLifetime,
                AccessTokenType = (AccessTokenType)dqm.AccessTokenType,
                AllowAccessTokensViaBrowser = dqm.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = dqm.AllowedCorsOrigins,
                AllowedGrantTypes = dqm.AllowedGrantTypes,
                AllowedScopes = dqm.AllowedScopes,
                AllowOfflineAccess = dqm.AllowOfflineAccess,
                AllowPlainTextPkce = dqm.AllowPlainTextPkce,
                AllowRememberConsent = dqm.AllowRememberConsent,
                AlwaysSendClientClaims = dqm.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = dqm.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = dqm.BackChannelLogoutSessionRequired,
                ClientSecrets = dqm.ClientSecrets?.Select(x => Mapper<IdentityServerClientSecretDM, Secret>.MapToNewObj(x)).ToList(),
                BackChannelLogoutUri = dqm.BackChannelLogoutUri,
                Claims = dqm.Claims?.Select((x) => new ClientClaim() { Type = x.Type, Value = x.Value, ValueType = x.ValueType }).ToList(),
                ClientClaimsPrefix = dqm.ClientClaimsPrefix,
                ClientName = dqm.ClientName,
                ClientUri = dqm.ClientUri,
                ConsentLifetime = dqm.ConsentLifetime,
                Description = dqm.Description,
                DeviceCodeLifetime = dqm.DeviceCodeLifetime,
                Enabled = dqm.Enabled,
                EnableLocalLogin = dqm.EnableLocalLogin,
                IncludeJwtId = dqm.IncludeJwtId,
                LogoUri = dqm.LogoUri,
                PairWiseSubjectSalt = dqm.PairWiseSubjectSalt,
                PostLogoutRedirectUris = dqm.PostLogoutRedirectUris,
                IdentityProviderRestrictions = dqm.IdentityProviderRestrictions,
                AlwaysIncludeUserClaimsInIdToken = true,
                UserSsoLifetime = dqm.UserSsoLifetime,
                ProtocolType = dqm.ProtocolType,
                IdentityTokenLifetime = dqm.IdentityTokenLifetime,
                RequireClientSecret = dqm.RequireClientSecret,
                RedirectUris = dqm.RedirectUris,
                RequirePkce = dqm.RequirePkce,
                RequireConsent = dqm.RequireConsent,
                UserCodeType = dqm.UserCodeType,
                SlidingRefreshTokenLifetime = dqm.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = dqm.UpdateAccessTokenClaimsOnRefresh,
                RequireRequestObject = dqm.RequireRequestObject,
                FrontChannelLogoutUri = dqm.FrontChannelLogoutUri,
                FrontChannelLogoutSessionRequired = dqm.FrontChannelLogoutSessionRequired,
                RefreshTokenExpiration = (TokenExpiration)dqm.RefreshTokenExpiration,
                RefreshTokenUsage = (TokenUsage)dqm.RefreshTokenUsage,
            };

            return res;
        }
    }
}
