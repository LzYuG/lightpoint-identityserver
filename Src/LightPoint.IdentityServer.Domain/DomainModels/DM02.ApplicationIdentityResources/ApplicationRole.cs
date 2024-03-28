using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources.ExtensionProperties;
using LightPoint.IdentityServer.Shared;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources
{
    [Table("ApplicationRoles")]
    public class ApplicationRole : IdentityRole<Guid>, IDomainModel<Guid>
    {
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
        /// 是否根角色
        /// </summary>
        public bool IsRoot { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 归属租户
        /// </summary>
        public string? TenantIdentifier { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public List<ApplicationRoleProperty>? Properties { get; set; }

        public virtual async Task<DomainModelOperationRusult> BeforeAddOrUpdate(RepositoryFactory repositoryFactory)
        {
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, ApplicationRole>();
            return new DomainModelOperationRusult(await commandRepository.DeleteValueObjects<long, ApplicationRoleProperty>(nameof(ApplicationRoleProperty.ApplicationRoleId), Id, null));
        }

        public virtual async Task<DomainModelOperationRusult> BeforeDelete(RepositoryFactory repositoryFactory)
        {
            if (IsRoot)
            {
                return DomainModelOperationRusult.Fail("无法删除根角色");
            }
            var commandRepository = repositoryFactory.GetCommandRepository<Guid, ApplicationRole>();
            return new DomainModelOperationRusult(await commandRepository.DeleteValueObjects<long, ApplicationRoleProperty>(nameof(ApplicationRoleProperty.ApplicationRoleId), Id, null));
        }

        public virtual async Task<DomainModelValidationResult> DataValidation(RepositoryFactory repositoryFactory)
        {
            var queryRepository = repositoryFactory.GetQueryRepository<Guid, ApplicationRole>();
            if (await queryRepository.HasBoAsync(x => x.TenantIdentifier == this.TenantIdentifier && x.Name == this.Name && x.Id != this.Id))
            {
                return DomainModelValidationResult.Fail("当前租户归属下，已有同名角色");
            }
            return DomainModelValidationResult.Success();
        }

    }
}
