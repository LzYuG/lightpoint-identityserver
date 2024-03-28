using LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Device.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Consent.Models
{
    public class ConsentModel
    {
        public string? UserCode { get; set; }
        public bool ConfirmUserCode { get; set; }
        public bool AllowRememberConsent { get; set; }

        #region 上次记住的数据
        public IEnumerable<string>? ScopesConsented { get; set; }

        public bool RememberConsent { get; set; }

        public string? Description { get; set; }
        #endregion

        public IEnumerable<ScopeViewModel>? IdentityScopes { get; set; }
        public IEnumerable<ScopeViewModel>? ApiScopes { get; set; }
    }
}
