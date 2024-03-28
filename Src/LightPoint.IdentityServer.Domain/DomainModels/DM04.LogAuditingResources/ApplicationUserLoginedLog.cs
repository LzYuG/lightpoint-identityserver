using LightPoint.IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources
{
    /// <summary>
    /// 用户登录日志
    /// </summary>
    public class ApplicationUserLoginedLog : LogBase
    {
        /// <summary>
        /// 关联的用户Id
        /// </summary>
        public Guid ApplicationUserId { get; set; }
        /// <summary>
        /// 关联的用户名
        /// </summary>
        public string? ApplicationUserName { get; set; }
        /// <summary>
        /// 输入的账户名
        /// </summary>
        public string? InputUserName { get; set; }
        /// <summary>
        /// 输入的密码值
        /// </summary>
        public string? InputPassword { get; set; }
        /// <summary>
        /// 是否成功登录
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 登录结果的解释
        /// </summary>
        public string? LoginResultDescription { get; set; }
        /// <summary>
        /// 登录类型
        /// </summary>
        public ApplicationUserLoginedLogType ApplicationUserLoginedLogType { get; set; }
    }
}
