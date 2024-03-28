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
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential;
using System.Data.Common;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Imps
{
    public class IdentityServerClientMapper : ModelMapper<Guid, IdentityServerClient, IdentityServerClientDQM, IdentityServerClientDCM>
    {
        private readonly IIdentityServerResourceSecretService _identityServerResourceSecretService;

        public IdentityServerClientMapper(IIdentityServerResourceSecretService identityServerResourceSecretService, IConfidentialService confidentialService) : base(confidentialService)
        {
            _identityServerResourceSecretService = identityServerResourceSecretService;
        }

        public override async Task<IdentityServerClient?> ToDomainModel(IdentityServerClientDCM command)
        {
            var res = Mapper<IdentityServerClientDCM, IdentityServerClient>.MapToNewObj(command, (dtoCommand, dm) =>
            {
                dm.Properties = ExtensionPropertiesHelper.MapperToExtensionProperties(dtoCommand).Select(x => new IdentityServerClientProperty()
                {
                    Key = x.Key,
                    Value = x.Value,
                    TenantIdentifier = command.TenantIdentifier,
                    Client = dm,
                    ClientId = dm.Id
                }).ToList();

                dm.AllowedCorsOrigins = dtoCommand.AllowedCorsOrigins?.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => new IdentityServerClientCorsOrigin()
                {
                    Origin = x,
                    TenantIdentifier = command.TenantIdentifier,
                    CreateTime = DateTime.Now,
                    Client = dm,
                    ClientId = dm.Id
                }).ToList();

                dm.AllowedGrantTypes = dtoCommand.AllowedGrantTypes?.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => new IdentityServerClientGrantType()
                {
                    GrantType = x,
                    TenantIdentifier = command.TenantIdentifier,
                    CreateTime = DateTime.Now,
                    Client = dm,
                    ClientId = dm.Id
                }).ToList();

                dm.IdentityProviderRestrictions = dtoCommand.IdentityProviderRestrictions?.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => new IdentityServerClientIdPRestriction()
                {
                    Provider = x,
                    TenantIdentifier = command.TenantIdentifier,
                    CreateTime = DateTime.Now,
                    Client = dm,
                    ClientId = dm.Id
                }).ToList();

                dm.PostLogoutRedirectUris = dtoCommand.PostLogoutRedirectUris?.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => new IdentityServerClientPostLogoutRedirectUri()
                {
                    RedirectUri = x,
                    TenantIdentifier = command.TenantIdentifier,
                    CreateTime = DateTime.Now,
                    Client = dm,
                    ClientId = dm.Id
                }).ToList();

                dm.RedirectUris = dtoCommand.RedirectUris?.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => new IdentityServerClientRedirectUri()
                {
                    RedirectUri = x,
                    TenantIdentifier = command.TenantIdentifier,
                    CreateTime = DateTime.Now,
                    Client = dm,
                    ClientId = dm.Id
                }).ToList();

                dm.AllowedScopes = dtoCommand.AllowedScopes?.Where(x => !string.IsNullOrWhiteSpace(x))?.Select(x => new IdentityServerClientScope()
                {
                    Scope = x,
                    TenantIdentifier = command.TenantIdentifier,
                    CreateTime = DateTime.Now,
                    Client = dm,
                    ClientId = dm.Id
                }).ToList();

                

                dm.Claims = dtoCommand.Claims?.Select(x => new IdentityServerClientClaim()
                {
                    Value = x.Value,
                    Type = x.Type,
                    TenantIdentifier = command.TenantIdentifier,
                    CreateTime = DateTime.Now,
                    Client = dm,
                    ClientId = dm.Id
                }).ToList();
            });

            var secrets = new List<IdentityServerClientSecret>();
            if (command.ClientSecrets != null)
            {
                foreach (var secret in command.ClientSecrets)
                {
                    var qm = (IdentityServerClientSecretDM)(await _identityServerResourceSecretService.Decryption(secret));
                    var temp = Mapper<IdentityServerClientSecretDM, IdentityServerClientSecret>.MapToNewObj(qm);
                    temp!.TenantIdentifier = command.TenantIdentifier;
                    temp!.CreateTime = DateTime.Now;
                    temp!.Client = res;
                    temp!.ClientId = res!.Id;
                    secrets.Add(temp);
                }
            }
            res!.ClientSecrets = secrets;

            return res;
        }

        public override async Task<IdentityServerClientDQM?> ToQueryDto(IdentityServerClient domain)
        {
            var dtoQuery = Mapper<IdentityServerClient, IdentityServerClientDQM>.MapToNewObj(domain, (dm, dtoQuery) =>
            {
                dtoQuery!.AllowedCorsOrigins = dm.AllowedCorsOrigins?.Select(x => x.Origin).ToList()!;
                dtoQuery!.AllowedGrantTypes = dm.AllowedGrantTypes?.Select(x => x.GrantType).ToList()!;
                dtoQuery!.IdentityProviderRestrictions = dm.IdentityProviderRestrictions?.Select(x => x.Provider).ToList()!;
                dtoQuery!.PostLogoutRedirectUris = dm.PostLogoutRedirectUris?.Select(x => x.RedirectUri).ToList()!;
                dtoQuery!.RedirectUris = dm.RedirectUris?.Select(x => x.RedirectUri).ToList()!;
                dtoQuery!.AllowedScopes = dm.AllowedScopes?.Select(x => x.Scope).ToList()!;

                
                dtoQuery!.Claims = dm.Claims?.Select(x => new IdentityServerClientClaimDM()
                {
                    Type = x.Type,
                    Value = x.Value,
                    ClientId = dm.Id,
                    CreateTime = x.CreateTime,
                    TenantIdentifier = dm.TenantIdentifier
                }).ToList()!;
            });
            var secrets = new List<IdentityServerClientSecretDM>();
            if (domain.ClientSecrets != null)
            {
                foreach (var secret in domain.ClientSecrets)
                {
                    var qm = (IdentityServerClientSecretDM)(await _identityServerResourceSecretService.Encryption(Mapper<IdentityServerClientSecret, IdentityServerClientSecretDM>.MapToNewObj(secret)!));
                    qm.IsPersistence = true;
                    secrets.Add(qm);
                }
            }

            dtoQuery!.ClientSecrets = secrets;


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

            return dtoQuery;
        }
    }
}
