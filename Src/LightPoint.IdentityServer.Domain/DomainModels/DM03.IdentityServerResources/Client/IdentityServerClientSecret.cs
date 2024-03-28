using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client
{
    [Table("IdentityServerClientSecrets")]
    public class IdentityServerClientSecret : IdentityServerSecret
    {
        public Guid ClientId { get; set; }
        public IdentityServerClient? Client { get; set; }
    }
}