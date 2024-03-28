using LightPoint.IdentityServer.DtoModels.Base;

namespace LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources
{
    /// <summary>
    /// 系统用户
    /// </summary>
    public class ApplicationUserDCM : CommandDto<Guid>
    {
        /// <summary>
        /// 用户头像
        /// </summary>
        public Guid? AvatarId { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 是否开启多因子认证
        /// 与TwoFactorEnabled对应，
        /// TwoFactorEnabled   开启则从ApplicationUserMultiFactorAuthentication任选一个认证方式
        /// MutilFactorEnabled 开启则需要完成ApplicationUserMultiFactorAuthentication绑定的全部认证方式
        /// </summary>
        public bool MutilFactorEnabled { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public List<string>? Properties { get; set; }
        /// <summary>
        /// Claims
        /// </summary>
        public List<ApplicationUserClaimDM>? ApplicationUserClaims { get; set; }

        public List<ApplicationUserMultiFactorAuthenticationDM>? ApplicationUserMultiFactorAuthentications { get; set; }

        public List<ApplicationUserExternalIdentityProviderCredentialDM>? ApplicationUserExternalIdentityProviderCredentials { get; set; }

        /// <summary>
        /// 是否根用户
        /// </summary>
        public bool IsRoot { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string? Password { get; set; }

        public string? ConfirmPassword { get; set; }

        #region IdentityUser

        //
        // 摘要:
        //     Gets or sets the user name for this user.
        public virtual string? UserName { get; set; }

        //
        // 摘要:
        //     Gets or sets the normalized user name for this user.
        public virtual string? NormalizedUserName { get; set; }

        //
        // 摘要:
        //     Gets or sets the email address for this user.
        public virtual string? Email { get; set; }

        //
        // 摘要:
        //     Gets or sets the normalized email address for this user.
        public virtual string? NormalizedEmail { get; set; }

        //
        // 摘要:
        //     Gets or sets a flag indicating if a user has confirmed their email address.
        //
        // 值:
        //     True if the email address has been confirmed, otherwise false.
        public virtual bool EmailConfirmed { get; set; }

        //
        // 摘要:
        //     Gets or sets a salted and hashed representation of the password for this user.
        public virtual string? PasswordHash { get; set; }

        //
        // 摘要:
        //     A random value that must change whenever a users credentials change (password
        //     changed, login removed)
        public virtual string? SecurityStamp { get; set; }

        //
        // 摘要:
        //     A random value that must change whenever a user is persisted to the store
        public virtual string? ConcurrencyStamp { get; set; } = Guid.NewGuid().ToString();


        //
        // 摘要:
        //     Gets or sets a telephone number for the user.
        public virtual string? PhoneNumber { get; set; }

        //
        // 摘要:
        //     Gets or sets a flag indicating if a user has confirmed their telephone address.
        //
        // 值:
        //     True if the telephone number has been confirmed, otherwise false.
        public virtual bool PhoneNumberConfirmed { get; set; }

        //
        // 摘要:
        //     Gets or sets a flag indicating if two factor authentication is enabled for this
        //     user.
        //
        // 值:
        //     True if 2fa is enabled, otherwise false.
        public virtual bool TwoFactorEnabled { get; set; }

        //
        // 摘要:
        //     Gets or sets the date and time, in UTC, when any user lockout ends.
        //
        // 言论：
        //     A value in the past means the user is not locked out.
        public virtual DateTimeOffset? LockoutEnd { get; set; }

        //
        // 摘要:
        //     Gets or sets a flag indicating if the user could be locked out.
        //
        // 值:
        //     True if the user could be locked out, otherwise false.
        public virtual bool LockoutEnabled { get; set; }

        //
        // 摘要:
        //     Gets or sets the number of failed login attempts for the current user.
        public virtual int AccessFailedCount { get; set; }
        #endregion
    }
}
