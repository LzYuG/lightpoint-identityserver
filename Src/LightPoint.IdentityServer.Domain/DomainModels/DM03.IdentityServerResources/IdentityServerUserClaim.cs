using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources
{
    public abstract class IdentityServerUserClaim : DomainModelBase<long>
    {
        public string? Type { get; set; }
    }
}
