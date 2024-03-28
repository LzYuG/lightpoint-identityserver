using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.IdentityResource
{
    [Table("IdentityServerIdentityResourceProperties")]
    public class IdentityServerIdentityResourceProperty : ExtensionProperty
    {
        public Guid IdentityResourceId { get; set; }
        public IdentityServerIdentityResource? IdentityResource { get; set; }
    }
}
