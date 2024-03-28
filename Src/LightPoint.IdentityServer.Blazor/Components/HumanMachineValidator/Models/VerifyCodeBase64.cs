using Microsoft.AspNetCore.Mvc;

namespace LightPoint.IdentityServer.Blazor.Components.HumanMachineValidator.Models
{
    public class VerifyCodeBase64
    {
        public string? ImageBase64Str { get; set; }

        public string? Result { get; set; }
    }
}
