using IdentityServer4;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Device.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;
using LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.PageServices.Models;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.Shared.DataProcessingFacades;
using LightPoint.IdentityServer.Shared.Helpers;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace LightPoint.IdentityServer.QuickStart.Server.IdentityServer4.LightPointIdentityServer.ServiceImps.PageServices
{
    public class IdentityServerDevicePageService : IIdentityServerDevicePageService
    {
        private readonly IAppService<Guid, IdentityServerDeviceFlowCode, IdentityServerDeviceFlowCodeDM, IdentityServerDeviceFlowCodeDM> _identityServerDeviceFlowCodeService;
        private readonly IDeviceFlowInteractionService _interaction;

        public IdentityServerDevicePageService(IAppService<Guid, IdentityServerDeviceFlowCode, IdentityServerDeviceFlowCodeDM, IdentityServerDeviceFlowCodeDM> identityServerDeviceFlowCodeService,
            IDeviceFlowInteractionService interaction)
        {
            _identityServerDeviceFlowCodeService = identityServerDeviceFlowCodeService;
            _interaction = interaction;
        }

        public async Task<DeviceAuthorizationModel?> BuildDeviceAuthorizationModel(string userCode, DeviceAuthorizationSubmitModel? model = null)
        {
            var request = await _interaction.GetAuthorizationContextAsync(userCode);
            if (request != null)
            {
                return CreateConsentViewModel(userCode, model, request);
            }

            return null;
        }

        private DeviceAuthorizationModel CreateConsentViewModel(string userCode, DeviceAuthorizationSubmitModel? model, DeviceFlowAuthorizationRequest request)
        {
            var vm = new DeviceAuthorizationModel
            {
                UserCode = userCode,
                Description = model?.Description,

                RememberConsent = model?.RememberConsent ?? true,
                ScopesConsented = model?.ScopesConsented ?? Enumerable.Empty<string>(),

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

        public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
        {
            return new ScopeViewModel
            {
                Value = parsedScopeValue.RawValue,
                // todo: use the parsed scope value in the display?
                DisplayName = apiScope.DisplayName ?? apiScope.Name,
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

        public async Task<IdentityServerDeviceFlowCodeDM?> GetDeviceFlowCode(string userCode, string tenantIdentifiler)
        {
            // Ids4用的utc时间
            var res = await _identityServerDeviceFlowCodeService.GetApiBoAsync(x => x.UserCode == userCode.Sha256() && x.TenantIdentifier == tenantIdentifiler && x.Expiration > DateTime.UtcNow, true);
            return res;
        }

        public async Task<IActionResult> SubmitDeviceAuthorization(LightPointPageModel page, DeviceAuthorizationSubmitModel deviceAuthorizationSubmitModel)
        {
            if (deviceAuthorizationSubmitModel == null) throw new ArgumentNullException(nameof(deviceAuthorizationSubmitModel));

            var result = await ProcessConsent(deviceAuthorizationSubmitModel);
            if (result.HasValidationError) return page.LPRedirect("/Device/Error");

            return page.LPRedirect("/Device/Success");
        }

        private async Task<ProcessConsentResult> ProcessConsent(DeviceAuthorizationSubmitModel model)
        {
            var result = new ProcessConsentResult();

            var request = await _interaction.GetAuthorizationContextAsync(model.UserCode);
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
                await _interaction.HandleRequestAsync(model.UserCode, grantedConsent);
                result.RedirectUri = model.ReturnUrl;
                result.Client = request.Client;
            }

            return result;
        }
    }
}
