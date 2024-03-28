using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Store.Models
{
    public class ApplicationMenu
    {
        public string? Id { get; set; }
        public string? Name { get; set; }

        public string? Path { get; set; }

        public string? Icon { get; set; }

        public bool OnlyRoot { get; set; }

        public bool OnlyRootTenant { get; set; }

        public bool IsLink { get; set; }

        public string? AllowRoles { get; set; }

        public List<ApplicationMenu> Childrens { get; set; } = new List<ApplicationMenu>();
    }
}
