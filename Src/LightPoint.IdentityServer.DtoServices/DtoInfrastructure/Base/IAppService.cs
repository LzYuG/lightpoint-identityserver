using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels;
using System.Linq.Expressions;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base
{
    public interface IAppService<Tid, TDomain, TQuery, TCommand>
        where TDomain : class, IDomainModelBase<Tid>, new()
        where TQuery : class, IQueryDtoBase<Tid>, new()
        where TCommand : class, ICommandDtoBase<Tid>, new()
    {
        IQueryRepository<Tid, TDomain> QueryRepository { get; }

        ICommandRepository<Tid, TDomain> CommandRepository { get; }
        IModelMapper<Tid, TDomain, TQuery, TCommand> ModelMapper { get; }

        #region 获取数据集合数值的方法
        Task<decimal> GetMultiplySumAsync(Expression<Func<TDomain, bool>> predicateExpression, string sumProperties,
            int roundCount = 2);
        Task<int> GetApiAmountAsync(Expression<Func<TDomain, bool>> predicateExpression);
        /// <summary>
        /// 获取对象集合中的某个属性相加的值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicateExpression"></param>
        /// <param name="sumProperty"></param>
        /// <returns></returns>
        Task<TResult> GetSumAsync<TResult>(Expression<Func<TDomain, bool>> predicateExpression, string sumProperty);
        /// <summary>
        /// 获取对象集合中的某个属性的平均值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicateExpression"></param>
        /// <param name="averageProperty"></param>
        /// <returns></returns>
        Task<decimal> GetAverageAsync<TResult>(Expression<Func<TDomain, bool>> predicateExpression, string averageProperty);

        #endregion

        #region 获取单条数据
        Task<TQuery?> GetApiBoAsync(Expression<Func<TDomain, bool>> predicateExpression, params Expression<Func<TDomain, object>>[] includePropertiesExpression);
        Task<TQuery?> GetApiBoAsync(Guid id, params Expression<Func<TDomain, object>>[] includePropertiesExpressions);
        Task<TQuery?> GetApiBoAsync(Expression<Func<TDomain, bool>> predicateExpression, bool isAutoSetIncludeExpression);
        Task<TQuery?> GetApiBoAsync(Guid id, bool isAutoSetIncludeExpression);
        /// <summary>
        /// 获取最后一条数据
        /// </summary>
        /// <param name="orderExpression"></param>
        /// <param name="isDesc"></param>
        /// <param name="includePropertiesExpression"></param>
        /// <returns></returns>
        Task<TQuery?> GetLastApiBoAsync(Expression<Func<TDomain, bool>> predicateExpression,
            Expression<Func<TDomain, object>> orderExpression,
            bool isDesc, params Expression<Func<TDomain, object>>[] includePropertiesExpression);

        Task<TQuery?> GetLastApiBoAsync(Expression<Func<TDomain, bool>> predicateExpression,
            Expression<Func<TDomain, object>> orderExpression,
            bool isDesc, bool isAutoSetIncludeExpression);
        #endregion


        #region 获取集合数据
        Task<List<TQuery>> GetApiBoCollectionAsync(Expression<Func<TDomain, bool>> predicateExpression,
            Expression<Func<TDomain, object>> orderExpression,
            params Expression<Func<TDomain, object>>[] includePropertiesExpression);

        Task<List<TQuery>> GetApiBoCollectionAsync(Expression<Func<TDomain, bool>> predicateExpression,
            Expression<Func<TDomain, object>> orderExpression,
            bool isAutoSetIncludeExpression);

        Task<List<TQuery>> GetApiBoCollectionAsync(int start, int takeAmount, Expression<Func<TDomain, bool>> predicateExpression,
            bool PositiveOrder,
            Expression<Func<TDomain, object>> orderPropertyExpression,
            bool isAutoSetIncludeExpression);

        Task<TableDataHelper<TQuery>> GetApiBoCollectionAsyncBySearchData(SearchParams searchFormData,
            Expression<Func<TDomain, bool>> predicateExpression,
            bool isAutoSetIncludeExpression);

        /// <summary>
        /// 获取一组Api模型数据
        /// </summary>
        /// <param name="takeAmount">获取的数据的条数</param>
        /// <param name="predicateExpression">条件表达式</param>
        /// <param name="PositiveOrder">是否正序</param>
        /// <param name="orderPropertyExpression">排序表达式</param>
        /// <param name="includePropertiesExpression">关联表达式</param>
        /// <returns></returns>
        Task<List<TQuery>> GetApiBoCollectionAsync(int start, int takeAmount, Expression<Func<TDomain, bool>> predicateExpression,
            bool PositiveOrder,
            Expression<Func<TDomain, object>> orderPropertyExpression,
            params Expression<Func<TDomain, object>>[] includePropertiesExpression);

        Task<TableDataHelper<TQuery>> GetApiBoCollectionAsyncBySearchData(SearchParams searchFormData,
            Expression<Func<TDomain, bool>> predicateExpression,
            params Expression<Func<TDomain, object>>[] includePropertiesExpression);
        #endregion




        #region 获取数据集合状态
        Task<bool> HasBoAsync(Expression<Func<TDomain, bool>> predicateExpression);
        #endregion

        #region 数据持久化
        Task<DataAccessResult> DeleteBoAsync(Expression<Func<TDomain, bool>> predicateExpression);
        Task<DataAccessResult> LazyUpdateAsync(Expression<Func<TDomain, bool>> predicateExpression,
            List<KeyValuePair<string, object>> propAndValues);

        Task<DataAccessResult> SetAndSaveEntityData(IEnumerable<TCommand> apiEntitys, params Expression<Func<TDomain, object>>[] expressions);

        Task<DataAccessResult> SetAndSaveEntityData(TCommand apiEntity, params Expression<Func<TDomain, object>>[] expressions);

        Task<DataAccessResult> SetAndSaveEntityData(IEnumerable<TCommand> apiEntitys, bool isAutoSetIncludeExpression);

        Task<DataAccessResult> SetAndSaveEntityData(TCommand apiEntity, bool isAutoSetIncludeExpression);
        #endregion


    }
}
