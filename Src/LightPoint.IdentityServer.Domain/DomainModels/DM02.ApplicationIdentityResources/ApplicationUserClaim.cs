using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Shared;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources
{
    /// <summary>
    /// 用户标识
    /// </summary>
    [Table("ApplicationUserClaims")]
    public class ApplicationUserClaim : IdentityUserClaim<Guid>, IDomainModelBase<int>
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
        /// 归属租户
        /// </summary>
        public string? TenantIdentifier { get; set; }

        public async Task<DomainModelValidationResult> DataValidation(RepositoryFactory repositoryFactory)
        {
            return await Task.Run(() => DomainModelValidationResult.Success());
        }

        public virtual async Task<DomainModelOperationRusult> BeforeAddOrUpdate(RepositoryFactory repositoryFactory)
        {
            return await Task.Run(() => DomainModelOperationRusult.Success());
        }

        public virtual async Task<DomainModelOperationRusult> BeforeDelete(RepositoryFactory repositoryFactory)
        {
            return await Task.Run(() => DomainModelOperationRusult.Success());
        }
    }
}
