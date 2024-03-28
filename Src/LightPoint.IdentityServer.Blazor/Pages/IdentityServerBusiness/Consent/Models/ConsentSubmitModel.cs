using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Consent.Models
{
    public class ConsentSubmitModel
    {
        public string? UserCode { get; set; }
        public IEnumerable<string>? ScopesConsented { get; set; }
        public bool RememberConsent { get; set; } = true;
        public string? ReturnUrl { get; set; }
        public string? Description { get; set; }

        public bool IsAllow { get; set; }
    }
}
