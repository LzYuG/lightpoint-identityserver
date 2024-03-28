using AntDesign;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Consent.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Shared;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Consent
{
    partial class Consent
    {
        #region Injects
        [Inject]
        public IJSRuntime? JSRuntime { get; set; }
        [Inject]
        public TenantInfoAccessor? TenantInfoAccessor { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }
        [Inject]
        public IApplicationUserService? ApplicationUserService { get; set; }
        [Inject]
        public IIdentityServerClientService? IdentityServerClientService { get; set; }
        [Inject]
        public IMessageService? MessageService { get; set; }
        [Inject]
        public IIdentityServerConsentPageService? IdentityServerConsentPageService { get; set; }
        [Inject]
        public ToolsJsInterop? ToolsJsInterop { get; set; }
        #endregion


        public string? UserCode { get; set; }
        public string? NewUserCode { get; set; }

        #region TemplateParams
        public IdentityServerBusinessTemplatePageParams IdentityServerBusinessTemplatePageParams { get; set; } = new IdentityServerBusinessTemplatePageParams();
        #endregion

        #region PageData

        private string? TabActiveKey = "0";

        private string? ClientId = "";

        public string? ReturnUrl { get; set; }

        [Parameter]
        public string? AntiForgeryToken { get; set; }

        // 默认模板
        public string? Template { get; set; } = "";

        private IdentityServerClientDQM? IdentityServerClient { get; set; }
        [Parameter]
        public ConsentModel? ConsentModel { get; set; } = new ConsentModel();

        private ConsentSubmitModel? ConsentSubmitModel { get; set; }
        #endregion

        #region Events

        private async Task Allow()
        {
            ConsentSubmitModel!.IsAllow = true;
            ConsentSubmitModel.UserCode = UserCode;
            List<string> scopesConsented = new List<string>();
            if (ConsentModel?.ApiScopes != null)
            {
                scopesConsented.AddRange(ConsentModel.ApiScopes.Where(x => x.Checked).Select(x => x.Value!));
            }
            if (ConsentModel?.IdentityScopes != null)
            {
                scopesConsented.AddRange(ConsentModel.IdentityScopes.Where(x => x.Checked).Select(x => x.Value!));
            }
            ConsentSubmitModel.ScopesConsented = scopesConsented;
            ConsentSubmitModel.ReturnUrl = ReturnUrl;

            await ToolsJsInterop!.PostForm("/Consent", ConsentSubmitModel, AntiForgeryToken);
        }

        #endregion


        protected override async Task OnInitializedAsync()
        {
            ReturnUrl = NavigationManager!.GetQueryParam("returnUrl");

            if (!string.IsNullOrWhiteSpace(ReturnUrl) && ReturnUrl.StartsWith("/connect/authorize/callback?"))
            {
                var callbackQuery = ReturnUrl.Split("?")[1];
                var clientiParams = callbackQuery.Split("&").FirstOrDefault(x => x.StartsWith("client_id="));
                if (clientiParams != null && clientiParams.Split("=").Length == 2)
                {
                    ClientId = clientiParams.Split("=")[1];
                }

                IdentityServerClient = await IdentityServerClientService!.GetApiBoAsync(x => x.ClientId == ClientId && x.TenantIdentifier == TenantInfoAccessor!.TenantInfo!.TenantIdentifier, true);

                IdentityServerBusinessTemplatePageParams!.SubTitle = "正在进行授权";

                if (IdentityServerClient != null)
                {
                    if(ConsentSubmitModel == null)
                    {
                        ConsentSubmitModel = new ConsentSubmitModel();
                    }

                    if (!string.IsNullOrWhiteSpace(IdentityServerClient.LoginPageBackgroundImgUri))
                    {
                        IdentityServerBusinessTemplatePageParams.BackgroundImgUri = IdentityServerClient.LoginPageBackgroundImgUri;
                    }
                    if (!string.IsNullOrWhiteSpace(IdentityServerClient.LogoUri))
                    {
                        IdentityServerBusinessTemplatePageParams.ClientLogoUri = IdentityServerClient.LogoUri;
                    }
                    if (!string.IsNullOrWhiteSpace(IdentityServerClient.PageTemplate))
                    {
                        IdentityServerBusinessTemplatePageParams.Template = IdentityServerClient.PageTemplate;
                    }
                    if (!string.IsNullOrWhiteSpace(IdentityServerClient.Name))
                    {
                        IdentityServerBusinessTemplatePageParams.ClientName = IdentityServerClient.Name;
                        IdentityServerBusinessTemplatePageParams.SubTitle = "正在进行授权：" + IdentityServerClient.Name;
                    }
                }

            }

            await LoadConfigsAsync();            

            StateHasChanged();
        }
    }
}
