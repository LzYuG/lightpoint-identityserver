using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Pages.IdentityServerBusiness.Accounts.Models
{
    public class ExternalProvider
    {
        public string? DisplayName { get; set; }
        public string? AuthenticationScheme { get; set; }
    }
}
