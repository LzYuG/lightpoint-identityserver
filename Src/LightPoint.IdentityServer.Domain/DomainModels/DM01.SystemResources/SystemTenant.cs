using LightPoint.IdentityServer.Domain.DomainInfrastructure.RepositoryInterfaces;
using LightPoint.IdentityServer.Domain.DomainModels.DM00.Common;
using LightPoint.IdentityServer.Shared.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightPoint.IdentityServer.Domain.DomainModels.DM01.SystemResources
{
    /// <summary>
    /// 系统的租户
    /// </summary>
    public class SystemTenant : DomainModel<Guid>
    {
        /// <summary>
        /// 有效期至
        /// </summary>
        public DateTime ExpireTime { get; set; }
        /// <summary>
        /// 是否被禁用
        /// </summary>
        public bool Disabled { get; set; }
        /// <summary>
        /// 服务密码，独立的租户申请一些被限制的接口时使用
        /// </summary>
        [Confidential(Type = ConfidentialAttribute.ConfidentialType.单向加密)]
        public string? ServerSecret { get; set; }
        /// <summary>
        /// 是否根租户
        /// </summary>
        public bool IsRoot { get; set; }

        public override async Task<DomainModelValidationResult> DataValidation(RepositoryFactory repositoryFactory)
        {
            var queryRepository = repositoryFactory.GetQueryRepository<Guid, SystemTenant>();
            if (string.IsNullOrWhiteSpace(TenantIdentifier))
            {
                return DomainModelValidationResult.Fail("身份不能为空");
            }
            // 禁止同TenantIdentifier
            if(await queryRepository.HasBoAsync(x=>x.TenantIdentifier == this.TenantIdentifier
                && x.Id != this.Id))
            {
                return DomainModelValidationResult.Fail("已有相同身份的租户");
            }
            return DomainModelValidationResult.Success();
        }
    }
}
