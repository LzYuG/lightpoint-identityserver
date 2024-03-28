using LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS01.SystemResources.Interfaces
{
    public interface ISystemTenantService : IAppService<Guid, SystemTenant, SystemTenantDQM, SystemTenantDCM>
    {

        Task<bool> IsRootTenant(string tenantIdentifier);
    }
}
