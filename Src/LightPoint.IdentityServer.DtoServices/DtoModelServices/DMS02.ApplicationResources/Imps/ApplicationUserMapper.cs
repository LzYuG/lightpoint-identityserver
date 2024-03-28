using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources.ExtensionProperties;
using LightPoint.IdentityServer.DtoModels.DM00.Common;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Imps
{
    public class ApplicationUserMapper : ModelMapper<Guid, ApplicationUser, ApplicationUserDQM, ApplicationUserDCM>
    {
        private readonly IConfidentialService _confidentialService;
        private readonly IQueryRepository<Guid, ApplicationUser> _queryRepository;

        public ApplicationUserMapper(IConfidentialService confidentialService, IQueryRepository<Guid, ApplicationUser> queryRepository) : base(confidentialService)
        {
            _confidentialService = confidentialService;
            _queryRepository = queryRepository;
        }

        public override async Task<ApplicationUser?> ToDomainModel(ApplicationUserDCM command)
        {
            var inDbData = await _queryRepository.GetBoAsync(x => x.Id == command.Id);
            if(inDbData != null)
            {
                // 密码无法使用常规手段修改
                command.PasswordHash = inDbData.PasswordHash;
            }
            var result = Mapper<ApplicationUserDCM, ApplicationUser>.MapToNewObj(command, (userDCM, user) =>
            {
                user.ApplicationUserMultiFactorAuthentications = userDCM.ApplicationUserMultiFactorAuthentications?.Select(x => Mapper<ApplicationUserMultiFactorAuthenticationDM, ApplicationUserMultiFactorAuthentication>.MapToNewObj(x, (dm, domain) =>
                {
                    domain.ApplicationUserId = user.Id;
                    domain.ApplicationUser = user;
                })).ToList()!;
                user.ApplicationUserExternalIdentityProviderCredentials = userDCM.ApplicationUserExternalIdentityProviderCredentials?.Select(x => Mapper<ApplicationUserExternalIdentityProviderCredentialDM, ApplicationUserExternalIdentityProviderCredential>.MapToNewObj(x)).ToList()!;
                user.Properties = ExtensionPropertiesHelper.MapperToExtensionProperties(userDCM).Select(x => new ApplicationUserProperty()
                {
                    Key = x.Key,
                    Value = x.Value,
                    ApplicationUser = user,
                    ApplicationUserId = user.Id
                }).ToList();
            });
            if(result!.ApplicationUserMultiFactorAuthentications != null)
            {
                for (var i = 0; i < result.ApplicationUserMultiFactorAuthentications.Count; i++)
                {
                    result.ApplicationUserMultiFactorAuthentications[i] = await _confidentialService.EncryptModelConfidentialPropsAsync(result.ApplicationUserMultiFactorAuthentications[i]);
                }
            }
            return result;
        }

        public override Task<ApplicationUserDQM?> ToQueryDto(ApplicationUser domain)
        {
            var user = Mapper<ApplicationUser, ApplicationUserDQM>.MapToNewObj(domain, (userDM, userDQM) =>
            {
                userDQM.ApplicationUserMultiFactorAuthentications = userDM.ApplicationUserMultiFactorAuthentications?.Select(x => Mapper<ApplicationUserMultiFactorAuthentication, ApplicationUserMultiFactorAuthenticationDM>.MapToNewObj(x, (domain, dm) =>
                {
                    dm.Id = 0;
                })).ToList()!;
                userDQM.ApplicationUserExternalIdentityProviderCredentials = userDM.ApplicationUserExternalIdentityProviderCredentials?.Select(x => Mapper<ApplicationUserExternalIdentityProviderCredential, ApplicationUserExternalIdentityProviderCredentialDM>.MapToNewObj(x)).ToList()!;
            });

            user = ExtensionPropertiesHelper.ExtensionPropertiesToData(domain.Properties?.Select(x => new ExtensionPropertyDM()
            {
                Key = x.Key,
                Value = x.Value,
                CreateTime = x.CreateTime,
                TenantIdentifier = x.TenantIdentifier,
                IsDeleted = x.IsDeleted,
                Id = x.Id,
                SortCode = x.SortCode
            }), user);

            return Task.FromResult(user);
        }
    }
}
