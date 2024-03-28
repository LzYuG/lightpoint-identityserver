using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoModels.Base
{
    public class QueryDtoBase<T> : IQueryDtoBase<T>
    {
        public T Id { get; set; } = default!;
        public string? SortCode { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateTime { get; set; }

        public string? TenantIdentifier { get; set; }
    }
}
