using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoInfrastructure.Confidential
{
    /// <summary>
    /// 系统中一些常规的字段加密使用的接口
    /// </summary>
    public interface IConfidentialService
    {
        /// <summary>
        /// 双向加密
        /// </summary>
        /// <param name="paintext">明文</param>
        /// <returns></returns>
        Task<string> BidirectionalEncrypt(string paintext);

        /// <summary>
        /// 双向解密
        /// </summary>
        /// <param name="encryptedText">密文</param>
        /// <returns></returns>
        Task<string> BidirectionalDecrypt(string encryptedText);

        /// <summary>
        /// 单向加密
        /// </summary>
        /// <param name="paintext">明文</param>
        /// <returns></returns>
        Task<string> UnbidirectionalEncrypt(string paintext);

        /// <summary>
        /// 单向加密校验
        /// </summary>
        /// <param name="paintext">明文</param>
        /// <param name="encryptedText">密文</param>
        /// <returns></returns>
        Task<bool> UnbidirectionalEncryptValidate(string paintext, string encryptedText);
    }
}
