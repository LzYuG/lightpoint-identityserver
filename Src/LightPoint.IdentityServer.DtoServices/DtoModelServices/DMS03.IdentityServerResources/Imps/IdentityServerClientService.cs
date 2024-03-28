using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
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
    public class IdentityServerClientService : AppService<Guid, IdentityServerClient, IdentityServerClientDQM, IdentityServerClientDCM>, IIdentityServerClientService
    {
        public IdentityServerClientService(IQueryRepository<Guid, IdentityServerClient> queryRepository, ICommandRepository<Guid, IdentityServerClient> commandRepository, IModelMapper<Guid, IdentityServerClient, IdentityServerClientDQM, IdentityServerClientDCM> modelMapper) : base(queryRepository, commandRepository, modelMapper)
        {
        }
    }
}
