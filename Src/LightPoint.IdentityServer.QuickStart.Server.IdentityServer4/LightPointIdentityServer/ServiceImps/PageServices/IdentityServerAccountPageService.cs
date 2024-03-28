using IdentityServer4.Events;
using IdentityServer4;
using IdentityServer4.Services;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Mvc.RazorPages;
using IdentityModel;
using LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.IdentityServer4Imps.Models;
using LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.IdentityServer4Imps;
using IdentityServer4.Stores;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.PageServices
{
    public class IdentityServerAccountPageService : IIdentityServerAccountPageService
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IClientStore _clientStore;
        private readonly TenantInfo _tenantInfo;

        public IdentityServerAccountPageService(IIdentityServerInteractionService interaction,
            IApplicationUserService applicationUserService,
            IAuthenticationSchemeProvider schemeProvider, IClientStore clientStore,
            TenantInfo tenantInfo)
        {
            _interaction = interaction;
            _applicationUserService = applicationUserService;
            _schemeProvider = schemeProvider;
            _clientStore = clientStore;
            _tenantInfo = tenantInfo;
        }

        public async Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model)
        {
            var vm = await BuildLoginViewModelAsync(model.ReturnUrl!);
            vm.Username = model.Username;
            vm.RememberLogin = model.RememberLogin;
            return vm;
        }

        public async Task<IActionResult> SignIn(LightPointPageModel page, ApplicationUserDQM validatedUser, LoginInputModel loginInputModel)
        {
            AuthenticationProperties? props = null;
            var context = await _interaction.GetAuthorizationContextAsync(loginInputModel.ReturnUrl);
            if (loginInputModel.RememberLogin)
            {
                props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                };
            };

            var isuser = new IdentityServerUser(validatedUser.Id.ToString())
            {
                DisplayName = validatedUser.UserName
            };

            var roles = await _applicationUserService.InRoles(validatedUser.Id, _tenantInfo.TenantIdentifier!);
            foreach(var role in roles)
            {
                isuser.AdditionalClaims.Add(new System.Security.Claims.Claim("role", role));
            }

            await page.HttpContext.SignInAsync(isuser, props);

            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    return page.LPRedirect("/Redirect?returnUrl=" + loginInputModel.ReturnUrl);
                }
                return page.LPRedirect(loginInputModel.ReturnUrl!);
            }
            if (page.Url.IsLocalUrl(loginInputModel.ReturnUrl))
            {
                return page.LPRedirect(loginInputModel.ReturnUrl!);
            }
            else if (string.IsNullOrEmpty(loginInputModel.ReturnUrl))
            {
                return page.LPRedirect("~/");
            }
            else
            {
                throw new Exception("invalid return URL");
            }
        }

        public async Task<IActionResult> SignOut(LightPointPageModel page, LogoutInputModel model)
        {
            // build a model so the logged out page knows what to display
            var vm = await BuildLoggedOutViewModelAsync(page.HttpContext, model.LogoutId!);

            if (page.HttpContext.User?.Identity!.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await page.HttpContext.SignOutAsync();
            }

            // check if we need to trigger sign-out at an upstream identity provider
            if (vm.TriggerExternalSignout)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                string url = page.Url.Action("Logout", new { logoutId = vm.LogoutId })!;

                // this triggers a redirect to the external provider for sign-out
                return page.SignOut(new AuthenticationProperties { RedirectUri = url }, vm.ExternalAuthenticationScheme);
            }
            return page.LPRedirect("/Account/LoggedOut", vm);
        }


        #region Helpers
        private async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
            {
                var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

                // this is meant to short circuit the UI and only trigger the one external IdP
                var vm = new LoginViewModel
                {
                    EnableLocalLogin = local,
                    ReturnUrl = returnUrl,
                    Username = context?.LoginHint,
                };

                if (!local)
                {
                    vm.ExternalProviders = new[] { new ExternalProvider { AuthenticationScheme = context?.IdP } };
                }

                return vm;
            }

            var schemes = await _schemeProvider.GetAllSchemesAsync();

            var providers = schemes
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName ?? x.Name,
                    AuthenticationScheme = x.Name
                }).ToList();

            var allowLocal = true;
            if (context?.Client.ClientId != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                    }
                }
            }

            return new LoginViewModel
            {
                AllowRememberLogin = true,
                EnableLocalLogin = allowLocal && true,
                ReturnUrl = returnUrl,
                Username = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            };
        }

        private async Task<LoggedOutViewModel> BuildLoggedOutViewModelAsync(HttpContext httpContext, string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(logoutId);

            var vm = new LoggedOutViewModel
            {
                AutomaticRedirectAfterSignOut = true,
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri!,
                ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId! : logout?.ClientName!,
                SignOutIframeUrl = logout?.SignOutIFrameUrl!,
                LogoutId = logoutId
            };

            if (httpContext.User?.Identity!.IsAuthenticated == true)
            {
                var idp = httpContext.User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
                if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
                {
                    var providerSupportsSignout = await httpContext.GetSchemeSupportsSignOutAsync(idp);
                    if (providerSupportsSignout)
                    {
                        if (vm.LogoutId == null)
                        {
                            // if there's no current logout context, we need to create one
                            // this captures necessary info from the current logged in user
                            // before we signout and redirect away to the external IdP for signout
                            vm.LogoutId = await _interaction.CreateLogoutContextAsync();
                        }

                        vm.ExternalAuthenticationScheme = idp;
                    }
                }
            }

            return vm;
        }
        #endregion
    }
}
