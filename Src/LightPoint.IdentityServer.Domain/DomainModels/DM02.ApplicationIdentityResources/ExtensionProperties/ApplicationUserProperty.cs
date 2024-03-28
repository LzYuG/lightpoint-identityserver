using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources.ExtensionProperties
{
    [Table("ApplicationUserProperties")]
    public class ApplicationUserProperty : ExtensionProperty
    {
        public Guid ApplicationUserId { get; set; }
        public ApplicationUser? ApplicationUser { get; set; }
    }
}
