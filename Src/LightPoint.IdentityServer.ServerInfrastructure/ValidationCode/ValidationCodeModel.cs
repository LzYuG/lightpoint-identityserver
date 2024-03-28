using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.ValidationCode
{
    public class ValidationCodeModel
    {
        public string? OperationId { get; set; }
        /// <summary>
        /// 勿暴露给用户
        /// </summary>
        public string? Code { get; set; }
        public DateTime ExpireTime { get; set; }

        public DateTime CreateTime { get; set; }

        public string? Ip { get; set; }
    }
}
