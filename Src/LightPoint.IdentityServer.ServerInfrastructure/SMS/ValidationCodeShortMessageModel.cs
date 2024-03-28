using LightPoint.IdentityServer.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.ServerInfrastructure.SMS
{
    public class ValidationCodeShortMessageModel
    {
        /// <summary>
        /// 发送给
        /// </summary>
        public string? To { get; set; }

        public string? ValidationCode { get; set; }

        public string? RemoteIP { get; set; }
    }
}
