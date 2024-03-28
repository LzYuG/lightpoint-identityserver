using AntDesign;
using LightPoint.IdentityServer.Blazor.Components.Tools;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts
{
    partial class LoggedOut : GlobalConfigPageBase
    {
        [Inject]
        public NavigationManager? NavigationManager { get; set; }
        [Inject]
        public ToolsJsInterop? ToolsJsInterop { get; set; }
        /// <summary>
        /// 登出后回调地址
        /// </summary>
        public string? RedirectUri { get; set; } = "";
        /// <summary>
        /// 外部授权平台需要注销的地址
        /// </summary>
        public string? SignOutIframeUrl { get; set; } = "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                RedirectUri = HttpUtility.UrlDecode(NavigationManager!.GetQueryParam("PostLogoutRedirectUri"));
                SignOutIframeUrl = HttpUtility.UrlDecode(NavigationManager!.GetQueryParam("SignOutIframeUrl"));
                await this.LoadServerCommonConfigAsync();
                StateHasChanged();
                if (this.CurrentServerCommonConfig!.AutoRedirectWhenLogouted)
                {
                    await ToolsJsInterop!.Redirect(RedirectUri!);
                }
            }
        }
    }
}
