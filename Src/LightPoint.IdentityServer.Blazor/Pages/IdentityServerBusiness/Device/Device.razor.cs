using AntDesign;
using AntDesign.Core.Extensions;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Device.Models;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces;
using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Shared;
using LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Client;
using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources.Operations;
using LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Base;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Device
{
    partial class Device
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
        public IAppService<Guid, IdentityServerDeviceFlowCode, IdentityServerDeviceFlowCodeDM, IdentityServerDeviceFlowCodeDM>? IdentityServerDeviceFlowCodeService { get; set; }
        [Inject]
        public IIdentityServerDevicePageService? IdentityServerDevicePageService { get; set; }
        [Inject]
        public ToolsJsInterop? ToolsJsInterop { get; set; }
        #endregion


        public string? UserCode { get; set; }
        public string? NewUserCode { get; set; }

        #region TemplateParams
        public IdentityServerBusinessTemplatePageParams IdentityServerBusinessTemplatePageParams { get; set; } = new IdentityServerBusinessTemplatePageParams();
        #endregion

        #region PageData
        private RegisterModel RegisterModel { get; set; } = new RegisterModel();

        private string? TabActiveKey = "0";

        private string? ClientId = "";

        [Parameter]
        public string? AntiForgeryToken { get; set; }

        // 默认模板
        public string? Template { get; set; } = "";

        private IdentityServerClientDQM? IdentityServerClient { get; set; }

        private IdentityServerDeviceFlowCodeDM? IdentityServerDeviceFlowCode { get; set; }

        private DeviceAuthorizationModel? DeviceAuthorizationModel { get; set; } = new DeviceAuthorizationModel();

        private DeviceAuthorizationSubmitModel? DeviceAuthorizationSubmitModel { get; set; }
        #endregion

        #region Events

        private async Task Allow()
        {
            DeviceAuthorizationSubmitModel!.IsAllow = true;
            DeviceAuthorizationSubmitModel.UserCode = UserCode;
            List<string> scopesConsented = new List<string>();
            if(DeviceAuthorizationModel?.ApiScopes != null)
            {
                scopesConsented.AddRange(DeviceAuthorizationModel.ApiScopes.Where(x => x.Checked).Select(x => x.Value!));
            }
            if (DeviceAuthorizationModel?.IdentityScopes != null)
            {
                scopesConsented.AddRange(DeviceAuthorizationModel.IdentityScopes.Where(x => x.Checked).Select(x => x.Value!));
            }
            DeviceAuthorizationSubmitModel.ScopesConsented = scopesConsented;

            await ToolsJsInterop!.PostForm("/Device", DeviceAuthorizationSubmitModel, AntiForgeryToken);
        }

        private void NotAllow()
        {
            DeviceAuthorizationSubmitModel!.IsAllow = false;
            NavigationManager!.NavigateTo("/device", true);
        }


        private async Task RefreshDeviceCode()
        {
            if (!string.IsNullOrWhiteSpace(NewUserCode))
            {
                IdentityServerDeviceFlowCode = await IdentityServerDevicePageService!.GetDeviceFlowCode(NewUserCode, TenantInfoAccessor!.TenantInfo!.TenantIdentifier!);
                if (IdentityServerDeviceFlowCode == null)
                {
                    await MessageService!.Error("用户码已失效!");
                    UserCode = "";
                }
                else
                {
                    UserCode = NewUserCode;
                    DeviceAuthorizationModel = await IdentityServerDevicePageService!.BuildDeviceAuthorizationModel(UserCode!, DeviceAuthorizationSubmitModel);
                    if(DeviceAuthorizationSubmitModel == null)
                    {
                        DeviceAuthorizationSubmitModel = new DeviceAuthorizationSubmitModel();
                    }
                    ClientId = IdentityServerDeviceFlowCode!.ClientId;
                }
            }

            IdentityServerClient = await IdentityServerClientService!.GetApiBoAsync(x => x.ClientId == ClientId && x.TenantIdentifier == TenantInfoAccessor!.TenantInfo!.TenantIdentifier, true);

            IdentityServerBusinessTemplatePageParams!.SubTitle = "正在进行设备流授权";

            if (IdentityServerClient != null)
            {
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
                    IdentityServerBusinessTemplatePageParams.SubTitle = "正在进行设备流授权：" + IdentityServerClient.Name;
                }
            }

            StateHasChanged();
        }

        #endregion


        protected override async Task OnInitializedAsync()
        {
            await LoadConfigsAsync();
            NewUserCode = NavigationManager!.GetQueryParam("userCode");

            await RefreshDeviceCode();
        }
    }
}
