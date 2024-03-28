using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Consent.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Device.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;
using LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.IdentityServer4Imps;
using LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.PageServices.Models;
using LightPoint.IdentityServer.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.PageServices
{
    public class IdentityServerConsentPageService : IIdentityServerConsentPageService
    {
        private readonly IIdentityServerInteractionService _interaction;

        public IdentityServerConsentPageService(IIdentityServerInteractionService interaction)
        {
            _interaction = interaction;
        }

        public async Task<ConsentModel?> BuildConsentModel(string returnUrl, ConsentSubmitModel? model = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null)
            {
                return CreateConsentViewModel(model, request);
            }
            else
            {
                return null;
            }
        }

        private ConsentModel CreateConsentViewModel(
           ConsentSubmitModel? model,
           AuthorizationRequest request)
        {
            var vm = new ConsentModel
            {
                RememberConsent = model?.RememberConsent ?? true,
                ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),
                Description = model?.Description,
                AllowRememberConsent = request.Client.AllowRememberConsent
            };

            vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) || model == null)).ToArray();

            var apiScopes = new List<ScopeViewModel>();
            foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
            {
                var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
                if (apiScope != null)
                {
                    var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) || model == null);
                    apiScopes.Add(scopeVm);
                }
            }
            if (ConsentOptionsContents.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
            {
                apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) || model == null));
            }
            vm.ApiScopes = apiScopes;

            return vm;
        }

        private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
        {
            return new ScopeViewModel
            {
                Value = identity.Name,
                DisplayName = identity.DisplayName ?? identity.Name,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
                Checked = check || identity.Required
            };
        }

        private ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            var displayName = apiScope.DisplayName ?? apiScope.Name;
            if (!String.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
            {
                displayName += ":" + parsedScopeValue.ParsedParameter;
            }

            return new ScopeViewModel
            {
                Value = parsedScopeValue.RawValue,
                DisplayName = displayName,
                Description = apiScope.Description,
                Emphasize = apiScope.Emphasize,
                Required = apiScope.Required,
                Checked = check || apiScope.Required
            };
        }

        private ScopeViewModel GetOfflineAccessScope(bool check)
        {
            return new ScopeViewModel
            {
                Value = IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = ConsentOptionsContents.OfflineAccessDisplayName,
                Description = ConsentOptionsContents.OfflineAccessDescription,
                Emphasize = true,
                Checked = check
            };
        }

        public async Task<IActionResult> SubmitConsent(LightPointPageModel page, ConsentSubmitModel deviceAuthorizationSubmitModel)
        {
            var result = await ProcessConsent(deviceAuthorizationSubmitModel);

            if (result.IsRedirect)
            {
                var context = await _interaction.GetAuthorizationContextAsync(deviceAuthorizationSubmitModel.ReturnUrl);
                if (context?.IsNativeClient() == true)
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return page.LPRedirect("Redirect?redirectUri=" + result.RedirectUri);
                }

                return page.LPRedirect(result.RedirectUri!);
            }
            return page.LPRedirect("/Consent/Error");
        }

        private async Task<ProcessConsentResult> ProcessConsent(ConsentSubmitModel model)
        {
            var result = new ProcessConsentResult();

            // validate return url is still valid
            var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null) return result;

            ConsentResponse? grantedConsent = null;

            // user clicked 'no' - send back the standard 'access_denied' response
            if (!model.IsAllow)
            {
                grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };
            }
            // user clicked 'yes' - validate the data
            else if (model.IsAllow)
            {
                // if the user consented to some scope, build the response model
                if (model.ScopesConsented != null && model.ScopesConsented.Any())
                {
                    var scopes = model.ScopesConsented;
                    if (ConsentOptionsContents.EnableOfflineAccess == false)
                    {
                        scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess);
                    }

                    grantedConsent = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray(),
                        Description = model.Description
                    };
                }
                else
                {
                    result.ValidationError = ConsentOptionsContents.MustChooseOneErrorMessage;
                }
            }
            else
            {
                result.ValidationError = ConsentOptionsContents.InvalidSelectionErrorMessage;
            }

            if (grantedConsent != null)
            {
                await _interaction.GrantConsentAsync(request, grantedConsent);
                result.RedirectUri = model!.ReturnUrl;
                result.Client = request.Client;
            }

            return result;
        }
    }
}
