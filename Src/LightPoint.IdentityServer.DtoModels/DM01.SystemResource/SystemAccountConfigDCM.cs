using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE01.SystemResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources
{
    public class SystemAccountConfigDCM : CommandDtoBase<Guid>
    {
        /// <summary>
        /// 允许用户自注册
        /// </summary>
        public bool SelfRegisterEnabled { get; set; } = false;
        /// <summary>
        /// 注册的时候强制校验手机号码
        /// </summary>
        public bool ValidationPhoneNumberWhenRegister { get; set; } = false;
        /// <summary>
        /// 注册的时候强制校验邮箱
        /// </summary>
        public bool ValidationEmailWhenRegister { get; set; } = false;

        /// <summary>
        /// 用户名校验的正则表达式
        /// </summary>
        public string? AccountUserNameValidationRegex { get; set; } = "^[a-zA-Z0-9_]{3,}$";
        public string? AccountUserNameValidateFailedMessage { get; set; } = "用户名必须是三位以上的数字或字母，字符仅支持下划线";

        /// <summary>
        /// 密码校验的正则表达式
        /// </summary>
        public string? AccountUserPasswordValidationRegex { get; set; } = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d@#$%^&*()_+\-=[\]{};':""\\|,.<>/?]{6,}$";
        public string? AccountUserPasswordValidateFailedMessage { get; set; } = "用户密码必须是6位以上的字符，包含大小写字母、字符、数字";
        /// <summary>
        /// 是否需要管理员审核
        /// </summary>
        public bool RequireAdminExamine { get; set; } = false;

        /// <summary>
        /// 最大登录失败次数
        /// </summary>
        public int MaxLoginFailCount { get; set; } = 5;
        /// <summary>
        /// 超过最大登录次数锁定时间（秒）
        /// </summary>
        public int LockAccountTime { get; set; } = 60 * 5;

        /// <summary>
        /// 是否强制绑定手机号码
        /// </summary>
        public bool MustConfirmPhoneNumber { get; set; }
        /// <summary>
        /// 是否强制绑定邮箱
        /// </summary>
        public bool MustConfirmEmail { get; set; }
        /// <summary>
        /// 全局MFA策略
        /// </summary>
        public SystemAccountConfigMFAPolicy SystemAccountConfigMFAPolicy { get; set; }
    }
}
