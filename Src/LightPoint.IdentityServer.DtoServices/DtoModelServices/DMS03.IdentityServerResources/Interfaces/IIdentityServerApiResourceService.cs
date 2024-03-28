using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces
{
    public interface IIdentityServerApiResourceService : IAppService<Guid, IdentityServerApiResource, IdentityServerApiResourceDQM, IdentityServerApiResourceDCM>
    {
    }
}
