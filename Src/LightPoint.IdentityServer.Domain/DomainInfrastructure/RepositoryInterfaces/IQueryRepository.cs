using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using LightPoint.IdentityServer.Domain.DomainModels;

namespace LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces
{
    public interface IQueryRepository<Tid, T>
        where T : class, IDomainModelBase<Tid>, new()
    {

        #region 获取数据集合数值的方法
        /// <summary>
        /// 根据条件获取对象数量
        /// </summary>
        /// <param name="predicateExpression"></param>
        /// <returns></returns>
        Task<int> GetAmountAsync(Expression<Func<T, bool>> predicateExpression);

        /// <summary>
        /// 根据条件，变量名，关联表达式，获取乘积的和， 多个属性名使用英文","分割
        /// 全部通过decimal返回
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicateExpression"></param>
        /// <param name="sumProperties"></param>
        /// <param name="includeExpressions"></param>
        /// <returns></returns>
        Task<decimal> GetMultiplySumAsync(Expression<Func<T, bool>> predicateExpression, string sumProperties,
            int roundCount = 2);
        /// <summary>
        /// 获取对象集合中的某个属性相加的值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicateExpression"></param>
        /// <param name="sumProperty"></param>
        /// <returns></returns>
        Task<TResult> GetSumAsync<TResult>(Expression<Func<T, bool>> predicateExpression, string sumProperty);
        /// <summary>
        /// 获取对象集合中的某个属性的平均值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicateExpression"></param>
        /// <param name="averageProperty"></param>
        /// <returns></returns>
        Task<decimal> GetAverageAsync<TResult>(Expression<Func<T, bool>> predicateExpression, string averageProperty);
        #endregion

        #region 获得单条数据
        /// <summary>
        /// 根据id获取实体模型并获取关联对象数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="includePropertiesExpression"></param>
        /// <returns></returns>
        Task<T?> GetBoAsync(Guid id, params Expression<Func<T, object>>[] includeExpressions);
        /// <summary>
        /// 根据条件获取实体模型并获取关联对象数据
        /// </summary>
        /// <param name="predicateExpression"></param>
        /// <returns></returns>
        Task<T?> GetBoAsync(Expression<Func<T, bool>> predicateExpression, params Expression<Func<T, object>>[] includeExpressions);

        Task<T?> GetLastApiBoAsync(Expression<Func<T, bool>> predicateExpression,
            Expression<Func<T, object>> orderExpression, bool isDesc, params Expression<Func<T, object>>[] includeExpressions);
        #endregion


        #region 获取集合数据
        /// <summary>
        /// 根据参数获取集合对象
        /// </summary>
        /// <param name="takeAmount">提取集合数据数量，如果为0，则提取全部</param>
        /// <param name="PositiveOrder">排序的方向，如果为true则为正序，为false则为倒序</param>
        /// <param name="predicateExpression">提取集合数据过滤条件，如果为Null，则提取全部</param>
        /// <param name="orderPropertyExpression">提取集合的排序属性，不允许为空</param>
        /// <param name="includePropertiesExpression"></param>
        /// <returns></returns>
        Task<IQueryable<T>> GetBoCollectionAsync(int start, int takeAmount, Expression<Func<T, bool>> predicateExpression,
            bool positiveOrder,
            Expression<Func<T, object>> orderPropertyExpression, params Expression<Func<T, object>>[] includeExpressions);

        Task<TableDataHelper<T>?> GetBoCollectionAsyncBySearchParams(SearchParams searchFormData,
            Expression<Func<T, bool>> predicateExpression, params Expression<Func<T, object>>[] includeExpressions);
        #endregion


        #region 获取数据集合状态
        /// <summary>
        /// 根据条件判断一条数据库是否已在数据库
        /// </summary>
        /// <param name="predicateExpression"></param>
        /// <returns></returns>
        Task<bool> HasBoAsync(Expression<Func<T, bool>> predicateExpression);
        #endregion

        #region 聚合获取值对象Queryable
        Task<IQueryable<TValueObject>?> GetValueObjects<TValueObjectId, TValueObject>(string aggregateRootName, Tid aggregateRootValue)
            where TValueObject : class, IDomainModelBase<TValueObjectId>, new();
        #endregion
    }
}
