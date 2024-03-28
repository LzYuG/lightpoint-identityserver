using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;
using LightPoint.IdentityServer.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.DM01.SystemResource
{
    /// <summary>
    /// 邮箱服务配置
    /// </summary>
    public class ServerEmailConfigDQM : QueryDtoBase<Guid>
    {
        public string? PlatformType { get; set; }

        public string? SMTPServerHost { get; set; }

        public int SMTPServerPort { get; set; }

        public string? From { get; set; }
        public string? FromName { get; set; }
        public string? Account { get; set; }
        [Confidential]
        public string? Password { get; set; }

        /// <summary>
        /// 开启每日发送上限
        /// </summary>
        public bool EnableDailySendLimit { get; set; }
        /// <summary>
        /// 同IP每日发送的上限
        /// </summary>
        public int DailySendLimitWithSameIP { get; set; } = 100;
        /// <summary>
        /// 同IP同邮箱每日发送的上限
        /// </summary>
        public int DailySendLimitWithSameIPAndEmail { get; set; } = 5;

        //// <summary>
        /// 邮箱号码校验的正则表达式
        /// </summary>
        public string? EmailValidationRegex { get; set; } = @"^[\w-]+(\.[\w-]+)*@[\w-]+(\.[\w-]+)+$";
        public string? EmailValidateFailedMessage { get; set; } = @"请输入合规的邮箱地址";

        /// <summary>
        /// 回复地址
        /// </summary>
        public string? ReplyTo { get; set; }
        /// <summary>
        /// 回复人
        /// </summary>
        public string? ReplyToName { get; set; }
        /// <summary>
        /// 发送邮件的客户端名称，一般作为邮件内容的标题
        /// </summary>
        public string? EmailClientName { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        #region 验证码
        /// <summary>
        /// 验证码邮件模板值
        /// </summary>
        public string? ValidationCodeTemplate { get; set; }
        /// <summary>
        /// 如不设置验证码邮件模板值，则以路径读取的验证码邮件模板值为准
        /// </summary>
        public string? ValidationCodeTemplatePath { get; set; } = "/templates/email/EmailValidationCode.html";
        /// <summary>
        /// 验证码邮件主题
        /// </summary>
        public string? ValidationCodeSubject { get; set; }
        #endregion
    }
}
