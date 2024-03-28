using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.IdentityResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.IdentityResource;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces
{
    public interface IIdentityServerIdentityResourceService : IAppService<Guid, IdentityServerIdentityResource, IdentityServerIdentityResourceDQM, IdentityServerIdentityResourceDCM>
    {
    }
}
