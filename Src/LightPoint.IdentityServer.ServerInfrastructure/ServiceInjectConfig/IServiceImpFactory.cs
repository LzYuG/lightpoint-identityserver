using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LightPoint.IdentityServer.ServerInfrastructure.ServiceInjectConfig
{
    /// <summary>
    /// 使用可变更的服务实现，需要实现该接口
    /// </summary>
    public interface IServiceImpFactory : IRepositoryImpFactory
    {
        /// <summary>
        /// 获取默认的视图模型服务的接口类型
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        Type GetDtoServiceInterfaceType<Tid, TDomain, TQuery, TCommand>()
            where TDomain : class, IDomainModelBase<Tid>, new()
            where TQuery : class, IQueryDtoBase<Tid>, new()
            where TCommand : class, ICommandDtoBase<Tid>, new();

        /// <summary>
        /// 获取默认的视图模型服务的实现类型
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        Type GetDtoServiceImplementType<Tid, TDomain, TQuery, TCommand>()
            where TDomain : class, IDomainModelBase<Tid>, new()
            where TQuery : class, IQueryDtoBase<Tid>, new()
            where TCommand : class, ICommandDtoBase<Tid>, new();

        /// <summary>
        /// 获取邮件发送的实现类型
        /// </summary>
        /// <returns></returns>
        Type GetEmailServiceImplementType();
        /// <summary>
        /// 获取短信发送的实现类型
        /// </summary>
        /// <returns></returns>
        Type GetShortMessageServerServiceImplementType();
        Type GetConfidentialServiceImplementType();
        Type GetValidationCodeCreaterImplementType();
        Type GetCacheImplementType();
        Type GetApplicationMenuStoreImplementType();
        Type GetFileRepositoryImplementType();

        /// <summary>
        /// 用于注入数据库上下文的方法
        /// 注入的类型一般是用于仓储使用的数据上下文实现
        /// </summary>
        /// <returns>在项目中的DbContext实现类</returns>
        void AddDataContext(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment);
        /// <summary>
        /// Hangfire注入
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <param name="environment"></param>
        void AddHangfire(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment);
        /// <summary>
        /// .AddEntityFrameworkStores<DbContext>()
        /// </summary>
        /// <param name="services"></param>
        /// <param name="identityBuild"></param>
        void AddIdentity(IServiceCollection services);
    }
}
