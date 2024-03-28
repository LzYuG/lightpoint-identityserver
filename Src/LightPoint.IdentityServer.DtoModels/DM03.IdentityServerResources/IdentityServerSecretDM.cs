using LightPoint.IdentityServer.DtoModels.Base;
using LightPoint.IdentityServer.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.DtoModels.DM03.IdentityServerResources
{
    public abstract class IdentityServerSecretDM : DtoBase<int>
    {
        public string? Description { get; set; }
        public string? Value { get; set; } = "";
        public DateTime? Expiration { get; set; }
        public string Type { get; set; } = "SharedSecret";
        public DateTime Created { get; set; } = DateTime.Now;

        /// <summary>
        /// 本值是使用在LightPointIdentityServer上的，ids框架使用的Type应根据该值判断并存储在Type及Value
        /// </summary>
        public string? LightPointSecretType { get; set; } = "SharedSecret";

        /// <summary>
        /// 是否是已经持久化到数据库的密码
        /// </summary>
        public bool IsPersistence { get; set; }
    }
}