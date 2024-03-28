using LightPoint.IdentityServer.DtoModels.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Operations
{
    public class IdentityServerPersistedGrantDM : DtoBase<Guid>
    {
        public string? Key { get; set; }
        public string? Type { get; set; }
        public string? SubjectId { get; set; }
        public string? SessionId { get; set; }
        public string? ClientId { get; set; }
        public string? Description { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? Expiration { get; set; }
        public DateTime? ConsumedTime { get; set; }
        public string? Data { get; set; }
    }
}
