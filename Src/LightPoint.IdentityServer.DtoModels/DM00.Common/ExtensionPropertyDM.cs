
using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.DM00.Common
{
    public class ExtensionPropertyDM : DtoBase<long>
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
