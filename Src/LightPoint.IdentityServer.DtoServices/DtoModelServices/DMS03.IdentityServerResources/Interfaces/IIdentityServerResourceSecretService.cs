using LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.DtoServices.DtoModelServices.DMS03.IdentityServerResources.Interfaces
{
    public interface IIdentityServerResourceSecretService
    {
        /// <summary>
        /// 加密的实现，一般用在从视图模型到实体模型的转换
        /// 一般判断是否持久化之后，无需再转换
        /// </summary>
        /// <param name="secretDM"></param>
        /// <returns></returns>
        Task<IdentityServerSecretDM> Encryption(IdentityServerSecretDM secretDM);
        /// <summary>
        /// 解密的实现，一般用于实体模型到视图模型的转换
        /// </summary>
        /// <param name="secretDM"></param>
        /// <returns></returns>
        Task<IdentityServerSecretDM> Decryption(IdentityServerSecretDM secretDM);
    }
}
