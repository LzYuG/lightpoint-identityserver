using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;
using LightPoint.IdentityServer.Shared.Attributes;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE02.ApplicationIdentityResources;

namespace LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources
{
    /// <summary>
    /// 用户外部身份平台登录的凭据,用于登录后关联回当前用户
    /// </summary>
    public class ApplicationUserExternalIdentityProviderCredentialDM : DtoBase<long>
    {
        /// <summary>
        /// 凭据值
        /// </summary>
        [Confidential]
        public string? CredentialValue { get; set; }
        /// <summary>
        /// 凭据扩展值1
        /// </summary>
        [Confidential]
        public string? CredentialValueExtension1 { get; set; }
        /// <summary>
        /// 凭据扩展值2
        /// </summary>
        [Confidential]
        public string? CredentialValueExtension2 { get; set; }
        /// <summary>
        /// 凭据扩展值3
        /// </summary>
        [Confidential]
        public string? CredentialValueExtension3 { get; set; }
        /// <summary>
        /// 一些校验操作所需要的密钥，每次更新都应该生成新的随机密钥
        /// </summary>
        [Confidential]
        public string? Secret { get; set; }
        /// <summary>
        /// 外部身份平台的类型
        /// </summary>
        public ExternalIdentityProviderType ExternalIdentityProviderType { get; set; }
        /// <summary>
        /// 用户
        /// </summary>
        public virtual ApplicationUserDQM? ApplicationUser { get; set; }
        /// <summary>
        /// 租户
        /// 每个租户使用的外部身份平台对应的OpenId值都可能不唯一
        /// </summary>
        public new string? TenantIdentifier { get; set; }
    }
}
