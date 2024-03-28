using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Imps
{
    public class ApplicationRoleService : AppService<Guid, ApplicationRole, ApplicationRoleDQM, ApplicationRoleDCM>, IApplicationRoleService
    {
        public ApplicationRoleService(IQueryRepository<Guid, ApplicationRole> queryRepository, ICommandRepository<Guid, ApplicationRole> commandRepository, IModelMapper<Guid, ApplicationRole, ApplicationRoleDQM, ApplicationRoleDCM> modelMapper) : base(queryRepository, commandRepository, modelMapper)
        {
        }
    }
}
