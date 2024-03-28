using LightPoint.IdentityServer.DtoModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client
{
    public class IdentityServerClientClaimDM : DtoBase<long>
    {
        public string? Type { get; set; }
        public string? Value { get; set; }
        public string ValueType { get; set; } = ClaimValueTypes.String;

        public Guid ClientId { get; set; }
    }
}
