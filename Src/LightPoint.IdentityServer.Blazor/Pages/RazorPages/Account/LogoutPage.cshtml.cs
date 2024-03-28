using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LightPoint.IdentityServer.Shared.Helpers.OidcConstants;

namespace LightPoint.IdentityServer.Blazor.Pages.RazorPages.Account
{
    public class LogoutPageModel : LightPointPageModel
    {
        private readonly ILogger<LoginPageModel> _logger;
        private readonly IIdentityServerAccountPageService _identityServerAccountPageService;
        private readonly IAntiforgery _antiforgery;

        public LogoutPageModel(ILogger<LoginPageModel> logger, IIdentityServerAccountPageService identityServerAccountPageService,
            IAntiforgery antiforgery) : base(antiforgery)
        {
            _logger = logger;
            _identityServerAccountPageService = identityServerAccountPageService;
            _antiforgery = antiforgery;
        }

        public virtual void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiForgeryToken = tokens.RequestToken;
        }

        public async Task<IActionResult> OnPostAsync(LogoutInputModel model)
        {
            return await _identityServerAccountPageService.SignOut(this, model);
        }
    }
}
