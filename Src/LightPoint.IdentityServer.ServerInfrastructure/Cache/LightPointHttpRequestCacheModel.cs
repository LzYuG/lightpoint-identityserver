using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Cache
{
    public class LightPointHttpRequestCacheModel
    {
        public int StatusCode { get; set; }

        public string? ContentType { get; set; }

        public string? Content { get; set; }
    }
}
