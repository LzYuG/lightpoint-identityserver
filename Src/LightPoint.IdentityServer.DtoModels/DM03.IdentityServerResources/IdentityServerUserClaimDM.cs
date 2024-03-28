using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources
{
    public abstract class IdentityServerUserClaimDM : DtoBase<int>
    {
        public string? Type { get; set; }
    }
}
