using LightPoint.IdentityServer.DtoModels.Tools.Mappers;

namespace LightPoint.IdentityServer.DtoModels.Base
{
    public class QueryDto<T> : QueryDtoBase<T>, IQueryDto<T>
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
