using LightPoint.IdentityServer.Shared.BusinessEnums.BE02.ApplicationIdentityResources;
using System.ComponentModel.DataAnnotations;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models
{
    public class LoginInputModel
    {
        #region 账号密码
        public string? Username { get; set; }
        public string? Password { get; set; }
        #endregion

        #region 短信验证码
        public string? PhoneNumber { get; set; }
        public string? ShortMessageCode { get; set; }
        #endregion

        #region 邮箱验证码
        public string? Email { get; set; }
        public string? EmailCode { get; set; }
        #endregion

        public bool RememberLogin { get; set; }
        public string? ReturnUrl { get; set; }

        public LoginType LoginType { get; set; }
        /// <summary>
        /// 如果需要MFA校验，会携带MFA通过的证明，一般是由用户开启的MFA数据的Secret字段作为密钥生成
        /// </summary>
        public string? MFAToken { get; set; }
        /// <summary>
        /// 如果是2FA的情况下，使用的校验方式
        /// </summary>
        public ApplicationUserMultiFactorAuthenticationType TowFactorAuthenticationType { get; set; }
    }


    public enum LoginType
    {
        账号密码,
        短信验证码,
        邮箱验证码
    }
}