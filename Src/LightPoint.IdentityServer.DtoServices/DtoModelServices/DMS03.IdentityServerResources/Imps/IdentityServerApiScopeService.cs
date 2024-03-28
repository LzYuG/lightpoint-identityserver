using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiScope;
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
    public class IdentityServerApiScopeService : AppService<Guid, IdentityServerApiScope, IdentityServerApiScopeDQM, IdentityServerApiScopeDCM>, IIdentityServerApiScopeService
    {
        public IdentityServerApiScopeService(IQueryRepository<Guid, IdentityServerApiScope> queryRepository, ICommandRepository<Guid, IdentityServerApiScope> commandRepository, IModelMapper<Guid, IdentityServerApiScope, IdentityServerApiScopeDQM, IdentityServerApiScopeDCM> modelMapper) : base(queryRepository, commandRepository, modelMapper)
        {
        }
    }
}
