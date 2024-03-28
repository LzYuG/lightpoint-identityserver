using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.IdentityResource
{
    [Table("IdentityServerIdentityResourceClaims")]
    public class IdentityServerIdentityResourceClaim : IdentityServerUserClaim
    {
        public Guid IdentityResourceId { get; set; }
        public IdentityServerIdentityResource? IdentityResource { get; set; }
    }
}
