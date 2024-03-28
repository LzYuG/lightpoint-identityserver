using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Consent.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.Device;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LightPoint.IdentityServer.Blazor.Pages.RazorPages.Consent
{
    public class ConsentPageModel : LightPointPageModel
    {
        private readonly ILogger<DevicePageModel> _logger;
        private readonly IIdentityServerConsentPageService _identityServerConsentPageService;
        private readonly IAntiforgery _antiforgery;

        public ConsentPageModel(ILogger<DevicePageModel> logger, IIdentityServerConsentPageService identityServerConsentPageService,
            IAntiforgery antiforgery) : base(antiforgery)
        {
            _logger = logger;
            _identityServerConsentPageService = identityServerConsentPageService;
            _antiforgery = antiforgery;
        }

        public ConsentModel? ConsentModel { get; set; }

        public async Task OnGetAsync(string returnUrl)
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiForgeryToken = tokens.RequestToken;
            ConsentModel = await _identityServerConsentPageService!.BuildConsentModel(returnUrl, null);
        }

        public async Task<IActionResult> OnPostAsync(ConsentSubmitModel model)
        {
            model.ScopesConsented = model.ScopesConsented?.FirstOrDefault()?.Split(",");
            return await _identityServerConsentPageService.SubmitConsent(this, model);
        }
    }
}
