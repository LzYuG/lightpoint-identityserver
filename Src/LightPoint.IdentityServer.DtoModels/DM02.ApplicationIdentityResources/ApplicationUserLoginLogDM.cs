using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;

namespace LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources
{
    /// <summary>
    /// 用户登录日志
    /// </summary>
    public class ApplicationUserLoginLogDM : DtoBase<Guid>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// 登录的时间
        /// </summary>
        public DateTime LoginTime { get; set; }
        /// <summary>
        /// 登录的客户端id
        /// </summary>
        public string? ClientId { get; set; }
        /// <summary>
        /// 系统租户id
        /// </summary>
        public Guid? SystemTenantId { get; set; }
        /// <summary>
        /// 系统租户名称
        /// </summary>
        public string? SystemTenantName { get; set; }
        /// <summary>
        /// 系统租户的身份唯一码
        /// </summary>
        public string? SystemTenantIdentifier { get; set; }
        /// <summary>
        /// 登录的ip
        /// </summary>
        public string? RemoteIp { get; set; }
        /// <summary>
        /// 登录失效时间
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
