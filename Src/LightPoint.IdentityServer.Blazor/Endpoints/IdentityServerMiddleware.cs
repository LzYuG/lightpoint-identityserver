using LightPoint.IdentityServer.ServerInfrastructure.EndpointInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Endpoints
{
    /// <summary>
    /// 对于一些需要使用HttpContext的页面请求
    /// 统一使用该中间件进行处理
    /// </summary>
    public class IdentityServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public IdentityServerMiddleware(RequestDelegate next, ILogger<EndpointMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            await _next(context);
        }
    }
}
