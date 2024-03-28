using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Consent.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Device.Models;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces
{
    public interface IIdentityServerConsentPageService
    {
        Task<ConsentModel?> BuildConsentModel(string returnUrl, ConsentSubmitModel? model = null);

        Task<IActionResult> SubmitConsent(LightPointPageModel page, ConsentSubmitModel deviceAuthorizationSubmitModel);
    }
}
