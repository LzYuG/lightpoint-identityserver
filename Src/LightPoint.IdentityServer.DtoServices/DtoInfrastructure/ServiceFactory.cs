using LightPoint.IdentityServer.Domain.DomainModels;
using System.Reflection;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using Microsoft.Extensions.DependencyInjection;
using LightPoint.IdentityServer.DtoModels.Base;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure
{
    public class ServiceFactory : IServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IAppService<Tid, TDomain, TQuery, TCommand> GetAppService<Tid, TDomain, TQuery, TCommand>()
            where TDomain : class, IDomainModelBase<Tid>, new()
            where TQuery : class, IQueryDtoBase<Tid>, new()
            where TCommand : class, ICommandDtoBase<Tid>, new()
        {
            var appService = Assembly.GetExecutingAssembly().GetTypes().FirstOrDefault(x => x.BaseType == typeof(AppService<Tid, TDomain, TQuery, TCommand>));
            if (appService != null)
            {
                return (IAppService<Tid, TDomain, TQuery, TCommand>)_serviceProvider.GetService(appService)!;
            }
            else
            {
                return _serviceProvider.GetService<IAppService<Tid, TDomain, TQuery, TCommand>>()!;
            }
        }

    }
}
