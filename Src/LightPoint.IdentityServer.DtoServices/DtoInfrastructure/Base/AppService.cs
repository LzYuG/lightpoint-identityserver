using LightPoint.IdentityServer.Shared;
using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels;
using System.Linq.Expressions;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using LightPoint.IdentityServer.Shared.Helpers;
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base
{
    public class AppService<Tid, TDomain, TQuery, TCommand> : IAppService<Tid, TDomain, TQuery, TCommand>
        where TDomain : class, IDomainModelBase<Tid>, new()
        where TQuery : class, IQueryDtoBase<Tid>, new()
        where TCommand : class, ICommandDtoBase<Tid>, new()
    {

        public IQueryRepository<Tid, TDomain> QueryRepository { get; }
        public ICommandRepository<Tid, TDomain> CommandRepository { get; }
        public IModelMapper<Tid, TDomain, TQuery, TCommand> ModelMapper { get; }

        public AppService(IQueryRepository<Tid, TDomain> queryRepository,
            ICommandRepository<Tid, TDomain> commandRepository,
            IModelMapper<Tid, TDomain, TQuery, TCommand> modelMapper)
        {
            QueryRepository = queryRepository;
            CommandRepository = commandRepository;
            ModelMapper = modelMapper;
        }

        #region 辅助方法

        /// <summary>
        /// 设置自引用关系的缩进
        /// </summary>
        /// <returns></returns>
        private List<TQuery> SetIndent<TOther>(IEnumerable<TQuery> boAms, IEnumerable<TOther> bos) where TOther : class, IData<Tid>, new()
        {
            for (var i = 0; i < bos.Count(); i++)
            {
                string str = "";
                SetIndentHelper(ref str, bos.ElementAt(i));
                boAms.ElementAt(i).GetType().GetProperty("Name")!.SetValue(boAms.ElementAt(i), str + LambdaCreater.GetPropertyValue<TQuery>(boAms.ElementAt(i), "Name"));
            }
            return boAms.ToList();
        }
        /// <summary>
        /// 设置自引用的递归方法
        /// </summary>
        /// <typeparam name="TOther"></typeparam>
        /// <param name="selectAM"></param>
        /// <param name="bo"></param>
        private void SetIndentHelper<TOther>(ref string str, TOther bo) where TOther : class, IData<Tid>, new()
        {
            if (LambdaCreater.GetProperty<TOther>(bo, "Parent") != null)
            {
                if (LambdaCreater.GetProperty<TOther>((TOther)LambdaCreater.GetProperty<TOther>(bo, "Parent")!, "Parent") != null)
                {
                    if (((TOther)LambdaCreater.GetProperty<TOther>((TOther)LambdaCreater.GetProperty<TOther>(bo, "Parent")!, "Parent")!).Id!.Equals(bo.Id) == true)
                    {
                        return;
                    }
                }
                if (string.IsNullOrEmpty(str))
                {
                    str += "└";
                }
                else
                {
                    str += "─";
                }
                var parentBo = LambdaCreater.GetConvertProperty<Tid, TOther, TOther>(bo, "Parent");
                if (!parentBo.Id!.Equals(bo.Id) || LambdaCreater.GetConvertProperty<Tid, TOther, TOther>(parentBo, "Parent").Id!.Equals(bo.Id))
                {
                    SetIndentHelper(ref str, parentBo);
                }

            }
        }
        #endregion

        #region 获取数据集合数值的方法

        public virtual async Task<decimal> GetMultiplySumAsync(Expression<Func<TDomain, bool>> predicateExpression, string sumProperties,
            int roundCount = 2)
        {
            return await QueryRepository.GetMultiplySumAsync(predicateExpression, sumProperties, roundCount);
        }

        public virtual async Task<int> GetApiAmountAsync(Expression<Func<TDomain, bool>> predicateExpression)
        {
            if (predicateExpression != null)
                return await QueryRepository.GetAmountAsync(predicateExpression);
            else
                return 0;
        }
        /// <summary>
        /// 获取对象集合中的某个属性相加的值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicateExpression"></param>
        /// <param name="sumProperty"></param>
        /// <returns></returns>
        public virtual async Task<TResult> GetSumAsync<TResult>(Expression<Func<TDomain, bool>> predicateExpression, string sumProperty)
        {
            return await QueryRepository.GetSumAsync<TResult>(predicateExpression, sumProperty);
        }
        /// <summary>
        /// 获取对象集合中的某个属性的平均值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="predicateExpression"></param>
        /// <param name="averageProperty"></param>
        /// <returns></returns>
        public virtual async Task<decimal> GetAverageAsync<TResult>(Expression<Func<TDomain, bool>> predicateExpression, string averageProperty)
        {
            return await QueryRepository.GetAverageAsync<TResult>(predicateExpression, averageProperty);
        }
        #endregion
        #region 获取数据集合状态
        /// <summary>
        /// 判断一条数据是否已在数据库
        /// </summary>
        /// <param name="bo"></param>
        /// <returns></returns>

        public virtual async Task<bool> HasBoAsync(Expression<Func<TDomain, bool>> predicateExpression)
             => await QueryRepository.HasBoAsync(predicateExpression);
        #endregion

        #region 获取单条数据

        public virtual async Task<TQuery?> GetApiBoAsync(Expression<Func<TDomain, bool>> predicateExpression, params Expression<Func<TDomain, object>>[] includePropertiesExpression)
        {
            var bo = await QueryRepository.GetBoAsync(predicateExpression, includePropertiesExpression);
            if (bo == null)
            {
                return null;
            }
            return await ModelMapper.ToQueryDto(bo);
        }

        public virtual async Task<TQuery?> GetApiBoAsync(Guid id, params Expression<Func<TDomain, object>>[] includePropertiesExpression)
        {
            var bo = await QueryRepository.GetBoAsync(id, includePropertiesExpression);
            if (bo == null)
            {
                return null;
            }
            return await ModelMapper.ToQueryDto(bo);
        }

        public virtual async Task<TQuery?> GetApiBoAsync(Expression<Func<TDomain, bool>> predicateExpression, bool isAutoSetIncludeExpression)
        {
            var expressions = LambdaCreater.GetIncludeExpressions<TDomain>();
            return await GetApiBoAsync(predicateExpression, expressions);
        }

        public virtual async Task<TQuery?> GetApiBoAsync(Guid id, bool isAutoSetIncludeExpression)
        {
            var expressions = LambdaCreater.GetIncludeExpressions<TDomain>();
            return await GetApiBoAsync(id, expressions);
        }


        /// <summary>
        /// 获取最后一条数据
        /// </summary>
        /// <returns></returns>
        public virtual async Task<TQuery?> GetLastApiBoAsync(Expression<Func<TDomain, bool>> predicateExpression,
            Expression<Func<TDomain, object>> orderExpression,
            bool isDesc, params Expression<Func<TDomain, object>>[] includePropertiesExpressions)
        {
            var bo = await QueryRepository.GetLastApiBoAsync(predicateExpression, orderExpression, isDesc, includePropertiesExpressions);
            if (bo == null)
            {
                return null;
            }
            return await ModelMapper.ToQueryDto(bo);
        }

        public virtual async Task<TQuery?> GetLastApiBoAsync(Expression<Func<TDomain, bool>> predicateExpression,
           Expression<Func<TDomain, object>> orderExpression,
           bool isDesc, bool isAutoSetIncludeExpression)
        {
            var expressions = LambdaCreater.GetIncludeExpressions<TDomain>();
            return await GetLastApiBoAsync(predicateExpression, orderExpression, isDesc, expressions);
        }
        #endregion



        #region 获取集合数据
        public virtual async Task<List<TQuery>> GetApiBoCollectionAsync(Expression<Func<TDomain, bool>> predicateExpression,
            Expression<Func<TDomain, object>> orderExpression,
            params Expression<Func<TDomain, object>>[] includePropertiesExpression)
        {
            if (orderExpression == null)
            {
                orderExpression = x => x.SortCode!;
            }
            var bos = await QueryRepository.GetBoCollectionAsync(0, 100, predicateExpression, true, orderExpression, includePropertiesExpression);
            return await ModelMapper.ToQueryDto(bos);
        }

        public virtual async Task<List<TQuery>> GetApiBoCollectionAsync(int start, int takeAmount, Expression<Func<TDomain, bool>> predicateExpression,
            bool PositiveOrder,
            Expression<Func<TDomain, object>> orderPropertyExpression,
            params Expression<Func<TDomain, object>>[] includePropertiesExpression)
        {
            var bos = await QueryRepository.GetBoCollectionAsync(start, takeAmount, predicateExpression, PositiveOrder, orderPropertyExpression, includePropertiesExpression);
            return await ModelMapper.ToQueryDto(bos);
        }

        public virtual async Task<TableDataHelper<TQuery>> GetApiBoCollectionAsyncBySearchData(SearchParams searchFormData,
            Expression<Func<TDomain, bool>> predicateExpression,
            params Expression<Func<TDomain, object>>[] includePropertiesExpression)
        {
            var bos = await QueryRepository.GetBoCollectionAsyncBySearchParams(searchFormData, predicateExpression, includePropertiesExpression);
            var boAms = await ModelMapper.ToQueryDto(bos!.Datas!);
            if (searchFormData.IsIndent)
            {
                boAms = SetIndent(boAms, bos!.Datas!);
            }
            return new TableDataHelper<TQuery>() { Datas = boAms, Total = bos.Total };
        }


        public virtual async Task<List<TQuery>> GetApiBoCollectionAsync(Expression<Func<TDomain, bool>> predicateExpression,
            Expression<Func<TDomain, object>> orderExpression,
            bool isAutoSetIncludeExpression)
        {
            var expressions = LambdaCreater.GetIncludeExpressions<TDomain>();
            return await GetApiBoCollectionAsync(0, 100, predicateExpression, false, orderExpression, expressions);
        }

        public virtual async Task<List<TQuery>> GetApiBoCollectionAsync(int start, int takeAmount, Expression<Func<TDomain, bool>> predicateExpression,
            bool PositiveOrder,
            Expression<Func<TDomain, object>> orderPropertyExpression,
            bool isAutoSetIncludeExpression)
        {
            var expressions = LambdaCreater.GetIncludeExpressions<TDomain>();
            return await GetApiBoCollectionAsync(start, takeAmount, predicateExpression, PositiveOrder, orderPropertyExpression, expressions);
        }

        public virtual async Task<TableDataHelper<TQuery>> GetApiBoCollectionAsyncBySearchData(SearchParams searchFormData,
            Expression<Func<TDomain, bool>> predicateExpression,
            bool isAutoSetIncludeExpression)
        {
            var expressions = LambdaCreater.GetIncludeExpressions<TDomain>();
            return await GetApiBoCollectionAsyncBySearchData(searchFormData, predicateExpression, expressions);
        }
        #endregion


        #region 数据持久化

        public virtual async Task<DataAccessResult> DeleteBoAsync(Expression<Func<TDomain, bool>> predicateExpression)
        {
            return await CommandRepository.DeleteBoAsync(predicateExpression);
        }

        public virtual async Task<DataAccessResult> LazyUpdateAsync(Expression<Func<TDomain, bool>> predicateExpression,
            List<KeyValuePair<string, object>> propAndValues)
        {
            return await CommandRepository.LazyUpdateAsync(predicateExpression, propAndValues);
        }

        public virtual async Task<DataAccessResult> SetAndSaveEntityData(IEnumerable<TCommand> apiEntitys, params Expression<Func<TDomain, object>>[] expressions)
        {
            var resultBos = await ModelMapper.ToDomainModel(apiEntitys);
            return await this.CommandRepository.UpdateAndSaveAsync(resultBos, expressions);
        }

        public virtual async Task<DataAccessResult> SetAndSaveEntityData(TCommand apiEntity, params Expression<Func<TDomain, object>>[] expressions)
        {
            var bo = await ModelMapper.ToDomainModel(apiEntity);
            return await this.CommandRepository.UpdateAndSaveAsync(bo!, expressions);
        }

        public virtual async Task<DataAccessResult> SetAndSaveEntityData(IEnumerable<TCommand> apiEntitys, bool isAutoSetIncludeExpression)
        {
            var expressions = LambdaCreater.GetIncludeExpressions<TDomain>();
            return await SetAndSaveEntityData(apiEntitys, expressions);
        }

        public virtual async Task<DataAccessResult> SetAndSaveEntityData(TCommand apiEntity, bool isAutoSetIncludeExpression)
        {
            var expressions = LambdaCreater.GetIncludeExpressions<TDomain>();
            return await this.SetAndSaveEntityData(apiEntity, expressions);
        }
        #endregion


    }
}
