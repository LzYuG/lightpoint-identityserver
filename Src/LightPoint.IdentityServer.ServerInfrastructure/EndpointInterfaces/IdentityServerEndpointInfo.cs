using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.EndpointInterfaces
{
    public class IdentityServerEndpointInfo
    {
        public IdentityServerEndpointInfo(string? route, string? endpointName, Type? endpointType)
        {
            Route = route;
            EndpointName = endpointName;
            EndpointType = endpointType;
        }

        public string? Route { get; set; }

        public string? EndpointName { get; set; }

        public Type? EndpointType { get; set; }
    }
}
