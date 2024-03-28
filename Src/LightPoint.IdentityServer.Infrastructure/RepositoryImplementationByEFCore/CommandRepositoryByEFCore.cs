using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore.Contexts;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using LightPoint.IdentityServer.Shared.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore
{
    public class CommandRepositoryByEFCore<Tid, T> : ICommandRepository<Tid, T>
        where T : class, IDomainModelBase<Tid>, new()
    {
        private readonly ILightPointDbContextFactory _dbContextFactoryFactory;
        private readonly RepositoryFactory _repositoryFactory;

        public CommandRepositoryByEFCore(ILightPointDbContextFactory dbContextFactoryFactory, RepositoryFactory repositoryFactory)
        {
            _dbContextFactoryFactory = dbContextFactoryFactory;
            _repositoryFactory = repositoryFactory;
        }

        #region 数据持久化

        public virtual async Task<DataAccessResult> DeleteBoAsync(Expression<Func<T, bool>> predicateExpression)
        {
            
            var dataContext = await _dbContextFactoryFactory.CreateDbContextAsync();
            DataAccessResult dataStatus = new DataAccessResult();
            try
            {
                var bos = await dataContext.Set<T>().Where(predicateExpression).ToListAsync();
                if (bos != null)
                {
                    foreach(var bo in bos)
                    {
                        if ((await bo.BeforeDelete(_repositoryFactory)).Successed)
                        {
                            dataContext.Remove(bo);
                        }
                    }
                    await dataContext.SaveChangesAsync();
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
        }


        public virtual async Task<DataAccessResult> UpdateAndSaveAsync(IEnumerable<T> bos, params Expression<Func<T, object>>[] includeExpressions)
        {
            var dataContext = await _dbContextFactoryFactory.CreateDbContextAsync();
            var temps = new List<T>();
            foreach(var bo in bos)
            {
                if(bo.CreateTime == default)
                {
                    bo.CreateTime = DateTime.Now;
                }
                if((await bo.DataValidation(_repositoryFactory)).Successed && (await bo.BeforeAddOrUpdate(_repositoryFactory)).Successed)
                {
                    temps.Add(bo);
                }
            }
            bos = temps;
            var query = dataContext.Set<T>().AsQueryable();
            if (includeExpressions != null)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    query = query.Include(includeExpression);
                }
            }
            if (bos != null)
            {
                var props = typeof(T).GetProperties();
                foreach (var bo in bos)
                {
                    var dbData = await query.FirstOrDefaultAsync(x => x.Id!.Equals(bo.Id));
                    if (dbData != null)
                    {
                        foreach (var prop in props)
                        {
                            prop.SetValue(dbData, prop.GetValue(bo));
                        }
                    }
                    else
                    {
                        dataContext.Set<T>().Add(bo);
                    }
                }
                try
                {
                    await dataContext.SaveChangesAsync();
                    return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作成功, Message = "操作成功" };
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作失败, Message = "操作失败" };
                }
            }
            return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作失败, Message = "数据为空" };
        }

        public virtual async Task<DataAccessResult> UpdateAndSaveAsync(T bo, params Expression<Func<T, object>>[] includeExpressions)
        {
            if (bo.CreateTime == default)
            {
                bo.CreateTime = DateTime.Now;
            }
            var dataContext = await _dbContextFactoryFactory.CreateDbContextAsync();
            var validationResult = await bo.DataValidation(_repositoryFactory);
            if (!validationResult.Successed)
            {
                return DataAccessResult.Error("数据校验失败：" + validationResult.Message);
            }
            if(!(await bo.BeforeAddOrUpdate(_repositoryFactory)).Successed)
            {
                return DataAccessResult.Error("更新前置操作失败：" + validationResult.Message);
            }
            var query = dataContext.Set<T>().AsQueryable();
            if (includeExpressions != null)
            {
                foreach (var includeExpression in includeExpressions)
                {
                    query.Include(includeExpression);
                }
            }
            var props = typeof(T).GetProperties();
            var dbData = await query.FirstOrDefaultAsync(x => x.Id!.Equals(bo.Id));

            if (dbData != null)
            {
                foreach (var prop in props)
                {
                    prop.SetValue(dbData, prop.GetValue(bo));
                }
            }
            else
            {
                await dataContext.Set<T>().AddAsync(bo);
            }
            try
            {
                await dataContext.SaveChangesAsync();
                await dataContext.DisposeAsync();
                return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作成功, Message = "操作成功" };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作失败, Message = "操作失败" };
            }
        }

        public virtual async Task<DataAccessResult> LazyUpdateAsync(Expression<Func<T, bool>> predicateExpression,
            List<KeyValuePair<string, object>> propAndValues)
        {
            var dataContext = await _dbContextFactoryFactory.CreateDbContextAsync();
            try
            {
                var boCollection = dataContext.Set<T>().Where(predicateExpression);

                await boCollection.ForEachAsync((bo) =>
                {

                    dataContext.Attach(bo);
                    if (propAndValues != null)
                    {
                        foreach (var propAndValue in propAndValues)
                        {
                            var prop = typeof(T).GetProperty(propAndValue.Key);
                            if (propAndValue.Value == null)
                            {
                                prop!.SetValue(bo, propAndValue.Value);
                            }
                            else
                            {
                                if (prop != null && prop.PropertyType != propAndValue.Value.GetType())
                                {
                                    continue;
                                }
                                prop!.SetValue(bo, propAndValue.Value);
                                dataContext.Entry(bo).Property(propAndValue.Key).IsModified = true;
                            }

                        }
                    }

                });
                await dataContext.SaveChangesAsync();
                return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作成功, Message = "更新成功" };
            }
            catch (Exception ex)
            {
                return new DataAccessResult() { DataStatusEnum = DataAccessResultEnum.操作失败, Message = ex.Message };
            }
        }
        #endregion


        #region 聚合对值对象的管理方法
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TValueObjectId"></typeparam>
        /// <typeparam name="TValueObject"></typeparam>
        /// <param name="aggregateRootName">聚合根属性名称</param>
        /// <param name="aggregateRootValue">防止执行与聚合无关的操作，聚合根仍需传递</param>
        /// <param name="predicateExpression">删除的条件表达式</param>
        /// <returns></returns>
        public virtual async Task<DataAccessResult> DeleteValueObjects<TValueObjectId, TValueObject>(string aggregateRootName, Tid aggregateRootValue,
            Expression<Func<TValueObject, bool>>? predicateExpression)
            where TValueObject : class, IDomainModelBase<TValueObjectId>, new()
        {
            var dataContext = await _dbContextFactoryFactory.CreateDbContextAsync();
            // CreateLambda
            var searchLambda = LambdaCreater.GetPropertyEquals<TValueObject, Tid>(aggregateRootName, aggregateRootValue);
            if(predicateExpression != null)
            {
                searchLambda = searchLambda.ExpressionAnd(predicateExpression);
            }
            dataContext.Set<TValueObject>().RemoveRange(dataContext.Set<TValueObject>().Where(searchLambda));
            try
            {
                await dataContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                return DataAccessResult.Error("对子项的删除操作失败");
            }

            return DataAccessResult.Success("对子项的删除操作成功");
            
        }

        #endregion
    }
}
