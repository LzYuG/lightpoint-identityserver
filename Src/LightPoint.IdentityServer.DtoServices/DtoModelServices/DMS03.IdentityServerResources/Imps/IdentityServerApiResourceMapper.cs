using LightPoint.IdentityServer.DtoModels.DM00.Common;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces.MapperTools;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.MapperInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces;
using System.Data.Common;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Imps
{
    public class IdentityServerApiResourceMapper : ModelMapper<Guid, IdentityServerApiResource, IdentityServerApiResourceDQM, IdentityServerApiResourceDCM>
    {
		private readonly IIdentityServerResourceSecretService _identityServerResourceSecretService;

		public IdentityServerApiResourceMapper(IConfidentialService confidentialService, IIdentityServerResourceSecretService identityServerResourceSecretService) : base(confidentialService)
        {
			_identityServerResourceSecretService = identityServerResourceSecretService;
		}

        public override async Task<IdentityServerApiResource?> ToDomainModel(IdentityServerApiResourceDCM command)
        {
            var res = Mapper<IdentityServerApiResourceDCM, IdentityServerApiResource>.MapToNewObj(command, (dtoCommand, dm) =>
            {
                dm.Properties = ExtensionPropertiesHelper.MapperToExtensionProperties(dtoCommand).Select(x => new IdentityServerApiResourceProperty()
                {
                    Key = x.Key,
                    Value = x.Value,
                    TenantIdentifier = command.TenantIdentifier,
                    ApiResource = dm,
                    ApiResourceId = dm.Id
                }).ToList();


				dm.Scopes = dtoCommand.Scopes.Select(x => new IdentityServerApiResourceScope()
                {
                    Scope = x,
                    TenantIdentifier = command.TenantIdentifier,
                    CreateTime = DateTime.Now,
                    ApiResource = dm,
                    ApiResourceId = dm.Id
                }).ToList();
            });


            var secrets = new List<IdentityServerApiResourceSecret>();
            if (command.Secrets != null)
            {
                foreach (var secret in command.Secrets)
                {
                    var qm = (IdentityServerApiResourceSecretDM)(await _identityServerResourceSecretService.Decryption(secret));
                    var temp = Mapper<IdentityServerApiResourceSecretDM, IdentityServerApiResourceSecret>.MapToNewObj(qm);
                    temp!.TenantIdentifier = command.TenantIdentifier;
                    temp!.CreateTime = DateTime.Now;
                    temp!.ApiResource = res;
                    temp!.ApiResourceId = res!.Id;
                    secrets.Add(temp);
                }
            }
            res!.Secrets = secrets;

            return res;
        }

        public override async Task<IdentityServerApiResourceDQM?> ToQueryDto(IdentityServerApiResource domain)
        {
            var dtoQuery = Mapper<IdentityServerApiResource, IdentityServerApiResourceDQM>.MapToNewObj(domain);
            dtoQuery!.Scopes = domain.Scopes.Select(x => x.Scope).ToList()!;

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

			var secrets = new List<IdentityServerApiResourceSecretDM>();
			if (domain.Secrets != null)
			{
				foreach (var secret in domain.Secrets)
				{
					var qm = (IdentityServerApiResourceSecretDM)(await _identityServerResourceSecretService.Encryption(Mapper<IdentityServerApiResourceSecret, IdentityServerApiResourceSecretDM>.MapToNewObj(secret)!));
					qm.IsPersistence = true;
					secrets.Add(qm);
				}
			}

			dtoQuery!.Secrets = secrets;

			return dtoQuery;
        }
    }
}
