using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Layout.Header
{
    public class BreadcrumbItemModel
    {
        public string? Name { get; set; }

        public string? Id { get; set; }

        public string? Path { get; set; }
    }
}
