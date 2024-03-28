using LightPoint.IdentityServer.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM03.IdentityServerResources
{
    public abstract class IdentityServerSecret : DomainModelBase<long>
    {
        public string? Description { get; set; }
        public string? Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; } = "SharedSecret";
        public DateTime Created { get; set; } = DateTime.Now;
        /// <summary>
        /// 本值是使用在LightPointIdentityServer上的，ids框架使用的Type应根据该值判断并存储在Type及Value
        /// </summary>
        public string? LightPointSecretType { get; set; } = "SharedSecret";
    }
}