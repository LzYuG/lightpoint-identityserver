using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources.ExtensionProperties
{
    [Table("ApplicationRoleProperties")]
    public class ApplicationRoleProperty : ExtensionProperty
    {
        public Guid ApplicationRoleId { get; set; }
        public ApplicationRole? ApplicationRole { get; set; }
    }
}
