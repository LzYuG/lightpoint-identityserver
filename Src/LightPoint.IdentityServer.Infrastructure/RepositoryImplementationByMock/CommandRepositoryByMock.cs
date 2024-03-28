using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByMock.MockDatas;
using System.Linq.Expressions;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.Shared.Helpers;

namespace LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByMock
{
    public class CommandRepositoryByMock<Tid, T> : ICommandRepository<Tid, T>
        where T : class, IDomainModelBase<Tid>, new()
    {
        private readonly MockData _dataContext;

        public CommandRepositoryByMock(MockData mockData)
        {
            _dataContext = mockData;
        }

        public MockData DataContext { get => _dataContext; }


        #region 数据持久化

        public virtual async Task<DataAccessResult> DeleteBoAsync(Expression<Func<T, bool>> predicateExpression)
        {
            return await Task.Run(() =>
            {
                DataAccessResult dataStatus = new DataAccessResult();
                try
                {
                    var bos = _dataContext.Set<T>().Where(predicateExpression);
                    if (bos != null)
                    {
                        foreach (var bo in bos)
                        {
                            _dataContext.List<T>().Remove(bo);
                        }
                        dataStatus.DataStatusEnum = DataAccessResultEnum.操作成功;
                        dataStatus.Message = "删除成功";
                    }
                }
                catch (Exception ex)
                {
                    dataStatus.Message = ex.Message;
                    dataStatus.DataStatusEnum = DataAccessResultEnum.操作失败;
                }
                return dataStatus;
            });

        }


        /// <summary>
        /// 快速更新一组关系
        /// </summary>
        /// <returns></returns>
        public virtual async Task<DataAccessResult> UpdateAndSaveAsync(IEnumerable<T> bos, params Expression<Func<T, object>>[] includeExpressions)
        {
            return await Task.Run(() =>
            {
                List<Delegate> delegates = new List<Delegate>();
                if (bos != null)
                {
                    foreach (var bo in bos)
                    {
                        var index = _dataContext.List<T>().FindIndex(x => x.Id!.Equals(bo.Id));
                        if (index != -1)
                        {
                            // 更新
                            _dataContext.List<T>().RemoveAt(index);
                            _dataContext.List<T>().Add(bo);
                        }
                        else
                        {
                            _dataContext.List<T>().Add(bo);
                        }
                    }
                    try
                    {
                        return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作成功, Message = "操作成功" };
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作失败, Message = "操作失败" };
                    }
                }
                return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作失败, Message = "数据为空" };
            });
        }

        public virtual async Task<DataAccessResult> UpdateAndSaveAsync(T bo, params Expression<Func<T, object>>[] includeExpressions)
        {
            return await Task.Run(() =>
            {
                List<Delegate> delegates = new List<Delegate>();

                var index = _dataContext.List<T>().FindIndex(x => x.Id!.Equals(bo.Id));
                if (index != -1)
                {
                    _dataContext.List<T>().RemoveAt(index);
                    _dataContext.List<T>().Add(bo);
                }
                else
                {
                    _dataContext.List<T>().Add(bo);
                }
                try
                {
                    return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作成功, Message = "操作成功" };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作失败, Message = "操作失败" };
                }
            });

        }

        public virtual async Task<DataAccessResult> LazyUpdateAsync(Expression<Func<T, bool>> predicateExpression,
            List<KeyValuePair<string, object>> propAndValues)
        {
            return await Task.Run(() =>
            {
                try
                {
                    var boCollection = _dataContext.Set<T>().Where(predicateExpression).ToList();



                    boCollection.ForEach((bo) =>
                    {
                        var index = _dataContext.List<T>().FindIndex(x => x.Id!.Equals(bo.Id));
                        if (propAndValues != null)
                        {
                            foreach (var propAndValue in propAndValues)
                            {
                                var prop = typeof(T).GetProperty(propAndValue.Key);
                                if (propAndValue.Value == null)
                                {
                                    prop!.SetValue(_dataContext.List<T>()[index], propAndValue.Value);
                                }
                                else
                                {
                                    if (prop != null && prop.PropertyType != propAndValue.Value.GetType())
                                    {
                                        continue;
                                    }
                                    prop!.SetValue(_dataContext.List<T>()[index], propAndValue.Value);
                                }

                            }
                        }

                    });
                    return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作成功, Message = "更新成功" };
                }
                catch (Exception ex)
                {
                    return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作失败, Message = ex.Message };
                }
            });

        }
        #endregion


        #region 聚合对值对象的管理方法

        public virtual async Task<DataAccessResult> DeleteValueObjects<TValueObjectId, TValueObject>(string aggregateRootName, Tid aggregateRootValue,
            Expression<Func<TValueObject, bool>>? predicateExpression)
            where TValueObject : class, IDomainModelBase<TValueObjectId>, new()
        {
            return await Task.Run(() =>
            {
                // CreateLambda
                var searchLambda = LambdaCreater.GetPropertyEquals<TValueObject, Tid>(aggregateRootName, aggregateRootValue);
                if(predicateExpression != null)
                    searchLambda = searchLambda.ExpressionAnd(predicateExpression);
                var datas = DataContext.Set<TValueObject>().Where(searchLambda).ToList();
                foreach (var data in datas)
                {
                    DataContext.List<TValueObject>().Remove(data);
                }

                return DataAccessResult.Success("对子项的删除操作成功");
            });
        }

        #endregion
    }
}
