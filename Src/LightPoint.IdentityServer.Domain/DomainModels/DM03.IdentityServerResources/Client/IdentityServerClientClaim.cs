using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client
{
    [Table("IdentityServerClientClaims")]
    public class IdentityServerClientClaim : DomainModelBase<long>
    {
        public string? Type { get; set; }
        public string? Value { get; set; }

        public Guid ClientId { get; set; }
        public IdentityServerClient? Client { get; set; }
    }
}
