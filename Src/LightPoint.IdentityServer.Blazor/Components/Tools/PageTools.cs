using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace LightPoint.IdentityServer.Blazor.Components.Tools
{
    public static class PageTools
    {
        public static string? GetQueryParam(this NavigationManager navigationManager, string key)
        {
            var uri = navigationManager!.ToAbsoluteUri(navigationManager!.Uri);
            string? paramValue = HttpUtility.ParseQueryString(uri.Query).Get(key);
            return paramValue ?? null;
        }
    }
}
