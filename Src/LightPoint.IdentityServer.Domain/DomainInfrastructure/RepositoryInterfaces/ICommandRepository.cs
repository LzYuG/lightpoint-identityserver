using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.Shared.Helpers;

namespace LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces
{
    public interface ICommandRepository<Tid, T>
        where T : class, IDomainModelBase<Tid>, new()
    {
        #region 数据持久化
        /// <summary>
        /// 根据条件删除数据
        /// </summary>
        /// <param name="predicateExpression"></param>
        /// <returns></returns>
        Task<DataAccessResult> DeleteBoAsync(Expression<Func<T, bool>> predicateExpression);
        /// <summary>
        /// 更新一组数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bos"></param>
        /// <param name="includeExpressions"></param>
        /// <returns></returns>
        Task<DataAccessResult> UpdateAndSaveAsync(IEnumerable<T> bos, params Expression<Func<T, object>>[] includeExpressions);

        Task<DataAccessResult> UpdateAndSaveAsync(T bo, params Expression<Func<T, object>>[] includeExpressions);

        Task<DataAccessResult> LazyUpdateAsync(Expression<Func<T, bool>> predicateExpression,
            List<KeyValuePair<string, object>> propAndValues);
        #endregion

        #region 聚合对值对象的管理方法
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValueObjectId"></typeparam>
        /// <typeparam name="TValueObject"></typeparam>
        /// <param name="aggregateRootValue">防止执行与聚合无关的操作，聚合根仍需传递</param>
        /// <param name="predicateExpression">删除的条件表达式</param>
        /// <returns></returns>
        Task<DataAccessResult> DeleteValueObjects<TValueObjectId, TValueObject>(string aggregateRootName, Tid aggregateRootValue,
             Expression<Func<TValueObject, bool>>? predicateExpression)
             where TValueObject : class, IDomainModelBase<TValueObjectId>, new();

        #endregion
    }
}
