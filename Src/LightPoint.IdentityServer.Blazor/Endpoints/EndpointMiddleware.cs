using LightPoint.IdentityServer.ServerInfrastructure.EndpointInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Blazor.Endpoints
{
    /// <summary>
    /// 参照IdentityServer4 端点处理的中间件，对OpenIddict这些需要自实现端点的框架使用
    /// </summary>
    public class EndpointMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly List<IdentityServerEndpointInfo> _identityServerEndpointInfos;

        public EndpointMiddleware(RequestDelegate next, ILogger<EndpointMiddleware> logger, List<IdentityServerEndpointInfo> identityServerEndpointInfos)
        {
            _next = next;
            _logger = logger;
            _identityServerEndpointInfos = identityServerEndpointInfos;
        }

        /// <summary>
        /// Invokes the middleware.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                var endpoint = Find(context);
                if (endpoint != null)
                {
                    _logger.LogInformation("Invoking IdentityServer endpoint: {endpointType} for {url}", endpoint.GetType().FullName, context.Request.Path.ToString());

                    var result = await endpoint.ProcessAsync(context);

                    if (result != null)
                    {
                        _logger.LogTrace("Invoking result: {type}", result.GetType().FullName);
                        await result.ExecuteAsync(context);
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unhandled exception: {exception}", ex.Message);
                throw;
            }

            await _next(context);
        }

        public IEndpoint? Find(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            foreach (var endpoint in _identityServerEndpointInfos)
            {
                var path = endpoint.Route;
                if (context.Request.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
                {
                    var endpointName = endpoint.EndpointName;
                    _logger.LogDebug("请求了路径： {path} ，对应的端点类型： {endpoint}", context.Request.Path, endpointName);

                    return GetEndpointHandler(endpoint, context);
                }
            }

            _logger.LogTrace("No endpoint entry found for request path: {path}", context.Request.Path);

            return null;
        }

        private IEndpoint? GetEndpointHandler(IdentityServerEndpointInfo endpoint, HttpContext context)
        {
            // 端点是否开启先不处理
            //if (_options.Endpoints.IsEndpointEnabled(endpoint))
            //{


            //    _logger.LogDebug("Endpoint enabled: {endpoint}, failed to create handler: {endpointHandler}", endpoint.Name, endpoint.Handler.FullName);
            //}
            //else
            //{
            //    _logger.LogWarning("Endpoint disabled: {endpoint}", endpoint.Name);
            //}

            if (context.RequestServices.GetService(endpoint.EndpointType!) is IEndpoint handler)
            {
                return handler;
            }
            return null;
        }
    }
}
