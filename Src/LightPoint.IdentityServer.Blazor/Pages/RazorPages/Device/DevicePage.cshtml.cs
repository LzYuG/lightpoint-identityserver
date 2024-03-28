using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Device.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.RazorPages.Device
{
    public class DevicePageModel : LightPointPageModel
    {
        private readonly ILogger<DevicePageModel> _logger;
        private readonly IIdentityServerDevicePageService _identityServerDevicePageService;
        private readonly IAntiforgery _antiforgery;

        public DevicePageModel(ILogger<DevicePageModel> logger, IIdentityServerDevicePageService identityServerDevicePageService,
            IAntiforgery antiforgery) : base(antiforgery)
        {
            _logger = logger;
            _identityServerDevicePageService = identityServerDevicePageService;
            _antiforgery = antiforgery;
        }

        public virtual void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiForgeryToken = tokens.RequestToken;
        }

        public async Task<IActionResult> OnPostAsync(DeviceAuthorizationSubmitModel model)
        {
            return await _identityServerDevicePageService.SubmitDeviceAuthorization(this, model);
        }
    }
}
