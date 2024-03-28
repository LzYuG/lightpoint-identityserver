using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources.ExtensionProperties;
using LightPoint.IdentityServer.DtoModels.DM00.Common;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Imps
{
    public class ApplicationRoleMapper : ModelMapper<Guid, ApplicationRole, ApplicationRoleDQM, ApplicationRoleDCM>
    {
        public ApplicationRoleMapper(IConfidentialService confidentialService) : base(confidentialService)
        {
        }

        public override Task<ApplicationRole?> ToDomainModel(ApplicationRoleDCM command)
        {
            return Task.FromResult(Mapper<ApplicationRoleDCM, ApplicationRole>.MapToNewObj(command, (roleDCM, role) =>
            {
                role.Properties = ExtensionPropertiesHelper.MapperToExtensionProperties(roleDCM).Select(x => new ApplicationRoleProperty()
                {
                    Key = x.Key,
                    Value = x.Value,
                    ApplicationRole = role,
                    ApplicationRoleId = role.Id
                }).ToList();
            }));
        }

        public override Task<ApplicationRoleDQM?> ToQueryDto(ApplicationRole domain)
        {
            var role = Mapper<ApplicationRole, ApplicationRoleDQM>.MapToNewObj(domain);

            role = ExtensionPropertiesHelper.ExtensionPropertiesToData(domain.Properties?.Select(x => new ExtensionPropertyDM()
            {
                Key = x.Key,
                Value = x.Value,
                CreateTime = x.CreateTime,
                TenantIdentifier = x.TenantIdentifier,
                IsDeleted = x.IsDeleted,
                Id = x.Id,
                SortCode = x.SortCode
            }), role);

            return Task.FromResult(role);
        }
    }
}
