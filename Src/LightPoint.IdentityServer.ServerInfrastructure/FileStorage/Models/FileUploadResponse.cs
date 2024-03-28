using LightPoint.IdentityServer.DtoModels.DM01.SystemResource;

namespace LightPoint.IdentityServer.ServerInfrastructure.FileStorage.Models
{
    public class FileUploadResponse
    {
        public bool IsSuccess { get; set; }

        public string? ResultMsg { get; set; }

        public SystemCommonFileDM? File { get; set; }
    }
}
