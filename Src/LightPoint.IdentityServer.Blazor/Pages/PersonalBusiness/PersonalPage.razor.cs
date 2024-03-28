using LightPoint.IdentityServer.Blazor.Layout.Helpers;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.PersonalBusiness
{
    partial class PersonalPage : GlobalConfigPageBase
    {
        [Inject]
        public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        [Inject]
        public ILayoutInitor? LayoutInitor { get; set; }
        public AuthenticationState? AuthenticationState { get; set; }
        public string WelcomeText { get; set; } = ((DateTime.Now.Hour > 6 ? (DateTime.Now.Hour > 12 ? (DateTime.Now.Hour > 18 ? "晚上好" : "下午好") : "早上好") : "晚上好"));
        [Inject]
        public IApplicationUserService? ApplicationUserService { get; set; }
        public ApplicationUserDCM? ApplicationUserDCM { get; set; }
        [Inject]
        public TenantInfoAccessor? TenantInfoAccessor { get; set; }
        public TenantInfo? TenantInfo { get; set; }

        public string LogoUri { get; set; } = "_content/LightPoint.IdentityServer.Blazor/LightPointLogo.png";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                AuthenticationState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
                WelcomeText += ":" + AuthenticationState.User.Identity?.Name;
                TenantInfo = TenantInfoAccessor!.TenantInfo;
                await this.LoadConfigsAsync();
                await this.RefreshApplicationUser();
                StateHasChanged();
            }
        }

        private async Task RefreshApplicationUser()
        {
            ApplicationUserDCM = Mapper<ApplicationUserDQM, ApplicationUserDCM>.MapToNewObj((await ApplicationUserService!.GetApiBoAsync(x => x.Id == Guid.Parse(AuthenticationState!.User.FindFirst("sub")!.Value), true))!);
            if (ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications == null)
            {
                ApplicationUserDCM!.ApplicationUserMultiFactorAuthentications = new List<ApplicationUserMultiFactorAuthenticationDM>();
            }
            StateHasChanged();
        }

        private async Task GoLogout()
        {
            await LayoutInitor!.Logout();
        }
    }
}