using LightPoint.IdentityServer.ServerInfrastructure.Cache.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using System.Security.Claims;

namespace LightPoint.IdentityServer.ServerInfrastructure.Cache
{
    public class LightPointHttpRequestCacheMiddleware : IMiddleware
    {
        private readonly ILightPointCache _lightPointCache;

        public LightPointHttpRequestCacheMiddleware(ILightPointCache lightPointCache)
        {
            _lightPointCache = lightPointCache;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint != null && !endpoint.Metadata.OfType<LightPointSkipCacheAttribute>().Any() &&
                (endpoint.Metadata.OfType<LightPointHttpGlobalsRequestCacheAttribute>().Any()
                || endpoint.Metadata.OfType<LightPointHttpUserRequestCacheAttribute>().Any()))
            {
                TimeSpan cacheTimeFromNowTo = TimeSpan.FromMinutes(1);

                // 获取所有请求的因素，作为Key
                var resultKey = "";

                if (endpoint.Metadata.OfType<LightPointHttpUserRequestCacheAttribute>().Any())
                {
                    var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
                    resultKey += userId;

                    // first
                    cacheTimeFromNowTo = endpoint.Metadata.OfType<LightPointHttpUserRequestCacheAttribute>().First().CacheTimeFromNowTo;
                }
                else
                {
                    cacheTimeFromNowTo = endpoint.Metadata.OfType<LightPointHttpGlobalsRequestCacheAttribute>().First().CacheTimeFromNowTo;
                }

                var request = context.Request;
                // 请求方法
                var requestMethod = request.Method;
                // 请求地址
                var url = request.GetEncodedPathAndQuery();
                resultKey += requestMethod + url;

                // 允许缓存
                context.Request.EnableBuffering();
                if ((request.Method.ToUpper() == "POST" || request.Method.ToUpper() == "PUT") && request.Headers.ContainsKey("Content-Length"))
                {
                    var requestBodyReader = new StreamReader(context.Request.Body);
                    var requestBodyStr = await requestBodyReader.ReadToEndAsync();
                    // 还原请求体
                    context.Request.Body.Position = 0;
                    resultKey += requestBodyStr;
                }

                // 有缓存记录
                var cacheData = await _lightPointCache.GetItemAsymc<LightPointHttpRequestCacheModel>(resultKey);
                if (cacheData != null)
                {
                    // 自定义返回内容  
                    context.Response.StatusCode = cacheData.StatusCode;
                    context.Response.ContentType = cacheData.ContentType;
                    await context.Response.WriteAsync(cacheData.Content!);
                    return;
                }
                var originalResponseStream = context.Response.Body;
                using var memoryStream = new MemoryStream();
                context.Response.Body = memoryStream;

                await next(context);

                // 仅缓存成功请求
                if (context.Response.StatusCode != 200)
                {
                    return;
                }
                LightPointHttpRequestCacheModel lightPointHttpRequestCacheModel = new LightPointHttpRequestCacheModel()
                {
                    StatusCode = context.Response.StatusCode,
                    ContentType = context.Response.ContentType
                };
                // 响应体
                memoryStream.Position = 0;
                var responseBodyReader = new StreamReader(memoryStream);
                var responseBodyStr = await responseBodyReader.ReadToEndAsync();
                memoryStream.Position = 0;
                // 读取后还原
                await memoryStream.CopyToAsync(originalResponseStream);
                context.Response.Body = originalResponseStream;
                lightPointHttpRequestCacheModel.Content = responseBodyStr;
                await _lightPointCache.SetItemAsymc(resultKey!, lightPointHttpRequestCacheModel, cacheTimeFromNowTo);
            }
            else
            {
                await next(context);
            }
        }

    }
}
