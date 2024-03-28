using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models;
using LightPoint.IdentityServer.Blazor.Pages.RazorPages.RazorPageBase;
using LightPoint.IdentityServer.Domain.DomainModels.DM02.ApplicationIdentityResources;
using LightPoint.IdentityServer.DtoModels.DM02.ApplicationIdentityResources;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Interfaces
{
    public interface IIdentityServerAccountPageService
    {
        Task<LoginViewModel> BuildLoginViewModelAsync(LoginInputModel model);

        Task<IActionResult> SignIn(LightPointPageModel page, ApplicationUserDQM validatedUser, LoginInputModel loginInputModel);

        Task<IActionResult> SignOut(LightPointPageModel page, LogoutInputModel model);
    }
}
