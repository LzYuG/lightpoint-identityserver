using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources.ExtensionProperties;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE03.IdentityServerResources;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client
{
    /// <summary>
    /// 身份认证服务客户端的抽象类
    /// Id 根据实际的实现框架中的客户端来实现
    /// </summary>
    [Table("IdentityServerClients")]
    public class IdentityServerClient : DomainModel<Guid>
    {
        public bool Enabled { get; set; } = true;
        public string? ClientId { get; set; }
        public string ProtocolType { get; set; } = "oidc";
        public bool RequireClientSecret { get; set; } = true;
        public string? ClientName { get; set; }
        public string? ClientUri { get; set; }
        public string? LogoUri { get; set; }
        public bool RequireConsent { get; set; } = false;
        public bool AllowRememberConsent { get; set; } = true;
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        public bool RequirePkce { get; set; } = true;
        public bool AllowPlainTextPkce { get; set; }
        public bool RequireRequestObject { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public string? FrontChannelLogoutUri { get; set; }
        public bool FrontChannelLogoutSessionRequired { get; set; } = true;
        public string? BackChannelLogoutUri { get; set; }
        public bool BackChannelLogoutSessionRequired { get; set; } = true;
        public bool AllowOfflineAccess { get; set; }
        public int IdentityTokenLifetime { get; set; } = 300;
        public string? AllowedIdentityTokenSigningAlgorithms { get; set; }
        public int AccessTokenLifetime { get; set; } = 3600;
        public int AuthorizationCodeLifetime { get; set; } = 300;
        public int? ConsentLifetime { get; set; } = null;
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        public int RefreshTokenUsage { get; set; } = (int)TokenUsage.OneTimeOnly;
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        public int RefreshTokenExpiration { get; set; } = (int)TokenExpiration.Absolute;
        public int AccessTokenType { get; set; } = 0; // AccessTokenType.Jwt;
        public bool EnableLocalLogin { get; set; } = true;
        public bool IncludeJwtId { get; set; }
        public List<IdentityServerClientClaim>? Claims { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public string ClientClaimsPrefix { get; set; } = "client_";
        public string? PairWiseSubjectSalt { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        public DateTime? LastAccessed { get; set; }
        public int? UserSsoLifetime { get; set; }
        public string? UserCodeType { get; set; }
        public int DeviceCodeLifetime { get; set; } = 300;
        public bool NonEditable { get; set; }


        public List<IdentityServerClientRedirectUri>? RedirectUris { get; set; }
        public List<IdentityServerClientPostLogoutRedirectUri>? PostLogoutRedirectUris { get; set; }
        public List<IdentityServerClientScope>? AllowedScopes { get; set; }
        public List<IdentityServerClientIdPRestriction>? IdentityProviderRestrictions { get; set; }
        public List<IdentityServerClientCorsOrigin>? AllowedCorsOrigins { get; set; }
        public List<IdentityServerClientProperty>? Properties { get; set; }
        public List<IdentityServerClientGrantType>? AllowedGrantTypes { get; set; }
        public List<IdentityServerClientSecret>? ClientSecrets { get; set; }


        public override async Task<DomainModelOperationRusult> BeforeAddOrUpdate(RepositoryFactory repositoryFactory)
        {
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, ApplicationRole>();
            var dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientCorsOrigin>(nameof(IdentityServerClientCorsOrigin.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientRedirectUri>(nameof(IdentityServerClientRedirectUri.ClientId), Id, null);
            if(!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientPostLogoutRedirectUri>(nameof(IdentityServerClientPostLogoutRedirectUri.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientScope>(nameof(IdentityServerClientScope.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientIdPRestriction>(nameof(IdentityServerClientIdPRestriction.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientProperty>(nameof(IdentityServerClientProperty.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientProperty>(nameof(IdentityServerClientProperty.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientGrantType>(nameof(IdentityServerClientGrantType.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientSecret>(nameof(IdentityServerClientSecret.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            return new DomainModelOperationRusult(dataAccessResult);
        }

        public override async Task<DomainModelOperationRusult> BeforeDelete(RepositoryFactory repositoryFactory)
        {
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, ApplicationRole>();
            var dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientCorsOrigin>(nameof(IdentityServerClientCorsOrigin.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientRedirectUri>(nameof(IdentityServerClientRedirectUri.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientPostLogoutRedirectUri>(nameof(IdentityServerClientPostLogoutRedirectUri.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientScope>(nameof(IdentityServerClientScope.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientIdPRestriction>(nameof(IdentityServerClientIdPRestriction.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientProperty>(nameof(IdentityServerClientProperty.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientProperty>(nameof(IdentityServerClientProperty.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientGrantType>(nameof(IdentityServerClientGrantType.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, IdentityServerClientSecret>(nameof(IdentityServerClientSecret.ClientId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            return new DomainModelOperationRusult(dataAccessResult);
        }
    }
}
