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
    public class ServerShortMessageServiceConfigDCM : CommandDtoBase<Guid>
    {
        public string? PlatformType { get; set; }

        public string? SMSAccount { get; set; }
        [Confidential]
        public string? SMSPassword { get; set; }

        public string? SMSHost { get; set; }

        public int SMSPort { get; set; }

        /// <summary>
        /// 开启每日发送上限
        /// </summary>
        public bool EnableDailySendLimit { get; set; }
        /// <summary>
        /// 同IP每日发送的上限
        /// </summary>
        public int DailySendLimitWithSameIP { get; set; } = 100;
        /// <summary>
        /// 同IP同号码每日发送的上限
        /// </summary>
        public int DailySendLimitWithSameIPAndPhone { get; set; } = 5;
        /// <summary>
        /// 手机号校验的正则表达式
        /// </summary>
        public string? PhoneNumberValidationRegex { get; set; } = "^1(3[0-9]|4[01456879]|5[0-35-9]|6[2567]|7[0-8]|8[0-9]|9[0-35-9])\\d{8}$";
        public string? PhoneNumberValidateFailedMessage { get; set; } = "请输入中国范围内11位手机号码";


        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 一般情况下，短信模板是需要对应短信平台审核的，该属性仅仅是作一个显示
        /// </summary>
        public string? ValidationCodeTemplate { get; set; }
        /// <summary>
        /// 短信验证码模板id
        /// </summary>
        public string? ValidationCodeTemplateId { get; set; }
        /// <summary>
        /// 短信验证码签名
        /// </summary>
        public string? ValidationCodeSignName { get; set; }
        /// <summary>
        /// 短信验证码参数填充值
        /// 如阿里云短信： { code : "666666"} 配置为=> { code : {CODE} }
        /// </summary>
        public string? ValidationCodeValueParam { get; set; }
    }
}
