using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS01.SystemResources.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS01.SystemResources.Imps
{
    public class SystemTenantService : AppService<Guid, SystemTenant, SystemTenantDQM, SystemTenantDCM>, ISystemTenantService
    {
        public SystemTenantService(IQueryRepository<Guid, SystemTenant> queryRepository, ICommandRepository<Guid, SystemTenant> commandRepository, IModelMapper<Guid, SystemTenant, SystemTenantDQM, SystemTenantDCM> modelMapper) : base(queryRepository, commandRepository, modelMapper)
        {
        }


        public async Task<bool> IsRootTenant(string tenantIdentifier)
        {
            return await QueryRepository.HasBoAsync(x => x.TenantIdentifier == tenantIdentifier);
        }
    }
}
