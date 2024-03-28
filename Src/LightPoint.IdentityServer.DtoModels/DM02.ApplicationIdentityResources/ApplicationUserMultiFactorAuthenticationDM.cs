using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;
using LightPoint.IdentityServer.Shared.Attributes;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE02.ApplicationIdentityResources;

namespace LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources
{
    /// <summary>
    /// 用户多因子认证
    /// 该数据是不区分租户的
    /// </summary>
    public class ApplicationUserMultiFactorAuthenticationDM : DtoBase<long>
    {
        public bool IsEnable { get; set; }
        /// <summary>
        /// 多因子认证所需参数
        /// </summary>
        [Confidential]
        public string? FactorValue { get; set; }
        /// <summary>
        /// 多因子认证所需扩展参数1
        /// </summary>
        [Confidential]
        public string? FactorValueExtension1 { get; set; }
        /// <summary>
        /// 多因子认证所需扩展参数2
        /// </summary>
        [Confidential]
        public string? FactorValueExtension2 { get; set; }
        /// <summary>
        /// 多因子认证所需扩展参数3
        /// </summary>
        [Confidential]
        public string? FactorValueExtension3 { get; set; }
        /// <summary>
        /// 一些校验操作所需要的密钥，每次更新都应该生成新的随机密钥
        /// </summary>
        [Confidential]
        public string? Secret { get; set; }
        /// <summary>
        /// 认证类型
        /// </summary>
        public ApplicationUserMultiFactorAuthenticationType ApplicationUserMultiFactorAuthenticationType { get; set; }
        /// <summary>
        /// 关联的用户
        /// </summary>
        public Guid ApplicationUserId { get; set; }
        public string? ApplicationUserName { get; set; }
    }
}
