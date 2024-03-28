using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels
{
    public interface IDomainModelBase<T> : IData<T>
    {
        /// <summary>
        /// 领域模型需要提供自我数据校验的方法
        /// </summary>
        /// <returns></returns>
        Task<DomainModelValidationResult> DataValidation(RepositoryFactory repositoryFactory);

        #region 如果是聚合，则应对值对象进行管理
        Task<DomainModelOperationRusult> BeforeAddOrUpdate(RepositoryFactory repositoryFactory);
        Task<DomainModelOperationRusult> BeforeDelete(RepositoryFactory repositoryFactory);
        #endregion
    }
}
