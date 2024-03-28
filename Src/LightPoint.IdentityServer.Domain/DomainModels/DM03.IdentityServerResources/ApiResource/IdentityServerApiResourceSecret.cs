using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource
{
    [Table("IdentityServerApiResourceSecrets")]
    public class IdentityServerApiResourceSecret : IdentityServerSecret
    {
        public Guid ApiResourceId { get; set; }
        public IdentityServerApiResource? ApiResource { get; set; }
    }
}
