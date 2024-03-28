using LightPoint.IdentityServer.Blazor.Layout.Header;
using LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS01.SystemResources.Interfaces;
using LightPoint.IdentityServer.ServerInfrastructure.Middlewares.MutilTenant;
using Microsoft.AspNetCore.Components.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Layout.Helpers
{
    public class LayoutInitor : ILayoutInitor
    {
        private readonly ToolsJsInterop _toolsJsInterop;
        private readonly NavigationManager _navigationManager;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly TenantInfoAccessor _tenantInfoAccessor;
        private readonly ISystemTenantService _systemTenantService;
        private readonly IApplicationMenuStore _applicationMenuStore;

        public LayoutInitor(ToolsJsInterop toolsJsInterop, NavigationManager navigationManager, AuthenticationStateProvider authenticationStateProvider,
            TenantInfoAccessor tenantInfoAccessor, ISystemTenantService systemTenantService, IApplicationMenuStore applicationMenuStore)
        {
            _toolsJsInterop = toolsJsInterop;
            _navigationManager = navigationManager;
            _authenticationStateProvider = authenticationStateProvider;
            _tenantInfoAccessor = tenantInfoAccessor;
            _systemTenantService = systemTenantService;
            _applicationMenuStore = applicationMenuStore;
        }

        public async Task<List<ApplicationMenu>> GetMenus()
        {
            var tenant = await _systemTenantService.GetApiBoAsync(x => x.TenantIdentifier == _tenantInfoAccessor.TenantIdentifier && x.ExpireTime > DateTime.Now);
            if(tenant == null)
            {
                // 实际是空的主页，用于触发登录
                return new List<ApplicationMenu>()
                {
                    new ApplicationMenu()
                    {
                        Id = Guid.NewGuid().ToString(), Name = "主页", Icon = "home", Path = "/",
                    },
                };
            }
            var menus = await _applicationMenuStore.GetMenus();
            var userState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            if(userState != null && userState.User != null && userState.User.Claims.Any())
            {
                var resMenu = new List<ApplicationMenu>();
                foreach(var topMenu in menus)
                {
                    if (string.IsNullOrWhiteSpace(topMenu.AllowRoles) || topMenu.AllowRoles.Split(",").Any(x=> userState.User.IsInRole(x)))
                    {
                        resMenu.Add(topMenu);
                    }
                }
                if (!tenant.IsRoot)
                {
                    resMenu = resMenu.Where(x => !x.OnlyRootTenant).ToList();
                }
                return resMenu;
            }
            else
            {
                // 实际是空的主页，用于触发登录
                return new List<ApplicationMenu>()
                {
                    new ApplicationMenu()
                    {
                        Id = Guid.NewGuid().ToString(), Name = "主页", Icon = "home", Path = "/",
                    },
                };
            }
        }

        public async Task<AvatarModel> GetUserInfo()
        {
            var userState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            return new AvatarModel() { UserName = userState.User.FindFirstValue("name"), UserId = userState.User.FindFirstValue("sub") };
        }

        public async Task GoProfile(AvatarModel model)
        {
            await Task.Run(() =>
            {
                _navigationManager.NavigateTo("/Personal");
            });
        }

        public async Task Logout()
        {
            await _toolsJsInterop.Redirect("/Account/Logout");
        }
    }
}
