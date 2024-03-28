using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.DtoModels.DM00.Common;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE03.IdentityServerResources;

namespace LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client
{
    /// <summary>
    /// 身份认证服务客户端的抽象类
    /// Id 根据实际的实现框架中的客户端来实现
    /// </summary>
    public class IdentityServerClientDCM : CommandDto<Guid>
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


        public List<string>? AllowedScopes { get; set; }
        public List<IdentityServerClientClaimDM>? Claims { get; set; }
        public List<string>? IdentityProviderRestrictions { get; set; }
        public List<string>? AllowedGrantTypes { get; set; }

        public List<string>? AllowedCorsOrigins { get; set; }
        public List<string>? RedirectUris { get; set; }
        public List<string>? PostLogoutRedirectUris { get; set; }
        public List<IdentityServerClientSecretDM>? ClientSecrets { get; set; }

        /// <summary>
        /// 扩展属性，登录界面的背景图片地址
        /// </summary>
        [ExtensionProperty]
        public string? LoginPageBackgroundImgUri { get; set; }
        /// <summary>
        /// 扩展属性，身份服务界面的模板
        /// </summary>
        [ExtensionProperty]
        public string? PageTemplate { get; set; } = "";

        /// <summary>
        /// Scopes 字符串形式，使用英文逗号分隔输入
        /// </summary>
        public string? ScopesFormValueHelper { get; set; }

    }
}
