using LightPoint.IdentityServer.Domain.DomainModels;
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure
{
    public interface IServiceFactory
    {
        IAppService<Tid, TDomain, TQuery, TCommand> GetAppService<Tid, TDomain, TQuery, TCommand>()
            where TDomain : class, IDomainModelBase<Tid>, new()
            where TQuery : class, IQueryDtoBase<Tid>, new()
            where TCommand : class, ICommandDtoBase<Tid>, new();
    }
}
