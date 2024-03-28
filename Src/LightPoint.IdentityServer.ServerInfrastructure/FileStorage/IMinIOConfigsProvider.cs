using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.FileStorage
{
    public interface IMinIOConfigsProvider
    {
        /// <summary>
        /// 使用的桶
        /// </summary>
        string Bucket { get; set; }

        /// <summary>
        /// 用户
        /// </summary>
        string AccessKey { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        string SecretKey { get; set; }

        /// <summary>
        /// 端点（不需要协议前缀）
        /// </summary>
        string EndPoint { get; set; }
        /// <summary>
        /// 是否使用SSL
        /// </summary>
        bool WithSSL { get; set; }
        /// <summary>
        /// 分享地址过期时间（秒数）
        /// </summary>
        int SharedUrlExpireSecends { get; set; }
    }
}
