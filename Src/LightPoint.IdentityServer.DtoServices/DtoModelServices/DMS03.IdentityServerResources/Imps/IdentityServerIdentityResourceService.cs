using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.IdentityResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.IdentityResource;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Imps
{
    public class IdentityServerIdentityResourceService : AppService<Guid, IdentityServerIdentityResource, IdentityServerIdentityResourceDQM, IdentityServerIdentityResourceDCM>, IIdentityServerIdentityResourceService
    {
        public IdentityServerIdentityResourceService(IQueryRepository<Guid, IdentityServerIdentityResource> queryRepository, ICommandRepository<Guid, IdentityServerIdentityResource> commandRepository, IModelMapper<Guid, IdentityServerIdentityResource, IdentityServerIdentityResourceDQM, IdentityServerIdentityResourceDCM> modelMapper) : base(queryRepository, commandRepository, modelMapper)
        {
        }
    }
}
