using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources
{
    public abstract class LogBase : DomainModelBase<Guid>
    {
        /// <summary>
        /// 申请信息发送的客户端IP
        /// </summary>
        public string? RemoteIP { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}
