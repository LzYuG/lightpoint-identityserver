
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

namespace LightPoint.IdentityServer.ServerInfrastructure.Cache.Attributes
{
    /// <summary>
    /// 单个用户的请求缓存标注，即单个用户独享缓存
    /// 禁止为文件上传等Body Content较大的接口设置该标注
    /// </summary>
    public class LightPointHttpUserRequestCacheAttribute : Attribute
    {
        public LightPointHttpUserRequestCacheAttribute()
        {

        }

        public LightPointHttpUserRequestCacheAttribute(TimeSpan cacheTimeFromNowTo)
        {
            CacheTimeFromNowTo = cacheTimeFromNowTo;
        }

        /// <summary>
        /// 缓存时间
        /// </summary>
        public TimeSpan CacheTimeFromNowTo { get; set; }
    }
}
