using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources.ExtensionProperties;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.Shared;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources
{
    /// <summary>
    /// 系统用户
    /// </summary>
    [Table("ApplicationUsers")]
    public class ApplicationUser : IdentityUser<Guid>, IDomainModel<Guid>
    {
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 简介
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public Guid? AvatarId { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public string? SortCode { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 是否已经删除
        /// </summary>
        public bool IsDeleted { get; set; }
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
        /// 租户
        /// </summary>
        public string? TenantIdentifier { get; set; }
        /// <summary>
        /// 是否根用户
        /// </summary>
        public bool IsRoot { get; set; }
        /// <summary>
        /// 等待管理员审核中，如果开启了需要审核的选项，则用户自注册之后，还要经过管理员审核通过
        /// </summary>
        public bool WaitAdminExamine { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public List<ApplicationUserProperty>? Properties { get; set; }
        /// <summary>
        /// MFA凭据
        /// </summary>
        public List<ApplicationUserMultiFactorAuthentication>? ApplicationUserMultiFactorAuthentications { get; set; }
        /// <summary>
        /// 外部登录凭据
        /// </summary>
        public List<ApplicationUserExternalIdentityProviderCredential>? ApplicationUserExternalIdentityProviderCredentials { get; set; }

        public virtual async Task<DomainModelValidationResult> DataValidation(RepositoryFactory repositoryFactory)
        {
            var queryRepository = repositoryFactory.GetQueryRepository<Guid, ApplicationUser>();
            if (await queryRepository.HasBoAsync(x => x.UserName == this.UserName && x.Id != this.Id))
            {
                return DomainModelValidationResult.Fail("已有相同用户名的用户");
            }
            return DomainModelValidationResult.Success();
        }

        public virtual async Task<DomainModelOperationRusult> BeforeAddOrUpdate(RepositoryFactory repositoryFactory)
        {
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, ApplicationUser>();
            var dataAccessResult = await commandRepository.DeleteValueObjects<long, ApplicationUserProperty>(nameof(ApplicationUserProperty.ApplicationUserId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, ApplicationUserExternalIdentityProviderCredential>(nameof(ApplicationUserExternalIdentityProviderCredential.ApplicationUserId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, ApplicationUserMultiFactorAuthentication>(nameof(ApplicationUserMultiFactorAuthentication.ApplicationUserId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            return new DomainModelOperationRusult(dataAccessResult);
        }

        public virtual async Task<DomainModelOperationRusult> BeforeDelete(RepositoryFactory repositoryFactory)
        {
            if (IsRoot)
            {
                return DomainModelOperationRusult.Fail("无法删除根用户");
            }
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, ApplicationUser>();
            var dataAccessResult = await commandRepository.DeleteValueObjects<long, ApplicationUserProperty>(nameof(ApplicationUserProperty.ApplicationUserId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, ApplicationUserExternalIdentityProviderCredential>(nameof(ApplicationUserExternalIdentityProviderCredential.ApplicationUserId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            dataAccessResult = await commandRepository.DeleteValueObjects<long, ApplicationUserMultiFactorAuthentication>(nameof(ApplicationUserMultiFactorAuthentication.ApplicationUserId), Id, null);
            if (!dataAccessResult.IsSuccess) return new DomainModelOperationRusult(dataAccessResult);
            return new DomainModelOperationRusult(dataAccessResult);
        }
    }
}
