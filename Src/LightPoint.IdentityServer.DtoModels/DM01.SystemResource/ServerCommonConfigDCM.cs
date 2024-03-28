
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared.BusinessEnums.BE01.SystemResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.DM01.SystemResource
{
    public class ServerCommonConfigDCM : CommandDtoBase<Guid>
    {
        /// <summary>
        /// 登出后自动回调
        /// </summary>
        public bool AutoRedirectWhenLogouted { get; set; } = true;
        /// <summary>
        /// 允许用户自行重置密码
        /// </summary>
        public bool EnableUserSelfResetPassword { get; set; } = true;
        /// <summary>
        /// 允许使用邮箱验证码重置密码
        /// </summary>
        public bool EnableResetPasswordByEmail { get; set; } = true;
        /// <summary>
        /// 允许使用手机验证码重置密码
        /// </summary>
        public bool EnableResetPasswordByPhone { get; set; }
        /// <summary>
        /// 允许使用手机号码（验证码）登录
        /// </summary>
        public bool EnableLoginWithPhoneNumber { get; set; }
        /// <summary>
        /// 允许使用邮箱（验证码）登录
        /// </summary>
        public bool EnableLoginWithEmail { get; set; }
        /// <summary>
        /// 允许登录的时候选择记住我的登录（长效登录）
        /// </summary>
        public bool AllowRemenberme { get; set; } = true;
        /// <summary>
        /// 启用人机校验功能
        /// </summary>
        public bool EnableHumanMachineVerification { get; set; } = true;
        /// <summary>
        /// 人机校验类型
        /// </summary>
        public HumanMachineVerificationType HumanMachineVerificationType { get; set; } = HumanMachineVerificationType.随机字符校验码;


        /// <summary>
        /// 是否自动清除日志
        /// </summary>
        public bool IsAutoClearLogs { get; set; }
        /// <summary>
        /// 日志保存天数
        /// </summary>
        public int LogsSaveDays { get; set; } = 90;
    }
}
