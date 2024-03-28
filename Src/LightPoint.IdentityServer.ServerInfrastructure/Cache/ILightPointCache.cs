using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Cache
{
    /// <summary>
    /// 约束缓存使用的接口
    /// </summary>
    public interface ILightPointCache
    {
        Task<bool> SetItemAsymc<T>(string key, T obj, TimeSpan expire);
        Task<bool> SetItemAsymc<T>(string key, T obj);
        Task<T?> GetItemAsymc<T>(string key);

        Task<bool> SetItemAsymc(string key, string obj, TimeSpan expire);
        Task<bool> SetItemAsymc(string key, string obj);
        Task<string?> GetItemAsymc(string key);

        Task<bool> RemoveItemAsymc(string key);
    }
}
