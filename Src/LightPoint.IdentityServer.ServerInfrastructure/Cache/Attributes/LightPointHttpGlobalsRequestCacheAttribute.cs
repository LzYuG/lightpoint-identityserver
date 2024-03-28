using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Cache.Attributes
{
    /// <summary>
    /// 全局的请求缓存标注，即不同用户访问同一个缓存
    /// 禁止为文件上传等Body Content较大的接口设置该标注
    /// </summary>
    public class LightPointHttpGlobalsRequestCacheAttribute : Attribute
    {
        public LightPointHttpGlobalsRequestCacheAttribute()
        {

        }
        public LightPointHttpGlobalsRequestCacheAttribute(TimeSpan cacheTimeFromNowTo)
        {
            CacheTimeFromNowTo = cacheTimeFromNowTo;
        }

        /// <summary>
        /// 缓存时间
        /// </summary>
        public TimeSpan CacheTimeFromNowTo { get; set; } = TimeSpan.FromMinutes(1);
    }
}
