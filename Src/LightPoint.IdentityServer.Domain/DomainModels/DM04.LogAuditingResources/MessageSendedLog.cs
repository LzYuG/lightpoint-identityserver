using LightPoint.IdentityServer.Shared.BusinessEnums.BE04.LogAuditingResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM04.LogAuditingResources
{
    public class MessageSendedLog : LogBase
    {
        /// <summary>
        /// 消息主体的描述，一般能表明含义即可
        /// </summary>
        public string? MessageContentDescription { get; set; }
        /// <summary>
        /// 关键值
        /// </summary>
        public string? KeyValue1 { get; set; }
        /// <summary>
        /// 关键值
        /// </summary>
        public string? KeyValue2 { get; set; }
        /// <summary>
        /// 关键值
        /// </summary>
        public string? KeyValue3 { get; set; }
        /// <summary>
        /// 发送给
        /// </summary>
        public string? To { get; set; }
        /// <summary>
        /// 操作人身份Id
        /// </summary>
        public string? OperationIdentityId { get; set; }
        /// <summary>
        /// 操作人名称
        /// </summary>
        public string? OperationIdentityName { get; set; }
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageSendedLogType MessageSendedLogType { get; set; }

    }
}
