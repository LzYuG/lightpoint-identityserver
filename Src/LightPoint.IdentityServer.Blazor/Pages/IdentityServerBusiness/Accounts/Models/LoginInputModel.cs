using LightPoint.IdentityServer.Shared.BusinessEnums.BE02.ApplicationIdentityResources;
using System.ComponentModel.DataAnnotations;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models
{
    public class LoginInputModel
    {
        #region �˺�����
        public string? Username { get; set; }
        public string? Password { get; set; }
        #endregion

        #region ������֤��
        public string? PhoneNumber { get; set; }
        public string? ShortMessageCode { get; set; }
        #endregion

        #region ������֤��
        public string? Email { get; set; }
        public string? EmailCode { get; set; }
        #endregion

        public bool RememberLogin { get; set; }
        public string? ReturnUrl { get; set; }

        public LoginType LoginType { get; set; }
        /// <summary>
        /// �����ҪMFAУ�飬��Я��MFAͨ����֤����һ�������û�������MFA���ݵ�Secret�ֶ���Ϊ��Կ����
        /// </summary>
        public string? MFAToken { get; set; }
        /// <summary>
        /// �����2FA������£�ʹ�õ�У�鷽ʽ
        /// </summary>
        public ApplicationUserMultiFactorAuthenticationType TowFactorAuthenticationType { get; set; }
    }


    public enum LoginType
    {
        �˺�����,
        ������֤��,
        ������֤��
    }
}