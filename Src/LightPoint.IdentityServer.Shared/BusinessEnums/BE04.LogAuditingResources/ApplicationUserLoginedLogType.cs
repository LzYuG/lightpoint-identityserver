using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources
{
    public enum ApplicationUserLoginedLogType
    {
        账号密码登录,
        手机验证码登录,
        邮箱验证码登录,
        外部身份供应商登录
    }
}
