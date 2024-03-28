using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Layout.Header
{
    public class CacheTagModel
    {
        public string Path { get; set; } = "";

        public string Name { get; set; } = "";

        public bool Popconfirm { get; set; }
    }
}
