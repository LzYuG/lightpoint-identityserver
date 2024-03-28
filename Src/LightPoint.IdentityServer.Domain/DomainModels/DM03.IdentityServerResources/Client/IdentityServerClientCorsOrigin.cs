using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Client
{
    [Table("IdentityServerClientCorsOrigins")]
    public class IdentityServerClientCorsOrigin : DomainModelBase<long>
    {
        public string? Origin { get; set; }

        public Guid ClientId { get; set; }
        public IdentityServerClient? Client { get; set; }
    }
}
