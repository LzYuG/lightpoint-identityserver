using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.PersonalBusiness.Models
{
    public class PasswordResetWithEmailValidationCode
    {
        [Required(ErrorMessage = "请输入邮箱地址")]
        public string? EmailAddress { get; set; }
        /// <summary>
        /// 人机校验码
        /// </summary>
        public string? HumanMachineVerificationCode { get; set; }
        [Required(ErrorMessage = "请输入新密码")]
        public string? NewPassword { get; set; }
        [Compare(nameof(NewPassword), ErrorMessage = "两次密码输入不一致")]
        public string? NewPasswordConfirm { get; set; }
    }
}
