using LightPoint.IdentityServer.Shared;

namespace LightPoint.IdentityServer.DtoModels.Base
{
    public interface ICommandDto<T> : ICommandDtoBase<T>
    {
        string? Name { get; set; }
        string? Description { get; set; }
    }
}
