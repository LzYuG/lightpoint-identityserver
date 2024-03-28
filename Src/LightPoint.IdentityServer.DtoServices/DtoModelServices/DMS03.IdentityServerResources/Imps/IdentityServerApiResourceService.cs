using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Imps
{
    public class IdentityServerApiResourceService : AppService<Guid, IdentityServerApiResource, IdentityServerApiResourceDQM, IdentityServerApiResourceDCM>, IIdentityServerApiResourceService
    {
        public IdentityServerApiResourceService(IQueryRepository<Guid, IdentityServerApiResource> queryRepository, ICommandRepository<Guid, IdentityServerApiResource> commandRepository, IModelMapper<Guid, IdentityServerApiResource, IdentityServerApiResourceDQM, IdentityServerApiResourceDCM> modelMapper) : base(queryRepository, commandRepository, modelMapper)
        {
        }
    }
}
