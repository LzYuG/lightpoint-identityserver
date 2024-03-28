using LightPoint.IdentityServer.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Shared
{
    public class IdentityServerBusinessTemplatePageParams
    {
        public string? BackgroundImgUri { get; set; } = PageConstants.DefaultLoginImage;
        public string? ClientLogoUri { get; set; } = PageConstants.DefaultLogo;
        public string? ClientName { get; set; } = PageConstants.MainApplicationName;
        public string? Title { get; set; } = PageConstants.MainApplicationName;
        public string? SubTitle { get; set; }
        public string? Template { get; set; } = "";

    }
}
