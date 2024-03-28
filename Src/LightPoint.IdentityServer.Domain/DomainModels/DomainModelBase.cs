
using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels
{
    public class DomainModelBase<T> : IDomainModelBase<T>
    {
        public T Id { get; set; } = default!;
        public string? SortCode { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public string? TenantIdentifier { get; set; }
        public virtual async Task<DomainModelValidationResult> DataValidation(RepositoryFactory repositoryFactory)
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
