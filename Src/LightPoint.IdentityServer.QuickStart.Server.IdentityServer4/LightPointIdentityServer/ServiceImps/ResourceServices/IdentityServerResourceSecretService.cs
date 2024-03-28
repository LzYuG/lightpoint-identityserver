using IdentityServer4;
using IdentityServer4.Models;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces;
using LightPoint.IdentityServer.Shared.Helpers;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.ResourceServices
{
    public class IdentityServerResourceSecretService : IIdentityServerResourceSecretService
    {
        public async Task<IdentityServerSecretDM> Decryption(IdentityServerSecretDM secretDM)
        {
            return await Task.Run(() =>
            {
                if (!secretDM.IsPersistence)
                {
                    if (secretDM.Type == IdentityServerResourcesConstants.SecretTypes.SharedSecret)
                    {
                        secretDM.Value = secretDM.Value.Sha256();
                    }
                    else if (secretDM.Type == IdentityServerResourcesConstants.SecretTypes.X509CertificateThumbprint)
                    {
                        secretDM.Type = IdentityServerConstants.SecretTypes.X509CertificateThumbprint;
                    }
                    else if (secretDM.Type == IdentityServerResourcesConstants.SecretTypes.X509CertificateBase64)
                    {
                        secretDM.Type = IdentityServerConstants.SecretTypes.X509CertificateBase64;
                    }
                    else if (secretDM.Type == IdentityServerResourcesConstants.SecretTypes.JsonWebKey)
                    {
                        secretDM.Type = IdentityServerConstants.SecretTypes.JsonWebKey;
                    }
                }
                return secretDM;
            });
        }

        public async Task<IdentityServerSecretDM> Encryption(IdentityServerSecretDM secretDM)
        {
            return await Task.Run(() =>
            {
                return secretDM;
            });
        }
    }
}
