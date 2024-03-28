using LightPoint.IdentityServer.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM00.Common
{
    public abstract class ExtensionProperty : DomainModelBase<long>
    {
        public string? Key { get; set; }
        public string? Value { get; set; }
    }
}
