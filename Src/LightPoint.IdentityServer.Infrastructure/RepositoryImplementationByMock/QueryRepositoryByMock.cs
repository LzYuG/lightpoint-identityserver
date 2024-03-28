using System.Linq.Expressions;
using LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByMock.MockDatas;
using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.Shared.Helpers;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using LightPoint.IdentityServer.Shared.Extensions;

namespace LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByMock
{
    public class QueryRepositoryByMock<Tid, T> : IQueryRepository<Tid, T>
        where T : class, IDomainModelBase<Tid>, new()
    {
        private readonly MockData _dataContext;

        public QueryRepositoryByMock(MockData mockData)
        {
            _dataContext = mockData;
        }

        public MockData DataContext { get => _dataContext; }

        #region 获取数据集合数值的方法

        /// <summary>
        /// 根据条件，变量名，关联表达式，获取乘积的和， 多个属性名使用英文","分割
        /// 全部通过decimal返回
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicateExpression"></param>
        /// <param name="sumProperties"></param>
        /// <param name="includeExpressions"></param>
        /// <returns></returns>
        public virtual async Task<decimal> GetMultiplySumAsync(Expression<Func<T, bool>> predicateExpression, string sumProperties,
            int roundCount = 2)
        {
            return await Task.Run(() =>
            {
                IQueryable<T> query = _dataContext.Set<T>();
                decimal result;
                if (predicateExpression != null)
                {
                    query = query.Where(predicateExpression);
                }
                var lambda = LambdaCreater.GetBoPropertyMultiply<T>(sumProperties);
                result = query.Sum(lambda);
                return Math.Round(result, roundCount);
            });

        }

        public virtual async Task<int> GetAmountAsync(Expression<Func<T, bool>> predicateExpression) => await Task.Run(() => _dataContext.Set<T>().Count(predicateExpression));

        public virtual async Task<TResult> GetSumAsync<TResult>(Expression<Func<T, bool>> predicateExpression, string sumProperty)
        {
            return await Task.Run(() =>
            {
                IQueryable<T> query = _dataContext.Set<T>();
                object result = null!;
                if (predicateExpression != null)
                {
                    query = query.Where(predicateExpression);
                }
                if (typeof(TResult) == typeof(int))
                {
                    result = query.Sum(LambdaCreater.GetIntBoPropertyObj<T>(sumProperty));
                }
                else if (typeof(TResult) == typeof(long))
                {
                    result = query.Sum(LambdaCreater.GetLongBoPropertyObj<T>(sumProperty));
                }
                else if (typeof(TResult) == typeof(decimal))
                {
                    result = query.Sum(LambdaCreater.GetDecimalBoPropertyObj<T>(sumProperty));
                }
                else if (typeof(TResult) == typeof(float))
                {
                    result = query.Sum(LambdaCreater.GetFloatBoPropertyObj<T>(sumProperty));
                }
                else if (typeof(TResult) == typeof(double))
                {
                    result = query.Sum(LambdaCreater.GetDoubleBoPropertyObj<T>(sumProperty));
                }
                return (TResult)result!;
            });

        }
        public virtual async Task<decimal> GetAverageAsync<TResult>(Expression<Func<T, bool>> predicateExpression, string averageProperty)
        {
            return await Task.Run(() =>
            {
                IQueryable<T> query = _dataContext.Set<T>();
                object result = null!;
                if (predicateExpression != null)
                {
                    query = query.Where(predicateExpression);
                }
                if (typeof(TResult) == typeof(int))
                {
                    result = query.Average(LambdaCreater.GetIntBoPropertyObj<T>(averageProperty));
                }
                else if (typeof(TResult) == typeof(long))
                {
                    result = query.Average(LambdaCreater.GetLongBoPropertyObj<T>(averageProperty));
                }
                else if (typeof(TResult) == typeof(decimal))
                {
                    result = query.Average(LambdaCreater.GetDecimalBoPropertyObj<T>(averageProperty));
                }
                else if (typeof(TResult) == typeof(float))
                {
                    result = query.Average(LambdaCreater.GetFloatBoPropertyObj<T>(averageProperty));
                }
                else if (typeof(TResult) == typeof(double))
                {
                    result = query.Average(LambdaCreater.GetDoubleBoPropertyObj<T>(averageProperty));
                }
                return (decimal)result!;
            });

        }
        #endregion

        #region 获取单条数据
        public virtual async Task<T?> GetBoAsync(Expression<Func<T, bool>> predicateExpression, params Expression<Func<T, object>>[] includeExpressions)
        {
            return await Task.Run(() =>
            {
                IQueryable<T> query = _dataContext.Set<T>();
                return query.FirstOrDefault(predicateExpression);
            });

        }
        public virtual async Task<T?> GetBoAsync(Guid id, params Expression<Func<T, object>>[] includeExpressions)
        {
            return await Task.Run(() =>
            {
                IQueryable<T> query = _dataContext.Set<T>();

                return query.FirstOrDefault(x => x.Id!.Equals(id));
            });

        }

        public virtual async Task<T?> GetLastApiBoAsync(Expression<Func<T, bool>> predicateExpression,
            Expression<Func<T, object>> orderExpression, bool isDesc, params Expression<Func<T, object>>[] includeExpressions)
        {
            return await Task.Run(() =>
            {
                IQueryable<T> query = _dataContext.Set<T>();

                if (predicateExpression != null) query = query.Where(predicateExpression);
                // 是否降序
                if (isDesc && orderExpression != null) query = query.OrderByDescending(orderExpression);

                if (!isDesc && orderExpression != null) query = query.OrderBy(orderExpression);

                return query.LastOrDefault();
            });

        }
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
        public async Task<IQueryable<T>> GetBoCollectionAsync(int start, int takeAmount, Expression<Func<T, bool>> predicateExpression,
            bool PositiveOrder,
            Expression<Func<T, object>> orderPropertyExpression, params Expression<Func<T, object>>[] includeExpressions)
        {
            return await Task.Run(() =>
            {
                IQueryable<T> query = _dataContext.Set<T>();

                if (predicateExpression != null)
                    query = query.Where(predicateExpression);
                if (PositiveOrder)
                    query = query.OrderBy(orderPropertyExpression);
                else
                    query = query.OrderByDescending(orderPropertyExpression);
                if (takeAmount != 0)
                    query = query.Skip(start).Take(takeAmount);
                var result = query.ToList();
                return result.AsQueryable();
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchFormData"></param>
        /// <param name="includePropertiesExpression"></param>
        /// <returns></returns>
        public async Task<TableDataHelper<T>?> GetBoCollectionAsyncBySearchParams(SearchParams searchFormData,
            Expression<Func<T, bool>> predicateExpression, params Expression<Func<T, object>>[] includeExpressions)
        {
            return await Task.Run(() =>
            {
                if (searchFormData == null) return default;
                IQueryable<T> query = _dataContext.Set<T>();


                List<Expression<Func<T, bool>>> searchExpressions = new List<Expression<Func<T, bool>>>();
                if (!string.IsNullOrWhiteSpace(searchFormData.SearchTerm))
                {
                    // 如果有名字，就只搜名字
                    if (typeof(T).GetProperty("Name") != null)
                    {
                        searchExpressions.Add(LambdaCreater.GetContains<T>("Name", searchFormData.SearchTerm));
                    }
                    else
                    {
                        var properties = typeof(T).GetProperties();
                        foreach (var property in properties)
                        {
                            // 只搜索string类型
                            if (property.PropertyType == typeof(string))
                            {
                                searchExpressions.Add(LambdaCreater.GetContains<T>(property.Name, searchFormData.SearchTerm));
                            }
                        }
                    }
                }


                if (predicateExpression != null)
                {
                    query = query.Where(predicateExpression);
                }

                if (searchExpressions != null && searchExpressions.Count > 0)
                {
                    Expression<Func<T, bool>> searchExpression = searchExpressions[0];
                    if (searchExpressions.Count > 1)
                    {
                        for (var i = 1; i < searchExpressions.Count; i++)
                        {
                            searchExpression = searchExpression.ExpressionOr(searchExpressions[i]);
                        }
                    }
                    query = query.Where(searchExpression);
                }

                var filteredCount = query.Count();


                searchFormData.OrderProp = searchFormData.OrderProp.FirstToUpper();
                if (searchFormData.IsDesc)
                    query = query.OrderByDescending(LambdaCreater.GetBoProperty<T>(searchFormData.OrderProp));
                else
                    query = query.OrderBy(LambdaCreater.GetBoProperty<T>(searchFormData.OrderProp));

                if (searchFormData.Length != 0)
                    query = query.Skip(searchFormData.Start).Take(searchFormData.Length);
                else
                {
                    // 最多给100
                    query = query.Take(100);
                }

                var result = query.ToList();

                return new TableDataHelper<T>() { Datas = result.AsQueryable(), Total = filteredCount };
            });

        }
        #endregion


        #region 获取数据集合状态
        public async Task<bool> HasBoAsync(Expression<Func<T, bool>> predicateExpression) => await Task.Run(() =>
            _dataContext.Set<T>().Any(predicateExpression));
        #endregion


        #region 聚合获取值对象Queryable
        public virtual async Task<IQueryable<TValueObject>?> GetValueObjects<TValueObjectId, TValueObject>(string aggregateRootName, Tid aggregateRootValue)
            where TValueObject : class, IDomainModelBase<TValueObjectId>, new()
        {
            return await Task.Run(() =>
            {
                // CreateLambda
                var searchLambda = LambdaCreater.GetPropertyEquals<TValueObject, Tid>(aggregateRootName, aggregateRootValue);
                return DataContext.Set<TValueObject>().Where(searchLambda).AsQueryable();
            });
        }
        #endregion
    }
}
