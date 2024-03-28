using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;

namespace LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiScope
{
    public class IdentityServerApiScopeDCM : CommandDto<Guid>
    {
        public bool Enabled { get; set; } = true;
        public string? DisplayName { get; set; }
        public bool Required { get; set; }
        public bool Emphasize { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<string>? UserClaims { get; set; }
    }
}
