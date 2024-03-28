using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces
{
    public interface IApplicationUserService : IAppService<Guid, ApplicationUser, ApplicationUserDQM, ApplicationUserDCM>
    {
        Task<DataAccessResult> ValidateCredentials(string userName, string password);

        Task<DataAccessResult> RegisterUser(SystemAccountConfigDQM? systemAccountConfig, ApplicationUserDCM applicationUser, string password, IEnumerable<ApplicationUserClaimDM> claims, IEnumerable<ApplicationRoleDCM> roles);

        Task<DataAccessResult> ResetPassword(SystemAccountConfigDQM? systemAccountConfig, string userName, string newPassword);

        Task<DataAccessResult> AddToRoles(ApplicationUserDCM applicationUser, IEnumerable<ApplicationRoleDCM> roles);

        Task<DataAccessResult> RemoveFromRoles(ApplicationUserDCM applicationUser, IEnumerable<ApplicationRoleDCM> roles);

        Task<List<string>> InRoles(Guid userId, string tenantIdentifier);

        Task<List<ApplicationUserClaimDM>> GetClaims(Guid userId, string tenantIdentifier);

        Task<DataAccessResult> SetAndSaveEntityData(SystemAccountConfigDQM? systemAccountConfig, ApplicationUserDCM apiEntity);
    }
}
