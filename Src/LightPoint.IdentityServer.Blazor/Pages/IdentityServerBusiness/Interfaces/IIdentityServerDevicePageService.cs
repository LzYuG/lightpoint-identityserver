using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Device.Models;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces
{
    public interface IIdentityServerDevicePageService
    {
        Task<IdentityServerDeviceFlowCodeDM?> GetDeviceFlowCode(string userCode, string tenantIdentifiler);

        Task<DeviceAuthorizationModel?> BuildDeviceAuthorizationModel(string userCode, DeviceAuthorizationSubmitModel? model = null);

        Task<IActionResult> SubmitDeviceAuthorization(LightPointPageModel page, DeviceAuthorizationSubmitModel deviceAuthorizationSubmitModel);
    }
}
