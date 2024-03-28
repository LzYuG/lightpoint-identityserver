using LightPoint.IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources
{
    public class ServerRunningLog : LogBase
    {
        /// <summary>
        /// 详细描述
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// 错误
        /// </summary>
        public string? Errors { get; set; }
        /// <summary>
        /// 日志类型
        /// </summary>
        public ServerRunningLogType ServerRunningLogType { get; set; }

    }
}
