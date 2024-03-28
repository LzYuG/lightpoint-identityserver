using LightPoint.IdentityServer.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client
{
    [Table("IdentityServerClientScopes")]
    public class IdentityServerClientScope : DomainModelBase<long>
    {
        public string? Scope { get; set; }

        public Guid ClientId { get; set; }
        public IdentityServerClient? Client { get; set; }
    }
}
