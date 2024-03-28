using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.PersonalBusiness.Models
{
    public class PasswordResetWithOldPassword
    {
        [Required(ErrorMessage = "请输入旧密码")]
        public string? OldPassword { get; set; }
        [Required(ErrorMessage = "请输入新密码")]
        public string? NewPassword { get; set; }
        [Compare(nameof(NewPassword), ErrorMessage = "两次密码输入不一致")]
        public string? NewPasswordConfirm { get; set; }
    }
}
