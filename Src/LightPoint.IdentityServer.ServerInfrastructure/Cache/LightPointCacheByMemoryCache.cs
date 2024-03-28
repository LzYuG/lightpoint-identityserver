using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Cache
{
    /// <summary>
    /// 内存缓存的实现类
    /// </summary>
    public class LightPointCacheByMemoryCache : ILightPointCache
    {
        private readonly IMemoryCache _memoryCache;

        public LightPointCacheByMemoryCache(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T?> GetItemAsymc<T>(string key)
        {
            return await Task.Run(() =>
            {
                return _memoryCache.Get<T>(key);
            });
        }

        public async Task<string?> GetItemAsymc(string key)
        {
            return await Task.Run(() =>
            {
                return _memoryCache.Get(key)?.ToString();
            });
        }

        public async Task<bool> SetItemAsymc<T>(string key, T obj, TimeSpan expire)
        {
            return await Task.Run(() =>
            {
                var res = _memoryCache.Set(key, obj, expire);
                return res != null;
            });
        }

        public async Task<bool> SetItemAsymc<T>(string key, T obj)
        {
            return await Task.Run(() =>
            {
                var res = _memoryCache.Set(key, obj);
                return res != null;
            });
        }

        public async Task<bool> SetItemAsymc(string key, string obj, TimeSpan expire)
        {
            return await Task.Run(() =>
            {
                var res = _memoryCache.Set(key, obj, expire);
                return res != null;
            });
        }

        public async Task<bool> SetItemAsymc(string key, string obj)
        {
            return await Task.Run(() =>
            {
                var res = _memoryCache.Set(key, obj);
                return res != null;
            });
        }


        public async Task<bool> RemoveItemAsymc(string key)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _memoryCache.Remove(key);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            });
        }
    }
}
