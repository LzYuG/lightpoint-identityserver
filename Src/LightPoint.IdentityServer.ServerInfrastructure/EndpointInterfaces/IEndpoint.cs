using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.EndpointInterfaces
{
    public interface IEndpoint
    {
        Task<IEndpointResponseModel> ProcessAsync(HttpContext httpContext);
    }
}
