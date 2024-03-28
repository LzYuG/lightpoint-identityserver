using LightPoint.IdentityServer.DtoModels.Base;

namespace LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.ApiResource
{
    public class IdentityServerApiResourceDQM : QueryDto<Guid>
    {
        public bool Enabled { get; set; } = true;
        public string? DisplayName { get; set; }
        public string? AllowedAccessTokenSigningAlgorithms { get; set; }
        public bool ShowInDiscoveryDocument { get; set; } = true;
        public List<IdentityServerApiResourceSecretDM> Secrets { get; set; } = new List<IdentityServerApiResourceSecretDM>();
        public List<string> Scopes { get; set; } = new List<string>();
        public List<string> UserClaims { get; set; } = new List<string>();
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime? Updated { get; set; }
        public DateTime? LastAccessed { get; set; }
        public bool NonEditable { get; set; }
    }
}
