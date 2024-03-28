using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiScope
{
    [Table("IdentityServerApiScopeClaims")]
    public class IdentityServerApiScopeClaim : IdentityServerUserClaim
    {
        public Guid ScopeId { get; set; }
        public IdentityServerApiScope? Scope { get; set; }
    }
}
