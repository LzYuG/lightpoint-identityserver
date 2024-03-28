using LightPoint.IdentityServer.Shared;

namespace LightPoint.IdentityServer.DtoModels.Base
{
    public interface IQueryDto<T> : IQueryDtoBase<T>
    {
        string? Name { get; set; }
        string? Description { get; set; }
    }
}
