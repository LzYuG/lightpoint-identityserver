using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.IdentityResource;
using LightPoint.IdentityServer.DtoModels.DM00.Common;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.IdentityResource;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiScope;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Imps
{
    public class IdentityServerApiScopeMapper : ModelMapper<Guid, IdentityServerApiScope, IdentityServerApiScopeDQM, IdentityServerApiScopeDCM>
    {
        public IdentityServerApiScopeMapper(IConfidentialService confidentialService) : base(confidentialService)
        {
        }

        public override Task<IdentityServerApiScope?> ToDomainModel(IdentityServerApiScopeDCM command)
        {
            return Task.FromResult(Mapper<IdentityServerApiScopeDCM, IdentityServerApiScope>.MapToNewObj(command, (dtoCommand, dm) =>
            {
                dm.Properties = ExtensionPropertiesHelper.MapperToExtensionProperties(dtoCommand).Select(x => new IdentityServerApiScopeProperty()
                {
                    Key = x.Key,
                    Value = x.Value,
                    TenantIdentifier = command.TenantIdentifier,
                    Scope = dm,
                    ScopeId = dm.Id
                }).ToList();

                dm.UserClaims = dtoCommand.UserClaims?.Select(x => new IdentityServerApiScopeClaim()
                {
                    Type = x,
                    TenantIdentifier = command.TenantIdentifier,
                    CreateTime = DateTime.Now,
                    Scope = dm,
                    ScopeId = dm.Id
                }).ToList();
            }));
        }

        public override Task<IdentityServerApiScopeDQM?> ToQueryDto(IdentityServerApiScope domain)
        {
            var dtoQuery = Mapper<IdentityServerApiScope, IdentityServerApiScopeDQM>.MapToNewObj(domain, (dm, dtoQuery) =>
            {
                dtoQuery!.UserClaims = dm.UserClaims?.Select(x => x.Type).ToList()!;
            });

            dtoQuery = ExtensionPropertiesHelper.ExtensionPropertiesToData(domain.Properties?.Select(x => new ExtensionPropertyDM()
            {
                Key = x.Key,
                Value = x.Value,
                CreateTime = x.CreateTime,
                TenantIdentifier = x.TenantIdentifier,
                IsDeleted = x.IsDeleted,
                Id = x.Id,
                SortCode = x.SortCode
            }), dtoQuery);

            return Task.FromResult(dtoQuery);
        }
    }
}
