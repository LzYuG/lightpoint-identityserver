using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.Cache.Attributes
{
    /// <summary>
    /// 用于标注需要跳过缓存的Action
    /// </summary>
    public class LightPointSkipCacheAttribute : Attribute
    {
    }
}
