using Hangfire;
using Hangfire.MemoryStorage;
using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.Infrastructure.RepositoryImplementationByEFCore;
using LightPoint.IdentityServer.ServerInfrastructure.Cache;
using LightPoint.IdentityServer.ServerInfrastructure.Email;
using LightPoint.IdentityServer.ServerInfrastructure.FileStorage;
using LightPoint.IdentityServer.ServerInfrastructure.SMS;
using LightPoint.IdentityServer.ServerInfrastructure.ValidationCode;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LightPoint.IdentityServer.ServerInfrastructure.ServiceInjectConfig
{
    /// <summary>
    /// 服务类型的基础实现，服务应继承该实现类并确认注入的实现类型
    /// </summary>
    public abstract class ServiceImpFactoryBase : IServiceImpFactory
    {
        /// <summary>
        /// 获取默认的Query仓储的实现类型
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <returns></returns>
        public virtual Type GetQueryRepositoryImplementType<Tid, TDomain>()
            where TDomain : class, IDomainModelBase<Tid>, new()
        {
            return typeof(QueryRepositoryByEFCore<Tid, TDomain>);
        }

        /// <summary>
        /// 获取默认的Command仓储的实现类型
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <returns></returns>
        public virtual Type GetCommandRepositoryImplementType<Tid, TDomain>()
            where TDomain : class, IDomainModelBase<Tid>, new()
        {
            return typeof(CommandRepositoryByEFCore<Tid, TDomain>);
        }

        /// <summary>
        /// 获取默认的视图模型服务的接口类型
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public virtual Type GetDtoServiceInterfaceType<Tid, TDomain, TQuery, TCommand>()
            where TDomain : class, IDomainModelBase<Tid>, new()
            where TQuery : class, IQueryDtoBase<Tid>, new()
            where TCommand : class, ICommandDtoBase<Tid>, new()
        {
            return typeof(IAppService<Tid, TDomain, TQuery, TCommand>);
        }

        /// <summary>
        /// 获取默认的视图模型服务的实现类型
        /// </summary>
        /// <typeparam name="TDomain"></typeparam>
        /// <typeparam name="TDto"></typeparam>
        /// <returns></returns>
        public virtual Type GetDtoServiceImplementType<Tid, TDomain, TQuery, TCommand>()
            where TDomain : class, IDomainModelBase<Tid>, new()
            where TQuery : class, IQueryDtoBase<Tid>, new()
            where TCommand : class, ICommandDtoBase<Tid>, new()
        {
            return typeof(AppService<Tid, TDomain, TQuery, TCommand>);
        }

        /// <summary>
        /// 用于注入数据库上下文的方法
        /// 注入的类型一般是用于仓储使用的数据上下文实现
        /// </summary>
        /// <returns>在项目中的DbContext实现类</returns>
        public abstract void AddDataContext(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment);

        /// <summary>
        /// 获取缓存的实现类型
        /// </summary>
        /// <returns>在项目中的缓存实现类</returns>
        public Type GetCacheImplementType()
        {
            return typeof(LightPointCacheByMemoryCache);
        }

        public abstract void AddIdentity(IServiceCollection services);

        public Type GetEmailServiceImplementType()
        {
            return typeof(SmtpEmailService);
        }

        public Type GetShortMessageServerServiceImplementType()
        {
            return typeof(AliCloundShortMessageServerService);
        }

        public abstract Type GetConfidentialServiceImplementType();

        public virtual void AddHangfire(IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            services.AddHangfire(configuration =>
            {
                configuration.UseMemoryStorage();
            });
            // 启动后台任务
            services.AddHangfireServer();
        }

        public virtual Type GetValidationCodeCreaterImplementType()
        {
            return typeof(ValidationCodeCreater);
        }

        public virtual Type GetFileRepositoryImplementType()
        {
            return typeof(FileRepositoryImpByLocalStorage);
        }

        public abstract Type GetApplicationMenuStoreImplementType();
    }
}
