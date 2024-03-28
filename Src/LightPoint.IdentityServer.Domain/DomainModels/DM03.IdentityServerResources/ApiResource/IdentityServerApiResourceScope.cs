using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.ApiResource
{
    [Table("IdentityServerApiResourceScopes")]
    public class IdentityServerApiResourceScope : DomainModelBase<long>
    {
        public string? Scope { get; set; }
        public Guid ApiResourceId { get; set; }
        public IdentityServerApiResource? ApiResource { get; set; }
    }
}
