using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models
{
    public class ResetPasswordModel
    {
        public string? PhoneNumber { get; set; }
        public string? ShortMessageCode { get; set; }
        public string? Email { get; set; }
        public string? EmailCode { get; set; }

        public string? NewPassword { get; set; }

        public string? NewPasswordConfirm { get; set; }
    }

    public enum ResetPasswordType
    {
        短信验证码,
        邮箱验证码,
    }
}
