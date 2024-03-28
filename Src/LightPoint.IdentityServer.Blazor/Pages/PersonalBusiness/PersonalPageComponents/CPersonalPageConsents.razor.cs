using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.Tools.Mappers;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS02.ApplicationResources.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.PersonalBusiness.PersonalPageComponents
{
    partial class CPersonalPageConsents
    {
        [Inject]
        public AuthenticationStateProvider? AuthenticationStateProvider { get; set; }
        [Inject]
        public IApplicationUserService? ApplicationUserService { get; set; }
        public AuthenticationState? AuthenticationState { get; set; }
        public ApplicationUserDCM? ApplicationUserDCM { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                AuthenticationState = await AuthenticationStateProvider!.GetAuthenticationStateAsync();
                await LoadUser();
                StateHasChanged();
            }
        }

        private async Task LoadUser()
        {
            ApplicationUserDCM = Mapper<ApplicationUserDQM, ApplicationUserDCM>.MapToNewObj((await ApplicationUserService!.GetApiBoAsync(x => x.Id == Guid.Parse(AuthenticationState!.User.FindFirst("sub")!.Value)))!);
        }
    }
}
