using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential
{
    public static class ConfidentialConstants
    {
        /// <summary>
        /// 加密之后，字符串前置值，用于辨识字段是否被加密过
        /// </summary>
        public static string EncryptedPreString = "2024_LIGHTPOINT_CONFIDENTIAL_ENCRYPTED_";
    }
}
