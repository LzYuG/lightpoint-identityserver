using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.Home.Models
{
    public class InitorModel
    {
        [Required(ErrorMessage = "根租户名称不能为空")]
        public string? RootTenant { get; set; } = "根租户";
        [Required(ErrorMessage = "根租户唯一标识不能为空")]
        public string? RootTenantIdentifier { get; set; } = "root";
        [Required(ErrorMessage = "最高权限角色名称不能为空")]
        public string? SystemAdminRoleName { get; set; } = "SystemSuperAdmin";
        [Required(ErrorMessage = "最高权限角色显示名称不能为空")]
        public string? SystemAdminRoleDisplayName { get; set; } = "系统超级管理员";
        [Required(ErrorMessage = "管理员用户名不能为空")]
        public string? AdminUserName { get; set; } = "admin";
        [Required(ErrorMessage = "管理员密码不能为空")]
        public string? AdminUserPassword { get; set; } = "";
        [Required(ErrorMessage = "管理员密码确认不能为空")]
        [Compare(nameof(AdminUserPassword), ErrorMessage = "两次输入的密码不一样")]
        public string? AdminUserPasswordConfirm { get; set; } = "";
    }
}
